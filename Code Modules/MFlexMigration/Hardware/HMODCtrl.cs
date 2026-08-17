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

        private const string allHMODResetPin = "RESET_PINS";
        private static string allHMODDigiPins = "HMOD_DIGITAL_PINS";
        private static string hmodTxResetPin = "HMOD_RESET";
        private static string hmodCxResetPin = "CHMOD_RESET";
        private static string dataLevels = "HMOD";
        private static string dataTimings = "HMOD";
        public static string hmodTxDataPin = "HMOD_DIN";
        public static string hmodCxDataPin = "CHMOD_DIN";

        public static void HMODInitialization(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODResetPin);
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

        public static void THMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }
        
        public static void CHMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxResetPin);
            HMOD_Reset.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(5 * Globals.mS);
            HMOD_Reset.WriteStatic(PinState._1);
        }
        
        public static void AllHMODReset(ISemiconductorModuleContext tsmContext)
        {
            InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODResetPin);
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
            HMOD_DIN.ApplyLevelsandTimings(dataLevels, dataTimings);
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_5_10", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_5_10", new uint[] { HMOD_Data_10, HMOD_Data_9, HMOD_Data_8, HMOD_Data_7, HMOD_Data_6, HMOD_Data_5 });
            HMOD_DIN.BurstPattern("HMOD_5_10_pat");
            Globals.TheHdw.Wait(5 * Globals.mS);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodTxDataPin);
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODResetPin);
            HMOD_Reset.WriteStatic(PinState._0);

            InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, allHMODDigiPins);
            HMOD_DigiPins.ApplyLevelsandTimings(dataLevels, dataTimings);

            HMOD1to4(tsmContext, HMOD_Data_1: 0, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);
            HMOD5to10(tsmContext, HMOD_Data_5: 0, HMOD_Data_6: 0, HMOD_Data_7: 0, HMOD_Data_8: 0, HMOD_Data_9: 0, HMOD_Data_10: 0);
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
            InstrumentControl.Digital CHMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, hmodCxDataPin);
            CHMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            CHMOD_DIN.CreateSourceWaveformBroadcast("Checker_HMOD", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            CHMOD_DIN.WriteSourceWaveformBroadcast("Checker_HMOD", new uint[] { 
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
                HMOD_Data_1 }
            );
            CHMOD_DIN.BurstPattern("Checker_HMOD_pat");
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
    }
}
