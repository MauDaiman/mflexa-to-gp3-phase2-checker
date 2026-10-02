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
    /// Samples the RC node voltage against time, so the ramp's asymptote, time constant and
    /// shape are measured rather than inferred.
    ///
    /// Why this step exists. The rise time steps report three window widths per channel, and
    /// those three numbers cannot distinguish the two candidate defects. Fitting the measured
    /// SL12 widths gives an identical 2.31% residual for a reduced asymptote of 0.947 full
    /// scale and for a constant threshold offset of +121 mV, both wanting tau near 832 us
    /// against the modelled 909 us. The degeneracy is structural: near this operating point
    /// the two causes produce the same signature, so no number of parts separates them. A
    /// direct V(t) curve does.
    ///
    /// The threshold offset half of that pair has since been ruled out on the bench. The SMU
    /// forces the window edges at exactly 0.2274 V and 2.0466 V, which are 10% and 90% of the
    /// 2.274 V that calibration stored for the channel, and reads them back to eight decimal
    /// places, so the calibrate-to-force chain is sound. It draws 4.3 and 4.5 uA, which raised
    /// the possibility of an I times R error between the force point and the comparator, but
    /// that current is sourced rather than sunk, so the drop has the wrong sign to lengthen a
    /// width, and the channels are remote sensed, so it is regulated out in any case. That
    /// leaves the asymptote, which is what this step measures.
    ///
    /// The method is the calibration step's measurement repeated at a series of delays. The
    /// changeover diverts the divided node to the DIO pin, the drive is stepped, and the PPMU
    /// reads the node while forcing 0 A. Each point discharges first, so every point starts
    /// from the same zero rather than from wherever the previous one left the capacitor.
    ///
    /// What it can and cannot settle. The drive here is WriteStatic, the same as calibration,
    /// so this measures the static path's curve. It yields the true tau and confirms the
    /// static asymptote, but it cannot show whether a pattern burst reaches the same level,
    /// because the rise time step's step is driven from the pattern rather than from
    /// WriteStatic. That question is answered by inspecting the drive pins' state on the
    /// repeated capture vector in the Digital Pattern Editor, not from here.
    ///
    /// Two systematic effects bias every point later in time by the same amount: the aperture
    /// integrates forward from the delay rather than straddling it, and there is driver latency
    /// between the wait returning and the acquisition starting. Both are common to all points,
    /// so an offline fit should carry a free time offset alongside the asymptote and tau. The
    /// asymptote itself is unaffected, because the tail is flat.
    ///
    /// The last point at 13.2 tau is settled to within 2 ppm, so it is published as the
    /// asymptote rather than as an estimate of one. Note that this duplicates what calibration
    /// already measures: both drive with WriteStatic and read the same node, so this step's
    /// asymptote is a cross check on that number, not new information. The new information is
    /// tau and the shape.
    /// </summary>
    public class POOL_VtSweep
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

        // Delays as multiples of the option's tau, so one list serves all three options even
        // though their time constants differ twentyfold. At the 10 kohm option's 909 us these
        // run from 91 us to 12 ms.
        //
        // The list reaches down to 0.1 tau because Globals.TheHdw.Wait is a Stopwatch spin
        // wait, not Thread.Sleep: that file comments the Sleep out explicitly because its
        // resolution is over 10 ms. A spin wait on the high resolution counter lands within a
        // microsecond or two, so the early points carry real information about the shape and
        // are worth sampling. The knee between 0.1 and 2.2 tau is where a shape error shows
        // up; the tail beyond that pins the asymptote.
        private static readonly double[] TauMultiples =
        {
            0.1, 0.25, 0.55, 1.1, 1.65, 2.2, 3.3, 4.4, 6.6, 8.8, 13.2
        };

        // 20 us against a tau near 850 us, so a point averages over 2% of a time constant.
        // The calibration step's 2 ms aperture is correct there because it reads a settled
        // node, but on a ramp it would integrate over 2.4 tau and smear the curve flat. This
        // is the one constant that must not be copied from that step.
        private const double PpmuApertureTimeSec = 20e-6;

        // Forcing 0 A is what makes the read high impedance. The node sits behind the 15K
        // divider at about 7.5 kohm, so a forced voltage would pull the level down.
        private const double PpmuForceCurrentAmps = 0.0;
        private const double PpmuForceCurrentRangeAmps = 2e-6;

        // The 6571 pin electronics span -2 V to 6 V. Limits are set to the full span so a
        // surprise reading is reported rather than silently clamped.
        private const double PpmuVoltageLimitHighVolts = 6.0;
        private const double PpmuVoltageLimitLowVolts = -2.0;

        // Tau is estimated from the point nearest one tau, where d(V/A)/d(tau) is largest.
        private const double TauEstimateTargetMultiple = 1.1;

        // Largest fraction of tau the aperture may occupy before it stops measuring a point on
        // the ramp and starts averaging a span of it. At 2% the smearing is below the PPMU's own
        // accuracy; by 40% the curve is flattened into something that still looks like data.
        private const double MaxApertureFractionOfTau = 0.05;

        /// <summary>
        /// Sweeps the node voltage against time for the requested combinations.
        ///
        /// Defaults to every slot and channel, matching the calibration and rise time steps, so
        /// the 312 published IDs line up exactly with the generated limit rows. A Semiconductor
        /// Multi Test step errors on a Tests tab entry that is never published, so a narrower
        /// default would fail the step for the 299 combinations it skipped rather than silently
        /// measuring less.
        ///
        /// The full sweep costs about 4.5 s: 24 combinations at roughly 190 ms, which is eleven
        /// discharges of ten tau each at 100 ms plus 42 tau of delays at 38 ms. The discharges,
        /// not the delays, dominate. Pass slot 12 and channel 1 for a single combination when
        /// working on the bench.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot to sweep, 12, 14, 21, or 0 for all three.</param>
        /// <param name="channel">POOL channel 1 to 8, or 0 for all eight.</param>
        /// <param name="inputOption">
        /// Input option 0 for 50 ohm 3.3 V, 1 for 50 ohm 10 V, 2 for 10 kohm 10 V, or -1 for
        /// all three.
        /// </param>
        /// <param name="publishResults">
        /// Publishes each sampled point, the asymptote and the tau estimate as per site
        /// results. This requires a Semiconductor Multi Test step with one Tests tab entry per
        /// published ID, so set it false to run the step as an Action and log only.
        /// </param>
        public static void PoolNodeVoltageSweep(
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
                    RejectIfApertureTooLong(option);

                    foreach (int ch in SelectedChannels(channel))
                    {
                        SweepOne(tsmContext, poolSlot, ch, option, publishResults);
                    }
                }
            }
        }

        /// <summary>
        /// Sweeps one slot, channel and input option combination.
        ///
        /// The relay path and the PPMU configuration are set up once and held for the whole
        /// delay list. Only the drive state and the wait change between points, which keeps the
        /// relays still and makes the points directly comparable.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="option">Input option.</param>
        /// <param name="publishResults">Publishes the sampled points as per site results.</param>
        /// <summary>
        /// Rejects an option whose time constant is too short for the fixed PPMU aperture.
        ///
        /// The aperture integrates forward from each delay, so it must be small against tau or
        /// every point averages a large span of the ramp. At 909 us the 20 us aperture is 2.2%
        /// of tau and harmless. On the 50 ohm options tau is 47.6 us, making the aperture 42% of
        /// tau and the shortest delay shorter than the aperture itself, so the first several
        /// points would integrate almost the same interval.
        ///
        /// This throws rather than warning because the failure is silent: the step would return
        /// a full set of plausible voltages describing a curve that does not exist.
        /// </summary>
        /// <param name="option">Input option being swept.</param>
        private static void RejectIfApertureTooLong(PoolInputOption option)
        {
            double tau = PoolInputOptionModel.TauSec(option);
            double fraction = PpmuApertureTimeSec / tau;

            if (fraction > MaxApertureFractionOfTau)
            {
                throw new NotSupportedException(string.Format(
                    "POOL V(t) sweep cannot measure {0}: tau is {1:F1} us so the {2:F1} us PPMU "
                        + "aperture spans {3:P0} of it, above the {4:P0} limit. The shortest delay "
                        + "is {5:F2} us. Sweep TenKOhm10V instead: the RC topology is shared, so "
                        + "the shape and the asymptote fraction carry over, and only tau scales.",
                    option,
                    tau * 1e6,
                    PpmuApertureTimeSec * 1e6,
                    fraction,
                    MaxApertureFractionOfTau,
                    TauMultiples[0] * tau * 1e6));
            }
        }

        private static void SweepOne(
            ISemiconductorModuleContext tsmContext,
            PoolSlot slot,
            int channel,
            PoolInputOption option,
            bool publishResults)
        {
            string prefix = "POOL_VT_" + PoolCalibrationStore.SlotName(slot)
                + "_CH" + channel + "_" + option;
            double driveVolts = PoolInputOptionModel.DriveVoltsFor(option);
            double tauSec = PoolInputOptionModel.TauSec(option);
            double dischargeSec = PoolRecordGeometry.DischargeTimeSec(option);

            HMODControl.AllHMODReset(tsmContext);

            Digital drive = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Drive[(int)slot]);
            Digital sense = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Dio[channel - 1]);

            try
            {
                // One Apply for the whole path. HMOD writes overwrite the entire chain, so
                // splitting these would reopen whatever went out first. The changeover is
                // closed, which is what diverts the divided node to the DIO pin.
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

                var points = new double[TauMultiples.Length][];
                for (int index = 0; index < TauMultiples.Length; index++)
                {
                    points[index] = MeasureAtDelay(
                        drive, sense, dischargeSec, TauMultiples[index] * tauSec);

                    if (publishResults)
                    {
                        tsmContext.PublishPerSite(
                            points[index], prefix + "_" + Label(TauMultiples[index]));
                    }
                }

                // The last point is at 13.2 tau, settled to within 2 ppm, so it is the
                // asymptote rather than an estimate of it.
                double[] asymptote = points[points.Length - 1];
                double[] tauEstimate = EstimateTau(points, asymptote, tauSec);

                Report(prefix, points, asymptote, tauEstimate, tauSec);

                if (publishResults)
                {
                    tsmContext.PublishPerSite(asymptote, prefix + "_Asymptote");
                    tsmContext.PublishPerSite(tauEstimate, prefix + "_TauEstimate");
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
        /// Discharges the node, steps the drive and reads the node after one delay.
        ///
        /// The discharge is what makes the points independent. Without it a point would start
        /// from the level the previous, longer delay left behind, and the curve would flatten
        /// into the asymptote from the first point onward while still looking like data.
        /// </summary>
        /// <param name="drive">Drive pin session.</param>
        /// <param name="sense">Sense pin session, already in PPMU mode and sourcing 0 A.</param>
        /// <param name="dischargeSec">Time to hold the node low, ten tau.</param>
        /// <param name="delaySec">Delay from the step to the read.</param>
        private static double[] MeasureAtDelay(
            Digital drive,
            Digital sense,
            double dischargeSec,
            double delaySec)
        {
            drive.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(dischargeSec);

            drive.WriteStatic(PinState._1);
            Globals.TheHdw.Wait(delaySec);

            // One array per instrument; a single DIO pin lives on a single 6571, so index 0
            // holds the per site readings.
            return sense.PPMUMeasure(PpmuMeasurementType.Voltage)[0];
        }

        /// <summary>
        /// Estimates tau per site by inverting V = A (1 - exp(-t / tau)) at the point nearest
        /// one tau.
        ///
        /// Returns NaN for a site whose reading cannot be inverted, rather than a number, so a
        /// bad site is visible instead of being absorbed into a plausible looking average.
        /// </summary>
        /// <param name="points">Readings for each delay in <see cref="TauMultiples"/> order.</param>
        /// <param name="asymptote">Settled level per site.</param>
        /// <param name="tauSec">Modelled tau, used to convert the multiple into a delay.</param>
        private static double[] EstimateTau(double[][] points, double[] asymptote, double tauSec)
        {
            int nearest = 0;
            for (int index = 1; index < TauMultiples.Length; index++)
            {
                if (Math.Abs(TauMultiples[index] - TauEstimateTargetMultiple)
                    < Math.Abs(TauMultiples[nearest] - TauEstimateTargetMultiple))
                {
                    nearest = index;
                }
            }

            double delaySec = TauMultiples[nearest] * tauSec;
            double[] sampled = points[nearest];
            var estimate = new double[sampled.Length];

            for (int site = 0; site < sampled.Length; site++)
            {
                double fraction = asymptote[site] == 0.0
                    ? double.NaN
                    : sampled[site] / asymptote[site];

                estimate[site] = fraction > 0.0 && fraction < 1.0
                    ? -delaySec / Math.Log(1.0 - fraction)
                    : double.NaN;
            }

            return estimate;
        }

        /// <summary>
        /// Logs the curve for site 0, with each point normalised to the asymptote and compared
        /// against the single pole exponential the width limits assume. A systematic deviation
        /// in the normalised column is the shape error the width residuals implied.
        /// </summary>
        /// <param name="prefix">Result name prefix.</param>
        /// <param name="points">Readings for each delay.</param>
        /// <param name="asymptote">Settled level per site.</param>
        /// <param name="tauEstimate">Estimated tau per site.</param>
        /// <param name="tauSec">Modelled tau.</param>
        private static void Report(
            string prefix,
            double[][] points,
            double[] asymptote,
            double[] tauEstimate,
            double tauSec)
        {
            if (asymptote.Length == 0)
            {
                return;
            }

            // TestSteps.Common.Debug is a checker class, so qualify the framework one.
            System.Diagnostics.Debug.WriteLine(
                prefix + " site 0: asymptote " + asymptote[0].ToString("F4")
                + " V, modelled tau " + (tauSec * 1e6).ToString("F1")
                + " us, estimated tau " + (tauEstimate[0] * 1e6).ToString("F1") + " us");

            for (int index = 0; index < TauMultiples.Length; index++)
            {
                double measured = points[index][0];
                double normalised = asymptote[0] == 0.0 ? double.NaN : measured / asymptote[0];
                double ideal = 1.0 - Math.Exp(-TauMultiples[index]);

                System.Diagnostics.Debug.WriteLine(
                    "  " + TauMultiples[index].ToString("F2") + " tau ("
                    + (TauMultiples[index] * tauSec * 1e6).ToString("F0") + " us): "
                    + measured.ToString("F4") + " V, normalised "
                    + normalised.ToString("F4") + ", single pole " + ideal.ToString("F4"));
            }
        }

        /// <summary>
        /// Published ID suffix for a delay, as the tau multiple scaled by one hundred so the
        /// name carries no decimal point. 0.55 tau becomes Tau055 and 13.2 tau becomes Tau1320.
        /// </summary>
        /// <param name="multiple">Delay as a multiple of tau.</param>
        private static string Label(double multiple)
        {
            return "Tau" + ((int)Math.Round(multiple * 100.0)).ToString("D3");
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
