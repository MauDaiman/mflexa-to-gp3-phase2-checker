// <copyright file="LoadBoardCtrl.cs" company="NI">
// Copyright (c) NI. All right reserved under the NI Sample Code License.
// http://ni.com/samplecodelicense
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.STS;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{

    /// <summary>
    /// Class of defining tester and applications specific load board power.
    /// </summary>
    public static class LoadBoardCtrl
    {
        /// <summary>
        /// Array of pin names defined in the pinmap file that connect to the System Power Supply at P143.
        /// </summary>
        internal static readonly string[] SystemPowerSupplyPins = new string[] { "PPS4110_POS6V", "PPS4110_POS20V", "PPS4110_NEG20V" };

        /// <summary>
        /// Array of the voltage levels to be supplied to the SystemPowerSupplyPins.
        /// </summary>
        private static readonly double[] SystemPowerSupplyVoltages = new double[SystemPowerSupplyPins.Length];

        /// <summary>
        /// Current Limit to be applied to the SystemPowerSupplyPins.
        /// </summary>
        private static readonly double CurrentLimit = 1;

        /// <summary>
        /// Array of load board supply names, formated with dixed voltage level & spring pin block location.
        /// For example: { POS12V_P143, POS48V_P143}.
        /// </summary>
        internal static readonly string[] FixedVoltageSupplyStrings = new string[] { "POS12V_P143", "POS12V_P179_CHY", "POS24V_P102_CH0", "POS24V_P102_CH1", "POS48V_P179_CHX", "POS48V_P143" };

        /// <summary>
        ///  24V Fixed-Voltage Auxilary Power Supply.
        ///  Spring Pin Block Location: P102 (CHy).
        ///  Name in STS Maintence Software: AUX PS1 CH0.
        /// </summary>
        private const string POS24V_P102_CH0 = "AuxPs1/0";

        /// <summary>
        ///  24V Fixed-Voltage Auxilary Power Supply.
        ///  Spring Pin Block Location: P102 (CHx).
        ///  Name in STS Maintence Software: AUX PS1 CH1.
        /// </summary>
        private const string POS24V_P102_CH1 = "AuxPs1/1";

        /// <summary>
        ///  12V Fixed-Voltage Auxilary Power Supply.
        ///  Spring Pin Block Location: P179 (CHy).
        ///  Name in STS Maintence Software: AUX PS1 CH2.
        /// </summary>
        private const string POS12V_P179_CHY = "AuxPs1/2";

        /// <summary>
        ///  48V Fixed-Voltage Auxilary Power Supply.
        ///  Spring Pin Block Location: P179 (CHx).
        ///  Name in STS Maintence Software: AUX PS1 CH3.
        /// </summary>
        private const string POS48V_P179_CHX = "AuxPs1/3";

        /// <summary>
        /// This method either enables or disables the output of the System Power Supply pins defined in user's pinmap file,
        /// as well as the fixed-voltage power supplies provided by the testhead.
        /// The system power supply lines are first operated on simultaneously. 
        /// Then the fixed-voltage supplies are operated on sequentially, in the same order as the input parameters are presented.
        /// Modify this code to implement an alternative bring-up sequence for the load board supplies. 
        /// This method should be called directly from TestStand within the ProcessSetup sequence.
        /// </summary>
        /// <param name="tsmContext">Reference to a Semiconductor Module Context object</param>
        /// <param name="enablePos6v">Positive 5V Supply pin (Default: "LB_p5") defined in the pinmap file that connect to the System Power Supply at P143</param>
        /// <param name="enablePos20v">Positive 15V Supply pin (Default: "LB_p15") defined in the pinmap file that connect to the System Power Supply at P143</param>
        /// <param name="enableNeg20v">Negative 15V Supply pin (Default: "LB_n15") defined in the pinmap file that connect to the System Power Supply at P143</param>
        /// <param name="enablePos12vAtP143">Postive 12V System Supply at P143.</param>
        /// <param name="enablePos12vAtP179">Postive 12V Aux Supply at P179.</param>
        /// <param name="enableNeg24vAtP102">Negative 24V Aux Supply at P102.</param>
        /// <param name="enablePos24vAtP102">Postive 24V Aux Supply at P102.</param>
        /// <param name="enablePos48vAtP179">Postive 48V Aux Supply at P179.</param>
        /// <param name="enablePos48vAtP143">Postive 48V System Supply at P143.</param>
        /// <param name="offlineModeEnabled">Offline Mode Flag.</param>
        public static void EnableLoadBoardSupplies(
            ISemiconductorModuleContext tsmContext,
            bool enablePos6v,
            bool enablePos20v,
            bool enableNeg20v,
            bool enablePos12vAtP143,
            bool enablePos12vAtP179,
            bool enableNeg24vAtP102,
            bool enablePos24vAtP102,
            bool enablePos48vAtP179,
            bool enablePos48vAtP143,
            bool offlineModeEnabled = false)
        {
            // Enable System Power Supply at P143
            EnableSystemPowerSupplies(
                tsmContext,
                enablePos6v,
                enablePos20v,
                enableNeg20v);

            // Enable TestHead Fixed-Voltage Power Supplies.
            EnableFixedVoltageSupplies(
                enablePos12vAtP143, // +12V @ P143.
                enablePos12vAtP179, // +12V @ P179 (CHy) - AUX PS1 CH2.
                enableNeg24vAtP102, // -24V @ P102 (CHy) - AUX PS1 CH0.
                enablePos24vAtP102, // +24V @ P102 (CHx) - AUX PS1 CH1.
                enablePos48vAtP179, // +48V @ P179 (CHx) - AUX PS1 CH3.
                enablePos48vAtP143, // +48V @ P143.
                offlineModeEnabled // Offline Mode.
                );
        }

        /// <summary>
        /// This method either enables or disables the output of the System Power Supply pins defined in user's pinmap file.
        /// The power supplies are operated on simultaneously.
        /// This method can be called directly from TestStand or from a statement from another calling method.
        /// </summary>
        /// <param name="enablePos5v">Positive 5V Supply pin (Default: "LB_p5") defined in the pinmap file that connect to the System Power Supply at P143</param>
        /// <param name="enablePos15v">Positive 15V Supply pin (Default: "LB_p15") defined in the pinmap file that connect to the System Power Supply at P143</param>
        /// <param name="enableNeg15v">Negative 15V Supply pin (Default: "LB_n15") defined in the pinmap file that connect to the System Power Supply at P143</param>
        public static void EnableSystemPowerSupplies(
            ISemiconductorModuleContext tsmContext,
            bool enablePos6v,
            bool enablePos20v,
            bool enableNeg20v)
        {
            // Initialize the Voltage Levels to be applied to the SystemPowerSupplyPins. 
            // Modify these values and the parameter names if using different voltages.
            SystemPowerSupplyVoltages[0] = enablePos6v ? 6 : 0;
            SystemPowerSupplyVoltages[1] = enablePos20v ? 6.5 : 0;
            SystemPowerSupplyVoltages[2] = enableNeg20v ? -6.5 : 0;

            // Get sessions for the pins defined in SystemPowerSupplyPins
            DCPower sysPwrSupplySessions = InstrCtrl.DCPowerPinsToSessions(tsmContext, SystemPowerSupplyPins);

            // Force voltage on the SmuSupplyPins based on the voltage level and current limit configured
            sysPwrSupplySessions.ForceVoltage(SystemPowerSupplyVoltages, CurrentLimit);
        }

        /// <summary>
        /// This method either enables or disables the output of the various fixed-voltage load board power supplies provided by the testhead.
        /// The power supplies are operated on sequentially, in the same order as the input parameters are presented.
        /// This method can be called directly from TestStand or from a statement from another calling method.
        /// </summary>
        /// <param name="enablePos12vAtP143">Postive 12V System Supply at P143.</param>
        /// <param name="enablePos12vAtP179">Postive 12V Aux Supply at P179.</param>
        /// <param name="enableNeg24vAtP102">Negative 24V Aux Supply at P102.</param>
        /// <param name="enablePos24vAtP102">Postive 24V Aux Supply at P102.</param>
        /// <param name="enablePos48vAtP179">Postive 48V Aux Supply at P179.</param>
        /// <param name="enablePos48vAtP143">Postive 48V System Supply at P143.</param>
        /// <param name="offlineModeEnabled">Offline Mode Flag.</param>
        public static void EnableFixedVoltageSupplies(
            bool enablePos12vAtP143,
            bool enablePos12vAtP179,
            bool enablePos24vAtP102Ch0,
            bool enablePos24vAtP102Ch1,
            bool enablePos48vAtP179,
            bool enablePos48vAtP143,
            bool offlineModeEnabled = false)
        {
            try
            {
                string[] auxSupplies = new string[] { POS12V_P179_CHY, POS24V_P102_CH0, POS24V_P102_CH1, POS48V_P179_CHX };
                bool[] auxSupplyStates = new bool[] { enablePos12vAtP179, enablePos24vAtP102Ch0, enablePos24vAtP102Ch1, enablePos48vAtP179 };

                using (var testHead = new TestHead(GetSystemControllerIpAddress(), 5000, optionString: $"Simulate={(offlineModeEnabled ? 1 : 0)}"))
                {
                    testHead.DIB12V(enablePos12vAtP143);
                    for (int i = 0; i < auxSupplies.Length; i++)
                    {
                        testHead.AuxSupplyConfigureOutputEnabled(auxSupplies[i], auxSupplyStates[i]);
                    }

                    testHead.DIB48V(enablePos48vAtP143);
                }
            }
            catch (Exception)
            {
                // ToDo: Improve Error Handling
                throw;
            }
        }

        /// <summary>
        /// This method returns the state of the six different fixed-voltage load board power supplies provided by the testhead.
        /// The power supplies are measured and queried sequentially, from lowest voltage supply (12V) to highest voltage supply (48V).
        /// This method can be called directly from TestStand or from a statement from another calling method.
        /// </summary>
        /// <param name="offlineModeEnabled">OPTIONAL. Tells method whether to simulate the connection to the Test Head or not.</param>
        /// <returns>Dictionary keyed by each item in SupplyStrings, where the value pair is a boolean array of {powerGood, inhibitCtrl, inhibitStatus}.</returns>
        public static Dictionary<string, bool[]> QueryFixedVoltageSupplies(bool offlineModeEnabled = false)
        {
            int numSupplies = FixedVoltageSupplyStrings.Length;
            bool[] powerGood = new bool[numSupplies];
            bool[] inhibitCtrl = new bool[numSupplies];
            bool[] inhibitStatus = new bool[numSupplies];
            Dictionary<string, bool[]> result = new Dictionary<string, bool[]>();

            try
            {
                using (var testHead = new TestHead(GetSystemControllerIpAddress(), 5000, optionString: $"Simulate={(offlineModeEnabled ? 1 : 0)}"))
                {
                    // Bring Up 12V First.
                    testHead.DIB12VEnabled(
                        out powerGood[0],
                        out inhibitCtrl[0],
                        out inhibitStatus[0]);

                    // Query All Aux Supplies at once, starting with the 12V.
                    testHead.AuxSupplyQueryState(
                        "AuxPs1/0:3",
                        out List<bool> powerGoodTemp,
                        out List<bool> inhibitCtrlTemp,
                        out List<bool> inhibitStatusTemp,
                        out _);

                    // Bring Up System 48V Last.
                    testHead.DIB48VEnabled(
                        out powerGood[5],
                        out inhibitCtrl[5],
                        out inhibitStatus[5]);

                    // Copy temp list values into local variables.
                    powerGoodTemp.CopyTo(powerGood, 1);
                    inhibitCtrlTemp.CopyTo(inhibitCtrl, 1);
                    inhibitStatusTemp.CopyTo(inhibitStatus, 1);
                };
            }

            catch (Exception)
            {
                // ToDo: Improve Error Handling
                throw;
            }

            for (int i = 0; i < numSupplies; i++)
            {
                result.Add(FixedVoltageSupplyStrings[i], new bool[] { powerGood[i], inhibitCtrl[i], inhibitStatus[i] });
            }

            return result;
        }

        /// <summary>
        /// This method queries the state of one of six different fixed-voltage load board power supplies provided by the testhead.
        /// This method can be called directly from TestStand or from a statement from another calling method.
        /// </summary>
        /// <param name="supplyName">Name of the requested supply being queried.</param>
        /// <param name="offlineModeEnabled">OPTIONAL. Tells method whether to simulate the connection to the Test Head or not.</param>
        /// <returns>Query Results for the requested supply, where the returned boolean array of results contains 3 supply state values: {powerGood, inhibitCtrl, inhibitStatus} </returns>
        public static bool[] QueryFixedVoltageSupply(string supplyName, bool offlineModeEnabled = false)
        {
            Dictionary<string, bool[]> queryResults = QueryFixedVoltageSupplies(offlineModeEnabled);
            return queryResults[supplyName];
        }

        /// <summary>
        /// This method prints the query results to a table string,
        /// which can be written to the System.Console output window or
        /// logged to disk with File IO.
        /// </summary>
        /// <param name="queryResults">Dictionary object of results from QueryFixedVoltageSupplies() method.</param>
        /// <returns>Table formated string of queryResults.</returns>
        public static string PrintFixedVoltageSupplyQueryResults(Dictionary<string, bool[]> queryResults)
        {
            // Setup table, formater, headers, and filler character row.
            StringBuilder table = new StringBuilder();
            int maxSupplyStringLength = Enumerable.Max(FixedVoltageSupplyStrings.Select(x => x.Length));
            string tableFormater = $"|{{0,{maxSupplyStringLength + 1}}} |{{1,11}} |{{2,16}} |{{3,16}} |\n";
            string headerString = String.Format(tableFormater, "Supply Name", "Power Good", "Inhibit Control", "Inhibit Status");
            string tableFiller = new string('-', headerString.Length) + '\n';

            // Build table header
            table.Append(tableFiller);
            table.Append(headerString);
            table.Append(tableFiller);

            // Build table rows
            foreach (var supply in FixedVoltageSupplyStrings)
            {
                table.Append(String.Format(
                    tableFormater,
                    supply,
                    queryResults[supply][0],
                    queryResults[supply][1],
                    queryResults[supply][2])
                    );
            }

            // Table footer
            table.Append(tableFiller);

            return table.ToString();
        }

        private static string GetSystemControllerIpAddress()
        {
            string mSystemControllerIpAddress = "172.22.11.2";
            try
            {
                const string SystemDefinitionFileName = "STS_Definition.xml";
                const string STSSystemDefinition = "STSSystemDefinition";
                const string SystemController = "SystemController";
                const string IP = "IP";

                string systemDefinitionFilePath = Path.Combine(Environment.GetEnvironmentVariable("STSMSData"), SystemDefinitionFileName);

                // define default namespace used in sts definition xml
                XNamespace systemDefinitionXmlNamespace = @"http://www.ni.com/TestStand/SemiconductorModule/STSSystemDefinition.xsd";

                // load xml document
                var xDoc = XDocument.Load(systemDefinitionFilePath);

                // parse xml document for system controller ip address
                var systemDefinitionElement = xDoc.Element(systemDefinitionXmlNamespace + STSSystemDefinition);
                var systemControllerElement = systemDefinitionElement.Element(systemDefinitionXmlNamespace + SystemController);
                var systemControllerIpAddress = systemControllerElement.Attribute(IP).Value;

                if (!string.IsNullOrEmpty(systemControllerIpAddress))
                {
                    mSystemControllerIpAddress = systemControllerIpAddress;
                }
            }
            catch (FileNotFoundException)
            {
                // Assume Offline Mode - Use default address.
                mSystemControllerIpAddress = "127.0.0.1";
            }
            catch (Exception)
            {
                // ToDo: Improve Error Handling
                throw;
            }

            return mSystemControllerIpAddress;
        }
    }
}
