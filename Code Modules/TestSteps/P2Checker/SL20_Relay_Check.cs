using System;
using TestSteps.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class SL20_Relay_Check
    {
        private const double RelaySettleSec = 15e-3;
        public static void SL20Check(ISemiconductorModuleContext tsmContext, 
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
                new RelayEntry("T_SUPPORT_SL20_UDB32", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K44"))),
                new RelayEntry("T_SUPPORT_SL20_UDB33", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K45"))),
                new RelayEntry("T_SUPPORT_SL20_UDB34", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K46"))),
                new RelayEntry("T_SUPPORT_SL20_UDB35", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K47"))),
                new RelayEntry("T_SUPPORT_SL20_UDB36", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K48"))),
                new RelayEntry("T_SUPPORT_SL20_UDB37", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K49"))),
                new RelayEntry("T_SUPPORT_SL20_UDB38", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K50"))),
                new RelayEntry("T_SUPPORT_SL20_UDB39", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K51"))),
                new RelayEntry("T_SUPPORT_SL20_UDB40", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K52"))),
                new RelayEntry("T_SUPPORT_SL20_UDB41", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K53"))),
                new RelayEntry("T_SUPPORT_SL20_UDB42", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K54"))),
                new RelayEntry("T_SUPPORT_SL20_UDB43", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K55"))),
                new RelayEntry("T_SUPPORT_SL20_UDB44", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K56"))),
                new RelayEntry("T_SUPPORT_SL20_UDB45", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K57"))),
                new RelayEntry("T_SUPPORT_SL20_UDB46", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K58"))),
                new RelayEntry("T_SUPPORT_SL20_UDB47", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K59"))),
                new RelayEntry("T_SUPPORT_SL20_UDB48", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K60"))),
                new RelayEntry("T_SUPPORT_SL20_UDB49", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K61"))),
                new RelayEntry("T_SUPPORT_SL20_UDB50", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K62"))),
                new RelayEntry("T_SUPPORT_SL20_UDB51", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K63"))),
                new RelayEntry("T_SUPPORT_SL20_UDB52", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K64"))),
                new RelayEntry("T_SUPPORT_SL20_UDB53", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K65"))),
                new RelayEntry("T_SUPPORT_SL20_UDB54", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K66"))),
                new RelayEntry("T_SUPPORT_SL20_UDB55", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K67"))),
                new RelayEntry("T_SUPPORT_SL20_UDB56", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K68"))),
                new RelayEntry("T_SUPPORT_SL20_UDB57", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K69"))),
                new RelayEntry("T_SUPPORT_SL20_UDB58", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K70"))),
                new RelayEntry("T_SUPPORT_SL20_UDB59", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K71"))),
                new RelayEntry("T_SUPPORT_SL20_UDB60", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K72"))),
                new RelayEntry("T_SUPPORT_SL20_UDB61", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K1"))),
                new RelayEntry("T_SUPPORT_SL20_UDB62", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K2"))),
                new RelayEntry("T_SUPPORT_SL20_UDB63", () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_5: HMODControl.RelayID72("K3"))),
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
