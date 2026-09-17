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
        private const double RelaySettleSec = 15e-3;
        public static void SL33Check(ISemiconductorModuleContext tsmContext, 
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
                new RelayEntry("T_SUPPORT_SL33_UDB96", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K4"))),
                new RelayEntry("T_SUPPORT_SL33_UDB97", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K5"))),
                new RelayEntry("T_SUPPORT_SL33_UDB98", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K6"))),
                new RelayEntry("T_SUPPORT_SL33_UDB99", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K7"))),
                new RelayEntry("T_SUPPORT_SL33_UDB100", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K8"))),
                new RelayEntry("T_SUPPORT_SL33_UDB101", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K9"))),
                new RelayEntry("T_SUPPORT_SL33_UDB102", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K10"))),
                new RelayEntry("T_SUPPORT_SL33_UDB103", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K11"))),
                new RelayEntry("T_SUPPORT_SL33_UDB104", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K12"))),
                new RelayEntry("T_SUPPORT_SL33_UDB105", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K13"))),
                new RelayEntry("T_SUPPORT_SL33_UDB106", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K14"))),
                new RelayEntry("T_SUPPORT_SL33_UDB107", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K15"))),
                new RelayEntry("T_SUPPORT_SL33_UDB108", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K16"))),
                new RelayEntry("T_SUPPORT_SL33_UDB109", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K17"))),
                new RelayEntry("T_SUPPORT_SL33_UDB110", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K18"))),
                new RelayEntry("T_SUPPORT_SL33_UDB111", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K19"))),
                new RelayEntry("T_SUPPORT_SL33_UDB112", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K20"))),
                new RelayEntry("T_SUPPORT_SL33_UDB113", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K21"))),
                new RelayEntry("T_SUPPORT_SL33_UDB114", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K22"))),
                new RelayEntry("T_SUPPORT_SL33_UDB115", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K23"))),
                new RelayEntry("T_SUPPORT_SL33_UDB116", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K24"))),
                new RelayEntry("T_SUPPORT_SL33_UDB117", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K25"))),
                new RelayEntry("T_SUPPORT_SL33_UDB118", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K26"))),
                new RelayEntry("T_SUPPORT_SL33_UDB119", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K27"))),
                new RelayEntry("T_SUPPORT_SL33_UDB120", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K28"))),
                new RelayEntry("T_SUPPORT_SL33_UDB121", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K29"))),
                new RelayEntry("T_SUPPORT_SL33_UDB122", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K30"))),
                new RelayEntry("T_SUPPORT_SL33_UDB123", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K31"))),
                new RelayEntry("T_SUPPORT_SL33_UDB124", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K32"))),
                new RelayEntry("T_SUPPORT_SL33_UDB125", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K33"))),
                new RelayEntry("T_SUPPORT_SL33_UDB126", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K34"))),
                new RelayEntry("T_SUPPORT_SL33_UDB127", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K35"))),
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
