using System;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps.P1Checker
{
    public class HMOD_Relay_Check
    {
        private const string DmmPin = "P143_4081_DMM";
        private const double DmmApertureSeconds = 1e-3;
        private const double DmmVoltageRange = 100.0;
        private const double RelaySettleSec = 5e-3;
        private const double LoadSettleSec = 10e-3;

        public static void HMODCheck(ISemiconductorModuleContext tsmContext)
        {
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmPin);

            dmm.Abort();
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                DmmApertureSeconds, DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: DmmVoltageRange);
            dmm.Initiate();

            try
            {
                HMODControl.HMOD14to18(tsmContext, HMOD_Data_18: HMODControl.RelayID("K23"));
                Globals.TheHdw.Wait(RelaySettleSec);

                var hmodGroups = new (string name, Action setRelays)[]
                {
                    ("HMOD1", () => HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: 0xFFFFFFFF)),
                    ("HMOD2", () => HMODControl.HMOD1to4(tsmContext, HMOD_Data_2: 0xFFFFFFFF)),
                    ("HMOD3", () => HMODControl.HMOD1to4(tsmContext, HMOD_Data_3: 0xFFFFFFFF)),
                    ("HMOD4", () => HMODControl.HMOD1to4(tsmContext, HMOD_Data_4: 0xFFFFFFFF)),
                    ("HMOD5", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_5: 0xFFFFFFFF)),
                    ("HMOD6", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_6: 0xFFFFFFFF)),
                    ("HMOD7", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_7: 0xFFFFFFFF)),
                    ("HMOD8", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_8: 0xFFFFFFFF)),
                    ("HMOD9", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_9: 0xFFFFFFFF)),
                    ("HMOD10", () => HMODControl.HMOD5to10(tsmContext, HMOD_Data_10: 0xFFFFFFFF)),
                    ("HMOD11", () => HMODControl.HMOD11to13(tsmContext, HMOD_Data_11: 0xFFFFFFFF)),
                    ("HMOD12", () => HMODControl.HMOD11to13(tsmContext, HMOD_Data_12: 0xFFFFFFFF)),
                    ("HMOD13", () => HMODControl.HMOD11to13(tsmContext, HMOD_Data_13: 0xFFFFFFFF)),
                    ("HMOD14", () => HMODControl.HMOD14to18(tsmContext, HMOD_Data_14: 0xFFFFFFFF, HMOD_Data_18: HMODControl.RelayID("K23"))),
                    ("HMOD15", () => HMODControl.HMOD14to18(tsmContext, HMOD_Data_15: 0xFFFFFFFF, HMOD_Data_18: HMODControl.RelayID("K23"))),
                    ("HMOD16", () => HMODControl.HMOD14to18(tsmContext, HMOD_Data_15: 0xFFFFFFFF, HMOD_Data_18: HMODControl.RelayID("K23"))),
                    ("HMOD17", () => HMODControl.HMOD14to18(tsmContext, HMOD_Data_15: 0xFFFFFFFF, HMOD_Data_18: HMODControl.RelayID("K23"))),
                    ("HMOD19", () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_19: 0xFFFFFFFF)),
                    ("HMOD20", () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: 0xFFFFFFFF)),
                    ("HMOD21", () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: 0xFFFFFFFF)),
                    ("HMOD22", () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_22: 0xFFFFFFFF)),
                    ("HMOD23", () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_23: 0xFFFFFFFF))
                };

                foreach (var (name, setRelays) in hmodGroups)
                {
                    double[] baseline = dmm.Read();

                    setRelays();
                    Globals.TheHdw.Wait(LoadSettleSec);
                    double[] loaded = dmm.Read();

                    HMODControl.THMODReset(tsmContext);
                    HMODControl.HMOD14to18(tsmContext, HMOD_Data_18: HMODControl.RelayID("K23"));
                    Globals.TheHdw.Wait(RelaySettleSec);

                    dmm.PinQueryContext.Publish(new double[] { baseline[0] }, $"{name}_Baseline");
                    dmm.PinQueryContext.Publish(new double[] { loaded[0] }, $"{name}_Loaded");
                    dmm.PinQueryContext.Publish(new double[] { baseline[0] - loaded[0] }, $"{name}_Droop");
                }
            }
            finally
            {
                HMODControl.THMODReset(tsmContext);
                dmm.Abort();
            }
        }
    }
}
