using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.Modules
{
    public class SL24_DC30_DA_Check
    {
        private const double SettlingTimeSec = 1e-3;

        /// <summary>
        /// Checks DIB Access functionality for Slot24 DC30 channels by forcing 5V on two 1Kohm resistors
        /// located at the MFlex checker board. One 1Kohm is directly connected to the SMU-4162,
        /// the other 1Kohm gets connected through DIB Access.
        /// </summary>
        public static void SL24DACheck(ISemiconductorModuleContext tsmContext)
        {
            double[] sl24OddI, sl24EvenI;

            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            // HMOD1 - 1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            // HMOD2 - 0000 0000 0000 0000 0000 0000 1111 1111 = 255
            // HMOD3 - 0101 0101 0101 0101 0000 0000 0000 0000 = 1431633920
            // HMOD4 - 0000 0000 0000 0000 0000 0000 0000 0101 = 5
            HMODControl.HMOD1to4(tsmContext, 
                HMOD_Data_1: HMODControl.RelayRange(21, 32), 
                HMOD_Data_2: HMODControl.RelayRange(1, 8), 
                HMOD_Data_3: HMODControl.RelayID("K17, K19, K21, K23, K25, K27, K29, K31"), 
                HMOD_Data_4: HMODControl.RelayID("K1, K3"));

            // Initiate Pin to Session
            DCPower sl24Dc30 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL24_DC30");
            DCPower sl24OddCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL24_ODD_CH");
            DCPower sl24EvenCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL24_EVEN_CH");

            // Configure and acquisition SMU's
            sl24Dc30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl24Dc30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            sl24Dc30.ConfigureVoltageLevelRange(24.0);
            sl24Dc30.ConfigureOutputConnected(true);
            sl24Dc30.ConfigureOutputEnabled(true);

            // Force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
            sl24Dc30.ForceVoltage(voltageLevel: 5, currentLimit: 60e-3);
            Globals.TheHdw.Wait(SettlingTimeSec);

            // Measure current expected to be +10mA, ODD channels
            sl24OddCh.Measure(out _, out sl24OddI);

            // Turn ON HMOD relays to connect EVEN channels to DIB Access
            // HMOD1 - 1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            // HMOD2 - 0000 0000 0000 0000 0000 0000 1111 1111 = 255
            // HMOD3 - 1010 1010 1010 1010 0000 0000 0000 0000 = 2863267840
            // HMOD4 - 0000 0000 0000 0000 0000 0000 0000 1010 = 10
            HMODControl.HMOD1to4(tsmContext, 
                HMOD_Data_1: HMODControl.RelayRange(21, 32), 
                HMOD_Data_2: HMODControl.RelayRange(1, 8), 
                HMOD_Data_3: HMODControl.RelayID("K18, K20, K22, K24, K26, K28, K30, K32"), 
                HMOD_Data_4: HMODControl.RelayID("K2, K4"));

            // Measure current expected to be +10mA, EVEN channels
            sl24EvenCh.Measure(out _, out sl24EvenI);

            // Return to initial settings
            sl24Dc30.ForceVoltage(voltageLevel: 0, currentLimit: 60e-3);

            // Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT24 DC30
            HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: HMODControl.RelayRange(1, 20));

            sl24Dc30.Abort();
            sl24Dc30.ConfigureOutputEnabled(false);
            sl24Dc30.ConfigureOutputConnected(false);

            // Publish results
            sl24OddCh.PinQueryContext.Publish(sl24OddI, "Odd_Current");
            sl24EvenCh.PinQueryContext.Publish(sl24EvenI, "Even_Current");
        }
    }
}
