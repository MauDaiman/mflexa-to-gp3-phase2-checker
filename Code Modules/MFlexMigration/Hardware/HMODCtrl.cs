using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.ModularInstruments.NIDigital;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class HMODControl
    {
        //Convert HMOD K* relay values to UNSIGNED INT32
        private static uint grp_xptsw_pins_hmod14 = RelayID("K5, K6, K7, K12, K13, K14, K15, K21, K22, K23, K24, K25, K26");
        private static uint grp_xptsw_pins_hmod15 = RelayID("K5, K6, K7, K21, K22, K23");
        private static uint grp_xptsw_pins_hmod16 = RelayID("K21, K22, K23");
        private static uint grp_xptsw_pins_hmod17 = RelayID("K5, K6, K7, K21, K22, K23");
        private static uint grp_xptsw_pins_hmod18 = RelayID("K5, K6, K7");
        private static uint HMOD1_site0_row_col_pins = RelayID("K2, K3, K5, K6, K8, K11, K12, K17, K18, K19, K20, K24, K25, K27, K28, K29, K30, K31"); //K8 - X25
        private static uint HMOD2_site0_row_col_pins = RelayID("K2, K3, K4, K5, K7, K17, K18, K19, K26");
        private static uint HMOD1_site1_row_col_pins = RelayID("K1, K4, K9, K10, K13, K14, K15, K16, K32");
        private static uint HMOD2_site1_row_col_pins = RelayID("K6, K9, K10, K11, K12, K13, K14, K15, K16, K20, K21, K22, K23, K24, K25, K27, K28"); //K16 - X25
        private static uint HMOD5_dgs = RelayID("K6, K8, K9, K10, K11, K12, K13, K14, K15, K21, K23, K24, K25");
        private static uint HMOD6_dgs = RelayID("K9, K10, K11, K27, K28");
        private static uint HMOD7_dgs = RelayID("K7, K8, K9, K10, K11, K12, K13, K14, K15, K22, K23, K24, K25");
        private static uint HMOD8_dgs = RelayID("K9, K10, K11, K27, K28");
        private static uint HMOD9_dgs = RelayID("K6, K7, K8, K9, K10, K11, K12, K13, K14, K15, K21, K22, K23, K24, K25");
        private static uint HMOD10_dgs = RelayID("K6, K9, K10, K11, K21, K24, K27, K28");

        private static uint chmod1 = RelayID("K1, K32");
        private static uint[] chmod2 = RelayID72("K1, K3, K72");
        private static uint[] chmod3 = RelayID72("K1, K3, K70, K72");
        private static uint[] chmod4 = RelayID72("K1, K3, K70, K5, K72");
        private static uint[] chmod5 = RelayID72("K1, K3, K70, K5, K68, K72");
        private static uint[] chmod6 = RelayID72("K1, K3, K70, K5, K68, K7, K72");

        private const string allHMODResetPin = "RESET_PINS";
        private const string allHMODDigiPins = "HMOD_DIGITAL_PINS";
        private static string allCHMODDigiPins = "CHMOD_DIGITAL_PINS";
        private const string hmodTxResetPin = "HMOD_RESET";
        private const string hmodCxResetPin = "CHMOD_RESET";
        private const string dataLevels = "HMOD";
        private const string dataTimings = "HMOD";
        private const string hmodTxDataPin = "HMOD_DIN";
        private const string hmodCxDataPin = "CHMOD_DIN";

        /// <summary>
        /// Initializes all HMOD digital pins, applies levels/timings, and sets default relay configurations.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void HMODInitialization(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxResetPin);
            HMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODDigiPins);
            HMOD_DigiPins.ApplyLevelsandTimings(dataLevels, dataTimings);

            HMOD1to4(tsmContext,
                HMOD_Data_1: HMOD1_site0_row_col_pins + HMOD1_site1_row_col_pins,
                HMOD_Data_2: HMOD2_site0_row_col_pins + HMOD2_site1_row_col_pins);

            //##grp_xptsw_pins##
            //##Site0## ##Site1##
            HMOD14to18(tsmContext,
                HMOD_Data_14: grp_xptsw_pins_hmod14,
                HMOD_Data_15: grp_xptsw_pins_hmod15,
                HMOD_Data_16: grp_xptsw_pins_hmod16,
                HMOD_Data_17: grp_xptsw_pins_hmod17,
                HMOD_Data_18: grp_xptsw_pins_hmod18);

            HMOD5to10(tsmContext,
                HMOD_Data_5: HMOD5_dgs,
                HMOD_Data_6: HMOD6_dgs,
                HMOD_Data_7: HMOD7_dgs,
                HMOD_Data_8: HMOD8_dgs,
                HMOD_Data_9: HMOD9_dgs,
                HMOD_Data_10: HMOD10_dgs);
        }

        /// <summary>
        /// Resets the Tx Board HMODs by pulsing the HMOD_RESET pin low then high.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void THMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }
        
        /// <summary>
        /// Resets the Checker Board HMODs by pulsing the CHMOD_RESET pin low then high.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void CHMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }
        
        /// <summary>
        /// Resets all HMODs (both Tx Board and Checker Board) by pulsing the combined reset pin.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void AllHMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }

        /// <summary>
        /// Shifts 32-bit data into Tx Board HMOD1 through HMOD4.
        /// </summary>
        public static void HMOD1to4(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_1 = 0,
            uint HMOD_Data_2 = 0,
            uint HMOD_Data_3 = 0,
            uint HMOD_Data_4 = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_1_4", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_1_4", new uint[] { HMOD_Data_4, HMOD_Data_3, HMOD_Data_2, HMOD_Data_1 });
            HMOD_DIN.BurstPattern("HMOD_1_4_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
        /// <summary>
        /// Shifts 32-bit data into Tx Board HMOD5 through HMOD10.
        /// </summary>
        public static void HMOD5to10(ISemiconductorModuleContext tsmContext,
           uint HMOD_Data_5 = 0,
           uint HMOD_Data_6 = 0,
           uint HMOD_Data_7 = 0,
           uint HMOD_Data_8 = 0,
           uint HMOD_Data_9 = 0,
           uint HMOD_Data_10 = 0
           )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_5_10", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_5_10", new uint[] { HMOD_Data_10, HMOD_Data_9, HMOD_Data_8, HMOD_Data_7, HMOD_Data_6, HMOD_Data_5 });
            HMOD_DIN.BurstPattern("HMOD_5_10_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        /// <summary>
        /// Shifts 32-bit data into Tx Board HMOD11 through HMOD13.
        /// </summary>
        public static void HMOD11to13(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_11 = 0,
            uint HMOD_Data_12 = 0,
            uint HMOD_Data_13 = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_11_13", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_11_13", new uint[] { HMOD_Data_13, HMOD_Data_12, HMOD_Data_11 });
            HMOD_DIN.BurstPattern("HMOD_11_13_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        /// <summary>
        /// Shifts 32-bit data into Tx Board HMOD14 through HMOD18.
        /// </summary>
        public static void HMOD14to18(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_14 = 0,
            uint HMOD_Data_15 = 0,
            uint HMOD_Data_16 = 0,
            uint HMOD_Data_17 = 0,
            uint HMOD_Data_18 = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_14_18", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_14_18", new uint[] { HMOD_Data_18, HMOD_Data_17, HMOD_Data_16, HMOD_Data_15, HMOD_Data_14 });
            HMOD_DIN.BurstPattern("HMOD_14_18_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
        /// <summary>
        /// Shifts 32-bit data into Tx Board HMOD19 through HMOD23.
        /// </summary>
        public static void HMOD19to23(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_19 = 0,
            uint HMOD_Data_20 = 0,
            uint HMOD_Data_21 = 0,
            uint HMOD_Data_22 = 0,
            uint HMOD_Data_23 = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_19_23", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_19_23", new uint[] { HMOD_Data_23, HMOD_Data_22, HMOD_Data_21, HMOD_Data_20, HMOD_Data_19 });
            HMOD_DIN.BurstPattern("HMOD_19_23_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
 
        /// <summary>
        /// Resets all HMOD registers and clears all shift data to zero.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void HMODCleanup(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODResetPin);
            HMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODDigiPins);
            HMOD_DigiPins.ApplyLevelsandTimings(dataLevels, dataTimings);

            HMOD1to4(tsmContext, HMOD_Data_1: 0, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);
            HMOD5to10(tsmContext, HMOD_Data_5: 0, HMOD_Data_6: 0, HMOD_Data_7: 0, HMOD_Data_8: 0, HMOD_Data_9: 0, HMOD_Data_10: 0);
            HMOD14to18(tsmContext, HMOD_Data_14: 0, HMOD_Data_15: 0, HMOD_Data_16: 0, HMOD_Data_17: 0, HMOD_Data_18: 0);
            HMOD19to23(tsmContext, HMOD_Data_19: 0, HMOD_Data_20: 0, HMOD_Data_21: 0, HMOD_Data_22: 0, HMOD_Data_23: 0);

        }

        /// <summary>
        /// Converts a comma-separated relay string (e.g. "K1, K5, K21") into a uint bitmask
        /// where bit N-1 represents relay KN.
        /// </summary>
        /// <param name="relayToggle">Comma or semicolon-separated relay names (K1–K32).</param>
        /// <param name="maxRelay">Maximum relay number (default 32).</param>
        public static uint RelayID(string relayToggle, int maxRelay = 32)
        {
            if (relayToggle == null) throw new ArgumentNullException(nameof(relayToggle));

            uint result = 0;
            foreach (var raw in relayToggle.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var digits = new string(raw.Where(char.IsDigit).ToArray());
                if (int.TryParse(digits, out int n) && n >= 1 && n <= maxRelay)
                    result |= (1u << (n - 1));
            }
            return result;
        }

        /// <summary>
        /// Creates a uint bitmask with all relays from startRelay to endRelay (inclusive) set.
        /// </summary>
        /// <param name="startRelay">First relay number (1-based, 1–32).</param>
        /// <param name="endRelay">Last relay number (1-based, 1–32).</param>
        public static uint RelayRange(int startRelay, int endRelay)
        {
            if (startRelay < 1 || endRelay < 1 || startRelay > 32 || endRelay > 32)
                throw new ArgumentOutOfRangeException("Relay numbers must be between 1 and 32.");
            if (startRelay > endRelay)
                throw new ArgumentException("startRelay must be <= endRelay.");

            uint result = 0;
            for (int i = startRelay; i <= endRelay; i++)
                result |= (1u << (i - 1));
            return result;
        }

        /// <summary>
        /// Shifts 32-bit data into Checker Board HMOD1 through HMOD13
        /// (1x 32-ch + 12x 32-bit groups = 416 clocks, 392 effective bits).
        /// </summary>
        public static void CHMOD1to13(ISemiconductorModuleContext tsmContext,
           uint HMOD_Data_1 = 0,
           uint HMOD_Data_2 = 0,
           uint HMOD_Data_3 = 0,
           uint HMOD_Data_4 = 0,
           uint HMOD_Data_5 = 0,
           uint HMOD_Data_6 = 0,
           uint HMOD_Data_7 = 0,
           uint HMOD_Data_8 = 0,
           uint HMOD_Data_9 = 0,
           uint HMOD_Data_10 = 0,
           uint HMOD_Data_11 = 0,
           uint HMOD_Data_12 = 0,
           uint HMOD_Data_13 = 0
           )
        // There are a total of 1 pc 32-ch HMOD and 5 pcs 72-ch HMOD
        // Total databits is 32*1 + 72*5 = 392
        // Configuring 392 bits into a group of 32 bits =  392/32 = 12.25 or 13
        // Thus 13 groups of 32 bits
        // Of the 416(32*13) databits, only up to 392 databit will be clocked in. CS will be set to high on the 393rd Databit
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            // Turn ON relays to connect DIO pins to HSD
            HMOD14to18(tsmContext, HMOD_Data_14: RelayID("K1, K2, K3, K4"));

            InstrumentControl.Digital CHMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxDataPin);
            CHMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            CHMOD_DIN.CreateSourceWaveformBroadcast("HMOD_1_13", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);

            CHMOD_DIN.WriteSourceWaveformBroadcast("HMOD_1_13", new uint[] { 
                HMOD_Data_13, 
                    HMOD_Data_12, 
                        HMOD_Data_11, 
                            HMOD_Data_10,
                                HMOD_Data_9, 
                                    HMOD_Data_8, 
                                        HMOD_Data_7, 
                                            HMOD_Data_6,
                                                HMOD_Data_5, 
                                                    HMOD_Data_4, 
                                                        HMOD_Data_3, 
                                                            HMOD_Data_2, 
                                                                HMOD_Data_1 });

            CHMOD_DIN.BurstPattern("HMOD_1_13_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        public static void CHMODInit(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital CHMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxResetPin);
            CHMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital CHMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, allCHMODDigiPins);
            CHMOD_DigiPins.ApplyLevelsandTimings(
                levelsSheetName: "HMOD",
                timingsSheetName: "HMOD");

            CHMOD1to6(tsmContext,
                HMOD_Data_1: chmod1,
                HMOD_Data_2: chmod2,
                HMOD_Data_3: chmod3,
                HMOD_Data_4: chmod4,
                HMOD_Data_5: chmod5,
                HMOD_Data_6: chmod6);

            // CHMOD1to13(tsmContext, HMOD_Data_1: chmod1);
        }

        /// <summary>
        /// Shifts data into the Checker Board HMOD chain (392 bits total).
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="HMOD_Data_1">32-bit data for Checker HMOD1 (K1–K32).</param>
        /// <param name="HMOD_Data_2">72-bit data for Checker HMOD2 (K1–K72), as uint[3].</param>
        /// <param name="HMOD_Data_3">72-bit data for Checker HMOD3 (K1–K72), as uint[3].</param>
        /// <param name="HMOD_Data_4">72-bit data for Checker HMOD4 (K1–K72), as uint[3].</param>
        /// <param name="HMOD_Data_5">72-bit data for Checker HMOD5 (K1–K72), as uint[3].</param>
        /// <param name="HMOD_Data_6">72-bit data for Checker HMOD6 (K1–K72), as uint[3].</param>
        public static void CHMOD1to6(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_1 = 0,       // 32-bit HMOD (K1–K32)
            uint[] HMOD_Data_2 = null,  // 72-bit HMOD (K1–K72) → uint[3]
            uint[] HMOD_Data_3 = null,
            uint[] HMOD_Data_4 = null,
            uint[] HMOD_Data_5 = null,
            uint[] HMOD_Data_6 = null)
        {
            // Build the full 392-bit waveform (1 bit per sample, MSB first)
            // Shift order: last in chain shifts first → HMOD6, HMOD5, ..., HMOD1
            uint[] waveform = new uint[392];
            int offset = 0;

            ExpandBits(waveform, ref offset, HMOD_Data_6, 72);
            ExpandBits(waveform, ref offset, HMOD_Data_5, 72);
            ExpandBits(waveform, ref offset, HMOD_Data_4, 72);
            ExpandBits(waveform, ref offset, HMOD_Data_3, 72);
            ExpandBits(waveform, ref offset, HMOD_Data_2, 72);
            ExpandBits(waveform, ref offset, HMOD_Data_1, 32);

            // Turn ON relays to connect DIO pins to HSD
            HMOD14to18(tsmContext, HMOD_Data_14: RelayID("K1, K2, K3, K4"));

            InstrumentControl.Digital CHMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxDataPin);
            CHMOD_DIN.ApplyLevelsandTimings(
                levelsSheetName: "HMOD",
                timingsSheetName: "HMOD");

            /*CHMOD_DIN.CreateSourceWaveformBroadcast(
                waveformName: "CHMOD", 
                SourceDataMapping.Broadcast, 
                sampleWidth: 1, 
                BitOrder.MostSignificantBitFirst);*/

            // Write 1-bit-per-sample waveform via raw session 
            foreach (var ssc in CHMOD_DIN.SSC)
            {
                ssc.Session.SourceWaveforms.WriteBroadcast(
                    waveformName: "HMOD_1_6",
                    waveformData: waveform);
            }

            CHMOD_DIN.BurstPattern(
                startLabel: "HMOD_1_6_pat",
                selectDigitalFunction: true,
                waitUntilDone: true);
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

