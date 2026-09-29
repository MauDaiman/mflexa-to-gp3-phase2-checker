using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
namespace TestSteps.P2Checker
{
    public class SL10_DIFFMETER_Check
    {
        private const string DmmS16Pin = "P131_DIFF_METER";
        private const string DmmS13Pin = "P131_4081_DMM";
        private const int PairCount = 10;
        private const int PairsPerHmod = 5;
        private const int Meter1FinalPair = 9;
        private const int Meter2FinalPair = 10;
        private const double SettlingTimeSec = 10e-3;
        private const double SmuCurrentLimit = 10e-3;
        private static readonly int[] Meter1Channels = { 1, 2, 5, 6, 9, 10, 13, 14, 17, 18 };
        /// <summary>
        /// Differential circuit check for SL10 DC30 channels.
        /// Forces 10V on odd channel and 9V on even channel, measures differential
        /// voltage (+1V / -1V) by swapping HI/LO via HMOD24 (chPair 1–5) or HMOD25 (chPair 6–10).
        /// Meter routed through HMOD13 K13/K16.
        /// Final phase connects both meters simultaneously on CH17/CH18 pair.
        /// </summary>
        public static void SL10DiffMeterCheck(ISemiconductorModuleContext tsmContext)
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

                bool isHmod24 = !UsesHmod25(chPair);

                int relayOffset = RelayOffsetFor(chPair);

                bool usesMeter1 = Array.IndexOf(Meter1Channels, chOdd) >= 0;

                Dmm dmm = usesMeter1 ? dmmS16 : dmmS13;

                uint meterRelay = usesMeter1
                    ? HMODControl.RelayID("K13")
                    : HMODControl.RelayID("K16");

                HMODControl.AllHMODReset(tsmContext);

                HMODControl.HMOD1to4(tsmContext,
                    HMOD_Data_1: HMODControl.RelayRange(21, 32),
                    HMOD_Data_2: HMODControl.RelayRange(1, 8));

                HMODControl.HMOD5to10(tsmContext, HMOD_Data_7: HMODControl.RelayRange(6, 15));

                DCPower smuHi = InstrCtrl.DCPowerPinsToSessions(tsmContext, $"DC30V_SL10_CH{chOdd}");
                DCPower smuLo = InstrCtrl.DCPowerPinsToSessions(tsmContext, $"DC30V_SL10_CH{chEven}");

                ConfigureSmu(smuHi, 10.0);
                ConfigureSmu(smuLo, 9.0);
                try
                {
                    uint[] relayData = HMODControl.RelayID72($"K{relayOffset + 1}, K{relayOffset + 4}", true);
                    HMODControl.HMOD11to13_24to25(tsmContext,
                        hmodData13: meterRelay,
                        hmodData24: isHmod24 ? relayData : null,
                        hmodData25: isHmod24 ? null : relayData);

                    Globals.TheHdw.Wait(SettlingTimeSec);

                    double[] posReading = dmm.Read();
                    uint[] relayDataNeg = HMODControl.RelayID72($"K{relayOffset + 2}, K{relayOffset + 3}", true);
                    HMODControl.HMOD11to13_24to25(tsmContext,
                        hmodData13: meterRelay,
                        hmodData24: isHmod24 ? relayDataNeg : null,
                        hmodData25: isHmod24 ? null : relayDataNeg);

                    Globals.TheHdw.Wait(SettlingTimeSec);

                    double[] negReading = dmm.Read();

                    tsmContext.PublishPerSite(posReading, $"SL10_CH{chOdd}_CH{chEven}_POS");
                    tsmContext.PublishPerSite(negReading, $"SL10_CH{chOdd}_CH{chEven}_NEG");
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

            HMODControl.HMOD1to4(tsmContext,
                HMOD_Data_1: HMODControl.RelayRange(21, 32),
                HMOD_Data_2: HMODControl.RelayRange(1, 8));

            HMODControl.HMOD5to10(tsmContext, HMOD_Data_7: HMODControl.RelayRange(6, 15));

            // METER1 and METER2 are separate buses (SL10_DCDIFF_METER1_HI/LO and
            // SL10_DCDIFF_METER2_HI/LO on the 089357 Translator Board). A pair's HMOD24/25 relay
            // block lands on only one of those buses, so the two meters cannot both read one pair.
            // This previously closed both meter taps against a single pair, leaving METER2
            // connected to a bus with nothing driven onto it, so it read ~5 mV. HMOD13 K13 and K16
            // are independent two-pole relays that each switch their own DMM's HI and LO, so
            // nothing was contending. Meter1Channels places pair 9 on the METER1 bus and pair 10 on
            // the METER2 bus, so each meter path is exercised with the pair that feeds it.
            MeasureMeterPath(tsmContext, dmmS16, Meter1FinalPair, "K13", "SL10_HMOD13_METER1");
            MeasureMeterPath(tsmContext, dmmS13, Meter2FinalPair, "K16", "SL10_HMOD13_METER2");
        }
/// <summary>
        /// Single source of truth for which HMOD24/25 relay block serves a channel pair. Both the
        /// per-pair loop and FinalCheck route through this, so they cannot disagree about the
        /// block the way a hand-written offset expression allowed.
        /// </summary>
        /// <param name="chPair">Channel pair, 1 to 10.</param>
        private static int RelayOffsetFor(int chPair)
        {
            if (chPair < 1 || chPair > PairCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(chPair), $"Channel pair {chPair} is outside 1-{PairCount}.");
            }
            return 40 + ((chPair - 1) % PairsPerHmod) * 4;
        }

        /// <summary>
        /// Forces one channel pair to a 1 V differential and reads it on the meter whose bus that
        /// pair is wired to, proving that meter path works end to end.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="dmm">DMM session for the meter under test.</param>
        /// <param name="chPair">Channel pair whose relay block feeds that meter's bus.</param>
        /// <param name="meterRelay">HMOD13 relay connecting the meter to its bus.</param>
        /// <param name="publishId">Published data ID for the reading.</param>
        private static void MeasureMeterPath(
            ISemiconductorModuleContext tsmContext,
            Dmm dmm,
            int chPair,
            string meterRelay,
            string publishId)
        {
            int relayOffset = RelayOffsetFor(chPair);
            int chOdd = (chPair * 2) - 1;

            DCPower smuHi = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DC30V_SL10_CH" + chOdd);
            DCPower smuLo = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DC30V_SL10_CH" + (chOdd + 1));

            ConfigureSmu(smuHi, 10.0);
            ConfigureSmu(smuLo, 9.0);
            try
            {
                uint[] blockRelays = HMODControl.RelayID72(
                    "K" + (relayOffset + 1) + ", K" + (relayOffset + 4), true);

                HMODControl.HMOD11to13_24to25(tsmContext,
                    hmodData13: HMODControl.RelayID(meterRelay),
                    hmodData24: UsesHmod25(chPair) ? null : blockRelays,
                    hmodData25: UsesHmod25(chPair) ? blockRelays : null);

                Globals.TheHdw.Wait(SettlingTimeSec);
                tsmContext.PublishPerSite(dmm.Read(), publishId);
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

        /// <summary>
        /// Pairs 1-5 are wired to HMOD24 and pairs 6-10 to HMOD25.
        /// </summary>
        private static bool UsesHmod25(int chPair)
        {
            return chPair > PairsPerHmod;
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

            // Lower the Current Limit value before narrowing the Current Limit Range. DCSetup
            // leaves every ALLDC channel at a 100 mA limit with a 100 mA range, so narrowing the
            // range first would leave the stale value outside its own range and the driver
            // rejects the write. ForceVoltage is given an explicit range for the same reason.
            smu.ConfigureCurrentLimit(currentLimit: SmuCurrentLimit);
            smu.ConfigureCurrentLimitRange(currentLimitRange: SmuCurrentLimit);

            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);
            smu.ForceVoltage(
                voltageLevel: voltage,
                currentLimit: SmuCurrentLimit,
                currentLimitRange: SmuCurrentLimit);
        }

    }
}