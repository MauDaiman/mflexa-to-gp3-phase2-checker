using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
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
        /// Initializes all NI-Fgen sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitFgenSessions(ISemiconductorModuleContext tsmContext)
        {
            var FgenInstrumentsNames = tsmContext.GetNIFGenInstrumentNames();

            Parallel.ForEach(FgenInstrumentsNames, FgenInstrumentsName =>
            {
                var session = new NIFgen(FgenInstrumentsName, false, true);
                tsmContext.SetNIFGenSession(FgenInstrumentsName, session);
            });
        }

        /// <summary>
        /// Closes all active NI-Fgen sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseFgenSessions(ISemiconductorModuleContext tsmContext)
        {
            Parallel.ForEach(tsmContext.GetAllNIFGenSessions(), session =>
            {
                session.Close();
            });
        }

        /// <summary>
        /// Generates a NI-Fgen session based on the provided Pin name and session context.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name.</param>
        /// <returns>
        /// A single NI-Fgen session.
        /// </returns>
        public static Fgen FgenPinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return FgenPinsToSessions(tsmContext, pins);
        }
        
        /// <summary>
        /// Generates a series of NI-Fgen session based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A NI-Fgen object containing all Pin defined sessions.
        /// </returns>
        public static Fgen FgenPinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetNIFGenSessions(pins, out var fgenSessions, out var channelLists);

            var fgenssc = new FgenSSC[fgenSessions.Length];

            var siteNumbers = tsmContext.SiteNumbers.ToArray();

            for (int sessionNdx = 0; sessionNdx < fgenSessions.Length; sessionNdx++)
            {
                fgenssc[sessionNdx].Session = fgenSessions[sessionNdx];
                fgenssc[sessionNdx].PinIndex = Array.IndexOf(pins, fgenssc[sessionNdx].Pin);
                fgenssc[sessionNdx].SiteIndex = Array.IndexOf(siteNumbers, fgenssc[sessionNdx].SiteNumber);
            }

            return new Fgen() { PinQueryContext = pqc, Pins = pins, SiteNumbers = siteNumbers, SSC = fgenssc, ChannelLists = channelLists };
        }
    }

    /// <summary>
    /// Structure reference to for Fgen SSC members
    /// </summary>
    public struct FgenSSC
    {
        /// <summary>
        /// NI-Fgen session field property.
        /// </summary>
        public NIFgen Session { get; set; }
        /// <summary>
        ///  Pin string property.
        /// </summary>
        public string Pin { get; set; }
        /// <summary>
        /// Pin Index.
        /// </summary>
        public int PinIndex { get; set; }
        /// <summary>
        /// Site number.
        /// </summary>
        public int SiteNumber { get; set; }
        /// <summary>
        /// Site index in array.
        /// </summary>
        public int SiteIndex { get; set; }
    }

    /// <summary>
    /// Class definition for Fgen Sessions Objects.
    /// </summary>
    public class Fgen
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>       
        public NIFGenMultiplePinMultipleSessionQueryContext PinQueryContext { get; set; }

        /// <summary>
        /// Dmm SSC definition.
        /// </summary>
        public FgenSSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Site numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }

        /// <summary>
        /// Channels Lists.
        /// </summary>
        public string[] ChannelLists { get; set; }

        /// <summary>
        /// Aborts any previously initiated signal generation.
        /// </summary>
        public void AbortGeneration() => Parallel.ForEach(SSC, ssc => ssc.Session.AbortGeneration());

        /// <summary>
        /// Initiates signal generation.
        /// </summary>
        public void InitiateGeneration() => Parallel.ForEach(SSC, ssc => ssc.Session.InitiateGeneration());

        /// <summary>
        /// Causes a transition to the committed state.
        /// </summary>
        /// <remarks>This method verifies driver attribute values, reserves the device, and commits the attribute values to the device. If the attribute values are all valid, NI-FGEN sets the device hardware configuration to match the session configuration. This  method does not support the NI 5401/5404/5411/5431 signal generators. In the committed state, you can load waveforms, scripts, and sequences into memory. If any driver attributes are changed, NI-FGEN implicitly transitions back to the idle state, where you can program all session properties before applying them to the device. This method has no effect if the device is already in the committed or generating state.</remarks>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.Session.Commit());

        /// <summary>
        /// Configures the channels used with the session.
        /// </summary>
        /// <param name="channels">The channel(s) that all subsequent channel-based properties in the session configure.</param>
        /// <remarks>If you call this method, you must immediately call it after opening the session, and before configuring any properties or writing data. Valid values for channels are non-negative integers. For example, 0 is the only valid value on devices with one channel, while devices with two channels support  values of 0 and 1. You can specify more than one channel by inserting commas between values (for example, "0,1").</remarks>
        public void ConfigureChannels(string channels) => Parallel.ForEach(SSC, ssc => ssc.Session.ConfigureChannels(channels));

        /// <summary>
        /// Routes signals (clocks, triggers, and events) to the output terminal you specify.
        /// </summary>
        /// <param name="signalSource">The source of the signal to route.</param>
        /// <param name="signalIdentifier">The instance of the selected signal to export.</param>
        /// <param name="outputTerminal">The output terminal to export the signal.</param>
        /// <remarks>If you export a signal with this method and commit the session, the signal is routed to the output terminal you specify.</remarks>
        public void ExportSignal(SignalSource signalSource, string signalIdentifier, string outputTerminal) => Parallel.ForEach(SSC, ssc => ssc.Session.ExportSignal(signalSource, signalIdentifier, outputTerminal));

        /// <summary>
        /// Disposes the NIFgen session.
        /// </summary>
        public void Dispose() => Parallel.ForEach(SSC, ssc => ssc.Session.Dispose());

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the System.Type of the object.</param>
        /// <returns>An array of service objects of type serviceType or null if there is no service object of type serviceType.</returns>
        public object[] GetService(Type serviceType)
        {
            var service = new object[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                service[index] = SSC[index].Session.GetService(serviceType);
            });
            return service;
        }

        /// <summary>
        /// Sets the trigger mode for the signal generator.
        /// </summary>
        /// <param name="channelName">The channel name for which you want to set the trigger mode.</param>
        /// <param name="triggerMode">The trigger mode to set.</param>
        /// <remarks>
        /// Refer to Trigger Modes in the NI Signal Generators Help for descriptions of the specific behavior for supported trigger modes. The signal generator must not be in the generating state when you change this property.         
        /// </remarks>
        public void SetTriggerMode(string channelName, TriggerMode triggerMode) => Parallel.ForEach(SSC, ssc => ssc.Session.Trigger.SetTriggerMode(channelName, triggerMode));

        /// <summary>
        /// Sets the output mode of the signal generator.
        /// </summary>
        /// <param name="outputmode">The output mode of the signal generator.</param>
        /// <remarks>
        /// Valid output modes are: function, arbitrary, sequence, frequncy list and script.
        /// </remarks>
        public void ConfigureOutputMode(OutputMode outputmode) => Parallel.ForEach(SSC, ssc => ssc.Session.Output.OutputMode = outputmode);

        /// <summary>
        /// Sets the sample clock mode for the signal generator.
        /// </summary>
        /// <param name="clockMode">The sample clock mode for the signal generator.</param>
        /// <remarks> For signal generators that support it, this property allows switching the sample ClockMode.HighResolutionPartiallyQualified clocking mode. Property cannot be changed while device is generating a waveform.</remarks>
        public void ConfigureSampleClockMode(ClockMode clockMode) => Parallel.ForEach(SSC, ssc => ssc.Session.Timing.SampleClock.ClockMode = clockMode);

        /// <summary>
        /// Sets the sample clock source.
        /// </summary>
        /// <param name="clockSource">The sample clock source. The default value is "OnboardClock".</param>
        /// <remarks>Valid values: OnboardClock, ClkIn, PXI_Star, PXI_Trig0, PXI_Trig1, PXI_Trig2, PXI_Trig3, PXI_Trig4, PXI_Trig5, PXI_Trig6, PXI_Trig7 and DDC_ClkIn. Property cannot be changed while device is generating a waveform.</remarks>
        public void ConfigureSampleClockSource(string clockSource = "OnboardClock") => Parallel.ForEach(SSC, ssc => ssc.Session.Timing.SampleClock.Source = clockSource);

        /// <summary>
        /// Configures signal generator in arbitrary mode and writes out waveform data. 
        /// </summary>
        /// <param name="channelName">The channel name for which you want to set the trigger mode.</param>
        /// <param name="sampleRate">Sample rate in samples per second at which the generator generates the points in the arbitrary waveform.</param>
        /// <param name="sampleClockMode">Sets the sample clock mode for the signal generator.</param>
        /// <param name="gain">Sets the channel gain.</param>
        /// <param name="waveformData">Contains the waveform data to write.</param>
        /// <remarks>Generation is not initiated automatically, Use initiate command to start generation.</remarks>
        public void ConfigureAndWriteArbitraryWaveform(string channelName, double sampleRate, ClockMode sampleClockMode, double gain, short[] waveformData)
        {
            var waveformHandle = new int[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.Trigger.SetTriggerMode(channelName, TriggerMode.Single);
                SSC[index].Session.Output.OutputMode = OutputMode.Arbitrary;
                SSC[index].Session.Arbitrary.SampleRate= sampleRate;
                SSC[index].Session.Timing.SampleClock.ClockMode = sampleClockMode;
                SSC[index].Session.Output.SetEnabled(channelName, true);
                waveformHandle[index] = SSC[index].Session.Arbitrary.Waveform.Allocate(channelName, waveformData.Length);
                SSC[index].Session.Arbitrary.Waveform.Write(channelName, waveformHandle[index], waveformData);
                SSC[index].Session.Arbitrary.SetGain(channelName, gain);
            });
        }

        /// <summary>
        /// Configures the properties of the signal generator that affect standard waveform generation.
        /// </summary>
        /// <param name="Waveform">The standard waveform that you want the signal generator to produce.</param>
        /// <param name="channelName">The channel to be used.</param>
        /// <param name="amplitude">The peak-to-peak amplitude of the standard waveform that you want the signal generator to produce.</param>
        /// <param name="DCoffset">The DC offset of the standard waveform that you want the signal generator to produce.</param>
        /// <param name="startPhase">The horizontal offset, in degrees of one waveform cycle, of the standard waveform that you want the signal generator to produce.</param>
        /// <param name="frequency">The frequency in Hz of the standard waveform that you want the signal generator to produce.</param>
        /// <remarks>When specifying the startPhase, a start phase of 180 degrees means output generation begins halfway through the waveform cycle. A start phase of 360 degrees offsetsthe output by an entire waveform cycle and is therefore identical to a start phase of 0 degrees. The value of dcOffset is the offset at the output terminal. The value is the offset from ground to the center of the waveform that you specify with waveformFunction. For example, to configure a waveform with a peak-to-peak amplitude of 10.00 V to range from 0.00 V to +10.00 V, set dcOffset to 5.00 V.</remarks>
        public void ConfigureStandardFunctionWaveform(StandardWaveform Waveform, string channelName, double amplitude, double DCoffset, double startPhase, double frequency)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.Output.OutputMode = OutputMode.Function;
                SSC[index].Session.StandardWaveform.Configure(channelName, Waveform, amplitude, DCoffset, frequency, startPhase);
            });
        }

        /// <summary>
        /// Configures the signal generator reference clock source and frequency.
        /// </summary>
        /// <param name="clockSource">The reference clock source that you want the signal generator to use.</param>
        /// <param name="referencefrequency">The reference clock frequency in hertz (Hz).</param>
        /// <remarks>The signal generator uses the reference clock to tune the sample clock timebase of the signal generator so that the frequency, stability, and accuracy of the sample clock timebase matches that of the reference clock.</remarks>
        public void ConfigureReferenceClock(string clockSource, double referencefrequency) => Parallel.ForEach(SSC, ssc => ssc.Session.Timing.ReferenceClock.Configure(clockSource, referencefrequency));

        /// <summary>
        /// Waits until the device is done generating or until the maximum time has expired.
        /// </summary>
        /// <param name="maxTime">The timeout value in seconds.</param>
        /// <remarks>Call this method after calling InitiateGeneration.</remarks>
        public void WaitUntilDone(double maxTime) => Parallel.ForEach(SSC, ssc => ssc.Session.WaitUntilDone(TimeSpan.FromSeconds(maxTime)));
    }
}
