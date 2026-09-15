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
            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType, ForceMode.ForceCurrent);

            HMODControl.AllHMODReset(tsmContext);

            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // Grounds the METER LO

            // Connect HSD to Checker board HMOD SPI pins
            HMODControl.HMOD14to18(tsmContext, HMOD_Data_14: HMODControl.RelayRange(1, 4));

            var relayCheckLoop = new RelayEntry[]
            {
                new RelayEntry("T_SUPPORT_SL45_UDB64", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K36"))),
                new RelayEntry("T_SUPPORT_SL45_UDB65", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K37"))),
                new RelayEntry("T_SUPPORT_SL45_UDB66", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K38"))),
                new RelayEntry("T_SUPPORT_SL45_UDB67", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K39"))),
                new RelayEntry("T_SUPPORT_SL45_UDB68", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K40"))),
                new RelayEntry("T_SUPPORT_SL45_UDB69", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K41"))),
                new RelayEntry("T_SUPPORT_SL45_UDB70", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K42"))),
                new RelayEntry("T_SUPPORT_SL45_UDB71", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K43"))),
                new RelayEntry("T_SUPPORT_SL45_UDB72", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K44"))),
                new RelayEntry("T_SUPPORT_SL45_UDB73", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K45"))),
                new RelayEntry("T_SUPPORT_SL45_UDB74", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K46"))),
                new RelayEntry("T_SUPPORT_SL45_UDB75", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K47"))),
                new RelayEntry("T_SUPPORT_SL45_UDB76", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K48"))),
                new RelayEntry("T_SUPPORT_SL45_UDB77", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K49"))),
                new RelayEntry("T_SUPPORT_SL45_UDB78", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K50"))),
                new RelayEntry("T_SUPPORT_SL45_UDB79", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K51"))),
                new RelayEntry("T_SUPPORT_SL45_UDB80", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K52"))),
                new RelayEntry("T_SUPPORT_SL45_UDB81", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K53"))),
                new RelayEntry("T_SUPPORT_SL45_UDB82", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K54"))),
                new RelayEntry("T_SUPPORT_SL45_UDB83", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K55"))),
                new RelayEntry("T_SUPPORT_SL45_UDB84", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K56"))),
                new RelayEntry("T_SUPPORT_SL45_UDB85", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K57"))),
                new RelayEntry("T_SUPPORT_SL45_UDB86", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K58"))),
                new RelayEntry("T_SUPPORT_SL45_UDB87", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K59"))),
                new RelayEntry("T_SUPPORT_SL45_UDB88", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K60"))),
                new RelayEntry("T_SUPPORT_SL45_UDB89", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K61"))),
                new RelayEntry("T_SUPPORT_SL45_UDB90", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K62"))),
                new RelayEntry("T_SUPPORT_SL45_UDB91", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K63"))),
                new RelayEntry("T_SUPPORT_SL45_UDB92", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K64"))),
                new RelayEntry("T_SUPPORT_SL45_UDB93", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K65"))),
                new RelayEntry("T_SUPPORT_SL45_UDB94", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K66"))),
                new RelayEntry("T_SUPPORT_SL45_UDB95", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K67"))),
            };

            try
            {
                foreach (var relayCheck in relayCheckLoop)
                {
                    HMODControl.CHMODReset(tsmContext);

                    relayCheck.HmodRelay();
                    Globals.TheHdw.Wait(RelaySettleSec);

                    double[] pulledUp = meter.MeasureVoltage(tsmContext); // 12V

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
                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);

                meter.Cleanup(tsmContext);
                HMODControl.AllHMODReset(tsmContext);
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
