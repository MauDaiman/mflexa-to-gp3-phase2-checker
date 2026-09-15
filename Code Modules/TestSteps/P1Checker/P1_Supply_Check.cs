using System;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps.P1Checker
{
    public class P1_Supply_Check
    {
        private const string DmmPin = "P143_4081_DMM";
        private const double DmmApertureSeconds = 1e-3;
        private const double DmmVoltageRange = 100.0;

        public static void SupplyCheck(ISemiconductorModuleContext tsmContext)
        {
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmPin);

            dmm.Abort();
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                DmmApertureSeconds, DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: DmmVoltageRange);
            dmm.Initiate();

            try
            {
                foreach (var entry in _supplyCheckResults)
                {
                    HMODControl.HMOD14to18(tsmContext, HMOD_Data_18: HMODControl.RelayID(entry.RelayId));
                    double[] reading = dmm.Read();
                    dmm.PinQueryContext.Publish(new double[] { reading[0] }, entry.PublishedName);
                }
            }
            finally
            {
                HMODControl.THMODReset(tsmContext);
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

        private static readonly SupplyCheckEntry[] _supplyCheckResults = new[]
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
