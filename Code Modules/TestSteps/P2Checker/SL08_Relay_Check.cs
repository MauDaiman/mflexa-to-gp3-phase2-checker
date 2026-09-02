using System;
using TestSteps.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class SL08_Relay_Check
    {
        private const double RelaySettleSec = 5e-3;
        public static void SL08Check(ISemiconductorModuleContext tsmContext, 
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
                new RelayEntry("T_SUPPORT_SL08_UDB20", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K32"))),
                new RelayEntry("T_SUPPORT_SL08_UDB21", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K33"))),
                new RelayEntry("T_SUPPORT_SL08_UDB22", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K34"))),
                new RelayEntry("T_SUPPORT_SL08_UDB23", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K35"))),
                new RelayEntry("T_SUPPORT_SL08_UDB24", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K36"))),
                new RelayEntry("T_SUPPORT_SL08_UDB25", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K37"))),
                new RelayEntry("T_SUPPORT_SL08_UDB26", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K38"))),
                new RelayEntry("T_SUPPORT_SL08_UDB27", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K39"))),
                new RelayEntry("T_SUPPORT_SL08_UDB28", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K40"))),
                new RelayEntry("T_SUPPORT_SL08_UDB29", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K41"))),
                new RelayEntry("T_SUPPORT_SL08_UDB30", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K42"))),
                new RelayEntry("T_SUPPORT_SL08_UDB31", () => HMODControl.CHMOD1to13(tsmContext, HMOD_Data_4: HMODControl.RelayID("K43"))),
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
