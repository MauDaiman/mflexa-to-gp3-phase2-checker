using System;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P1Checker
{
    public class All_POOL2_Check
    {
        private const double CurrentForceLevel = 1e-3;
        private const double CurrentLevelRange = 10e-3;
        private const double SettlingTimeSec = 5e-3;

        private static readonly string[] _digitalDIOPins = new[]
        {
            "P154_6571_DIO_24", "P154_6571_DIO_25", "P154_6571_DIO_26", "P154_6571_DIO_27",
            "P154_6571_DIO_28", "P154_6571_DIO_29", "P154_6571_DIO_30", "P154_6571_DIO_31"
        };

        public static void POOLOpenCheck(ISemiconductorModuleContext tsmContext)
        {
            Digital digital = InstrCtrl.DigitalPinsToSessions(tsmContext, _digitalDIOPins);
            digital.Abort();

            try
            {
                HMODControl.THMODReset(tsmContext);

                digital.SelectFunction(SelectedFunction.Ppmu);
                digital.PPMUForceCurrent(CurrentForceLevel, CurrentLevelRange, enableSourcing: true);
                Globals.TheHdw.Wait(SettlingTimeSec);

                double[][] voltagesBeforeHMOD = digital.PPMUMeasure(PpmuMeasurementType.Voltage);
                digital.Abort();

                HMODControl.HMOD11to13(tsmContext,
                    HMOD_Data_11: HMODControl.RelayID("K1, K2, K3, K4, K5, " +
                        "K6, K7, K9, K10, K11, " +
                        "K12, K13, K14, K15, K16, " +
                        "K18, K19, K20, K21, K22, " +
                        "K23, K24, K25, K27, K28, " +
                        "K29, K30, K31, K32"),
                    HMOD_Data_12: HMODControl.RelayID("K1, K2, K4, K5, K6, " +
                        "K7, K8, K9, K10, K11, " +
                        "K13, K14, K15, K16, K17, " +
                        "K18, K19, K20, K22, K23, " +
                        "K24, K25, K26, K27, K28, " +
                        "K29, K31, K32"),
                    HMOD_Data_13: HMODControl.RelayID("K1, K2, K3, K4, K5, " +
                        "K6, K8"));

                digital.SelectFunction(SelectedFunction.Ppmu);
                digital.PPMUForceCurrent(CurrentForceLevel, CurrentLevelRange, enableSourcing: true);
                Globals.TheHdw.Wait(SettlingTimeSec);

                double[][] voltagesAfterHMOD = digital.PPMUMeasure(PpmuMeasurementType.Voltage);

                double[] vAllBeforeHMOD = new double[voltagesBeforeHMOD[0].Length];
                double[] vAllAfterHMOD = new double[voltagesAfterHMOD[0].Length];
                for (int i = 0; i < voltagesBeforeHMOD[0].Length; i++)
                {
                    vAllBeforeHMOD[i] = voltagesBeforeHMOD[0][i];
                    vAllAfterHMOD[i] = voltagesAfterHMOD[0][i];
                }
                digital.PinQueryContext.Publish(vAllBeforeHMOD, "POOL_Open");
                digital.PinQueryContext.Publish(vAllAfterHMOD, "POOL_Grounded");
            }
            finally
            {
                digital.SelectFunction(SelectedFunction.Digital);
                HMODControl.THMODReset(tsmContext);
            }
        }
    }
}
