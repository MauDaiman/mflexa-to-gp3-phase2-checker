using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps
{
    class ADuM225_master_timeset
    {
        public static readonly string[,] TimeSets = new string[,]
        {
            { "Time Set", "Period", "CPP", "Name", "Setup", "Src", "Fmt", "On", "Data", "Return", "Off", "Mode", "Open", "Close", "Comment" },
            { "tset_0", "0.000001", "", "data_ins", "i/o", "PAT", "RL", "0", "0.00000025", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_0", "0.000001", "", "data_outs", "i/o", "PAT", "NR", "0", "0", "0.00000075", "0.000001", "Edge", "0.0000005", "0.0000005", "nil" },
            { "tset_func", "0.000001", "", "data_ins", "i/o", "PAT", "NR", "0", "0", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_func", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_PWL_10M", "0.000001", "", "data_ins", "i/o", "PAT", "RH", "0", "0.00000025", "0.0000005", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_PWL_10M", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.00000075", "0.000001", "Edge", "0.00000075", "0.00000075", "nil" },
            { "tset_PWH_10M", "0.000001", "", "data_ins", "i/o", "PAT", "RL", "0", "0.00000025", "0.0000005", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_PWH_10M", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.00000075", "0.000001", "Edge", "0.00000075", "0.00000075", "nil" },
            { "tset_find_edge", "0.000001", "", "data_ins", "i/o", "PAT", "NR", "0", "0", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_find_edge", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "0.0000001", "0.0000001", "nil" },
            { "tset_sweep", "0.000001", "", "data_ins", "i/o", "PAT", "RL", "0", "0.00000057", "0.0000006", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_sweep", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.00000075", "0.000001", "Edge", "0", "0", "nil" },
            { "tset_slow_speed_H", "0.000001", "", "data_ins", "i/o", "PAT", "RH", "0", "0", "0.0000005", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
            { "tset_slow_speed_H", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "0.0000003", "0.0000003", "nil" }
        };
    }
}
