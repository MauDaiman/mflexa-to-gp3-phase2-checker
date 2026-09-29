using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;

namespace TestSteps.P2Checker
{
    /// <summary>
    /// Slot 2 (BBAC CAPTURE) circuit checker for BBAC CAPTURE CH1 and CH2.
    /// Tx Board routing per 02-089357 BBAC CAPTURE CH1 (SLOT2) and CH2 (SLOT2) sheets; Checker
    /// Board capture network per 02-101092 "SLOT 2 (BBAC CAPTURE) CIRCUIT CHECKER".
    /// The stimulus reuses the BBAC source circuitry: the PXIe-4467 AOUT drives SRCx+/- and the
    /// source checker K7 relay diverts them onto the BBAC CAPx test points, which the Checker
    /// Board capture relays feed into CAPx+/-.
    /// </summary>
    public class SL02_BBAC_CAP_Check
    {
        private const double SettlingTimeSec = 25e-3;
        private const double AcSourceFrequencyHz = 1000;
        private const double AcSourceAmplitudeV = 5; // 10 Vpp expressed as zero-to-peak
        private const double AinSamplingRateHz = 100e3;
        private const int AinSampleCount = 1000;
        private const double AinRangeV = 10;
        private const double SmuCurrentRangeA = 10e-3;
        private const double SmuCurrentLimitA = 10e-3;
        private const double SmuVoltageRangeV = 6;

        /// <summary>
        /// Runs the BBAC CAPTURE CH1 checker sequence on the Slot 2 connector.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="senseType">Meter sense mode (Local or Remote).</param>
        /// <param name="meterType">Meter resource selection (0 = PXIe-4137, 1 = PXIe-4081).</param>
        public static void SL02BBACCapCh1Check(ISemiconductorModuleContext tsmContext,
            DCPowerMeasurementSense senseType,
            int meterType = 0)
        {
            RunChannel(tsmContext, BbacCapChannel.Ch1, senseType, meterType);
        }

        /// <summary>
        /// Runs the BBAC CAPTURE CH2 checker sequence on the Slot 2 connector.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="senseType">Meter sense mode (Local or Remote).</param>
        /// <param name="meterType">Meter resource selection (0 = PXIe-4137, 1 = PXIe-4081).</param>
        public static void SL02BBACCapCh2Check(ISemiconductorModuleContext tsmContext,
            DCPowerMeasurementSense senseType,
            int meterType = 0)
        {
            RunChannel(tsmContext, BbacCapChannel.Ch2, senseType, meterType);
        }

        /// <summary>
        /// Executes the full BBAC capture checker procedure for one BBAC capture channel.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="ch">Per-channel relay and pin constants.</param>
        /// <param name="senseType">Meter sense mode.</param>
        /// <param name="meterType">Meter resource selection.</param>
        private static void RunChannel(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            DCPowerMeasurementSense senseType,
            int meterType)
        {
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType, ForceMode.ForceCurrent);

            HMODControl.AllHMODReset(tsmContext);

            // SPI resources belong to the Checker Board HMOD circuit for this check
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);

            InstrCtrl.SetDAQmxAOFuncGenTasks(tsmContext,
                offset: 0,
                amplitude: AcSourceAmplitudeV,
                frequency: AcSourceFrequencyHz,
                type: NationalInstruments.DAQmx.AOFunctionGenerationType.Sine);
            DAQmx acSource = InstrCtrl.PinsToDAQmxTasks(tsmContext, ch.AcSourcePin);

            InstrCtrl.SetDAQmxAITasks(tsmContext,
                samplingRate: AinSamplingRateHz,
                sampleSize: AinSampleCount,
                minimumVoltage: -AinRangeV,
                maximumVoltage: AinRangeV);
            DAQmx analogIn = InstrCtrl.PinsToDAQmxTasks(tsmContext, ch.AinPin);

            DCPower ppmuCapPlus = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.PpmuCapPlusPin);
            DCPower ppmuCapMinus = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.PpmuCapMinusPin);

            ConfigureSmu(ppmuCapPlus);
            ConfigureSmu(ppmuCapMinus);

            var state = new CapRelayState();

            // HMOD4 K25/K26 rest on CC COMMON1, so CH1's PXIe-4147 CH2/CH3 LO sense lines are
            // already on its own cc common with no relay driven. CH2 has to hold them on CC COMMON2
            // for its whole run, otherwise they fall back to CC COMMON1 and CH2 ends up sensing the
            // other channel's reference. LoSenseTie is null for CH1, so this is a no-op there.
            // LoChangeover is deliberately NOT held here. HMOD13 K31/K32 are the AGND star-ground for
            // CC COMMON1/CC COMMON2, so energising one lifts that cc common off ground. It may only be
            // closed while K3 has the cc common on BBAC_1V_REF and must follow K3 exactly, otherwise
            // every other measurement in the step runs with an ungrounded cc common.
            state.Set(true, ch.LoSenseTie);
            state.Apply(tsmContext);

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // METER_LO to GND

            try
            {
                CheckAinFunctionality(tsmContext, ch, state, acSource, analogIn);
                CheckCapturePogoWithPpmu(tsmContext, ch, state, acSource, ppmuCapPlus, ppmuCapMinus);
                CheckPpmuToDibAccess(tsmContext, ch, state, meter, ppmuCapPlus, ppmuCapMinus);
                CheckDgsCapCcCommon(tsmContext, ch, state, meter, ppmuCapMinus);

                // The connectivity check is purely reference -> net -> meter and uses no SMU, so both
                // PPMUs are taken off the pins first. Left enabled they sit at 0 V with remote sense
                // referenced to the cc common, which floats once KM is released, so a disabled channel
                // regulating against an undefined sense point can offset the net being measured.
                ShutdownSmu(ppmuCapPlus);
                ShutdownSmu(ppmuCapMinus);

                CheckDgsCapConnectivity(tsmContext, ch, state, meter);
            }
            finally
            {
                acSource.Stop();
                analogIn.Stop();

                ShutdownSmu(ppmuCapPlus);
                ShutdownSmu(ppmuCapMinus);

                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);

                meter.Cleanup(tsmContext);
                HMODControl.CHMODReset(tsmContext);
                HMODControl.AllHMODReset(tsmContext);
            }
        }

        /// <summary>
        /// Checks the AIN path: the source relays put the PXIe-4467 AOUT on SRCx+/-, K7 diverts
        /// them onto the BBAC CAPx test points, the Checker Board capture relays feed them into
        /// CAPx+/-, and KA/KB plus KC/KD route CAPx+/- back to the PXIe-4467 differential analog
        /// input where the peak-to-peak amplitude of the 1 kHz 10 Vpp sine is measured.
        /// </summary>
        private static void CheckAinFunctionality(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            CapRelayState state,
            DAQmx acSource,
            DAQmx analogIn)
        {
            state.Set(true, ch.SrcKa, ch.SrcKb, ch.SrcKg, ch.SrcKh, ch.K7);
            state.Set(true, ch.CapPlusToSrc, ch.CapMinusToSrc);
            state.Set(true, ch.Ka, ch.Kb, ch.Kc, ch.Kd);
            state.Apply(tsmContext);

            acSource.ConfigureAOFuncGen(
                NationalInstruments.DAQmx.AOFunctionGenerationType.Sine,
                frequency: AcSourceFrequencyHz,
                amplitude: AcSourceAmplitudeV,
                offset: 0);
            acSource.StartAOFuncGen();
            Globals.TheHdw.Wait(SettlingTimeSec);

            analogIn.Start();
            double[][] ainSamples = analogIn.ReadAnalog(AinSampleCount);
            analogIn.Stop();

            double[] ainVpp = ainSamples.Select(samples => samples.Max() - samples.Min()).ToArray();
            tsmContext.PublishPerSite(ainVpp, ch.Prefix + "_AinDiff_Vpp");
        }

        /// <summary>
        /// Checks the CAPx+/- pogo pins with the PXIe-4147 PPMUs: KE/KI connect CH2 to CAPx+ and
        /// KG/KJ connect CH3 to CAPx-, both measured against AGND. The AC stimulus is then turned
        /// off and the whole capture input path released.
        /// </summary>
        private static void CheckCapturePogoWithPpmu(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            CapRelayState state,
            DAQmx acSource,
            DCPower ppmuCapPlus,
            DCPower ppmuCapMinus)
        {
            state.Set(true, ch.Ke, ch.Ki, ch.Kg, ch.Kj);
            state.Apply(tsmContext);

            // currentLevelRange is passed explicitly because ForceCurrent otherwise derives it from
            // the level, and a 0 A level would ask the driver for a 0 A range.
            ppmuCapPlus.ForceCurrent(currentLevel: 0, voltageLimit: SmuVoltageRangeV, currentLevelRange: SmuCurrentRangeA);
            ppmuCapMinus.ForceCurrent(currentLevel: 0, voltageLimit: SmuVoltageRangeV, currentLevelRange: SmuCurrentRangeA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            ppmuCapPlus.Measure(out double[] capPlus, out _);
            ppmuCapMinus.Measure(out double[] capMinus, out _);

            ppmuCapPlus.PinQueryContext.Publish(capPlus, ch.Prefix + "_CapP_V");
            ppmuCapMinus.PinQueryContext.Publish(capMinus, ch.Prefix + "_CapN_V");

            acSource.Stop();

            state.Set(false, ch.Ke, ch.Ki, ch.Kg, ch.Kj);
            state.Set(false, ch.Ka, ch.Kb, ch.Kc, ch.Kd);
            state.Set(false, ch.CapPlusToSrc, ch.CapMinusToSrc);
            state.Set(false, ch.K7, ch.SrcKa, ch.SrcKb, ch.SrcKg, ch.SrcKh);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks PPMU to DIB access: KE/KF and KG/KH connect the PXIe-4147 CH2 and CH3 PPMUs to the
        /// ACC_CAPx+/- pogo pins, both force +1 V, and K1 then K2 tap each net onto METER_HI so both
        /// read +1 V.
        /// </summary>
        private static void CheckPpmuToDibAccess(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            CapRelayState state,
            IMeterStrategy meter,
            DCPower ppmuCapPlus,
            DCPower ppmuCapMinus)
        {
            state.Set(true, ch.Ke, ch.Kf, ch.Kg, ch.Kh);
            state.Apply(tsmContext);

            // KE/KF connect CH2 to ACC_CAPx+ and KG/KH connect CH3 to ACC_CAPx-, so both PPMUs have
            // to source +1 V for both nets to read +1 V. Only CapPlus used to be driven, which left
            // ACC_CAPx- floating on whatever charge it still held: AccCapN read 1.5 V to 1.7 V on
            // both channels and drifted run to run while AccCapP sat steady at 0.9998.
            ppmuCapPlus.ForceVoltage(voltageLevel: 1, currentLimit: SmuCurrentLimitA);
            ppmuCapMinus.ForceVoltage(voltageLevel: 1, currentLimit: SmuCurrentLimitA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            MeasureOnMeter(tsmContext, state, meter, ch.K1, ch.Prefix + "_AccCapP_V");
            MeasureOnMeter(tsmContext, state, meter, ch.K2, ch.Prefix + "_AccCapN_V");

            ppmuCapPlus.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);
            ppmuCapMinus.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);

            state.Set(false, ch.Ke, ch.Kf, ch.Kg, ch.Kh);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks DGSCap / cc common1 functionality: KM ties CC COMMON1 to DGSCapx, KG/KH put the
        /// PXIe-4147 CH3 on ACC_CAPx- and force +1 V, and K2 taps that net onto METER_HI. With K3
        /// closed DGSCapx sits on the 1.0 V reference so the reading is +2 V; with K3 open
        /// DGSCapx returns to AGND and the reading is +1 V.
        /// </summary>
        private static void CheckDgsCapCcCommon(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            CapRelayState state,
            IMeterStrategy meter,
            DCPower ppmuCapMinus)
        {
            // LoChangeover goes closed with K3 and is released with it below, so the cc common is only
            // off its AGND star-ground while K3 holds it at BBAC_1V_REF: HMOD13 K31 for CC COMMON1,
            // K32 for CC COMMON2, with LoSenseTie bringing CH2's per-channel sense lines along. The
            // channels are configured for DCPowerMeasurementSense.Remote, so the LO sense line sets
            // the regulation reference; without it the SMU regulates against AGND, the K3 lift has no
            // return path to be referenced against, and the DgsRef reading stays at the forced +1 V
            // instead of +2 V and matches DgsGnd.
            state.Set(true, ch.Km, ch.Kg, ch.Kh, ch.K3, ch.LoChangeover, ch.K2);
            state.Apply(tsmContext);

            ppmuCapMinus.ForceVoltage(voltageLevel: 1, currentLimit: SmuCurrentLimitA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_AccCapN_DgsRef_V");

            // LoChangeover is released with K3 so the cc common returns to its AGND star-ground the
            // moment it is no longer held at BBAC_1V_REF.
            state.Set(false, ch.K3, ch.LoChangeover); // DGSCapx back to AGND
            state.Apply(tsmContext);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_AccCapN_DgsGnd_V");

            ppmuCapMinus.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);

            // KM is released here rather than in CheckDgsCapConnectivity, which used to release a
            // relay it never set. Leaving KM closed tied CC COMMON1 onto DGSCapx for the whole of
            // that function while K3 held DGSCapx at the 1.0 V reference and the PXIe-4147 was
            // still enabled, so the reference was loaded by the SMU's LO network. Each function now
            // owns the relays it sets.
            state.Set(false, ch.Km, ch.Kg, ch.Kh, ch.K2);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks DGSCap to BBAC capture circuit connectivity: with K3 referencing DGSCapx to
        /// 1.0 V, KN/KF then KO/KH switch DGSCapx onto ACC_CAPx+ and ACC_CAPx-, and each net must
        /// read the 1.0 V reference on METER_HI.
        /// </summary>
        private static void CheckDgsCapConnectivity(ISemiconductorModuleContext tsmContext,
            BbacCapChannel ch,
            CapRelayState state,
            IMeterStrategy meter)
        {
            state.Set(true, ch.K3, ch.Kn, ch.Kf);
            MeasureOnMeter(tsmContext, state, meter, ch.K1, ch.Prefix + "_DgsFunc_AccCapP_V");
            state.Set(false, ch.Kn, ch.Kf);

            state.Set(true, ch.Ko, ch.Kh);
            MeasureOnMeter(tsmContext, state, meter, ch.K2, ch.Prefix + "_DgsFunc_AccCapN_V");
            state.Set(false, ch.Ko, ch.Kh, ch.K3);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Closes one Checker Board METER_HI tap relay, publishes the meter reading, then reopens
        /// the tap.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="state">Accumulated relay state.</param>
        /// <param name="meter">Meter strategy in use.</param>
        /// <param name="meterTap">Checker Board relay tapping the net under test onto METER_HI.</param>
        /// <param name="publishId">Published data ID for the measurement.</param>
        private static void MeasureOnMeter(ISemiconductorModuleContext tsmContext,
            CapRelayState state,
            IMeterStrategy meter,
            CapRelay meterTap,
            string publishId)
        {
            state.Set(true, meterTap);
            state.Apply(tsmContext);

            // The tap has to settle before the meter reads. Without this the first measurement after
            // CheckDgsCapCcCommon was sampled while the net was still discharging from +2 V toward the
            // +1 V reference, so DgsFunc_AccCapP landed anywhere between the two and moved every run,
            // while the later AccCapN reading passed only because the extra relay operations ahead of
            // it supplied the delay by accident.
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), publishId);

            state.Set(false, meterTap);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Applies the common SMU configuration used by the capture PPMU channels.
        /// </summary>
        /// <param name="smu">The DCPower session wrapper to configure.</param>
        private static void ConfigureSmu(DCPower smu)
        {
            smu.Abort();
            smu.ConfigureSettings(
                apertureTime: 10e-3,
                apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            smu.ConfigureVoltageLevelRange(voltageLevelRange: SmuVoltageRangeV);
            smu.ConfigureCurrentLevelRange(currentLevelRange: SmuCurrentRangeA);

            // The current limit value must come down before the range is narrowed. DCSetup leaves
            // 100 mA on every ALLDC pin, and these PXIe-4147 channels are in that group, so setting
            // a 10 mA limit range first would leave a 100 mA limit outside its own range and the
            // driver rejects the next property write with -1074097882.
            smu.ConfigureCurrentLimit(currentLimit: SmuCurrentLimitA);
            smu.ConfigureCurrentLimitRange(currentLimitRange: SmuCurrentLimitA);
            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);
        }

        /// <summary>
        /// Returns an SMU to 0 V and disconnects its output.
        /// </summary>
        /// <param name="smu">The DCPower session wrapper to shut down.</param>
        private static void ShutdownSmu(DCPower smu)
        {
            smu.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);
            smu.ConfigureOutputEnabled(false);
            smu.ConfigureOutputConnected(false);
            smu.Abort();
        }

        /// <summary>
        /// Shift register that carries a given BBAC capture relay.
        /// </summary>
        private enum CapRegister
        {
            /// <summary>Tx Board HMOD4, 32-bit.</summary>
            Hmod4,

            /// <summary>Tx Board HMOD8, 32-bit.</summary>
            Hmod8,

            /// <summary>Tx Board HMOD10, 32-bit.</summary>
            Hmod10,

            /// <summary>Tx Board HMOD13, 32-bit.</summary>
            Hmod13,

            /// <summary>Tx Board HMOD18, 32-bit (BBAC SOURCE CH1 stimulus path).</summary>
            Hmod18,

            /// <summary>Tx Board HMOD23, 32-bit (BBAC SOURCE CH2 stimulus path).</summary>
            Hmod23,

            /// <summary>Tx Board HMOD24, 72-bit (BBAC CAPTURE CH1 DGSCap relays).</summary>
            Hmod24,

            /// <summary>Tx Board HMOD25, 72-bit (BBAC CAPTURE CH2 DGSCap relays).</summary>
            Hmod25,

            /// <summary>Checker Board HMOD1, 32-bit.</summary>
            Chmod1,

            /// <summary>Checker Board HMOD6, 72-bit.</summary>
            Chmod6
        }

        /// <summary>
        /// One BBAC capture relay: the shift register that carries it and its bit mask.
        /// </summary>
        private sealed class CapRelay
        {
            /// <summary>
            /// Creates a relay descriptor.
            /// </summary>
            /// <param name="register">Shift register carrying the relay.</param>
            /// <param name="relays">Comma separated relay names, for example "K28" or "K6, K7".</param>
            public CapRelay(CapRegister register, string relays)
            {
                Register = register;

                // HMOD24 and HMOD25 are miswired over K21 to K56, so their relay numbers are
                // remapped to the physical relay. CHMOD6 is wired correctly and must not be.
                bool hmod2425 = register == CapRegister.Hmod24
                    || register == CapRegister.Hmod25;

                Wide = hmod2425 || register == CapRegister.Chmod6;
                Mask = Wide
                    ? HMODControl.RelayID72(relays, hmod2425)
                    : new uint[] { HMODControl.RelayID(relays) };
            }

            /// <summary>Shift register carrying this relay.</summary>
            public CapRegister Register { get; }

            /// <summary>True for 72-bit registers, false for 32-bit registers.</summary>
            public bool Wide { get; }

            /// <summary>Relay bit mask: uint[1] for 32-bit registers, uint[3] for 72-bit ones.</summary>
            public uint[] Mask { get; }
        }

        /// <summary>
        /// Accumulated relay state for every shift register involved in the BBAC capture path.
        /// A single <see cref="Apply"/> call shifts the whole state out so no register latches
        /// stale data from a previous step.
        /// </summary>
        private sealed class CapRelayState
        {
            private uint _hmod4;
            private uint _hmod8;
            private uint _hmod10;
            private uint _hmod13;
            private uint _hmod18;
            private uint _hmod23;
            private uint _chmod1;
            private readonly uint[] _hmod24 = new uint[3];
            private readonly uint[] _hmod25 = new uint[3];
            private readonly uint[] _chmod6 = new uint[3];

            /// <summary>
            /// Closes or opens the given relays in the accumulated state. Call
            /// <see cref="Apply"/> afterwards to shift the state into the hardware.
            /// </summary>
            /// <param name="connect">True to close the relays, false to open them.</param>
            /// <param name="relays">Relays to change.</param>
            public void Set(bool connect, params CapRelay[] relays)
            {
                foreach (CapRelay relay in relays)
                {
                    // Null entries are allowed so a channel can leave an optional relay out of
                    // its definition without the call sites needing a conditional.
                    if (relay == null)
                    {
                        continue;
                    }

                    switch (relay.Register)
                    {
                        case CapRegister.Hmod4:
                            _hmod4 = Toggle(_hmod4, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod8:
                            _hmod8 = Toggle(_hmod8, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod10:
                            _hmod10 = Toggle(_hmod10, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod13:
                            _hmod13 = Toggle(_hmod13, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod18:
                            _hmod18 = Toggle(_hmod18, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod23:
                            _hmod23 = Toggle(_hmod23, relay.Mask[0], connect);
                            break;
                        case CapRegister.Chmod1:
                            _chmod1 = Toggle(_chmod1, relay.Mask[0], connect);
                            break;
                        case CapRegister.Hmod24:
                            Toggle(_hmod24, relay.Mask, connect);
                            break;
                        case CapRegister.Hmod25:
                            Toggle(_hmod25, relay.Mask, connect);
                            break;
                        case CapRegister.Chmod6:
                            Toggle(_chmod6, relay.Mask, connect);
                            break;
                    }
                }
            }

            /// <summary>
            /// Shifts the current relay state into the Tx Board and Checker Board HMOD chains and
            /// waits for the relays to settle. HMOD14 K1-K4 stay closed so the DIO pins remain
            /// connected to the HSD that drives the shift registers.
            /// </summary>
            /// <param name="tsmContext">The semiconductor module context.</param>
            public void Apply(ISemiconductorModuleContext tsmContext)
            {
                HMODControl.HMOD1to4(tsmContext, HMOD_Data_4: _hmod4);

                HMODControl.HMOD5to10(tsmContext,
                    HMOD_Data_8: _hmod8,
                    HMOD_Data_10: _hmod10);

                HMODControl.HMOD14to18(tsmContext,
                    HMOD_Data_14: HMODControl.RelayID("K1, K2, K3, K4"),
                    HMOD_Data_18: _hmod18);

                HMODControl.HMOD19to23(tsmContext, HMOD_Data_23: _hmod23);

                HMODControl.HMOD11to13_24to25(tsmContext,
                    hmodData13: _hmod13,
                    hmodData24: _hmod24,
                    hmodData25: _hmod25);

                HMODControl.CHMOD1to6(tsmContext,
                    HMOD_Data_1: _chmod1,
                    HMOD_Data_6: _chmod6);

                Globals.TheHdw.Wait(SettlingTimeSec);
            }

            /// <summary>
            /// Sets or clears the masked bits of a 32-bit relay word.
            /// </summary>
            /// <param name="current">Current relay word.</param>
            /// <param name="mask">Relay bits to change.</param>
            /// <param name="connect">True to close the masked relays, false to open them.</param>
            private static uint Toggle(uint current, uint mask, bool connect)
            {
                return connect ? (current | mask) : (current & ~mask);
            }

            /// <summary>
            /// Sets or clears the masked bits of a 72-bit relay word held as three uints.
            /// </summary>
            /// <param name="current">Current relay words (uint[3]).</param>
            /// <param name="mask">Relay bits to change (uint[3]).</param>
            /// <param name="connect">True to close the masked relays, false to open them.</param>
            private static void Toggle(uint[] current, uint[] mask, bool connect)
            {
                for (int i = 0; i < current.Length; i++)
                {
                    current[i] = Toggle(current[i], mask[i], connect);
                }
            }
        }

        /// <summary>
        /// Per-channel relay descriptors and instrument pin names for one BBAC capture channel.
        /// Relay designators come from the schematics, not from the BBAC Capture Checker block
        /// diagram. The block diagram mnemonics map as follows.
        ///
        /// CH1: KA=HMOD4 K28, KB=HMOD4 K30, KC=HMOD4 K29, KD=HMOD4 K31, KE=HMOD4 K32,
        /// KF=HMOD8 K29, KG=HMOD8 K30, KH=HMOD8 K31, KI=HMOD8 K32, KJ=HMOD10 K29,
        /// KK=HMOD4 K25, KL=HMOD4 K26, KM=HMOD4 K27, KN=HMOD24 K71, KO=HMOD24 K72,
        /// K7=CHMOD1 K13 (source sheet), K3=CHMOD1 K19,
        /// CAP1+/CAP1- to SRC1+/SRC1-=CHMOD6 K6/K7, K1=CHMOD6 K8, K2=CHMOD6 K9, K4=CHMOD6 K10.
        ///
        /// CH2: KA=HMOD10 K31, KB=HMOD13 K23, KC=HMOD10 K32, KD=HMOD13 K24, KE=HMOD13 K25,
        /// KF=HMOD13 K26, KG=HMOD13 K27, KH=HMOD13 K28, KI=HMOD13 K29, KJ=HMOD13 K30,
        /// KK=HMOD4 K25, KL=HMOD4 K26 (PXIe-4147 LO sense shared with CH1), KM=HMOD10 K30,
        /// KN=HMOD25 K71, KO=HMOD25 K72, K7=CHMOD1 K16 (source sheet), K3=CHMOD1 K20,
        /// CAP2+/CAP2- to SRC2+/SRC2-=CHMOD6 K11/K12, K1=CHMOD6 K13, K2=CHMOD6 K14,
        /// K4=CHMOD6 K15.
        ///
        /// Both channels share the PXIe-4147 CH2 / CH3 PPMUs, so KK and KL are identical.
        /// </summary>
        private sealed class BbacCapChannel
        {
            /// <summary>BBAC CAPTURE CH1: PXIe-4467 AO0/AI0, PXIe-4147 CH2/CH3.</summary>
            public static readonly BbacCapChannel Ch1 = new BbacCapChannel
            {
                Prefix = "BBAC_CAP_CH1",

                AcSourcePin = "P122_4467_SRC_AO0",
                AinPin = "P122_4467_SRC_AI0",
                PpmuCapPlusPin = "P103_4147_SMU_CH2",       // PXIe-4147 CH2, per 02-089357 sheet 76
                PpmuCapMinusPin = "P103_4147_SMU_CH3",      // PXIe-4147 CH3, per 02-089357 sheet 76

                SrcKa = new CapRelay(CapRegister.Hmod18, "K29"),
                SrcKb = new CapRelay(CapRegister.Hmod18, "K31"),
                SrcKg = new CapRelay(CapRegister.Hmod18, "K30"),
                SrcKh = new CapRelay(CapRegister.Hmod18, "K32"),

                Ka = new CapRelay(CapRegister.Hmod4, "K28"),
                Kb = new CapRelay(CapRegister.Hmod4, "K30"),
                Kc = new CapRelay(CapRegister.Hmod4, "K29"),
                Kd = new CapRelay(CapRegister.Hmod4, "K31"),
                Ke = new CapRelay(CapRegister.Hmod4, "K32"),
                Kf = new CapRelay(CapRegister.Hmod8, "K29"),
                Kg = new CapRelay(CapRegister.Hmod8, "K30"),
                Kh = new CapRelay(CapRegister.Hmod8, "K31"),
                Ki = new CapRelay(CapRegister.Hmod8, "K32"),
                Kj = new CapRelay(CapRegister.Hmod10, "K29"),
                // HMOD13 K31 releases CC COMMON1 from its AGND star-ground. HMOD4 K25/K26 must not
                // be driven for this channel, or the CH2/CH3 LO sense lines would be pulled to CC
                // COMMON2 while this channel is referenced to CC COMMON1.
                LoChangeover = new CapRelay(CapRegister.Hmod13, "K31"),
                LoSenseTie = null,

                Km = new CapRelay(CapRegister.Hmod4, "K27"),
                Kn = new CapRelay(CapRegister.Hmod24, "K71"),
                Ko = new CapRelay(CapRegister.Hmod24, "K72"),

                K7 = new CapRelay(CapRegister.Chmod1, "K13"),
                K3 = new CapRelay(CapRegister.Chmod1, "K19"),
                CapPlusToSrc = new CapRelay(CapRegister.Chmod6, "K6"),
                CapMinusToSrc = new CapRelay(CapRegister.Chmod6, "K7"),
                K1 = new CapRelay(CapRegister.Chmod6, "K8"),
                K2 = new CapRelay(CapRegister.Chmod6, "K9"),
                K4 = new CapRelay(CapRegister.Chmod6, "K10")
            };

            /// <summary>BBAC CAPTURE CH2: PXIe-4467 AO1/AI1, PXIe-4147 CH2/CH3.</summary>
            public static readonly BbacCapChannel Ch2 = new BbacCapChannel
            {
                Prefix = "BBAC_CAP_CH2",

                AcSourcePin = "P122_4467_SRC_AO1",
                AinPin = "P122_4467_SRC_AI1",
                // Per 02-089357 sheet 77, BBAC CAPTURE CH2 reuses the same PXIe-4147 CH2 and CH3
                // as CH1: CH2_HI reaches ACC_CAP2+ through HMOD13 K25/K26 and CH3_HI reaches
                // ACC_CAP2- through HMOD13 K27/K28, where CH1 instead used HMOD4 K32 / HMOD8 K29
                // and HMOD8 K30/K31. Only one capture channel may be driven at a time.
                PpmuCapPlusPin = "P103_4147_SMU_CH2",
                PpmuCapMinusPin = "P103_4147_SMU_CH3",

                SrcKa = new CapRelay(CapRegister.Hmod23, "K23"),
                SrcKb = new CapRelay(CapRegister.Hmod23, "K28"),
                SrcKg = new CapRelay(CapRegister.Hmod23, "K24"),
                SrcKh = new CapRelay(CapRegister.Hmod23, "K29"),

                Ka = new CapRelay(CapRegister.Hmod10, "K31"),
                Kb = new CapRelay(CapRegister.Hmod13, "K23"),
                Kc = new CapRelay(CapRegister.Hmod10, "K32"),
                Kd = new CapRelay(CapRegister.Hmod13, "K24"),
                Ke = new CapRelay(CapRegister.Hmod13, "K25"),
                Kf = new CapRelay(CapRegister.Hmod13, "K26"),
                Kg = new CapRelay(CapRegister.Hmod13, "K27"),
                Kh = new CapRelay(CapRegister.Hmod13, "K28"),
                Ki = new CapRelay(CapRegister.Hmod13, "K29"),
                Kj = new CapRelay(CapRegister.Hmod13, "K30"),
                // HMOD13 K32 is CC COMMON2's counterpart to CH1's K31. Both are normally closed to
                // AGND, so the relay has to be energized to release the star-ground before the node
                // can be lifted. This channel was originally given CH1's K31, which releases CC
                // COMMON1 instead, leaving CC COMMON2 grounded and DgsRef pinned at the forced +1 V.
                // SL02_BBAC_SRC uses the same pair as its per-channel MaskKq, K31 for CH1 and K32
                // for CH2, and its CcCommon_DGS passes at 1.999 on both channels.
                LoChangeover = new CapRelay(CapRegister.Hmod13, "K32"),

                // HMOD4 K25/K26 COM_A are P103_4147_SMU_CH2_LO_S and CH3_LO_S, with A_NO on CC
                // COMMON2 and A_NC on CC COMMON1 per 02-089357 sheet 76. They are driven for the whole
                // CH2 run: released mid-run the sense lines revert to CC COMMON1 and CH2 measures
                // against the other channel's reference. Both are on HMOD4, so one Apply covers them.
                LoSenseTie = new CapRelay(CapRegister.Hmod4, "K25, K26"),

                Km = new CapRelay(CapRegister.Hmod10, "K30"),
                Kn = new CapRelay(CapRegister.Hmod25, "K71"),
                Ko = new CapRelay(CapRegister.Hmod25, "K72"),

                K7 = new CapRelay(CapRegister.Chmod1, "K16"),
                K3 = new CapRelay(CapRegister.Chmod1, "K20"),
                CapPlusToSrc = new CapRelay(CapRegister.Chmod6, "K11"),
                CapMinusToSrc = new CapRelay(CapRegister.Chmod6, "K12"),
                K1 = new CapRelay(CapRegister.Chmod6, "K13"),
                K2 = new CapRelay(CapRegister.Chmod6, "K14"),
                K4 = new CapRelay(CapRegister.Chmod6, "K15")
            };

            /// <summary>Published data ID prefix for this channel.</summary>
            public string Prefix { get; private set; }

            /// <summary>Pin name of the PXIe-4467 AOFuncGen output used as the AC stimulus.</summary>
            public string AcSourcePin { get; private set; }

            /// <summary>Pin name of the PXIe-4467 differential analog input.</summary>
            public string AinPin { get; private set; }

            /// <summary>Pin name of the PXIe-4147 PPMU wired to CAPx+ / ACC_CAPx+.</summary>
            public string PpmuCapPlusPin { get; private set; }

            /// <summary>Pin name of the PXIe-4147 PPMU wired to CAPx- / ACC_CAPx-.</summary>
            public string PpmuCapMinusPin { get; private set; }

            /// <summary>Source sheet KA: PXIe-4467 AOUT+ onto the SRCx+ network.</summary>
            public CapRelay SrcKa { get; private set; }

            /// <summary>Source sheet KB: SRCx+ network onto the Slot 2 SRCx+ pin.</summary>
            public CapRelay SrcKb { get; private set; }

            /// <summary>Source sheet KG: PXIe-4467 AOUT- onto the SRCx- network.</summary>
            public CapRelay SrcKg { get; private set; }

            /// <summary>Source sheet KH: SRCx- network onto the Slot 2 SRCx- pin.</summary>
            public CapRelay SrcKh { get; private set; }

            /// <summary>KA: PXIe-4467 AIN+ onto the CAPx+ network.</summary>
            public CapRelay Ka { get; private set; }

            /// <summary>KB: CAPx+ network onto the Slot 2 CAPx+ pin.</summary>
            public CapRelay Kb { get; private set; }

            /// <summary>KC: PXIe-4467 AIN- onto the CAPx- network.</summary>
            public CapRelay Kc { get; private set; }

            /// <summary>KD: CAPx- network onto the Slot 2 CAPx- pin.</summary>
            public CapRelay Kd { get; private set; }

            /// <summary>KE: PXIe-4147 CH2 onto the ACC_CAPx+ row.</summary>
            public CapRelay Ke { get; private set; }

            /// <summary>KF: ACC_CAPx+ row onto the Slot 2 ACC_CAPx+ pin.</summary>
            public CapRelay Kf { get; private set; }

            /// <summary>KG: PXIe-4147 CH3 onto the ACC_CAPx- row.</summary>
            public CapRelay Kg { get; private set; }

            /// <summary>KH: ACC_CAPx- row onto the Slot 2 ACC_CAPx- pin.</summary>
            public CapRelay Kh { get; private set; }

            /// <summary>KI: column relay tying the CAPx+ rail to the ACC_CAPx+ row.</summary>
            public CapRelay Ki { get; private set; }

            /// <summary>KJ: column relay tying the CAPx- rail to the ACC_CAPx- row.</summary>
            public CapRelay Kj { get; private set; }

            /// <summary>
            /// KK/KL: the single changeover relay on the PXIe-4147 shared LO. All four 4147
            /// channels tie to one LO node (P103_4147_SMU_CH0-CH3_LO), so one relay serves both
            /// capture channels and both source channels: closed selects CC COMMON1, open selects
            /// CC COMMON2. Both are star grounds. Not exercised by the checker procedure, and it
            /// must be left in its default state so the BBAC source channels are not disturbed.
            /// </summary>
            public CapRelay LoChangeover { get; private set; }

            /// <summary>
            /// Holds this channel's PXIe-4147 CH2/CH3 LO sense lines on CC COMMON2 for the whole run.
            /// HMOD4 K25/K26 rest on CC COMMON1, so CH1 needs no relay and this is null there. Only
            /// the LO force is ganged across CH0-CH3; the sense lines are per channel, and under
            /// DCPowerMeasurementSense.Remote they must stay on this channel's own cc common.
            /// </summary>
            public CapRelay LoSenseTie { get; private set; }

            /// <summary>KM: CC COMMON1 onto DGSCapx.</summary>
            public CapRelay Km { get; private set; }

            /// <summary>KN: ACC_CAPx+ row onto DGSCapx.</summary>
            public CapRelay Kn { get; private set; }

            /// <summary>KO: ACC_CAPx- row onto DGSCapx.</summary>
            public CapRelay Ko { get; private set; }

            /// <summary>
            /// K7 on the BBAC source sheet: SRCx+/- changeover. Closed diverts SRCx+/- onto the
            /// BBAC CAPx test points instead of the bridge rectifier.
            /// </summary>
            public CapRelay K7 { get; private set; }

            /// <summary>
            /// K3: DGSCapx changeover. Open selects AGND, closed the 1.0 V BBAC reference.
            /// </summary>
            public CapRelay K3 { get; private set; }

            /// <summary>Checker Board relay tying CAPx+ to the SRCx+ test point.</summary>
            public CapRelay CapPlusToSrc { get; private set; }

            /// <summary>Checker Board relay tying CAPx- to the SRCx- test point.</summary>
            public CapRelay CapMinusToSrc { get; private set; }

            /// <summary>K1: ACC_CAPx+ onto METER_HI.</summary>
            public CapRelay K1 { get; private set; }

            /// <summary>K2: ACC_CAPx- onto METER_HI.</summary>
            public CapRelay K2 { get; private set; }

            /// <summary>K4: DGSCapx onto METER_LO.</summary>
            public CapRelay K4 { get; private set; }
        }
    }
}
