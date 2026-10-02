using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P2Checker
{
    /// <summary>
    /// DC and RC model of the three TFE input options, as confirmed on the bench.
    ///
    /// The checker board drives through R273 = 1 kohm into C27 = 1 uF, and each input option
    /// presents its own input impedance and its own attenuation between the RC node and the
    /// window comparator. The 10 kohm option divides by two through R55/R66; the 50 ohm 10 V
    /// option divides by two through R62/R63; the 50 ohm 3.3 V option does not divide at all.
    /// That last difference is not legible from the schematic text and was established by
    /// measurement.
    ///
    /// Probe readings against this model, with the DIO pin diverted to the node by K7:
    ///
    ///   option        drive   predicted   measured   residual
    ///   50 ohm 3.3 V   3.3 V    157.1 mV    156 mV    -1.1 mV
    ///   50 ohm 10 V    5.0 V    119.0 mV    118 mV    -1.0 mV
    ///   10 kohm 10 V   5.0 V   2272.7 mV   2270 mV    -2.7 mV
    ///
    /// The residuals are a roughly constant -1 mV rather than a constant percentage, so they
    /// are PPMU offset and not a gain error: the resistor ratios are good to a few tenths of
    /// a percent. In particular there is no few percent drive droop, which rules amplitude
    /// error out as the cause of a long 10/90 width.
    /// </summary>
    public static class PoolInputOptionModel
    {
        /// <summary>Checker board series resistor R273, in ohms.</summary>
        public const double SourceOhms = 1000.0;

        /// <summary>Checker board capacitor C27, in farads.</summary>
        public const double CapacitanceFarads = 1e-6;

        /// <summary>
        /// Drive level used for calibration and for measurement. Both must use the same level
        /// because the full scale scales with it, so it lives here rather than in either step.
        /// </summary>
        public const double NominalDriveVolts = 5.0;

        private const double FiftyOhmInputOhms = 50.0;
        private const double TenKOhmInputOhms = 10000.0;

        /// <summary>TFE input impedance presented by an option, in ohms.</summary>
        /// <param name="option">Input option.</param>
        public static double InputOhms(PoolInputOption option)
        {
            return option == PoolInputOption.TenKOhm10V ? TenKOhmInputOhms : FiftyOhmInputOhms;
        }

        /// <summary>
        /// Attenuation between the RC node and the comparator input. Unity for the 50 ohm
        /// 3.3 V option, one half for both 10 V options.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static double PostDividerGain(PoolInputOption option)
        {
            return option == PoolInputOption.FiftyOhm3V3 ? 1.0 : 0.5;
        }

        /// <summary>
        /// Highest level the option's relay input is rated for. The 50 ohm 3.3 V option is
        /// rated at the relay input, which sees the full drive level, not the attenuated node.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static double MaxDriveVolts(PoolInputOption option)
        {
            return option == PoolInputOption.FiftyOhm3V3 ? 3.3 : 10.0;
        }

        /// <summary>Drive level to use for an option, clamped to its rating.</summary>
        /// <param name="option">Input option.</param>
        public static double DriveVoltsFor(PoolInputOption option)
        {
            return Math.Min(NominalDriveVolts, MaxDriveVolts(option));
        }

        /// <summary>Gain from the drive pin to the RC node, set by the input divider.</summary>
        /// <param name="option">Input option.</param>
        public static double NodeGain(PoolInputOption option)
        {
            double inputOhms = InputOhms(option);
            return inputOhms / (SourceOhms + inputOhms);
        }

        /// <summary>Gain from the drive pin all the way to the comparator input.</summary>
        /// <param name="option">Input option.</param>
        public static double FullScaleGain(PoolInputOption option)
        {
            return NodeGain(option) * PostDividerGain(option);
        }

        /// <summary>
        /// Loaded RC time constant, in seconds. 909.09 us for the 10 kohm option and 47.62 us
        /// for both 50 ohm options, which load the 1 kohm source twenty times harder.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static double TauSec(PoolInputOption option)
        {
            double inputOhms = InputOhms(option);
            return SourceOhms * inputOhms / (SourceOhms + inputOhms) * CapacitanceFarads;
        }

        /// <summary>
        /// Comparator full scale predicted from the option's gain and its clamped drive level.
        /// This is the fallback when the PPMU reading is unusable, and the reference the
        /// reading is sanity checked against.
        /// </summary>
        /// <param name="option">Input option.</param>
        public static double PredictedFullScaleVolts(PoolInputOption option)
        {
            return FullScaleGain(option) * DriveVoltsFor(option);
        }
    }

    /// <summary>
    /// Carries the measured comparator full scale from the Calibration sequence to the rise
    /// time steps in MainSequence, using TSM per site data.
    ///
    /// Per site data is scoped to the semiconductor module context, so Calibration and
    /// MainSequence must run in the same execution for a stored value to be visible. Calling
    /// Calibration as a sequence before MainSequence satisfies that; launching it as its own
    /// execution does not, and the rise time step then silently falls back to the predicted
    /// level. TryLoad reports which happened rather than hiding it.
    /// </summary>
    public static class PoolCalibrationStore
    {
        /// <summary>
        /// Fractions of the predicted level that bound an acceptable PPMU reading. A reading
        /// outside this window means the measurement did not reach the node, most likely
        /// because the changeover failed to divert and the pin is still on the ECL XOR output
        /// at about -1.0 V or -1.7 V. That is a real relay fault worth surfacing.
        /// </summary>
        public const double AcceptanceLowFraction = 0.5;

        /// <summary>Upper bound of the acceptance window, as a fraction of the prediction.</summary>
        public const double AcceptanceHighFraction = 1.5;

        /// <summary>
        /// Site data key for one slot, channel and input option combination.
        /// </summary>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        public static string Key(PoolSlot slot, int channel, PoolInputOption option)
        {
            return "POOL_FS_" + SlotName(slot) + "_CH" + channel + "_" + option;
        }

        /// <summary>Human readable slot name, SL12, SL14 or SL21.</summary>
        /// <param name="slot">Slot under test.</param>
        public static string SlotName(PoolSlot slot)
        {
            switch (slot)
            {
                case PoolSlot.SL12:
                    return "SL12";
                case PoolSlot.SL14:
                    return "SL14";
                default:
                    return "SL21";
            }
        }

        /// <summary>True when a reading is close enough to the prediction to be believable.</summary>
        /// <param name="measured">PPMU reading in volts.</param>
        /// <param name="option">Input option the reading was taken on.</param>
        public static bool IsPlausible(double measured, PoolInputOption option)
        {
            if (double.IsNaN(measured) || double.IsInfinity(measured))
            {
                return false;
            }

            double predicted = PoolInputOptionModel.PredictedFullScaleVolts(option);
            return measured >= AcceptanceLowFraction * predicted
                && measured <= AcceptanceHighFraction * predicted;
        }

        /// <summary>Stores per site full scale levels for one combination.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        /// <param name="fullScaleVolts">Per site full scale levels in volts.</param>
        public static void Save(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option,
            double[] fullScaleVolts)
        {
            tsmContext.SetSiteData(Key(slot, channel, option), fullScaleVolts);
        }

        /// <summary>
        /// Reads back per site full scale levels, or falls back to the predicted level on
        /// every site when Calibration did not run in this execution.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        /// <param name="fullScaleVolts">Per site full scale levels in volts.</param>
        /// <returns>True when the values came from a stored measurement.</returns>
        public static bool TryLoad(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option,
            out double[] fullScaleVolts)
        {
            string key = Key(slot, channel, option);
            if (tsmContext.SiteDataExists(key))
            {
                fullScaleVolts = tsmContext.GetSiteData<double>(key);
                return true;
            }

            int siteCount = tsmContext.SiteNumbers.Count;
            fullScaleVolts = new double[siteCount];
            double predicted = PoolInputOptionModel.PredictedFullScaleVolts(option);
            for (int site = 0; site < siteCount; site++)
            {
                fullScaleVolts[site] = predicted;
            }

            return false;
        }

        /// <summary>
        /// Single threshold SMU level to use for one combination.
        ///
        /// The threshold SMUs are shared across sites, so one level has to serve all of them
        /// and the lowest site's full scale is taken. That keeps both thresholds inside every
        /// site's ramp, which biases a higher site's width slightly short rather than losing it
        /// to a NaN. Falls back to the predicted level when nothing usable is stored.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        /// <param name="calibrated">True when the level came from a stored measurement.</param>
        public static double SmuLevelFor(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option,
            out bool calibrated)
        {
            double[] perSite;
            calibrated = TryLoad(tsmContext, slot, channel, option, out perSite);

            double smallest = double.NaN;
            foreach (double value in perSite)
            {
                if (double.IsNaN(value))
                {
                    continue;
                }

                if (double.IsNaN(smallest) || value < smallest)
                {
                    smallest = value;
                }
            }

            return double.IsNaN(smallest)
                ? PoolInputOptionModel.PredictedFullScaleVolts(option)
                : smallest;
        }
    }

    /// <summary>
    /// Calibration sequence step. Measures the comparator full scale for every POOL slot,
    /// channel and input option and stores it for the rise time steps in MainSequence.
    ///
    /// Each combination is measured by energising the channel's input changeover so the
    /// divided node is diverted to the 6571 DIO pin, driving the slot's HSD200 pin to a DC
    /// level, letting the RC settle, and reading the node with the PPMU while forcing 0 A.
    /// Because the changeover is a single pole with the comparator on one throw and the DIO
    /// pin on the other, that reading is the level the comparator input sees, not a proxy
    /// for it.
    ///
    /// This replaces the VOH bisection the rise time steps used to run. The bisection found
    /// the same level indirectly, using "did the window close on every site" as a one bit
    /// comparator, at a cost of eight bursts per channel. Beyond being about six times
    /// slower it had two defects this does not: its resolution was around 10 mV against the
    /// PPMU's 1 mV, far too coarse for the 50 ohm options whose full scale is only about
    /// 120 mV; and because one SMU level serves every site it converged on the lowest site,
    /// so every other site then read long at 10/90. A PPMU read is per site.
    /// </summary>
    public class POOL_Calibration
    {
        private const int ChannelCount = 8;

        private static readonly PoolSlot[] AllSlots =
        {
            PoolSlot.SL12, PoolSlot.SL14, PoolSlot.SL21
        };

        private static readonly PoolInputOption[] AllOptions =
        {
            PoolInputOption.FiftyOhm3V3, PoolInputOption.FiftyOhm10V, PoolInputOption.TenKOhm10V
        };

        // Five tau at the 10 kohm option's 909 us, rounded up, so the node is settled to
        // better than 1 % before the aperture opens. Generous for the 50 ohm options, which
        // settle twenty times faster.
        private const double SettlingTimeSec = 10e-3;

        // 2 ms averages out mains hum and ECL supply noise. Calibration runs once, so the
        // time costs nothing.
        private const double PpmuApertureTimeSec = 2e-3;

        // Forcing 0 A is what makes the read high impedance. The node sits behind the 15K
        // divider at about 7.5 kohm, so a forced voltage would pull the level down.
        private const double PpmuForceCurrentAmps = 0.0;
        private const double PpmuForceCurrentRangeAmps = 2e-6;

        // The 6571 pin electronics span -2 V to 6 V. Limits are set to the full span so a
        // surprise reading is reported rather than silently clamped.
        private const double PpmuVoltageLimitHighVolts = 6.0;
        private const double PpmuVoltageLimitLowVolts = -2.0;

        /// <summary>
        /// Measures and stores the comparator full scale for the requested combinations.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot to calibrate, 12, 14, 21, or 0 for all three.</param>
        /// <param name="channel">POOL channel 1 to 8, or 0 for all eight.</param>
        /// <param name="inputOption">
        /// Input option 0 for 50 ohm 3.3 V, 1 for 50 ohm 10 V, 2 for 10 kohm 10 V, or -1 for
        /// all three.
        /// </param>
        /// <param name="publishResults">
        /// Publishes each measured full scale as a per site result. This requires a
        /// Semiconductor Multi Test step with one Tests tab entry per published ID, so set it
        /// false to run the step as an Action instead, storing site data and logging only.
        /// </param>
        public static void PoolFullScaleCalibration(
            ISemiconductorModuleContext tsmContext,
            int slot = 0,
            int channel = 0,
            int inputOption = 2,
            bool publishResults = true)
        {
            foreach (PoolSlot poolSlot in SelectedSlots(slot))
            {
                foreach (PoolInputOption option in SelectedOptions(inputOption))
                {
                    foreach (int ch in SelectedChannels(channel))
                    {
                        CalibrateOne(tsmContext, poolSlot, ch, option, publishResults);
                    }
                }
            }
        }

        /// <summary>
        /// Measures one slot, channel and input option combination and stores the result.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        /// <param name="publishResults">Publishes the measured full scale as a per site result.</param>
        private static void CalibrateOne(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option,
            bool publishResults)
        {
            string prefix = "POOL_CAL_" + PoolCalibrationStore.SlotName(slot)
                + "_CH" + channel + "_" + option;
            double driveVolts = PoolInputOptionModel.DriveVoltsFor(option);

            HMODControl.AllHMODReset(tsmContext);

            Digital drive = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Drive[(int)slot]);
            Digital sense = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Dio[channel - 1]);

            try
            {
                // One Apply for the whole path. HMOD writes overwrite the entire chain, so
                // splitting these would reopen whatever went out first. Changeover is closed
                // here and only here; the rise time steps must leave it open.
                var state = new PoolHmodState();
                state.Close(
                    PoolPathMap.SlotSelect(slot, channel),
                    PoolPathMap.InputOption(option, channel),
                    PoolPathMap.Changeover(channel),
                    PoolPathMap.DigitalTap(channel));
                state.CloseChecker(
                    PoolPathMap.CheckerDrive(slot),
                    PoolPathMap.CheckerFanOut(slot, channel));
                state.Apply(tsmContext);

                sense.Abort();
                sense.SelectFunction(SelectedFunction.Ppmu);
                sense.PPMUConfigureApertureTime(PpmuApertureTimeSec);
                sense.PPMUConfigureVoltageLimits(
                    voltageLimitHigh: PpmuVoltageLimitHighVolts,
                    voltageLimitLow: PpmuVoltageLimitLowVolts);
                sense.PPMUForceCurrent(
                    currentLevel: PpmuForceCurrentAmps,
                    currentLevelRange: PpmuForceCurrentRangeAmps);
                sense.PPMUSource();

                drive.Abort();
                drive.SelectFunction(SelectedFunction.Digital);
                drive.ConfigureVoltgeLevels(vil: 0, vih: driveVolts, vol: 0, voh: 0, vterm: 0);
                drive.WriteStatic(PinState._1);

                Globals.TheHdw.Wait(SettlingTimeSec);

                // One array per instrument; a single DIO pin lives on a single 6571, so index
                // 0 holds the per site readings.
                double[] measured = sense.PPMUMeasure(PpmuMeasurementType.Voltage)[0];
                double[] accepted = Accept(measured, option, prefix);

                PoolCalibrationStore.Save(tsmContext, slot, channel, option, accepted);

                if (publishResults)
                {
                    tsmContext.PublishPerSite(accepted, prefix + "_FullScale");
                }
            }
            finally
            {
                drive.WriteStatic(PinState._0);
                drive.Abort();
                sense.SelectFunction(SelectedFunction.Off);
                sense.Abort();
                HMODControl.AllHMODReset(tsmContext);
            }
        }

        /// <summary>
        /// Replaces any implausible per site reading with the predicted level, logging the
        /// substitution. A substituted site keeps the step running but records that the path
        /// did not measure, which is the signature of a changeover that failed to divert.
        /// </summary>
        /// <param name="measured">Per site PPMU readings in volts.</param>
        /// <param name="option">Input option the readings were taken on.</param>
        /// <param name="prefix">Result name prefix, used for the log line.</param>
        private static double[] Accept(double[] measured, PoolInputOption option, string prefix)
        {
            double predicted = PoolInputOptionModel.PredictedFullScaleVolts(option);
            var accepted = new double[measured.Length];

            for (int site = 0; site < measured.Length; site++)
            {
                if (PoolCalibrationStore.IsPlausible(measured[site], option))
                {
                    accepted[site] = measured[site];
                    continue;
                }

                accepted[site] = predicted;

                // TestSteps.Common.Debug is a checker class, so qualify the framework one.
                System.Diagnostics.Debug.WriteLine(
                    prefix + " site " + site + " read " + measured[site].ToString("F4")
                    + " V, outside " + (PoolCalibrationStore.AcceptanceLowFraction * predicted)
                        .ToString("F4")
                    + " to " + (PoolCalibrationStore.AcceptanceHighFraction * predicted)
                        .ToString("F4")
                    + " V; substituting the predicted " + predicted.ToString("F4") + " V");
            }

            return accepted;
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
