using System;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps.P1Checker
{
    public class All_DC30_Check
    {
        private const double CurrentForceLevel = 1e-3;
        private const double CurrentLevelRange = 10e-3;
        private const double VoltageComplianceLimit = 5.0;
        private const double VoltageLimitRange = 6.0;
        private const double VoltageBelowClamp = 3.0;
        private const double CurrentLimitRange = 2e-3;
        private const double SettlingTimeSec = 20e-3;

        private static readonly string[] _dc30Odd = new[]
        {
            "DC30V_SL24_CH1", "DC30V_SL24_CH3", "DC30V_SL24_CH5", "DC30V_SL24_CH7", "DC30V_SL24_CH9",
            "DC30V_SL24_CH11", "DC30V_SL24_CH13", "DC30V_SL24_CH15", "DC30V_SL24_CH17", "DC30V_SL24_CH19",
            "DC30V_SL04_CH1", "DC30V_SL04_CH3", "DC30V_SL04_CH5", "DC30V_SL04_CH7", "DC30V_SL04_CH9",
            "DC30V_SL04_CH11", "DC30V_SL04_CH13", "DC30V_SL04_CH15", "DC30V_SL04_CH17", "DC30V_SL04_CH19",
            "DC30V_SL10_CH1", "DC30V_SL10_CH3", "DC30V_SL10_CH5", "DC30V_SL10_CH7", "DC30V_SL10_CH9",
            "DC30V_SL10_CH11", "DC30V_SL10_CH13", "DC30V_SL10_CH15", "DC30V_SL10_CH17", "DC30V_SL10_CH19"
        };

        private static readonly string[] _dc30Even = new[]
        {
            "DC30V_SL24_CH2", "DC30V_SL24_CH4", "DC30V_SL24_CH6", "DC30V_SL24_CH8", "DC30V_SL24_CH10",
            "DC30V_SL24_CH12", "DC30V_SL24_CH14", "DC30V_SL24_CH16", "DC30V_SL24_CH18", "DC30V_SL24_CH20",
            "DC30V_SL10_CH2", "DC30V_SL10_CH4", "DC30V_SL10_CH6", "DC30V_SL10_CH8", "DC30V_SL10_CH10",
            "DC30V_SL10_CH12", "DC30V_SL10_CH14", "DC30V_SL10_CH16", "DC30V_SL10_CH18", "DC30V_SL10_CH20",
            "DC30V_SL04_CH2", "DC30V_SL04_CH4", "DC30V_SL04_CH6", "DC30V_SL04_CH8", "DC30V_SL04_CH10",
            "DC30V_SL04_CH12", "DC30V_SL04_CH14", "DC30V_SL04_CH16", "DC30V_SL04_CH18", "DC30V_SL04_CH20"
        };

        public static void DC30OVCheck(ISemiconductorModuleContext tsmContext)
        {
            DCPower smuHI = InstrCtrl.DCPowerPinsToSessions(tsmContext, _dc30Odd);
            DCPower smuLO = InstrCtrl.DCPowerPinsToSessions(tsmContext, _dc30Even);

            smuHI.Abort(); smuLO.Abort();

            smuHI.ConfigureOutputFunction(DCPowerSourceOutputFunction.DCCurrent);
            smuHI.ConfigureCurrentLevel(-CurrentForceLevel);
            smuHI.ConfigureCurrentLevelRange(CurrentLevelRange);
            smuHI.ConfigureVoltageLimit(VoltageComplianceLimit);
            smuHI.ConfigureVoltageLimitRange(VoltageLimitRange);
            smuHI.ConfigureSense(DCPowerMeasurementSense.Local);
            smuHI.ConfigureOutputEnabled(true);

            smuLO.ConfigureOutputFunction(DCPowerSourceOutputFunction.DCVoltage);
            smuLO.ConfigureVoltageLevel(VoltageBelowClamp);
            smuLO.ConfigureVoltageLevelRange(VoltageLimitRange);
            smuLO.ConfigureCurrentLimit(CurrentLimitRange);
            smuLO.ConfigureCurrentLimitRange(CurrentLevelRange);
            smuLO.ConfigureSense(DCPowerMeasurementSense.Local);
            smuLO.ConfigureOutputEnabled(true);

            try
            {
                HMODControl.THMODReset(tsmContext);
                HMODControl.HMOD1to4(tsmContext, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    HMOD_Data_4: HMODControl.RelayID("K1, K2, K3, K4, K5, " +
                        "K6, K7, K8, K9, K10, " +
                        "K11, K12, K13, K14, K15, " +
                        "K16, K17, K18, K19, K20, " +
                        "K21, K22, K23, K24"));

                smuHI.Initiate(); smuLO.Initiate();
                Globals.TheHdw.Wait(SettlingTimeSec);

                smuHI.Measure(out double[] voltagesHI, out double[] currentsHI);
                smuLO.Measure(out double[] voltagesLO, out double[] currentsLO);

                smuHI.PinQueryContext.Publish(voltagesHI, "DC30_HI_Voltage");
                smuLO.PinQueryContext.Publish(currentsLO, "DC30_LO_Current");
            }
            finally
            {
                smuHI.ConfigureOutputEnabled(false);
                smuLO.ConfigureOutputEnabled(false);
                HMODControl.THMODReset(tsmContext);
            }
        }
    }
}
