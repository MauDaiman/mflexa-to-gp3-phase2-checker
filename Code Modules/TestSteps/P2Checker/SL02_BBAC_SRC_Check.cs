using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using TestSteps.Common;

namespace TestSteps.P2Checker
{
    /// <summary>
    /// Slot 2 (BBAC SOURCE) circuit checker for BBAC SOURCE CH1 and CH2.
    /// Tx Board routing per 02-089357 sheets 74 (CH1) and 75 (CH2); Checker Board bridge
    /// rectifier and DIB access network per 02-101092 sheet 53.
    /// </summary>
    public class SL02_BBAC_SRC_Check
    {
        private const double SettlingTimeSec = 25e-3;
        private const double AcSourceFrequencyHz = 1000;
        private const double AcSourceAmplitudeV = 5; // 10 Vpp expressed as zero-to-peak
        private const double SmuCurrentRangeA = 10e-3;
        private const double SmuCurrentLimitA = 10e-3;
        private const double SmuVoltageRangeV = 6;

        /// <summary>
        /// Runs the BBAC SOURCE CH1 checker sequence on the Slot 2 connector.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="senseType">Meter sense mode (Local or Remote).</param>
        /// <param name="meterType">Meter resource selection (0 = PXIe-4137, 1 = PXIe-4081).</param>
        public static void SL02BBACSrcCh1Check(ISemiconductorModuleContext tsmContext,
            DCPowerMeasurementSense senseType,
            int meterType = 0)
        {
            RunChannel(tsmContext, BbacChannel.Ch1, senseType, meterType);
        }

        /// <summary>
        /// Runs the BBAC SOURCE CH2 checker sequence on the Slot 2 connector.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="senseType">Meter sense mode (Local or Remote).</param>
        /// <param name="meterType">Meter resource selection (0 = PXIe-4137, 1 = PXIe-4081).</param>
        public static void SL02BBACSrcCh2Check(ISemiconductorModuleContext tsmContext,
            DCPowerMeasurementSense senseType,
            int meterType = 0)
        {
            RunChannel(tsmContext, BbacChannel.Ch2, senseType, meterType);
        }

        /// <summary>
        /// Executes the full BBAC source checker procedure for one BBAC source channel.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="ch">Per-channel relay and pin constants.</param>
        /// <param name="senseType">Meter sense mode.</param>
        /// <param name="meterType">Meter resource selection.</param>
        private static void RunChannel(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
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
            DCPower smuCommonMode = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.CommonModePin);
            DCPower smuAccMinus = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.AccMinusPin);
            DCPower smuAccRef = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.AccRefPin);
            DCPower smuAccPlus = InstrCtrl.DCPowerPinsToSessions(tsmContext, ch.AccPlusPin);

            ConfigureSmu(smuCommonMode);
            ConfigureSmu(smuAccMinus);
            ConfigureSmu(smuAccRef);
            ConfigureSmu(smuAccPlus);

            var state = new RelayState();

            // K7 stays open so SRCx+/- remain routed through the bridge rectifier.
            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK7, false);

            // KR selects which cc common the shared PXIe-4163 CH18-CH23 LO sense follows, and it is
            // held for the whole channel because every 4163 measurement has to regulate against this
            // channel's own cc common. K22's rest position is CC COMMON1, so CH1 needs it open and
            // its mask is empty, making this a no-op. CH2 needs it closed onto CC COMMON2 from the
            // start: it used to be closed and released inside CheckCcCommonPoint, which runs after
            // CheckDgsSrcReference, so DgsRef_AccSrcRef was measured with the 4163 sense on CH1's cc
            // common and failed while DgsRef_AccSrcN/AccSrcP passed on other SMUs.
            state.TxHmod13 = Toggle(state.TxHmod13, ch.MaskKr, true);

            try
            {
                CheckBridgeRectifierAndPpmu(tsmContext, ch, state, meter, acSource, smuAccMinus, smuAccPlus);
                CheckSrcRefConnectivity(tsmContext, ch, state, meter, acSource);
                CheckCommonMode(tsmContext, ch, state, smuCommonMode, smuAccRef);
                CheckDibAccessSrcRef(tsmContext, ch, state, meter, smuCommonMode);
                CheckDibAccessSrcMinusPlus(tsmContext, ch, state, meter, smuAccMinus, smuAccPlus);
                CheckDgsSrcReference(tsmContext, ch, state, meter, smuAccMinus, smuAccRef, smuAccPlus);
                CheckCcCommonPoint(tsmContext, ch, state, meter, smuAccPlus);
                CheckDgsSrcFunctionality(tsmContext, ch, state, meter);
            }
            finally
            {
                acSource.Stop();

                ShutdownSmu(smuCommonMode);
                ShutdownSmu(smuAccMinus);
                ShutdownSmu(smuAccRef);
                ShutdownSmu(smuAccPlus);

                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);

                meter.Cleanup(tsmContext);
                HMODControl.CHMODReset(tsmContext);
                HMODControl.AllHMODReset(tsmContext);
            }
        }

        /// <summary>
        /// Sources 1 kHz 10 Vpp on SRCx+/- (KA, KB, KG, KH), measures the Checker Board bridge
        /// rectifier output through the 1.6 kHz LPF on METER (K1), then connects SRCx+/- to the
        /// PXIe-4163 PPMU (KC/KK and KV/KI) and measures SRCout+/-.
        /// Checker Board K7 stays open so SRCx+/- remain on the bridge rectifier (K7 normally
        /// closed contact), not on the BBAC CAP test points.
        /// </summary>
        private static void CheckBridgeRectifierAndPpmu(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DAQmx acSource,
            DCPower smuAccMinus,
            DCPower smuAccPlus)
        {
            ch.SetSourceRelays(state, ch.MaskKa | ch.MaskKb | ch.MaskKg | ch.MaskKh, true);
            state.Apply(tsmContext);

            acSource.ConfigureAOFuncGen(
                NationalInstruments.DAQmx.AOFunctionGenerationType.Sine,
                frequency: AcSourceFrequencyHz,
                amplitude: AcSourceAmplitudeV,
                offset: 0);
            acSource.StartAOFuncGen();

            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK1, true); // K1: bridge output to METER_HI/LO
            state.Apply(tsmContext);

            // The rectifier output has to charge before it is read. Measuring straight after
            // StartAOFuncGen made this reading depend purely on instrument timing: the PXIe-4139
            // returned 4.895 V and the PXIe-4081 20.54 V on the same board, the latter over the 15 V
            // limit.
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_BridgeRect_V");

            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK1, false);

            // KV/KI: SRCx- to the PXIe-4163 acc- channel; KC/KK: SRCx+ to the acc+ channel
            ch.SetTx23Relays(state, ch.MaskKv | ch.MaskKc, true);
            ch.SetAccRelays(state, ch.MaskKi, true);
            ch.SetAccRelays(state, ch.MaskKk, true);
            state.Apply(tsmContext);

            // currentLevelRange is passed explicitly because ForceCurrent otherwise derives it from
            // the level, and a 0 A level would ask the driver for a 0 A range.
            smuAccMinus.ForceCurrent(currentLevel: 0, voltageLimit: SmuVoltageRangeV, currentLevelRange: SmuCurrentRangeA);
            smuAccPlus.ForceCurrent(currentLevel: 0, voltageLimit: SmuVoltageRangeV, currentLevelRange: SmuCurrentRangeA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            smuAccMinus.Measure(out double[] srcOutMinus, out _);
            smuAccPlus.Measure(out double[] srcOutPlus, out _);

            smuAccMinus.PinQueryContext.Publish(srcOutMinus, ch.Prefix + "_SrcOutN_V");
            smuAccPlus.PinQueryContext.Publish(srcOutPlus, ch.Prefix + "_SrcOutP_V");

            ch.SetAccRelays(state, ch.MaskKi, false);
            ch.SetAccRelays(state, ch.MaskKk, false);
            ch.SetTx23Relays(state, ch.MaskKv | ch.MaskKc, false);
        }

        /// <summary>
        /// Verifies SRCxREF reaches the AC source circuit: KD and KE close the divider onto
        /// SRCxREF (KF stays open so its normally closed contact selects SRCxREF rather than the
        /// common-mode node), K2 taps SRCxREF onto METER_HI and RL0 grounds METER_LO.
        /// </summary>
        private static void CheckSrcRefConnectivity(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DAQmx acSource)
        {
            ch.SetTx23Relays(state, ch.MaskKd | ch.MaskKe, true);
            ch.SetChmod72Relays(state, ch.MaskK2, true);
            state.Apply(tsmContext);

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // METER_LO to AGND
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_SrcRef_V");

            ch.SetChmod72Relays(state, ch.MaskK2, false);
            ch.SetTx23Relays(state, ch.MaskKd | ch.MaskKe, false);
            ch.SetSourceRelays(state, ch.MaskKa | ch.MaskKb | ch.MaskKg | ch.MaskKh, false);
            state.Apply(tsmContext);

            acSource.Stop(); // PXIe-4467 AOUT back to 0 V
        }

        /// <summary>
        /// Checks the common-mode / DC offset path: KE and KF steer the PXIe-4147 onto the
        /// common-mode node, KS and KT bring the PXIe-4163 ACC_SRCxREF channel onto the same
        /// node, and the +1 V forced by the PXIe-4147 is read back on that channel.
        /// </summary>
        private static void CheckCommonMode(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            DCPower smuCommonMode,
            DCPower smuAccRef)
        {
            ch.SetTx23Relays(state, ch.MaskKe | ch.MaskKf | ch.MaskKs, true);
            ch.SetAccRelays(state, ch.MaskKt, true);
            state.Apply(tsmContext);

            smuCommonMode.ForceVoltage(voltageLevel: 1, currentLimit: SmuCurrentLimitA);
            smuAccRef.ForceCurrent(currentLevel: 0, voltageLimit: SmuVoltageRangeV, currentLevelRange: SmuCurrentRangeA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            smuAccRef.Measure(out double[] commonMode, out _);
            smuAccRef.PinQueryContext.Publish(commonMode, ch.Prefix + "_CommonMode_V");
        }

        /// <summary>
        /// Checks DIB access of ACC_SRCxREF: KU extends the still-energised common-mode node out
        /// to the ACC_SRCxREF pogo and K3 taps it onto METER_HI, so the +1 V forced by the
        /// PXIe-4147 must appear there. Releases the whole common-mode path on exit.
        /// </summary>
        private static void CheckDibAccessSrcRef(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DCPower smuCommonMode)
        {
            ch.SetAccRelays(state, ch.MaskKu, true);
            ch.SetChmod72Relays(state, ch.MaskK3, true);
            state.Apply(tsmContext);

            // KU extends the still-energised common-mode node out to the pin, so let it settle before
            // reading. Without this the value tracked the meter rather than the board: 0.9992 V on the
            // PXIe-4139 against 0.8341 V on the PXIe-4081 for the same channel.
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_AccSrcRef_V");

            ch.SetChmod72Relays(state, ch.MaskK3, false);
            smuCommonMode.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);

            ch.SetAccRelays(state, Or(ch.MaskKt, ch.MaskKu), false);
            ch.SetTx23Relays(state, ch.MaskKe | ch.MaskKf | ch.MaskKs, false);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks DIB access of ACC_SRCx- (KI/KJ, K5) and ACC_SRCx+ (KK/KL, K6): each net is
        /// forced to +1 V by its PXIe-4163 channel and measured on METER_HI.
        /// </summary>
        private static void CheckDibAccessSrcMinusPlus(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DCPower smuAccMinus,
            DCPower smuAccPlus)
        {
            MeasureAccNetOnMeter(tsmContext, ch, state, meter, smuAccMinus,
                Or(ch.MaskKi, ch.MaskKj), ch.MaskK5, forceVoltage: 1, publishId: ch.Prefix + "_AccSrcN_V");

            MeasureAccNetOnMeter(tsmContext, ch, state, meter, smuAccPlus,
                Or(ch.MaskKk, ch.MaskKl), ch.MaskK6, forceVoltage: 1, publishId: ch.Prefix + "_AccSrcP_V");

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);
        }

        /// <summary>
        /// Checks DGSsrc connectivity: K4 ties DGSsrcx to the 1.0 V ADR130 reference and K8
        /// returns METER_LO on DGSsrcx, so each DIB access net forced to +2 V must read +1 V.
        /// </summary>
        private static void CheckDgsSrcReference(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DCPower smuAccMinus,
            DCPower smuAccRef,
            DCPower smuAccPlus)
        {
            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, true);
            ch.SetChmod72Relays(state, ch.MaskK8, true);
            state.Apply(tsmContext);

            MeasureAccNetOnMeter(tsmContext, ch, state, meter, smuAccMinus,
                Or(ch.MaskKi, ch.MaskKj), ch.MaskK5, forceVoltage: 2, publishId: ch.Prefix + "_DgsRef_AccSrcN_V");

            MeasureAccNetOnMeter(tsmContext, ch, state, meter, smuAccRef,
                Or(ch.MaskKt, ch.MaskKu), ch.MaskK3, forceVoltage: 2, publishId: ch.Prefix + "_DgsRef_AccSrcRef_V");

            MeasureAccNetOnMeter(tsmContext, ch, state, meter, smuAccPlus,
                Or(ch.MaskKk, ch.MaskKl), ch.MaskK6, forceVoltage: 2, publishId: ch.Prefix + "_DgsRef_AccSrcP_V");

            ch.SetChmod72Relays(state, ch.MaskK8, false);
            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, false);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks the cc common point: with cc common on AGND, ACC_SRCx+ forced to +1 V reads
        /// +1 V; KQ then moves cc common off AGND, KP ties it to DGSsrcx and K4 references
        /// DGSsrcx to 1.0 V, so the same forced +1 V must read +2 V.
        ///
        /// RL0 grounds METER_LO for the duration. Both readings are referenced to AGND, and the
        /// +1 V to +2 V shift is only meaningful that way: the SMU return moves from 0 V to the
        /// 1.0 V DGSsrcx reference while the meter stays on AGND. K8 is not used here because
        /// returning METER_LO on DGSsrcx would move with cc common and cancel the shift.
        /// </summary>
        private static void CheckCcCommonPoint(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DCPower smuAccPlus)
        {
            ch.SetAccRelays(state, Or(ch.MaskKk, ch.MaskKl), true);
            ch.SetChmod72Relays(state, ch.MaskK6, true);
            state.Apply(tsmContext);

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // METER_LO to AGND

            smuAccPlus.ForceVoltage(voltageLevel: 1, currentLimit: SmuCurrentLimitA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_CcCommon_AGND_V");

            // KR is not touched here. It selects which cc common the shared PXIe-4163 LO sense
            // follows and RunChannel holds it for the whole channel, so the SMU already regulates
            // against this channel's cc common. With the wrong selection the +1 V to +2 V shift
            // cannot appear no matter what KQ, KP and K4 do, and the channel reports the forced
            // +1 V for both CcCommon_AGND and CcCommon_DGS, identical to four digits.
            state.TxHmod13 = Toggle(state.TxHmod13, ch.MaskKq, true); // KQ: cc common off AGND
            ch.SetAccRelays(state, ch.MaskKp, true);                  // KP: cc common to DGSsrcx
            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, true);     // K4: DGSsrcx to 1.0 V
            state.Apply(tsmContext);

            // Four relays move the cc common from AGND up to the 1.0 V reference, so the node has to
            // settle before it is read, exactly as the CcCommon_AGND reading above does.
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), ch.Prefix + "_CcCommon_DGS_V");

            smuAccPlus.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);

            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, false);
            ch.SetAccRelays(state, ch.MaskKp, false);
            state.TxHmod13 = Toggle(state.TxHmod13, ch.MaskKq, false);
            ch.SetChmod72Relays(state, ch.MaskK6, false);
            ch.SetAccRelays(state, Or(ch.MaskKk, ch.MaskKl), false);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Checks DGSsrc drive functionality: with K4 applying the 1.0 V reference to DGSsrcx,
        /// each DIB access net is switched onto DGSsrcx (KM/KJ, KN/KU, KO/KL) and must read the
        /// 1.0 V reference on METER_HI against a grounded METER_LO.
        /// </summary>
        private static void CheckDgsSrcFunctionality(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter)
        {
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // METER_LO to GND

            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, true);

            MeasureDgsNetOnMeter(tsmContext, ch, state, meter,
                Or(ch.MaskKm, ch.MaskKj), ch.MaskK5, ch.Prefix + "_DgsFunc_AccSrcN_V");

            MeasureDgsNetOnMeter(tsmContext, ch, state, meter,
                Or(ch.MaskKn, ch.MaskKu), ch.MaskK3, ch.Prefix + "_DgsFunc_AccSrcRef_V");

            MeasureDgsNetOnMeter(tsmContext, ch, state, meter,
                Or(ch.MaskKo, ch.MaskKl), ch.MaskK6, ch.Prefix + "_DgsFunc_AccSrcP_V");

            state.Chmod1 = Toggle(state.Chmod1, ch.MaskK4, false);
            state.Apply(tsmContext);

            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);
        }

        /// <summary>
        /// Forces a voltage on one DIB access net with its PXIe-4163 channel, taps that net onto
        /// METER_HI, publishes the measurement, then releases the net.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="ch">Per-channel relay and pin constants.</param>
        /// <param name="state">Accumulated relay state.</param>
        /// <param name="meter">Meter strategy in use.</param>
        /// <param name="smu">PXIe-4163 channel wired to the net under test.</param>
        /// <param name="accMask">Tx Board DIB access relays for the net.</param>
        /// <param name="meterMask">Checker Board relay tapping the net onto METER_HI.</param>
        /// <param name="forceVoltage">Voltage forced by the PXIe-4163 channel.</param>
        /// <param name="publishId">Published data ID for the measurement.</param>
        private static void MeasureAccNetOnMeter(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            DCPower smu,
            uint[] accMask,
            uint[] meterMask,
            double forceVoltage,
            string publishId)
        {
            ch.SetAccRelays(state, accMask, true);
            ch.SetChmod72Relays(state, meterMask, true);
            state.Apply(tsmContext);

            smu.ForceVoltage(voltageLevel: forceVoltage, currentLimit: SmuCurrentLimitA);
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), publishId);

            smu.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimitA);

            ch.SetChmod72Relays(state, meterMask, false);
            ch.SetAccRelays(state, accMask, false);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Routes one DIB access net onto DGSsrcx and METER_HI, publishes the measurement, then
        /// releases the net. No instrument forcing is involved.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="ch">Per-channel relay and pin constants.</param>
        /// <param name="state">Accumulated relay state.</param>
        /// <param name="meter">Meter strategy in use.</param>
        /// <param name="accMask">Tx Board relays tying the net to DGSsrcx.</param>
        /// <param name="meterMask">Checker Board relay tapping the net onto METER_HI.</param>
        /// <param name="publishId">Published data ID for the measurement.</param>
        private static void MeasureDgsNetOnMeter(ISemiconductorModuleContext tsmContext,
            BbacChannel ch,
            RelayState state,
            IMeterStrategy meter,
            uint[] accMask,
            uint[] meterMask,
            string publishId)
        {
            ch.SetAccRelays(state, accMask, true);
            ch.SetChmod72Relays(state, meterMask, true);
            state.Apply(tsmContext);

            // Settle before reading, as every other measurement in this step does. No instrument is
            // forcing here, so the net reaches the reference through the relay path alone.
            Globals.TheHdw.Wait(SettlingTimeSec);

            meter.PublishResult(tsmContext, meter.MeasureVoltage(tsmContext), publishId);

            ch.SetChmod72Relays(state, meterMask, false);
            ch.SetAccRelays(state, accMask, false);
            state.Apply(tsmContext);
        }

        /// <summary>
        /// Applies the common SMU configuration used by every BBAC source measurement channel.
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
        /// Sets or clears the masked bits of a 32-bit HMOD relay word.
        /// </summary>
        /// <param name="current">Current relay word.</param>
        /// <param name="mask">Relay bits to change.</param>
        /// <param name="connect">True to close the masked relays, false to open them.</param>
        private static uint Toggle(uint current, uint mask, bool connect)
        {
            return connect ? (current | mask) : (current & ~mask);
        }

        /// <summary>
        /// Combines two 72-bit relay masks into one.
        /// </summary>
        /// <param name="first">First relay mask (uint[3]).</param>
        /// <param name="second">Second relay mask (uint[3]).</param>
        private static uint[] Or(uint[] first, uint[] second)
        {
            uint[] result = new uint[first.Length];
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = first[i] | second[i];
            }

            return result;
        }

        /// <summary>
        /// Sets or clears the masked bits of a 72-bit HMOD relay word held as three uints.
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

        /// <summary>
        /// Accumulated relay state for every HMOD register involved in the BBAC source path.
        /// A single <see cref="Apply"/> call shifts the whole state out so no register latches
        /// stale data from a previous step.
        /// </summary>
        private sealed class RelayState
        {
            /// <summary>Tx Board HMOD13 word: cc common star-ground changeover relays.</summary>
            public uint TxHmod13;

            /// <summary>Tx Board HMOD18 word: BBAC SOURCE CH1 PXIe-4467 routing (K29-K32).</summary>
            public uint TxHmod18;

            /// <summary>
            /// Tx Board HMOD23 word: CH1 common-mode network and column relays (K17-K22) plus the
            /// whole CH2 source and common-mode network (K23-K32).
            /// </summary>
            public uint TxHmod23;

            /// <summary>Tx Board HMOD24 word: BBAC SOURCE CH1 DIB access routing.</summary>
            public uint[] TxHmod24 = new uint[3];

            /// <summary>Tx Board HMOD25 word: BBAC SOURCE CH2 DIB access routing.</summary>
            public uint[] TxHmod25 = new uint[3];

            /// <summary>Checker Board HMOD1 word: bridge meter taps and DGSsrc reference select.</summary>
            public uint Chmod1;

            /// <summary>Checker Board HMOD5 word: BBAC SOURCE CH1 METER_HI/METER_LO taps.</summary>
            public uint[] Chmod5 = new uint[3];

            /// <summary>Checker Board HMOD6 word: BBAC SOURCE CH2 METER_HI/METER_LO taps.</summary>
            public uint[] Chmod6 = new uint[3];

            /// <summary>
            /// Shifts the current relay state into the Tx Board and Checker Board HMOD chains and
            /// waits for the relays to settle. HMOD14 K1-K4 stay closed so the DIO pins remain
            /// connected to the HSD that drives the shift registers.
            /// </summary>
            /// <param name="tsmContext">The semiconductor module context.</param>
            public void Apply(ISemiconductorModuleContext tsmContext)
            {
                HMODControl.HMOD14to18(tsmContext,
                    HMOD_Data_14: HMODControl.RelayID("K1, K2, K3, K4"),
                    HMOD_Data_18: TxHmod18);

                HMODControl.HMOD19to23(tsmContext, HMOD_Data_23: TxHmod23);

                HMODControl.HMOD11to13_24to25(tsmContext,
                    hmodData13: TxHmod13,
                    hmodData24: TxHmod24,
                    hmodData25: TxHmod25);

                HMODControl.CHMOD1to6(tsmContext,
                    HMOD_Data_1: Chmod1,
                    HMOD_Data_5: Chmod5,
                    HMOD_Data_6: Chmod6);

                Globals.TheHdw.Wait(SettlingTimeSec);
            }
        }

        /// <summary>
        /// Per-channel relay masks and instrument pin names for one BBAC source channel.
        /// Relay designators come from the schematics, not from the block diagram: Tx Board
        /// 02-089357 sheet 74 (CH1) and sheet 75 (CH2), Checker Board 02-101092 sheet 53.
        /// The mnemonic names used by the BBAC Source Checker block diagram map as follows.
        ///
        /// CH1: KA=HMOD18 K29, KB=HMOD18 K31, KG=HMOD18 K30, KH=HMOD18 K32,
        /// KD=HMOD23 K17, KE=HMOD23 K18, KF=HMOD23 K19, KV=HMOD23 K20, KS=HMOD23 K21,
        /// KC=HMOD23 K22, KI=HMOD24 K65, KJ=HMOD24 K66, KT=HMOD24 K67, KU=HMOD24 K68,
        /// KK=HMOD24 K69, KL=HMOD24 K70, KM=HMOD24 K61, KN=HMOD24 K62, KO=HMOD24 K63,
        /// KP=HMOD24 K64, KQ=HMOD13 K31, KR=HMOD13 K22,
        /// K1=CHMOD1 K15, K7=CHMOD1 K13, K4=CHMOD1 K14,
        /// K2=CHMOD5 K68, K5=CHMOD5 K69, K3=CHMOD5 K70, K6=CHMOD5 K71, K8=CHMOD5 K72.
        ///
        /// CH2: KA=HMOD23 K23, KB=HMOD23 K28, KG=HMOD23 K24, KH=HMOD23 K29,
        /// KD=HMOD23 K25, KE=HMOD23 K26, KF=HMOD23 K27, KV=HMOD23 K30, KS=HMOD23 K31,
        /// KC=HMOD23 K32, KI..KP=HMOD25 K65/K66/K67/K68/K69/K70/K61/K62/K63/K64,
        /// KQ=HMOD13 K32, KR=HMOD13 K22,
        /// K1=CHMOD1 K18, K7=CHMOD1 K16, K4=CHMOD1 K17,
        /// K2=CHMOD6 K1, K5=CHMOD6 K2, K3=CHMOD6 K3, K6=CHMOD6 K4, K8=CHMOD6 K5.
        /// </summary>
        private sealed class BbacChannel
        {
            /// <summary>BBAC SOURCE CH1: PXIe-4467 AO0, PXIe-4147 CH0, PXIe-4163 CH18/19/20.</summary>
            public static readonly BbacChannel Ch1 = new BbacChannel
            {
                Prefix = "BBAC_CH1",

                AcSourcePin = "P122_4467_SRC_AO0",
                CommonModePin = "P103_4147_SMU_CH0",        // PXIe-4147 CH0
                AccMinusPin = "P138_4163_SMU_CH18",
                AccRefPin = "P138_4163_SMU_CH19",
                AccPlusPin = "P138_4163_SMU_CH20",

                SourceOnHmod23 = false,
                MaskKa = HMODControl.RelayID("K29"),
                MaskKb = HMODControl.RelayID("K31"),
                MaskKg = HMODControl.RelayID("K30"),
                MaskKh = HMODControl.RelayID("K32"),

                MaskKd = HMODControl.RelayID("K17"),
                MaskKe = HMODControl.RelayID("K18"),
                MaskKf = HMODControl.RelayID("K19"),
                MaskKv = HMODControl.RelayID("K20"),
                MaskKs = HMODControl.RelayID("K21"),
                MaskKc = HMODControl.RelayID("K22"),

                AccOnHmod25 = false,
                MaskKi = HMODControl.RelayID72("K65"),
                MaskKj = HMODControl.RelayID72("K66"),
                MaskKt = HMODControl.RelayID72("K67"),
                MaskKu = HMODControl.RelayID72("K68"),
                MaskKk = HMODControl.RelayID72("K69"),
                MaskKl = HMODControl.RelayID72("K70"),
                MaskKm = HMODControl.RelayID72("K61"),
                MaskKn = HMODControl.RelayID72("K62"),
                MaskKo = HMODControl.RelayID72("K63"),
                MaskKp = HMODControl.RelayID72("K64"),

                MaskKq = HMODControl.RelayID("K31"),

                // HMOD13 K22 is a CH1/CH2 selector on the shared PXIe-4163 LO sense
                // (P138_4163_SMU_CH18_CH23_LO_S), not an AGND/CC COMMON2 changeover as its
                // original comment said: open routes the sense to CC COMMON1, closed to CC
                // COMMON2. CH1 therefore needs it left open, so its mask is empty and the
                // close/release in CheckCcCommonPoint is a no-op. Driving K22 for CH1 moved the
                // lift to CH2: CcCommon_DGS read 1.999 on CH2 and 0.9999 on CH1, an exact swap.
                MaskKr = 0,

                ChmodOn6 = false,
                MaskK1 = HMODControl.RelayID("K15"),
                MaskK4 = HMODControl.RelayID("K14"),
                MaskK7 = HMODControl.RelayID("K13"),
                MaskK2 = HMODControl.RelayID72("K68"),
                MaskK5 = HMODControl.RelayID72("K69"),
                MaskK3 = HMODControl.RelayID72("K70"),
                MaskK6 = HMODControl.RelayID72("K71"),
                MaskK8 = HMODControl.RelayID72("K72")
            };

            /// <summary>BBAC SOURCE CH2: PXIe-4467 AO1, PXIe-4147 CH1, PXIe-4163 CH21/22/23.</summary>
            public static readonly BbacChannel Ch2 = new BbacChannel
            {
                Prefix = "BBAC_CH2",

                AcSourcePin = "P122_4467_SRC_AO1",
                CommonModePin = "P103_4147_SMU_CH1",        // PXIe-4147 CH1
                AccMinusPin = "P138_4163_SMU_CH21",
                AccRefPin = "P138_4163_SMU_CH22",
                AccPlusPin = "P138_4163_SMU_CH23",

                SourceOnHmod23 = true,
                MaskKa = HMODControl.RelayID("K23"),
                MaskKb = HMODControl.RelayID("K28"),
                MaskKg = HMODControl.RelayID("K24"),
                MaskKh = HMODControl.RelayID("K29"),

                MaskKd = HMODControl.RelayID("K25"),
                MaskKe = HMODControl.RelayID("K26"),
                MaskKf = HMODControl.RelayID("K27"),
                MaskKv = HMODControl.RelayID("K30"),
                MaskKs = HMODControl.RelayID("K31"),
                MaskKc = HMODControl.RelayID("K32"),

                AccOnHmod25 = true,
                MaskKi = HMODControl.RelayID72("K65"),
                MaskKj = HMODControl.RelayID72("K66"),
                MaskKt = HMODControl.RelayID72("K67"),
                MaskKu = HMODControl.RelayID72("K68"),
                MaskKk = HMODControl.RelayID72("K69"),
                MaskKl = HMODControl.RelayID72("K70"),
                MaskKm = HMODControl.RelayID72("K61"),
                MaskKn = HMODControl.RelayID72("K62"),
                MaskKo = HMODControl.RelayID72("K63"),
                MaskKp = HMODControl.RelayID72("K64"),

                // K32 is CH2's own cc common changeover. Driving CH1's K31 here instead was tried
                // and made no difference: CH1 read 1.999 V while CH2 stayed at 1.008 V, so cc
                // common is not a shared node and CH2's 1 V shortfall originates elsewhere,
                // most likely MaskKp or the K4 tie to DGSsrc2.
                MaskKq = HMODControl.RelayID("K32"),
                MaskKr = HMODControl.RelayID("K22"),

                ChmodOn6 = true,
                MaskK1 = HMODControl.RelayID("K18"),
                MaskK4 = HMODControl.RelayID("K17"),
                MaskK7 = HMODControl.RelayID("K16"),
                MaskK2 = HMODControl.RelayID72("K1"),
                MaskK5 = HMODControl.RelayID72("K2"),
                MaskK3 = HMODControl.RelayID72("K3"),
                MaskK6 = HMODControl.RelayID72("K4"),
                MaskK8 = HMODControl.RelayID72("K5")
            };

            /// <summary>Published data ID prefix for this channel.</summary>
            public string Prefix { get; private set; }

            /// <summary>Pin name of the PXIe-4467 AOFuncGen output used as the AC source.</summary>
            public string AcSourcePin { get; private set; }

            /// <summary>Pin name of the PXIe-4147 channel driving the common-mode / DC offset node.</summary>
            public string CommonModePin { get; private set; }

            /// <summary>Pin name of the PXIe-4163 channel wired to ACC_SRCx-.</summary>
            public string AccMinusPin { get; private set; }

            /// <summary>Pin name of the PXIe-4163 channel wired to ACC_SRCxREF.</summary>
            public string AccRefPin { get; private set; }

            /// <summary>Pin name of the PXIe-4163 channel wired to ACC_SRCx+.</summary>
            public string AccPlusPin { get; private set; }

            /// <summary>True when the PXIe-4467 source relays live on HMOD23 instead of HMOD18.</summary>
            public bool SourceOnHmod23 { get; private set; }

            /// <summary>KA: PXIe-4467 AOUT+ onto the SRCx+ network.</summary>
            public uint MaskKa { get; private set; }

            /// <summary>KB: SRCx+ network onto the Slot 2 SRCx+ pin through the 499 R / 3300 pF LPF.</summary>
            public uint MaskKb { get; private set; }

            /// <summary>KG: PXIe-4467 AOUT- onto the SRCx- network.</summary>
            public uint MaskKg { get; private set; }

            /// <summary>KH: SRCx- network onto the Slot 2 SRCx- pin through the 499 R / 3300 pF LPF.</summary>
            public uint MaskKh { get; private set; }

            /// <summary>KD: 49.9 R divider onto the SRCxREF branch.</summary>
            public uint MaskKd { get; private set; }

            /// <summary>KE: second series relay of the SRCxREF / common-mode branch.</summary>
            public uint MaskKe { get; private set; }

            /// <summary>
            /// KF: changeover selecting COMMON MODE/DC OFFSET SRCx when closed, SRCxREF when open.
            /// </summary>
            public uint MaskKf { get; private set; }

            /// <summary>KV: column relay tying the SRCx- rail to the ACC_SRCx- measure row.</summary>
            public uint MaskKv { get; private set; }

            /// <summary>KS: column relay tying the SRCxREF / common-mode rail to the ACC_SRCxREF row.</summary>
            public uint MaskKs { get; private set; }

            /// <summary>KC: column relay tying the SRCx+ rail to the ACC_SRCx+ measure row.</summary>
            public uint MaskKc { get; private set; }

            /// <summary>True when the DIB access relays live on HMOD25 instead of HMOD24.</summary>
            public bool AccOnHmod25 { get; private set; }

            /// <summary>KI: PXIe-4163 force/sense onto the ACC_SRCx- row.</summary>
            public uint[] MaskKi { get; private set; }

            /// <summary>KJ: ACC_SRCx- row onto the Slot 2 ACC_SRCx- pin.</summary>
            public uint[] MaskKj { get; private set; }

            /// <summary>KT: PXIe-4163 force/sense onto the ACC_SRCxREF row.</summary>
            public uint[] MaskKt { get; private set; }

            /// <summary>KU: ACC_SRCxREF row onto the Slot 2 ACC_SRCxREF pin.</summary>
            public uint[] MaskKu { get; private set; }

            /// <summary>KK: PXIe-4163 force/sense onto the ACC_SRCx+ row.</summary>
            public uint[] MaskKk { get; private set; }

            /// <summary>KL: ACC_SRCx+ row onto the Slot 2 ACC_SRCx+ pin.</summary>
            public uint[] MaskKl { get; private set; }

            /// <summary>KM: ACC_SRCx- row onto DGS_SRCx.</summary>
            public uint[] MaskKm { get; private set; }

            /// <summary>KN: ACC_SRCxREF row onto DGS_SRCx.</summary>
            public uint[] MaskKn { get; private set; }

            /// <summary>KO: ACC_SRCx+ row onto DGS_SRCx.</summary>
            public uint[] MaskKo { get; private set; }

            /// <summary>KP: cc common onto DGS_SRCx.</summary>
            public uint[] MaskKp { get; private set; }

            /// <summary>
            /// KQ: PXIe-4147 LO changeover on HMOD13. Open selects AGND, closed selects cc common.
            /// </summary>
            public uint MaskKq { get; private set; }

            /// <summary>
            /// KR: PXIe-4163 LO sense changeover on HMOD13. Open selects AGND, closed CC COMMON2.
            /// </summary>
            public uint MaskKr { get; private set; }

            /// <summary>True when the Checker Board taps live on CHMOD6 instead of CHMOD5.</summary>
            public bool ChmodOn6 { get; private set; }

            /// <summary>K1: bridge rectifier output onto METER_HI / METER_LO.</summary>
            public uint MaskK1 { get; private set; }

            /// <summary>K4: DGS_SRCx changeover. Open selects AGND, closed the 1.0 V BBAC reference.</summary>
            public uint MaskK4 { get; private set; }

            /// <summary>
            /// K7: SRCx+/- changeover. Open routes them through the bridge rectifier (normally
            /// closed contact); closed diverts them to the BBAC CAPx test points.
            /// </summary>
            public uint MaskK7 { get; private set; }

            /// <summary>K2: SRCxREF onto METER_HI.</summary>
            public uint[] MaskK2 { get; private set; }

            /// <summary>K5: ACC_SRCx- onto METER_HI.</summary>
            public uint[] MaskK5 { get; private set; }

            /// <summary>K3: ACC_SRCxREF onto METER_HI.</summary>
            public uint[] MaskK3 { get; private set; }

            /// <summary>K6: ACC_SRCx+ onto METER_HI.</summary>
            public uint[] MaskK6 { get; private set; }

            /// <summary>K8: DGS_SRCx onto METER_LO.</summary>
            public uint[] MaskK8 { get; private set; }

            /// <summary>
            /// Sets or clears PXIe-4467 source relays on whichever Tx Board HMOD carries them.
            /// </summary>
            /// <param name="state">Relay state to update.</param>
            /// <param name="mask">Relay bits to change.</param>
            /// <param name="connect">True to close, false to open.</param>
            public void SetSourceRelays(RelayState state, uint mask, bool connect)
            {
                if (SourceOnHmod23)
                {
                    state.TxHmod23 = Toggle(state.TxHmod23, mask, connect);
                }
                else
                {
                    state.TxHmod18 = Toggle(state.TxHmod18, mask, connect);
                }
            }

            /// <summary>
            /// Sets or clears common-mode / column relays on Tx Board HMOD23.
            /// </summary>
            /// <param name="state">Relay state to update.</param>
            /// <param name="mask">Relay bits to change.</param>
            /// <param name="connect">True to close, false to open.</param>
            public void SetTx23Relays(RelayState state, uint mask, bool connect)
            {
                state.TxHmod23 = Toggle(state.TxHmod23, mask, connect);
            }

            /// <summary>
            /// Sets or clears DIB access relays on Tx Board HMOD24 or HMOD25.
            /// </summary>
            /// <param name="state">Relay state to update.</param>
            /// <param name="mask">Relay bits to change.</param>
            /// <param name="connect">True to close, false to open.</param>
            public void SetAccRelays(RelayState state, uint[] mask, bool connect)
            {
                Toggle(AccOnHmod25 ? state.TxHmod25 : state.TxHmod24, mask, connect);
            }

            /// <summary>
            /// Sets or clears Checker Board meter tap relays on CHMOD5 or CHMOD6.
            /// </summary>
            /// <param name="state">Relay state to update.</param>
            /// <param name="mask">Relay bits to change.</param>
            /// <param name="connect">True to close, false to open.</param>
            public void SetChmod72Relays(RelayState state, uint[] mask, bool connect)
            {
                Toggle(ChmodOn6 ? state.Chmod6 : state.Chmod5, mask, connect);
            }
        }
    }
}
