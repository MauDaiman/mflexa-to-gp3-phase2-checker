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
	class ADuM225_qc_flow
	{
		// Declare a read only array of strings of TNames for continuity
		public static readonly string[,] continuity = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "pos_diode", "1", "0.35", "0.71", "", "", "", "", "8", "", "8", "Fail" },
			{ "neg_diode", "", "-0.7", "-0.35", "", "", "", "", "8", "", "8", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N0_1p9_1p9
		public static readonly string[,] idle_supply_current_N0_1p9_1p9 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_1p9_1p9", "9", "0.003", "0.0096", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_1p9_1p9", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_1p9_1p9", "", "0.0005", "0.0012", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_1p9_1p9", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N1_1p9_1p9
		public static readonly string[,] idle_supply_current_N1_1p9_1p9 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_1p9_1p9", "13", "0.0005", "0.0012", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_1p9_1p9", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_1p9_1p9", "", "0.003", "0.0096", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_1p9_1p9", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_1Mbps_1p9_1p9
		public static readonly string[,] dynamic_supply_current_1Mbps_1p9_1p9 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_1MB_VDD1_1p9_1p9", "17", "0.0005", "0.006", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_1MB_VDD2_1p9_1p9", "", "0.0005", "0.0018", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_25Mbps_1p9_1p9
		public static readonly string[,] dynamic_supply_current_25Mbps_1p9_1p9 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_25MB_VDD1_1p9_1p9", "19", "0.0005", "0.0064", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_25MB_VDD2_1p9_1p9", "", "0.0005", "0.0028", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_100Mbps_1p9_1p9
		public static readonly string[,] dynamic_supply_current_100Mbps_1p9_1p9 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_100MB_VDD1_1p9_1p9", "21", "0.0005", "0.0084", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_100MB_VDD2_1p9_1p9", "", "0.0005", "0.0058", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N0_2p75_2p75
		public static readonly string[,] idle_supply_current_N0_2p75_2p75 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_2p75_2p75", "23", "0.003", "0.0095", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_2p75_2p75", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_2p75_2p75", "", "0.0005", "0.0012", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_2p75_2p75", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N1_2p75_2p75
		public static readonly string[,] idle_supply_current_N1_2p75_2p75 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_2p75_2p75", "27", "0.0005", "0.0012", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_2p75_2p75", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_2p75_2p75", "", "0.003", "0.0095", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_2p75_2p75", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_1Mbps_2p75_2p75
		public static readonly string[,] dynamic_supply_current_1Mbps_2p75_2p75 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_1MB_VDD1_2p75_2p75", "31", "0.0005", "0.0062", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_1MB_VDD2_2p75_2p75", "", "0.0005", "0.0019", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_25Mbps_2p75_2p75
		public static readonly string[,] dynamic_supply_current_25Mbps_2p75_2p75 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_25MB_VDD1_2p75_2p75", "33", "0.0005", "0.0066", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_25MB_VDD2_2p75_2p75", "", "0.0005", "0.0028", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_100Mbps_2p75_2p75
		public static readonly string[,] dynamic_supply_current_100Mbps_2p75_2p75 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_100MB_VDD1_2p75_2p75", "35", "0.0005", "0.009", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_100MB_VDD2_2p75_2p75", "", "0.0005", "0.0058", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N0_3p6_3p6
		public static readonly string[,] idle_supply_current_N0_3p6_3p6 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_3p6_3p6", "37", "0.003", "0.0097", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_3p6_3p6", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_3p6_3p6", "", "0.0005", "0.0013", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_3p6_3p6", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N1_3p6_3p6
		public static readonly string[,] idle_supply_current_N1_3p6_3p6 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_3p6_3p6", "41", "0.0005", "0.0013", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_3p6_3p6", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_3p6_3p6", "", "0.003", "0.0097", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_3p6_3p6", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_1Mbps_3p6_3p6
		public static readonly string[,] dynamic_supply_current_1Mbps_3p6_3p6 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_1MB_VDD1_3p6_3p6", "45", "0.0005", "0.0062", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_1MB_VDD2_3p6_3p6", "", "0.0005", "0.0019", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_25Mbps_3p6_3p6
		public static readonly string[,] dynamic_supply_current_25Mbps_3p6_3p6 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_25MB_VDD1_3p6_3p6", "47", "0.0005", "0.0067", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_25MB_VDD2_3p6_3p6", "", "0.0005", "0.0031", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_100Mbps_3p6_3p6
		public static readonly string[,] dynamic_supply_current_100Mbps_3p6_3p6 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_100MB_VDD1_3p6_3p6", "49", "0.0005", "0.0091", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_100MB_VDD2_3p6_3p6", "", "0.0005", "0.0068", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N0_5p5_5p5
		public static readonly string[,] idle_supply_current_N0_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_5p5_5p5", "51", "0.003", "0.01", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_5p5_5p5", "", "0.0005", "0.0019", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_5p5_5p5", "", "0.0005", "0.0013", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_5p5_5p5", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for idle_supply_current_N1_5p5_5p5
		public static readonly string[,] idle_supply_current_N1_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_VDD1_high_5p5_5p5", "55", "0.0005", "0.0013", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_high_5p5_5p5", "", "0.0005", "0.0018", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD1_low_5p5_5p5", "", "0.003", "0.01", "", "", "", "", "7", "", "7", "Fail" },
			{ "IDD_VDD2_low_5p5_5p5", "", "0.0005", "0.0019", "", "", "", "", "7", "", "7", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_1Mbps_5p5_5p5
		public static readonly string[,] dynamic_supply_current_1Mbps_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_1MB_VDD1_5p5_5p5", "59", "0.0005", "0.0068", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_1MB_VDD2_5p5_5p5", "", "0.0005", "0.002", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_25Mbps_5p5_5p5
		public static readonly string[,] dynamic_supply_current_25Mbps_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_25MB_VDD1_5p5_5p5", "61", "0.0005", "0.0072", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_25MB_VDD2_5p5_5p5", "", "0.0005", "0.0032", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for dynamic_supply_current_100Mbps_5p5_5p5
		public static readonly string[,] dynamic_supply_current_100Mbps_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "IDD_100MB_VDD1_5p5_5p5", "63", "0.0005", "0.0093", "", "", "", "", "9", "", "9", "Fail" },
			{ "IDD_100MB_VDD2_5p5_5p5", "", "0.0005", "0.0081", "", "", "", "", "9", "", "9", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tr_prop_delay_1M_1p7_1p7
		public static readonly string[,] Tr_prop_delay_1M_1p7_1p7 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPLH_VOA_1p7_1p7", "65", "0.000000006", "0.000000013", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPLH_VOB_1p7_1p7", "", "0.000000006", "0.000000013", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tf_prop_delay_1M_1p7_1p7
		public static readonly string[,] Tf_prop_delay_1M_1p7_1p7 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPHL_VOA_1p7_1p7", "67", "0.000000006", "0.000000013", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPHL_VOB_1p7_1p7", "", "0.000000006", "0.000000013", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for pulse_width_distortion_1M_1p7_1p7
		public static readonly string[,] pulse_width_distortion_1M_1p7_1p7 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPWD_VOA_1p7_1p7", "69", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPWD_VOB_1p7_1p7", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for ch_ch_match_1M_1p7_1p7
		public static readonly string[,] ch_ch_match_1M_1p7_1p7 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPSKCD_Fall_1p7_1p7", "71", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_RiseFall_1p7_1p7", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_Rise_1p7_1p7", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for func_1Mbps_1p7_1p7
		public static readonly string[,] func_1Mbps_1p7_1p7 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_5Mbps_1p7_1p7
		public static readonly string[,] func_5Mbps_1p7_1p7 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_50Mbps_1p7_1p7
		public static readonly string[,] func_50Mbps_1p7_1p7 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_75MHz_1p7_1p7
		public static readonly string[,] func_75MHz_1p7_1p7 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_slow_speed_1p7_1p7
		public static readonly string[,] func_slow_speed_1p7_1p7 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for input_output_levels_1p7_1p7
		public static readonly string[,] input_output_levels_1p7_1p7 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "VOL_20uA", "108", "-0.05", "0.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_2mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_4mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_20uA", "", "1.6", "1.8", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_2mA", "", "1.3", "1.8", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_4mA", "", "1.3", "1.8", "", "", "", "", "11", "", "11", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tr_prop_delay_1M_2p25_2p25
		public static readonly string[,] Tr_prop_delay_1M_2p25_2p25 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPLH_VOA_2p25_2p25", "124", "0.000000005", "0.000000012", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPLH_VOB_2p25_2p25", "", "0.000000005", "0.000000012", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tf_prop_delay_1M_2p25_2p25
		public static readonly string[,] Tf_prop_delay_1M_2p25_2p25 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPHL_VOA_2p25_2p25", "126", "0.000000005", "0.000000012", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPHL_VOB_2p25_2p25", "", "0.000000005", "0.000000012", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for pulse_width_distortion_1M_2p25_2p25
		public static readonly string[,] pulse_width_distortion_1M_2p25_2p25 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPWD_VOA_2p25_2p25", "128", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPWD_VOB_2p25_2p25", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for ch_ch_match_1M_2p25_2p25
		public static readonly string[,] ch_ch_match_1M_2p25_2p25 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPSKCD_Fall_2p25_2p25", "130", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_RiseFall_2p25_2p25", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_Rise_2p25_2p25", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for func_1Mbps_2p25_2p25
		public static readonly string[,] func_1Mbps_2p25_2p25 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_5Mbps_2p25_2p25
		public static readonly string[,] func_5Mbps_2p25_2p25 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_50Mbps_2p25_2p25
		public static readonly string[,] func_50Mbps_2p25_2p25 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_75MHz_2p25_2p25
		public static readonly string[,] func_75MHz_2p25_2p25 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_slow_speed_2p25_2p25
		public static readonly string[,] func_slow_speed_2p25_2p25 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for input_output_levels_2p25_2p25
		public static readonly string[,] input_output_levels_2p25_2p25 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "VOL_20uA", "167", "-0.05", "0.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_2mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_4mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_20uA", "", "2.15", "2.35", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_2mA", "", "1.85", "2.35", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_4mA", "", "1.85", "2.35", "", "", "", "", "11", "", "11", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tr_prop_delay_1M_3p0_3p0
		public static readonly string[,] Tr_prop_delay_1M_3p0_3p0 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPLH_VOA_3p0_3p0", "183", "0.000000004", "0.000000011", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPLH_VOB_3p0_3p0", "", "0.000000004", "0.000000011", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tf_prop_delay_1M_3p0_3p0
		public static readonly string[,] Tf_prop_delay_1M_3p0_3p0 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPHL_VOA_3p0_3p0", "185", "0.000000004", "0.000000011", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPHL_VOB_3p0_3p0", "", "0.000000004", "0.000000011", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for pulse_width_distortion_1M_3p0_3p0
		public static readonly string[,] pulse_width_distortion_1M_3p0_3p0 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPWD_VOA_3p0_3p0", "187", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPWD_VOB_3p0_3p0", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for ch_ch_match_1M_3p0_3p0
		public static readonly string[,] ch_ch_match_1M_3p0_3p0 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPSKCD_Fall_3p0_3p0", "189", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_RiseFall_3p0_3p0", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_Rise_3p0_3p0", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for func_1Mbps_3p0_3p0
		public static readonly string[,] func_1Mbps_3p0_3p0 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_5Mbps_3p0_3p0
		public static readonly string[,] func_5Mbps_3p0_3p0 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_50Mbps_3p0_3p0
		public static readonly string[,] func_50Mbps_3p0_3p0 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_75MHz_3p0_3p0
		public static readonly string[,] func_75MHz_3p0_3p0 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_slow_speed_3p0_3p0
		public static readonly string[,] func_slow_speed_3p0_3p0 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for input_output_levels_3p0_3p0
		public static readonly string[,] input_output_levels_3p0_3p0 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "VOL_20uA", "226", "-0.05", "0.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_2mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_4mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_20uA", "", "2.9", "3.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_2mA", "", "2.6", "3.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_4mA", "", "2.6", "3.1", "", "", "", "", "11", "", "11", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tr_prop_delay_1M_4p5_4p5
		public static readonly string[,] Tr_prop_delay_1M_4p5_4p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPLH_VOA_4p5_4p5", "242", "0.000000004", "0.00000001", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPLH_VOB_4p5_4p5", "", "0.000000004", "0.00000001", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for Tf_prop_delay_1M_4p5_4p5
		public static readonly string[,] Tf_prop_delay_1M_4p5_4p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPHL_VOA_4p5_4p5", "244", "0.000000004", "0.00000001", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPHL_VOB_4p5_4p5", "", "0.000000004", "0.00000001", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for pulse_width_distortion_1M_4p5_4p5
		public static readonly string[,] pulse_width_distortion_1M_4p5_4p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPWD_VOA_4p5_4p5", "246", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPWD_VOB_4p5_4p5", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for ch_ch_match_1M_4p5_4p5
		public static readonly string[,] ch_ch_match_1M_4p5_4p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "tPSKCD_Fall_4p5_4p5", "248", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_RiseFall_4p5_4p5", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" },
			{ "tPSKCD_Rise_4p5_4p5", "", "-0.000000003", "0.000000003", "", "", "", "", "15", "", "15", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for func_1Mbps_4p5_4p5
		public static readonly string[,] func_1Mbps_4p5_4p5 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_5Mbps_4p5_4p5
		public static readonly string[,] func_5Mbps_4p5_4p5 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_50Mbps_4p5_4p5
		public static readonly string[,] func_50Mbps_4p5_4p5 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_75MHz_4p5_4p5
		public static readonly string[,] func_75MHz_4p5_4p5 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for func_slow_speed_4p5_4p5
		public static readonly string[,] func_slow_speed_4p5_4p5 = new string[,]
		{

		};
		
		// Declare a read only array of strings of TNames for input_output_levels_4p5_4p5
		public static readonly string[,] input_output_levels_4p5_4p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "VOL_20uA", "285", "-0.05", "0.1", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_2mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOL_4mA", "", "-0.05", "0.4", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_20uA", "", "4.4", "4.6", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_2mA", "", "4.1", "4.6", "", "", "", "", "11", "", "11", "Fail" },
			{ "VOH_4mA", "", "4.1", "4.6", "", "", "", "", "11", "", "11", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for LEAKAGE_5p5_5p5
		public static readonly string[,] LEAKAGE_5p5_5p5 = new string[,]
		{
			//{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "PassBin", "FailBin", "PassSort", "FailSort", "Result" }
			{ "Iin_low_VDD1", "301", "-0.000001", "0.000001", "", "", "", "", "13", "", "13", "Fail" },
			{ "Iin_high_VDD1", "", "-0.000001", "0.000001", "", "", "", "", "13", "", "13", "Fail" }
		};
		
		// Declare a read only array of strings of TNames for PowerDown
		public static readonly string[,] PowerDown = new string[,]
		{
			//PseudoCode  : Tag: TheTag: StopTestLimitInformation;
		};
	}
}
