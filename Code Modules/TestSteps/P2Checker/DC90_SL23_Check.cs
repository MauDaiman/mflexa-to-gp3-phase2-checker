using System;
using System.Linq;
using static TestSteps.Common.DAQmxRelay;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.DAQmx;

namespace TestSteps.P2Checker
{
    public class DC90_SL23_Check
    {
        private const string dc90PinGroup = "DC90_PINS";
        private const string dc90RelayPinGroup = "DC90_RELAY_DRIVE";
        private const double SettlingTimeSec = 1e-3;
        private const double RelaySettleSec = 5e-3;

        /// <summary>
        /// Checks if DC90 (PXIe-4137) signals are able to reach the MFlex board.
        /// Done by forcing +1mA on the resistor load located at the MFlex board checker.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL23Check(ISemiconductorModuleContext tsmContext, DCPowerMeasurementSense senseType)
        {
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, dc90PinGroup);
            HMODControl.AllHMODReset(tsmContext);

            DaqRelayDrive(tsmContext, dc90RelayPinGroup, true);

            // Turn ON RL13 and RL14 to connect resistor
            Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, true);
            Globals.TheHdw.Wait(RelaySettleSec);

            try
            {
                smu.ConfigureSettings(
                    apertureTime: 10e-3,
                    apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
                smu.ConfigureSense(
                    sense: senseType,
                    initiateSessionAfter: false);
                smu.ConfigureVoltageLevelRange(voltageLevelRange: 6);
                smu.ConfigureCurrentLevelRange(currentLevelRange: 10e-3);
                smu.ConfigureOutputConnected();
                smu.ConfigureOutputEnabled();

                // Force current level and set measure voltage range
                smu.ForceCurrent(currentLevel: 1e-3, voltageLimit: 10);

                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure voltage
                smu.Measure(out double[] voltages, out double[] _);

                // Publish results
                smu.PinQueryContext.Publish(voltages, "Voltage");
            }
            finally
            {
                // Return to initial settings
                smu.ForceCurrent(currentLevel: 0, voltageLimit: 10);
                smu.ConfigureOutputEnabled(false);
                smu.ConfigureOutputConnected(false);
                smu.Abort();

                DaqRelayDrive(tsmContext, dc90RelayPinGroup, false);

                // Disconnect DC90_1A from the METER_HI option
                Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, false);
            }
        }
    }
}
