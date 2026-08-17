using System;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.DAQmx;

namespace TestSteps.Modules
{
    public class DC90_SL23_Check
    {
        public const string dc90PinGroup = "DC90_PINS";
        public const string dc90RelayPinGroup = "DC90_RELAY_DRIVE";

        public static double settlingTime = 1e-3;

        public static void SL23Check(ISemiconductorModuleContext tsmContext)
        {
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, dc90PinGroup);
            HMODControl.AllHMODReset(tsmContext);

            OnBoardRelayDrive(tsmContext, dc90RelayPinGroup, true); // Turn ON K1-16 driven by DAQ
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL13", "RL14" }, true); // Turn ON RL13 and RL14 to connect resistor 

            smu.ConfigureSettings(
                apertureTime: 10e-3,
                apertureTimeUnitsinSeconds:
                DCPowerMeasureApertureTimeUnits.Seconds);
            smu.ConfigureSense(
                DCPowerMeasurementSense.Remote,
                initiateSessionAfter: false);

            // Force current level and set measure voltage range
            smu.ForceCurrent(
                currentLevel: 1e-3, 
                voltageLimit: 10);
            smu.ConfigureVoltageLevelRange(voltageLevelRange: 6);
            smu.Initiate();

            smu.ConfigureOutputConnected();
            smu.ConfigureOutputEnabled();

            Globals.TheHdw.Wait(settlingTime);

            smu.Measure(out double[] voltages, out double[] _); // Measure voltage

            smu.ForceCurrent( // Force current level and set measure voltage range
                currentLevel: 0, 
                voltageLimit: 10);
            smu.Abort();
            smu.ConfigureOutputEnabled(false);
            smu.ConfigureOutputConnected(false);

            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL13", "RL14" }, false); // Connect DC90_1A for the METER_HI option

            smu.PinQueryContext.Publish(voltages, "Voltage");
        }

        public static void OnBoardRelayDrive(ISemiconductorModuleContext tsmContext, string doPin, bool state)
        {
            DAQmx daqTask = InstrCtrl.PinsToDAQmxTasks(tsmContext, doPin);
            bool[] data = Enumerable.Repeat(state, daqTask.SSC.Length).ToArray();
            daqTask.WriteDigital(data, autoStart: true);
            Globals.TheHdw.Wait(5 * Globals.mS);
        }
    }
    }
}
