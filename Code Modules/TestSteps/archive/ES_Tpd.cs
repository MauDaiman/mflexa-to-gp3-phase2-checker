using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps
{
    class ES_Tpd
    {
        public static readonly string[,] EdgeSets = new string[,]
        {
            { "Pin/Group", "Edge Set", "Src", "Fmt", "On", "Data", "Return", "Off", "Mode", "Open", "Close", "Comment" },
            { "inputs", "In0", "PAT", "NR", "0", "0", "", "0.00000002", "Off", "", "", "" },
            { "inputs", "In1", "PAT", "NR", "0", "0.000000005", "", "0.00000002", "Off", "", "", "" },
            { "inputs", "In2", "PAT", "NR", "0", "0.00000001", "", "0.00000002", "Off", "", "", "" },
            { "inputs", "In3", "PAT", "NR", "0", "0.000000015", "", "0.00000002", "Off", "", "", "" },
            { "outputs", "Strb0", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "0", "", "" },
            { "outputs", "Strb1", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.9231E-10", "", "" },
            { "outputs", "Strb2", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.8462E-10", "", "" },
            { "outputs", "Strb3", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "5.7693E-10", "", "" },
            { "outputs", "Strb4", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "7.6924E-10", "", "" },
            { "outputs", "Strb5", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "9.6155E-10", "", "" },
            { "outputs", "Strb6", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.15386E-09", "", "" },
            { "outputs", "Strb7", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.34617E-09", "", "" },
            { "outputs", "Strb8", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.53848E-09", "", "" },
            { "outputs", "Strb9", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.73079E-09", "", "" },
            { "outputs", "Strb10", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "1.9231E-09", "", "" },
            { "outputs", "Strb11", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "2.11541E-09", "", "" },
            { "outputs", "Strb12", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "2.30772E-09", "", "" },
            { "outputs", "Strb13", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "2.50003E-09", "", "" },
            { "outputs", "Strb14", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "2.69234E-09", "", "" },
            { "outputs", "Strb15", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "2.88465E-09", "", "" },
            { "outputs", "Strb16", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.07696E-09", "", "" },
            { "outputs", "Strb17", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.26927E-09", "", "" },
            { "outputs", "Strb18", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.46158E-09", "", "" },
            { "outputs", "Strb19", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.65389E-09", "", "" },
            { "outputs", "Strb20", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "3.8462E-09", "", "" },
            { "outputs", "Strb21", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "4.03851E-09", "", "" },
            { "outputs", "Strb22", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "4.23082E-09", "", "" },
            { "outputs", "Strb23", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "4.42313E-09", "", "" },
            { "outputs", "Strb24", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "4.61544E-09", "", "" },
            { "outputs", "Strb25", "PAT", "ROFF", "0", "0", "", "0.00000002", "Edge", "4.80775E-09", "", "" }
        };
    }
}
