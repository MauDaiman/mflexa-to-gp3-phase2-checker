using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P1Checker
{
    public class All_HSD200_Check
    {
        private const double CurrentForceLevel = 1e-3;
        private const double CurrentLevelRange = 10e-3;
        private const double VoltageBelowClamp = 3.0;
        private const double SettlingTimeSec = 5e-3;

        private static readonly string[] _hsd200HI = new[]
        {
            "T_HSD200_SL11_CH5", "T_HSD200_SL11_CH9", "T_HSD200_SL11_CH13", "T_HSD200_SL11_CH17",
            "T_HSD200_SL11_CH21", "T_HSD200_SL11_CH25", "T_HSD200_SL11_CH29",
            "T_HSD200_SL11_CH33", "T_HSD200_SL11_CH37", "T_HSD200_SL11_CH41", "T_HSD200_SL11_CH45",
            "T_HSD200_SL13_CH33", "T_HSD200_SL13_CH37", "T_HSD200_SL13_CH41", "T_HSD200_SL13_CH45",
            "T_HSD200_SL13_CH1", "T_HSD200_SL13_CH5", "T_HSD200_SL13_CH9", "T_HSD200_SL13_CH13",
            "T_HSD200_SL13_CH17", "T_HSD200_SL13_CH21", "T_HSD200_SL13_CH25", "T_HSD200_SL13_CH29",
            "T_HSD200_SL15_CH1", "T_HSD200_SL15_CH5", "T_HSD200_SL15_CH9", "T_HSD200_SL15_CH13",
            "T_HSD200_SL15_CH17", "T_HSD200_SL15_CH21", "T_HSD200_SL15_CH25", "T_HSD200_SL15_CH29",
            "T_HSD200_SL15_CH33", "T_HSD200_SL15_CH37", "T_HSD200_SL15_CH41", "T_HSD200_SL15_CH45"
        };

        private static readonly string[] _hsd200LO = new[]
        {
            "T_HSD200_SL11_CH6", "T_HSD200_SL11_CH7", "T_HSD200_SL11_CH8",
            "T_HSD200_SL11_CH10", "T_HSD200_SL11_CH11", "T_HSD200_SL11_CH12",
            "T_HSD200_SL11_CH14", "T_HSD200_SL11_CH15", "T_HSD200_SL11_CH16",
            "T_HSD200_SL11_CH18", "T_HSD200_SL11_CH19", "T_HSD200_SL11_CH20",
            "T_HSD200_SL11_CH22", "T_HSD200_SL11_CH23", "T_HSD200_SL11_CH24",
            "T_HSD200_SL11_CH26", "T_HSD200_SL11_CH27", "T_HSD200_SL11_CH28",
            "T_HSD200_SL11_CH30", "T_HSD200_SL11_CH31", "T_HSD200_SL11_CH32",
            "T_HSD200_SL11_CH34", "T_HSD200_SL11_CH35", "T_HSD200_SL11_CH36",
            "T_HSD200_SL11_CH38", "T_HSD200_SL11_CH39", "T_HSD200_SL11_CH40",
            "T_HSD200_SL11_CH42", "T_HSD200_SL11_CH43", "T_HSD200_SL11_CH44",
            "T_HSD200_SL11_CH46", "T_HSD200_SL11_CH47", "T_HSD200_SL11_CH48",
            "T_HSD200_SL13_CH34", "T_HSD200_SL13_CH35", "T_HSD200_SL13_CH36",
            "T_HSD200_SL13_CH38", "T_HSD200_SL13_CH39", "T_HSD200_SL13_CH40",
            "T_HSD200_SL13_CH42", "T_HSD200_SL13_CH43", "T_HSD200_SL13_CH44",
            "T_HSD200_SL13_CH46", "T_HSD200_SL13_CH47", "T_HSD200_SL13_CH48",
            "T_HSD200_SL13_CH2", "T_HSD200_SL13_CH3", "T_HSD200_SL13_CH4",
            "T_HSD200_SL13_CH6", "T_HSD200_SL13_CH7", "T_HSD200_SL13_CH8",
            "T_HSD200_SL13_CH10", "T_HSD200_SL13_CH11", "T_HSD200_SL13_CH12",
            "T_HSD200_SL13_CH14", "T_HSD200_SL13_CH15", "T_HSD200_SL13_CH16",
            "T_HSD200_SL13_CH18", "T_HSD200_SL13_CH19", "T_HSD200_SL13_CH20",
            "T_HSD200_SL13_CH22", "T_HSD200_SL13_CH23", "T_HSD200_SL13_CH24",
            "T_HSD200_SL13_CH26", "T_HSD200_SL13_CH27", "T_HSD200_SL13_CH28",
            "T_HSD200_SL13_CH30", "T_HSD200_SL13_CH31", "T_HSD200_SL13_CH32",
            "T_HSD200_SL15_CH2", "T_HSD200_SL15_CH3", "T_HSD200_SL15_CH4",
            "T_HSD200_SL15_CH6", "T_HSD200_SL15_CH7", "T_HSD200_SL15_CH8",
            "T_HSD200_SL15_CH10", "T_HSD200_SL15_CH11", "T_HSD200_SL15_CH12",
            "T_HSD200_SL15_CH14", "T_HSD200_SL15_CH15", "T_HSD200_SL15_CH16",
            "T_HSD200_SL15_CH18", "T_HSD200_SL15_CH19", "T_HSD200_SL15_CH20",
            "T_HSD200_SL15_CH22", "T_HSD200_SL15_CH23", "T_HSD200_SL15_CH24",
            "T_HSD200_SL15_CH26", "T_HSD200_SL15_CH27", "T_HSD200_SL15_CH28",
            "T_HSD200_SL15_CH30", "T_HSD200_SL15_CH31", "T_HSD200_SL15_CH32",
            "T_HSD200_SL15_CH34", "T_HSD200_SL15_CH35", "T_HSD200_SL15_CH36",
            "T_HSD200_SL15_CH38", "T_HSD200_SL15_CH39", "T_HSD200_SL15_CH40",
            "T_HSD200_SL15_CH42", "T_HSD200_SL15_CH43", "T_HSD200_SL15_CH44",
            "T_HSD200_SL15_CH46", "T_HSD200_SL15_CH47", "T_HSD200_SL15_CH48"
        };

        public static void HSDCheck(ISemiconductorModuleContext tsmContext)
        {
            Digital digitalHI = InstrCtrl.DigitalPinsToSessions(tsmContext, _hsd200HI);
            Digital digitalLO = InstrCtrl.DigitalPinsToSessions(tsmContext, _hsd200LO);
            digitalHI.Abort(); digitalLO.Abort();

            try
            {
                HMODControl.THMODReset(tsmContext);
                HMODControl.HMOD14to18(tsmContext, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF,
                    HMOD_Data_18: HMODControl.RelayID("K1, K2, K3, K4, K5, " +
                        "K6, K7, K8, K9, K10, " +
                        "K11, K12, K13, K14, K15, " +
                        "K16"));
                HMODControl.HMOD19to23(tsmContext, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF);

                digitalHI.SelectFunction(SelectedFunction.Ppmu);
                digitalHI.PPMUForceVoltage(VoltageBelowClamp, CurrentLevelRange, true);

                digitalLO.SelectFunction(SelectedFunction.Ppmu);
                digitalLO.PPMUForceCurrent(-CurrentForceLevel, CurrentLevelRange, true);
                Globals.TheHdw.Wait(SettlingTimeSec);

                double[][] currents = digitalHI.PPMUMeasure(PpmuMeasurementType.Current);
                double[][] voltages = digitalLO.PPMUMeasure(PpmuMeasurementType.Voltage);

                double[] iAll = new double[currents[0].Length];
                for (int i = 0; i < currents[0].Length; i++)
                    iAll[i] = currents[0][i];
                digitalHI.PinQueryContext.Publish(iAll, "HSD_HI_Current");

                double[] vAll = new double[voltages[0].Length];
                for (int i = 0; i < voltages[0].Length; i++)
                    vAll[i] = voltages[0][i];
                digitalLO.PinQueryContext.Publish(vAll, "HSD_LO_Voltage");
            }
            finally
            {
                digitalHI.SelectFunction(SelectedFunction.Digital);
                digitalLO.SelectFunction(SelectedFunction.Digital);
                HMODControl.THMODReset(tsmContext);
            }
        }
    }
}
