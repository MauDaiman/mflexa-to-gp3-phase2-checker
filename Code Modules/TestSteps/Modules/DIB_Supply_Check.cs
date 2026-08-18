using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDmm;

namespace TestSteps.Modules
{
    public class DIB_Supply_Check
    {
        private const string dmmP143 = "P143_4081_DMM";

        /// <summary>
        /// Measures 12 on-board power supply voltages via DMM through HMOD18 relay muxing
        /// and publishes each reading per site.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context for session and site management.</param>
        public static void SupplyCheck(ISemiconductorModuleContext tsmContext)
        {
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, dmmP143);

            dmm.Abort();
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                apertureTime: 1e-3,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: 100
                );
            dmm.Initiate();

            try
            {
                foreach (var entry in supplyCheckResults)
                {
                    HMODControl.HMOD14to18(tsmContext, HMOD_Data_18: HMODControl.RelayID(entry.RelayId));
                    double[] reading = dmm.Read();
                    dmm.PinQueryContext.Publish(new double[] { reading[0] }, entry.PublishedName);
                }
            }
            finally
            {
                HMODControl.AllHMODReset(tsmContext);
                dmm.Abort();
            }
        }

        private sealed class SupplyCheckEntry
        {
            public string RelayId { get; }
            public string PublishedName { get; }

            public SupplyCheckEntry(string relayId, string publishedName)
            {
                RelayId = relayId;
                PublishedName = publishedName;
            }
        }

        private static readonly SupplyCheckEntry[] supplyCheckResults = new[]
        {
            new SupplyCheckEntry("17", "DIB_Supply_5V_1"),
            new SupplyCheckEntry("18", "DIB_Supply_5V_2"),
            new SupplyCheckEntry("19", "DIB_Supply_5V_3"),
            new SupplyCheckEntry("20", "DIB_Supply_12V_Slave"),
            new SupplyCheckEntry("21", "Master_15V"),
            new SupplyCheckEntry("22", "Slave_15V"),
            new SupplyCheckEntry("23", "Relay_Supply_12V"),
            new SupplyCheckEntry("24", "TFE_5V"),
            new SupplyCheckEntry("25", "HMOD_5V"),
            new SupplyCheckEntry("26", "COMP_n5p2V"),
            new SupplyCheckEntry("27", "XOR_n5V"),
            new SupplyCheckEntry("28", "n2V"),
        };
    }
}
