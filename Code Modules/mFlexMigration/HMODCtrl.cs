using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace HMOD
{
    public static class HMODCtrl
    {
        public static void InitHMOD(ISemiconductorModuleContext tsmContext)
        {
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_RESET"); //for TxBoard HMOD
            HMOD_Reset.WriteStatic(PinState._0);
           
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital ChckrBrd_HMOD_Rst = InstrCtrl.DigitalPinsToSessions(tsmContext, "CheckerBrd_HMOD_RST"); // for Chckr Board HMOD
            ChckrBrd_HMOD_Rst.WriteStatic(PinState._0);       

            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DigiPins = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DigiPins");
            HMOD_DigiPins.ApplyLevelsandTimings("HMOD", "HMOD");

            HMOD1to4(tsmContext);
            HMOD5to10(tsmContext);
            HMOD11to13(tsmContext);
            HMOD14to18(tsmContext);
            HMOD19to23(tsmContext);
            ChckrBoardHMOD1to13(tsmContext); 
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DIN");
            HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_1_4", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);  // call tdms file //
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_1_4", new uint[] { HMOD_Data_4, HMOD_Data_3, HMOD_Data_2, HMOD_Data_1});  //assign 32 bit data(x4), HMOD4(MSB) first to the tdms //
            HMOD_DIN.BurstPattern("HMOD_1_4_pat"); //burst pattern with the updated 32 bit(x4) data
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DIN");
            HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_5_10", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_5_10", new uint[] { HMOD_Data_10, HMOD_Data_9, HMOD_Data_8, HMOD_Data_7, HMOD_Data_6, HMOD_Data_5});
            HMOD_DIN.BurstPattern("HMOD_5_10_pat");
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DIN");
            HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_11_13", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_11_13", new uint[] { HMOD_Data_13, HMOD_Data_12, HMOD_Data_11});
            HMOD_DIN.BurstPattern("HMOD_11_13_pat");
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DIN");
            HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_14_18", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_14_18", new uint[] { HMOD_Data_18, HMOD_Data_17, HMOD_Data_16, HMOD_Data_15, HMOD_Data_14});
            HMOD_DIN.BurstPattern("HMOD_14_18_pat");
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
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_DIN");
            HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            HMOD_DIN.CreateSourceWaveformBroadcast("HMOD_19_23", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            HMOD_DIN.WriteSourceWaveformBroadcast("HMOD_19_23", new uint[] { HMOD_Data_23, HMOD_Data_22, HMOD_Data_21, HMOD_Data_20, HMOD_Data_19});
            HMOD_DIN.BurstPattern("HMOD_19_23_pat");
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        public static void ChckrBoardHMOD1to13(ISemiconductorModuleContext tsmContext,
           uint ChckrBoardHMOD_Data_1 = 0,
           uint ChckrBoardHMOD_Data_2 = 0,
           uint ChckrBoardHMOD_Data_3 = 0,
           uint ChckrBoardHMOD_Data_4 = 0,
           uint ChckrBoardHMOD_Data_5 = 0,
           uint ChckrBoardHMOD_Data_6 = 0,
           uint ChckrBoardHMOD_Data_7 = 0,
           uint ChckrBoardHMOD_Data_8 = 0,
           uint ChckrBoardHMOD_Data_9 = 0,
           uint ChckrBoardHMOD_Data_10 = 0,
           uint ChckrBoardHMOD_Data_11 = 0,
           uint ChckrBoardHMOD_Data_12 = 0,
           uint ChckrBoardHMOD_Data_13 = 0
           )
        //There are a total of 1 pc 32-ch HMOD and 5 pcs 72-ch HMOD
        // total databits is 32*1 + 72*5 = 392
        //configuring 392 bits into a group of 32 bits =  392/32 = 12.25 or 13
        //Thus 13 groups of 32 bits
        // Of the 416(32*13) databits, only up to 392 databit will be clocked in. CS will be set to high on the 393rd Databit
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital CheckerBrd_HMOD_DIN = InstrCtrl.DigitalPinsToSessions(tsmContext, "CheckerBrd_HMOD_DIN");
            CheckerBrd_HMOD_DIN.ApplyLevelsandTimings("HMOD", "HMOD");
            CheckerBrd_HMOD_DIN.CreateSourceWaveformBroadcast("TxChecker_HMOD", SourceDataMapping.Broadcast, 32, BitOrder.MostSignificantBitFirst);
            CheckerBrd_HMOD_DIN.WriteSourceWaveformBroadcast("TxChecker_HMOD", new uint[] { ChckrBoardHMOD_Data_13, ChckrBoardHMOD_Data_12, ChckrBoardHMOD_Data_11, ChckrBoardHMOD_Data_10, 
                                                                                 ChckrBoardHMOD_Data_9, ChckrBoardHMOD_Data_8, ChckrBoardHMOD_Data_7, ChckrBoardHMOD_Data_6, 
                                                                                 ChckrBoardHMOD_Data_5, ChckrBoardHMOD_Data_4, ChckrBoardHMOD_Data_3, ChckrBoardHMOD_Data_2, ChckrBoardHMOD_Data_1 });
            CheckerBrd_HMOD_DIN.BurstPattern("TxChecker_HMOD_pat");
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
    }
}
