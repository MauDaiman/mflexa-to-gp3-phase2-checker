using System;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P1Checker
{
    public class Window_Comparator_Check
    {
        private const double CurrentForceLevel = 1e-3;
        private const double CurrentLevelRange = 10e-3;
        private const double SmuCurrentLimit = 10e-3;
        private const double SettlingTimeSec = 25e-3;

        private static readonly string[] _vOHPins = new[]
        {
            "P163_4163_SMU_CH18", "P163_4163_SMU_CH20", "P163_4163_SMU_CH22", "P163_4163_SMU_CH23",
            "P151_4163_SMU_CH18", "P151_4163_SMU_CH20", "P151_4163_SMU_CH22", "P138_4163_SMU_CH16"
        };

        private static readonly string[] _vOLPins = new[]
        {
            "P163_4163_SMU_CH17", "P163_4163_SMU_CH19", "P163_4163_SMU_CH21", "P151_4163_SMU_CH16",
            "P151_4163_SMU_CH17", "P151_4163_SMU_CH19", "P151_4163_SMU_CH21", "P151_4163_SMU_CH23"
        };

        private static readonly string[] _digitalDIOPins = new[]
        {
            "P154_6571_DIO_24", "P154_6571_DIO_25", "P154_6571_DIO_26", "P154_6571_DIO_27",
            "P154_6571_DIO_28", "P154_6571_DIO_29", "P154_6571_DIO_30", "P154_6571_DIO_31"
        };

        public static void WindowComparatorCheck(ISemiconductorModuleContext tsmContext)
        {
            DCPower smu1 = InstrCtrl.DCPowerPinsToSessions(tsmContext, _vOHPins);
            DCPower smu2 = InstrCtrl.DCPowerPinsToSessions(tsmContext, _vOLPins);
            Digital digital = InstrCtrl.DigitalPinsToSessions(tsmContext, _digitalDIOPins);

            digital.Abort();
            digital.SelectFunction(SelectedFunction.Digital);

            smu1.Abort(); smu2.Abort();
            smu1.ConfigureOutputFunction(DCPowerSourceOutputFunction.DCVoltage);
            smu1.ConfigureCurrentLimit(SmuCurrentLimit);
            smu1.ConfigureCurrentLimitRange(SmuCurrentLimit);
            smu1.ConfigureSense(DCPowerMeasurementSense.Local);
            smu1.ConfigureOutputEnabled(true);

            smu2.ConfigureOutputFunction(DCPowerSourceOutputFunction.DCVoltage);
            smu2.ConfigureCurrentLimit(SmuCurrentLimit);
            smu2.ConfigureCurrentLimitRange(SmuCurrentLimit);
            smu2.ConfigureSense(DCPowerMeasurementSense.Local);
            smu2.ConfigureOutputEnabled(true);

            try
            {
                HMODControl.THMODReset(tsmContext);
                HMODControl.HMOD11to13(tsmContext,
                    HMOD_Data_11: HMODControl.RelayID("K4, K9, K13, K18, K22, K27, K31"),
                    HMOD_Data_12: HMODControl.RelayID("K4, K8, K13, K17, K22, K26, K31"),
                    HMOD_Data_13: HMODControl.RelayID("K3, K8, K9, K10, K11"));

                digital.Abort();
                digital.SelectFunction(SelectedFunction.Ppmu);
                digital.PPMUForceCurrent(CurrentForceLevel, CurrentLevelRange, enableSourcing: true);

                smu1.ConfigureVoltageLevel(0.5);
                smu2.ConfigureVoltageLevel(-0.5);
                smu1.Initiate(); smu2.Initiate();
                Globals.TheHdw.Wait(SettlingTimeSec);

                double[][] insideWindow = digital.PPMUMeasure(PpmuMeasurementType.Voltage);

                smu1.Abort(); smu2.Abort();
                smu1.ConfigureVoltageLevel(2.0);
                smu2.ConfigureVoltageLevel(1.0);
                smu1.Initiate(); smu2.Initiate();
                Globals.TheHdw.Wait(SettlingTimeSec);

                double[][] outsideWindow = digital.PPMUMeasure(PpmuMeasurementType.Voltage);

                smu1.Abort(); smu2.Abort();

                double[] insideVals = new double[insideWindow[0].Length];
                double[] outsideVals = new double[outsideWindow[0].Length];
                for (int i = 0; i < insideWindow[0].Length; i++)
                {
                    insideVals[i] = insideWindow[0][i];
                    outsideVals[i] = outsideWindow[0][i];
                }
                digital.PinQueryContext.Publish(insideVals, "WinComp_Inside");
                digital.PinQueryContext.Publish(outsideVals, "WinComp_Outside");
            }
            finally
            {
                HMODControl.THMODReset(tsmContext);
                smu1.ConfigureOutputEnabled(false);
                smu2.ConfigureOutputEnabled(false);
            }
        }
    }
}
