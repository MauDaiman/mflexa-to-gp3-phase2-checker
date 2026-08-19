using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;
using NationalInstruments.ModularInstruments.NIDigital;

namespace TestSteps.Common
{
    /// <summary>
    /// Controls the Checker Board HMOD shift-register chain (6 HMODs: 1x 32-ch + 5x 72-ch = 392 bits).
    /// </summary>
    public static class HMODCtrl
    {

        private const string allHMODResetPin = "RESET_PINS";
        private static string allHMODDigiPins = "HMOD_DIGITAL_PINS";
        private static string hmodTxResetPin = "HMOD_RESET";
        private static string hmodCxResetPin = "CHMOD_RESET";
        private static string dataLevels = "HMOD";
        private static string dataTimings = "HMOD";
        public static string hmodTxDataPin = "HMOD_DIN";
        public static string hmodCxDataPin = "CHMOD_DIN";

        /// <summary>
        /// Shifts data into the Checker Board HMOD chain (392 bits total).
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="hmod1Data">32-bit data for Checker HMOD1 (K1–K32).</param>
        /// <param name="hmod2Data">72-bit data for Checker HMOD2 (K1–K72), as uint[3].</param>
        /// <param name="hmod3Data">72-bit data for Checker HMOD3 (K1–K72), as uint[3].</param>
        /// <param name="hmod4Data">72-bit data for Checker HMOD4 (K1–K72), as uint[3].</param>
        /// <param name="hmod5Data">72-bit data for Checker HMOD5 (K1–K72), as uint[3].</param>
        /// <param name="hmod6Data">72-bit data for Checker HMOD6 (K1–K72), as uint[3].</param>
        public static void CHMOD1to13(ISemiconductorModuleContext tsmContext,
            uint hmod1Data = 0,       // 32-bit HMOD (K1–K32)
            uint[] hmod2Data = null,  // 72-bit HMOD (K1–K72) → uint[3]
            uint[] hmod3Data = null,
            uint[] hmod4Data = null,
            uint[] hmod5Data = null,
            uint[] hmod6Data = null)
        {
            // Build the full 392-bit waveform (1 bit per sample, MSB first)
            // Shift order: last in chain shifts first → HMOD6, HMOD5, ..., HMOD1
            uint[] waveform = new uint[392];
            int offset = 0;
            ExpandBits(waveform, ref offset, hmod6Data, 72);
            ExpandBits(waveform, ref offset, hmod5Data, 72);
            ExpandBits(waveform, ref offset, hmod4Data, 72);
            ExpandBits(waveform, ref offset, hmod3Data, 72);
            ExpandBits(waveform, ref offset, hmod2Data, 72);
            ExpandBits(waveform, ref offset, hmod1Data, 32);

            Digital CHMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxDataPin);
            CHMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");

            // Write 1-bit-per-sample waveform via raw session 
            foreach (var ssc in CHMOD_DIN.SSC)
            {
                ssc.Session.SourceWaveforms.WriteBroadcast("CHMOD", waveform);
            }
            CHMOD_DIN.BurstPattern("CHMOD_pat");
            Globals.TheHdw.Wait(5e-3);
        }

        private static void ExpandBits(uint[] waveform, ref int offset, uint[] data, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                int bitIndex = bitCount - 1 - i; // MSB first
                int wordIndex = bitIndex / 32;
                int bitPosition = bitIndex % 32;
                if (data != null && wordIndex < data.Length && ((data[wordIndex] >> bitPosition) & 1u) != 0)
                    waveform[offset] = 1;
                offset++;
            }
        }

        private static void ExpandBits(uint[] waveform, ref int offset, uint data, int bitCount)
        {
            ExpandBits(waveform, ref offset, new uint[] { data }, bitCount);
        }

        /// <summary>
        /// Converts a comma-separated relay string (e.g. "K1, K3, K72") into a uint[3] bitmask
        /// suitable for 72-channel HMOD data parameters.
        /// </summary>
        /// <param name="relayToggle">Comma or semicolon-separated relay names (K1–K72).</param>
        public static uint[] RelayID72(string relayToggle)
        {
            uint[] result = new uint[3]; // 72 bits = 3 words
            foreach (var raw in relayToggle.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var digits = new string(raw.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out int n) && n >= 1 && n <= 72)
                {
                    result[(n - 1) / 32] |= (1u << ((n - 1) % 32));
                }
            }
            return result;
        }
    }
}
