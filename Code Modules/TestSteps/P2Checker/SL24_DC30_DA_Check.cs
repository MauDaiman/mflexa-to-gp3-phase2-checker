using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class SL24_DC30_DA_Check
    {
        private const double SettlingTimeSec = 5e-3;

        /// <summary>
        /// Checks DIB Access functionality for Slot24 DC30 channels by forcing 5V on two 1Kohm resistors
        /// located at the MFlex checker board. One 1Kohm is directly connected to the SMU-4162,
        /// the other 1Kohm gets connected through DIB Access.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL24DACheck(ISemiconductorModuleContext tsmContext)
        {
            double[] sl24OddI, sl24EvenI;

            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            HMODControl.HMOD1to4(tsmContext,
                HMOD_Data_2: HMODControl.RelayRange(9, 28),
                HMOD_Data_4: HMODControl.RelayID("K5, K7, K9, K11, K13, K15, K17, K19, K21, K23"));

            // HMOD 9 GNDS the LO from DGS
            HMODControl.HMOD5to10(tsmContext, HMOD_Data_9: HMODControl.RelayRange(6, 15));

            // Initiate Pin to Session
            DCPower sl24OddCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL24_ODD_CH");
            DCPower sl24EvenCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL24_EVEN_CH");

            // Configure and acquisition SMU's
            sl24OddCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl24OddCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl24OddCh.ConfigureVoltageLevelRange(24.0);
            sl24OddCh.ConfigureOutputConnected(true);
            sl24OddCh.ConfigureOutputEnabled(true);

            sl24EvenCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl24EvenCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl24EvenCh.ConfigureVoltageLevelRange(24.0);
            sl24EvenCh.ConfigureOutputConnected(true);
            sl24EvenCh.ConfigureOutputEnabled(true);

            try
            {
                // Force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
                sl24OddCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                sl24EvenCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, ODD channels
                sl24OddCh.Measure(out _, out sl24OddI);

                // Turn ON HMOD relays to connect EVEN channels to DIB Access
                HMODControl.HMOD1to4(tsmContext,
                    HMOD_Data_2: HMODControl.RelayRange(9, 28),
                    HMOD_Data_4: HMODControl.RelayID("K6, K8, K10, K12, K14, K16, K18, K20, K22, K24"));
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, EVEN channels
                sl24EvenCh.Measure(out _, out sl24EvenI);

                // Publish results
                sl24OddCh.PinQueryContext.Publish(sl24OddI, "Odd_Current");
                sl24EvenCh.PinQueryContext.Publish(sl24EvenI, "Even_Current");
            }
            finally
            {
                // Return to initial settings
                sl24OddCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);
                sl24EvenCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);

                // Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT24 DC30
                HMODControl.HMOD1to4(tsmContext, HMOD_Data_2: HMODControl.RelayRange(9, 28));

                sl24OddCh.ConfigureOutputEnabled(false);
                sl24OddCh.ConfigureOutputConnected(false);

                sl24EvenCh.ConfigureOutputEnabled(false);
                sl24EvenCh.ConfigureOutputConnected(false);

                sl24OddCh.Abort();
                sl24EvenCh.Abort();

                HMODControl.AllHMODReset(tsmContext);
            }
        }
    }
}
