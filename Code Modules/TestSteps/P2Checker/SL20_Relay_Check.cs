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
        private const double RelaySettleSec = 5e-3;
        public static void SL20Check(ISemiconductorModuleContext tsmContext, 
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
                new RelayEntry("T_SUPPORT_SL20_UDB32", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K44"))),
                new RelayEntry("T_SUPPORT_SL20_UDB33", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K45"))),
                new RelayEntry("T_SUPPORT_SL20_UDB34", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K46"))),
                new RelayEntry("T_SUPPORT_SL20_UDB35", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K47"))),
                new RelayEntry("T_SUPPORT_SL20_UDB36", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K48"))),
                new RelayEntry("T_SUPPORT_SL20_UDB37", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K49"))),
                new RelayEntry("T_SUPPORT_SL20_UDB38", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K50"))),
                new RelayEntry("T_SUPPORT_SL20_UDB39", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K51"))),
                new RelayEntry("T_SUPPORT_SL20_UDB40", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K52"))),
                new RelayEntry("T_SUPPORT_SL20_UDB41", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K53"))),
                new RelayEntry("T_SUPPORT_SL20_UDB42", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K54"))),
                new RelayEntry("T_SUPPORT_SL20_UDB43", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K55"))),
                new RelayEntry("T_SUPPORT_SL20_UDB44", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K56"))),
                new RelayEntry("T_SUPPORT_SL20_UDB45", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K57"))),
                new RelayEntry("T_SUPPORT_SL20_UDB46", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K58"))),
                new RelayEntry("T_SUPPORT_SL20_UDB47", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K59"))),
                new RelayEntry("T_SUPPORT_SL20_UDB48", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K60"))),
                new RelayEntry("T_SUPPORT_SL20_UDB49", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K61"))),
                new RelayEntry("T_SUPPORT_SL20_UDB50", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K62"))),
                new RelayEntry("T_SUPPORT_SL20_UDB51", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K63"))),
                new RelayEntry("T_SUPPORT_SL20_UDB52", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K64"))),
                new RelayEntry("T_SUPPORT_SL20_UDB53", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K65"))),
                new RelayEntry("T_SUPPORT_SL20_UDB54", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K66"))),
                new RelayEntry("T_SUPPORT_SL20_UDB55", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K67"))),
                new RelayEntry("T_SUPPORT_SL20_UDB56", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K68"))),
                new RelayEntry("T_SUPPORT_SL20_UDB57", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K69"))),
                new RelayEntry("T_SUPPORT_SL20_UDB58", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K70"))),
                new RelayEntry("T_SUPPORT_SL20_UDB59", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K71"))),
                new RelayEntry("T_SUPPORT_SL20_UDB60", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K72"))),
                new RelayEntry("T_SUPPORT_SL20_UDB61", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K1"))),
                new RelayEntry("T_SUPPORT_SL20_UDB62", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K2"))),
                new RelayEntry("T_SUPPORT_SL20_UDB63", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_5: HMODControl.RelayID("K3"))),
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
