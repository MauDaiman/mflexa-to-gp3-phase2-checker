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
            AddToDictionary("rise_pd_set", new PatternEntry("Tpd_rise_AB_192"));
            AddToDictionary("fall_pd_set", new PatternEntry("Tpd_fall_AB_192"));
            AddToDictionary("idd_pd_set", new PatternEntry("idd_dyn"));
            AddToDictionary("idd_mux_pd_set", new PatternEntry("idd_dyn_mux"));
            AddToDictionary("throughput_pd_set", new PatternEntry("pat_xMHz"));
            AddToDictionary("throughput_pd_set", new PatternEntry("throughput_xMHz"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_serial_pulse_high"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_compliment"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_parallel"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_serial"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_gray_code"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_zeros"));
            AddToDictionary("throughput_pd_set", new PatternEntry("func_throughput_ones"));
            AddToDictionary("throughput_mux_pd_set", new PatternEntry("func_throughput_mux_serial"));
            AddToDictionary("throughput_mux_pd_set", new PatternEntry("func_throughput_mux_parallel"));
            AddToDictionary("throughput_mux_pd_set", new PatternEntry("pat_xMHz_mux"));
            AddToDictionary("slow_speed_pd_set", new PatternEntry("func_slow_speed_serial_high"));
            AddToDictionary("pulse_width_low_10M_pd_set", new PatternEntry("func_tPWL_10M"));
            AddToDictionary("pulse_width_high_10M_pd_set", new PatternEntry("func_tPWH_10M"));
            AddToDictionary("VDD1_UVLO_N1_pd_set", new PatternEntry("UVLO_VDD1_N1"));
            AddToDictionary("VDD2_UVLO_N1_pd_set", new PatternEntry("UVLO_VDD2_N1"));
            AddToDictionary("VDD1_UVLO_N0_pd_set", new PatternEntry("UVLO_VDD1_N0"));
            AddToDictionary("VDD2_UVLO_N0_pd_set", new PatternEntry("UVLO_VDD2_N0"));
            AddToDictionary("vil_vih_pd_set", new PatternEntry("vil_vih_search"));
            AddToDictionary("static_setup_pd_set", new PatternEntry("static_init"));
            AddToDictionary("default_state_high_pd_set", new PatternEntry("powered_default_state_high"));
            AddToDictionary("default_state_low_pd_set", new PatternEntry("powered_default_state_low"));
            AddToDictionary("rise_char_set", new PatternEntry("Tpd_rise_AB_192"));
            AddToDictionary("fall_char_set", new PatternEntry("Tpd_fall_AB_192"));
            AddToDictionary("idd_char_set", new PatternEntry("idd_dyn"));
            AddToDictionary("idd_mux_char_set", new PatternEntry("idd_dyn_mux"));
            AddToDictionary("throughput_char_set", new PatternEntry("pat_xMHz"));
            AddToDictionary("throughput_char_set", new PatternEntry("throughput_xMHz"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_serial_pulse_high"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_compliment"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_parallel"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_serial"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_gray_code"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_zeros"));
            AddToDictionary("throughput_char_set", new PatternEntry("func_throughput_ones"));
            AddToDictionary("throughput_mux_char_set", new PatternEntry("func_throughput_mux_serial"));
            AddToDictionary("throughput_mux_char_set", new PatternEntry("func_throughput_mux_parallel"));
            AddToDictionary("throughput_mux_char_set", new PatternEntry("pat_xMHz_mux"));
            AddToDictionary("slow_speed_char_set", new PatternEntry("func_slow_speed_serial_high"));
            AddToDictionary("pulse_width_low_10M_char_set", new PatternEntry("func_tPWL_10M"));
            AddToDictionary("pulse_width_high_10M_char_set", new PatternEntry("func_tPWH_10M"));
            AddToDictionary("VDD1_UVLO_N1_char_set", new PatternEntry("UVLO_VDD1_N1"));
            AddToDictionary("VDD2_UVLO_N1_char_set", new PatternEntry("UVLO_VDD2_N1"));
            AddToDictionary("VDD1_UVLO_N0_char_set", new PatternEntry("UVLO_VDD1_N0"));
            AddToDictionary("VDD2_UVLO_N0_char_set", new PatternEntry("UVLO_VDD2_N0"));
            AddToDictionary("vil_vih_char_set", new PatternEntry("vil_vih_search"));
            AddToDictionary("static_setup_char_set", new PatternEntry("static_init"));
            AddToDictionary("default_state_high_char_set", new PatternEntry("powered_default_state_high"));
            AddToDictionary("default_state_low_char_set", new PatternEntry("powered_default_state_low"));
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


