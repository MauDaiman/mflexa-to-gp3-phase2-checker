using System;
using TestSteps.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.ModularInstruments.NIDCPower;
namespace TestSteps.P2Checker
{
    public class SL33_Relay_Check
    {
        private const double RelaySettleSec = 5e-3;
        public static void SL33Check(ISemiconductorModuleContext tsmContext, 
            DCPowerMeasurementSense senseType, 
            int meterType = 0)
        {
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // Grounds the METER LO

            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType);

            var relayCheckLoop = new RelayEntry[]
            {
                new RelayEntry("T_SUPPORT_SL33_UDB96", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K4"))),
                new RelayEntry("T_SUPPORT_SL33_UDB97", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K5"))),
                new RelayEntry("T_SUPPORT_SL33_UDB98", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K6"))),
                new RelayEntry("T_SUPPORT_SL33_UDB99", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K7"))),
                new RelayEntry("T_SUPPORT_SL33_UDB100", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K8"))),
                new RelayEntry("T_SUPPORT_SL33_UDB101", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K9"))),
                new RelayEntry("T_SUPPORT_SL33_UDB102", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K10"))),
                new RelayEntry("T_SUPPORT_SL33_UDB103", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K11"))),
                new RelayEntry("T_SUPPORT_SL33_UDB104", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K12"))),
                new RelayEntry("T_SUPPORT_SL33_UDB105", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K13"))),
                new RelayEntry("T_SUPPORT_SL33_UDB106", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K14"))),
                new RelayEntry("T_SUPPORT_SL33_UDB107", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K15"))),
                new RelayEntry("T_SUPPORT_SL33_UDB108", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K16"))),
                new RelayEntry("T_SUPPORT_SL33_UDB109", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K17"))),
                new RelayEntry("T_SUPPORT_SL33_UDB110", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K18"))),
                new RelayEntry("T_SUPPORT_SL33_UDB111", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K19"))),
                new RelayEntry("T_SUPPORT_SL33_UDB112", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K20"))),
                new RelayEntry("T_SUPPORT_SL33_UDB113", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K21"))),
                new RelayEntry("T_SUPPORT_SL33_UDB114", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K22"))),
                new RelayEntry("T_SUPPORT_SL33_UDB115", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K23"))),
                new RelayEntry("T_SUPPORT_SL33_UDB116", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K24"))),
                new RelayEntry("T_SUPPORT_SL33_UDB117", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K25"))),
                new RelayEntry("T_SUPPORT_SL33_UDB118", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K26"))),
                new RelayEntry("T_SUPPORT_SL33_UDB119", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K27"))),
                new RelayEntry("T_SUPPORT_SL33_UDB120", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K28"))),
                new RelayEntry("T_SUPPORT_SL33_UDB121", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K29"))),
                new RelayEntry("T_SUPPORT_SL33_UDB122", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K30"))),
                new RelayEntry("T_SUPPORT_SL33_UDB123", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K31"))),
                new RelayEntry("T_SUPPORT_SL33_UDB124", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K32"))),
                new RelayEntry("T_SUPPORT_SL33_UDB125", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K33"))),
                new RelayEntry("T_SUPPORT_SL33_UDB126", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K34"))),
                new RelayEntry("T_SUPPORT_SL33_UDB127", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K35"))),
            };

            try
            {
                foreach (var relayCheck in relayCheckLoop)
                {
                    HMODControl.CHMODReset(tsmContext);
                    Globals.TheHdw.Wait(RelaySettleSec);

                    double[] pulledUp = meter.MeasureVoltage(tsmContext); // 12V

                    relayCheck.HmodRelay();
                    Relay.ControlRelay(tsmContext, relayCheck.RelayID, true);
                    Globals.TheHdw.Wait(RelaySettleSec);

                    double[] pulledDown = meter.MeasureVoltage(tsmContext); // 0V
                    Relay.ControlRelay(tsmContext, relayCheck.RelayID, false);

                    meter.PublishResult(tsmContext, pulledUp, relayCheck.RelayID + "_Pull_Up");
                    meter.PublishResult(tsmContext, pulledDown, relayCheck.RelayID + "_Pull_Down");
                }
            }
            finally
            {
                meter.Cleanup(tsmContext);
                HMODControl.AllHMODReset(tsmContext);
                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);
            }
        }
        private sealed class RelayEntry
        {
            public string RelayID { get; }
            public Action HmodRelay { get; }
            public RelayEntry(string relayID, Action hmodRelay)
            {
                RelayID = relayID;
                HmodRelay = hmodRelay;
            }
        }
    }
}
