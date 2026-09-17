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
    public class SL13_HSD200_DA_Check
    {
        private const double SettlingTimeSec = 15e-3;
        public static void SL13DACheck(ISemiconductorModuleContext tsmContext,
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
                HMOD_Data_15: HMODControl.RelayRange(17, 32),
                HMOD_Data_16: HMODControl.RelayRange(1, 32));

            var hsdEntries = new HSDEntry[]
            {
                new HSDEntry("SL13_HSD_ACC1",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K60")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K17")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K18")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K19")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K20"))),
                new HSDEntry("SL13_HSD_ACC2",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K65")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K21")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K22")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K23")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K24"))),
                new HSDEntry("SL13_HSD_ACC3",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K70")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K25")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K26")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K27")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K28"))),
                new HSDEntry("SL13_HSD_ACC4",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K3")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K29")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K30")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K31")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_20: HMODControl.RelayID("K32"))),
                new HSDEntry("SL13_HSD_ACC5",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K8")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K1")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K2")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K3")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K4"))),
                new HSDEntry("SL13_HSD_ACC6",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K13")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K5")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K6")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K7")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K8"))),
                new HSDEntry("SL13_HSD_ACC7",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K18")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K9")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K10")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K11")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K12"))),
                new HSDEntry("SL13_HSD_ACC8",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K23")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K13")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K14")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K15")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K16"))),
                new HSDEntry("SL13_HSD_ACC9",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K28")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K17")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K18")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K19")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K20"))),
                new HSDEntry("SL13_HSD_ACC10",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K33")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K21")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K22")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K23")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K24"))),
                new HSDEntry("SL13_HSD_ACC11",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K38")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K25")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K26")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K27")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K28"))),
                new HSDEntry("SL13_HSD_ACC12",
                    () => HMODControl.CHMOD1to6(tsmContext, HMOD_Data_3: HMODControl.RelayID72("K43")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K29")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K30")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K31")),
                    () => HMODControl.HMOD19to23(tsmContext, HMOD_Data_21: HMODControl.RelayID("K32")))
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

                        // Connect DA to METER_HI
                        entry.DARelay();

                        entry.Relay1();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read1 = meter.MeasureVoltage(tsmContext);

                        entry.Relay2();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read2 = meter.MeasureVoltage(tsmContext);

                        entry.Relay3();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read3 = meter.MeasureVoltage(tsmContext);

                        entry.Relay4();
                        Globals.TheHdw.Wait(SettlingTimeSec);

                        double[] read4 = meter.MeasureVoltage(tsmContext);

                        meter.PublishResult(tsmContext, read1, entry.HSDGroup + "_DA_Voltage_1");
                        meter.PublishResult(tsmContext, read2, entry.HSDGroup + "_DA_Voltage_2");
                        meter.PublishResult(tsmContext, read3, entry.HSDGroup + "_DA_Voltage_3");
                        meter.PublishResult(tsmContext, read4, entry.HSDGroup + "_DA_Voltage_4");
                    }
                    finally
                    {
                        digital.Abort();
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
            public Action DARelay { get; }
            public Action Relay1 { get; }
            public Action Relay2 { get; }
            public Action Relay3 { get; }
            public Action Relay4 { get; }

            public HSDEntry(string hsdGroup, Action daRelay, Action relay1, Action relay2, Action relay3, Action relay4)
            {
                HSDGroup = hsdGroup;
                DARelay = daRelay;
                Relay1 = relay1;
                Relay2 = relay2;
                Relay3 = relay3;
                Relay4 = relay4;
            }
        }
    }
}
