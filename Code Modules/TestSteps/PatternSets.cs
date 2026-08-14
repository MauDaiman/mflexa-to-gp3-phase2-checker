using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.ModularInstruments;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
	public class PatternEntry
	{
		public string FileName { get; set; }
		public string StartLabel { get; set; }
		public string StopLabel { get; set; }
		
		public PatternEntry(string fileName, string startLabel = "", string stopLabel = "")
		{
			FileName = fileName;
			StartLabel = startLabel;
			StopLabel = stopLabel;
		}
	}
	public static class PatternData
	{
		public static Dictionary<string, List<PatternEntry>> PatternSetDictionary { get; } = new Dictionary<string, List<PatternEntry>>();
		
		static PatternData()
		{
			// Add the data to the dictionary
			AddToDictionary("rise_pd_set", new PatternEntry(".\\vectors\\Tpd_rise_AB_192.PAT"));
			AddToDictionary("fall_pd_set", new PatternEntry(".\\vectors\\Tpd_fall_AB_192.PAT"));
			AddToDictionary("idd_pd_set", new PatternEntry(".\\vectors\\idd_dyn.PAT"));
			AddToDictionary("idd_mux_pd_set", new PatternEntry(".\\vectors\\idd_dyn_mux.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\pat_xMHz.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\throughput_xMHz.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_serial_pulse_high.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_compliment.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_parallel.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_serial.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_gray_code.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_zeros.PAT"));
			AddToDictionary("throughput_pd_set", new PatternEntry(".\\vectors\\func_throughput_ones.PAT"));
			AddToDictionary("throughput_mux_pd_set", new PatternEntry(".\\vectors\\func_throughput_mux_serial.PAT"));
			AddToDictionary("throughput_mux_pd_set", new PatternEntry(".\\vectors\\func_throughput_mux_parallel.PAT"));
			AddToDictionary("throughput_mux_pd_set", new PatternEntry(".\\vectors\\pat_xMHz_mux.PAT"));
			AddToDictionary("slow_speed_pd_set", new PatternEntry(".\\vectors\\func_slow_speed_serial_high.PAT"));
			AddToDictionary("pulse_width_low_10M_pd_set", new PatternEntry(".\\vectors\\func_tPWL_10M.PAT"));
			AddToDictionary("pulse_width_high_10M_pd_set", new PatternEntry(".\\vectors\\func_tPWH_10M.PAT"));
			AddToDictionary("VDD1_UVLO_N1_pd_set", new PatternEntry(".\\vectors\\UVLO_VDD1_N1.PAT"));
			AddToDictionary("VDD2_UVLO_N1_pd_set", new PatternEntry(".\\vectors\\UVLO_VDD2_N1.PAT"));
			AddToDictionary("VDD1_UVLO_N0_pd_set", new PatternEntry(".\\vectors\\UVLO_VDD1_N0.PAT"));
			AddToDictionary("VDD2_UVLO_N0_pd_set", new PatternEntry(".\\vectors\\UVLO_VDD2_N0.PAT"));
			AddToDictionary("vil_vih_pd_set", new PatternEntry(".\\vectors\\vil_vih_search.PAT"));
			AddToDictionary("static_setup_pd_set", new PatternEntry(".\\vectors\\static_init.PAT"));
			AddToDictionary("default_state_high_pd_set", new PatternEntry(".\\vectors\\powered_default_state_high.PAT"));
			AddToDictionary("default_state_low_pd_set", new PatternEntry(".\\vectors\\powered_default_state_low.PAT"));
			AddToDictionary("rise_char_set", new PatternEntry(".\\vectors\\Tpd_rise_AB_192.PAT"));
			AddToDictionary("fall_char_set", new PatternEntry(".\\vectors\\Tpd_fall_AB_192.PAT"));
			AddToDictionary("idd_char_set", new PatternEntry(".\\vectors\\idd_dyn.PAT"));
			AddToDictionary("idd_mux_char_set", new PatternEntry(".\\vectors\\idd_dyn_mux.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\pat_xMHz.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\throughput_xMHz.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_serial_pulse_high.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_compliment.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_parallel.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_serial.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_gray_code.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_zeros.PAT"));
			AddToDictionary("throughput_char_set", new PatternEntry(".\\vectors\\func_throughput_ones.PAT"));
			AddToDictionary("throughput_mux_char_set", new PatternEntry(".\\vectors\\func_throughput_mux_serial.PAT"));
			AddToDictionary("throughput_mux_char_set", new PatternEntry(".\\vectors\\func_throughput_mux_parallel.PAT"));
			AddToDictionary("throughput_mux_char_set", new PatternEntry(".\\vectors\\pat_xMHz_mux.PAT"));
			AddToDictionary("slow_speed_char_set", new PatternEntry(".\\vectors\\func_slow_speed_serial_high.PAT"));
			AddToDictionary("pulse_width_low_10M_char_set", new PatternEntry(".\\vectors\\func_tPWL_10M.PAT"));
			AddToDictionary("pulse_width_high_10M_char_set", new PatternEntry(".\\vectors\\func_tPWH_10M.PAT"));
			AddToDictionary("VDD1_UVLO_N1_char_set", new PatternEntry(".\\vectors\\UVLO_VDD1_N1.PAT"));
			AddToDictionary("VDD2_UVLO_N1_char_set", new PatternEntry(".\\vectors\\UVLO_VDD2_N1.PAT"));
			AddToDictionary("VDD1_UVLO_N0_char_set", new PatternEntry(".\\vectors\\UVLO_VDD1_N0.PAT"));
			AddToDictionary("VDD2_UVLO_N0_char_set", new PatternEntry(".\\vectors\\UVLO_VDD2_N0.PAT"));
			AddToDictionary("vil_vih_char_set", new PatternEntry(".\\vectors\\vil_vih_search.PAT"));
			AddToDictionary("static_setup_char_set", new PatternEntry(".\\vectors\\static_init.PAT"));
			AddToDictionary("default_state_high_char_set", new PatternEntry(".\\vectors\\powered_default_state_high.PAT"));
			AddToDictionary("default_state_low_char_set", new PatternEntry(".\\vectors\\powered_default_state_low.PAT"));
		}
		
		private static void AddToDictionary(string key, PatternEntry entry)
		{
			if (!PatternSetDictionary.ContainsKey(key))
			{
				PatternSetDictionary[key] = new List<PatternEntry>();
			}
			PatternSetDictionary[key].Add(entry);
		}
	}
}
