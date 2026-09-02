using System;
using TestSteps.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.ModularInstruments.NIDCPower;
namespace TestSteps.P2Checker
{
    public class SL45_Relay_Check
    {
        private const double RelaySettleSec = 5e-3;
        public static void SL45Check(ISemiconductorModuleContext tsmContext, 
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
                new RelayEntry("T_SUPPORT_SL45_UDB64", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K36"))),
                new RelayEntry("T_SUPPORT_SL45_UDB65", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K37"))),
                new RelayEntry("T_SUPPORT_SL45_UDB66", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K38"))),
                new RelayEntry("T_SUPPORT_SL45_UDB67", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K39"))),
                new RelayEntry("T_SUPPORT_SL45_UDB68", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K40"))),
                new RelayEntry("T_SUPPORT_SL45_UDB69", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K41"))),
                new RelayEntry("T_SUPPORT_SL45_UDB70", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K42"))),
                new RelayEntry("T_SUPPORT_SL45_UDB71", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K43"))),
                new RelayEntry("T_SUPPORT_SL45_UDB72", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K44"))),
                new RelayEntry("T_SUPPORT_SL45_UDB73", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K45"))),
                new RelayEntry("T_SUPPORT_SL45_UDB74", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K46"))),
                new RelayEntry("T_SUPPORT_SL45_UDB75", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K47"))),
                new RelayEntry("T_SUPPORT_SL45_UDB76", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K48"))),
                new RelayEntry("T_SUPPORT_SL45_UDB77", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K49"))),
                new RelayEntry("T_SUPPORT_SL45_UDB78", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K50"))),
                new RelayEntry("T_SUPPORT_SL45_UDB79", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K51"))),
                new RelayEntry("T_SUPPORT_SL45_UDB80", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K52"))),
                new RelayEntry("T_SUPPORT_SL45_UDB81", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K53"))),
                new RelayEntry("T_SUPPORT_SL45_UDB82", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K54"))),
                new RelayEntry("T_SUPPORT_SL45_UDB83", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K55"))),
                new RelayEntry("T_SUPPORT_SL45_UDB84", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K56"))),
                new RelayEntry("T_SUPPORT_SL45_UDB85", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K57"))),
                new RelayEntry("T_SUPPORT_SL45_UDB86", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K58"))),
                new RelayEntry("T_SUPPORT_SL45_UDB87", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K59"))),
                new RelayEntry("T_SUPPORT_SL45_UDB88", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K60"))),
                new RelayEntry("T_SUPPORT_SL45_UDB89", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K61"))),
                new RelayEntry("T_SUPPORT_SL45_UDB90", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K62"))),
                new RelayEntry("T_SUPPORT_SL45_UDB91", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K63"))),
                new RelayEntry("T_SUPPORT_SL45_UDB92", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K64"))),
                new RelayEntry("T_SUPPORT_SL45_UDB93", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K65"))),
                new RelayEntry("T_SUPPORT_SL45_UDB94", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K66"))),
                new RelayEntry("T_SUPPORT_SL45_UDB95", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K67"))),
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
