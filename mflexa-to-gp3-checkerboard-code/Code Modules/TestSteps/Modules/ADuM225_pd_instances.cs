using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using System.Linq;
using System.Threading.Tasks;

namespace TestSteps
{
    public class ADuM225_pd_instances
    {
        //Measure Ground Resistance
        public static void Meas_Ground_Resistance(ISemiconductorModuleContext tsmContext)
        {
            double[][] VP1A;
            double[][] VP1B;
            double IP2A = -100e-6;
            double IP2B = -1e-3;
            double[] GroundResistance = new double[4];

            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.DCPower VDD2 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VDD2");
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital VOA = InstrCtrl.DigitalPinsToSessions(tsmContext, "VOA");

            VDD2.ConfigureOutputConnected(false);

            Globals.TheHdw.PPMU.Pins((PinList) "VOA,VOB").Connect();

            //OriginalCode: End With;
            //OriginalCode: TheHdw.PPMU.Pins(TestPins).ForceI 0.0005, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).ForceI(0.0005, 2 * mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").ForceI(-0.00001, 2 * Globals.mA);

            Globals.TheHdw.PPMU.Pins("VOB").ForceI(IP2A, 2 * Globals.mA);
            Globals.TheHdw.Wait(0.002);


            VP1A = VOA.PPMUMeasure(PpmuMeasurementType.Voltage);

            Globals.TheHdw.PPMU.Pins("VOB").ForceI(IP2B, 2 * Globals.mA);
            Globals.TheHdw.Wait(0.002);

            //OriginalCode: pos_diode = TheHdw.PPMU.Pins(TestPins).Read;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: pos_diode = TheHdw.PPMU.Pins(TestPins).Read();
            VP1B = VOA.PPMUMeasure(PpmuMeasurementType.Voltage);

            Parallel.For(0, 4, i =>
            {
                GroundResistance[i] = (VP1A[i][0] - VP1B[i][0]) / (IP2B - IP2A);
            });

            Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").ForceI(0, 2 * Globals.mA);
            VDD2.ConfigureOutputConnected(true);
        }

        public static void continuity(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "cty";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_continuity.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.continuity;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.continuity_pd("cont_pins");
        }

        public static void idle_supply_current_N0_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N0_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N0_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N0_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",1.9,0.2,1.9,0.2,"","","","",true);	//PXIe-4162 max current is only 100mA -adrian
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 1.9, 0.01, 1.9, 0.01, "", "", "", "", true);
        }

        public static void idle_supply_current_N1_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N1_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N1_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N1_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 1.9, 0.2, 1.9, 0.2, "", "", "", "", true);
        }

        public static void dynamic_supply_current_1Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_1Mbps_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_1Mbps_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_1Mbps_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);	//PXIe-4162 max current is only 100mA -adrian
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 1.9, 0.1, 1.9, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 1.9, 0.1, 1.9, 0.1, "K3,K4", "", "", "", false);  //let K3,K4 remain connected -adrian
        }

        public static void dynamic_supply_current_25Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "25Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_25Mbps_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_25Mbps_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_25Mbps_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 1.9, 0.1, 1.9, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 1.9, 0.1, 1.9, 0.1, "", "", "", "", false);    //K3,K4 remains connected -adrian
        }

        public static void dynamic_supply_current_100Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_100Mbps_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_100Mbps_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_100Mbps_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 1.9, 0.1, 1.9, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 1.9, 0.1, 1.9, 0.1, "", "", "", "K3,K4", false);  //K3,K4 remains connected; disconnect for next test -adrian
        }

        public static void idle_supply_current_N0_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N0_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N0_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N0_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",2.75,0.2,2.75,0.2,"","","","",true);
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 2.75, 0.1, 2.75, 0.1, "", "", "", "", true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 2.75, 0.01, 2.75, 0.01, "", "", "", "", true, false);
        }

        public static void idle_supply_current_N1_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N1_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N1_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N1_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",2.75,0.2,2.75,0.2,"","","","",true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 2.75, 0.1, 2.75, 0.1, "", "", "", "", true);
        }

        public static void dynamic_supply_current_1Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_1Mbps_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_1Mbps_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_1Mbps_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 2.75, 0.1, 2.75, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 2.75, 0.1, 2.75, 0.1, "K3,K4", "", "", "", false);
        }

        public static void dynamic_supply_current_25Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "25Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_25Mbps_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_25Mbps_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_25Mbps_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 2.75, 0.1, 2.75, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 2.75, 0.1, 2.75, 0.1, "", "", "", "", false);
        }

        public static void dynamic_supply_current_100Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_100Mbps_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_100Mbps_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_100Mbps_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 2.75, 0.1, 2.75, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 2.75, 0.1, 2.75, 0.1, "", "", "", "K3,K4", false);
        }

        public static void idle_supply_current_N0_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N0_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N0_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N0_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",3.6,0.2,3.6,0.2,"","","","",true);
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 3.6, 0.1, 3.6, 0.1, "", "", "", "", true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 3.6, 0.01, 3.6, 0.01, "", "", "", "", true, false);
        }

        public static void idle_supply_current_N1_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N1_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N1_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N1_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",3.6,0.2,3.6,0.2,"","","","",true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 3.6, 0.1, 3.6, 0.1, "", "", "", "", true);
        }

        public static void dynamic_supply_current_1Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_1Mbps_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_1Mbps_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_1Mbps_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 3.6, 0.1, 3.6, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 3.6, 0.1, 3.6, 0.1, "K3,K4", "", "", "", false);
        }

        public static void dynamic_supply_current_25Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "25Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_25Mbps_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_25Mbps_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_25Mbps_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 3.6, 0.1, 3.6, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 3.6, 0.1, 3.6, 0.1, "", "", "", "", false);
        }

        public static void dynamic_supply_current_100Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_100Mbps_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_100Mbps_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_100Mbps_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 3.6, 0.1, 3.6, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 3.6, 0.1, 3.6, 0.1, "", "", "", "K3,K4", false);
        }

        public static void idle_supply_current_N0_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N0_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N0_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N0_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",5.5,0.2,5.5,0.2,"","","","",true);
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 5.5, 0.1, 5.5, 0.1, "", "", "", "", true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 5.5, 0.01, 5.5, 0.01, "", "", "", "", true, false);
        }

        public static void idle_supply_current_N1_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_idle_supply_current_N1_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_idle_supply_current_N1_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.idle_supply_current_N1_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",5.5,0.2,5.5,0.2,"","","","",true);
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 5.5, 0.1, 5.5, 0.1, "", "", "", "", true);
        }

        public static void dynamic_supply_current_1Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_1Mbps_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_1Mbps_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_1Mbps_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 5.5, 0.1, 5.5, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 2.0E-06, 5.5, 0.1, 5.5, 0.1, "K3,K4", "", "", "", false);
        }

        public static void dynamic_supply_current_25Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "25Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_25Mbps_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_25Mbps_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_25Mbps_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 5.5, 0.1, 5.5, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 80.0E-09, 5.5, 0.1, 5.5, 0.1, "", "", "", "", false);
        }

        public static void dynamic_supply_current_100Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_dynamic_supply_current_100Mbps_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_dynamic_supply_current_100Mbps_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.dynamic_supply_current_100Mbps_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
            //TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 5.5, 0.1, 5.5, 0.1, "K3,K4", "", "", "K3,K4", false);
            TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT", 20.0E-09, 5.5, 0.1, 5.5, 0.1, "", "", "", "K3,K4", false);
        }

        public static void Tr_prop_delay_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tr_prop_delay_1M_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_Tr_prop_delay_1M_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tr_prop_delay_1M_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_rise_AB_192.PAT","","Rise",0,1.7,0.2,1.7,0.2,"","","","");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 0, 1.7, 0.1, 1.7, 0.1, "", "", "", "", true);
        }

        public static void Tf_prop_delay_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tf_prop_delay_1M_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_Tf_prop_delay_1M_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tf_prop_delay_1M_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_fall_AB_192.PAT","","Fall",0,1.7,0.2,1.7,0.2,"","","","");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 0, 1.7, 0.1, 1.7, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 0, 1.7, 0.1, 1.7, 0.1, "", "", "", "", false);
        }

        public static void pulse_width_distortion_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.pulse_width_distortion_1M_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.PulseWidthDistortion("data_outs", "", "", "", "");
        }

        public static void ch_ch_match_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.ch_ch_match_1M_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.ChChMatch("", "", "", "");
        }

        public static void func_1Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_1Mbps_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_func_1Mbps_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_1Mbps_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 1.7, 0.2, 1.7, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 1.7, 0.1, 1.7, 0.1, "", "", "", "");
        }

        public static void func_5Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "5MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_5Mbps_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_func_5Mbps_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_5Mbps_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 1.7, 0.2, 1.7, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 1.7, 0.1, 1.7, 0.1, "", "", "", "");
        }

        public static void func_50Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_50Mbps_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_func_50Mbps_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_50Mbps_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 1.7, 0.2, 1.7, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 1.7, 0.1, 1.7, 0.1, "", "", "", "");

        }
        
        public static void func_75MHz_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "75MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_75MHz_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_func_75MHz_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_75MHz_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 26.667E-09, 1, 1.7, 0.2, 1.7, 0.2, "", "", "", "");
            TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 13.3333E-09, 1, 1.7, 0.1, 1.7, 0.1, "", "", "", "");    //IDD clamp value from .2 to .1 due to NIGP3 limitation; period change due to changes in timing and pattern on 75MHz func -adrian
        }

        public static void func_slow_speed_1p7_1p7(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_slow_speed_1p7_1p7.digitiming";
            Globals.levelSheetName = "Levels_func_slow_speed_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_slow_speed_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("slow_speed_pd_set", 1.7, 0.2, 1.7, 0.2, "", "", "", "");
            TestProgram.Functional("slow_speed_pd_set", 1.7, 0.1, 1.7, 0.1, "", "", "", "");
        }

        public static void input_output_levels_1p7_1p7(ISemiconductorModuleContext tsmContext, string RUN = "Orig")
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p7/1p7";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_input_output_levels_1p7_1p7.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.input_output_levels_1p7_1p7;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.input_output_levels_1p7_1p7_pd();
            //TestProgram.input_output_levels_1p7_1p7_pd_DOE1();
            //TestProgram.input_output_levels_1p7_1p7_pd_DOE2();
            //TestProgram.input_output_levels_1p7_1p7_pd_DOE3();

            switch (RUN)
            {
                case "Orig":
                    TestProgram.input_output_levels_1p7_1p7_pd();
                    break;
                case "DOE1":
                    TestProgram.input_output_levels_1p7_1p7_pd_DOE1();
                    break;
                case "DOE2":
                    TestProgram.input_output_levels_1p7_1p7_pd_DOE2();
                    break;
                case "DOE3":
                    TestProgram.input_output_levels_1p7_1p7_pd_DOE3();
                    break;
                default:
                    break;
            }
        }

        public static void Tr_prop_delay_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tr_prop_delay_1M_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_Tr_prop_delay_1M_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tr_prop_delay_1M_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 1, 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 1, 2.25, 0.1, 2.25, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 1, 2.25, 0.1, 2.25, 0.1, "", "", "", "", true);
        }

        public static void Tf_prop_delay_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tf_prop_delay_1M_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_Tf_prop_delay_1M_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tf_prop_delay_1M_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 1, 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 1, 2.25, 0.1, 2.25, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 1, 2.25, 0.1, 2.25, 0.1, "", "", "", "", false);
        }

        public static void pulse_width_distortion_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.pulse_width_distortion_1M_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.PulseWidthDistortion("data_outs", "", "", "", "");
        }

        public static void ch_ch_match_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.ch_ch_match_1M_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.ChChMatch("", "", "", "");
        }

        public static void func_1Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_1Mbps_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_func_1Mbps_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_1Mbps_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 2.25, 0.1, 2.25, 0.1, "", "", "", "");
        }

        public static void func_5Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "5MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_5Mbps_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_func_5Mbps_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_5Mbps_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 2.25, 0.1, 2.25, 0.1, "", "", "", "");
        }

        public static void func_50Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_50Mbps_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_func_50Mbps_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_50Mbps_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 2.25, 0.1, 2.25, 0.1, "", "", "", "");
        }

        public static void func_75MHz_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "75MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_75MHz_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_func_75MHz_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_75MHz_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 26.667E-09, 1, 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 13.333E-09, 1, 2.25, 0.1, 2.25, 0.1, "", "", "", "");  //IDD clamp value from .2 to .1 due to NIGP3 limitation; period change due to changes in timing and pattern on 75MHz func -adrian
        }

        public static void func_slow_speed_2p25_2p25(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_slow_speed_2p25_2p25.digitiming";
            Globals.levelSheetName = "Levels_func_slow_speed_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_slow_speed_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("slow_speed_pd_set", 2.25, 0.2, 2.25, 0.2, "", "", "", "");
            TestProgram.Functional("slow_speed_pd_set", 2.25, 0.1, 2.25, 0.1, "", "", "", "");
        }

        public static void input_output_levels_2p25_2p25(ISemiconductorModuleContext tsmContext, string RUN = "Orig")
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p25/2p25";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_input_output_levels_2p25_2p25.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.input_output_levels_2p25_2p25;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.input_output_levels_2p25_2p25_pd();
            switch (RUN)
            {
                case "Orig":
                    TestProgram.input_output_levels_2p25_2p25_pd();
                    break;
                case "DOE1":
                    TestProgram.input_output_levels_2p25_2p25_pd_DOE1();
                    break;
                case "DOE2":
                    TestProgram.input_output_levels_2p25_2p25_pd_DOE2();
                    break;
                case "DOE3":
                    TestProgram.input_output_levels_2p25_2p25_pd_DOE3();
                    break;
                default:
                    break;
            }
        }

        public static void Tr_prop_delay_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tr_prop_delay_1M_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_Tr_prop_delay_1M_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tr_prop_delay_1M_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 2, 3, 0.2, 3, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 2, 3, 0.1, 3, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 2, 3, 0.1, 3, 0.1, "", "", "", "", true);
        }

        public static void Tf_prop_delay_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tf_prop_delay_1M_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_Tf_prop_delay_1M_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tf_prop_delay_1M_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 2, 3, 0.2, 3, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 2, 3, 0.1, 3, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 2, 3, 0.1, 3, 0.1, "", "", "", "", false);
        }

        public static void pulse_width_distortion_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.pulse_width_distortion_1M_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.PulseWidthDistortion("data_outs", "", "", "", "");
        }

        public static void ch_ch_match_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.ch_ch_match_1M_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.ChChMatch("", "", "", "");
        }

        public static void func_1Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_1Mbps_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_func_1Mbps_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_1Mbps_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 3, 0.2, 3, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 3, 0.1, 3, 0.1, "", "", "", "");
        }

        public static void func_5Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "5MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_5Mbps_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_func_5Mbps_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_5Mbps_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 3, 0.2, 3, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 3, 0.1, 3, 0.1, "", "", "", "");
        }

        public static void func_50Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_50Mbps_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_func_50Mbps_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_50Mbps_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 3, 0.2, 3, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 3, 0.1, 3, 0.1, "", "", "", "");
        }

        public static void func_75MHz_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "75MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_75MHz_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_func_75MHz_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_75MHz_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 26.667E-09, 1, 3, 0.2, 3, 0.2, "", "", "", "");
            TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 13.333E-09, 1, 3, 0.1, 3, 0.1, "", "", "", "");    //IDD clamp value from .2 to .1 due to NIGP3 limitation; period change due to changes in timing and pattern on 75MHz func -adrian
        }

        public static void func_slow_speed_3p0_3p0(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_slow_speed_3p0_3p0.digitiming";
            Globals.levelSheetName = "Levels_func_slow_speed_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_slow_speed_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("slow_speed_pd_set", 3, 0.2, 3, 0.2, "", "", "", "");
            TestProgram.Functional("slow_speed_pd_set", 3, 0.1, 3, 0.1, "", "", "", "");
        }

        public static void input_output_levels_3p0_3p0(ISemiconductorModuleContext tsmContext, string RUN = "Orig")
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p0/3p0";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_input_output_levels_3p0_3p0.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.input_output_levels_3p0_3p0;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.input_output_levels_3p0_3p0_pd();
            switch (RUN)
            {
                case "Orig":
                    TestProgram.input_output_levels_3p0_3p0_pd();
                    break;
                case "DOE1":
                    TestProgram.input_output_levels_3p0_3p0_pd_DOE1();
                    break;
                case "DOE2":
                    TestProgram.input_output_levels_3p0_3p0_pd_DOE2();
                    break;
                case "DOE3":
                    TestProgram.input_output_levels_3p0_3p0_pd_DOE3();
                    break;
                default:
                    break;
            }
        }

        public static void Tr_prop_delay_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tr_prop_delay_1M_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_Tr_prop_delay_1M_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tr_prop_delay_1M_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 3, 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 3, 4.5, 0.1, 4.5, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_rise_AB_192.PAT", "", "Rise", 3, 4.5, 0.1, 4.5, 0.1, "", "", "", "", true);
        }

        public static void Tf_prop_delay_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_Tf_prop_delay_1M_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_Tf_prop_delay_1M_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.Tf_prop_delay_1M_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 3, 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            //TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 3, 4.5, 0.1, 4.5, 0.1, "", "", "", "");
            TestProgram.PropDelay("data_outs", 0, 0.000000000192308, "", ".\\vectors\\Tpd_fall_AB_192.PAT", "", "Fall", 3, 4.5, 0.1, 4.5, 0.1, "", "", "", "", false);
        }

        public static void pulse_width_distortion_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.pulse_width_distortion_1M_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.PulseWidthDistortion("data_outs", "", "", "", "");
        }

        public static void ch_ch_match_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.ch_ch_match_1M_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.ChChMatch("", "", "", "");
        }

        public static void func_1Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_1Mbps_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_func_1Mbps_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_1Mbps_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 4.5, 0.1, 4.5, 0.1, "", "", "", "");
        }

        public static void func_5Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "5MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_5Mbps_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_func_5Mbps_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_5Mbps_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 4.5, 0.1, 4.5, 0.1, "", "", "", "");
        }

        public static void func_50Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "50MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_50Mbps_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_func_50Mbps_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_50Mbps_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("throughput_pd_set", 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            TestProgram.Functional("throughput_pd_set", 4.5, 0.1, 4.5, 0.1, "", "", "", "");
        }

        public static void func_75MHz_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "75MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_75MHz_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_func_75MHz_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_75MHz_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 26.667E-09, 1, 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            TestProgram.FunctionalModifyTiming("throughput_mux_pd_set", "data_ins", "data_outs", "data_ins_odd", "", "tset_phase1", "tset_phase2", "tset_phase3", "tset_phase4", 13.333E-09, 1, 4.5, 0.1, 4.5, 0.1, "", "", "", "");    //IDD clamp value from .2 to .1 due to NIGP3 limitation; period change due to changes in timing and pattern on 75MHz func -adrian
        }

        public static void func_slow_speed_4p5_4p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "1MHz";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_func_slow_speed_4p5_4p5.digitiming";
            Globals.levelSheetName = "Levels_func_slow_speed_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.func_slow_speed_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.Functional("slow_speed_pd_set", 4.5, 0.2, 4.5, 0.2, "", "", "", "");
            TestProgram.Functional("slow_speed_pd_set", 4.5, 0.1, 4.5, 0.1, "", "", "", "");
        }

        public static void input_output_levels_4p5_4p5(ISemiconductorModuleContext tsmContext, string RUN = "Orig")
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "4p5/4p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_input_output_levels_4p5_4p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.input_output_levels_4p5_4p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.input_output_levels_4p5_4p5_pd();
            switch (RUN)
            {
                case "Orig":
                    TestProgram.input_output_levels_4p5_4p5_pd();
                    break;
                case "DOE1":
                    TestProgram.input_output_levels_4p5_4p5_pd_DOE1();
                    break;
                case "DOE2":
                    TestProgram.input_output_levels_4p5_4p5_pd_DOE2();
                    break;
                case "DOE3":
                    TestProgram.input_output_levels_4p5_4p5_pd_DOE3();
                    break;
                default:
                    break;
            }
        }

        public static void LEAKAGE_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.levelSheetName = "Levels_LEAKAGE_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.LEAKAGE_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            //TestProgram.PpmuForcevMeasi("VIA,VIB", "Both", "", "", "VIA,VIB", 5.5, 0.2, "", 5.5, 0.2, "", "", "", "");
 // ttb           TestProgram.PpmuForcevMeasi("VIA,VIB", "Both", "", "", "VIA,VIB", 5.5, 0.1, "", 5.5, 0.1, "", "", "", "");
        }

        public static void PowerDown(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.PowerDown;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.PowerDown();
        }

        public static void post_idle_supply_current_N0_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N0_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N0_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N0_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 1.9, 0.2, 1.9, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N1_1p9_1p9(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "1p9/1p9";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N1_1p9_1p9.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N1_1p9_1p9.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N1_1p9_1p9;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 1.9, 0.2, 1.9, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N0_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N0_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N0_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N0_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 2.75, 0.2, 2.75, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N1_2p75_2p75(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "2p75/2p75";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N1_2p75_2p75.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N1_2p75_2p75.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N1_2p75_2p75;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 2.75, 0.2, 2.75, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N0_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N0_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N0_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N0_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 3.6, 0.2, 3.6, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N1_3p6_3p6(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "3p6/3p6";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N1_3p6_3p6.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N1_3p6_3p6.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N1_3p6_3p6;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 3.6, 0.2, 3.6, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N0_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N0_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N0_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N0_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 5.5, 0.2, 5.5, 0.2, "", "", "", "", true);
        }

        public static void post_idle_supply_current_N1_5p5_5p5(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = "5p5/5p5";
            Globals.DCSpecs.Selector = "Typical";

            //AC Specs Section
            Globals.ACSpecs.Category = "2Mbps";
            Globals.ACSpecs.Selector = "Typical";

            //Sheet Parameters Section
            Globals.timingSheetName = "Timing_post_idle_supply_current_N1_5p5_5p5.digitiming";
            Globals.levelSheetName = "Levels_post_idle_supply_current_N1_5p5_5p5.digilevels";
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.post_idle_supply_current_N1_5p5_5p5;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 5.5, 0.2, 5.5, 0.2, "", "", "", "", true);
        }

        public static void RunIPATTests_I(ISemiconductorModuleContext tsmContext)
        {
            //Pass TSM context to Globals for all sessions and site arrangements
            Globals.tsmContext = tsmContext;
            Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

            //Environment Section 
            Globals.Environment = null;

            //DC Specs Section
            Globals.DCSpecs.Category = null;
            Globals.DCSpecs.Selector = null;

            //AC Specs Section
            Globals.ACSpecs.Category = null;
            Globals.ACSpecs.Selector = null;

            //Sheet Parameters Section
            Globals.edgeSetSheetName = null;
            Globals.edgeSetSheetName = null;
            Globals.MixedSignalTiming = null;

            ////Load the Flow Information Sheet
            //string[,] TNames = ADuM225_pd_ipat_flow.RunIPATTests_I;
            //Specs.TNames(TNames);
            //PublishID Counter
            Globals.tsmContext.SetGlobalData("publishID", 0);

            //FORMAT ARGUMENTS HERE
            TestProgram.RunIPATTests_t();
        }

        //public static void PowerDown(ISemiconductorModuleContext tsmContext)
        //{
        //	//Pass TSM context to Globals for all sessions and site arrangements
        //	Globals.tsmContext = tsmContext;
        //	Globals.Active = Globals.tsmContext.SiteNumbers.ToList();

        //	//Environment Section 
        //	Globals.Environment = null;

        //	//DC Specs Section
        //	Globals.DCSpecs.Category = null;
        //	Globals.DCSpecs.Selector = null;

        //	//AC Specs Section
        //	Globals.ACSpecs.Category = null;
        //	Globals.ACSpecs.Selector = null;

        //	//Sheet Parameters Section
        //	Globals.edgeSetSheetName = null;
        //	Globals.edgeSetSheetName = null;
        //	Globals.MixedSignalTiming = null;

        //	//Load the Flow Information Sheet
        //	string[,] TNames = ADuM225_pd_ipat_flow.PowerDown;
        //	Specs.TNames(TNames);

        //	//FORMAT ARGUMENTS HERE
        //	TestProgram.PowerDown();
        //}

    }
}
