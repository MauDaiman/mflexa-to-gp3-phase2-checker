using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;
using static TestSteps.Common.PoolRelay;

namespace TestSteps.P2Checker
{
    /// <summary>
    /// POOL PL OUT rise time check for slot SL12, channels 1 to 8.
    ///
    /// Signal path for channel n:
    ///   T_HSD200_SL11_CH5 -> CHMOD6 K16 -> AD8244BRMZ buffer -> 1 kohm -> 1 uF
    ///     -> CHMOD6 fan out relay -> T_POOL_SL12_OUT_CH_n
    ///     -> HMOD slot select -> TFE 10 kohm input option
    ///     -> AD96687BRZ window comparator -> MC100EL07DR2G XOR
    ///     -> output changeover -> 6571 DIO(23+n) or SCOPE 5172 CH(n-1)
    ///
    /// The XOR is high only while the ramp is between VOL and VOH, so its pulse width is
    /// the rise time between those two thresholds. Moving VOL/VOH gives the 10/90, 20/80
    /// and 30/70 rise times from the same ramp. See PoolRcModel for the RC derivation.
    ///
    /// SL12 differs from SL14 and SL21 only in the SlotSelect column, the CHMOD6 drive
    /// relay and the CHMOD6 fan out column below.
    /// </summary>
    public class SL12_POOL_Check
    {
        private const int ChannelCount = 8;

        // 10 ms is about 11 tau, leaving under 0.002% residual charge on the capacitor.
        private const double DischargeTimeSec = 10e-3;

        // Relay and SMU settling before each burst, matching the BBAC checkers.
        private const double SettlingTimeSec = 20e-3;

        private const double SmuCurrentLimit = 10e-3;
        private const double SmuVoltageLevelRange = 6.0;
        private const double SmuApertureTimeSec = 10e-3;

        // Translator board relay tables, indexed by channel - 1. The board allocates nine
        // consecutive relays per channel packed across HMOD11, HMOD12 and HMOD13 with no
        // gaps, so channels 4 and 8 straddle a register boundary. Written out literally
        // rather than computed so a failing channel's relays can be read directly.

        /// <summary>Connects the channel's TFE input to T_POOL_SL12_OUT_CH_n.</summary>
        private static readonly PoolRelay[] SlotSelect =
        {
            Hmod11(1), Hmod11(10), Hmod11(19), Hmod11(28),
            Hmod12(5), Hmod12(14), Hmod12(23), Hmod12(32)
        };

        /// <summary>
        /// Selects the 10 kohm / 10 V max TFE input option. This is the only option usable
        /// for rise time; both 50 ohm options collapse the RC to about 47.6 us.
        /// </summary>
        private static readonly PoolRelay[] Input10kOhm =
        {
            Hmod11(6), Hmod11(15), Hmod11(24), Hmod12(1),
            Hmod12(10), Hmod12(19), Hmod12(28), Hmod13(5)
        };

        /// <summary>De-energised rests on NC to the 6571; energised routes to the 5172.</summary>
        private static readonly PoolRelay[] OutputChangeover =
        {
            Hmod11(7), Hmod11(16), Hmod11(25), Hmod12(2),
            Hmod12(11), Hmod12(20), Hmod12(29), Hmod13(6)
        };

        /// <summary>Taps the XOR output to SCOPE 5172 CH(n-1).</summary>
        private static readonly PoolRelay[] ScopeTap =
        {
            Hmod11(8), Hmod11(17), Hmod11(26), Hmod12(3),
            Hmod12(12), Hmod12(21), Hmod12(30), Hmod13(7)
        };

        /// <summary>Taps the XOR output to 6571 DIO(23+n).</summary>
        private static readonly PoolRelay[] DigitalTap =
        {
            Hmod11(9), Hmod11(18), Hmod11(27), Hmod12(4),
            Hmod12(13), Hmod12(22), Hmod12(31), Hmod13(8)
        };

        // HMOD13 K9/K10/K11 ground the CH12..CH17 LO sense line of the P163, P151 and P138
        // 4163 modules. Each module's CH18..CH23 LO sense is hard wired to AGND and needs
        // no relay, which is why most channels below need nothing. Leaving a required
        // ground open floats the SMU sense line, the failure mode behind the BBAC
        // DgsRef_AccSrcRef drift.
        private static readonly PoolRelay[][] LoSenseGrounds =
        {
            new[] { Hmod13(9) },      // CH1 VOL = P163 CH17
            new PoolRelay[0],         // CH2 CH19/CH20, both hard grounded
            new PoolRelay[0],         // CH3 CH21/CH22, both hard grounded
            new[] { Hmod13(10) },     // CH4 VOL = P151 CH16
            new[] { Hmod13(10) },     // CH5 VOL = P151 CH17
            new PoolRelay[0],         // CH6 CH19/CH20, both hard grounded
            new PoolRelay[0],         // CH7 CH21/CH22, both hard grounded
            new[] { Hmod13(11) }      // CH8 VOH = P138 CH16
        };

        // Window comparator threshold SMUs. This allocation follows no derivable rule and
        // matches the arrays in P1Checker/Window_Comparator_Check.cs.
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

        // Checker board drawing 02-101092, CHMOD6. K16 connects T_HSD200_SL11_CH5 to the
        // SL12 buffer; the fan out relays connect the SL12 RC node to each POOL output.
        // Channel 1 interleaves with the other slots' input relays at K17/K19/K21, so SL12
        // takes K17 while channels 2 to 8 are contiguous from K22 in steps of 3.
        private const int CheckerDriveRelay = 16;

        private static readonly int[] CheckerFanOutRelays = { 17, 22, 25, 28, 31, 34, 37, 40 };

        /// <summary>
        /// Measures the rise time of every SL12 POOL channel, or of one channel when
        /// <paramref name="channel"/> is given. This is the TestStand entry point: one step
        /// covers the whole slot, matching how SL04_DIFFMETER and SL10_DC30 are driven.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="captureType">Capture back end, 0 for 6571 digital or 1 for 5172 scope.</param>
        /// <param name="channel">POOL channel 1 to 8, or 0 to sweep all eight.</param>
        public static void SL12PoolRiseTimeCheck(
            ISemiconductorModuleContext tsmContext,
            int captureType = 0,
            int channel = 0)
        {
            if (channel < 0 || channel > ChannelCount)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(channel), channel, "Channel must be 0 for all channels, or 1..8.");
            }

            if (channel != 0)
            {
                MeasureChannel(tsmContext, channel, captureType);
                return;
            }

            for (int sweep = 1; sweep <= ChannelCount; sweep++)
            {
                MeasureChannel(tsmContext, sweep, captureType);
            }
        }

        /// <summary>
        /// Measures one SL12 POOL channel at the 10/90, 20/80 and 30/70 threshold pairs.
        /// Publishes three widths in seconds plus two width ratios that are independent of tau
        /// and of every component tolerance.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="captureType">Capture back end, 0 for 6571 digital or 1 for 5172 scope.</param>
        private static void MeasureChannel(
            ISemiconductorModuleContext tsmContext,
            int channel,
            int captureType)
        {
            int index = channel - 1;
            string prefix = "SL12_POOL_CH" + channel;

            HMODControl.AllHMODReset(tsmContext);

            IPoolCaptureStrategy capture = PoolCaptureFactory.Create((PoolCaptureType)captureType);

            // The shared timing sheet runs at 100 ns, so the digital strategy's 1 us default
            // would scale every reported width by ten. Only the digital strategy exposes the
            // override; the 5172 path keeps its own sample rate.
            var digital = capture as PoolDigital6571Strategy;
            if (digital != null)
            {
                digital.ConfigureRecord(
                    PoolRecordGeometry.SamplePeriodSec,
                    PoolRecordGeometry.SamplesToFetch(PoolInputOption.TenKOhm10V));
            }
            DCPower smuVoh = InstrCtrl.DCPowerPinsToSessions(tsmContext, VohPins[index]);
            DCPower smuVol = InstrCtrl.DCPowerPinsToSessions(tsmContext, VolPins[index]);

            try
            {
                // Every relay for this path goes out in one Apply. HMOD writes overwrite the
                // whole chain, so splitting these across calls would reopen earlier closures.
                var state = new PoolHmodState();
                state.Close(SlotSelect[index], Input10kOhm[index]);
                state.Close(LoSenseGrounds[index]);
                capture.SelectOutputPath(state, OutputChangeover[index], ScopeTap[index], DigitalTap[index]);
                state.CloseChecker(CheckerDriveRelay, CheckerFanOutRelays[index]);
                // TestSteps.Common.Debug is a checker class, so qualify the framework one.
                System.Diagnostics.Debug.WriteLine(prefix + " relays: " + state);
                state.Apply(tsmContext);

                capture.Configure(tsmContext, channel);
                ConfigureThresholdSmu(smuVoh);
                ConfigureThresholdSmu(smuVol);

                // Thresholds follow the full scale the Calibration sequence measured for this
                // channel, not the nominal 2.2727 V and no longer a VOH bisection here.
                bool calibrated;
                double fullScale = PoolCalibrationStore.SmuLevelFor(
                    tsmContext, PoolSlot.SL12, channel, PoolInputOption.TenKOhm10V,
                    out calibrated);
                System.Diagnostics.Debug.WriteLine(
                    prefix + " comparator full scale: " + fullScale.ToString("F4")
                    + " V, nominal " + PoolRcModel.ComparatorFullScaleVolts.ToString("F4")
                    + " V, source " + (calibrated ? "calibration" : "prediction"));

                var widths = new double[PoolRcModel.Pairs.Length][];
                for (int pair = 0; pair < PoolRcModel.Pairs.Length; pair++)
                {
                    PoolRcModel.ThresholdPair thresholds = PoolRcModel.Pairs[pair];

                    smuVol.ForceVoltage(
                        voltageLevel: thresholds.LowVoltsFor(fullScale), currentLimit: SmuCurrentLimit);
                    smuVoh.ForceVoltage(
                        voltageLevel: thresholds.HighVoltsFor(fullScale), currentLimit: SmuCurrentLimit);

                    Globals.TheHdw.Wait(SettlingTimeSec);

                    widths[pair] = capture.StepAndMeasure(tsmContext, DischargeTimeSec);
                    tsmContext.PublishPerSite(widths[pair], prefix + "_RiseTime_" + thresholds.Name);
                }

                tsmContext.PublishPerSite(Ratio(widths[0], widths[1]), prefix + "_Ratio_90_10_over_80_20");
                tsmContext.PublishPerSite(Ratio(widths[1], widths[2]), prefix + "_Ratio_80_20_over_70_30");
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

        /// <summary>
        /// Per site quotient of two width arrays. NaN propagates, and a zero or negative
        /// denominator yields NaN rather than an infinity.
        /// </summary>
        private static double[] Ratio(double[] numerator, double[] denominator)
        {
            var result = new double[numerator.Length];
            for (int site = 0; site < numerator.Length; site++)
            {
                result[site] = site < denominator.Length && denominator[site] > 0
                    ? numerator[site] / denominator[site]
                    : double.NaN;
            }

            return result;
        }
    }
}
