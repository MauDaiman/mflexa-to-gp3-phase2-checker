using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
namespace TestSteps.P2Checker
{
    public class SL04_DIFFMETER_Check
    {
        private const string DmmS16Pin = "P131_DIFF_METER";
        private const string DmmS13Pin = "P131_4081_DMM";
        private const double SettlingTimeSec = 5e-3;
        private const double SmuCurrentLimit = 1e-3;
        private static readonly int[] Meter1Channels = { 1, 2, 5, 6, 9, 10, 13, 14, 17, 18 };

        /// <summary>
        /// Differential circuit check for SL04 DC30 channels.
        /// Forces 10V on odd channel and 9V on even channel, measures differential
        /// voltage (+1V / -1V) by swapping HI/LO via HMOD24.
        /// All SL04 channel pairs use HMOD24 K1–K40, meter routed through HMOD13 K12/K15.
        /// Final phase connects both meters simultaneously on CH17/CH18 pair.
        /// </summary>
        public static void SL04DiffMeterCheck(ISemiconductorModuleContext tsmContext)
        {
            HMODControl.AllHMODReset(tsmContext);
            Dmm dmmS16 = InstrCtrl.DmmPinsToSessions(tsmContext, DmmS16Pin);
            Dmm dmmS13 = InstrCtrl.DmmPinsToSessions(tsmContext, DmmS13Pin);

            dmmS16.Abort();
            dmmS13.Abort();

            ConfigureDmm(dmmS16);
            ConfigureDmm(dmmS13);
            try
            {
                PerChannelDifferential(tsmContext, dmmS16, dmmS13);
                FinalCheck(tsmContext, dmmS16, dmmS13);
            }
            finally
            {
                dmmS16.Abort();
                dmmS13.Abort();
                HMODControl.AllHMODReset(tsmContext);
            }
        }
        private static void PerChannelDifferential(
            ISemiconductorModuleContext tsmContext,
            Dmm dmmS16,
            Dmm dmmS13)
        {
            for (int chPair = 1; chPair <= 10; chPair++)
            {
                int chOdd = (chPair * 2) - 1;
                int chEven = chPair * 2;

                int relayOffset = (chPair - 1) * 4;

                bool usesMeter1 = Array.IndexOf(Meter1Channels, chOdd) >= 0;

                Dmm dmm = usesMeter1 ? dmmS16 : dmmS13;

                uint meterRelay = usesMeter1
                    ? HMODControl.RelayID("K12")
                    : HMODControl.RelayID("K15");

                HMODControl.AllHMODReset(tsmContext);

                HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: HMODControl.RelayRange(1, 20));
                HMODControl.HMOD5to10(tsmContext, HMOD_Data_5: HMODControl.RelayRange(6, 15));

                DCPower smuHi = InstrCtrl.DCPowerPinsToSessions(tsmContext, $"DC30V_SL04_CH{chOdd}");
                DCPower smuLo = InstrCtrl.DCPowerPinsToSessions(tsmContext, $"DC30V_SL04_CH{chEven}");

                ConfigureSmu(smuHi, 10.0);
                ConfigureSmu(smuLo, 9.0);
                try
                {
                    HMODControl.HMOD11to13_24to25(tsmContext,
                        hmodData13: meterRelay,
                        hmodData24: HMODControl.RelayID72($"K{relayOffset + 1}, K{relayOffset + 4}"));

                    Globals.TheHdw.Wait(SettlingTimeSec);

                    double[] posReading = dmm.Read();

                    HMODControl.HMOD11to13_24to25(tsmContext,
                        hmodData13: meterRelay,
                        hmodData24: HMODControl.RelayID72($"K{relayOffset + 2}, K{relayOffset + 3}"));

                    Globals.TheHdw.Wait(SettlingTimeSec);
                    double[] negReading = dmm.Read();

                    tsmContext.PublishPerSite(posReading, $"SL04_CH{chOdd}_CH{chEven}_POS");
                    tsmContext.PublishPerSite(negReading, $"SL04_CH{chOdd}_CH{chEven}_NEG");
                }
                finally
                {
                    smuHi.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);
                    smuLo.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);

                    smuHi.ConfigureOutputEnabled(false);
                    smuHi.ConfigureOutputConnected(false);
                    smuLo.ConfigureOutputEnabled(false);
                    smuLo.ConfigureOutputConnected(false);

                    smuHi.Abort();
                    smuLo.Abort();
                }
            }
        }
        private static void FinalCheck(
            ISemiconductorModuleContext tsmContext,
            Dmm dmmS16,
            Dmm dmmS13)
        {
            HMODControl.AllHMODReset(tsmContext);

            HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: HMODControl.RelayRange(1, 20));
            HMODControl.HMOD5to10(tsmContext, HMOD_Data_5: HMODControl.RelayRange(6, 15));

            int relayOffset = (9 - 1) * 4;

            DCPower smuHi = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DC30V_SL04_CH18");
            DCPower smuLo = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DC30V_SL04_CH20");

            ConfigureSmu(smuHi, 10.0);
            ConfigureSmu(smuLo, 9.0);
            try
            {
                HMODControl.HMOD11to13_24to25(tsmContext,
                    hmodData13: HMODControl.RelayID("K12, K15"),
                    hmodData24: HMODControl.RelayID72($"K{relayOffset + 1}, K{relayOffset + 4}"));

                Globals.TheHdw.Wait(SettlingTimeSec);
                double[] meter1Reading = dmmS16.Read();
                double[] meter2Reading = dmmS13.Read();

                tsmContext.PublishPerSite(meter1Reading, "SL04_HMOD13_METER1");
                tsmContext.PublishPerSite(meter2Reading, "SL04_HMOD13_METER2");
            }
            finally
            {
                smuHi.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);
                smuLo.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);

                smuHi.ConfigureOutputEnabled(false);
                smuHi.ConfigureOutputConnected(false);
                smuLo.ConfigureOutputEnabled(false);
                smuLo.ConfigureOutputConnected(false);

                smuHi.Abort();
                smuLo.Abort();
            }
        }
        private static void ConfigureDmm(Dmm dmm)
        {
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                1e-3,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: 100);
        }
        private static void ConfigureSmu(DCPower smu, double voltage)
        {
            smu.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(sense: DCPowerMeasurementSense.Local, initiateSessionAfter: false);
            smu.ConfigureVoltageLevelRange(voltage);
            smu.ConfigureCurrentLimitRange(SmuCurrentLimit);
            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);
            smu.ForceVoltage(voltageLevel: voltage, currentLimit: SmuCurrentLimit);
        }

    }
}