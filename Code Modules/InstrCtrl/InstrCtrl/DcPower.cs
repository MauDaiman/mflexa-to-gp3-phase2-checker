using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.Tdms;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl
{
    /// <summary>
    /// Class used to manage Instrument Control methods.
    /// </summary>
    public partial class InstrCtrl
    {
        /// <summary>
        /// Default Power Line Frequency value = 60Hz.
        /// </summary>
        public static double PowerLineFrequency = 60;
        /// <summary>
        /// Default condition for UseSoftwareMeasureTriggers variable.
        /// </summary>
        public static bool UseSoftwareMeasureTriggers = false;
        /// <summary>
        /// Dictionaty definition for DCPower Model Instruments.
        /// </summary>
        public static Dictionary<string, int> DCPowerModelStringtoModelInt = new Dictionary<string, int>
        {
            { "NI PXI-4110", 4110},
            { "NI PXI-4130", 4130},
            { "NI PXI-4131A", 4131},
            { "NI PXI-4132", 4132},
            { "NI PXIe-4112", 4112},
            { "NI PXIe-4113", 4113},
            { "NI PXIe-4135", 4135},
            { "NI PXIe-4136", 4136},
            { "NI PXIe-4137", 4137},
            { "NI PXIe-4137 (40W)", 4137},
            { "NI PXIe-4138", 4138},
            { "NI PXIe-4139", 4139},
            { "NI PXIe-4139 (40W)", 4139},
            { "NI PXIe-4140", 4140},
            { "NI PXIe-4141", 4141},
            { "NI PXIe-4141 (High Sense Resistance)", 4141},
            { "NI PXIe-4142", 4142},
            { "NI PXIe-4143", 4143},
            { "NI PXIe-4144", 4144},
            { "NI PXIe-4145", 4145},
            { "NI PXIe-4154", 4154},
            { "NI PXIe-4162", 4162},
            { "NI PXIe-4163", 4163},
            { "NI PXIe-4147", 4147}
        };

        /// <summary>
        /// Dictionaty definition for DCPower System Pins.
        /// </summary>
        public static ConcurrentDictionary<string, byte> DCPowerSystemPins = new ConcurrentDictionary<string, byte>();
        /// <summary>
        /// Dictionaty definition for DCPower DUT Pins.
        /// </summary>
        public static ConcurrentDictionary<string, byte> DCPowerDutPins = new ConcurrentDictionary<string, byte>();
        /// <summary>
        /// Dictionaty definition for DCPower session to SSC members.
        /// </summary>
        public static ConcurrentDictionary<NIDCPower, DCPowerSSC> DCSessionToSSC = new ConcurrentDictionary<NIDCPower, DCPowerSSC>();

        /// <summary>
        /// Generates a NI-DCPower session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name string</param>
        /// <returns>
        /// A single NI-DCPower session.
        /// </returns>
        public static DCPower DCPowerPinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return DCPowerPinsToSessions(tsmContext, pins);
        }

        /// <summary>
        /// Generates a series of NI-DCPower sessions based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A NI-DCPower object conatining all Pin defined sessions.
        /// </returns>
        public static DCPower DCPowerPinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetNIDCPowerSessions(pins, out var dcPowerSessions, out var dcPowerChannelStrings);

            var dcpowerssc = new DCPowerSSC[dcPowerSessions.Length];

            var siteNumbers = tsmContext.SiteNumbers.ToArray();

            //for (int sessionNdx = 0; sessionNdx < dcPowerSessions.Length; sessionNdx++)
            //{
            //    dcpowerssc[sessionNdx] = DCSessionToSSC[dcPowerSessions[sessionNdx]];
            //    dcpowerssc[sessionNdx].PinIndex = Array.IndexOf(pins, dcpowerssc[sessionNdx].Pin);
            //    dcpowerssc[sessionNdx].SiteIndex = Array.IndexOf(siteNumbers, dcpowerssc[sessionNdx].SiteNumber);
            //}

            Parallel.For(0, dcPowerSessions.Length, sessionNdx =>   //Run in parallel -adrian
            {
               dcpowerssc[sessionNdx] = DCSessionToSSC[dcPowerSessions[sessionNdx]];
               dcpowerssc[sessionNdx].PinIndex = Array.IndexOf(pins, dcpowerssc[sessionNdx].Pin);
               dcpowerssc[sessionNdx].SiteIndex = Array.IndexOf(siteNumbers, dcpowerssc[sessionNdx].SiteNumber);
            });

            return new DCPower() { PinQueryContext = pqc, SSC = dcpowerssc, SiteNumbers = siteNumbers, Pins = pins };
        }

        /// <summary>
        /// Initializes all NI-DCPower sessions.
        /// </summary>
        /// <remarks>
        /// As optional parameters, powerline frequency (Hz) for AC sources and usage of software trigger for measurements can be configured for all sources.
        /// </remarks>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="powerLineFrequencyinHz">Value of Power Line Frequency in Hz. Default value = 60Hz.</param>
        /// <param name="useSoftwareMeasureTriggers">Defines if software triggers will be used. Default value is set to false (dont use software triggers).</param>
        public static void InitDCPowerSessions(ISemiconductorModuleContext tsmContext, double powerLineFrequencyinHz = 60.0, bool useSoftwareMeasureTriggers = false)
        {
            tsmContext.GetPins(out var dutPins, out var systemPins);

            foreach (var pin in dutPins) DutPins.TryAdd(pin, 0);
            foreach (var pin in systemPins) SystemPins.TryAdd(pin, 0);

            tsmContext.GetPins(InstrumentTypeIdConstants.NIDCPower, out var DCDUTPins, out var DCSystemPins);

            foreach (var pin in DCDUTPins) DCPowerDutPins.TryAdd(pin, 0);
            foreach (var pin in DCSystemPins) DCPowerSystemPins.TryAdd(pin, 0);

            PowerLineFrequency = powerLineFrequencyinHz;

            UseSoftwareMeasureTriggers = useSoftwareMeasureTriggers;

#pragma warning disable CS0618 // Type or member is obsolete
            var instrumentAliases = tsmContext.GetNIDCPowerInstrumentNames(out var channels);
#pragma warning restore CS0618 // Type or member is obsolete

            var sessions = new NIDCPower[instrumentAliases.Length];

            var AliasesandChannels = instrumentAliases.Zip(channels, (alias, chan) => new { Alias = alias, Channel = chan });
            Parallel.ForEach(AliasesandChannels, (AC, state, index) =>
            {
                // Open a new session to the instrument and channel pair
                // We are intentionally doing reset=false here, so that the session is opened regardless of instrument state
                var session = new NIDCPower(AC.Alias, AC.Channel, false);

                // Attempt to reset the session to its default state
                try { session.Utility.Reset(); }
                // If an exception is caught during reset this is typically an OCP error, which requres a full device reset to recover the instrument
                catch { session.Utility.ResetDevice(); }

                // Parse out the numeric model of the SMU
                int instrumentModel = int.Parse(Regex.Match(session.Identity.InstrumentModel, "[0-9]{4}").Value);

                // Configure Power line frequency if supported by the hardware
                switch (instrumentModel)
                {
                    case 4110:
                    case 4130:
                    case 4154:
                        // these models don't support the power line frequency property
                        break;
                    default:
                        session.Outputs[AC.Channel].Measurement.PowerLineFrequency = PowerLineFrequency;
                        break;
                }

                // Configure measure mode if supported by the hardware
                if (useSoftwareMeasureTriggers && instrumentModel != 4110)
                {
                    session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                    session.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger();
                }

                // Initiate the sessions when appropriate
                switch (instrumentModel)
                {
                    case 4112:
                    case 4113:
                        // The device output state of the 4112 and 4113 changes when you call initiate
                        // Their default reset is disconnected, but calling initite sets them to force 30mV (see manual for minimum programmable voltage level)
                        // This may not be desireable, avoid inititing devices of these type
                        break;
                    default:
                        // Initiating the session here is a debugging convenience
                        // This allows interactive measurements of the channels in tools like Digital Pattern Editor and InstrumentStudio
                        // For the majority of DCPower devices their reset state is Forcing 0V, calling initiate doesn't change anything about this
                        session.Control.Initiate();
                        break;
                }

                sessions[index] = session;
            });

            // Assign the sessions into the TSM context
            for (int i = 0; i < sessions.Length; i++)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                tsmContext.SetNIDCPowerSession(instrumentAliases[i], channels[i], sessions[i]);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            var allDCPins = new List<string>();
            allDCPins.AddRange(DCDUTPins);
            allDCPins.AddRange(DCSystemPins);

            var pqc = tsmContext.GetNIDCPowerSessions(allDCPins.ToArray(), out var dcPowerSessions, out var dcPowerChannelStrings);

            var dcpowerssc = new DCPowerSSC[dcPowerSessions.Length];

            for (int i = 0; i < dcpowerssc.Length; i++)
            {
                dcpowerssc[i].Session = dcPowerSessions[i];
                dcpowerssc[i].DriverChannelList = dcPowerChannelStrings[i];
                dcpowerssc[i].InstrumentModel = DCPowerModelStringtoModelInt[dcPowerSessions[i].Identity.InstrumentModel];
            }

            var siteNumbers = tsmContext.SiteNumbers.ToArray();

            if (DCDUTPins.Count() > 0)
                for (int siteNdx = 0; siteNdx < siteNumbers.Length; siteNdx++)
                {
                    for (int pinIndex = 0; pinIndex < DCDUTPins.Length; pinIndex++)
                    {
                        pqc.GetSessionAndChannelIndex(siteNumbers[siteNdx], DCDUTPins[pinIndex], out var sessNdx, out var chanNdx);

                        dcpowerssc[sessNdx].SiteNumber = siteNumbers[siteNdx];
                        dcpowerssc[sessNdx].Pin = DCDUTPins[pinIndex];

                        if (string.IsNullOrEmpty(dcpowerssc[sessNdx].TSMChannelList))
                        {
                            dcpowerssc[sessNdx].TSMChannelList = "site" + siteNumbers[siteNdx] + "/" + DCDUTPins[pinIndex];
                        }
                        else
                        {
                            var split = dcpowerssc[sessNdx].TSMChannelList.Split('/');
                            dcpowerssc[sessNdx].TSMChannelList = split[0] + "+" + siteNumbers[siteNdx] + "/" + DCDUTPins[pinIndex];
                        }
                    }
                }

            if (DCSystemPins.Count() > 0)
                for (int pinIndex = 0; pinIndex < DCSystemPins.Length; pinIndex++)
                {
                    pqc.GetSessionAndChannelIndex(siteNumbers[0], DCSystemPins[pinIndex], out var sessNdx, out var chanNdx);

                    dcpowerssc[sessNdx].SiteNumber = 0;
                    dcpowerssc[sessNdx].Pin = DCSystemPins[pinIndex];
                    dcpowerssc[sessNdx].TSMChannelList = DCSystemPins[pinIndex];
                }

            foreach (var ssc in dcpowerssc)
            {
                DCSessionToSSC[ssc.Session] = ssc;
            }
        }

        /// <summary>
        /// Closes all NI-DCPower sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseDCPowerSessions(ISemiconductorModuleContext tsmContext)
        {
            var sessions = tsmContext.GetAllNIDCPowerSessions();

            foreach (var session in sessions)
            {
                // leave instruments in their reset state
                try { session.Utility.Reset(); }
                // if an error occurs resetting, the device may have an OCP error, which can be recovered by resetting the device
                catch { session.Utility.ResetDevice(); }

                session.Close();
            }
        }
    }

    /// <summary>
    /// Structure reference to for DCPower SSC members.
    /// </summary>
    public struct DCPowerSSC
    {
        /// <summary>
        /// NI-DCPower session field property.
        /// </summary>
        public NIDCPower Session { get; set; }
        /// <summary>
        /// DriverChannelList string property.
        /// </summary>
        public string DriverChannelList { get; set; }
        /// <summary>
        ///  Pin string property.
        /// </summary>
        public string Pin { get; set; }
        /// <summary>
        /// DriverChannelList string property.
        /// </summary>
        public string TSMChannelList { get; set; }
        /// <summary>
        /// Instrument model number property.
        /// </summary>
        public int InstrumentModel { get; set; }
        /// <summary>
        /// Pin index property.
        /// </summary>
        public int PinIndex { get; set; }
        /// <summary>
        /// Site number property.
        /// </summary>
        public int SiteNumber { get; set; }
        /// <summary>
        /// Site index property.
        /// </summary>
        public int SiteIndex { get; set; }
    }

    /// <summary>
    /// Class definition for DCPower Sessions Objects.
    /// </summary>
    public class DCPower
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public NIDCPowerMultiplePinMultipleSessionQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// DCPower SSC definition.
        /// </summary>
        public DCPowerSSC[] SSC { get; set; }
        /// <summary>
        ///  Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Site Numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }
        /// <summary>
        /// Last measurement record stamp.
        /// </summary>
        public double[] LastMeasureRecordDt { get; set; }

        /// <summary>
        /// Transitions the NI-DCPower session from the Running state to the Uncommitted state. If a sequence is running, then the NI-DCPower session is stopped. Any configuration methods called after this method are not applied until the InitiateUnqualified() method is called. If power output is enabled when you call the AbortUnqualified() method, the output channels remain in their current state and continue providing power. Use the EnabledPartiallyQualified property to disable power output on a per channel basis. Use the ResetPartiallyQualified() method to disable output on all channels.
        /// </summary>
        /// <remarks>
        /// For information about the specific NI-DCPower software states, refer to the Programming States topic in the NI DC Power Supplies and SMUs Help.
        /// </remarks>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.Session.Control.Abort());

        /// <summary>
        /// Starts generation or acquisition, causing the NI-DCPower session to leave the Uncommitted state or Committed state and enter the Running state. To return to the Uncommitted state call the NationalInstruments.ModularInstruments.NIDCPower.DCPowerControl.AbortUnqualified method.
        /// </summary>
        /// <remarks>
        /// For more information about the specific NI-DCPower software states, refer to the Programming States topic in the NI DC Power Supplies and SMUs Help.
        /// </remarks>
        public void Initiate() => Parallel.ForEach(SSC, ssc => ssc.Session.Control.Initiate());

        /// <summary>
        /// Applies the settings that you configured previously to the device. Calling this method moves the NI-DCPower session from the Uncommitted state into the Committed state. If you modify any property after you call this method, the NI-DCPower session reverts to the Uncommitted state. Use the NationalInstruments.ModularInstruments.NIDCPower.DCPowerControl.InitiateUnqualified method to transition to the Running state.
        /// </summary>
        /// <remarks>
        /// Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states
        /// </remarks>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.Session.Control.Commit());

        /// <summary>
        /// Resets the specified channel(s) to a known state. This method disables power generation, resets channel properties to their default values, commits the channel properties, and leaves the channel(s) in the Uncommitted state.
        /// </summary>
        /// <remarks>
        /// You can use this method to clear certain errors in less time than using the ResetDevice method. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific software states.
        /// </remarks>
        public void Reset() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.Reset());

        /// <summary>
        /// Resets all instruments in the session to a known state.
        /// </summary>
        /// <remarks>
        /// The method disables power generation, resets all properties for all instruments included in the session to their default values, clears errors such as overtemperature and unexpected loss of auxiliary power, commits the instrument properties, and leaves the instrument(s) in the Uncommitted state. This method also performs a hard reset on the instrument(s) and driver software. This method has the same functionality as using reset in Measurement and Automation Explorer. This method opens the output relay on instruments that have an output relay.
        /// </remarks>
        public void ResetDevice() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.ResetDevice());

        /// <summary>
        /// Configures different measurement settings for each DCPower instruments contained in the session.
        /// </summary>
        /// <param name="apertureTime">Defines aperture time value. Value must match specified aperture time units. Default value is 16.67ms</param>
        /// <param name="apertureTimeUnitsinSeconds">Defines aperture time Units (seconds or PLCs). By default value is set to seconds.</param>
        /// <param name="sourceDelayinSeconds">Source delay. Valid values range from 0 to 167. The default value is 0.</param>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Normal.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is false.</param>
        /// <remarks>
        /// Parameters: ApertureTime (units and value), Measurement delay and transitent response.
        /// </remarks>
        public void ConfigureSettings(double apertureTime = 16.666e-3,
                                        DCPowerMeasureApertureTimeUnits apertureTimeUnitsinSeconds = DCPowerMeasureApertureTimeUnits.Seconds,
                                        double sourceDelayinSeconds = 0.0,
                                        DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Normal,
                                        bool initiateSessionAfter = false)
        {
            double CalcApertureTimeinSeconds;
            switch (apertureTimeUnitsinSeconds)
            {
                case DCPowerMeasureApertureTimeUnits.PowerLineCycles:
                    CalcApertureTimeinSeconds = apertureTime / InstrCtrl.PowerLineFrequency;
                    break;
                case DCPowerMeasureApertureTimeUnits.Seconds:
                    CalcApertureTimeinSeconds = apertureTime;
                    break;
                default:
                    throw new Exception("Invalid Aperture Time Unit Value");
            }

            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();

                switch (ssc.InstrumentModel)
                {
                    // cases not implemented for 4130 and 4154, it is assumed that these models aren't used anymore
                    case 4110:
                        // http://zone.ni.com/reference/en-XX/help/370736U-01/ni_dc_power_supplies_help/supportedproperties_4110/
                        // The 4110 uses samples to average and has a fixed sample rate of 3kHz, convert this to a number of samples to average
                        var samplesToAverage = Convert.ToInt32(3000.0 * CalcApertureTimeinSeconds);
                        if (samplesToAverage < 1) samplesToAverage = 1;
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.SamplesToAverage = samplesToAverage;
                        ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayinSeconds);
                        // Transient response not avaialble on 4110
                        break;

                    case 4112:
                    case 4113:
                        // http://zone.ni.com/reference/en-XX/help/370736U-01/ni_dc_power_supplies_help/supportedproperties_4112_4113/
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTimeUnits = apertureTimeUnitsinSeconds;
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = apertureTime;
                        // Transient response not available on 4112 and 4113
                        ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayinSeconds);
                        break;

                    default:
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTimeUnits = apertureTimeUnitsinSeconds;
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = apertureTime;
                        ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = transientResponse;
                        ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayinSeconds);
                        break;
                }
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Configures and inititates transient response.
        /// </summary>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Normal.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is false.</param>
        public void ConfigureTransient(DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Normal,
                                        bool initiateSessionAfter = false)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();

                switch (ssc.InstrumentModel)
                {
                    case 4110:
                    case 4112:
                    case 4113:
                        // Transient response not avaialble on 4110
                        // Transient response not available on 4112 and 4113
                        break;
                    default:
                        ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = transientResponse;
                        break;
                }
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Configures the number of measurements that compose a measure record. If you set this property to a value greater than 1, the DCPowerMeasurementWhenUnQualified property must be set to  AutomaticallyAfterSourceCompletePartiallyQualified or OnMeasureTriggerPartiallyQualified.
        /// </summary>
        /// <param name="recordLength">Valid values range from 1 to 16,777,216. The default value is 1.</param>
        /// <remarks>
        /// <para>
        /// This property is not supported by all devices. For more information about supported devices, refer to the Supported Properties by Device topic in the NI DC Power Supplies and SMUs Help. To use the per-channel version of this property, you must first initialize the session with the NationalInstruments.ModularInstruments.NIDCPower.NIDCPower.#ctor(System.String,System.Boolean,System.String)PartiallyQualified constructor. If the session was initialized with a deprecated constructor, use the per-session version of this property instead or include all channels.
        /// </para>
        /// </remarks>
        public void ConfigureMeasureRecordLength(int recordLength = 1)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = recordLength);
        }

        /// <summary>
        /// Configures aperture time settings (units and value) for all DCPower instruments contained in the session.
        /// </summary>
        /// <param name="apertureTime">Defines aperture time value. Value must match specified aperture time units. Default value is 16.67ms</param>
        /// <param name="apertureTimeUnitsinSeconds">Defines aperture time Units (seconds or PLCs). By default value is set to seconds.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is false.</param>
        public void ConfigureAperture(double apertureTime = 16.666e-3,
                                        DCPowerMeasureApertureTimeUnits apertureTimeUnitsinSeconds = DCPowerMeasureApertureTimeUnits.Seconds,
                                        bool initiateSessionAfter = false)
        {
            double CalcApertureTimeinSeconds;
            switch (apertureTimeUnitsinSeconds)
            {
                case DCPowerMeasureApertureTimeUnits.PowerLineCycles:
                    CalcApertureTimeinSeconds = apertureTime / InstrCtrl.PowerLineFrequency;
                    break;
                case DCPowerMeasureApertureTimeUnits.Seconds:
                    CalcApertureTimeinSeconds = apertureTime;
                    break;
                default:
                    throw new Exception("Invalid Aperture Time Unit Value");
            }

            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();

                switch (ssc.InstrumentModel)
                {
                    case 4110:
                        // http://zone.ni.com/reference/en-XX/help/370736U-01/ni_dc_power_supplies_help/supportedproperties_4110/
                        // The 4110 uses samples to average and has a fixed sample rate of 3kHz, convert this to a number of samples to average
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.SamplesToAverage = Convert.ToInt32(3000.0 * CalcApertureTimeinSeconds);
                        break;
                    default:
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTimeUnits = apertureTimeUnitsinSeconds;
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = apertureTime;
                        break;
                }
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Configures either local or remote sensing of the output voltage for the specified channel(s) contained in the session. 
        /// </summary>
        /// <param name="sense">Valid values Local or Remote. Default value is set to Remote.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is false.</param>
        /// <remarks>
        /// Default value is set to Remote. Refer to the Local and Remote Sense topic in the NI DC Power Supplies and SMUs Help for more information about sensing voltage on supported channels.
        /// </remarks>
        public void ConfigureSense(DCPowerMeasurementSense sense = DCPowerMeasurementSense.Remote,
                                     bool initiateSessionAfter = false)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();
                switch (ssc.InstrumentModel)
                {
                    case 4110:
                    case 4112:
                    case 4113:
                        // These instruments don't support remote sense, skip setting the property if on these models
                        break;
                    default:
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.Sense = sense;
                        break;
                }
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Configures measurement trigger settings for the specified channel(s) contained in the session. Acquisition is started after configuration (initiate state).
        /// </summary>
        /// <param name="perSiteTriggerSource">List of trigger sources per site.</param>
        /// <param name="recordLength">Valid values range from 1 to 16,777,216. The default value is 1.</param>
        /// <param name="edge">Specifies whether to configure the Measure trigger to assert on the rising or falling edge.</param>
        /// <param name="apertureInSeconds">Aperture time value in seconds. Default value is -1 (don't configure). </param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is false.</param>
        /// <remarks>
        /// <para>
        /// Receives an string array with trigger source per site.
        /// </para>
        /// <para>
        /// Allows for configuration of the trigger edge (default value Rising Edge) and optional aperture time (seconds). By default does not configures aperture time unless specified.
        /// </para>
        /// </remarks>
        public void ConfigureMeasureTrigger(string[] perSiteTriggerSource,
                                            int recordLength = 1,
                                            DCPowerTriggerEdge edge = DCPowerTriggerEdge.Rising,
                                            double apertureInSeconds = -1, // doesn't configure aperture if default value of -1 is provided
                                            bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                var trig = DCPowerDigitalEdgeMeasureTriggerInputTerminal.FromString(perSiteTriggerSource[ssc.SiteIndex]);
                ssc.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                ssc.Session.Triggers.MeasureTrigger.DigitalEdge.Configure(trig, edge);
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = recordLength;
                if (apertureInSeconds != -1) ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = apertureInSeconds;
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Checks if for the defined channel(s) in the session, output function and expected levels match defined settings (OutputFunction = DCVoltage).
        /// </summary>
        /// <param name="incorrectSupplies">Supply names which did not pass the expected voltage check.</param>
        /// <param name="expectedVoltages">Voltage values to check.</param>
        /// <remarks>
        /// Optional para
        /// expected voltages can be defined (array of double values). Default OutputFunction for comparison is set to DCVoltage.
        /// </remarks>
        /// <returns>
        /// Boolean value stating compare operation result (false if voltage levels or mode are not the expected ones).
        /// </returns>
        public bool CheckDCVoltageModeandLevels(out string incorrectSupplies, double[] expectedVoltages = null)
        {
            incorrectSupplies = String.Empty;
            if (expectedVoltages is null) expectedVoltages = new double[SSC.Length];

            var inCorrectMode = new bool[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Control.Initiate();

                var inSinglePoint = ssc.Session.Source.Mode == DCPowerSourceMode.SinglePoint;
                var inDCVoltage = ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function == DCPowerSourceOutputFunction.DCVoltage;
                var atExpectedVoltage = ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel == expectedVoltages[ssc.PinIndex];

                inCorrectMode[index] = inSinglePoint & inDCVoltage & atExpectedVoltage;
            });

            bool atExpected = true;

            for (int i = 0; i < inCorrectMode.Length; i++)
            {
                atExpected &= inCorrectMode[i];
                if (!inCorrectMode[i] && i == 0) incorrectSupplies += SSC[i].Pin;
                else if (!inCorrectMode[i]) incorrectSupplies += String.Concat(", ", SSC[i].Pin);
            }

            return atExpected;
        }

        /// <summary>
        /// Configures transient response settings for DCPower source (voltage and current), based on the SMU model. Source is initiated after configuration.
        /// </summary>
        /// <param name="VGBWMult">Voltage Gain Bandwidth multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="VCFreqMult">Voltage Compensation Frequency multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="VPZMult">Voltage Pole-Zero ratio multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="CGBWMult">Current Gain Bandwidth multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="CCFreqMult">Current Compensation Frequency multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="CPZMult">Current Pole-Zero Ratio multiplier. Default value is 1.0 (no Scaling).</param>
        /// <param name="model">Instrument Model Number.</param>
        /// <remarks>
        /// <para>
        /// If an invalid model for SMU passed, no configuration will ocurr.
        /// </para>
        /// <para>
        /// Optional input parameters allows for scaling of the GainBandwidth, CompensationFrequency and PoleZeroRatio for both voltage and current transient response. Default value for settings is set to No scaling.
        /// </para>
        /// </remarks>
        public void ConfigureCustomTransient(double VGBWMult = 1.0,
                                             double VCFreqMult = 1.0,
                                             double VPZMult = 1.0,
                                             double CGBWMult = 1.0,
                                             double CCFreqMult = 1.0,
                                             double CPZMult = 1.0,
                                             int model = 4147)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                if (ssc.InstrumentModel == model)
                {
                    ssc.Session.Control.Abort();
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = DCPowerSourceTransientResponse.Fast;
                    ssc.Session.Control.Commit();
                    var VGBW = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.GainBandwidth;
                    var VCF = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.CompensationFrequency;
                    var VPZ = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.PoleZeroRatio;
                    var CGBW = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.GainBandwidth;
                    var CCF = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.CompensationFrequency;
                    var CPZ = ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.PoleZeroRatio;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = DCPowerSourceTransientResponse.Custom;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.GainBandwidth = VGBW * VGBWMult;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.CompensationFrequency = VCF * VCFreqMult;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Voltage.PoleZeroRatio = VPZ * VPZMult;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.GainBandwidth = CGBW * CGBWMult;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.CompensationFrequency = CCF * CCFreqMult;
                    ssc.Session.Outputs[ssc.DriverChannelList].Source.CustomTransientResponse.Current.PoleZeroRatio = CPZ * CPZMult;
                    ssc.Session.Control.Initiate();
                }
            });
        }

        /// <summary>
        /// Forces the same voltage sequence in all sessions by synchronizing all slave sessions (indexes after 0) to master session (index 0).
        /// </summary>
        /// <param name="pinVoltageSequencePerSite">Per pin voltage sequence array, provided in a per-site basis</param>
        /// <param name="currentLimits">Current limit values per pin.</param>
        /// <param name="currentRanges">Current limit range values per pin.</param>
        /// <param name="SequenceLoopCount">Loop count. Default is 1 run (no looping).</param>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Fast.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for sequence event completion. Default value is set to 5 seconds.</param>
        /// <param name="sequenceStepSourceDelayinSeconds">Delay time in seconds betwen each loop. Default value is set to 45us.</param>
        /// <remarks>
        /// Voltage sequence can be set per site.
        /// </remarks>
        public void ForceVoltageSequenceSynchronized(
                            Dictionary<string, double[][]> pinVoltageSequencePerSite,
                            double[] currentLimits,
                            double[] currentRanges,
                            int SequenceLoopCount = 1,
                            DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Fast,
                            double sequenceTimeoutInSeconds = 5.0,
                            double sequenceStepSourceDelayinSeconds = 45e-6)
        {
            var initialSourceDelay = new PrecisionTimeSpan[SSC.Length];
            var initialMeasureWhen = new DCPowerMeasurementWhen[SSC.Length];
            var initialStartTrigger = new DCPowerDigitalEdgeStartTriggerInputTerminal[SSC.Length];
            var initialStartTriggerType = new DCPowerStartTriggerType[SSC.Length];

            // Build trigger terminals from channel at index 0 (used as master session)
            var startTrigger = DCPowerDigitalEdgeStartTriggerInputTerminal.FromString(String.Concat("/", SSC[0].Session.DriverOperation.IOResourceDescriptor, "/Engine", SSC[0].DriverChannelList, "/StartTrigger"));

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                var voltageLevelRange = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Select(x => Math.Abs(x)).Max();

                // Init single point source mode to the first value in the sequence, initiate the session to apply these ranges
                ForceVoltage(ssc, pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].First(), currentLimits[ssc.PinIndex], voltageLevelRange, currentRanges[ssc.PinIndex], initiateSessionAfter: true);

                // Abort and set voltage of the single point source mode to the last value in the sequence (used at the end)
                ssc.Session.Control.Abort();
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Last();

                // Store initial setting of measure when
                initialMeasureWhen[index] = ssc.Session.Measurement.Configuration.MeasureWhen;

                ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = transientResponse;

                // Save source delay so we can restore it at the end
                initialSourceDelay[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);

                var sequenceLength = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Length;

                var sourceDelay = PrecisionTimeSpan.FromSeconds(sequenceStepSourceDelayinSeconds);

                var sourceDelays = new PrecisionTimeSpan[sequenceLength];
                for (int i = 0; i < sequenceLength; i++) sourceDelays[i] = sourceDelay;

                // Set channel into sequence mode and apply the waveform
                SetSequenceWithSourceDelay(ssc, pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex], sourceDelays, SequenceLoopCount);

                // Make sure measure when isn't set to Automatically After Source Complete
                if (ssc.Session.Measurement.Configuration.MeasureWhen == DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)
                {
                    ssc.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                }

                if (index == 0)
                {
                    // Use first session as the master trigger and measure trigger
                    ssc.Session.Triggers.StartTrigger.Disable();
                    // commit, but do not start master session
                    ssc.Session.Control.Commit();
                }
                else
                {
                    // Synchronize slave sessions (indexes after 0) to master session (index 0)
                    // Set all other sessions to trigger off of the first sessions start trigger
                    initialStartTrigger[index] = ssc.Session.Triggers.StartTrigger.DigitalEdge.InputTerminal;
                    initialStartTriggerType[index] = ssc.Session.Triggers.StartTrigger.Type;
                    if (initialStartTrigger[index] != startTrigger)
                    {
                        ssc.Session.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    }
                    else if (initialStartTriggerType[index] != DCPowerStartTriggerType.DigitalEdge)
                    {
                        ssc.Session.Triggers.StartTrigger.Type = DCPowerStartTriggerType.DigitalEdge;
                    }
                    // initiate slave sessions
                    ssc.Session.Control.Initiate();
                }
            });

            // Initiate first session (other sessions are already started and waiting)
            SSC[0].Session.Control.Initiate();

            WaitForSequenceEngineDone(sequenceTimeoutInSeconds);

            // Reconfigure all sessions to single point mode and re-apply default trigger settings
            // Voltage levels and limits are already configured for the last sequence step in single point mode earlier
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                if (index > 0) ssc.Session.Triggers.StartTrigger.Disable();
                if (initialMeasureWhen[index] == DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete)
                {
                    ssc.Session.Measurement.Configuration.MeasureWhen = initialMeasureWhen[index];
                }
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = initialSourceDelay[index];
                ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
                ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Forces the same voltage sequence in all sessions by synchronizing all slave sessions (indexes after 0) to master session (index 0).
        /// </summary>
        /// <param name="pinVoltageSequence">Per pin voltage sequence array</param>
        /// <param name="currentLimits">Current limit values per pin.</param>
        /// <param name="currentRanges">Current limit range values per pin.</param>
        /// <param name="SequenceLoopCount">Loop count. Default is 1 run (no looping).</param>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Fast.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for sequence event completion. Default value is set to 5 seconds.</param>
        /// <param name="sequenceStepSourceDelayinSeconds">Delay time in seconds betwen each loop. Default value is set to 45us.</param>
        /// <remarks>
        /// Same sequence is applied to all sites.
        /// </remarks>
        public void ForceVoltageSequenceSynchronized(
                            Dictionary<string, double[]> pinVoltageSequence,
                            double[] currentLimits,
                            double[] currentRanges,
                            int SequenceLoopCount = 1,
                            DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Fast,
                            double sequenceTimeoutInSeconds = 5.0,
                            double sequenceStepSourceDelayinSeconds = 45e-6)
        {
            var pinVoltageSequencePerSite = new Dictionary<string, double[][]>();
            foreach (var pin in pinVoltageSequence.Keys)
            {
                var values = new double[SiteNumbers.Length][];
                for (int siteNdx = 0; siteNdx < SiteNumbers.Length; siteNdx++) values[siteNdx] = pinVoltageSequence[pin];

                pinVoltageSequencePerSite.Add(pin, values);
            }

            ForceVoltageSequenceSynchronized(pinVoltageSequencePerSite, currentLimits, currentRanges, SequenceLoopCount, transientResponse, sequenceTimeoutInSeconds, sequenceStepSourceDelayinSeconds);
        }

        /// <summary>
        /// Forces the same voltage sequence in all sessions by synchronizing all slave sessions (indexes after 0) to master session (index 0) and return the measurements performed.
        /// </summary>
        /// <param name="pinVoltageSequencePerSite">Per pin voltage sequence array, provided in a per-site basis</param>
        /// <param name="currentLimits">Current limit values per pin.</param>
        /// <param name="currentRanges">Current limit range values per pin.</param>
        /// <param name="SequenceLoopCount">Loop count. Default is 1 run (no looping).</param>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Fast.</param>
        /// <param name="requestedMeasureApertureTimeinSeconds">Requested measurement aperture time value specified in seconds.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for sequence event completion. Default value is set to 5 seconds.</param>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <param name="sequenceStepSourceDelayinSeconds">Delay time in seconds betwen each loop. Default value is set to 45us.</param>
        /// <returns>
        /// An array of DCPowerFetchResults data.
        /// </returns>
        /// <remarks>
        /// All other sessions are set to trigger off of the first sessions start trigger.
        /// </remarks>
        public DCPowerFetchResult[] ForceVoltageSequenceSynchronizedWithMeasures(
                            Dictionary<string, double[][]> pinVoltageSequencePerSite,
                            double[] currentLimits,
                            double[] currentRanges,
                            int SequenceLoopCount = 1,
                            DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Fast,
                            double requestedMeasureApertureTimeinSeconds = 0,
                            double measureTimeinSeconds = 0.0,
                            double sequenceTimeoutInSeconds = 5.0,
                            double sequenceStepSourceDelayinSeconds = 45e-6)
        {
            var measureRecordDt = new double[SSC.Length];
            var measureRecordLength = new int[SSC.Length];
            var initialAperture = new double[SSC.Length];
            var initialSourceDelay = new PrecisionTimeSpan[SSC.Length];
            var initialMeasureWhen = new DCPowerMeasurementWhen[SSC.Length];
            var initialMeasureTrigger = new DCPowerDigitalEdgeMeasureTriggerInputTerminal[SSC.Length];
            var initialMeasureTriggerType = new DCPowerMeasureTriggerType[SSC.Length];
            var initialStartTrigger = new DCPowerDigitalEdgeStartTriggerInputTerminal[SSC.Length];
            var initialStartTriggerType = new DCPowerStartTriggerType[SSC.Length];

            var MeasureEnabled = measureTimeinSeconds > 0.0;

            // This is used to slightly increase the length of the measure buffer to capture some points after the last sequence step
            // var extraMeasureTime = 50e-6;
            // This is an optional paramemter

            // Build trigger terminals from channel at index 0 (used as master session)
            var startTrigger = DCPowerDigitalEdgeStartTriggerInputTerminal.FromString(String.Concat("/", SSC[0].Session.DriverOperation.IOResourceDescriptor, "/Engine", SSC[0].DriverChannelList, "/StartTrigger"));
            var measureTrigger = DCPowerDigitalEdgeMeasureTriggerInputTerminal.FromString(String.Concat("/", SSC[0].Session.DriverOperation.IOResourceDescriptor, "/Engine", SSC[0].DriverChannelList, "/SourceCompleteEvent"));

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                var voltageLevelRange = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Select(x => Math.Abs(x)).Max();

                // Init single point source mode to the first value in the sequence, initiate the session to apply these ranges
                ForceVoltage(ssc, pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].First(), currentLimits[ssc.PinIndex], voltageLevelRange, currentRanges[ssc.PinIndex], initiateSessionAfter: true);

                // Abort and set voltage of the single point source mode to the last value in the sequence (used at the end)
                ssc.Session.Control.Abort();
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Last();

                // Configure waveform measurement mode, temp record length of 2 (416x workaround)
                initialMeasureWhen[index] = ssc.Session.Measurement.Configuration.MeasureWhen;
                ssc.Session.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;

                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 2;

                // Save aperture time so we can restore it at the end
                initialAperture[index] = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = requestedMeasureApertureTimeinSeconds;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.TransientResponse = transientResponse;

                // Save source delay so we can restore it at the end
                initialSourceDelay[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);

                var sequenceLength = pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex].Length;

                var sourceDelay = PrecisionTimeSpan.FromSeconds(sequenceStepSourceDelayinSeconds);

                var sourceDelays = new PrecisionTimeSpan[sequenceLength];
                for (int i = 0; i < sequenceLength; i++) sourceDelays[i] = sourceDelay;

                // Set channel into sequence mode and apply the waveform
                SetSequenceWithSourceDelay(ssc, pinVoltageSequencePerSite[ssc.Pin][ssc.SiteIndex], sourceDelays, SequenceLoopCount);

                if (index == 0)
                {
                    // Use first session as the master trigger and measure trigger
                    ssc.Session.Triggers.StartTrigger.Disable();
                }
                else
                {
                    // Synchronize slave sessions (indexes after 0) to master session (index 0)
                    // Set all other sessions to trigger off of the first sessions start trigger
                    initialStartTrigger[index] = ssc.Session.Triggers.StartTrigger.DigitalEdge.InputTerminal;
                    initialStartTriggerType[index] = ssc.Session.Triggers.StartTrigger.Type;
                    if (initialStartTrigger[index] != startTrigger)
                    {
                        ssc.Session.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    }
                    else if (initialStartTriggerType[index] != DCPowerStartTriggerType.DigitalEdge)
                    {
                        ssc.Session.Triggers.StartTrigger.Type = DCPowerStartTriggerType.DigitalEdge;
                    }
                }

                // Configure all sessions to start waveform measures on the same trigger from master session
                if (MeasureEnabled)
                {
                    ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = requestedMeasureApertureTimeinSeconds;
                    initialMeasureTrigger[index] = ssc.Session.Triggers.MeasureTrigger.DigitalEdge.InputTerminal;
                    initialMeasureTriggerType[index] = ssc.Session.Triggers.MeasureTrigger.Type;

                    if (initialMeasureTrigger[index] != measureTrigger)
                    {
                        ssc.Session.Triggers.MeasureTrigger.DigitalEdge.Configure(measureTrigger, DCPowerTriggerEdge.Rising);
                    }
                    else if (initialMeasureTriggerType[index] != DCPowerMeasureTriggerType.DigitalEdge)
                    {
                        ssc.Session.Triggers.MeasureTrigger.Type = DCPowerMeasureTriggerType.DigitalEdge;
                    }

                    // Read back actual measure record dT, calculate buffer sizes, configure measure record length
                    ssc.Session.Control.Commit();
                    measureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
                    measureRecordLength[index] = (int)Math.Ceiling((measureTimeinSeconds) / measureRecordDt[index]);
                    ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = measureRecordLength[index];

                    // Increase buffer size for measures if they aren't big enough
                    if (ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize < measureRecordLength[index])
                    {
                        ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize = measureRecordLength[index];
                    }
                }

                ssc.Session.Control.Commit();
            });

            // Initiate Sessions in reverse order (first session is master trigger, and has to be started last)
            for (int i = SSC.Length - 1; i >= 0; i--)
            {
                SSC[i].Session.Control.Initiate();
            }

            WaitForSequenceEngineDone(sequenceTimeoutInSeconds);

            DCPowerFetchResult[] measurements = null;

            if (MeasureEnabled)
            {
                measurements = Fetch(measureRecordLength, 1);
            }

            // Reconfigure all sessions to single point mode and re-apply default trigger settings
            // Voltage levels and limits are already configured for the last sequence step in single point mode earlier
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Triggers.StartTrigger.Disable();
                ssc.Session.Measurement.Configuration.MeasureWhen = initialMeasureWhen[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 1;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = initialAperture[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = initialSourceDelay[index];
                ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
                ssc.Session.Control.Initiate();
            });

            LastMeasureRecordDt = measureRecordDt;

            return measurements;
        }

        /// <summary>
        /// Gets the amount of time between the starts of two consecutive measurements in a measure record. Use this property only after you commit the desired measurement settings.
        /// </summary>
        /// <returns>
        /// Returns the amount of time between the starts of two consecutive measurements in a measure record.
        /// </returns>
        /// <remarks>
        /// This property is not available when NationalInstruments.ModularInstruments.NIDCPower.DCPowerMeasurementAutoZeroUnqualified is configured to NationalInstruments.ModularInstruments.NIDCPower.DCPowerMeasurementAutoZero.OncePartiallyQualified, because the amount of time between the first two measurements and the rest would differ. This property is not supported by all devices. For information about supported devices, refer to Supported Properties by Device in the NI DC Power Supplies and SMUs Help.
        /// </remarks>
        public double[] GetMeasureRecordDeltaTime()
        {
            var measureRecordDt = new double[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                measureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
            });

            return measureRecordDt;
        }

        /// <summary>
        /// Acquires measured waveforms from synchronized pattern burst in digital pins.
        /// </summary>
        /// <param name="digitalPins">Digital Pin Sessions object.</param>
        /// <param name="startScript">Start Label of pattern nurst.</param>
        /// <param name="requestedMeasureApertureTimeinSeconds">Requested measurement aperture time value specified in seconds.</param>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <returns>
        /// An array of DCPowerFetchResult measurements.
        /// </returns>
        public DCPowerFetchResult[] AcquireSynchronizedWaveformsPatternStart(
                            Digital digitalPins,
                            string startScript,
                            double requestedMeasureApertureTimeinSeconds = 0,
                            double measureTimeinSeconds = 0.0)
        {
            LastMeasureRecordDt = new double[SSC.Length];
            var measureRecordLength = new int[SSC.Length];
            var initialAperture = new double[SSC.Length];
            var initialSourceDelay = new PrecisionTimeSpan[SSC.Length];
            var initialMeasureWhen = new DCPowerMeasurementWhen[SSC.Length];
            var initialMeasureTrigger = new DCPowerDigitalEdgeMeasureTriggerInputTerminal[SSC.Length];
            var initialMeasureTriggerType = new DCPowerMeasureTriggerType[SSC.Length];

            var startTrigger = digitalPins.GetPerSiteStartTriggerTerminals();

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();

                // Configure waveform measurement mode, temp record length of 2 (416x workaround)
                initialMeasureWhen[index] = ssc.Session.Measurement.Configuration.MeasureWhen;
                initialMeasureTrigger[index] = ssc.Session.Triggers.MeasureTrigger.DigitalEdge.InputTerminal;
                initialMeasureTriggerType[index] = ssc.Session.Triggers.MeasureTrigger.Type;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 2;

                // Save aperture time so we can restore it at the end
                initialAperture[index] = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = requestedMeasureApertureTimeinSeconds;

                // Save source delay so we can restore it at the end
                initialSourceDelay[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);

                // Set SMU sessions to start measuring on digital pattern start trigger
                ssc.Session.Triggers.MeasureTrigger.DigitalEdge.Configure(startTrigger[ssc.SiteIndex], DCPowerTriggerEdge.Rising);

                // Read back actual measure record dT, calculate buffer sizes, configure measure record length
                ssc.Session.Control.Commit();

                LastMeasureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
                measureRecordLength[index] = (int)Math.Ceiling((measureTimeinSeconds) / LastMeasureRecordDt[index]);
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = measureRecordLength[index];

                // Increase buffer size for measures if they aren't big enough
                if (ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize < measureRecordLength[index])
                {
                    ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize = measureRecordLength[index];
                }

                ssc.Session.Control.Initiate();
            });

            digitalPins.BurstPattern(startScript, waitUntilDone: false);

            var measurements = Fetch(measureRecordLength, measureTimeinSeconds + 1);

            // Reconfigure all sessions to single point mode and re-apply default trigger settings
            // Voltage levels and limits are already configured for the last sequence step in single point mode earlier
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Measurement.Configuration.MeasureWhen = initialMeasureWhen[index];
                ssc.Session.Triggers.MeasureTrigger.DigitalEdge.InputTerminal = initialMeasureTrigger[index];
                ssc.Session.Triggers.MeasureTrigger.Type = initialMeasureTriggerType[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 1;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = initialAperture[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = initialSourceDelay[index];
                ssc.Session.Control.Initiate();
            });

            return measurements;
        }

        /// <summary>
        /// Configures Synchronized Waveform Acquisition parameters.
        /// </summary>
        /// <param name="perSiteStartTrigger">Specifies the input terminals of the digital edges for the Measure trigger.</param>
        /// <param name="requestedMeasureApertureTimeinSeconds">Requested measurement aperture time value specified in seconds.</param>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <remarks>
        /// Properties to configure:<br/>
        /// • perSiteStartTrigger<br/>
        /// • requestedMeasureApertureTimeinSeconds<br/>
        /// • measureTimeinSeconds
        /// </remarks>
        public void ConfigureSynchronizedWaveformAcquisition(
                            string[] perSiteStartTrigger,
                            double requestedMeasureApertureTimeinSeconds = 0,
                            double measureTimeinSeconds = 0.0)
        {
            LastMeasureRecordDt = new double[SSC.Length];
            var measureRecordLength = new int[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();

                // Configure waveform measurement mode, temp record length of 2 (416x workaround)
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 2;

                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = requestedMeasureApertureTimeinSeconds;

                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);

                // Set SMU sessions to start measuring on digital pattern start trigger
                ssc.Session.Triggers.MeasureTrigger.DigitalEdge.Configure(perSiteStartTrigger[ssc.SiteIndex], DCPowerTriggerEdge.Rising);

                // Read back actual measure record dT, calculate buffer sizes, configure measure record length
                ssc.Session.Control.Commit();

                LastMeasureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
                measureRecordLength[index] = (int)Math.Ceiling((measureTimeinSeconds) / LastMeasureRecordDt[index]);
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = measureRecordLength[index];

                // Increase buffer size for measures if they aren't big enough
                if (ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize < measureRecordLength[index])
                {
                    ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize = measureRecordLength[index];
                }

                ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Fetches a struct of various data created from measurements.
        /// </summary>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <remarks>
        /// Waveform data is obtained in a synchronous way.
        /// </remarks>
        /// <returns>
        /// An array of DCPowerFetchResult structures which represents the result of DCPowwerMeasurement.Fetch(string, PrecisionTimeSpan, int) Partially Qualified method.
        /// </returns>
        public DCPowerFetchResult[] FetchSynchronizedWaveforms(
                            double measureTimeinSeconds = -1.0)
        {
            var measureRecordLength = new int[SSC.Length];
            var lastMeasureRecordDt = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                lastMeasureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
                if (measureTimeinSeconds > 0.0) measureRecordLength[index] = (int)Math.Ceiling((measureTimeinSeconds) / lastMeasureRecordDt[index]);
                else measureRecordLength[index] = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength;
            });

            LastMeasureRecordDt = lastMeasureRecordDt;

            var measurements = Fetch(measureRecordLength, lastMeasureRecordDt[0] * measureRecordLength[0] + 1);

            return measurements;
        }

        /// <summary>
        /// Creates a TDMS file to store the waveforms data from DCPowerFetchResult objects into the specified file path. 
        /// </summary>
        /// <param name="waveforms">Waveform data to save. Data comes as a DCPowerFetchResult object array.</param>
        /// <param name="filePath">Absolute path to file where data is save.</param>
        /// <param name="channelGroupName">Header for TDSM file, if no value is provided automatic header will be appended. Default value is null.</param>
        /// <remarks>
        /// If channelGroupName variable is not specified, a new instance channel group is created with the name "Waveform Measurements [MM/dd/yyyy hh:mm:ss.fff]".
        /// </remarks>
        public void SaveWaveformsToTDMS(DCPowerFetchResult[] waveforms, string filePath, string channelGroupName = null)
        {
            var tdmsFileOptions = new TdmsFileOptions(TdmsFileFormat.Version20, TdmsFileAccess.ReadWrite, false, false, TdmsByteOrder.LittleEndian);
            var tdmsFile = new TdmsFile(filePath, tdmsFileOptions);

            string usedChannelGroupName = channelGroupName ?? "Waveform Measurements " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff");

            var channelGroup = new TdmsChannelGroup(usedChannelGroupName);
            tdmsFile.AddChannelGroup(channelGroup);

            for (int i = 0; i < SSC.Length; i++)
            {
                var voltageChannelName = SSC[i].TSMChannelList + " (V)";

                var voltageMeasurements = new TdmsChannel(voltageChannelName, TdmsDataType.Double);
                channelGroup.AddChannel(voltageMeasurements);
                voltageMeasurements.AppendData(waveforms[i].VoltageMeasurements);

                voltageMeasurements.AddProperty("wf_xname", TdmsPropertyDataType.String, voltageChannelName);
                voltageMeasurements.AddProperty("wf_xunit_string", TdmsPropertyDataType.String, "s");
                voltageMeasurements.AddProperty("wf_start_offset", TdmsPropertyDataType.Int32, 0);
                voltageMeasurements.AddProperty("wf_increment", TdmsPropertyDataType.Double, LastMeasureRecordDt[i]);

                var currentChannelName = SSC[i].TSMChannelList + " (A)";

                var currentMeasurements = new TdmsChannel(currentChannelName, TdmsDataType.Double);
                channelGroup.AddChannel(currentMeasurements);
                currentMeasurements.AppendData(waveforms[i].CurrentMeasurements);
                currentMeasurements.AddProperty("wf_xname", TdmsPropertyDataType.String, currentChannelName);
                currentMeasurements.AddProperty("wf_xunit_string", TdmsPropertyDataType.String, "s");
                currentMeasurements.AddProperty("wf_start_offset", TdmsPropertyDataType.Int32, 0);
                currentMeasurements.AddProperty("wf_increment", TdmsPropertyDataType.Double, LastMeasureRecordDt[i]);
            }

            tdmsFile.Save();
            tdmsFile.Close();
        }

        /// <summary>
        /// Acquires the syncrhonized waveforms previously run.
        /// </summary>
        /// <param name="requestedMeasureApertureTimeinSeconds">Requested measurement aperture time value specified in seconds.</param>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <returns>
        /// A DCPowerFetchResult array of measurements.
        /// </returns>
        public DCPowerFetchResult[] AcquireSynchronizedWaveforms(
                            double requestedMeasureApertureTimeinSeconds = 0,
                            double measureTimeinSeconds = 0.0)
        {
            var measureRecordDt = new double[SSC.Length];
            var measureRecordLength = new int[SSC.Length];
            var initialAperture = new double[SSC.Length];
            var initialSourceDelay = new PrecisionTimeSpan[SSC.Length];
            var initialMeasureWhen = new DCPowerMeasurementWhen[SSC.Length];
            var initialMeasureTrigger = new DCPowerDigitalEdgeMeasureTriggerInputTerminal[SSC.Length];
            var initialMeasureTriggerType = new DCPowerMeasureTriggerType[SSC.Length];

            // This is used to slightly increase the length of the measure buffer to capture some points after the last sequence step
            // var extraMeasureTime = 50e-6;
            // This is an optional paramemter

            // Build trigger terminals from channel at index 0 (used as master session)
            var measureTrigger = DCPowerDigitalEdgeMeasureTriggerInputTerminal.FromString(String.Concat("/", SSC[0].Session.DriverOperation.IOResourceDescriptor, "/Engine", SSC[0].DriverChannelList, "/MeasureTrigger"));

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();

                // Configure waveform measurement mode, temp record length of 2 (416x workaround)
                initialMeasureWhen[index] = ssc.Session.Measurement.Configuration.MeasureWhen;
                initialMeasureTrigger[index] = ssc.Session.Triggers.MeasureTrigger.DigitalEdge.InputTerminal;
                initialMeasureTriggerType[index] = ssc.Session.Triggers.MeasureTrigger.Type;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 2;

                // Save aperture time so we can restore it at the end
                initialAperture[index] = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = requestedMeasureApertureTimeinSeconds;

                // Save source delay so we can restore it at the end
                initialSourceDelay[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(0);

                if (index == 0)
                {
                    // Use first session as the software measure trigger
                    ssc.Session.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger();
                }
                else
                {
                    // Synchronize slave sessions (indexes after 0) to master session (index 0)
                    // Set all other sessions to trigger off of the first sessions measure trigger
                    ssc.Session.Triggers.MeasureTrigger.DigitalEdge.Configure(measureTrigger, DCPowerTriggerEdge.Rising);
                }

                // Read back actual measure record dT, calculate buffer sizes, configure measure record length
                ssc.Session.Control.Commit();

                measureRecordDt[index] = ssc.Session.Measurement.Configuration.RecordDeltaTime;
                measureRecordLength[index] = (int)Math.Ceiling((measureTimeinSeconds) / measureRecordDt[index]);
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = measureRecordLength[index];

                // Increase buffer size for measures if they aren't big enough
                if (ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize < measureRecordLength[index])
                {
                    ssc.Session.Outputs[ssc.DriverChannelList].Measurement.BufferSize = measureRecordLength[index];
                }

                ssc.Session.Control.Commit();
            });

            // Initiate Sessions in reverse order (first session is master trigger, and has to be started last)
            for (int i = SSC.Length - 1; i >= 0; i--)
            {
                SSC[i].Session.Control.Initiate();
            }

            SSC[0].Session.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();

            var measurements = Fetch(measureRecordLength, measureTimeinSeconds + 1);

            // Reconfigure all sessions to single point mode and re-apply default trigger settings
            // Voltage levels and limits are already configured for the last sequence step in single point mode earlier
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Measurement.Configuration.MeasureWhen = initialMeasureWhen[index];
                ssc.Session.Triggers.MeasureTrigger.DigitalEdge.InputTerminal = initialMeasureTrigger[index];
                ssc.Session.Triggers.MeasureTrigger.Type = initialMeasureTriggerType[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.RecordLength = 1;
                ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime = initialAperture[index];
                ssc.Session.Outputs[ssc.DriverChannelList].Source.SourceDelay = initialSourceDelay[index];
                ssc.Session.Control.Initiate();
            });

            LastMeasureRecordDt = measureRecordDt;

            return measurements;
        }

        /// <summary>
        /// Forces the same voltage sequence in all sessions by synchronizing all slave sessions (indexes after 0) to master session (index 0) and return the measurements performed.
        /// </summary>
        /// <param name="pinVoltageSequence">Per pin voltage sequence array.</param>
        /// <param name="currentLimits">Current limit values per pin.</param>
        /// <param name="currentRanges">Current limit range values per pin.</param>
        /// <param name="SequenceLoopCount">Loop count. Default is 1 run (no looping).</param>
        /// <param name="transientResponse">Defines the transient response between Custom, Slow, Normal and Fast. Default value is set to Fast.</param>
        /// <param name="requestedMeasureApertureTimeinSeconds">Requested measurement aperture time value specified in seconds.</param>
        /// <param name="measureTimeinSeconds">Desired measurement time in seconds.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for sequence event completion. Default value is set to 5 seconds.</param>
        /// <returns>
        /// An array of DCPowerFetchResults data.
        /// </returns>
        /// <remarks>
        /// No source delay is indluded in sequence step, all other sessions are set to trigger off of the first sessions start trigger.
        /// </remarks>
        public DCPowerFetchResult[] ForceVoltageSequenceSynchronizedWithMeasures(
                            Dictionary<string, double[]> pinVoltageSequence,
                            double[] currentLimits,
                            double[] currentRanges,
                            int SequenceLoopCount = 1,
                            DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Fast,
                            double requestedMeasureApertureTimeinSeconds = 0,
                            double measureTimeinSeconds = 0.0,
                            double sequenceTimeoutInSeconds = 5.0)
        {
            var pinVoltageSequencePerSite = new Dictionary<string, double[][]>();
            foreach (var pin in pinVoltageSequence.Keys)
            {
                var values = new double[SiteNumbers.Length][];
                for (int siteNdx = 0; siteNdx < SiteNumbers.Length; siteNdx++) values[siteNdx] = pinVoltageSequence[pin];

                pinVoltageSequencePerSite.Add(pin, values);
            }

            return ForceVoltageSequenceSynchronizedWithMeasures(pinVoltageSequencePerSite, currentLimits, currentRanges, SequenceLoopCount, transientResponse, requestedMeasureApertureTimeinSeconds, measureTimeinSeconds, sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Configures a series of voltage or current outputs for sequential sourcing and sets the amount of time, in seconds, between the start of two consecutive steps in a sequence.
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="Sequence">Sequence values to execute.</param>
        /// <param name="SequenceStepDeltainSeconds">Amount of time, in seconds, between the start of two consecutive steps in a sequence.</param>
        /// <param name="SequenceLoopCount">Number of times loop is repeated.</param>
        /// <remarks>
        /// <para>
        /// Series of voltage levels or current levels are defined using the Sequence input variable (valid values for this parameter are defined by the voltage level range or current level range).
        /// </para>
        /// <para>
        /// SequenceLoopCount parameter sets the number of times the sequences is run after initiation.
        /// </para>
        /// <para>
        /// Default value for SequenceStepDeltainSeconds The default value is 50 ms. This property does not apply to the last step of the last iteration of a sequence. Refer to the Sequence Step Delta Time topic in the NI DC Power Supplies and SMUs Help for more information.
        /// </para>
        /// </remarks>
        private static void SetSequenceWithDelta(DCPowerSSC ssc, double[] Sequence, double SequenceStepDeltainSeconds, int SequenceLoopCount)
        {
            SetSequence(ssc, Sequence, SequenceLoopCount);
            ssc.Session.Outputs[ssc.DriverChannelList].Source.SequenceStepDeltaTimeEnabled = true;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(SequenceStepDeltainSeconds);
        }

        /// <summary>
        /// Configures a series of voltage or current outputs for sequential sourcing.
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="Sequence">Sequence values to execute.</param>
        /// <param name="SequenceLoopCount">Number of times loop is repeated.</param>
        /// <remarks>
        /// <para>
        /// Series of voltage levels or current levels are defined using the Sequence input variable (valid values for this parameter are defined by the voltage level range or current level range).
        /// </para>
        /// <para>
        /// SequenceLoopCount parameter sets the number of times the sequences is run after initiation.
        /// </para>
        /// </remarks>
        private static void SetSequence(DCPowerSSC ssc, double[] Sequence, int SequenceLoopCount)
        {
            ssc.Session.Source.Mode = DCPowerSourceMode.Sequence;
            ssc.Session.Source.SequenceLoopCount = SequenceLoopCount;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.SetSequence(Sequence);
        }

        /// <summary>
        /// Configures a series of voltage or current outputs for sequential sourcing and the source delay for each iteration.
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="Sequence">Sequence values to execute.</param>
        /// <param name="sourceDelays">Source Delay in seconds between each sequence.</param>
        /// <param name="SequenceLoopCount">Number of times loop is repeated.</param>
        /// <remarks>
        /// <para>
        /// Series of voltage levels or current levels are defined using the Sequence input variable (valid values for this parameter are defined by the voltage level range or current level range).
        /// </para>
        /// <para>
        /// User can specify the source delay, in seconds, that follows the configuration of each value in the sequence (valid values must be in the range of [0,167]).
        /// </para>
        /// <para>
        /// SequenceLoopCount parameter sets the number of times the sequences is run after initiation.
        /// </para>
        /// </remarks>  
        private static void SetSequenceWithSourceDelay(DCPowerSSC ssc, double[] Sequence, PrecisionTimeSpan[] sourceDelays, int SequenceLoopCount)
        {
            ssc.Session.Source.Mode = DCPowerSourceMode.Sequence;
            ssc.Session.Source.SequenceLoopCount = SequenceLoopCount;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.SetSequence(Sequence, sourceDelays);
        }

        /// <summary>
        /// Sets the same voltage level (in volts) and current limit (in amperes) for the specified channel(s) that attempt to generate within the session. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify voltage and current level range.
        /// </para>
        /// </remarks>
        public void ForceVoltage(double voltageLevel,
                                   double currentLimit,
                                   double? voltageLevelRange = null,
                                   double? currentLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceVoltage(ssc, voltageLevel, currentLimit, voltageLevelRange, currentLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual voltage levels (defined in array) and the same current limit (in amperes) for the specified channel(s) that attempt to generate within the session. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify voltage and current level range.
        /// </para>
        /// </remarks>
        public void ForceVoltage(double[] voltageLevel,
                                   double currentLimit,
                                   double? voltageLevelRange = null,
                                   double? currentLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceVoltage(ssc, voltageLevel[ssc.PinIndex], currentLimit, voltageLevelRange, currentLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual voltage levels and current limits (defined in arrays) for the specified channel(s) that attempt to generate within the session. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify voltage and current level range.
        /// </para>
        /// </remarks>        
        public void ForceVoltage(double[] voltageLevel,
                                   double[] currentLimit,
                                   double? voltageLevelRange = null,
                                   double? currentLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceVoltage(ssc, voltageLevel[ssc.PinIndex], currentLimit[ssc.PinIndex], voltageLevelRange, currentLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual voltage levels, current limits, voltage ranges and current ranges (defined in arrays) for the specified channel(s) that attempt to generate within the session. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// </remarks> 
        public void ForceVoltage(double[] voltageLevel,
                                   double[] currentLimit,
                                   double[] voltageLevelRange,
                                   double[] currentLimitRange,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceVoltage(ssc, voltageLevel[ssc.PinIndex], currentLimit[ssc.PinIndex], voltageLevelRange[ssc.PinIndex], currentLimitRange[ssc.PinIndex], initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual voltage levels, current limits and current limit ranges (defined in arrays) for the specified channel(s) that attempt to generate within the session. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify voltage level range (default is null).
        /// </para>
        /// </remarks>
        public void ForceVoltage(double[] voltageLevel,
                                   double[] currentLimit,
                                   double[] currentLimitRange,
                                   double? voltageLevelRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceVoltage(ssc, voltageLevel[ssc.PinIndex], currentLimit[ssc.PinIndex], voltageLevelRange, currentLimitRange[ssc.PinIndex], initiateSessionAfter));
        }

        /// <summary>
        /// Sets the same voltage level (in volts) and current limit (in amperes) for the specified DCPower session that attempt to generate. By default voltage sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify voltage and current level range.
        /// </para>
        /// </remarks>
        private static void ForceVoltage(DCPowerSSC ssc,
                                    double voltageLevel,
                                    double currentLimit,
                                    double? voltageLevelRange = null,
                                    double? currentLimitRange = null,
                                    bool initiateSessionAfter = true)
        {
            // Use Level and Limit to find ranges if not specified
            double VLevelRange = voltageLevelRange ?? Math.Abs(voltageLevel);
            double CurLimitRange = currentLimitRange ?? Math.Abs(currentLimit);

            ssc.Session.Control.Abort();
            ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.ComplianceLimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel = voltageLevel;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevelRange = VLevelRange;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimit = currentLimit;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitRange = CurLimitRange;
            if (initiateSessionAfter) ssc.Session.Control.Initiate();
        }

        /// <summary>
        /// Clears measurement trigger settings for all sessions.
        /// </summary>
        /// <remarks>
        /// Sets source trigger and start trigger options to None.
        /// </remarks>
        public void ClearTriggers()
        {
            Parallel.ForEach(SSC, ssc =>
            {
                if (ssc.InstrumentModel != 4110)
                {
                    ssc.Session.Control.Abort();
                    ssc.Session.Triggers.SourceTrigger.Type = DCPowerSourceTriggerType.None;
                    ssc.Session.Triggers.StartTrigger.Type = DCPowerStartTriggerType.None;
                }
            });
        }

        /// <summary>
        /// Sources a DC voltage with the compliance limit symmetry set to asymmetric.
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property.</param>
        /// <param name="currentLimitHigh">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="currentLimitLow">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property</param>
        /// <param name="currentLimitRange">Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help or to the instrument specifications for information about valid ranges.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation or acquisition state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// Compliance limits for current generation and voltage generation for the device are applied asymmetrically with respect to 0 V and 0 A.
        /// </para>
        /// <para>
        /// Specify limit High and limit Low values to dictate operation range.
        /// </para>
        /// </remarks>
        public void ForceVoltageAsymmetric(double voltageLevel,
                                             double currentLimitHigh,
                                             double currentLimitLow,
                                             double? voltageLevelRange = null,
                                             double? currentLimitRange = null,
                                             bool initiateSessionAfter = true)
        {
            // Use Level and Limits to find ranges if not specified
            double VLevelRange = voltageLevelRange ?? Math.Abs(voltageLevel);
            double CurLimitRange = currentLimitRange ?? Math.Max(Math.Abs(currentLimitHigh), Math.Abs(currentLimitLow));

            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.ComplianceLimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel = voltageLevel;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevelRange = VLevelRange;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitHigh = currentLimitHigh;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitLow = currentLimitLow;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitRange = CurLimitRange;
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Sets the same current level (in amperes) and voltage limit (in volts) for the specified channel(s) that attempt to generate within the session. By default current sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelUnualified property.</param>
        /// <param name="voltageLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="currentLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify current and voltage level range.
        /// </para>
        /// </remarks>
        public void ForceCurrent(double currentLevel,
                                   double voltageLimit,
                                   double? currentLevelRange = null,
                                   double? voltageLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceCurrent(ssc, currentLevel, voltageLimit, currentLevelRange, voltageLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual current levels (defined in array) and the same voltage limit (in volts) for the specified channel(s) that attempt to generate within the session. By default current sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelUnualified property.</param>
        /// <param name="voltageLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="currentLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify current and voltage level range.
        /// </para>
        /// </remarks>
        public void ForceCurrent(double[] currentLevel,
                                   double voltageLimit,
                                   double? currentLevelRange = null,
                                   double? voltageLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceCurrent(ssc, currentLevel[ssc.PinIndex], voltageLimit, currentLevelRange, voltageLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the individual current levels and voltage limits (defined in arrays) for the specified channel(s) that attempt to generate within the session. By default current sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelUnualified property.</param>
        /// <param name="voltageLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="currentLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.
        /// </para>
        /// <para>
        /// Optional configuration available to modify current and voltage level range.
        /// </para>
        /// </remarks> 
        public void ForceCurrent(double[] currentLevel,
                                   double[] voltageLimit,
                                   double? currentLevelRange = null,
                                   double? voltageLimitRange = null,
                                   bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => ForceCurrent(ssc, currentLevel[ssc.PinIndex], voltageLimit[ssc.PinIndex], currentLevelRange, voltageLimitRange, initiateSessionAfter));
        }

        /// <summary>
        /// Sets the same current level (in amperes) and voltage limit (in volts) for the specified DCPower session that attempt to generate. By default current sourcing will be initiated (can be changed by user).
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelUnualified property.</param>
        /// <param name="voltageLimit">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="currentLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        private static void ForceCurrent(DCPowerSSC ssc,
                                    double currentLevel,
                                    double voltageLimit,
                                    double? currentLevelRange = null,
                                    double? voltageLimitRange = null,
                                    bool initiateSessionAfter = true)
        {
            // Use Level and Limit to find ranges if not specified
            double CurLevelRange = currentLevelRange ?? Math.Abs(currentLevel);
            double VLimitRange = voltageLimitRange ?? Math.Abs(voltageLimit);

            ssc.Session.Control.Abort();
            ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.ComplianceLimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevel = currentLevel;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevelRange = CurLevelRange;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimit = voltageLimit;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitRange = VLimitRange;
            if (initiateSessionAfter) ssc.Session.Control.Initiate();
        }

        /// <summary>
        /// Sources a DC current with the compliance limit symmetry set to asymmetric.
        /// </summary>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelUnualified property.</param>
        /// <param name="voltageLimitHigh">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="voltageLimitLow">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimit property.</param>
        /// <param name="currentLevelRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        /// <remarks>
        /// <para>
        /// Compliance limits for current generation and voltage generation for the device are applied asymmetrically with respect to 0 V and 0 A.
        /// </para>
        /// <para>
        /// Specify limit High and limit Low values to dictate operation range.
        /// </para>
        /// </remarks>
        public void ForceCurrentAsymmetric(double currentLevel,
                                             double voltageLimitHigh,
                                             double voltageLimitLow,
                                             double? currentLevelRange = null,
                                             double? voltageLimitRange = null,
                                             bool initiateSessionAfter = true)
        {
            // Use Level and Limits to find ranges if not specified
            double CurLevelRange = currentLevelRange ?? Math.Abs(currentLevel);
            double VLimitRange = voltageLimitRange ?? Math.Max(Math.Abs(voltageLimitHigh), Math.Abs(voltageLimitLow));

            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.Control.Abort();
                ssc.Session.Source.Mode = DCPowerSourceMode.SinglePoint;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.ComplianceLimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevel = currentLevel;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevelRange = CurLevelRange;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitHigh = voltageLimitHigh;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitLow = voltageLimitLow;
                ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitRange = VLimitRange;
                if (initiateSessionAfter) ssc.Session.Control.Initiate();
            });
        }

        /// <summary>
        /// Gets the Current limit values for all the configured devices in the session.
        /// </summary>
        /// <returns>
        /// An array of Current limit values in amperes that correspond to each channel(s).
        /// </returns>
        public double[] GetCurrentLimits()
        {
            var currentLimits = new double[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) => currentLimits[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimit);
            return currentLimits;
        }

        /// <summary>
        /// Gets the aperture time for all the configured devices in the session.
        /// </summary>
        /// <returns>
        /// An array of aperture time values in seconds that correspond to each channel(s).
        /// </returns>
        public double[] GetApertureTimesinSeconds()
        {
            var apertureTimes = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                switch (ssc.InstrumentModel)
                {
                    case 4110:
                        // http://zone.ni.com/reference/en-XX/help/370736U-01/ni_dc_power_supplies_help/supportedproperties_4110/
                        // The 4110 uses samples to average and has a fixed sample rate of 3kHz, convert this to a an aperture time in seconds
                        apertureTimes[index] = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.SamplesToAverage / 3000.0;
                        break;
                    default:
                        var aperture = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTime;
                        var apertureUnits = ssc.Session.Outputs[ssc.DriverChannelList].Measurement.ApertureTimeUnits;
                        if (apertureUnits == DCPowerMeasureApertureTimeUnits.PowerLineCycles)
                        {
                            aperture = aperture / InstrCtrl.PowerLineFrequency;
                        }
                        apertureTimes[index] = aperture;
                        break;
                }
            });

            return apertureTimes;
        }

        /// <summary>
        /// Configures the same Current Limit for all Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="currentLimit">Current Limit value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="currentLimitRange">Current Limit Range value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an single value for current limit (one per device within the session). 
        /// </remarks>
        public void SetCurrentLimit(double currentLimit, double? currentLimitRange = null)
        {
            Parallel.ForEach(SSC, ssc => SetCurrentLimit(ssc, currentLimit, currentLimitRange));
        }

        /// <summary>
        /// Configures Current Limit for Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="currentLimit">Current Limit value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="currentLimitRange">Current Limit Range value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an array of current limit (one per device within the session). 
        /// </remarks>
        public void SetCurrentLimit(double[] currentLimit, double? currentLimitRange = null)
        {
            Parallel.ForEach(SSC, ssc => SetCurrentLimit(ssc, currentLimit[ssc.PinIndex], currentLimitRange));
        }

        /// <summary>
        /// Configures Current Limit and Current Limit Range for Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="currentLimit">Current Limit value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="currentLimitRange">Current Limit Range value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an array of current limit and current limit ranges (one per device within the session). 
        /// </remarks>
        public void SetCurrentLimit(double[] currentLimit, double[] currentLimitRange)
        {
            Parallel.ForEach(SSC, ssc => SetCurrentLimit(ssc, currentLimit[ssc.PinIndex], currentLimitRange[ssc.PinIndex]));
        }

        /// <summary>
        /// Configures Current Limit and Current Limit Range for Voltage Measurements, for the DCpowerSSC sessions specified.
        /// </summary>
        /// <param name="ssc">DCPowerSCC object member.</param>
        /// <param name="currentLimit">Current Limit value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <param name="currentLimitRange">Current Limit Range value in amperes. The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Values for current limit and current limit range are defined in amperes.
        /// </remarks>
        private static void SetCurrentLimit(DCPowerSSC ssc, double currentLimit, double? currentLimitRange = null)
        {
            double curLimitRange = currentLimitRange ?? Math.Abs(currentLimit);

            ssc.Session.Control.Abort();
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimit = currentLimit;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitRange = curLimitRange;
            ssc.Session.Control.Initiate();
        }

        /// <summary>
        /// Configures measurement aquisition for all measurement channels.
        /// </summary>
        /// <param name="when">Specifies the condition on which event will ocurr. DCPowerMeasurementWhen option.</param>
        /// <param name="initiateSessionAfter">Bool value to move session to generation state after configuration. Default value is true.</param>
        /// <remarks>
        /// DCPowerMeasurementWhen parameter options: AfterSource, OnDemand and OnMeasureTrigger.
        /// </remarks>
        public void ConfigureMeasureWhen(DCPowerMeasurementWhen when, bool initiateSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc =>
            {

                if (ssc.InstrumentModel == 4110)
                {
                    // Only supported setting for the 4110 is on demand, don't do anything
                }
                else if (InstrCtrl.UseSoftwareMeasureTriggers && when == DCPowerMeasurementWhen.OnDemand)
                {
                    var initialSetting = ssc.Session.Measurement.Configuration.MeasureWhen;
                    var initialEdge = ssc.Session.Triggers.MeasureTrigger.Type;

                    if (initialSetting != DCPowerMeasurementWhen.OnMeasureTrigger ||
                        initialEdge != DCPowerMeasureTriggerType.SoftwareEdge)
                    {
                        ssc.Session.Control.Abort();
                        ssc.Session.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger();
                        if (initiateSessionAfter) ssc.Session.Control.Initiate();
                    }
                }
                else
                {
                    var initialSetting = ssc.Session.Measurement.Configuration.MeasureWhen;
                    if (initialSetting != when)
                    {
                        ssc.Session.Control.Abort();
                        ssc.Session.Measurement.Configuration.MeasureWhen = when;
                        if (initiateSessionAfter) ssc.Session.Control.Initiate();
                    }
                }
            });
        }

        /// <summary>
        /// Performs voltage and current measurements.
        /// </summary>
        /// <param name="voltageMeasurements">An array of numeric values with the voltage measurements performed at the channel(s) from the device session.</param>
        /// <param name="currentMeasurements">An array of numeric values with the and current measurements performed at the channel(s) from the device session.</param>
        /// <returns>
        /// An array of numeric values with the voltage and current measurements performed at the channel(s) from the device session.
        /// </returns>
        /// <remarks>
        /// Each call to this method blocks other method calls until the measurements are returned from the device. The order of the measurements returned in the array corresponds to the order on the specified output channel(s).
        /// </remarks>
        public void Measure(out double[] voltageMeasurements, out double[] currentMeasurements)
        {
            var vMeasurements = new double[SSC.Length];
            var iMeasurements = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                //DebugComment
                if (false)
                //if (InstrCtrl.UseSoftwareMeasureTriggers && ssc.InstrumentModel != 4110)
                {
#pragma warning disable CS0162 // Unreachable code detected
                    ssc.Session.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                    var measurements = ssc.Session.Measurement.Fetch(ssc.DriverChannelList, PrecisionTimeSpan.FromSeconds(5), 1);
                    vMeasurements[index] = measurements.VoltageMeasurements[0];
                    iMeasurements[index] = measurements.CurrentMeasurements[0];
#pragma warning restore CS0162 // Unreachable code detected
                }
                else
                {
                    var measurements = ssc.Session.Measurement.Measure(ssc.DriverChannelList);
                    vMeasurements[index] = measurements.VoltageMeasurements[0];
                    iMeasurements[index] = measurements.CurrentMeasurements[0];
                }
            });

            voltageMeasurements = vMeasurements;
            currentMeasurements = iMeasurements;
        }


        /// <summary>
        /// Performs voltage and current measurements.
        /// </summary>
        /// <param name="perPinVoltageMeasurements">An 2D array of per pin voltage measurements, provided in a per-site basis</param>
        /// <param name="perPinCurrentMeasurements">An 2D array of per pin current measurements, provided in a per-site basis</param>
        public void Measure(out double[,] perPinVoltageMeasurements, out double[,] perPinCurrentMeasurements)
        {
            perPinVoltageMeasurements = new double[Pins.Length, SiteNumbers.Length];
            perPinCurrentMeasurements = new double[Pins.Length, SiteNumbers.Length];

            Measure(out double[] voltages, out double[] currents);
            int sessionIndex = 0;
            foreach (var ssc in SSC)
            {
                perPinVoltageMeasurements[ssc.PinIndex, ssc.SiteIndex] = voltages[sessionIndex];
                perPinCurrentMeasurements[ssc.PinIndex, ssc.SiteIndex] = currents[sessionIndex];
                sessionIndex++;
            }
        }

        /// <summary>
        /// Queries the specified output channels to determine if it is operating at the compliance limit.
        /// </summary>
        /// <returns>
        /// An array of bools telling whether the specified output channel is in compliance with the evaluated measurement.
        /// </returns>
        public bool[] MeasureCompliance()
        {
            var complianceMeasurements = new bool[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                if (InstrCtrl.UseSoftwareMeasureTriggers && ssc.InstrumentModel != 4110)
                {
                    ssc.Session.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                    var measurements = ssc.Session.Measurement.Fetch(ssc.DriverChannelList, PrecisionTimeSpan.FromSeconds(5), 1);
                    complianceMeasurements[index] = measurements.InCompliance[0];
                }
                else
                {
                    complianceMeasurements[index] = ssc.Session.Measurement.QueryInCompliance(ssc.DriverChannelList);
                }
            });

            return complianceMeasurements;
        }

        /// <summary>
        /// Returns a struct of data created from measurements.
        /// </summary>
        /// <param name="sampleCount">Specifies the number of measurements to fetch.</param>
        /// <param name="timeoutInSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <remarks>
        /// User can specify the number of measurements to fetch (sampleCount) and the maximum time (timeoutInSeconds) allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.
        /// </remarks>
        /// <returns>
        /// Returns a NationalInstruments.ModularInstruments.NIDCPower.DCPowerFetchResult struct that contains arrays of current measurements, voltage measurements and compliance measurements.
        /// </returns>
        public DCPowerFetchResult[] Fetch(int sampleCount = 1, double timeoutInSeconds = 5.0)
        {
            var measurements = new DCPowerFetchResult[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, i) =>
            {
                measurements[i] = SSC[i].Session.Measurement.Fetch(SSC[i].DriverChannelList, PrecisionTimeSpan.FromSeconds(timeoutInSeconds), sampleCount);
            });

            return measurements;
        }

        /// <summary>
        /// Returns a struct of various data created from measurements.
        /// </summary>
        /// <param name="sampleCount">Specifies the number of measurements to fetch.</param>
        /// <param name="timeoutInSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <remarks>
        /// User can specify the number of measurements to fetch (sampleCount) and the maximum time (timeoutInSeconds) allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.
        /// </remarks>
        /// <returns>
        /// Returns a NationalInstruments.ModularInstruments.NIDCPower.DCPowerFetchResult struct that contains arrays of current measurements, voltage measurements and compliance measurements.
        /// </returns>
        public DCPowerFetchResult[] Fetch(int[] sampleCount, double timeoutInSeconds = 5.0)
        {
            var measurements = new DCPowerFetchResult[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                measurements[index] = SSC[index].Session.Measurement.Fetch(SSC[index].DriverChannelList, PrecisionTimeSpan.FromSeconds(timeoutInSeconds), sampleCount[index]);
            });

            return measurements;
        }

        /// <summary>
        /// Fetches and Publishes defined sample count data for the selected data (Voltage [V], Current [I], Voltage Delta [DeltaV], Current Delta [DeltaI] and Current Calculation [IC]).
        /// </summary>
        /// <param name="sampleCount">Specifies the number of measurements to fetch.</param>
        /// <param name="resistanceForCurrentCalculation">Resistance value used for current calculation in ohms. default value is 1ohm.</param>
        /// <param name="timeoutInSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="basePublishID">Base name to append to publish data ID. Default is empty.</param>
        /// <param name="publishCurrentData">Default value is set to true.</param>
        /// <param name="currentDataPublishID">Data ID for current values to publish.</param>
        /// <param name="publishVoltageData">Default value is set to true.</param>
        /// <param name="voltageDataPublishID">Data ID for volatage values to publish.</param>
        /// <param name="publishCurrentDelta">Default value is set to true.</param>
        /// <param name="currentDeltaPublishID">Data ID for current delta calulation values to publish.</param>
        /// <param name="publishVoltageDelta">Default value is set to true.</param>
        /// <param name="voltageDeltaPublishID">Data ID for voltage delta calculation values to publish.</param>
        /// <param name="publishCurrentCalculation">Default value is set to true.</param>
        /// <param name="currentCalculationID">Data ID for current calculation values to publish.</param>
        /// <remarks>
        /// Select data to publish via the associated bool value (enable to fetch and publish). Optional, user can provide a base name for all publish IDs.
        /// </remarks>
        public void FetchAndPublish(int sampleCount = 1, double resistanceForCurrentCalculation = 1, double timeoutInSeconds = 5.0, string basePublishID = "",
                        bool publishCurrentData = true, string currentDataPublishID = "I",
                        bool publishVoltageData = true, string voltageDataPublishID = "V",
                        bool publishCurrentDelta = true, string currentDeltaPublishID = "DeltaI",
                        bool publishVoltageDelta = true, string voltageDeltaPublishID = "DeltaV",
                        bool publishCurrentCalculation = true, string currentCalculationID = "IC")
        {

            var measurements = Fetch(sampleCount, timeoutInSeconds);

            PublishFetchData(measurements, resistanceForCurrentCalculation, basePublishID, publishCurrentData, currentDataPublishID, publishVoltageData, voltageDataPublishID, publishCurrentDelta, currentDeltaPublishID, publishVoltageDelta, voltageDeltaPublishID, publishCurrentCalculation, currentCalculationID);
        }

        /// <summary>
        /// Clears Fetch Backlog.
        /// </summary>
        public void ClearFetchBacklog()
        {
            Parallel.ForEach(SSC, (ssc) =>
            {
                var backlog = ssc.Session.Measurement.FetchBacklog;
                if (backlog > 0) ssc.Session.Measurement.Fetch(ssc.DriverChannelList, PrecisionTimeSpan.FromSeconds(1), backlog);
            });
        }

        /// <summary>
        /// Fetches and Publishes multi data for the selected data (Voltage [V], Current [I], Voltage Delta [DeltaV], Current Delta [DeltaI] and Current Calculation [IC]).
        /// </summary>
        /// <param name="fetchData">Fetched data to publish. Data comes in a DCPowerFetchResult array format.</param>
        /// <param name="resistanceForCurrentCalculation">Resistance value used for current calculation in ohms. default value is 1ohm.</param>
        /// <param name="basePublishID">Base name to append to publish data ID. Default is empty.</param>
        /// <param name="publishCurrentData">Default value is set to true.</param>
        /// <param name="currentDataPublishID">Data ID for current values to publish.</param>
        /// <param name="publishVoltageData">Default value is set to true.</param>
        /// <param name="voltageDataPublishID">Data ID for volatage values to publish.</param>
        /// <param name="publishCurrentDelta">Default value is set to true.</param>
        /// <param name="currentDeltaPublishID">Data ID for current delta calulation values to publish.</param>
        /// <param name="publishVoltageDelta">Default value is set to true.</param>
        /// <param name="voltageDeltaPublishID">Data ID for voltage delta calculation values to publish.</param>
        /// <param name="publishCurrentCalculation">Default value is set to true.</param>
        /// <param name="currentCalculationID">Data ID for current calculation values to publish.</param>
        /// <remarks>
        /// Select data to publish via the associated bool value (enable to fetch and publish). Optional, user can provide a base name for all publish IDs.
        /// </remarks>
        public void PublishFetchData(DCPowerFetchResult[] fetchData, double resistanceForCurrentCalculation = 1, string basePublishID = "",
            bool publishCurrentData = true, string currentDataPublishID = "I",
            bool publishVoltageData = true, string voltageDataPublishID = "V",
            bool publishCurrentDelta = true, string currentDeltaPublishID = "DeltaI",
            bool publishVoltageDelta = true, string voltageDeltaPublishID = "DeltaV",
            bool publishCurrentCalculation = true, string currentCalculationID = "IC")
        {
            var voltageData = new double[SSC.Length];
            var currentData = new double[SSC.Length];
            var voltageDelta = new double[SSC.Length];
            var currentDelta = new double[SSC.Length];
            var currentCalculation = new double[SSC.Length];

            var sampleCount = fetchData[0].VoltageMeasurements.Length;
            var appendSampleNumber = sampleCount > 1;
            var sampleString = "";

            // Only publish deltas if there's more than one sample
            publishCurrentDelta &= appendSampleNumber;
            publishVoltageDelta &= appendSampleNumber;

            for (int sample = 0; sample < sampleCount; sample++)
            {
                if (appendSampleNumber) sampleString = sample.ToString();

                for (int sessionNdx = 0; sessionNdx < SSC.Length; sessionNdx++)
                {
                    voltageDelta[sessionNdx] = fetchData[sessionNdx].VoltageMeasurements[sample] - voltageData[sessionNdx];
                    currentDelta[sessionNdx] = fetchData[sessionNdx].CurrentMeasurements[sample] - currentData[sessionNdx];
                    voltageData[sessionNdx] = fetchData[sessionNdx].VoltageMeasurements[sample];
                    currentData[sessionNdx] = fetchData[sessionNdx].CurrentMeasurements[sample];

                    for (int i = 0; i < voltageData.Length; i++)
                    {
                        currentCalculation[i] = voltageData[i] / resistanceForCurrentCalculation;
                    }
                }

                if (publishCurrentDelta) PinQueryContext.Publish(currentDelta, basePublishID + currentDeltaPublishID + sampleString);
                if (publishVoltageDelta) PinQueryContext.Publish(voltageDelta, basePublishID + publishVoltageDelta + sampleString);
                if (publishVoltageData) PinQueryContext.Publish(voltageData, basePublishID + voltageDataPublishID + sampleString);
                if (publishCurrentData) PinQueryContext.Publish(currentData, basePublishID + currentDataPublishID + sampleString);
                if (publishCurrentCalculation) PinQueryContext.Publish(currentCalculation, basePublishID + currentCalculationID + sampleString);
            }
        }

        /// <summary>
        /// Waits until the device has generated the Source Complete event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForSourceComplete(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.SourceCompleteEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.SourceCompleteEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Waits until the device has generated the Measure Complete event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForMeasureComplete(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.MeasureCompleteEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.MeasureCompleteEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Waits until the device has generated the Sequence Iteration Complete event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForSequenceIterationComplete(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.SequenceIterationCompleteEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.SequenceIterationCompleteEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Waits until the device has generated the Sequence engine done event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Maximum time to wait for sequence event completion. Default value is set to 5 seconds.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForSequenceEngineDone(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.SequenceEngineDoneEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.SequenceEngineDoneEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Waits until the device has generated the Pulse Complete event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForPulseComplete(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.PulseCompleteEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.PulseCompleteEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Waits until the device has generated the Pulse Trigger event. 
        /// </summary>
        /// <param name="timeoutinSeconds">Specifies the maximum time allowed for this method to complete, in seconds. If the method does not complete within this time interval, NI-DCPower returns an error.</param>
        /// <param name="firstSessionOnly">Bool to identify if is the first session only. Default is set to TRUE.</param>
        /// <remarks>
        /// <para>
        /// Parameter timeoutinSeconds specifies the maximum time allowed for this method to complete, in seconds (default value is 5 seconds). If the method does not complete within this time interval, returns an error. Optional, user can define if only wants to wait for the first session event or all of them.
        /// </para>
        /// <para>
        /// This method must only be called in the Running state. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </para>
        /// </remarks>
        public void WaitForPulseTrigger(Double timeoutinSeconds = 5.0, bool firstSessionOnly = true)
        {
            PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutinSeconds);
            if (firstSessionOnly)
            {
                SSC[0].Session.Events.ReadyForPulseTriggerEvent.WaitForEvent(timeout);
            }
            else
            {
                Parallel.ForEach(SSC, ssc =>
                {
                    ssc.Session.Events.ReadyForPulseTriggerEvent.WaitForEvent(timeout);
                });
            }
        }

        /// <summary>
        /// Sets the voltage level, in volts, for the specified channel(s).
        /// </summary>
        /// <param name="voltageLevel">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property. The channel must be enabled for the specified voltage level to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property. The channel must be enabled for the specified voltage level to take effect.
        /// </remarks>
        public void ConfigureVoltageLevel(double voltageLevel) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevel = voltageLevel);

        /// <summary>
        /// Sets the voltage level range, in volts, for the specified channel(s). The range defines the valid values to which the voltage level can be set.
        /// </summary>
        /// <param name="voltageLevelRange">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property. The channel must be enabled for the specified voltage level range to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.VoltageLevelRangeUnqualified property. The channel must be enabled for the specified voltage level range to take effect.
        /// </remarks>
        public void ConfigureVoltageLevelRange(double voltageLevelRange) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevelRange = voltageLevelRange);

        /// <summary>
        /// Sets the current limit, in amperes, for the output not to exceed when generating the desired voltage level on the specified channel(s).
        /// </summary>
        /// <param name="currentLimit">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit to take effect.
        /// </remarks>
        public void ConfigureCurrentLimit(double currentLimit) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimit = currentLimit);

        /// <summary>
        /// Sets the current limit high, in amperes, for the output not to exceed when generating the desired voltage level on the specified channel(s).
        /// </summary>
        /// <param name="currentLimitHigh">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit low to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit low to take effect.
        /// </remarks>
        public void ConfigureCurrentLimitHigh(double currentLimitHigh) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitHigh = currentLimitHigh);

        /// <summary>
        /// Sets the current limit low, in amperes, for the output not to exceed when generating the desired voltage level on the specified channel(s).
        /// </summary>
        /// <param name="currentLimitLow">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit low to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceVoltage.CurrentLimitRangeUnqualified property. The channel must be enabled for the specified current limit low to take effect.
        /// </remarks>
        public void ConfigureCurrentLimitLow(double currentLimitLow) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitLow = currentLimitLow);

        /// <summary>
        /// Sets the current limit range, in amperes, for the specified channel(s). 
        /// </summary>
        /// <param name="currentLimitRange">The range defines the valid value to which the current limit can be set. The channel must be enabled for the specified current limit to take effect. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for information about valid ranges.</param>
        /// <remarks>
        /// The range defines the valid value to which the current limit can be set. The channel must be enabled for the specified current limit to take effect. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for information about valid ranges.
        /// </remarks>
        public void ConfigureCurrentLimitRange(double currentLimitRange) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitRange = currentLimitRange);

        /// <summary>
        /// Sets the current level, in amperes, that the specified channel(s) attempt to generate.
        /// </summary>
        /// <param name="currentLevel">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property. Enable the channel for the specified current level to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.CurrentLevelRangeUnqualified property. Enable the channel for the specified current level to take effect.
        /// </remarks>
        public void ConfigureCurrentLevel(double currentLevel) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevel = currentLevel);

        /// <summary>
        /// Sets the current level range, in amperes, for the specified channel(s).
        /// </summary>
        /// <param name="currentLevelRange">The range defines the valid value to which the current level can be set. The channel must be enabled for the specified current level range to take effect. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for informatio about valid ranges.</param>
        /// <remarks>
        /// The range defines the valid value to which the current level can be set. The channel must be enabled for the specified current level range to take effect. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for informatio about valid ranges.
        /// </remarks>
        public void ConfigureCurrentLevelRange(double currentLevelRange) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevelRange = currentLevelRange);

        /// <summary>
        /// Sets the voltage limit for the output to not exceed when generating the  desired current level on the specified channels.
        /// </summary>
        /// <param name="voltageLimit">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property. The channel must be enabled for the specified voltage limit to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property. The channel must be enabled for the specified voltage limit to take effect.
        /// </remarks>
        public void ConfigureVoltageLimit(double voltageLimit) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimit = voltageLimit);

        /// <summary>
        /// Sets the voltage limit high for the output to not exceed when generating the desired current level on the specified channels.
        /// </summary>
        /// <param name="voltageLimitHigh">The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRange property. The channel must be enabled for the specified voltage limit high to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRange property. The channel must be enabled for the specified voltage limit high to take effect.
        /// </remarks>
        public void ConfigureVoltageLimitHigh(double voltageLimitHigh) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitHigh = voltageLimitHigh);

        /// <summary>
        /// Sets the voltage limit low for the output to not exceed when generating the desired current level on the specified channels. 
        /// </summary>
        /// <param name="voltageLimitLow">The valid values for this property are defined by the values you specify for  the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRange property. The channel must be enabled for the specified voltage limit low to take effect.</param>
        /// <remarks>
        /// The valid values for this property are defined by the values you specify for  the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitRange property. The channel must be enabled for the specified voltage limit low to take effect.
        /// </remarks>
        public void ConfigureVoltageLimitLow(double voltageLimitLow) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitLow = voltageLimitLow);

        /// <summary>
        /// Sets the voltage limit range, in volts, for the specified channel(s).
        /// </summary>
        /// <param name="voltageLimitRange">The valid values to which the voltage limit can be set. Use the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitAutorangeUnqualified property to enable automatic selection of the voltage limit range. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for information about valid ranges.</param>
        /// <remarks>
        /// The range defines the valid values to which the voltage limit can be set. Use the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputSourceCurrent.VoltageLimitAutorangeUnqualified property to enable automatic selection of the voltage limit range. Refer to the Ranges topic in the NI DC Power Supplies and SMUs Help for information about valid ranges.
        /// </remarks>
        public void ConfigureVoltageLimitRange(double voltageLimitRange) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitRange = voltageLimitRange);

        /// <summary>
        /// Sets whether to enable or disable generation on the specified channel(s). The default value is true if you use the niDCPower_InitializeWithChannels method to open the session, otherwise the default value is false.
        /// </summary>
        /// <param name="outputEnabled">The default value is true.</param>
        /// <remarks>
        /// If channels are in the Committed or Uncommitted states, enabling the output does not take effect until you call the NationalInstruments.ModularInstruments.NIDCPower.DCPowerOutputControl.InitiatePartiallyQualified method. Refer to the Programming States topic in the NI DC Power Supplies and SMUs Help for information about the specific NI-DCPower software states.
        /// </remarks>
        public void ConfigureOutputEnabled(bool outputEnabled = true) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Enabled = outputEnabled);

        /// <summary>
        /// Sets the output relay to connected (closed) or disconnected (open). Set this property to false to disconnect the output terminal from the output.
        /// </summary>
        /// <param name="outputConnected">The default value is true.</param>
        /// <remarks>
        /// Disconnect the output only if disconnecting is necessary for your application. For example, a battery connected to the output terminal might discharge unless the relay is disconnected. Excessive connecting and disconnecting of the output can cause premature wear on the relay. This property is not supported by all devices. Refer to the Supported Properties by Device topic in the NI DC Power Supplies and SMUs Help for information about supported devices.
        /// </remarks>
        public void ConfigureOutputConnected(bool outputConnected = true) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Connected = outputConnected);

        /// <summary>
        /// Configures the output resistance of the SMU channels.
        /// </summary>
        /// <param name="outputResistance">Output resistance value in ohms.</param>
        public void ConfigureOutputResistance(double outputResistance) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Resistance = outputResistance);

        /// <summary>
        /// Method is used to returns the max Current (A) Range for each DCPower Session.
        /// </summary>
        /// <returns>An array of double values with current range levels.</returns>
        public double[] GetCurrentLevelRange()
        {
            var maxCurrent = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                maxCurrent[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevelRange;
            });

            return maxCurrent;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public double[] GetCurrentLimit()
        {
            var currentLimit = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                currentLimit[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimit;
            });

            return currentLimit;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public double[] GetCurrentLimitRange()
        {
            var currentLimit = new double[SSC.Length];

            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                currentLimit[index] = ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.CurrentLimitRange;
            });

            return currentLimit;
        }

        /// <summary>
        /// Sets whether to select the current level range based on the desired current level (in amperes) for the specified DCPower session.
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="currentLevelAutorange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.CurrentLevelAutorange property.</param>
        /// <param name="commitSessionAfter">Bool value to move session to commited state after configuration. Default value is true.</param>
        private static void SetSourceCurrentAutorange(
            DCPowerSSC ssc,
            DCPowerSourceCurrentLevelAutorange currentLevelAutorange,
            bool commitSessionAfter = true)
        {
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.CurrentLevelAutorange = currentLevelAutorange;
        }

        /// <summary>
        /// Configures SourceCurrentAutorange for all devices in the current session.
        /// </summary>
        /// <param name="currentLevelAutorange">Specify whether to select the current level range based on the desired current level (in amperes) for the specified DCPower session.</param>
        /// <param name="commitSessionAfter">Bool value to move session to commited state after configuration. Default value is true.</param>
        /// <remarks>
        /// Receives an single value for DCPowerSourceCurrentLevelAutorange.
        /// </remarks>
        public void SetSourceCurrentAutorange(DCPowerSourceCurrentLevelAutorange currentLevelAutorange, bool commitSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => SetSourceCurrentAutorange(ssc, currentLevelAutorange, commitSessionAfter));
        }

        /// <summary>
        /// Configures SourceCurrentAutorange for all devices in the current session.
        /// </summary>
        /// <param name="currentLevelAutorange">Specify whether to select the current level range based on the desired current level (in amperes) for the specified DCPower session.</param>
        /// <param name="commitSessionAfter">Bool value to move session to commited state after configuration. Default value is true.</param>
        /// <remarks>
        /// Receives an array of DCPowerSourceCurrentLevelAutorange.(one per device within the session).
        /// </remarks>
        public void SetSourceCurrentAutorange(DCPowerSourceCurrentLevelAutorange[] currentLevelAutorange, bool commitSessionAfter = true)
        {
            Parallel.ForEach(SSC, ssc => SetSourceCurrentAutorange(ssc, currentLevelAutorange[ssc.PinIndex], commitSessionAfter));
        }

        /// <summary>
        /// Sets whether to select the voltage level range based on the desired voltage level (in volt) for the specified DCPower session.
        /// </summary>
        /// <param name="ssc">DCPowerSSC object member.</param>
        /// <param name="voltageLevelAutorange">The valid values for this property are defined by the values you specify for DCPowerOutputSourceVoltage.VoltageLevelAutorange property.</param>
        private static void SetSourceVoltageAutorange(
            DCPowerSSC ssc,
            DCPowerSourceVoltageLevelAutorange voltageLevelAutorange)
        {
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Voltage.VoltageLevelAutorange = voltageLevelAutorange;
        }

        /// <summary>
        /// Configures SourceVoltageAutorange for all devices in the current session.
        /// </summary>
        /// <param name="voltageLevelAutorange">Specify whether to select the voltage level range based on the desired voltage level (in volt) for the specified DCPower session.</param>
        /// <remarks>
        /// Receives an single value for DCPowerSourceVoltageLevelAutorange.
        /// </remarks>
        public void SetSourceVoltageAutorange(DCPowerSourceVoltageLevelAutorange voltageLevelAutorange)
        {
            Parallel.ForEach(SSC, ssc => SetSourceVoltageAutorange(ssc, voltageLevelAutorange));
        }

        /// <summary>
        /// Configures SourceVoltageAutorange for all devices in the current session.
        /// </summary>
        /// <param name="voltageLevelAutorange">Specify whether to select the voltage level range based on the desired voltage level (in volt) for the specified DCPower session.</param>
        /// <remarks>
        /// Receives an array of DCPowerSourceVoltageLevelAutorange.(one per device within the session).
        /// </remarks>
        public void SetSourceVoltageAutorange(DCPowerSourceVoltageLevelAutorange[] voltageLevelAutorange)
        {
            Parallel.ForEach(SSC, ssc => SetSourceVoltageAutorange(ssc, voltageLevelAutorange[ssc.PinIndex]));
        }

        /// <summary>
        /// Configures the same Voltage Limit for all Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="voltageLimit">Voltage Limit value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">Voltage Limit Range value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an single value for current limit (one per device within the session). 
        /// </remarks>
        public void SetVoltageLimit(double voltageLimit, double? voltageLimitRange = null)
        {
            Parallel.ForEach(SSC, ssc => SetVoltageLimit(ssc, voltageLimit, voltageLimitRange));
        }

        /// <summary>
        /// Configures Voltage Limit for Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="voltageLimit">Voltage Limit value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">Voltage Limit Range value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an array of current limit (one per device within the session). 
        /// </remarks>
        public void SetVoltageLimit(double[] voltageLimit, double? voltageLimitRange = null)
        {
            Parallel.ForEach(SSC, ssc => SetVoltageLimit(ssc, voltageLimit[ssc.PinIndex], voltageLimitRange));
        }

        /// <summary>
        /// Configures Voltage Limit and Voltage Limit Range for Voltage Measurements, for the for all devices in the current session.
        /// </summary>
        /// <param name="voltageLimit">Voltage Limit value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">Voltage Limit Range value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Receives an array of current limit and current limit ranges (one per device within the session). 
        /// </remarks>
        public void SetVoltageLimit(double[] voltageLimit, double[] voltageLimitRange)
        {
            Parallel.ForEach(SSC, ssc => SetVoltageLimit(ssc, voltageLimit[ssc.PinIndex], voltageLimitRange[ssc.PinIndex]));
        }

        /// <summary>
        /// Configures Voltage Limit and Voltage Limit Range for Voltage Measurements, for the DCpowerSSC sessions specified.
        /// </summary>
        /// <param name="ssc">DCPowerSCC object member.</param>
        /// <param name="voltageLimit">Voltage Limit value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <param name="voltageLimitRange">Voltage Limit Range value in volt. The valid values for this property are defined by the values you specify for DCPowerOutputSourceCurrent.VoltageLimitRangeUnqualified property.</param>
        /// <remarks>
        /// Values for current limit and current limit range are defined in amperes.
        /// </remarks>
        private static void SetVoltageLimit(DCPowerSSC ssc, double voltageLimit, double? voltageLimitRange = null)
        {
            double volLimitRange = voltageLimitRange ?? Math.Abs(voltageLimit);

            ssc.Session.Control.Abort();
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimit = voltageLimit;
            ssc.Session.Outputs[ssc.DriverChannelList].Source.Current.VoltageLimitRange = volLimitRange;
            ssc.Session.Control.Initiate();
        }

        /// <summary>
        /// Configures the method to generate.
        /// </summary>
        /// <param name="outputFunction">The method that the specified channel(s) attempt to generate.</param>
        public void ConfigureOutputFunction(DCPowerSourceOutputFunction outputFunction) => Parallel.ForEach(SSC, ssc => ssc.Session.Outputs[ssc.DriverChannelList].Source.Output.Function = outputFunction);
    }
}