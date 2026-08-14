using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace TestSteps
{
    public static class DAQmxRelayDriver
    {
        /// <summary>
        /// Controls DAQmx DIO relay drivers using the TSM pin map.
        /// Sets individual relay lines high or low based on a 16-bit mask.
        /// </summary>
        /// <param name="tsmContext">TSM Semiconductor Module Context</param>
        /// <param name="pinGroup">Pin or pin group name mapped to DAQmx DO channels (e.g., "DC90_RELAYS")</param>
        /// <param name="relayMask">Bitmask where each bit corresponds to a relay line (1=ON, 0=OFF)</param>
        public static void SetRelays(ISemiconductorModuleContext tsmContext, string pinGroup, uint relayMask)
        {
            DAQmx daqTask = InstrCtrl.PinsToDAQmxTasks(tsmContext, pinGroup);

            bool[][] lineStates = new bool[daqTask.SSC.Length][];

            for (int i = 0; i < daqTask.SSC.Length; i++)
            {
                int chCount = daqTask.SSC[i].DAQmxTask.DOChannels.Count;
                lineStates[i] = BitsToArray(relayMask, chCount);
            }

            daqTask.WriteDigitalMultiChannel(lineStates, autoStart: true);
        }

        /// <summary>
        /// Turns ON specific relay lines by index (0-based).
        /// </summary>
        /// <param name="tsmContext">TSM Semiconductor Module Context</param>
        /// <param name="pinGroup">Pin or pin group name mapped to DAQmx DO channels</param>
        /// <param name="relayIndices">Array of relay line indices to turn ON (0-based)</param>
        public static void CloseRelays(ISemiconductorModuleContext tsmContext, string pinGroup, int[] relayIndices)
        {
            uint mask = 0;
            foreach (int idx in relayIndices)
                mask |= (1u << idx);

            SetRelays(tsmContext, pinGroup, mask);
        }

        /// <summary>
        /// Turns ON all relay lines in the pin group.
        /// </summary>
        public static void CloseAllRelays(ISemiconductorModuleContext tsmContext, string pinGroup)
        {
            SetRelays(tsmContext, pinGroup, 0xFFFFFFFF);
        }

        /// <summary>
        /// Turns OFF all relay lines in the pin group.
        /// </summary>
        public static void OpenAllRelays(ISemiconductorModuleContext tsmContext, string pinGroup)
        {
            SetRelays(tsmContext, pinGroup, 0x00000000);
        }

        /// <summary>
        /// Turns ON or OFF a single relay line by index.
        /// Note: This writes ALL lines at once; other lines will be set to OFF.
        /// Use SetRelays with a full mask if you need to preserve other line states.
        /// </summary>
        /// <param name="tsmContext">TSM Semiconductor Module Context</param>
        /// <param name="pinGroup">Pin or pin group name mapped to DAQmx DO channels</param>
        /// <param name="relayIndex">0-based relay line index</param>
        /// <param name="close">True to close (energize), False to open (de-energize)</param>
        public static void SetSingleRelay(ISemiconductorModuleContext tsmContext, string pinGroup, int relayIndex, bool close)
        {
            uint mask = close ? (1u << relayIndex) : 0u;
            SetRelays(tsmContext, pinGroup, mask);
        }

        private static bool[] BitsToArray(uint value, int width)
        {
            var bits = new bool[width];
            for (int i = 0; i < width; i++)
                bits[i] = ((value >> i) & 1u) != 0;
            return bits;
        }
    }
}
