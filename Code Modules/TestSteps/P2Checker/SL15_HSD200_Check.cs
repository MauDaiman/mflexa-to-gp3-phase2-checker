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
    public class SL15_HSD200_Check
    {
        private const double SettlingTimeSec = 5e-3;

        public static void SL15Check(ISemiconductorModuleContext tsmContext, 
            DCPowerMeasurementSense senseType, 
            int meterType = 0)
        {
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // Grounds the METER LO

            HMODControl.AllHMODReset(tsmContext);

            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType, ForceMode.ForceCurrent);

            // Turn ON relays in SL11
            HMODControl.HMOD14to18(tsmContext,
                HMOD_Data_14: HMODControl.RelayID("K1, K2, K3, K4"),
                HMOD_Data_17: HMODControl.RelayRange(1, 32),
                HMOD_Data_18: HMODControl.RelayRange(1, 16));

            var hsdEntries = new HSDEntry[]
            {
                new HSDEntry("SL15_HSD_ACC1",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K44")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K45")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K46")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K47"))),
                new HSDEntry("SL15_HSD_ACC2",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K49")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K50")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K51")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K52"))),
                new HSDEntry("SL15_HSD_ACC3",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K54")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K55")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K56")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K57"))),
                new HSDEntry("SL15_HSD_ACC4",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K59")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K60")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K61")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K62"))),
                new HSDEntry("SL15_HSD_ACC5",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K64")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K65")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K66")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K67"))),
                new HSDEntry("SL15_HSD_ACC6",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K69")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K70")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K71")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K72"))),
                new HSDEntry("SL15_HSD_ACC7",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K2")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K3")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K4")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K5"))),
                new HSDEntry("SL15_HSD_ACC8",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K7")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K8")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K9")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K10"))),
                new HSDEntry("SL15_HSD_ACC9",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K12")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K13")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K14")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K15"))),
                new HSDEntry("SL15_HSD_ACC10",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K17")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K18")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K19")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K20"))),
                new HSDEntry("SL15_HSD_ACC11",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K22")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K23")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K24")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K25"))),
                new HSDEntry("SL15_HSD_ACC12",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K27")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K28")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K29")),
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_4: HMODControl.RelayID72("K30"))),
            };

            try
            {
                foreach (var entry in hsdEntries)
                {
                    Digital digital = InstrCtrl.DigitalPinsToSessions(tsmContext, entry.HSDGroup);
                    digital.Abort();

                    try
                    {
                        HMODControl.CHMODReset(tsmContext);

                        digital.SelectFunction(SelectedFunction.Digital);
                        digital.ConfigureVoltgeLevels(vil: 0, vih: 5, vol: 0, voh: 0, vterm: 0);
                        digital.WriteStatic(PinState._1);

                        entry.Relay1();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read1 = meter.MeasureVoltage(tsmContext);
                        HMODControl.CHMODReset(tsmContext);

                        entry.Relay2();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read2 = meter.MeasureVoltage(tsmContext);
                        HMODControl.CHMODReset(tsmContext);

                        entry.Relay3();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read3 = meter.MeasureVoltage(tsmContext);
                        HMODControl.CHMODReset(tsmContext);

                        entry.Relay4();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read4 = meter.MeasureVoltage(tsmContext);
                        HMODControl.CHMODReset(tsmContext);

                        meter.PublishResult(tsmContext, read1, entry.HSDGroup + "_Voltage_1");
                        meter.PublishResult(tsmContext, read2, entry.HSDGroup + "_Voltage_2");
                        meter.PublishResult(tsmContext, read3, entry.HSDGroup + "_Voltage_3");
                        meter.PublishResult(tsmContext, read4, entry.HSDGroup + "_Voltage_4");
                    }
                    finally
                    {
                        digital.Abort();
                        digital.WriteStatic(PinState._0);
                        HMODControl.CHMODReset(tsmContext);
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

            public HSDEntry(string hsdGroup, Action relay1, Action relay2, Action relay3, Action relay4)
            {
                HSDGroup = hsdGroup;
                Relay1 = relay1;
                Relay2 = relay2;
                Relay3 = relay3;
                Relay4 = relay4;
            }
        }
    }
}
