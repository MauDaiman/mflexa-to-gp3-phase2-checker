using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace TestSteps.Modules
{
    public class DC30_SL10_Check
    {
        private const string Sl10PinGroup = "SL10_DC30";
        private const double SettlingTimeSec = 1e-3;

        /// <summary>
        /// Checks if SMU-4162/63 signals are able to reach the MFlex board.
        /// Done by forcing +1mA on the resistor load located at the MFlex board checker.
        /// </summary>
        public static void SL10Check(ISemiconductorModuleContext tsmContext)
        {
            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);

            // Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            // HMOD1 - 1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            // HMOD2 - 0000 0000 0000 0000 0000 0000 1111 1111 = 255
            HMODControl.HMOD1to4(tsmContext, 
                HMOD_Data_1: HMODControl.RelayRange(21, 32), 
                HMOD_Data_2: HMODControl.RelayRange(1, 8));

            // Initiate Pin to Session
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, Sl10PinGroup);

            // Configure and acquisition SMU's
            smu.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            smu.ConfigureCurrentLevelRange(10e-3); // set current range
            smu.ConfigureOutputConnected(true);
            smu.ConfigureOutputEnabled(true);
            smu.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohm resistor

            Globals.TheHdw.Wait(SettlingTimeSec);

            // Measure voltage, expected to be +1V
            // At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4).
            // All DGS are also connected to GND by default.
            smu.Measure(out double[] measDC30SL10, out _);

            // Return to initial settings
            smu.ForceCurrent(currentLevel: 0, voltageLimit: 24);
            smu.Abort();
            smu.ConfigureOutputEnabled(false);
            smu.ConfigureOutputConnected(false);

            // Publish results
            smu.PinQueryContext.Publish(measDC30SL10, "Voltages");
        }
    }
}
