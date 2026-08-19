using System;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.DAQmx;

namespace TestSteps.Modules
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
        public static void SL23Check(ISemiconductorModuleContext tsmContext)
        {
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, dc90PinGroup);
            HMODControl.AllHMODReset(tsmContext);

            // Turn ON K1-16 driven by DAQ
            DaqRelayDrive(tsmContext, dc90RelayPinGroup, true);
            // Turn ON RL13 and RL14 to connect resistor
            Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, true);

            smu.ConfigureSettings(
                apertureTime: 10e-3,
                apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(
                DCPowerMeasurementSense.Remote,
                initiateSessionAfter: false);
            smu.ConfigureVoltageLevelRange(voltageLevelRange: 6);
            smu.ConfigureOutputConnected();
            smu.ConfigureOutputEnabled();

            // Force current level and set measure voltage range
            smu.ForceCurrent(currentLevel: 1e-3, voltageLimit: 10);

            Globals.TheHdw.Wait(SettlingTimeSec);

            // Measure voltage
            smu.Measure(out double[] voltages, out double[] _);

            // Return to initial settings
            smu.ForceCurrent(currentLevel: 0, voltageLimit: 10);
            smu.Abort();
            smu.ConfigureOutputEnabled(false);
            smu.ConfigureOutputConnected(false);

            // Disconnect DC90_1A from the METER_HI option
            Relay.ControlRelay(tsmContext, new string[] { "RL13", "RL14" }, false);

            // Publish results
            smu.PinQueryContext.Publish(voltages, "Voltage");
        }

        /// <summary>
        /// Drives on-board relays via DAQmx digital output and waits for relay settling.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="doPin">DAQmx digital output pin group name.</param>
        /// <param name="state">True to close relays, false to open.</param>
        private static void DaqRelayDrive(ISemiconductorModuleContext tsmContext, string doPin, bool state)
        {
            DAQmx daqTask = InstrCtrl.PinsToDAQmxTasks(tsmContext, doPin);
            bool[] data = Enumerable.Repeat(state, daqTask.SSC.Length).ToArray();
            daqTask.WriteDigital(data, autoStart: true);
            Globals.TheHdw.Wait(RelaySettleSec);
        }
    }
}
