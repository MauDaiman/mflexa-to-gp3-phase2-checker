using System;
using TestSteps.Common;
using NationalInstruments;
using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P2Checker
{
    public class SL13_HSD200_Check
    {
        private const double SettlingTimeSec = 5e-3;

        public static void SL13Check(ISemiconductorModuleContext tsmContext, 
            DCPowerMeasurementSense senseType, 
            int meterType = 0)
        {
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // Grounds the METER LO

            HMODControl.AllHMODReset(tsmContext);

            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType);

            // Turn ON relays in SL11
            HMODControl.HMOD14to18(tsmContext,
                HMOD_Data_15: HMODControl.RelayRange(17, 32),
                HMOD_Data_16: HMODControl.RelayRange(1, 32));

            // Turn ON relays SL11 to DIB Access
            HMODControl.HMOD19to23(tsmContext,
                HMOD_Data_20: HMODControl.RelayRange(17, 32),
                HMOD_Data_21: HMODControl.RelayRange(1, 32));

            var hsdEntries = new HSDEntry[]
            {
                new HSDEntry("SL13_HSD_ACC1",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K56")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K57")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K58")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K59")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K60"))),
                new HSDEntry("SL13_HSD_ACC2",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K61")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K62")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K63")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K64")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K65"))),
                new HSDEntry("SL13_HSD_ACC3",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K66")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K67")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K68")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K69")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K70"))),
                new HSDEntry("SL13_HSD_ACC4",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K71")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K72")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K1")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K2")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K3"))),
                new HSDEntry("SL13_HSD_ACC5",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K4")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K5")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K6")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K7")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K8"))),
                new HSDEntry("SL13_HSD_ACC6",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K9")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K10")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K11")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K12")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K13"))),
                new HSDEntry("SL13_HSD_ACC7",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K14")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K15")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K16")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K17")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K18"))),
                new HSDEntry("SL13_HSD_ACC8",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K19")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K20")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K21")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K22")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K23"))),
                new HSDEntry("SL13_HSD_ACC9",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K24")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K25")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K26")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K27")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K28"))),
                new HSDEntry("SL13_HSD_ACC10",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K29")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K30")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K31")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K32")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K33"))),
                new HSDEntry("SL13_HSD_ACC11",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K34")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K35")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K36")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K37")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K38"))),
                new HSDEntry("SL13_HSD_ACC12",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K39")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K40")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K41")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K42")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K43"))),
            };

            try
            {
                foreach (var entry in hsdEntries)
                {
                    Digital digital = InstrCtrl.DigitalPinsToSessions(tsmContext, entry.HSDGroup);
                    digital.Abort();

                    try
                    {
                        digital.SelectFunction(SelectedFunction.Ppmu);
                        digital.PPMUForceCurrent(currentLevel: -1e-3, currentLevelRange: 10e-3, enableSourcing: true); // -1mA

                        HMODControl.CHMODReset(tsmContext);
                        entry.Relay1();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read1 = meter.MeasureCurrent(tsmContext);

                        HMODControl.CHMODReset(tsmContext);
                        entry.Relay2();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read2 = meter.MeasureCurrent(tsmContext);

                        HMODControl.CHMODReset(tsmContext);
                        entry.Relay3();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read3 = meter.MeasureCurrent(tsmContext);

                        HMODControl.CHMODReset(tsmContext);
                        entry.Relay4();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read4 = meter.MeasureCurrent(tsmContext);

                        HMODControl.CHMODReset(tsmContext);
                        entry.DARelay();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] daRead = meter.MeasureCurrent(tsmContext);

                        meter.PublishResult(tsmContext, read1, entry.HSDGroup + "_Current_1");
                        meter.PublishResult(tsmContext, read2, entry.HSDGroup + "_Current_2");
                        meter.PublishResult(tsmContext, read3, entry.HSDGroup + "_Current_3");
                        meter.PublishResult(tsmContext, read4, entry.HSDGroup + "_Current_4");
                        meter.PublishResult(tsmContext, daRead, entry.HSDGroup + "_Current_DA");
                    }
                    finally
                    {
                        digital.Abort();
                        digital.SelectFunction(SelectedFunction.Digital);
                    }
                }
            }
            finally
            {
                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);
                meter.Cleanup(tsmContext);
                HMODControl.AllHMODReset(tsmContext);
            }
        }

        private sealed class HSDEntry
        {
            public string HSDGroup { get; }
            public Action Relay1 { get; }
            public Action Relay2 { get; }
            public Action Relay3 { get; }
            public Action Relay4 { get; }
            public Action DARelay { get; }

            public HSDEntry(string hsdGroup, Action relay1, Action relay2, Action relay3, Action relay4, Action daRelay)
            {
                HSDGroup = hsdGroup;
                Relay1 = relay1;
                Relay2 = relay2;
                Relay3 = relay3;
                Relay4 = relay4;
                DARelay = daRelay;
            }
        }
    }
}
