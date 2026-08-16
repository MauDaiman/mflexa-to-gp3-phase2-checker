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

        private static string hmodResetPin = "HMOD_RESET";
        private static string dataLevels = "HMOD";
        private static string dataTimings = "HMOD";
        private static string hmodDigiPins = "HMOD_DigiPins";
        public static string hmodDataPin = "HMOD_DIN";

        public static void HMODInitialization(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodResetPin);
            HMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDigiPins);
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

        public static void HMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }

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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_1_4", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_1_4", new uint[] { HMOD_Data_4, HMOD_Data_3, HMOD_Data_2, HMOD_Data_1 });
            HMOD_DIN.BurstPattern("HMOD_1_4_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_5_10", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_5_10", new uint[] { HMOD_Data_10, HMOD_Data_9, HMOD_Data_8, HMOD_Data_7, HMOD_Data_6, HMOD_Data_5 });
            HMOD_DIN.BurstPattern("HMOD_5_10_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
        public static void HMOD11to13_24to25(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_11 = 0,
            uint HMOD_Data_12 = 0,
            uint HMOD_Data_13 = 0,
            uint HMOD_Data_24_LO = 0,
            uint HMOD_Data_24_MID = 0,
            uint HMOD_Data_24_HI = 0,
            uint HMOD_Data_25_LO = 0,
            uint HMOD_Data_25_MID = 0,
            uint HMOD_Data_25_HI = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            var data = new uint[30];
            int idx = 0;

            PackBytes72(data, ref idx, HMOD_Data_25_LO, HMOD_Data_25_MID, HMOD_Data_25_HI);
            PackBytes72(data, ref idx, HMOD_Data_24_LO, HMOD_Data_24_MID, HMOD_Data_24_HI);
            PackBytes32(data, ref idx, HMOD_Data_13);
            PackBytes32(data, ref idx, HMOD_Data_12);
            PackBytes32(data, ref idx, HMOD_Data_11);

            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_11_13_24_25", SourceDataMapping.Broadcast, 8, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_11_13_24_25", data);
            HMOD_DIN.BurstPattern("HMOD_11_13_24_25_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        private static void PackBytes32(uint[] data, ref int idx, uint value)
        {
            data[idx++] = (value >> 24) & 0xFF;
            data[idx++] = (value >> 16) & 0xFF;
            data[idx++] = (value >> 8) & 0xFF;
            data[idx++] = value & 0xFF;
        }

        private static void PackBytes72(uint[] data, ref int idx, uint lo, uint mid, uint hi)
        {
            data[idx++] = (hi >> 16) & 0xFF;
            data[idx++] = (hi >> 8) & 0xFF;
            data[idx++] = hi & 0xFF;
            data[idx++] = (mid >> 16) & 0xFF;
            data[idx++] = (mid >> 8) & 0xFF;
            data[idx++] = mid & 0xFF;
            data[idx++] = (lo >> 16) & 0xFF;
            data[idx++] = (lo >> 8) & 0xFF;
            data[idx++] = lo & 0xFF;
        }
        public static void HMOD11to13(ISemiconductorModuleContext tsmContext,
            uint HMOD_Data_11 = 0,
            uint HMOD_Data_12 = 0,
            uint HMOD_Data_13 = 0
            )
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_11_13", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_11_13", new uint[] { HMOD_Data_13, HMOD_Data_12, HMOD_Data_11 });
            HMOD_DIN.BurstPattern("HMOD_11_13_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

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
            InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_14_18", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_14_18", new uint[] { HMOD_Data_18, HMOD_Data_17, HMOD_Data_16, HMOD_Data_15, HMOD_Data_14 });
            HMOD_DIN.BurstPattern("HMOD_14_18_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_19_23", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_19_23", new uint[] { HMOD_Data_23, HMOD_Data_22, HMOD_Data_21, HMOD_Data_20, HMOD_Data_19 });
            HMOD_DIN.BurstPattern("HMOD_19_23_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
 
        public static void HMODCleanup(ISemiconductorModuleContext tsmContext)
        {
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodResetPin);
            HMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodDigiPins);
            HMOD_DigiPins.ApplyLevelsandTimings(dataLevels, dataTimings);

            HMOD1to4(tsmContext, HMOD_Data_1: 0, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);
            HMOD5to10(tsmContext, HMOD_Data_5: 0, HMOD_Data_6: 0, HMOD_Data_7: 0, HMOD_Data_8: 0, HMOD_Data_9: 0, HMOD_Data_10: 0);
            HMOD11to13_24to25(tsmContext, HMOD_Data_11: 0, HMOD_Data_12: 0, HMOD_Data_13: 0, HMOD_Data_24_LO: 0, HMOD_Data_24_MID: 0, HMOD_Data_24_HI: 0, HMOD_Data_25_LO: 0, HMOD_Data_25_MID: 0, HMOD_Data_25_HI: 0);
            HMOD14to18(tsmContext, HMOD_Data_14: 0, HMOD_Data_15: 0, HMOD_Data_16: 0, HMOD_Data_17: 0, HMOD_Data_18: 0);
            HMOD19to23(tsmContext, HMOD_Data_19: 0, HMOD_Data_20: 0, HMOD_Data_21: 0, HMOD_Data_22: 0, HMOD_Data_23: 0);

        }

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

        public static (uint LO, uint MID, uint HI) RelayID72(string relayToggle)
        {
            if (relayToggle == null) throw new ArgumentNullException(nameof(relayToggle));

            uint lo = 0, mid = 0, hi = 0;
            foreach (var raw in relayToggle.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var digits = new string(raw.Where(char.IsDigit).ToArray());
                if (!int.TryParse(digits, out int n) || n < 1 || n > 72) continue;

                if (n <= 24)
                    lo |= (1u << (n - 1));
                else if (n <= 48)
                    mid |= (1u << (n - 25));
                else
                    hi |= (1u << (n - 49));
            }
            return (lo, mid, hi);
        }
    }
}
