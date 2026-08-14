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
    class ADuM225_pd_ipat_flow
    {
        // Declare a readonly array of strings of TNames for continuity
        public static readonly string[,] continuity = new string[,]
        {
             //{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "Pass", "Fail", "Pass", "Fail", "Result" }
            { "pos_diode", "1", "0.35", "0.51", "", "", "", "8", "", "8", "Fail" },
            { "neg_diode", "", "-0.5", "-0.35", "", "", "", "8", "", "8", "Fail" }
        };

        //-->other flow data before vil_vih_1p7_1p7

        // Declare a readonly array of strings of TNames for vil_vih_1p7_1p7
        public static readonly string[,] vil_vih_1p7_1p7 = new string[,]
        {
            //{ "TName", "TNum", "LoLim", "HiLim", "Scale", "Units", "Format", "Pass", "Fail", "Pass", "Fail", "Result" }
            { "VIA_vil", "120", "0.5448", "0.8152", "", "", "", "17", "", "17", "Fail" },
            { "VIB_vil", "", "0.5423", "0.8177", "", "", "", "17", "", "17", "Fail" },
            { "VIA_vih", "", "0.8879", "0.9821", "", "", "", "17", "", "17", "Fail" },
            { "VIB_vih", "", "0.8876", "0.9824", "", "", "", "17", "", "17", "Fail" }
        };
    }

}
