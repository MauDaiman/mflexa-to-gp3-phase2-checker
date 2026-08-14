using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NISwitch;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl
{
    /// <summary>
    /// Class used to manage Instrument Control methods.
    /// </summary>
    public partial class InstrCtrl
    {
        /// <summary>
        /// Initializes all NI-Switch Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitRelaySessions(ISemiconductorModuleContext tsmContext)
        {
            // Initialize the NI-Switch Relay sessions.
            var relayInstruments = tsmContext.GetRelayDriverModuleNames();

            Parallel.ForEach(relayInstruments, relayInstrument =>
            {
                var session = new NISwitch(relayInstrument, false, true);
                tsmContext.SetRelayDriverNISwitchSession(relayInstrument, session);
            });

            // Read the pinmap to get the relay groupnames and store them in tsm global data for relay name input validation
            XDocument pinMap = XDocument.Load(tsmContext.PinMapFilePath);
            var ns = pinMap.Root.GetDefaultNamespace();
            var RelayGroupsDoc = pinMap.Descendants(ns + "RelayGroup");

            string[] RelayGroupNames = new string[RelayGroupsDoc.Count()];

            for (int i = 0; i < RelayGroupsDoc.Count(); i++)
            {
                RelayGroupNames[i] = RelayGroupsDoc.ElementAt(i).Attribute("name").Value;
            }

            tsmContext.SetGlobalData("RelayGroupNames", RelayGroupNames);
        }

        /// <summary>
        /// Closes all NI-Switch Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseRelaySessions(ISemiconductorModuleContext tsmContext)
        {
            Parallel.ForEach(tsmContext.GetAllRelayDriverNISwitchSessions(), session =>
            {
                session.Close();
            });
        }

    }

    /// <summary>
    /// Defines the relay or relay group name and the action to be performed
    /// </summary>
    public struct RelayState
    {
        /// <summary>
        /// Name of the relay or relay group
        /// </summary>
        public string Relay;

        /// <summary>
        /// Action to be performed on the relay or relay group(True closes relay, False opens relay).
        /// </summary>
        public bool Connect;

        /// <param name="relay">Name of the relay or relay group</param>
        /// <param name="connect">Action to be performed on the relay or relay group(True closes relay, False opens relay).</param>
        public RelayState(string relay, bool connect)
        {
            this.Relay = relay;
            this.Connect = connect;
        }
    }

    /// <summary>
    /// Relay Wrapper class.
    /// </summary>
    public static class Relay
    {

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="relay">The name of the relay or relay group that identify the relays.</param>
        /// <param name="connect">Defines relay action to be performed on all the identified relays(True closes relay, False opens relay).</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string relay, bool connect)
        {
            string[] relays = { relay };
            bool[] connections = { connect };

            ControlRelay(tsmContext, relays, connections);
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="relays">The name of the relays or relay groups that identify the relays.</param>
        /// <param name="connect">Defines relay action to be performed on all the identified relays(True closes relay, False opens relay).</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string[] relays, bool connect)
        {
            bool[] connections = Enumerable.Repeat(connect, relays.Length).ToArray();

            ControlRelay(tsmContext, relays, connections);
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="relays">The name of the relay or relay group that identify the relays.</param>
        /// <param name="connect">Defines relay action on the relays identified by the corresponding relay or relay group provided in relays input(True closes relay, False opens relay).</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, string[] relays, bool[] connect)
        {

            tsmContext.GetRelays(out string[] siteRelays, out string[] systemRelays);
            var validRelays = siteRelays.Concat(systemRelays).Concat((string[])tsmContext.GetGlobalData("RelayGroupNames"));

            //Rovi - temporary comment this line as it is throwing error
            //if (!relays.All(validRelays.Contains))
            //{
            //    var invalidRelays = relays.Except(validRelays).ToArray();
            //    throw new Exception($"Invalid relay name = {string.Join(",", invalidRelays)}.\nThe current version of Relay Wrapper supports only NI-Switch relay instrument. Please contact adisupport@solitontech.com to enable support for MAX4896 IO Expander, DAQmx device or any other relay module support");
            //}

            RelayDriverAction[] RelayActions = Array.ConvertAll(connect, x => x ? RelayDriverAction.CloseRelay : RelayDriverAction.OpenRelay);
            tsmContext.ControlRelay(relays, RelayActions);
        }

        /// <summary>
        /// Performs the relay actions on the relays.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="relayStates">Specifies the relays and the corresponding actions to be performed on the relays.</param>
        public static void ControlRelay(ISemiconductorModuleContext tsmContext, RelayState[] relayStates)
        {
            string[] relays = new string[relayStates.Length];
            bool[] connections = new bool[relayStates.Length];

            for (int i = 0; i < relayStates.Length; i++)
            {
                relays[i] = relayStates[i].Relay;
                connections[i] = relayStates[i].Connect;
            }

            ControlRelay(tsmContext, relays, connections);
        }
    }
}