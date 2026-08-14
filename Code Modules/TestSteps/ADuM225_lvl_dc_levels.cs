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

namespace TestSteps
{
    public static class LevelsSheets
    {
        public static void InitializeDatafromSheet()
        {
            Globals.VIA_mux = new string[,]
            {
            { "VIA_mux", "", "Vil", "VDD1_vil", "" },
            { "VIA_mux", "", "Vih", "VDD1_vih", "" },
            { "VIA_mux", "", "Vol", "VDD1_vol", "" },
            { "VIA_mux", "", "Voh", "VDD1_voh", "" },
            { "VIA_mux", "", "Iol", "VDD1_iol_20", "" },
            { "VIA_mux", "", "Ioh", "VDD1_ioh_20", "" },
            { "VIA_mux", "", "Vt", "VDD1_vt", "" },
            { "VIA_mux", "", "Vch", "Vch_default", "" },
            { "VIA_mux", "", "Vcl", "Vcl_default", "" },
            { "VIA_mux", "", "Vph", "Vph_default", "" },
            { "VIA_mux", "", "Iph", "Iph_default", "" },
            { "VIA_mux", "", "Tpr", "Tpr_default", "" },
            { "VIA_mux", "", "DriverMode", "Largeswing-HiZ", "" },
            };

            Globals.VIB_mux = new string[,]
            {
            { "VIB_mux", "", "Vil", "VDD1_vil", "" },
            { "VIB_mux", "", "Vih", "VDD1_vih", "" },
            { "VIB_mux", "", "Vol", "VDD1_vol", "" },
            { "VIB_mux", "", "Voh", "VDD1_voh", "" },
            { "VIB_mux", "", "Iol", "VDD1_iol_20", "" },
            { "VIB_mux", "", "Ioh", "VDD1_ioh_20", "" },
            { "VIB_mux", "", "Vt", "VDD1_vt", "" },
            { "VIB_mux", "", "Vch", "Vch_default", "" },
            { "VIB_mux", "", "Vcl", "Vcl_default", "" },
            { "VIB_mux", "", "Vph", "Vph_default", "" },
            { "VIB_mux", "", "Iph", "Iph_default", "" },
            { "VIB_mux", "", "Tpr", "Tpr_default", "" },
            { "VIB_mux", "", "DriverMode", "Largeswing-HiZ", "" },
            };

            Globals.VOA_mux = new string[,]
            {
            { "VOA_mux", "", "Vil", "VDD2_vil", "" },
            { "VOA_mux", "", "Vih", "VDD2_vih", "" },
            { "VOA_mux", "", "Vol", "VDD2_vol", "" },
            { "VOA_mux", "", "Voh", "VDD2_voh", "" },
            { "VOA_mux", "", "Iol", "VDD2_iol_4000", "" },
            { "VOA_mux", "", "Ioh", "VDD2_ioh_4000", "" },
            { "VOA_mux", "", "Vt", "VDD2_vt", "" },
            { "VOA_mux", "", "Vch", "Vch_default", "" },
            { "VOA_mux", "", "Vcl", "Vcl_default", "" },
            { "VOA_mux", "", "Vph", "Vph_default", "" },
            { "VOA_mux", "", "Iph", "Iph_default", "" },
            { "VOA_mux", "", "Tpr", "Tpr_default", "" },
            { "VOA_mux", "", "DriverMode", "Largeswing-HiZ", "" },
            };

            Globals.VOB_mux = new string[,]
            {
            { "VOB_mux", "", "Vil", "VDD2_vil", "" },
            { "VOB_mux", "", "Vih", "VDD2_vih", "" },
            { "VOB_mux", "", "Vol", "VDD2_vol", "" },
            { "VOB_mux", "", "Voh", "VDD2_voh", "" },
            { "VOB_mux", "", "Iol", "VDD2_iol_4000", "" },
            { "VOB_mux", "", "Ioh", "VDD2_ioh_4000", "" },
            { "VOB_mux", "", "Vt", "VDD2_vt", "" },
            { "VOB_mux", "", "Vch", "Vch_default", "" },
            { "VOB_mux", "", "Vcl", "Vcl_default", "" },
            { "VOB_mux", "", "Vph", "Vph_default", "" },
            { "VOB_mux", "", "Iph", "Iph_default", "" },
            { "VOB_mux", "", "Tpr", "Tpr_default", "" },
            { "VOB_mux", "", "DriverMode", "Largeswing-HiZ", "" }
            };

            // Update the dictionary with the initialized arrays
            Globals.PinGroupMap["VIA_mux"] = Globals.VIA_mux;
            Globals.PinGroupMap["VIB_mux"] = Globals.VIB_mux;
            Globals.PinGroupMap["VOA_mux"] = Globals.VOA_mux;
            Globals.PinGroupMap["VOB_mux"] = Globals.VOB_mux;
        }
    }
}