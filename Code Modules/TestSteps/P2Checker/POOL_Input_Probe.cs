using System;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;
using static TestSteps.Common.PoolRelay;

namespace TestSteps.P2Checker
{
    /// <summary>POOL slot under test. The value is the slot's offset within a channel block.</summary>
    public enum PoolSlot
    {
        /// <summary>SL12, the first slot select relay in each channel block.</summary>
        SL12 = 0,

        /// <summary>SL14, the second slot select relay in each channel block.</summary>
        SL14 = 1,

        /// <summary>SL21, the third slot select relay in each channel block.</summary>
        SL21 = 2
    }

    /// <summary>
    /// TFE input option. The value is the option's offset within a channel block, so these
    /// are deliberately numbered from 3 rather than from 0.
    /// </summary>
    public enum PoolInputOption
    {
        /// <summary>50 ohm, 3.3 V max input option, R47.</summary>
        FiftyOhm3V3 = 3,

        /// <summary>50 ohm, 10 V max input option, R62/R63.</summary>
        FiftyOhm10V = 4,

        /// <summary>10 kohm, 10 V max resistor network input option, R64/R55/R66.</summary>
        TenKOhm10V = 5
    }

    /// <summary>
    /// Relay map for the 3 slots x 8 channels x 3 input options POOL matrix.
    ///
    /// The translator board allocates nine consecutive relays per channel, packed across
    /// HMOD11, HMOD12 and HMOD13 with no gaps, so a channel's block starts at global relay
    /// 1 + 9 * (channel - 1) counting HMOD11 K1 as 1 and HMOD13 K32 as 96. Within a block
    /// the offsets are fixed, as read from Tx Board 02-089357 for channel 1 where the block
    /// is HMOD11 K1 to K9:
    ///
    ///   +0  slot select, T_POOL_SL12_OUT_CH_n     (K1)
    ///   +1  slot select, T_POOL_SL14_OUT_CH_n     (K2)
    ///   +2  slot select, T_POOL_SL21_OUT_CH_n     (K3)
    ///   +3  input option, 50 ohm 3.3 V max        (K4)
    ///   +4  input option, 50 ohm 10 V max         (K5)
    ///   +5  input option, 10 kohm 10 V max        (K6)
    ///   +6  input changeover, comparator or DIO   (K7)
    ///   +7  tap to SCOPE 5172 CH(n-1)             (K8)
    ///   +8  tap to 6571 DIO(23+n)                 (K9)
    ///
    /// This reproduces all five literal tables in SL12_POOL_Check exactly, including the
    /// channel 4 and channel 8 straddles where a block crosses a register boundary, which
    /// is what confirms the packing: SlotSelect gives HMOD11 K1/K10/K19/K28 then HMOD12
    /// K5/K14/K23/K32, and Input10kOhm gives HMOD11 K6/K15/K24 then HMOD12 K1 and finally
    /// HMOD13 K5.
    /// </summary>
    public static class PoolPathMap
    {
        private const int RelaysPerChannel = 9;
        private const int RelaysPerRegister = 32;
        private const int ChannelCount = 8;

        /// <summary>
        /// Translates a global relay index into the register and K number that address it.
        /// </summary>
        /// <param name="global">Global relay index, 1 for HMOD11 K1 through 96 for HMOD13 K32.</param>
        private static PoolRelay FromGlobal(int global)
        {
            if (global < 1 || global > 3 * RelaysPerRegister)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(global), global, "Global relay index must be 1..96.");
            }

            int number = ((global - 1) % RelaysPerRegister) + 1;
            switch ((global - 1) / RelaysPerRegister)
            {
                case 0:
                    return Hmod11(number);
                case 1:
                    return Hmod12(number);
                default:
                    return Hmod13(number);
            }
        }

        /// <summary>Relay at a given offset within a channel's nine relay block.</summary>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="offset">Offset within the block, 0 to 8.</param>
        public static PoolRelay Relay(int channel, int offset)
        {
            if (channel < 1 || channel > ChannelCount)
            {
                throw new ArgumentOutOfRangeException(nameof(channel), channel, "Channel must be 1..8.");
            }

            if (offset < 0 || offset >= RelaysPerChannel)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be 0..8.");
            }

            return FromGlobal(1 + (RelaysPerChannel * (channel - 1)) + offset);
        }

        /// <summary>Connects the channel's TFE input to T_POOL_SLnn_OUT_CH_n.</summary>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static PoolRelay SlotSelect(PoolSlot slot, int channel)
        {
            return Relay(channel, (int)slot);
        }

        /// <summary>Selects one of the three TFE input options.</summary>
        /// <param name="option">Input option to select.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static PoolRelay InputOption(PoolInputOption option, int channel)
        {
            return Relay(channel, (int)option);
        }

        /// <summary>
        /// The channel's input changeover relay. De-energised the divided node feeds the
        /// window comparator; energised it is diverted to the 6571 DIO pin instead, bypassing
        /// the comparator and the XOR. Rise time measurement needs this left open.
        /// </summary>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static PoolRelay Changeover(int channel)
        {
            return Relay(channel, 6);
        }

        /// <summary>Taps the channel's output to SCOPE 5172 CH(n-1).</summary>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static PoolRelay ScopeTap(int channel)
        {
            return Relay(channel, 7);
        }

        /// <summary>Taps the channel's output to 6571 DIO(23+n).</summary>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static PoolRelay DigitalTap(int channel)
        {
            return Relay(channel, 8);
        }

        /// <summary>
        /// Checker board CHMOD6 relay that connects T_HSD200_SL11_CH(5+slot) to the slot's
        /// AD8244BRMZ buffer, so K16, K18 and K20 for SL12, SL14 and SL21.
        /// </summary>
        /// <param name="slot">Slot under test.</param>
        public static int CheckerDrive(PoolSlot slot)
        {
            return 16 + (2 * (int)slot);
        }

        /// <summary>
        /// Checker board CHMOD6 relay that fans the slot's RC node out to one POOL output.
        ///
        /// Channel 1 interleaves with the drive relays at K17, K19 and K21, one per slot.
        /// Channels 2 to 8 are a contiguous block of 21 relays from K22 to K42, three per
        /// channel in SL12, SL14, SL21 order.
        /// </summary>
        /// <param name="slot">Slot under test.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public static int CheckerFanOut(PoolSlot slot, int channel)
        {
            if (channel < 1 || channel > ChannelCount)
            {
                throw new ArgumentOutOfRangeException(nameof(channel), channel, "Channel must be 1..8.");
            }

            return channel == 1
                ? 17 + (2 * (int)slot)
                : 22 + (3 * (channel - 2)) + (int)slot;
        }
    }

    /// <summary>
    /// Bench diagnostic: forces a DC level on the slot's HSD200 drive pin, settles the RC,
    /// and reads the resulting DC level back on the channel's 6571 DIO pin with the PPMU.
    /// Nothing is compared against a limit; this exists to answer what the window
    /// comparator input actually sees for each of the three TFE input options.
    ///
    /// HMOD11 K7 and its per channel equivalents are an input path changeover, not an output
    /// one. De-energised, the divided node feeds the AD96687BRZ window comparator, which is
    /// normal POOL operation. Energised, it diverts that node to the 6571 DIO pin instead,
    /// bypassing the comparator and the XOR entirely, which is what makes this measurement
    /// possible. The POOL rise time steps therefore must leave it open, and the probe must
    /// close it.
    ///
    /// The step sweeps the drive level and publishes the measured level at each point. The
    /// slope of measured against driven identifies which node the diverted path taps:
    ///
    ///   slope ~ 1.000  the drive side of the 1 kohm, so upstream of the RC
    ///   slope ~ 0.909  the RC node itself, 10 kohm option, before the 15K divider
    ///   slope ~ 0.4545 TFE_IN, 10 kohm option, after the R55/R66 divide by two
    ///   slope ~ 0.0476 the RC node, either 50 ohm option
    ///   slope ~ 0      no DC path, so the diversion is not reaching the DIO pin
    ///
    /// 0.4545 is the expected result and the one worth having, because a level taken after the
    /// divider is the comparator full scale directly, with no dependence on the R55/R66 ratio.
    /// A reading parked near -1.0 V or -1.7 V regardless of drive means the DIO pin is still
    /// on the ECL XOR output, so the changeover did not divert and the full scale has to keep
    /// coming from the VOH bisection in the POOL steps.
    /// </summary>
    public class POOL_Input_Probe
    {
        // 5 tau at the 10 kohm option's 909 us, rounded up, so the node is settled to
        // better than 0.1% before the PPMU aperture opens. The 50 ohm options settle about
        // twenty times faster, so this is generous for them.
        private const double SettlingTimeSec = 10e-3;

        // Long aperture because this is a one off bench read, not a production test. At
        // 2 ms the PPMU averages out mains hum and the ECL supply noise.
        private const double PpmuApertureTimeSec = 2e-3;

        // The PPMU measures voltage while forcing 0 A, which is the high impedance read
        // needed on a node fed through 10 kohm. A 2 uA range keeps the forced current well
        // under the node's own currents without risking a range underflow.
        private const double PpmuForceCurrentAmps = 0.0;
        private const double PpmuForceCurrentRangeAmps = 2e-6;

        // The 6571 pin electronics span 6 V to -2 V, so the PPMU voltage limits are set to
        // the full span rather than to the expected reading. Clamping tighter would silently
        // cap a surprise instead of reporting it, which is the opposite of what a diagnostic
        // should do.
        private const double PpmuVoltageLimitHighVolts = 6.0;
        private const double PpmuVoltageLimitLowVolts = -2.0;

        // The 50 ohm 3.3 V max option is rated at the relay input, which is the full drive
        // level, not the attenuated node. Driving 5 V into it exceeds that rating, so the
        // drive is clamped for that option only.
        private const double ThreeVoltOptionMaxDriveVolts = 3.3;

        // Drive levels swept by default. Zero anchors the offset and the rest give a slope
        // over the usable range of all three options.
        private static readonly double[] DefaultDriveSweepVolts = { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };

        /// <summary>
        /// Routes one POOL path and reports the DC level the comparator input receives.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="slot">POOL slot, 12, 14 or 21.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        /// <param name="inputOption">TFE input option, 0 for 50 ohm 3.3 V, 1 for 50 ohm 10 V, 2 for 10 kohm 10 V.</param>
        /// <param name="divertToDio">
        /// Energises the channel's input changeover so the divided node goes to the DIO pin
        /// instead of the window comparator. True is the only setting that measures anything;
        /// false leaves the DIO pin on the ECL XOR output and is kept only so the two states
        /// can be compared on the bench.
        /// </param>
        public static void PoolInputProbe(
            ISemiconductorModuleContext tsmContext,
            int slot = 12,
            int channel = 1,
            int inputOption = 2,
            bool divertToDio = true)
        {
            PoolSlot poolSlot = ParseSlot(slot);
            PoolInputOption option = ParseInputOption(inputOption);

            double maxDrive = option == PoolInputOption.FiftyOhm3V3
                ? ThreeVoltOptionMaxDriveVolts
                : double.MaxValue;

            string prefix = "POOL_SL" + slot + "_CH" + channel + "_OPT" + inputOption;

            HMODControl.AllHMODReset(tsmContext);

            Digital drive = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Drive[(int)poolSlot]);
            Digital sense = InstrCtrl.DigitalPinsToSessions(
                tsmContext, PoolCapturePins.Dio[channel - 1]);

            try
            {
                // One Apply for the whole path. HMOD writes overwrite the entire chain, so
                // splitting these would reopen whatever went out first.
                var state = new PoolHmodState();
                state.Close(
                    PoolPathMap.SlotSelect(poolSlot, channel),
                    PoolPathMap.InputOption(option, channel),
                    PoolPathMap.DigitalTap(channel));

                if (divertToDio)
                {
                    state.Close(PoolPathMap.Changeover(channel));
                }

                state.CloseChecker(
                    PoolPathMap.CheckerDrive(poolSlot),
                    PoolPathMap.CheckerFanOut(poolSlot, channel));

                // TestSteps.Common.Debug is a checker class, so qualify the framework one.
                System.Diagnostics.Debug.WriteLine(prefix + " relays: " + state);
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

                foreach (double driveVolts in DefaultDriveSweepVolts)
                {
                    if (driveVolts > maxDrive)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            prefix + " skipping " + driveVolts.ToString("F1")
                            + " V, above the " + maxDrive.ToString("F1")
                            + " V rating of the selected input option");
                        continue;
                    }

                    // vih carries the drive level and WriteStatic holds the pin there. The
                    // compare levels are irrelevant on a pin that only drives, but vol and
                    // voh must still sit inside the pin electronics range.
                    drive.ConfigureVoltgeLevels(
                        vil: 0, vih: driveVolts, vol: 0, voh: 0, vterm: 0);
                    drive.WriteStatic(PinState._1);

                    Globals.TheHdw.Wait(SettlingTimeSec);

                    // PPMUMeasure returns one array per instrument. A single DIO pin lives on
                    // a single 6571, so there is exactly one entry and index 0 holds the per
                    // site readings, the same indexing All_POOL2_Check uses.
                    double[] measured = sense.PPMUMeasure(PpmuMeasurementType.Voltage)[0];

                    string label = prefix + "_Drive" + driveVolts.ToString("F1").Replace(".", "p");
                    tsmContext.PublishPerSite(measured, label);
                    System.Diagnostics.Debug.WriteLine(
                        label + " = " + string.Join(", ", Array.ConvertAll(
                            measured, v => v.ToString("F4") + " V")));
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

        /// <summary>Maps a slot number to its offset within a channel block.</summary>
        /// <param name="slot">Slot number, 12, 14 or 21.</param>
        private static PoolSlot ParseSlot(int slot)
        {
            switch (slot)
            {
                case 12:
                    return PoolSlot.SL12;
                case 14:
                    return PoolSlot.SL14;
                case 21:
                    return PoolSlot.SL21;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(slot), slot, "Slot must be 12, 14 or 21.");
            }
        }

        /// <summary>Maps a 0 to 2 step parameter to its relay offset within a channel block.</summary>
        /// <param name="inputOption">0 for 50 ohm 3.3 V, 1 for 50 ohm 10 V, 2 for 10 kohm 10 V.</param>
        private static PoolInputOption ParseInputOption(int inputOption)
        {
            switch (inputOption)
            {
                case 0:
                    return PoolInputOption.FiftyOhm3V3;
                case 1:
                    return PoolInputOption.FiftyOhm10V;
                case 2:
                    return PoolInputOption.TenKOhm10V;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(inputOption), inputOption, "Input option must be 0, 1 or 2.");
            }
        }
    }
}
