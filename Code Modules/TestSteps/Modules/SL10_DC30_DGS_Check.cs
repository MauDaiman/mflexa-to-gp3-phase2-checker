using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.Modules
{
    public class SL10_DC30_DGS_Check
    {
        private const double SettlingTimeSec = 1e-3;

        /// <summary>
        /// Checks DGS connectivity of each SMU-4162/63 channel. Each SMU channel forces current
        /// on the 1Kohm load located on the Checker board. Resulting voltage is measured at
        /// different DGS1/2/3/4 reference.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL10DGSCheck(ISemiconductorModuleContext tsmContext)
        {
            double[] MeasDC30_SL10_DGS1, MeasDC30_SL10_DGS2, MeasDC30_SL10_DGS3, MeasDC30_SL10_DGS4;

            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            // HMOD1 - 1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            // HMOD2 - 0000 0000 0000 0000 0000 0000 1111 1111 = 255
            HMODControl.HMOD1to4(tsmContext, 
                HMOD_Data_1: HMODControl.RelayRange(21, 32), 
                HMOD_Data_2: HMODControl.RelayRange(1, 8));

            // Initiate Pin to Session
            DCPower SL10_DC30 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL10_DC30");

            // Configure and acquisition SMU's
            SL10_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL10_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL10_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL10_DC30.ConfigureOutputConnected(true);
            SL10_DC30.ConfigureOutputEnabled(true);
            SL10_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohm resistor

            // Connect LO_S of SLOT10 DC30's to DGS1 only, disconnect LO_s channels from DGS2/3/4
            // HMOD7 - 0000 0001 1111 0000 0111 1111 1110 0000 = 32538592
            // HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODControl.HMOD5to10(tsmContext, 
                HMOD_Data_7: HMODControl.RelayID("K6, K7, K8, K9, K10, K11, K12, K13, K14, K15, K21, K22, K23, K24, K25"), 
                HMOD_Data_8: HMODControl.RelayID("K9, K10, K11, K27, K28"));

            HMODControl.CHMOD1to13(tsmContext, HMOD_Data_1: HMODControl.RelayID("K1")); // connect SL10 DGS1 to 1V reference
            Globals.TheHdw.Wait(SettlingTimeSec);

            // Measure voltage, expected to be +1V - 1V(DGS = 1V REF) = 0V
            // At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4).
            // All DGS are also connected to GND by default.
            SL10_DC30.Measure(out MeasDC30_SL10_DGS1, out _); // pin group measurement

            // Connect LO_S of SLOT10 DC30's to DGS2 only, disconnect LO_s channels from DGS1/3/4
            // HMOD7 - 0011 1110 0000 1111 1000 0000 0001 1111 = 1041203231
            // HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODControl.HMOD5to10(tsmContext, 
                HMOD_Data_7: HMODControl.RelayID("K1, K2, K3, K4, K5, K16, K17, K18, K19, K20, K26, K27, K28, K29, K30"), 
                HMOD_Data_8: HMODControl.RelayID("K9, K10, K11, K27, K28"));

            // Measure voltage, expected to be +1V - 2V(DGS = 2V REF) = -1V
            HMODControl.CHMOD1to13(tsmContext, HMOD_Data_1: HMODControl.RelayID("K2")); // connect SL10 DGS2 to 2V reference
            Globals.TheHdw.Wait(SettlingTimeSec);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS2, out _); // pin group measurement

            // Connect LO_S of SLOT10 DC30's to DGS3 only, disconnect LO_s channels from DGS1/2/4
            // HMOD7 - 1100 0001 1111 0000 0000 0000 0001 1111 = 3253731359
            // HMOD8 - 0000 1100 0000 0000 0001 1000 1111 1111 = 201332991
            HMODControl.HMOD5to10(tsmContext, 
                HMOD_Data_7: HMODControl.RelayID("K1, K2, K3, K4, K5, K21, K22, K23, K24, K25, K31, K32"), 
                HMOD_Data_8: HMODControl.RelayID("K1, K2, K3, K4, K5, K6, K7, K8, K12, K13, K27, K28"));

            // Measure voltage, expected to be +1V - 3V(DGS = 3V REF) = -2V
            HMODControl.CHMOD1to13(tsmContext, HMOD_Data_1: HMODControl.RelayID("K3")); // connect SL10 DGS3 to 3V reference
            Globals.TheHdw.Wait(SettlingTimeSec);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS3, out _); // pin group measurement

            // Connect LO_S of SLOT10 DC30's to DGS4 only, disconnect LO_s channels from DGS1/2/3
            // HMOD7 - 0000 0001 1111 0000 0000 0000 0001 1111 = 32505887
            // HMOD8 - 0000 0011 1111 1111 1110 0111 0000 0000 = 67102464
            HMODControl.HMOD5to10(tsmContext, 
                HMOD_Data_7: HMODControl.RelayID("K1, K2, K3, K4, K5, K21, K22, K23, K24, K25"), 
                HMOD_Data_8: HMODControl.RelayID("K9, K10, K11, K14, K15, K16, K17, K18, K19, K20, K21, K22, K23, K24, K25, K26"));

            // Measure voltage, expected to be +1V - 4V(DGS = 4V REF) = -3V
            HMODControl.CHMOD1to13(tsmContext, HMOD_Data_1: HMODControl.RelayID("K4")); // connect SL10 DGS4 to 4V reference
            Globals.TheHdw.Wait(SettlingTimeSec);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS4, out _); // pin group measurement

            // Return to initial settings
            SL10_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24);

            SL10_DC30.Abort();
            SL10_DC30.ConfigureOutputEnabled(false);
            SL10_DC30.ConfigureOutputConnected(false);

            // Publish results
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS1, "SL10_DC30_CHANNELS_DGS1");
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS2, "SL10_DC30_CHANNELS_DGS2");
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS3, "SL10_DC30_CHANNELS_DGS3");
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS4, "SL10_DC30_CHANNELS_DGS4");
        }
    }
}
