using System;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDmm;

namespace TestSteps.Common
{
    public class DAQmxRelay
    {
        private const string dc90PinGroup = "DC90_PINS";
        private const string K1 = "P127_6368_DIG0_P0_0";
        private const string K2 = "P127_6368_DIG0_P0_1";
        private const double SettlingTimeSec = 1e-3;
        private const double RelaySettleSec = 5e-3;

        /// <summary>
        /// Drives on-board relays via DAQmx digital output and waits for relay settling.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="doPin">DAQmx digital output pin group name.</param>
        /// <param name="state">True to close relays, false to open.</param>
        public static void DaqRelayDrive(ISemiconductorModuleContext tsmContext, string doPin, bool state)
        {
            DAQmx daqTask = InstrCtrl.PinsToDAQmxTasks(tsmContext, doPin);
            bool[] data = Enumerable.Repeat(state, daqTask.SSC.Length).ToArray();
            daqTask.WriteDigital(data, autoStart: true);
            Globals.TheHdw.Wait(RelaySettleSec);
        }

        /// <summary>
        /// Drives on-board relays via DAQmx digital output and waits for relay settling.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="doPin">DAQmx digital output pin group name.</param>
        /// <param name="state">True to close relays, false to open.</param>
        public static void DaqRelayDrive(ISemiconductorModuleContext tsmContext, string[] doPins, bool state)
        {
            foreach (var doPin in doPins)
            {
                DaqRelayDrive(tsmContext, doPin, state);
            }
        }
    }
}
