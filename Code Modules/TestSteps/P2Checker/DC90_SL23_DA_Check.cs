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
    public class DC90_SL23_DA_Check
    {
        private const string dc90PinGroup = "DC90_PINS";
        private const string dc90RelayPinGroup = "DC90_RELAY_DRIVE";
        private const string dc90DARelayPinGroup = "DC90_DA_RELAY_DRIVE";
        private const double SettlingTimeSec = 10e-3;

        /// <summary>
        /// Checks if DC90 (PXIe-4137) signals are able to reach the MFlex board.
        /// Done by forcing 5V on the resistor load located at the MFlex board checker.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL23DACheck(ISemiconductorModuleContext tsmContext, DCPowerMeasurementSense senseType)
        {
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, dc90PinGroup);
            HMODControl.AllHMODReset(tsmContext);

            DaqRelayDrive(tsmContext, dc90RelayPinGroup, true);

            // Turn ON K17-32 driven by DAQ
            DaqRelayDrive(tsmContext, dc90DARelayPinGroup, true);

            // Turn ON RL13 and RL14 to connect resistor
            Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, true);

            double[] currents = null;
            try
            {
                smu.ConfigureSettings(
                    apertureTime: 10e-3,
                    apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
                smu.ConfigureSense(
                    sense: senseType,
                    initiateSessionAfter: false);
                smu.ConfigureVoltageLevelRange(voltageLevelRange: 10);
                smu.ConfigureCurrentLimitRange(currentLimitRange: 20e-3);
                smu.ConfigureOutputConnected();
                smu.ConfigureOutputEnabled();

                // Force voltage level
                smu.ForceVoltage(voltageLevel: 5, currentLimit: 20e-3);

                Globals.TheHdw.Wait(SettlingTimeSec);

                // Measure voltage
                smu.Measure(out double[] _, out currents);
            }
            finally
            {
                // Return to initial settings
                smu.ForceVoltage(voltageLevel: 0, currentLimit: 20e-3);
                smu.ConfigureOutputEnabled(false);
                smu.ConfigureOutputConnected(false);
                smu.Abort();

                DaqRelayDrive(tsmContext, dc90RelayPinGroup, false);

                // Turn OFF K17-32 driven by DAQ
                DaqRelayDrive(tsmContext, dc90DARelayPinGroup, false);

                // Connect DC90_1A to the METER_HI option
                Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, false);
            }

            // Publish results
            smu.PinQueryContext.Publish(currents, "Current");
        }
    }
}
