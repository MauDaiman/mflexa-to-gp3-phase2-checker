using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl
{
    /// <summary>
    /// Class used to manage Instrument Control methods.
    /// </summary>
    public partial class InstrCtrl
    {
        /// <summary>
        /// Dictionaty definition for System Pins.
        /// </summary>
        public static ConcurrentDictionary<string, byte> SystemPins = new ConcurrentDictionary<string, byte>();

        /// <summary>
        /// Dictionaty definition for DUT Pins.
        /// </summary>
        public static ConcurrentDictionary<string, byte> DutPins = new ConcurrentDictionary<string, byte>();

        /// <summary>
        /// Initializes all generated NI-DCPower, NI-Digital, NI-Scope, NI-Dmm and Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="dcPowerPowerLineFrequencyinHz">Power Line frequency in Hz. Default vbalue is set to 60Hz</param>
        /// <param name="dcPoweruseSoftwareMeasureTriggers">Default value is set to False (not using software triggers).</param>
        public static void InitializeAllSessions(ISemiconductorModuleContext tsmContext, double dcPowerPowerLineFrequencyinHz = 60.0, bool dcPoweruseSoftwareMeasureTriggers = false)
        {
            Parallel.Invoke(() => InstrCtrl.InitDCPowerSessions(tsmContext, dcPowerPowerLineFrequencyinHz, dcPoweruseSoftwareMeasureTriggers),
                            () => InstrCtrl.InitDigitalSessions(tsmContext),
                            () => InstrCtrl.InitScopeSessions(tsmContext),
                            () => InstrCtrl.InitCustomRelaySessions(tsmContext),
                            () => InstrCtrl.InitRelaySessions(tsmContext),
                            () => InstrCtrl.InitDmmSessions(tsmContext),
                            () => InstrCtrl.SetDAQmxTasks(tsmContext)
                            );
        }

        /// <summary>
        /// Closes all active NI-DCPower, NI-Digital, NI-Scope, NI-Dmm and Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseAllSessions(ISemiconductorModuleContext tsmContext)
        {
            Parallel.Invoke(() => InstrCtrl.CloseDCPowerSessions(tsmContext),
                            () => InstrCtrl.CloseDigitalSessions(tsmContext),
                            () => InstrCtrl.CloseScopeSessions(tsmContext),
                            () => InstrCtrl.CloseCustomRelaySessions(tsmContext),
                            () => InstrCtrl.CloseRelaySessions(tsmContext),
                            () => InstrCtrl.CloseDmmSessions(tsmContext),
                            () => InstrCtrl.ClearDAQmxTasks(tsmContext)
                            );
        }

    }


}