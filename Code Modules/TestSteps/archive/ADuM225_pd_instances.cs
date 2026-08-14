using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps
{
	public class ADuM225_pd_instances
	{
		public static void continuity(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.continuity;
			Specs.TNames(TNames);

			//FORMAT ARGUMENTS HERE
			TestProgram.continuity_pd("cont_pins");
		}
		
		public static void idle_supply_current_N0_1p9_1p9(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N0_1p9_1p9;
			Specs.TNames(TNames);

			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",1.9,0.2,1.9,0.2,"","","","",true);
			//TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT", 1.9, 0.1, 1.9, 0.1, "", "", "", "", true);//200mA range is not supported for the latest pin map
		}
		
		public static void idle_supply_current_N1_1p9_1p9(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N1_1p9_1p9;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",1.9,0.2,1.9,0.2,"","","","",true);
		}
		
		public static void dynamic_supply_current_1Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_1Mbps_1p9_1p9;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_25Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_25Mbps_1p9_1p9;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_100Mbps_1p9_1p9(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_100Mbps_1p9_1p9;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,1.9,0.2,1.9,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void idle_supply_current_N0_2p75_2p75(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N0_2p75_2p75;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",2.75,0.2,2.75,0.2,"","","","",true);
		}
		
		public static void idle_supply_current_N1_2p75_2p75(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N1_2p75_2p75;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",2.75,0.2,2.75,0.2,"","","","",true);
		}
		
		public static void dynamic_supply_current_1Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_1Mbps_2p75_2p75;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_25Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_25Mbps_2p75_2p75;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_100Mbps_2p75_2p75(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_100Mbps_2p75_2p75;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,2.75,0.2,2.75,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void idle_supply_current_N0_3p6_3p6(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N0_3p6_3p6;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",3.6,0.2,3.6,0.2,"","","","",true);
		}
		
		public static void idle_supply_current_N1_3p6_3p6(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N1_3p6_3p6;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",3.6,0.2,3.6,0.2,"","","","",true);
		}
		
		public static void dynamic_supply_current_1Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_1Mbps_3p6_3p6;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_25Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_25Mbps_3p6_3p6;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_100Mbps_3p6_3p6(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_100Mbps_3p6_3p6;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,3.6,0.2,3.6,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void idle_supply_current_N0_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N0_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",5.5,0.2,5.5,0.2,"","","","",true);
		}
		
		public static void idle_supply_current_N1_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.idle_supply_current_N1_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyStatic(".\\vectors\\static_init.PAT",5.5,0.2,5.5,0.2,"","","","",true);
		}
		
		public static void dynamic_supply_current_1Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_1Mbps_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",2.0E-06,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_25Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_25Mbps_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",80.0E-09,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void dynamic_supply_current_100Mbps_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.dynamic_supply_current_100Mbps_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.DcviSupplyDynamic(".\\vectors\\idd_dyn.PAT",20.0E-09,5.5,0.2,5.5,0.2,"K3,K4","","","K3,K4",false);
		}
		
		public static void Tr_prop_delay_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tr_prop_delay_1M_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_rise_AB_192.PAT","","Rise",0,1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void Tf_prop_delay_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tf_prop_delay_1M_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_fall_AB_192.PAT","","Fall",0,1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void pulse_width_distortion_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.pulse_width_distortion_1M_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PulseWidthDistortion("data_outs","","","","");
		}
		
		public static void ch_ch_match_1M_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.ch_ch_match_1M_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.ChChMatch("","","","");
		}
		
		public static void func_1Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_1Mbps_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void func_5Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_5Mbps_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void func_50Mbps_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_50Mbps_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void func_75MHz_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_75MHz_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.FunctionalModifyTiming("throughput_mux_pd_set","data_ins","data_outs","data_ins_odd","","tset_phase1","tset_phase2","tset_phase3","tset_phase4",26.667E-09,1,1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void func_slow_speed_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_slow_speed_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("slow_speed_pd_set",1.7,0.2,1.7,0.2,"","","","");
		}
		
		public static void input_output_levels_1p7_1p7(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.input_output_levels_1p7_1p7;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.input_output_levels_1p7_1p7_pd();
		}
		
		public static void Tr_prop_delay_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tr_prop_delay_1M_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_rise_AB_192.PAT","","Rise",1,2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void Tf_prop_delay_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tf_prop_delay_1M_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_fall_AB_192.PAT","","Fall",1,2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void pulse_width_distortion_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.pulse_width_distortion_1M_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PulseWidthDistortion("data_outs","","","","");
		}
		
		public static void ch_ch_match_1M_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.ch_ch_match_1M_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.ChChMatch("","","","");
		}
		
		public static void func_1Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_1Mbps_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void func_5Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_5Mbps_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void func_50Mbps_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_50Mbps_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void func_75MHz_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_75MHz_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.FunctionalModifyTiming("throughput_mux_pd_set","data_ins","data_outs","data_ins_odd","","tset_phase1","tset_phase2","tset_phase3","tset_phase4",26.667E-09,1,2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void func_slow_speed_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_slow_speed_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("slow_speed_pd_set",2.25,0.2,2.25,0.2,"","","","");
		}
		
		public static void input_output_levels_2p25_2p25(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.input_output_levels_2p25_2p25;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.input_output_levels_2p25_2p25_pd();
		}
		
		public static void Tr_prop_delay_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tr_prop_delay_1M_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_rise_AB_192.PAT","","Rise",2,3,0.2,3,0.2,"","","","");
		}
		
		public static void Tf_prop_delay_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tf_prop_delay_1M_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_fall_AB_192.PAT","","Fall",2,3,0.2,3,0.2,"","","","");
		}
		
		public static void pulse_width_distortion_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.pulse_width_distortion_1M_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PulseWidthDistortion("data_outs","","","","");
		}
		
		public static void ch_ch_match_1M_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.ch_ch_match_1M_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.ChChMatch("","","","");
		}
		
		public static void func_1Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_1Mbps_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
		    TestProgram.Functional("throughput_pd_set",3,0.2,3,0.2,"","","","");
		}
		
		public static void func_5Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_5Mbps_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",3,0.2,3,0.2,"","","","");
		}
		
		public static void func_50Mbps_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_50Mbps_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",3,0.2,3,0.2,"","","","");
		}
		
		public static void func_75MHz_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_75MHz_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.FunctionalModifyTiming("throughput_mux_pd_set","data_ins","data_outs","data_ins_odd","","tset_phase1","tset_phase2","tset_phase3","tset_phase4",26.667E-09,1,3,0.2,3,0.2,"","","","");
		}
		
		public static void func_slow_speed_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_slow_speed_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("slow_speed_pd_set",3,0.2,3,0.2,"","","","");
		}
		
		public static void input_output_levels_3p0_3p0(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.input_output_levels_3p0_3p0;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.input_output_levels_3p0_3p0_pd();
		}
		
		public static void Tr_prop_delay_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tr_prop_delay_1M_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_rise_AB_192.PAT","","Rise",3,4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void Tf_prop_delay_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.Tf_prop_delay_1M_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PropDelay("data_outs",0,0.000000000192308,"",".\\vectors\\Tpd_fall_AB_192.PAT","","Fall",3,4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void pulse_width_distortion_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.pulse_width_distortion_1M_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PulseWidthDistortion("data_outs","","","","");
		}
		
		public static void ch_ch_match_1M_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.ch_ch_match_1M_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.ChChMatch("","","","");
		}
		
		public static void func_1Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_1Mbps_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void func_5Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_5Mbps_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void func_50Mbps_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_50Mbps_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("throughput_pd_set",4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void func_75MHz_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_75MHz_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.FunctionalModifyTiming("throughput_mux_pd_set","data_ins","data_outs","data_ins_odd","","tset_phase1","tset_phase2","tset_phase3","tset_phase4",26.667E-09,1,4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void func_slow_speed_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.func_slow_speed_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.Functional("slow_speed_pd_set",4.5,0.2,4.5,0.2,"","","","");
		}
		
		public static void input_output_levels_4p5_4p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.input_output_levels_4p5_4p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.input_output_levels_4p5_4p5_pd();
		}
		
		public static void LEAKAGE_5p5_5p5(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.LEAKAGE_5p5_5p5;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PpmuForcevMeasi("VIA,VIB","Both","","","VIA,VIB",5.5,0.2,"",5.5,0.2,"","","","");
		}
		
		public static void PowerDown(ISemiconductorModuleContext tsmContext)
		{
			//Pass TSM context to Globals for all sessions and site arrangements
			Globals.tsmContext = tsmContext;
			//Globals.Active = GlobalFunctions.ConvertToBooleanList(Globals.tsmContext.SiteNumbers.ToList());
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
			
			//Load the Flow Information Sheet
			string[,] TNames = ADuM225_qc_flow.PowerDown;
			Specs.TNames(TNames);
			
			//FORMAT ARGUMENTS HERE
			TestProgram.PowerDown();
		}
		
	}
}
