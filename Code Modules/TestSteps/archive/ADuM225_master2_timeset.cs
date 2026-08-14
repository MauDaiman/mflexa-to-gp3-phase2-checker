using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps
{
    class ADuM225_master2_timeset
    {
        public static readonly string[,] TimeSet = new string[,]
        {
                { "Time Set", "Period", "CPP", "Name", "Setup", "Src", "Fmt", "On", "Data", "Return", "Off", "Mode", "Open", "Close", "Comment" },
                { "tset_phase1", "0.000001", "", "VIA_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase1", "0.000001", "", "VIA", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase1", "0.000001", "", "VIB_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase1", "0.000001", "", "VIB", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase1", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "0.000000013", "0.000000013", "nil" },
                { "tset_phase2", "0.000001", "", "VIA_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase2", "0.000001", "", "VIA", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase2", "0.000001", "", "VIB_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase2", "0.000001", "", "VIB", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase2", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "2.0667E-08", "2.0667E-08", "nil" },
                { "tset_phase3", "0.000001", "", "VIA_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase3", "0.000001", "", "VIA", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase3", "0.000001", "", "VIB_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase3", "0.000001", "", "VIB", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase3", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "0", "0", "nil" },
                { "tset_phase4", "0.000001", "", "VIA_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase4", "0.000001", "", "VIA", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase4", "0.000001", "", "VIB_ODD", "mux", "PAT", "RL", "0", "0", "0.00000025", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase4", "0.000001", "", "VIB", "mux", "PAT", "RL", "0", "0.0000005", "0.00000075", "0.000001", "Edge", "0.000001", "0.000001", "nil" },
                { "tset_phase4", "0.000001", "", "data_outs", "i/o", "PAT", "ROFF", "0", "0", "0.000001", "0.000001", "Edge", "6.67E-09", "6.67E-09", "nil" }
        };
    }
}
