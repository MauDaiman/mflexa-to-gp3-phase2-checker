using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class Specs
    {
        //public void LoadDCSpecs(ISemiconductorModuleContext tsmContext)
        public static double DCSpecs(string specName)
        {
            //NI Specs name must begin with letter or underscore(_) and with a format Section.Variable
            //to be always safe we are adding underscore(_) at the beginning and after the dot(.)
            //Section = _Category+Selector+Environment, Variable = _Symbol
            //Example: _Cateogry+Selector+Environment._Variable --> __ctyTyp._VDD1
            //Also, special characters aside from underscore(_) are not allowed.
            //Example: _0p0/5p5Typ._VDD1 --> _0p0_5p5Typ._VDD1

            string DCSpecsName = "_" + Globals.DCSpecs.Category + Globals.DCSpecs.Selector + Globals.DCSpecs.Environment + "._";
            //return tsmContext.GetSpecificationsValue(DCSpecsName + specName);
            try
            {
                return Globals.tsmContext.GetSpecificationsValue(DCSpecsName + specName);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                //Console.WriteLine($"Error retrieving specification value: {ex.Message}");
                // Return a default value or handle the error as needed
                return double.NaN; // Returning NaN to indicate an error
            }
            /*
            Globals.VDD1 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1");
            Globals.VDD1_alt = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_alt");
            Globals.VDD2 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2");
            Globals.VDD2_alt = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_alt");
            Globals.VDD1_vih = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_vih");
            Globals.VDD1_vil = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_vil");
            Globals.VDD1_voh = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_voh");
            Globals.VDD1_vol = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_vol");
            Globals.VDD1_ioh_20 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_ioh_20");
            Globals.VDD1_iol_20 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_iol_20");
            Globals.VDD1_ioh_4000 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_ioh_4000");
            Globals.VDD1_iol_4000 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_iol_4000");
            Globals.VDD1_vt = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD1_vt");
            Globals.VDD2_vih = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_vih");
            Globals.VDD2_vil = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_vil");
            Globals.VDD2_voh = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_voh");
            Globals.VDD2_vol = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_vol");
            Globals.VDD2_ioh_20 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_ioh_20");
            Globals.VDD2_iol_20 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_iol_20");
            Globals.VDD2_ioh_4000 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_ioh_4000");
            Globals.VDD2_iol_4000 = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_iol_4000");
            Globals.VDD2_vt = tsmContext.GetSpecificationsValue(DCSpecsName + "VDD2_vt");
            Globals.idd_VDD1 = tsmContext.GetSpecificationsValue(DCSpecsName + "idd_VDD1");
            Globals.idd_VDD2 = tsmContext.GetSpecificationsValue(DCSpecsName + "idd_VDD2");
            */
        }
        //public void LoadACSpecs(ISemiconductorModuleContext tsmContext)
        public static double ACSpecs(string specName)
        {

            //NI Specs name must begin with letter or underscore(_) and with a format Section.Variable
            //to be always safe we are adding underscore(_) at the beginning and after the dot(.)
            //Section = _Category+Selector and Variable = _Symbol
            //Example: _Category+Selector._Symbol --> __1MbpsTyp._period
            //Also, special characters aside from underscore(_) are not allowed.
            string ACSpecsName = "_" + Globals.ACSpecs.Category + Globals.ACSpecs.Selector + Globals.ACSpecs.Environment + "._";
            //return tsmContext.GetSpecificationsValue(ACSpecsName + specName);
            try
            {
                return Globals.tsmContext.GetSpecificationsValue(ACSpecsName + specName);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                //Console.WriteLine($"Error retrieving specification value: {ex.Message}");
                // Return a default value or handle the error as needed
                return double.NaN; // Returning NaN to indicate an error
            }
            /*
            Globals.period = tsmContext.GetSpecificationsValue(ACSpecsName + "period");
            Globals.TpdPos1RuStepSize = tsmContext.GetSpecificationsValue(ACSpecsName + "TpdPos1RuStepSize");
            Globals.output_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "output_strobe");
            Globals.ph1_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "ph1_strobe");
            Globals.ph2_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "ph2_strobe");
            Globals.ph3_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "ph3_strobe");
            Globals.ph4_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "ph4_strobe");
            Globals.mux_ph1_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "mux_ph1_strobe");
            Globals.mux_ph2_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "mux_ph2_strobe");
            Globals.mux_ph3_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "mux_ph3_strobe");
            Globals.mux_ph4_strobe = tsmContext.GetSpecificationsValue(ACSpecsName + "mux_ph4_strobe");
            */
        }
        public static double GlobalsSpecs(string specName)
        {

            //NI Specs name must begin with letter or underscore(_) and with a format Section.Variable
            //to be always safe we are adding underscore(_) at the beginning and after the dot(.)
            //Section = "_" and Variable = _Symbol
            //Example: _._Symbol --> _._Vcl_default
            //Also, special characters aside from underscore(_) are not allowed.
            string GlobalsSpecsName = "_._";
            //return tsmContext.GetSpecificationsValue(GlobalsSpecsName + specName);
            try
            {
                return Globals.tsmContext.GetSpecificationsValue(GlobalsSpecsName + specName);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                //Console.WriteLine($"Error retrieving specification value: {ex.Message}");
                // Return a default value or handle the error as needed
                return double.NaN; // Returning NaN to indicate an error
            }
        }

        public static double GetValue(string specName)
        {
            double? specValue = Specs.DCSpecs(specName);
            if (specValue == double.NaN)
            {
                specValue = Specs.ACSpecs(specName);
            }
            if (specValue == double.NaN)
            {
                specName = RemoveBeforeSequence(specName, "_._");
                specValue = Specs.GlobalsSpecs(specName);
            }
            if (specValue == double.NaN)
            {
                throw new Exception($"No value found for spec '{specName}'.");
            }
            return specValue.Value;
        }

        public static void TNames(string[,] TNames)
        {
            int numRows = TNames.GetLength(0); // Get the number of rows
            Globals.TNames = new string[numRows]; // Initialize the array with the number of rows

            for (int i = 0; i < numRows; i++)
            {
                Globals.TNames[i] = TNames[i, 0]; // Access the first column of each row
            }
            Globals.accessCounter = 0;
            Globals.tsmContext.SetGlobalData("accessCounter", 0);
        }
        public static string RemoveBeforeSequence(string input, string sequence)
        {
            int index = input.IndexOf(sequence);
            if (index != -1)
            {
                return input.Substring(index);
            }
            return input; // Return the original string if the sequence is not found
        }

    }

    public class SubSpecs
    {
        public string Category;
        public string Selector;
        public string Environment;
    }
}
