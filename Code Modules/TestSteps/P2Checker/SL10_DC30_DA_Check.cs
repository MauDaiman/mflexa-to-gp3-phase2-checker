using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class SL10_DC30_DA_Check
    {
        private const double SettlingTimeSec = 5e-3;

        /// <summary>
        /// Checks DIB Access functionality for Slot10 DC30 channels by forcing 5V on two 1Kohm resistors
        /// located at the MFlex checker board. One 1Kohm is directly connected to the SMU-4162,
        /// the other 1Kohm gets connected through DIB Access.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL10DACheck(ISemiconductorModuleContext tsmContext)
        {
            double[] sl10OddI, sl10EvenI;

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

            // HMOD 7 GNDS the LO from DGS1
            HMODControl.HMOD5to10(tsmContext, HMOD_Data_7: HMODControl.RelayRange(6, 15));

            // Initiate Pin to Session
            DCPower sl10OddCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL10_ODD_CH");
            DCPower sl10EvenCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL10_EVEN_CH");

            // Configure and acquisition SMU's
            sl10OddCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl10OddCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl10OddCh.ConfigureVoltageLevelRange(24.0);
            sl10OddCh.ConfigureOutputConnected(true);
            sl10OddCh.ConfigureOutputEnabled(true);

            sl10EvenCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl10EvenCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl10EvenCh.ConfigureVoltageLevelRange(24.0);
            sl10EvenCh.ConfigureOutputConnected(true);
            sl10EvenCh.ConfigureOutputEnabled(true);

            try
            {
                // Force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
                sl10OddCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                sl10EvenCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, ODD channels
                sl10OddCh.Measure(out _, out sl10OddI);

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
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, EVEN channels
                sl10EvenCh.Measure(out _, out sl10EvenI);

                // Publish results
                sl10OddCh.PinQueryContext.Publish(sl10OddI, "Odd_Current");
                sl10EvenCh.PinQueryContext.Publish(sl10EvenI, "Even_Current");
            }
            finally
            {
                // Return to initial settings
                sl10OddCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);
                sl10EvenCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);

                // Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT10 DC30
                HMODControl.HMOD1to4(tsmContext,
                    HMOD_Data_1: HMODControl.RelayRange(21, 32),
                    HMOD_Data_2: HMODControl.RelayRange(1, 8));

                sl10OddCh.ConfigureOutputEnabled(false);
                sl10OddCh.ConfigureOutputConnected(false);

                sl10EvenCh.ConfigureOutputEnabled(false);
                sl10EvenCh.ConfigureOutputConnected(false);

                sl10OddCh.Abort();
                sl10EvenCh.Abort();
            }
        }
    }
}
