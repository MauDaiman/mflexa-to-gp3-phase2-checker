using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.P2Checker
{
    public class DC30_SL04_Check
    {
        private const string Sl04PinGroup = "SL04_DC30";
        private const double SettlingTimeSec = 5e-3;

        /// <summary>
        /// Checks if SMU-4162/63 signals are able to reach the MFlex board.
        /// Done by forcing +1mA on the resistor load located at the MFlex board checker.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL04Check(ISemiconductorModuleContext tsmContext)
        {
            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot04 DC30 channels
            // HMOD1 - DB1 to DB20 -> ON = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            HMODControl.HMOD1to4(tsmContext, HMOD_Data_1: HMODControl.RelayRange(1, 20));

            // HMOD 5 GNDS the LO from DGS
            HMODControl.HMOD5to10(tsmContext, HMOD_Data_5: HMODControl.RelayRange(6, 15));

            Globals.TheHdw.Wait(SettlingTimeSec);

            // Initiate Pin to Session
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, Sl04PinGroup);

            // Configure and acquisition SMU's
            smu.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            smu.ConfigureCurrentLevelRange(10e-3); // set current range
            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);

            try
            {
                smu.ForceCurrent(currentLevel: 1e-3, voltageLimit: 6); // force current on 1Kohm resistor

                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure voltage, expected to be +1V
                // At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4).
                // All DGS are also connected to GND by default.
                smu.Measure(out double[] measDC30SL04, out _);

                // Publish results
                smu.PinQueryContext.Publish(measDC30SL04, "Voltages");
            }
            finally
            {
                // Return to initial settings
                smu.ForceCurrent(currentLevel: 0, voltageLimit: 6);
                smu.ConfigureOutputEnabled(false);
                smu.ConfigureOutputConnected(false);
                smu.Abort();
                HMODControl.AllHMODReset(tsmContext);
            }
        }
    }
}
