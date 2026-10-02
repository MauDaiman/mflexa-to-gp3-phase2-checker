using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;

namespace TestSteps.P2Checker
{
    /// <summary>
    /// Capture record geometry for one input option.
    ///
    /// A single 100 ns timing sheet serves every option. The period and the record depth are
    /// separate knobs and only the depth needs to vary: at 100 ns the 50 ohm options resolve
    /// their 40 us narrowest pair to 0.25%, while the 10 kohm option simply needs ten times
    /// the samples to contain its much longer ramp. Two timing sheets would mean a per option
    /// branch in the capture setup and a second sheet to keep in sync, which is the kind of
    /// silent mismatch this step has already been bitten by twice.
    /// </summary>
    public static class PoolRecordGeometry
    {
        /// <summary>Vector period of the shared timing sheet, in seconds.</summary>
        public const double SamplePeriodSec = 100e-9;

        /// <summary>
        /// Samples to fetch for an option, sized so the 90% crossing lands inside the record
        /// with wide margin. A crossing that falls off the end of the record is
        /// indistinguishable from a missing edge, so an out of tolerance part must report a
        /// width that fails its limit rather than a NaN.
        ///
        /// The earlier 26000 was sized from the nominal 2.303 tau crossing at 20933 samples
        /// plus allowance for a capacitor 24% high. Raw captures of all eight SL12 channels
        /// disproved that margin: the measured 90% crossings land at 2484 to 2599 us, that is
        /// 24843 to 25993 samples, so CH6 cleared the 26000 sample window by 0.7 us, CH1 by
        /// 7.3 us and CH8 by 10.6 us. Those three are the channels that returned an
        /// intermittent NaN on the 10/90 pair, and the cause is the window edge, not a missing
        /// edge or a dead channel.
        ///
        /// The crossings run late because the 10/90 high threshold at 0.9 full scale sits only
        /// 60 to 200 mV below the asymptote the node actually reaches, so the signal creeps
        /// across it on the flattest part of the ramp. Fixing that is a separate question
        /// about the drive level; this depth change only stops the record from truncating a
        /// crossing that does occur.
        ///
        /// 49000 covers 5.4 tau for the 10 kohm option and needs the pattern repeat count to
        /// be at least 49000. The 50 ohm options keep 3000, already 6.3 tau of their 47.6 us
        /// time constant, so their crossing near 1090 samples has ample room.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static int SamplesToFetch(PoolInputOption option)
        {
            return option == PoolInputOption.TenKOhm10V ? 49000 : 3000;
        }

        /// <summary>
        /// Time to hold the node low before stepping. Ten time constants leaves under 0.005%
        /// residual charge, so the ramp starts from a known zero rather than from wherever the
        /// previous pair left the capacitor.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static double DischargeTimeSec(PoolInputOption option)
        {
            return 10.0 * PoolInputOptionModel.TauSec(option);
        }
    }

    /// <summary>
    /// POOL PL OUT rise time across all three TFE input options.
    ///
    /// For each slot, channel and option this measures the comparator window crossing time at
    /// the 10/90, 20/80 and 30/70 threshold pairs and publishes the three widths plus the two
    /// width ratios. Thresholds are derived from the full scale the Calibration sequence
    /// measured for that exact combination, so no VOH bisection runs here.
    ///
    /// The expected widths differ twenty fold between options, because the 50 ohm options load
    /// the 1 kohm source to 47.6 us against the 10 kohm option's 909 us:
    ///
    ///   option        tau        10/90      20/80     30/70
    ///   50 ohm both   47.62 us   104.6 us    66.0 us   40.3 us
    ///   10 kohm       909.09 us  1997.5 us  1260.3 us  770.3 us
    ///
    /// The two ratios are 1.5850 and 1.6361 for every option, since they cancel tau and
    /// amplitude alike. That makes them the tolerance immune part of the check and the better
    /// diagnostic: a wrong capacitor scales all three widths and leaves the ratios correct,
    /// whereas a fragmented pulse breaks the ratios. If a width reads long while the ratios
    /// hold, suspect amplitude; if the ratios break, suspect the capture.
    ///
    /// The input changeover is left open throughout, which is what routes the divided node to
    /// the window comparator. Only the Calibration step closes it.
    /// </summary>
    public class POOL_RiseTime_Options
    {
        private const int ChannelCount = 8;

        // Relay and SMU settling before each burst, matching the BBAC checkers.
        private const double SettlingTimeSec = 20e-3;

        // The only threshold pair measured. 10-90 and 20-80 are retired because the thresholds
        // sit too close to the asymptote: a 1% amplitude error moves the 10-90 width by 5.79%
        // against 30-70's 2.55%, so 10-90 failed 40 of 40 results while 30-70 passed 40 of 40
        // from the same captures. 30-70 also sits in the steepest part of the ramp, where a
        // voltage error converts into the smallest time error. See Reports/RC_Charge_Times.xlsx.
        //
        // Retiring the other two pairs also retires both published ratios, since each needs two
        // pairs. That removes the only tau independent shape monitor from this step; shape is
        // now covered only by POOL_VtSweep, which is a characterisation step and not run in
        // production.
        private const string MeasuredPairName = "30_70";

        private const double SmuCurrentLimit = 10e-3;
        private const double SmuVoltageLevelRange = 6.0;
        private const double SmuApertureTimeSec = 10e-3;

        private static readonly PoolSlot[] AllSlots =
        {
            PoolSlot.SL12, PoolSlot.SL14, PoolSlot.SL21
        };

        private static readonly PoolInputOption[] AllOptions =
        {
            PoolInputOption.FiftyOhm3V3, PoolInputOption.FiftyOhm10V, PoolInputOption.TenKOhm10V
        };

        // Window comparator threshold SMUs, indexed by channel - 1. This allocation follows no
        // derivable rule and matches the arrays in P1Checker/Window_Comparator_Check.cs. It is
        // identical in all three of SL12_POOL_Check, SL14_POOL_Check and SL21_POOL_Check,
        // because the threshold SMUs sit on the TFE side of the slot select relays and so are
        // shared by every slot. Only the checker board relays differ per slot, and PoolPathMap
        // derives those.
        private static readonly string[] VohPins =
        {
            "P163_4163_SMU_CH18", "P163_4163_SMU_CH20", "P163_4163_SMU_CH22", "P163_4163_SMU_CH23",
            "P151_4163_SMU_CH18", "P151_4163_SMU_CH20", "P151_4163_SMU_CH22", "P138_4163_SMU_CH16"
        };

        private static readonly string[] VolPins =
        {
            "P163_4163_SMU_CH17", "P163_4163_SMU_CH19", "P163_4163_SMU_CH21", "P151_4163_SMU_CH16",
            "P151_4163_SMU_CH17", "P151_4163_SMU_CH19", "P151_4163_SMU_CH21", "P151_4163_SMU_CH23"
        };

        // HMOD13 K9/K10/K11 ground the CH12..CH17 LO sense line of the P163, P151 and P138
        // 4163 modules. Each module's CH18..CH23 LO sense is hard wired to AGND and needs no
        // relay, which is why most channels need nothing. Leaving a required ground open floats
        // the SMU sense line, the failure mode behind the BBAC DgsRef_AccSrcRef drift.
        private static readonly PoolRelay[][] LoSenseGrounds =
        {
            new[] { PoolRelay.Hmod13(9) },   // CH1 VOL = P163 CH17
            new PoolRelay[0],                // CH2 CH19/CH20, both hard grounded
            new PoolRelay[0],                // CH3 CH21/CH22, both hard grounded
            new[] { PoolRelay.Hmod13(10) },  // CH4 VOL = P151 CH16
            new[] { PoolRelay.Hmod13(10) },  // CH5 VOL = P151 CH17
            new PoolRelay[0],                // CH6 CH19/CH20, both hard grounded
            new PoolRelay[0],                // CH7 CH21/CH22, both hard grounded
            new[] { PoolRelay.Hmod13(11) }   // CH8 VOH = P138 CH16
        };

        /// <summary>
        /// Measures rise time for the requested slot, channel and input option combinations.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test, 12, 14, 21, or 0 for all three.</param>
        /// <param name="channel">POOL channel 1 to 8, or 0 for all eight.</param>
        /// <param name="inputOption">
        /// Input option 0 for 50 ohm 3.3 V, 1 for 50 ohm 10 V, 2 for 10 kohm 10 V, or -1 for
        /// all three.
        /// </param>
        public static void PoolRiseTimeByOption(
            ISemiconductorModuleContext tsmContext,
            int slot = 0,
            int channel = 0,
            int inputOption = 2)
        {
            foreach (PoolSlot poolSlot in SelectedSlots(slot))
            {
                foreach (PoolInputOption option in SelectedOptions(inputOption))
                {
                    foreach (int ch in SelectedChannels(channel))
                    {
                        MeasureChannel(tsmContext, poolSlot, ch, option);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the one threshold pair this step measures.
        ///
        /// Looked up by name rather than by index so that reordering PoolRcModel.Pairs, which is
        /// shared with the retired SL slot steps, cannot silently change which pair is measured.
        /// </summary>
        private static PoolRcModel.ThresholdPair MeasuredPair()
        {
            foreach (PoolRcModel.ThresholdPair pair in PoolRcModel.Pairs)
            {
                if (pair.Name == MeasuredPairName)
                {
                    return pair;
                }
            }

            throw new InvalidOperationException(
                "PoolRcModel.Pairs contains no pair named " + MeasuredPairName
                + ", so the only measured pair is missing from the model.");
        }

        /// <summary>
        /// Measures one slot, channel and option at the 30-70 threshold pair.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        private static void MeasureChannel(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option)
        {
            int index = channel - 1;
            string prefix = "POOL_" + PoolCalibrationStore.SlotName(slot)
                + "_CH" + channel + "_" + option;

            HMODControl.AllHMODReset(tsmContext);

            // Constructed directly rather than through PoolCaptureFactory because the record
            // geometry override is specific to the digital strategy. The 5172 path is not
            // verified for this step.
            var capture = new PoolDigital6571Strategy();
            DCPower smuVoh = InstrCtrl.DCPowerPinsToSessions(tsmContext, VohPins[index]);
            DCPower smuVol = InstrCtrl.DCPowerPinsToSessions(tsmContext, VolPins[index]);

            bool calibrated;
            double level = PoolCalibrationStore.SmuLevelFor(
                tsmContext, slot, channel, option, out calibrated);

            if (!calibrated)
            {
                // TestSteps.Common.Debug is a checker class, so qualify the framework one.
                System.Diagnostics.Debug.WriteLine(
                    prefix + " has no stored calibration; using the predicted full scale "
                    + PoolInputOptionModel.PredictedFullScaleVolts(option).ToString("F4")
                    + " V. Calibration must run in the same execution as MainSequence.");
            }

            try
            {
                // One Apply for the whole path. HMOD writes overwrite the entire chain, so
                // splitting these would reopen whatever went out first. The input changeover is
                // deliberately absent: open is what feeds the window comparator.
                var state = new PoolHmodState();
                state.Close(
                    PoolPathMap.SlotSelect(slot, channel),
                    PoolPathMap.InputOption(option, channel),
                    PoolPathMap.DigitalTap(channel));
                state.Close(LoSenseGrounds[index]);
                state.CloseChecker(
                    PoolPathMap.CheckerDrive(slot),
                    PoolPathMap.CheckerFanOut(slot, channel));
                System.Diagnostics.Debug.WriteLine(prefix + " relays: " + state);
                state.Apply(tsmContext);

                capture.ConfigureRecord(
                    PoolRecordGeometry.SamplePeriodSec,
                    PoolRecordGeometry.SamplesToFetch(option));
                capture.Configure(tsmContext, channel);

                ConfigureThresholdSmu(smuVoh);
                ConfigureThresholdSmu(smuVol);

                double dischargeTimeSec = PoolRecordGeometry.DischargeTimeSec(option);
                PoolRcModel.ThresholdPair thresholds = MeasuredPair();

                smuVol.ForceVoltage(
                    voltageLevel: thresholds.LowVoltsFor(level), currentLimit: SmuCurrentLimit);
                smuVoh.ForceVoltage(
                    voltageLevel: thresholds.HighVoltsFor(level), currentLimit: SmuCurrentLimit);

                Globals.TheHdw.Wait(SettlingTimeSec);

                double[] widths = capture.StepAndMeasure(tsmContext, dischargeTimeSec);
                tsmContext.PublishPerSite(widths, prefix + "_RiseTime_" + thresholds.Name);
            }
            finally
            {
                capture.Cleanup(tsmContext);
                ShutDownThresholdSmu(smuVoh);
                ShutDownThresholdSmu(smuVol);
                HMODControl.AllHMODReset(tsmContext);
            }
        }

        private static void ConfigureThresholdSmu(DCPower smu)
        {
            smu.Abort();
            smu.ConfigureSettings(
                apertureTime: SmuApertureTimeSec,
                apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            smu.ConfigureVoltageLevelRange(voltageLevelRange: SmuVoltageLevelRange);
            smu.ConfigureCurrentLimitRange(currentLimitRange: SmuCurrentLimit);
            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);
        }

        private static void ShutDownThresholdSmu(DCPower smu)
        {
            if (smu == null)
            {
                return;
            }

            smu.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);
            smu.ConfigureOutputEnabled(false);
            smu.ConfigureOutputConnected(false);
            smu.Abort();
        }

        /// <summary>Slots to visit, all three when the parameter is 0.</summary>
        /// <param name="slot">Slot number, 12, 14, 21, or 0 for all.</param>
        private static IEnumerable<PoolSlot> SelectedSlots(int slot)
        {
            if (slot == 0)
            {
                return AllSlots;
            }

            switch (slot)
            {
                case 12:
                    return new[] { PoolSlot.SL12 };
                case 14:
                    return new[] { PoolSlot.SL14 };
                case 21:
                    return new[] { PoolSlot.SL21 };
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(slot), slot, "Slot must be 0 for all, or 12, 14 or 21.");
            }
        }

        /// <summary>Input options to visit, all three when the parameter is -1.</summary>
        /// <param name="inputOption">Option index 0 to 2, or -1 for all.</param>
        private static IEnumerable<PoolInputOption> SelectedOptions(int inputOption)
        {
            if (inputOption == -1)
            {
                return AllOptions;
            }

            if (inputOption < 0 || inputOption >= AllOptions.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(inputOption), inputOption, "Input option must be -1 for all, or 0..2.");
            }

            return new[] { AllOptions[inputOption] };
        }

        /// <summary>Channels to visit, all eight when the parameter is 0.</summary>
        /// <param name="channel">Channel 1 to 8, or 0 for all.</param>
        private static IEnumerable<int> SelectedChannels(int channel)
        {
            if (channel == 0)
            {
                var all = new int[ChannelCount];
                for (int index = 0; index < ChannelCount; index++)
                {
                    all[index] = index + 1;
                }

                return all;
            }

            if (channel < 1 || channel > ChannelCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(channel), channel, "Channel must be 0 for all, or 1..8.");
            }

            return new[] { channel };
        }
    }
}
