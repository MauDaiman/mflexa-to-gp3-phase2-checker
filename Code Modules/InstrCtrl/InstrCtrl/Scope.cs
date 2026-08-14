using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
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
        /// Generates a NI-Scope session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name</param>
        /// <returns>
        /// A single NI-Scope session.
        /// </returns>
        public static Scope ScopePinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return ScopePinsToSessions(tsmContext, pins);
        }

        /// <summary>
        /// Generates a series of NI-Scope sessions based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A NI-Scope object containing all Pin defined sessions.
        /// </returns>
        public static Scope ScopePinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetNIScopeSessions(pins, out var scopeSessions, out var channelLists);

            var scopessc = new ScopeSSC[scopeSessions.Length];

            var sitenums = tsmContext.SiteNumbers.ToArray();

            var expandedPins = tsmContext.GetPinsInPinGroups(pins);

            tsmContext.GetPins(out var dutPins, out var systemPins);

            for (int sessionNdx = 0; sessionNdx < scopeSessions.Length; sessionNdx++)
            {
                scopessc[sessionNdx].Session = scopeSessions[sessionNdx];
                scopessc[sessionNdx].DriverChannelList = channelLists[sessionNdx];
                scopessc[sessionNdx].PerChannelDriverChannelList = channelLists[sessionNdx].Split(',').Select(p => p.Trim()).ToArray();
                scopessc[sessionNdx].PerChannelTSMChannelList = new string[scopessc[sessionNdx].PerChannelDriverChannelList.Length];
            }

            foreach (var site in sitenums)
            {
                foreach (var pin in expandedPins)
                {
                    pqc.GetSessionAndChannelIndex(site, pin, out var sessionNdx, out var channelNdx);
                    if (systemPins.Contains(pin))
                    {
                        scopessc[sessionNdx].PerChannelTSMChannelList[channelNdx] = pin;
                    }
                    else if (scopessc[sessionNdx].PerChannelTSMChannelList[channelNdx] == null)
                    {
                        scopessc[sessionNdx].PerChannelTSMChannelList[channelNdx] = "Site" + site.ToString() + "/" + pin;
                    }
                    else
                    {
                        var split = scopessc[sessionNdx].PerChannelTSMChannelList[channelNdx].Split('/');
                        scopessc[sessionNdx].PerChannelTSMChannelList[channelNdx] = split[0] + "+" + site.ToString() + "/" + pin;
                    }
                }
            }
            for (int sessionNdx = 0; sessionNdx < scopeSessions.Length; sessionNdx++)
            {
                scopessc[sessionNdx].TSMChannelList = string.Join(",", scopessc[sessionNdx].PerChannelTSMChannelList);
            }

            return new Scope() { PinQueryContext = pqc, Pins = pins, SiteNumbers = sitenums, SSC = scopessc };
        }

        /// <summary>
        /// Initializes all NI-Scope sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitScopeSessions(ISemiconductorModuleContext tsmContext)
        {
            var scopeInstruments = tsmContext.GetNIScopeInstrumentNames();

            Parallel.ForEach(scopeInstruments, scopeInstrument =>
            {
                var session = new NIScope(scopeInstrument, false, true);
                tsmContext.SetNIScopeSession(scopeInstrument, session);
            });
        }

        /// <summary>
        /// Closes all NI-Scope sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseScopeSessions(ISemiconductorModuleContext tsmContext)
        {
            Parallel.ForEach(tsmContext.GetAllNIScopeSessions(), session =>
            {
                session.Close();
            });
        }
    }

    /// <summary>
    /// Structure reference to for Scope SSC members
    /// </summary>
    public struct ScopeSSC
    {
        /// <summary>
        /// NI-Scope session field property.
        /// </summary>
        public NIScope Session { get; set; }
        /// <summary>
        /// TSMChannelList string property.
        /// </summary>
        public string TSMChannelList { get; set; }
        /// <summary>
        /// DriverChannelList string property.
        /// </summary>
        public string DriverChannelList { get; set; }
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
    /// Class definition for Scope Sessions Objects.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public NIScopeMultiplePinMultipleSessionQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// Scope SSC definition.
        /// </summary>
        public ScopeSSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Site numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }

        /// <summary>
        /// Aborts an acquisition and returns the digitizer to the Idle state. 
        /// </summary>
        /// <remarks>
        /// Call this method if the digitizer times out waiting for a trigger.
        /// </remarks>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Abort());

        /// <summary>
        /// Initiates a waveform acquisition.
        /// </summary>
        /// <remarks>
        /// After you call this method, the digitizer leaves the Idle state and waits for a trigger. The digitizer acquires a waveform for each enabled channel.
        /// </remarks>
        public void Initiate() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Initiate());

        /// <summary>
        /// Commits to hardware all the parameter settings associated with the task.
        /// </summary>
        /// <remarks>
        /// Use this method if you want a parameter change to be immediately reflected in the hardware. This method is not supported for Traditional NI-DAQ (Legacy) instruments.
        /// </remarks>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Commit());

        /// <summary>
        /// Resets all properties to their default value and stops export of all external signals and events.
        /// </summary>
        public void Reset() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.Reset());

        /// <summary>
        /// Performs a hard reset on the instrument.
        /// </summary>
        /// <remarks>
        /// Resets the instrument to a known state. The method disables power generation, resets session properties to their default values, properties clears errors such as overtemperature and unexpected loss of auxiliary power, commits the session properties, and leaves the session in the Uncommitted state. This method also performs a hard reset on the instrument and driver software. This method has the same functionality as using reset in Measurement and Automation Explorer. This will also open the output relay on instruments that have an output relay.
        /// </remarks>
        public void ResetDevice() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.ResetDevice());

        /// <summary>
        /// Configures the properties that control the electrical characteristics of the the digitizer vertical subsystem.
        /// </summary>
        /// <param name="verticalRange">The absolute value, in volts, of the input range for a channel.</param>
        /// <param name="verticalOffset">The location of the center of the range, in volts, with respect to ground.</param>
        /// <param name="verticalCoupling">The way the digitizer couples the input signal for the channel. When input coupling changes, the input stage takes a finite amount of time to settle.</param>
        /// <param name="probeAttenuation">The probe attenuation for the input channel. For example, for a 10:1 probe, set this property to 10.0.</param>
        /// <param name="enabled">A value indicating whether the digitizer acquires a waveform for the channel.</param>
        public void ConfigureVertical(double verticalRange, double verticalOffset = 0.0, ScopeVerticalCoupling verticalCoupling = ScopeVerticalCoupling.DC, double probeAttenuation = 1.0, bool enabled = true)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Channels[ssc.DriverChannelList].Configure(verticalRange, verticalOffset, verticalCoupling, probeAttenuation, enabled));
        }

        /// <summary>
        /// Configures the common properties of the horizontal subsystem for a multirecord acquisition in terms of minimum sample rate.
        /// </summary>
        /// <param name="sampleRateMin">The sampling rate for the acquisition.</param>
        /// <param name="numberOfPointsMin">The minimum number of points you need in the record for each channel</param>
        /// <param name="referencePosition">The position of the reference event in the waveform record specified as a percentage.</param>
        /// <param name="numberOfRecords">The number of records to acquire.</param>
        /// <param name="enforceRealtime">Indicates whether the digitizer enforces real-time measurements or allows equivalent-time (RIS) measurements. Not all digitizers support RIS. Refer to Features Supported by Device for more information.</param>
        public void ConfigureHorizontalTiming(double sampleRateMin, int numberOfPointsMin, double referencePosition = 50.0, int numberOfRecords = 1, bool enforceRealtime = false)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Timing.ConfigureTiming(sampleRateMin, numberOfPointsMin, referencePosition, numberOfRecords, enforceRealtime));
        }

        /// <summary>
        /// Configures the common properties of a digital trigger.
        /// </summary>
        /// <param name="source">The trigger source</param>
        /// <param name="slope">A value indicating whether you want a rising edge or a falling edge to trigger the digitizer.</param>
        /// <param name="holdoffinSeconds">The length of time, in seconds, that the digitizer waits after detecting a trigger before enabling NI-SCOPE to detect another trigger.</param>
        /// <param name="delayinSeconds">The length of time, in seconds, that the digitizer waits after receiving the trigger to start acquiring data.</param>
        /// <remarks>
        /// Allows for settings of Rising/Falling edge, hold-off and delay times for the trigger.
        /// </remarks>
        public void ConfigureDigitalEdgeTrigger(string source,
            ScopeTriggerSlope slope = ScopeTriggerSlope.Positive,
            double holdoffinSeconds = 0.0,
            double delayinSeconds = 0.0)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Trigger.ConfigureTriggerDigital(source, slope, PrecisionTimeSpan.FromSeconds(holdoffinSeconds), PrecisionTimeSpan.FromSeconds(delayinSeconds))
            );
        }

        /// <summary>
        /// Configures the acquisition type of the scope.
        /// </summary>
        /// <param name="acquisitionType">Specifies how the digitizer acquires data and fills the waveform record. Choose between Normal, FlexibleResolution and Dcd.</param>
        public void ConfigureAcquisition(ScopeAcquisitionType acquisitionType)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Acquisition.Type = acquisitionType);
        }

        /// <summary>
        /// Configures the input impedance and maximum input frequency of the channel.
        /// </summary>
        /// <param name="inputImpedance">Input impedance in ohms</param>
        /// <param name="inputFrequencyMax">Specifies the bandwidth of the channel at which the input circuitry attenuates the signal by 3 dB. Pass 0 for this value to use the hardware default bandwidth. Pass -1 for this value to achieve full bandwidth.</param>
        public void ConfigureChannelCharacteristics(double inputImpedance, double inputFrequencyMax)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Channels[ssc.DriverChannelList].ConfigureCharacteristics(inputImpedance, inputFrequencyMax));
        }

        /// <summary>
        /// Configures the properties for synchronizing the digitizer to an external clock or sending the digitizer's clock output to be used as a synchronizing clock for other devices.
        /// </summary>
        /// <param name="inputClockSource">Specifies the input source for the PLL reference clock (such as the 1-20 MHz clock on SMC-based devices) to which the digitizer is phase-locked for all digitizers.</param>
        /// <param name="outputClockSource">Specifies the input source for the PLL reference clock (such as the 1-20 MHz clock on SMC-based devices) to which the digitizer is phase-locked for all digitizers.</param>
        /// <param name="clockSynchronizationPulseSource">Specifies the line on which the sample clock or the one-time sync pulse is sent or received</param>
        /// <param name="masterEnabled">Specifies whether the device is a master or a slave; the master device is typically the originator of the trigger signal and clock sync pulse. For a standalone device, set this parameter to FALSE.</param>
        public void ConfigureClock(ScopeInputClockSource inputClockSource, ScopeOutputClockSource outputClockSource, ScopeClockSynchronizationPulseSource clockSynchronizationPulseSource, bool masterEnabled)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Timing.ConfigureClock(inputClockSource, outputClockSource, clockSynchronizationPulseSource, masterEnabled));
        }

        /// <summary>
        /// Returns the waveform from a previously initiated acquisition that the digitizer acquires for the specified channel. This method returns scaled voltage waveforms. Refer to Using Fetch Functions for more information on using this method.
        /// </summary>
        /// <param name="numberOfSamples">The maximum number of samples to fetch for each waveform. If the acquisition finishes with fewer points than requested, some instruments return partial data if the acquisition finished, was aborted, or a timeout of 0 was used. Use –1
        /// for this parameter if you want to fetch all available samples. The method reads the actual record length and attempts to acquire all available samples. If it fails to complete within the timeout period, the method returns an error.</param>
        /// <param name="timeoutInSeconds">The time to wait for data to be acquired. Using 0 for this parameter tells NI-SCOPE to fetch whatever is currently available. Using -1 for this parameter implies infinite timeout.</param>
        /// <returns>
        /// A collection of NationalInstruments.AnalogWaveform objects array, one per calling session.
        /// </returns>
        public AnalogWaveformCollection<double>[] Fetch(int numberOfSamples = -1, double timeoutInSeconds = 5.0)
        {
            var waveforms = new AnalogWaveformCollection<double>[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                waveforms[index] = ssc.Session.Channels[ssc.DriverChannelList].Measurement.FetchDouble(PrecisionTimeSpan.FromSeconds(timeoutInSeconds), numberOfSamples, waveforms[index]);
            });
            return waveforms;
        }

        /// <summary>
        /// Returns the waveform from a previously initiated acquisition that the digitizer acquires for the specified channel. This method returns scaled voltage waveforms. Refer to Using Fetch Functions for more information on using this method.
        /// </summary>
        /// <param name="numberOfSamples">The maximum number of samples to fetch for each waveform. If the acquisition finishes with fewer points than requested, some instruments return partial data if the acquisition finished, was aborted, or a timeout of 0 was used. Use –1
        /// for this parameter if you want to fetch all available samples. The method reads the actual record length and attempts to acquire all available samples. If it fails to complete within the timeout period, the method returns an error.</param>
        /// <param name="timeoutInSeconds">The time to wait for data to be acquired. Using 0 for this parameter tells NI-SCOPE to fetch whatever is currently available. Using -1 for this parameter implies infinite timeout.</param>
        /// <returns>
        /// A collection of NationalInstruments.AnalogWaveform objects array, one per calling session.
        /// </returns>
        public AnalogWaveformCollection<double>[] Read(PrecisionTimeSpan timeoutInSeconds, int numberOfSamples = -1)
        {
            var waveforms = new AnalogWaveformCollection<double>[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                waveforms[index] = ssc.Session.Channels[ssc.DriverChannelList].Measurement.Read(timeoutInSeconds, numberOfSamples, waveforms[index]);
            });
            return waveforms;
        }
    }
}