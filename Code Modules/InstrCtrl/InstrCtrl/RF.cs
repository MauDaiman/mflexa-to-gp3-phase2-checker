using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsa;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl
{
    public static partial class InstrCtrl
    {
        /// <summary>
        /// Sets Compensation data for the defined Pin names of the PXIe-5820 VST .
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pins"></param>
        /// <param name="compensationData"></param>
        /// <remarks>
        /// Compensation data is set per site per pin.
        /// </remarks>
        public static void Set5820Compensation(ISemiconductorModuleContext tsmContext, string[] pins, RFmx5820.CompensationData[] compensationData)
        {
            for (int i = 0; i < pins.Length; i++)
            {
                Set5820Compensation(tsmContext, pins[i], compensationData[i]);
            }
        }

        /// <summary>
        /// Sets Compensation data for the defined Pin name of the PXIe-5820 VST . 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pin"></param>
        /// <param name="compensationData"></param>
        /// <remarks>
        /// Compensation data is set per site per pin.
        /// </remarks>        /// 
        public static void Set5820Compensation(ISemiconductorModuleContext tsmContext, string pin, RFmx5820.CompensationData compensationData)
        {
            // This method must always be run from a single site subsystem
            RFmx5820.CompensationData[] compData = { compensationData };
            tsmContext.SetSiteData<RFmx5820.CompensationData>(pin, compData);
        }

        /// <summary>
        /// Generates a RFmx5820 session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pin"></param>
        /// <returns>
        /// A single RFmx5820 session
        /// </returns>
        public static RFmx5820 RFmx5820PinToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return RFmx5820PinsToSessions(tsmContext, pins);
        }

        /// <summary>
        /// Generates a series of RFmx5820 session based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pins"></param>
        /// <returns>
        /// A RFmx5820 object containing all Pin defined sessions.
        /// </returns>
        public static RFmx5820 RFmx5820PinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            RFmxInstrMX[] rfmxSessions = null;
            RFmxInstrMX[] rfmxAoSessions = null;
            Dictionary<string, NIRfmxSinglePinMultipleSessionQueryContext> PQCs = new Dictionary<string, NIRfmxSinglePinMultipleSessionQueryContext>();
            RFmx5820.CompensationData[] compensationData = new RFmx5820.CompensationData[tsmContext.SiteNumbers.Count];
            string port = "";
            string ipin = null;
            string qpin = null;

            // All pins passed should belong to the same 5820 and have identical rfmx sessions
            // Sites for each pin must be all I or all Q
            foreach (var pin in pins)
            {
                // Sample ports output: "I+ in" or  "Q+ in" 
                var pqc = tsmContext.GetNIRfmxSessions(pin, out rfmxSessions, out var pinPorts);
                var pinCompensationData = tsmContext.GetSiteData<RFmx5820.CompensationData>(pin);
                rfmxAoSessions = tsmContext.GetSiteData<RFmxInstrMX>("RFmxInstrAoSessions");
                if (Regex.IsMatch(String.Join("", pinPorts), @"I[\+\-]"))
                {
                    PQCs.Add(pin, pqc);
                    ipin = pin;
                    for (int i = 0; i < pinCompensationData.Length; i++)
                    {
                        compensationData[i].ScaleFactorI = pinCompensationData[i].ScaleFactorI;
                    }
                }
                else if (Regex.IsMatch(String.Join("", pinPorts), @"Q[\+\-]"))
                {
                    PQCs.Add(pin, pqc);
                    qpin = pin;
                    for (int i = 0; i < pinCompensationData.Length; i++)
                    {
                        compensationData[i].ScaleFactorQ = pinCompensationData[i].ScaleFactorQ;
                    }
                }
                for (int i = 0; i < pinCompensationData.Length; i++)
                {
                    compensationData[i].ExternalAttenuation = pinCompensationData[i].ExternalAttenuation;
                }
            }

            // Use pqcs to set port as I, Q, or IQ
            if (ipin != null)
                port += "I";
            if (qpin != null)
                port += "Q";

            var rfmxssc = new RFmx5820SSC[rfmxSessions.Length];
            var siteNums = tsmContext.SiteNumbers.ToArray();
            for (int i = 0; i < rfmxSessions.Length; i++)
            {
                rfmxssc[i].instrSession = rfmxSessions[i];
                rfmxssc[i].instrSessionAo = rfmxAoSessions[i];
                rfmxssc[i].specAn = rfmxSessions[i].GetSpecAnSignalConfiguration();
                rfmxssc[i].specAnAo = rfmxssc[i].instrSessionAo.GetSpecAnSignalConfiguration();
                rfmxssc[i].CompensationData = compensationData[i];
                rfmxssc[i].Port = port;
                rfmxssc[i].Index = i;
            }
            return new RFmx5820() { SSC = rfmxssc, PinQueryContexts = PQCs, SiteNumbers = siteNums, Port = port, Pins = pins, PinI = ipin, PinQ = qpin };
        }

        /// <summary>
        /// Generates a Rfsg5820 session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pin"></param>
        /// <returns>
        /// A single Rfsg5820 session
        /// </returns>
        public static Rfsg5820 Rfsg5820PinToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return Rfsg5820PinsToSessions(tsmContext, pins);
        }

        /// <summary>
        /// Generates a series of Rfsg5820 session based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pins"></param>
        /// <returns>
        /// A Rfsg5820 object containing all Pin defined sessions.
        /// </returns>
        public static Rfsg5820 Rfsg5820PinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            NIRfsg[] rfsgSessions = null;
            Dictionary<string, NIRfsgSinglePinMultipleSessionQueryContext> PQCs = new Dictionary<string, NIRfsgSinglePinMultipleSessionQueryContext>();
            string port = "";
            string pinI = "";
            string pinQ = "";

            // All pins passed should belong to the same 5820 and have identical rfmx sessions
            // Sites for each pin must be all I or all Q
            foreach (var pin in pins)
            {
                // Sample ports output: "I+ out" or  "Q- out" 
                var pqc = tsmContext.GetNIRfsgSessions(pin, out rfsgSessions, out var pinPorts);

                if (Regex.IsMatch(String.Join("", pinPorts), @"I[\+\-]"))
                {
                    PQCs.Add(pin, pqc);
                    pinI = pin;
                }
                else if (Regex.IsMatch(String.Join("", pinPorts), @"Q[\+\-]"))
                {
                    PQCs.Add(pin, pqc);
                    pinQ = pin;
                }
            }

            // Use pqcs to set port as I, Q, or IQ
            if (pinI != null)
                port += "I";
            if (pinQ != null)
                port += "Q";

            var rfsgssc = new Rfsg5820SSC[rfsgSessions.Length];
            var siteNumbers = tsmContext.SiteNumbers.ToArray();
            for (int i = 0; i < rfsgSessions.Length; i++)
            {
                rfsgssc[i].Session = rfsgSessions[i];
                rfsgssc[i].Port = port;
                rfsgssc[i].Index = i;
                rfsgssc[i].SiteNumber = siteNumbers[i];
                rfsgssc[i].SiteIndex = Array.IndexOf(siteNumbers, rfsgssc[i].SiteNumber);
            }
            return new Rfsg5820() { SSC = rfsgssc, PinQueryContexts = PQCs, SiteNumbers = siteNumbers, Pins = pins, PinI = pinI, PinQ = pinQ };
        }


        /// <summary>
        /// Generates NI-Rfsa and RFmxIntrMx sessions based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="pin"></param>
        /// <param name="rfsaSession"></param>
        /// <param name="port"></param>
        /// <param name="rfmxAnalysisSession"></param>
        /// <returns>
        /// A single NIRFsa Pin Session Query Context</returns>
        public static NIRfsaSinglePinSingleSessionQueryContext RfAnalysisPinToSessions(ISemiconductorModuleContext tsmContext, string pin,
                                                                out NIRfsa rfsaSession, out string port,
                                                                out RFmxInstrMX rfmxAnalysisSession)
        {
            var pcq = tsmContext.GetNIRfsaSession(pin, out rfsaSession, out port);
            tsmContext.GetNIRfmxSession(pin, out rfmxAnalysisSession, out _);
            return pcq;
        }

        /// <summary>
        /// Initializes all NI-RFmx sessions.
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="rfmxOptionString_HWSession"></param>
        public static void TSMInitializeRfmxSessions(ISemiconductorModuleContext tsmContext, string rfmxOptionString_HWSession)
        {
            var rfmxSessionDictionary = new Dictionary<string, RFmxInstrMX>();
            //var rfsgSessionDictionary = new Dictionary<string, NIRfsg>();

            var mbSelectedInstruments = new ModelBasedInstrumentSearchOptions();
            mbSelectedInstruments.Category = "RF";
            mbSelectedInstruments.Subcategory = "VST";

            var vstInstruments = tsmContext.GetModelBasedInstruments(mbSelectedInstruments);

            vstInstruments.Distinct().ToList().ForEach(instrument =>
            {
                if (instrument.TryGetResource("VST", out var resource))
                {
                    if (resource.TryGetPropertyValue("Resource Name", out var resourceName))
                    {
                        var instrSession = new RFmxInstrMX(resourceName, "");
                        rfmxSessionDictionary.Add(instrument.Name, instrSession);
                        //rfsgSessionDictionary.Add(instrument.Name, new NIRfsg(resourceName, false, true));
                    }
                }
            });

            foreach (var vstInst in vstInstruments)
            {
                tsmContext.SetNIRfmxSession(vstInst.Name, rfmxSessionDictionary[vstInst.Name]);
                //tsmContext.SetNIRfsgSession(vstInst.Name, rfsgSessionDictionary[vstInst.Name]);
            }
            //foreach (var rfsgSession in rfsgSessionDictionary.Values)
            //{
            //    rfsgSession.FrequencyReference.Configure(RfsgFrequencyReferenceSource.OnboardClock, 10e6);
            //    rfsgSession.Arb.PreFilterGain = -1.5;
            //    rfsgSession.Arb.WaveformSoftwareScalingFactor = 1.0 / Math.Sqrt(2);
            //    rfsgSession.Arb.GenerationMode = RfsgWaveformGenerationMode.Script;
            //    rfsgSession.RF.PowerLevelType = RfsgRFPowerLevelType.PeakPower;
            //    // Must attach 50ohm to negative terminals to operate as single ended. Software setting remains differential always.
            //    // Port level is interpreted as Vpk-pk in differential and Vpk as single ended.
            //    rfsgSession.IQOutPort[""].TerminalConfiguration = RfsgTerminalConfiguration.Differential;
            //    rfsgSession.IQOutPort.CarrierFrequency = 0;
            //}
            foreach (var instrSession in rfmxSessionDictionary.Values)
            {
                instrSession.ConfigureFrequencyReference("", RFmxInstrMXConstants.OnboardClock, 10e6);
                var specAn = instrSession.GetSpecAnSignalConfiguration();
                specAn.ConfigureFrequency("", 0);
            }
        }
        /// <summary>
        /// Initializes all NI-RF sessions.
        /// </summary>
        /// <param name="tsmContext"></param>
        /// <param name="rfmxOptionString_HWSession"></param>
        public static void TSMInitializeRfSessions(ISemiconductorModuleContext tsmContext, string rfmxOptionString_HWSession)
        {
            var rfmxSessionDictionary = new Dictionary<string, RFmxInstrMX>();
            var rfsgSessionDictionary = new Dictionary<string, NIRfsg>();

            var mbSelectedInstruments = new ModelBasedInstrumentSearchOptions();
            mbSelectedInstruments.Category = "RF";
            mbSelectedInstruments.Subcategory = "VST";

            var vstInstruments = tsmContext.GetModelBasedInstruments(mbSelectedInstruments);

            vstInstruments.Distinct().ToList().ForEach(instrument =>
            {
                if (instrument.TryGetResource("VST", out var resource))
                {
                    if (resource.TryGetPropertyValue("Resource Name", out var resourceName))
                    {
                        var instrSession = new RFmxInstrMX(resourceName, "");
                        rfmxSessionDictionary.Add(instrument.Name, instrSession);
                        rfsgSessionDictionary.Add(instrument.Name, new NIRfsg(resourceName, false, true));
                    }
                }
            });

            foreach (var vstInst in vstInstruments)
            {
                tsmContext.SetNIRfmxSession(vstInst.Name, rfmxSessionDictionary[vstInst.Name]);
                tsmContext.SetNIRfsgSession(vstInst.Name, rfsgSessionDictionary[vstInst.Name]);
            }
            foreach (var rfsgSession in rfsgSessionDictionary.Values)
            {
                rfsgSession.FrequencyReference.Configure(RfsgFrequencyReferenceSource.OnboardClock, 10e6);
                rfsgSession.Arb.PreFilterGain = -1.5;
                rfsgSession.Arb.WaveformSoftwareScalingFactor = 1.0 / Math.Sqrt(2);
                rfsgSession.Arb.GenerationMode = RfsgWaveformGenerationMode.Script;
                rfsgSession.RF.PowerLevelType = RfsgRFPowerLevelType.PeakPower;
                // Must attach 50ohm to negative terminals to operate as single ended. Software setting remains differential always.
                // Port level is interpreted as Vpk-pk in differential and Vpk as single ended.
                rfsgSession.IQOutPort[""].TerminalConfiguration = RfsgTerminalConfiguration.Differential;
                rfsgSession.IQOutPort.CarrierFrequency = 0;
            }
            foreach (var instrSession in rfmxSessionDictionary.Values)
            {
                instrSession.ConfigureFrequencyReference("", RFmxInstrMXConstants.OnboardClock, 10e6);
                var specAn = instrSession.GetSpecAnSignalConfiguration();
                specAn.ConfigureFrequency("", 0);
            }
            RFmxInstrMX[] instrAoSessions = new RFmxInstrMX[rfmxSessionDictionary.Values.Count];
            for (int i = 0; i < rfmxSessionDictionary.Values.Count; i++)
            {
                instrAoSessions[i] = new RFmxInstrMX("", "AnalysisOnly=1");
            }
            tsmContext.SetSiteData<RFmxInstrMX>("RFmxInstrAoSessions", instrAoSessions);
        }

        /// <summary>
        /// Closes all NI-RF sessions.
        /// </summary>
        /// <param name="tsmContext"></param>
        public static void TSMCloseRfmxSessions(ISemiconductorModuleContext tsmContext)
        {
            foreach (var rfmxHwSession in tsmContext.GetAllNIRfmxSessions())
            {
                rfmxHwSession.Close();
            }

            foreach (var rfmxAoSession in tsmContext.GetSiteData<RFmxInstrMX>("RFmxInstrAoSessions"))
            {
                rfmxAoSession.Close();
            }

            foreach (var niRfsgSession in tsmContext.GetAllNIRfsgSessions())
            {
                niRfsgSession.Close();
            }

            foreach (var niRfsaSession in tsmContext.GetAllNIRfsaSessions())
            {
                niRfsaSession.Close();
            }

        }

        /// <summary>
        /// Closes all NI-RF sessions (excluding RFmxInstrMX).
        /// </summary>
        /// <param name="tsmContext"></param>
        public static void TSMCloseRfSessions(ISemiconductorModuleContext tsmContext)
        {
            foreach (var rfmxHwSession in tsmContext.GetAllNIRfmxSessions())
            {
                rfmxHwSession.Close();
            }

            foreach (var niRfsgSession in tsmContext.GetAllNIRfsgSessions())
            {
                niRfsgSession.Close();
            }

            foreach (var niRfsaSession in tsmContext.GetAllNIRfsaSessions())
            {
                niRfsaSession.Close();
            }

        }
    }

    /// <summary>
    /// Structure reference to for RFmx5820SSC SSC members.
    /// </summary>
    public struct RFmx5820SSC
    {
        /// <summary>
        /// RFmxInstrMX instrSession field property.
        /// </summary>
        public RFmxInstrMX instrSession { get; set; }
        /// <summary>
        /// RFmxInstrMX instrSessionAo field property.
        /// </summary>
        public RFmxInstrMX instrSessionAo { get; set; }
        /// <summary>
        /// RFmxSpecAnMX specAn field property.
        /// </summary>
        public RFmxSpecAnMX specAn { get; set; }
        /// <summary>
        /// RFmxSpecAnMX specAnAo filed property.
        /// </summary>
        public RFmxSpecAnMX specAnAo { get; set; }
        /// <summary>
        /// RFmx5820 Compensation data struct field property.
        /// </summary>
        public RFmx5820.CompensationData CompensationData { get; set; }
        /// <summary>
        /// Port name string property.
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Index value property.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Site numbers property.
        /// </summary>
        public int[] SiteNumbers { get; set; }
        /// <summary>
        /// Site index property.
        /// </summary>
        public int[] SiteIndex { get; set; }
    }

    /// <summary>
    /// Class definition for RFmx5820 Sessions Objects.
    /// </summary>
    public class RFmx5820
    {
        /// <summary>
        /// RFmx5820 Compensation Data structure.
        /// </summary>
        public struct CompensationData
        {
            /// <summary>
            /// I scale factor used to used to compensate IQ measurement
            /// </summary>
            public double ScaleFactorI;
            /// <summary>
            /// Q scale factor used to compensate IQ measurement
            /// </summary>
            public double ScaleFactorQ;
            /// <summary>
            /// Used to compensate Spectrum measurement
            /// </summary>
            public double ExternalAttenuation;
        }

        /// <summary>
        /// RFmx Settings structure.
        /// </summary>
        public struct RFmxSettings
        {
            /// <summary>
            /// Reference Level.
            /// </summary>
            public double ReferenceLevel;
            /// <summary>
            /// Digital Trigger.
            /// </summary>
            public bool DigitalTrigger;
            /// <summary>
            /// Trigger Delay.
            /// </summary>
            public double TriggerDelay;
            /// <summary>
            /// Measurement type.
            /// </summary>
            public string Measurement;
            /// <summary>
            /// IQ settings.
            /// </summary>
            public IqSettings IQ;
            /// <summary>
            /// Spectrum settings.
            /// </summary>
            public SpectrumSettings Spectrum;
        }
        /// <summary>
        /// IQ settings structure.
        /// </summary>
        public struct IqSettings
        {
            /// <summary>
            /// Sampling rate.
            /// </summary>
            public double SampleRate;
            /// <summary>
            /// Measurement interval
            /// </summary>
            public double MeasurementInterval;
        }

        /// <summary>
        /// Spectrum settings structure.
        /// </summary>
        public struct SpectrumSettings
        {
            /// <summary>
            /// Start value.
            /// </summary>
            public double Start;
            /// <summary>
            /// Stop value.
            /// </summary>
            public double Stop;
            /// <summary>
            /// Sweep time
            /// </summary>
            public double SweepTime;
            /// <summary>
            /// Resoplution Bandwidth in Hertz.
            /// </summary>
            public double RBW;
        }

        /// <summary>
        /// NI-RFmx results structure.
        /// </summary>
        public struct RFmxResults
        {
            /// <summary>
            /// IQ results.
            /// </summary>
            public IqResults IQ;
            /// <summary>
            /// Spectrum Results.
            /// </summary>
            public SpectrumResults Spectrum;
        }

        /// <summary>
        /// IQ data results structure.
        /// </summary>
        public struct IqResults
        {
            /// <summary>
            /// Sample Number.
            /// </summary>
            public Dictionary<string, double[]> Samples;
        }

        /// <summary>
        /// Spectrum results structure.
        /// </summary>
        public struct SpectrumResults
        {
            /// <summary>
            /// Actual Resolution Bandwidth.
            /// </summary>
            public double actualRBW;
            /// <summary>
            /// Actual Sweep time.
            /// </summary>
            public double actualSweepTime;
            /// <summary>
            /// Spectrum trace.
            /// </summary>
            public Spectrum<float> SpectrumTrace;
            /// <summary>
            /// Total Power AoSpectrum.
            /// </summary>
            public double TotalPower_AoSpectrum;
            /// <summary>
            /// Total Power AoIQ.
            /// </summary>
            public double TotalPower_AoIq;
            /// <summary>
            /// Power Spectral Density AoSpectrum.
            /// </summary>
            public double PowerSpectralDensity_AoSpectrum;
            /// <summary>
            /// Power Spectral Density AoIQ.
            /// </summary>
            public double PowerSpectralDensity_AoIq;
            /// <summary>
            /// Peak Amplitude.
            /// </summary>
            public double PeakAmplitude;
            /// <summary>
            /// Peak Amplitude frequency.
            /// </summary>
            public double PeakAmplitudeFrequency;
        }
        /// <summary>
        /// Dictionaty definition for PinQueryContexts
        /// </summary>
        public Dictionary<string, NIRfmxSinglePinMultipleSessionQueryContext> PinQueryContexts { get; set; }
        /// <summary>
        /// RFmx5820 SSC definition.
        /// </summary>
        public RFmx5820SSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Pins I.
        /// </summary>
        public string PinI { get; set; }
        /// <summary>
        /// Pins Q.
        /// </summary>
        public string PinQ { get; set; }
        /// <summary>
        /// Port name.
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Site Numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }

        /// <summary>
        /// Stops acquisition and measurements associated with signal instance, which were previously initiated by the Initiate or measurement read methods.
        /// </summary>
        /// <remarks>
        /// Calling this method is optional, unless you want to stop a measurement before it is complete. This method executes even if there is an incoming error.
        /// </remarks>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.specAn.AbortMeasurements(""));

        /// <summary>
        /// Initiates all enabled measurements. Call this method after configuring the signal and measurement. This method asynchronously launches measurements in the background and immediately returns to the caller program.
        /// </summary>
        public void Initiate() => Parallel.ForEach(SSC, ssc => ssc.specAn.Initiate("", ""));

        /// <summary>
        /// Waits for the specified number for seconds for all the measurements to complete.
        /// </summary>
        public void WaitForMeasurementComplete() => Parallel.ForEach(SSC, ssc => ssc.specAn.WaitForMeasurementComplete("", 5));

        /// <summary>
        /// Commits settings to the hardware. Calling this method is optional. RFmxSpecAn commits settings to the hardware when you call the RFmxInstrMX Initiate or any of the measurement Read methods.
        /// </summary>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.specAn.Commit(""));

        /// <summary>
        /// Deletes all the named signal configurations in the session and resets all methods for the default signal instances of already loaded personalities in the session. This method disables all the calibration planes.
        /// </summary>
        public void Reset() => Parallel.ForEach(SSC, ssc => ssc.instrSession.ResetEntireSession());

        /// <summary>
        /// Configures NI-RFmx sessions using parameters defined in RFmxSettings file and put sessions into a Commited state.
        /// </summary>
        /// <param name="rfmxSettings"></param>
        /// <exception cref="Exception" ></exception>
        /// <remarks>
        /// RFmxSettings struct contains configurations for:<br/>
        /// •ReferenceLevel<br/>
        /// •DigitalTrigger<br/>
        /// •TriggerDelay<br/>
        /// •Measurement<br/>
        /// •IqSettings<br/>
        /// •SpectrumSettings
        /// </remarks>
        public void Configure(RFmxSettings rfmxSettings)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                // Configure direct hardware settings
                ssc.specAn.SetReferenceLevel("", rfmxSettings.ReferenceLevel);
                ssc.specAn.ConfigureDigitalEdgeTrigger("", RFmxInstrMXConstants.PxiTriggerLine0, RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising, rfmxSettings.TriggerDelay, rfmxSettings.DigitalTrigger);

                // Configure measurement specific settings
                switch (rfmxSettings.Measurement)
                {
                    case "IQ":
                        ssc.specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, false);
                        ssc.specAn.SetCenterFrequency("", 0);
                        ssc.specAn.SetExternalAttenuation("", 0); // rely on scale values for IQ measurement
                        ssc.specAn.IQ.Configuration.ConfigureAcquisition("", rfmxSettings.IQ.SampleRate, 1, rfmxSettings.IQ.MeasurementInterval, 0);
                        break;
                    case "Spectrum":
                        // Analysis only configure
                        var centerFreq = (rfmxSettings.Spectrum.Start + rfmxSettings.Spectrum.Stop) / 2;
                        ssc.specAnAo.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Chp, true);
                        ssc.specAnAo.ConfigureFrequency("", 0); // cannot use anything but 0 if we intend to use IQ record (avoid mixing unused channel into capture)
                        ssc.specAnAo.Chp.Configuration.ConfigureCarrierOffset("", centerFreq);
                        ssc.specAnAo.Chp.Configuration.ConfigureIntegrationBandwidth("", rfmxSettings.Spectrum.Stop - rfmxSettings.Spectrum.Start);
                        ssc.specAnAo.Chp.Configuration.ConfigureSpan("", rfmxSettings.Spectrum.Stop - rfmxSettings.Spectrum.Start);
                        // Use auto settings if 0 or negative number is passed
                        var rbwAutoAo = rfmxSettings.Spectrum.RBW <= 0 ? RFmxSpecAnMXChpRbwAutoBandwidth.True : RFmxSpecAnMXChpRbwAutoBandwidth.False;
                        var sweepAutoAo = rfmxSettings.Spectrum.SweepTime <= 0 ? RFmxSpecAnMXChpSweepTimeAuto.True : RFmxSpecAnMXChpSweepTimeAuto.False;
                        ssc.specAnAo.Chp.Configuration.SetRbwFilterAutoBandwidth("", rbwAutoAo);
                        ssc.specAnAo.Chp.Configuration.SetSweepTimeAuto("", sweepAutoAo);
                        if (rbwAutoAo == RFmxSpecAnMXChpRbwAutoBandwidth.False) { ssc.specAnAo.Chp.Configuration.SetRbwFilterBandwidth("", rfmxSettings.Spectrum.RBW); }
                        if (sweepAutoAo == RFmxSpecAnMXChpSweepTimeAuto.False) { ssc.specAnAo.Chp.Configuration.ConfigureSweepTime("", sweepAutoAo, rfmxSettings.Spectrum.SweepTime); }
                        ssc.specAnAo.Commit("");

                        // Get recommended settings
                        //ssc.specAnAo.Chp.Configuration.GetSweepTimeInterval("", out double reccommendedSweepTime); // Align sweep time to what AO wants to use
                        ssc.instrSessionAo.GetRecommendedNumberOfRecords("", out int numRecords);
                        ssc.instrSessionAo.GetRecommendedSpectralAcquisitionSpan("", out double recommendedSpan);
                        //ssc.instrSessionAo.GetRecommendedSpectralFftWindow("", out RFmxInstrMXRecommendedSpectralFftWindow recommendedFftWindow);
                        //ssc.instrSessionAo.GetRecommendedSpectralResolutionBandwidth("", out double reccommendedRbw);
                        ssc.instrSessionAo.GetRecommendedCenterFrequency("", out double recommendedCenterFrequency);
                        ssc.instrSessionAo.GetRecommendedIQAcquisitionTime("", out double recommendedAcquisitionTime);
                        ssc.instrSessionAo.GetRecommendedIQMinimumSampleRate("", out double recommendedSampleRate);

                        // translate to instrument session
                        rfmxSettings.Spectrum.SweepTime = recommendedAcquisitionTime;
                        //rfmxSettings.Spectrum.RBW = reccommendedRbw;
                        // Hardware session configure
                        //ssc.specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ | RFmxSpecAnMXMeasurementTypes.Spectrum, true);
                        ssc.specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.IQ, true);
                        ssc.specAn.SetExternalAttenuation("", ssc.CompensationData.ExternalAttenuation);
                        if (recommendedCenterFrequency != 0)
                        {
                            throw new Exception($"Center frequency must be zero. Requested value: {recommendedCenterFrequency}");
                        }
                        ssc.specAn.SetCenterFrequency("", recommendedCenterFrequency); // MUST be zero
                        //ssc.specAn.Spectrum.Configuration.SetSpan("", recommendedSpan);
                        //ssc.specAn.Spectrum.Configuration.SetFftWindow("", RFmxSpecAnMXSpectrumFftWindow.FlatTop); // for best amplitude accuracy
                        ssc.specAn.IQ.Configuration.ConfigureAcquisition("", recommendedSampleRate, 1, recommendedAcquisitionTime, 0);
                        // Use auto settings if 0 or negative number is passed
                        //var rbwAuto = rfmxSettings.Spectrum.RBW <= 0 ? RFmxSpecAnMXSpectrumRbwAutoBandwidth.True : RFmxSpecAnMXSpectrumRbwAutoBandwidth.False;
                        //var sweepAuto = rfmxSettings.Spectrum.SweepTime <= 0 ? RFmxSpecAnMXSpectrumSweepTimeAuto.True : RFmxSpecAnMXSpectrumSweepTimeAuto.False;
                        //ssc.specAn.Spectrum.Configuration.SetRbwFilterAutoBandwidth("", rbwAuto);
                        //if (rbwAuto == RFmxSpecAnMXSpectrumRbwAutoBandwidth.False) { ssc.specAn.Spectrum.Configuration.SetRbwFilterBandwidth("", rfmxSettings.Spectrum.RBW); }
                        //if (sweepAuto == RFmxSpecAnMXSpectrumSweepTimeAuto.False) { ssc.specAn.Spectrum.Configuration.ConfigureSweepTime("", sweepAuto, rfmxSettings.Spectrum.SweepTime); }
                        //switch (ssc.Port)
                        //{
                        //    case "I":
                        //        ssc.specAn.Spectrum.Configuration.SetAnalysisInput("", RFmxSpecAnMXSpectrumAnalysisInput.IOnly);
                        //        break;
                        //    case "Q":
                        //        ssc.specAn.Spectrum.Configuration.SetAnalysisInput("", RFmxSpecAnMXSpectrumAnalysisInput.QOnly);
                        //        break;
                        //    default:
                        //        throw new NotImplementedException($"Analysis input must be I or Q. Argument passed: {ssc.Port}.");
                        //}
                        break;
                }
                ssc.specAn.Commit("");
            });
        }

        /// <summary>
        /// Fetches I/Q data from a single record in an acquisition for all the sessions.
        /// </summary>
        /// <param name="rfmxSettings"></param>
        /// <param name="results"></param>
        /// <returns>
        /// An array of RFmxResults containing results fetched.
        /// </returns>
        /// <remarks>
        /// Available measurements: <br/>
        /// • IQ<br/>
        /// • Spectrum
        /// </remarks>
        public void Fetch(RFmxSettings rfmxSettings, out RFmxResults[] results)
        {
            double timeout = 1;
            var localResults = new RFmxResults[SSC.Length];
            Parallel.ForEach(SSC, (ssc, state, index) =>
            {
                localResults[index].IQ.Samples = new Dictionary<string, double[]>();
                switch (rfmxSettings.Measurement)
                {
                    case "IQ":
                        ComplexWaveform<ComplexSingle> IqWfm = null;
                        ssc.specAn.IQ.Results.FetchData("", timeout, 0, -1, ref IqWfm);
                        if (PinI != null)
                        {
                            var tempSamples = IqWfm.GetRealDataArray(false);
                            for (int i = 0; i < tempSamples.Length; i++)
                            {
                                tempSamples[i] = tempSamples[i] * ssc.CompensationData.ScaleFactorI;
                            }
                            localResults[index].IQ.Samples.Add(PinI, tempSamples);
                        }
                        if (PinQ != null)
                        {
                            var tempSamples = IqWfm.GetImaginaryDataArray(false);
                            for (int i = 0; i < tempSamples.Length; i++)
                            {
                                tempSamples[i] = tempSamples[i] * ssc.CompensationData.ScaleFactorQ;
                            }
                            localResults[index].IQ.Samples.Add(PinQ, tempSamples);
                        }
                        break;
                    case "Spectrum":
                        //Spectrum<float> spectrum = null;
                        //ssc.specAn.Spectrum.Results.FetchSpectrum("", timeout, ref spectrum);
                        ComplexWaveform<ComplexSingle> iqData = null;
                        ssc.specAn.IQ.Results.FetchData("", timeout, 0, -1, ref iqData);
                        //ssc.specAn.Spectrum.Configuration.GetRbwFilterBandwidth("", out localResults[index].Spectrum.actualRBW);
                        //ssc.specAn.Spectrum.Configuration.GetSweepTimeInterval("", out localResults[index].Spectrum.actualSweepTime);
                        //ssc.specAn.Spectrum.Results.GetPeakAmplitude("", out localResults[index].Spectrum.PeakAmplitude);
                        //ssc.specAn.Spectrum.Results.GetPeakFrequency("", out localResults[index].Spectrum.PeakAmplitudeFrequency);

                        // Make analysis only measurement with spectrum
                        //ssc.specAnAo.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Chp, true); // re-select to avoid RFmx error
                        //ssc.specAnAo.AnalyzeSpectrum1Waveform("", "", spectrum, true, 0);
                        //ssc.specAnAo.Chp.Results.FetchCarrierMeasurement("", timeout, out localResults[index].Spectrum.TotalPower_AoSpectrum,
                        //                                                              out localResults[index].Spectrum.PowerSpectralDensity_AoSpectrum,
                        //                                                              out double relativePower);

                        // Make analysis only measurement with iq
                        ssc.specAnAo.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Chp, true); // re-select to avoid RFmx error
                        double[] doubleData;
                        double relativePower;
                        switch (ssc.Port)
                        {
                            default:
                            case "I":
                                doubleData = iqData.GetRealDataArray(false);
                                break;
                            case "Q":
                                doubleData = iqData.GetImaginaryDataArray(false);
                                break;
                        }
                        float[] floatData = new float[doubleData.Length];
                        for (int i = 0; i < doubleData.Length; i++) { floatData[i] = Convert.ToSingle(doubleData[i]); }
                        float[] zeros = new float[floatData.Length];
                        var singleChannelWaveform = ComplexWaveform<ComplexSingle>.FromArray1D(ComplexSingle.ComposeArray(floatData, zeros));
                        singleChannelWaveform.PrecisionTiming = iqData.PrecisionTiming;
                        ssc.specAnAo.AnalyzeIQ1Waveform("", "", singleChannelWaveform, true, 0);
                        ssc.specAnAo.Chp.Results.FetchCarrierMeasurement("", timeout, out localResults[index].Spectrum.TotalPower_AoIq,
                                                                                      out localResults[index].Spectrum.PowerSpectralDensity_AoIq,
                                                                                      out relativePower);
                        ssc.specAnAo.Chp.Results.FetchSpectrum("", timeout, ref localResults[index].Spectrum.SpectrumTrace);
                        ssc.specAnAo.Chp.Configuration.GetRbwFilterBandwidth("", out localResults[index].Spectrum.actualRBW);
                        rfmxSettings.Spectrum.RBW = localResults[index].Spectrum.actualRBW;
                        // Add 6.02dB to result to account for SSB vs DSB
                        localResults[index].Spectrum.TotalPower_AoIq += 10 * Math.Log10(4);
                        localResults[index].Spectrum.PowerSpectralDensity_AoIq += 10 * Math.Log10(4);
                        break;
                }
            });
            results = localResults;
        }
    }

    /// <summary>
    /// Structure reference to for Rfsg5820SSC SSC members.
    /// </summary>
    public struct Rfsg5820SSC
    {
        /// <summary>
        /// NI-RFsg Session field property.
        /// </summary>
        public NIRfsg Session { get; set; }
        /// <summary>
        /// Port name string property.
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// Index value property
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Site number value property.
        /// </summary>
        public int SiteNumber { get; set; }
        /// <summary>
        /// Site index value property.
        /// </summary>
        public int SiteIndex { get; set; }
    }

    /// <summary>
    /// Class definition for RFsg5820 Sessions Objects.
    /// </summary>
    public class Rfsg5820
    {
        /// <summary>
        /// RFsg Settings Data Structure.
        /// </summary>
        public struct RfsgSettings
        {
            /// <summary>
            /// Voltage Level value.
            /// </summary>
            public double VoltageLevel;                            //Vpp
            /// <summary>
            /// Common Mode Offset value.
            /// </summary>
            public double CommonModeOffset;                      //V
            /// <summary>
            /// Differential Offset value.
            /// </summary>
            public double DifferentialOffset;                      //V
            /// <summary>
            /// External Attenuation value.
            /// </summary>
            public double ExternalAttenuation;                   //dB
            /// <summary>
            /// IQ Rate value.
            /// </summary>
            public double IQRate;
            /// <summary>
            /// Waveform Name.
            /// </summary>
            public string WaveformName;                          // = "waveform"
            /// <summary>
            /// Loop intinitely.
            /// </summary>
            public bool loop;                                   // true to loop infinitely
        }
        /// <summary>
        /// Dictionaty definition for PinQueryContexts
        /// </summary>
        public Dictionary<string, NIRfsgSinglePinMultipleSessionQueryContext> PinQueryContexts { get; set; }
        /// <summary>
        /// Rfsg5820SSC SSC definition.
        /// </summary>
        public Rfsg5820SSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Pin I name.
        /// </summary>
        public string PinI { get; set; }
        /// <summary>
        /// Pin Q name.
        /// </summary>
        public string PinQ { get; set; }
        /// <summary>
        /// Site Numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }        
        /// <summary>
        /// Stops signal generation.
        /// </summary>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.Session.Abort());

        /// <summary>
        /// Initiates signal generation, causing the NI-RFSG device to leave the Configuration state and enter the Generation state.
        /// </summary>
        public void Initiate() => Parallel.ForEach(SSC, ssc => ssc.Session.Initiate());

        /// <summary>
        /// Asserts the configured hardware parameters.
        /// </summary>
        /// <remarks>
        /// This method verifies property values, reserves the device, and commits the property values to the device. If the property values are all valid, the device configuration matches the session configuration. Calling this method moves the NI-RFSG device from the Configuration state to Committed state. After calling this method, changing any property reverts the NI-RFSG device to the Configuration state.
        /// </remarks>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.Commit());

        /// <summary>
        /// Resets all properties to their default values and moves the NI-RFSG device to the Configuration state.
        /// </summary>
        /// <remarks>
        /// This method aborts the generation, clears all routes, and resets session properties to the initial values. During a reset, routes of signals between this and other devices are released, regardless of which device created the route.
        /// </remarks>
        public void Reset() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.Reset());

        /// <summary>
        /// Performs a hard reset on the device by performing a set of actions.
        /// </summary>
        /// <remarks>
        /// The following actions are performed during hard reset:<br/> 
        /// • Signal generation is stopped.<br/> 
        /// • All routes are released.<br/> 
        /// • External bidirectional terminals are tristated.<br/> 
        /// • FPGAs are reset.<br/> 
        /// • Hardware is configured to its default state.<br/> 
        /// • All session properties are reset to their default states.
        /// <para>During a reset, routes of signals between this and other devices are released, regardless of which device created the route.</para>
        /// </remarks>
        public void ResetDevice() => Parallel.ForEach(SSC, ssc => ssc.Session.Utility.ResetDevice());

        /// <summary>
        /// Configures NI-RFmx sessions using parameters defined in RfsgSettings file and puts sessions into a Commited state.
        /// </summary>
        /// <param name="rfsgSettings"></param>
        /// <remarks>
        /// rfsgSettings struct contains configurations for:<br/>
        /// •VoltageLevel<br/>
        /// •CommonModeOffset<br/>
        /// •DifferentialOffset<br/>
        /// •ExternalAttenuation<br/>
        /// •IQRate<br/>
        /// •WaveformName<br/>
        /// •Loop (true enables looping)
        /// </remarks>
        public void Configure(RfsgSettings rfsgSettings)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                // Stop generator if it has been left playing
                try
                {
                    ssc.Session.Abort();
                }
                catch (Exception) { }

                // Set channel name as empty string for 5820. Units are Vpk-pk. Scale by sqrt(2) to undo software scaling of 1/sqrt(2)
                ssc.Session.IQOutPort[""].Level = Math.Abs(rfsgSettings.VoltageLevel) * Math.Sqrt(2);
                ssc.Session.IQOutPort[""].CommonModeOffset = rfsgSettings.CommonModeOffset;
                ssc.Session.IQOutPort[""].Offset = rfsgSettings.DifferentialOffset;
                ssc.Session.Arb.IQRate = rfsgSettings.IQRate;
                ssc.Session.RF.ExternalGain = -rfsgSettings.ExternalAttenuation;

                string loop = rfsgSettings.loop ? "loop" : "";
                string scriptName = $"{rfsgSettings.WaveformName}{ssc.Port}{loop}script"; // Example: ARB10Q50RBTDDIQloopscript
                ssc.Session.Arb.Scripting.SelectedScriptName = scriptName;
                ssc.Session.Utility.Commit();
            });
        }
    }

}