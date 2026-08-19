using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDmm;
using TestSteps.Common;

namespace TestSteps.Modules
{
    public class DIB_Supply_Check
    {
        private const double SettlingTimeSec = 5e-3;

        /// <summary>
        /// Measures 6 on-board power supply voltages via DMM through relay muxing
        /// and publishes each reading per site.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context for session and site management.</param>
        /// <param name="meterType">Meter selection: 0 = PXIe-4137 SMU, 1 = PXIe-4081 DMM.</param>
        public static void SupplyCheck(ISemiconductorModuleContext tsmContext, int meterType = 0)
        {
            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext);

            try
            {
                foreach (var entry in supplyCheckResults)
                {
                    // Turn ON relay
                    Relay.ControlRelay(tsmContext, entry.RelayId, true);
                    Globals.TheHdw.Wait(SettlingTimeSec);

                    double[] reading = meter.Measure(tsmContext);

                    // Turn OFF relay
                    Relay.ControlRelay(tsmContext, entry.RelayId, false);
                    meter.PublishResult(reading, entry.PublishedName);
                }
            }
            finally
            {
                meter.Cleanup(tsmContext);
            }
        }

        private sealed class SupplyCheckEntry
        {
            public string[] RelayId { get; }
            public string PublishedName { get; }

            public SupplyCheckEntry(string[] relayId, string publishedName)
            {
                RelayId = relayId;
                PublishedName = publishedName;
            }
        }

        private static readonly SupplyCheckEntry[] supplyCheckResults = new[]
        {
            new SupplyCheckEntry( new string[] { "RL7" }, "DIB_Supply_5V_1"),
            new SupplyCheckEntry( new string[] { "RL8" }, "DIB_Supply_5V_2"),
            new SupplyCheckEntry( new string[] { "RL9" }, "DIB_Supply_5V_3"),
            new SupplyCheckEntry( new string[] { "RL10" }, "DIB_Supply_12V_Slave"),
            new SupplyCheckEntry( new string[] { "RL11" }, "Master_15V"),
            new SupplyCheckEntry( new string[] { "RL12" }, "Slave_15V")
        };
    }
}
