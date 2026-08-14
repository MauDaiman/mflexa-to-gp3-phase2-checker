using NationalInstruments.ModularInstruments.NISwitch;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl
{
    /// <summary>
    /// Class used to manage Instrument Control methods.
    /// </summary>
    public partial class InstrCtrl
    {
        /// <summary>
        /// Generates a Custom Relay session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name string</param>
        /// <returns>
        /// A single Custom Relay session.
        /// </returns>
        public static CustomRelay CustomRelayPinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return CustomRelayPinsToSessions(tsmContext, pins);
        }

        /// <summary>
        /// Generates a series of Custom Relay sessions based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A CustomRelay object containing all Pin defined sessions.
        /// </returns>
        public static CustomRelay CustomRelayPinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetCustomSessions(CustomRelay.RelayInstrumentTypeID, pins, out var switchSessions, out var channelGroups, out var channelLists);

            var switchSSC = new CustomRelaySSC[switchSessions.Length];

            var sitenums = tsmContext.SiteNumbers.ToArray();

            var expandedPins = tsmContext.GetPinsInPinGroups(pins);

            tsmContext.GetPins(out var dutPins, out var systemPins);

            for (int sessionNdx = 0; sessionNdx < switchSessions.Length; sessionNdx++)
            {
                switchSSC[sessionNdx].SwitchSession = (NISwitch)switchSessions[sessionNdx];
                switchSSC[sessionNdx].DriverChannelGroup = CustomRelay.RelayInstrumentDefaultGroup;
                switchSSC[sessionNdx].DriverChannelList = channelLists[sessionNdx];
                switchSSC[sessionNdx].DriverChannelGroup = channelGroups[sessionNdx];
                switchSSC[sessionNdx].PerChannelDriverChannelList = channelLists[sessionNdx].Split(',').Select(p => p.Trim()).ToArray();
                switchSSC[sessionNdx].PerChannelTSMChannelList = new string[switchSSC[sessionNdx].PerChannelDriverChannelList.Length];
            }

            foreach (var site in sitenums)
            {
                foreach (var pin in expandedPins)
                {
                    pqc.GetSessionAndChannelIndex(site, pin, out var sessionNdx, out var channelNdx);
                    if (systemPins.Contains(pin))
                    {
                        switchSSC[sessionNdx].PerChannelTSMChannelList[channelNdx] = pin;
                    }
                    else if (switchSSC[sessionNdx].PerChannelTSMChannelList[channelNdx] == null)
                    {
                        switchSSC[sessionNdx].PerChannelTSMChannelList[channelNdx] = "Site" + site.ToString() + "/" + pin;
                    }
                    else
                    {
                        var split = switchSSC[sessionNdx].PerChannelTSMChannelList[channelNdx].Split('/');
                        switchSSC[sessionNdx].PerChannelTSMChannelList[channelNdx] = split[0] + "+" + site.ToString() + "/" + pin;
                    }
                }
            }
            for (int sessionNdx = 0; sessionNdx < switchSessions.Length; sessionNdx++)
            {
                switchSSC[sessionNdx].TSMChannelList = string.Join(",", switchSSC[sessionNdx].PerChannelTSMChannelList);
            }

            return new CustomRelay() { PinQueryContext = pqc, Pins = pins, SiteNumbers = sitenums, SSC = switchSSC };
        }

        /// <summary>
        /// Initializes all Custom Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitCustomRelaySessions(ISemiconductorModuleContext tsmContext)
        {
            tsmContext.GetCustomInstrumentNames(CustomRelay.RelayInstrumentTypeID, out var customRelayAliases, out var channelGroupIDs, out var channelLists);

            var relayMap = new Dictionary<string, List<string>>();

            for (int i = 0; i < customRelayAliases.Length; i++)
            {
                if (!relayMap.ContainsKey(customRelayAliases[i]))
                {
                    relayMap.Add(customRelayAliases[i], new List<string>());
                }
                relayMap[customRelayAliases[i]].Add(channelGroupIDs[i]);
            }

            var relayAliases = relayMap.Keys.ToArray();

            foreach (var alias in relayAliases)
            {
                var session = new NISwitch(alias, false, false);
                var relays = relayMap[alias];

                foreach (var relay in relays)
                {
                    tsmContext.SetCustomSession(CustomRelay.RelayInstrumentTypeID, alias, relay, session);
                }
            }
        }

        /// <summary>
        /// Closes all Custom Relay sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseCustomRelaySessions(ISemiconductorModuleContext tsmContext)
        {
            tsmContext.GetAllCustomSessions(CustomRelay.RelayInstrumentTypeID, out var sessions, out var channelGroupIDs, out var channelLists);

            var digitalSessions = tsmContext.GetAllNIDigitalPatternSessions();

            var relayMap = new Dictionary<NISwitch, List<string>>();

            for (int i = 0; i < sessions.Length; i++)
            {
                if (!relayMap.ContainsKey((NISwitch)sessions[i]))
                {
                    relayMap.Add((NISwitch)sessions[i], new List<string>());
                }
                relayMap[(NISwitch)sessions[i]].Add(channelGroupIDs[i]);
            }

            var switchSessions = relayMap.Keys.ToArray();

            foreach (var session in switchSessions)
            {
                var switchSession = (NISwitch)session;
                switchSession.Close();
            }

            /*
            Parallel.ForEach(sessions, (session, state, index) =>
            {
                if (channelGroupIDs[index] == CustomRelay.RelayInstrumentDefaultGroup)
                {
                    var switchSession = (NISwitch)session;
                    switchSession.Close();
                }
            }); /* */
        }
    }

    /// <summary>
    /// Structure refernce to for CustomRelay SSC members
    /// </summary>
    public struct CustomRelaySSC
    {
        /// <summary>
        /// NI-Switch session field property.
        /// </summary>
        public NISwitch SwitchSession { get; set; }
        /// <summary>
        /// TSMChannelList string property.
        /// </summary>
        public string TSMChannelList { get; set; }
        /// <summary>
        /// DriverChannelList string property.
        /// </summary>
        public string DriverChannelList { get; set; }
        /// <summary>
        /// DriverChannelGroup string property.
        /// </summary>
        public string DriverChannelGroup { get; set; }
        /// <summary>
        /// Per channel DriverChannelList string property.
        /// </summary>
        public string[] PerChannelDriverChannelList { get; set; }
        /// <summary>
        /// Per channel TSMChannelList string property.
        /// </summary>
        public string[] PerChannelTSMChannelList { get; set; }
    }

    /// <summary>
    /// Class definition for CustomRelay Sessions Objects.
    /// </summary>
    public class CustomRelay
    {
        /// <summary>
        /// Instrument type ID.
        /// </summary>
        public const string RelayInstrumentTypeID = "Switch Relay Driver";
        /// <summary>
        /// Default Instrument group.
        /// </summary>
        public const string RelayInstrumentDefaultGroup = "2567/Independent";
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public MultiplePinMultipleSessionQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// Custom Relay SSC definition.
        /// </summary>
        public CustomRelaySSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Site Numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }

        /// <summary>
        /// Performs the relay action for the relays. 
        /// </summary>
        /// <param name="closeRelays">Specifies whether to open or close a given relay, a TRUE bool value closes the relay.</param>
        /// <remarks>
        /// A boolean TRUE value indicates to close the relay.
        /// </remarks>
        public void ControlRelays(bool closeRelays)
        {
            var relayCommand = SwitchRelayAction.OpenRelay;
            if (closeRelays) relayCommand = SwitchRelayAction.CloseRelay;

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                var changeList = new List<string>();
                foreach (var channel in ssc.PerChannelDriverChannelList)
                {
                    var relayName = channel;
                    var currentPosition = ssc.SwitchSession.RelayOperations.GetRelayPosition(relayName);

                    // current position TRUE means relay is closed
                    // current position FALSE means relay is opened
                    var currentPositionBool = currentPosition == SwitchRelayPosition.Close;

                    // XOR connect command with current position, if TRUE then the state is different and it needs to be switched
                    if (closeRelays ^ currentPositionBool)
                    {
                        changeList.Add(relayName);
                    }
                }
                if (changeList.Count > 0) ssc.SwitchSession.RelayOperations.RelayControl(string.Join(",", changeList), relayCommand);
            });
        }
    }
}