using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class SL04_DC30_DA_Check
    {
        private const double SettlingTimeSec = 5e-3;

        /// <summary>
        /// Checks DIB Access functionality for Slot04 DC30 channels by forcing 5V on two 1Kohm resistors
        /// located at the MFlex checker board. One 1Kohm is directly connected to the SMU-4162,
        /// the other 1Kohm gets connected through DIB Access.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL04DACheck(ISemiconductorModuleContext tsmContext)
        {
            double[] sl04OddI, sl04EvenI;

            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            // HMOD1 - DB1 to DB20 -> ON          = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            // HMOD2 - DB29 and DB31 -> ON        = 0101 0000 0000 0000 0000 0000 0000 0000  = 1342177280
            // HMOD3 - DB1 to DB16 ODD bits -> ON = 0000 0000 0000 0000 0101 0101 0101 0101  = 21845
            HMODControl.HMOD1to4(tsmContext, 
                HMOD_Data_1: HMODControl.RelayRange(1, 20), 
                HMOD_Data_2: HMODControl.RelayID("K29, K31"), 
                HMOD_Data_3: HMODControl.RelayID("K1, K3, K5, K7, K9, K11, K13, K15"));

            // HMOD 5 GNDS the LO from DGS
            HMODControl.HMOD5to10(tsmContext, HMOD_Data_5: HMODControl.RelayRange(6, 15));

            // Initiate Pin to Session
            DCPower sl04OddCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL04_ODD_CH");
            DCPower sl04EvenCh = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SL04_EVEN_CH");

            // Configure and acquisition SMU's
            sl04OddCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl04OddCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl04OddCh.ConfigureVoltageLevelRange(24.0);
            sl04OddCh.ConfigureOutputConnected(true);
            sl04OddCh.ConfigureOutputEnabled(true);

            sl04EvenCh.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            sl04EvenCh.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            sl04EvenCh.ConfigureVoltageLevelRange(24.0);
            sl04EvenCh.ConfigureOutputConnected(true);
            sl04EvenCh.ConfigureOutputEnabled(true);

            try
            {
                // Force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
                sl04OddCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                sl04EvenCh.ForceVoltage(voltageLevel: 5, currentLimit: 50e-3);
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, ODD channels
                sl04OddCh.Measure(out _, out sl04OddI);

                // Turn ON HMOD relays to connect EVEN channels to DIB Access
                // HMOD1 - DB1 to DB20 -> ON          = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
                // HMOD2 - DB30 and DB32 -> ON        = 1010 0000 0000 0000 0000 0000 0000 0000  = 2684354560
                // HMOD3 - DB1 to DB16 EVEN bits -> ON = 0000 0000 0000 0000 1010 1010 1010 1010  = 43690
                HMODControl.HMOD1to4(tsmContext,
                    HMOD_Data_1: HMODControl.RelayRange(1, 20),
                    HMOD_Data_2: HMODControl.RelayID("K30, K32"),
                    HMOD_Data_3: HMODControl.RelayID("K2, K4, K6, K8, K10, K12, K14, K16"));
                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure current expected to be +10mA, EVEN channels
                sl04EvenCh.Measure(out _, out sl04EvenI);

                // Publish results
                sl04OddCh.PinQueryContext.Publish(sl04OddI, "Odd_Current");
                sl04EvenCh.PinQueryContext.Publish(sl04EvenI, "Even_Current");
            }
            finally
            {
                // Return to initial settings
                sl04OddCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);
                sl04EvenCh.ForceVoltage(voltageLevel: 0, currentLimit: 50e-3);

                // Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT4 DC30
                HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: HMODControl.RelayRange(1, 20));

                sl04OddCh.ConfigureOutputEnabled(false);
                sl04OddCh.ConfigureOutputConnected(false);

                sl04EvenCh.ConfigureOutputEnabled(false);
                sl04EvenCh.ConfigureOutputConnected(false);

                sl04OddCh.Abort();
                sl04EvenCh.Abort();
            }
        }
    }
}
