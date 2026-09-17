using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.P2Checker
{
    public class POOL2_Rise_Time_Check
    {
        // =====================================================================
        // Pin Names — update to match pin map
        // =====================================================================
        private const string DrivePin = "T_HSD200_SL11_CH5";
        private const string CapturePin = "P154_6571_DIO_24";
        private const string SmuVohPin = "P163_4163_SMU_CH18";
        private const string SmuVolPin = "P163_4163_SMU_CH17";

        // =====================================================================
        // HMOD6 Relay IDs — Checker Board (AD8244 + RC path)
        // =====================================================================
        private const string Hmod6Relay = "K16, K17";

        // =====================================================================
        // HMOD11 Relay IDs — Translator Board TFE
        // =====================================================================
        private const string Hmod11Relay = "K1, K4, K9";

        // =====================================================================
        // Timing
        // =====================================================================
        private const double SettlingTimeSec = 10e-3;
        private const double DischargeTimeSec = 10e-3;
        private const double CaptureTimeoutSec = 5.0;
        private const double SamplePeriodSec = 10e-6;

        // =====================================================================
        // Window Comparator Thresholds (4163 SMU)
        // Adjust based on actual attenuation factor of input option network.
        // Default: assumes ~0.5x divider → 2.5V full scale.
        //   VOL = 10% of 2.5V = 0.25V
        //   VOH = 90% of 2.5V = 2.25V
        // =====================================================================
        private const double ThresholdVol = 0.25;
        private const double ThresholdVoh = 2.25;
        private const double SmuCurrentLimit = 10e-3;
        private const double SmuApertureTimeSec = 10e-3;

        // =====================================================================
        // Digital Pattern — TODO: create compiled .digipat pattern file
        // The pattern must:
        //   - Drive pin: LOW pre-trigger, then HIGH for capture duration
        //   - Capture pin: sample XOR output using capture_start/capture_stop
        //   - Time set period must equal SamplePeriodSec (10 µs)
        // =====================================================================
        private const string RiseTimePatternLabel = "rise_time_pattern";
        private const string CaptureWaveformName = "rise_time_capture_wfm";
        private const int MaxCaptureSamples = 1000;

        // =====================================================================
        // Levels and Timing Sheet Names — must match .digitiming / .digilevels
        // =====================================================================
        private const string LevelsSheet = "Rise_time_levels";
        private const string TimingSheet = "Rise_time_timing";

        /// <summary>
        /// Measures the 10%-to-90% rise time of the 1kΩ + 1µF RC network on the
        /// checker board (U17 AD8244BRMZ buffer → R273 → C27) through the TFE
        /// window comparator (AD96687BRZ + MC100EL07DR2G XOR gate).
        ///
        /// Signal path:
        ///   6571 SL11 → HMOD6 K16 → AD8244 buffer → 1kΩ → ┬ → HMOD6 K17 → T_POOL_SL12_OUT_CH_1 → 10kΩ input option (HMOD11 K3/K6)→ AD96687 dual comparator → MC100EL07 XOR → HMOD11 K9 → P154_6571_DIO_24
        ///                                                 │                                                                       VOH ← 4163 SMU
        ///                                                1µF                                                                      VOL ← 4163 SMU
        ///                                                 │     
        ///                                                GND                                                                       
        ///                                                          
        /// Expected rise time: 2.197 × τ = 2.197 ms (τ = RC = 1 ms).
        /// </summary>
        public static void Pool2RiseTimeCheck(ISemiconductorModuleContext tsmContext)
        {
            HMODControl.AllHMODReset(tsmContext);

            // Close checker board relays: drive pin → AD8244 buffer, RC output → TFE pool
            HMODControl.CHMOD1to6(tsmContext,
                HMOD_Data_6: HMODControl.RelayID72(Hmod6Relay));

            // Close TFE input option relays: select 10kΩ/10V path
            HMODControl.HMOD11to13(tsmContext,
                HMOD_Data_11: HMODControl.RelayID(Hmod11Relay));

            Globals.TheHdw.Wait(SettlingTimeSec);

            // Configure 4163 SMU channels to set window comparator thresholds
            DCPower smuVoh = InstrCtrl.DCPowerPinsToSessions(tsmContext, SmuVohPin);
            DCPower smuVol = InstrCtrl.DCPowerPinsToSessions(tsmContext, SmuVolPin);

            Digital digital = InstrCtrl.DigitalPinsToSessions(tsmContext, new string[] { DrivePin, CapturePin });

            digital.Abort();

            try
            {
                // Set upper threshold (VOH)
                smuVoh.ConfigureSettings(
                    apertureTime: SmuApertureTimeSec,
                    apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
                smuVoh.ConfigureSense(
                    sense: DCPowerMeasurementSense.Remote,
                    initiateSessionAfter: false);
                smuVoh.ConfigureVoltageLevelRange(voltageLevelRange: 6);
                smuVoh.ConfigureCurrentLimitRange(currentLimitRange: SmuCurrentLimit);
                smuVoh.ConfigureOutputConnected(true);
                smuVoh.ConfigureOutputEnabled(true);
                smuVoh.ForceVoltage(voltageLevel: ThresholdVoh, currentLimit: SmuCurrentLimit);

                // Set lower threshold (VOL)
                smuVol.ConfigureSettings(
                    apertureTime: SmuApertureTimeSec,
                    apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
                smuVol.ConfigureSense(
                    sense: DCPowerMeasurementSense.Remote,
                    initiateSessionAfter: false);
                smuVol.ConfigureVoltageLevelRange(voltageLevelRange: 6);
                smuVol.ConfigureCurrentLimitRange(currentLimitRange: SmuCurrentLimit);
                smuVol.ConfigureOutputConnected(true);
                smuVol.ConfigureOutputEnabled(true);
                smuVol.ForceVoltage(voltageLevel: ThresholdVol, currentLimit: SmuCurrentLimit);

                Globals.TheHdw.Wait(SettlingTimeSec);

                // Configure drive pin: 0V / 5V levels
                digital.SelectFunction(SelectedFunction.Digital);

                // Apply levels and timing sheets before bursting
                digital.ApplyLevelsandTimings(LevelsSheet, TimingSheet);

                // Pre-discharge capacitor: drive LOW for 10τ
                digital.WriteStatic(PinState._0);
                Globals.TheHdw.Wait(DischargeTimeSec);

                // Burst the rise time measurement pattern:
                //   Drive pin outputs a LOW→HIGH step.
                //   Capture pin samples the XOR output at SamplePeriodSec intervals.
                //   XOR goes HIGH when Vc crosses VOL, LOW when Vc crosses VOH.
                //   Pulse width = rise time.
                digital.BurstPattern(
                    RiseTimePatternLabel,
                    selectDigitalFunction: true,
                    waitUntilDone: true,
                    timeoutinSeconds: CaptureTimeoutSec);

                // Fetch captured XOR output samples
                uint[][][] captureData = digital.FetchCaptureWaveform(
                    CaptureWaveformName,
                    samplesToRead: MaxCaptureSamples,
                    timeoutInSeconds: CaptureTimeoutSec);

                uint[][] perSiteData = digital.PerInstrumentToPerSiteData(captureData);

                // Calculate rise time: count consecutive HIGH samples × sample period
                double[] riseTimeResult = new double[perSiteData.Length];
                for (int site = 0; site < perSiteData.Length; site++)
                {
                    int highCount = 0;
                    if (perSiteData[site] != null)
                    {
                        highCount = perSiteData[site].Count(s => s == 1);
                    }
                    riseTimeResult[site] = highCount * SamplePeriodSec;
                }

                tsmContext.PublishPerSite(riseTimeResult, "POOL2_RiseTime_10_90");
            }
            finally
            {
                digital.WriteStatic(PinState._0);
                digital.Abort();

                smuVoh.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);
                smuVoh.ConfigureOutputEnabled(false);
                smuVoh.ConfigureOutputConnected(false);
                smuVoh.Abort();

                smuVol.ForceVoltage(voltageLevel: 0, currentLimit: SmuCurrentLimit);
                smuVol.ConfigureOutputEnabled(false);
                smuVol.ConfigureOutputConnected(false);
                smuVol.Abort();

                HMODControl.AllHMODReset(tsmContext);
            }
        }
    }
}
