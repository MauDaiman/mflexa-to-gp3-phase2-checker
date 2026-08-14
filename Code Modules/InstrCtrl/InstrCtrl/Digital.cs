using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// Generates a NI-Digital session based on the provided Pin name and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name string</param>
        /// <returns>
        /// A single NI-Digital session
        /// </returns>
        public static Digital DigitalPinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return DigitalPinsToSessions(tsmContext, pins);
        }
        /// <summary>
        /// Generates a series of NI-Digital session based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A NI-Digital object containing all Pin defined sessions.
        /// </returns>
        public static Digital DigitalPinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetNIDigitalPatternSessions(pins, out var digitalSessions, out var digitalChannelLists, out var digitalSiteLists);

            var digitalssc = new DigitalSSC[digitalSessions.Length];

            var siteNums = tsmContext.SiteNumbers.ToArray();

            //for (int i = 0; i < digitalSessions.Length; i++)
            //{
            //    digitalssc[i].Session = digitalSessions[i];
            //    digitalssc[i].ChannelList = digitalChannelLists[i];
            //    digitalssc[i].SiteList = digitalSiteLists[i];
            //    digitalssc[i].SiteNumbers = Array.ConvertAll(Regex.Split(digitalSiteLists[i], @"\D+").Where(w => !String.IsNullOrEmpty(w)).ToArray(), int.Parse);
            //    digitalssc[i].PinSet = digitalSessions[i].PinAndChannelMap.GetPinSet(digitalChannelLists[i]);
            //    digitalssc[i].Index = i;
            //    digitalssc[i].SiteIndex = new int[digitalssc[i].SiteNumbers.Length];
            //    for (int j = 0; j < digitalssc[i].SiteNumbers.Length; j++) digitalssc[i].SiteIndex[j] = Array.IndexOf(siteNums, digitalssc[i].SiteNumbers[j]);
            //}

            Parallel.For(0, digitalSessions.Length, i =>
           {
               digitalssc[i].Session = digitalSessions[i];
               digitalssc[i].ChannelList = digitalChannelLists[i];
               digitalssc[i].SiteList = digitalSiteLists[i];
               digitalssc[i].SiteNumbers = Array.ConvertAll(Regex.Split(digitalSiteLists[i], @"\D+").Where(w => !String.IsNullOrEmpty(w)).ToArray(), int.Parse);
               digitalssc[i].PinSet = digitalSessions[i].PinAndChannelMap.GetPinSet(digitalChannelLists[i]);
               digitalssc[i].Index = i;
               digitalssc[i].SiteIndex = new int[digitalssc[i].SiteNumbers.Length];
               Parallel.For(0, digitalssc[i].SiteNumbers.Length, j =>
              {
                  digitalssc[i].SiteIndex[j] = Array.IndexOf(siteNums, digitalssc[i].SiteNumbers[j]);
              });
           });

            return new Digital() { SSC = digitalssc, PinQueryContext = pqc, SiteNumbers = siteNums, Pins = pins };
        }

        /// <summary>
        /// Initializes all NI-Digital sessions. Reset device, loads pinMap, specs, levels and timing files (files are loaded from defined paths).
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitDigitalSessions(ISemiconductorModuleContext tsmContext)
        {
            var digitalInstruments = tsmContext.GetNIDigitalPatternInstrumentNames();

            var sessions = new List<NIDigital>();

            for (int i = 0; i < digitalInstruments.Length; i++)
            {
                var session = new NIDigital(digitalInstruments[i], false, false);
                sessions.Add(session);
                tsmContext.SetNIDigitalPatternSession(digitalInstruments[i], session);
            }

            Parallel.ForEach(sessions, session =>
            //foreach (var session in sessions)
            {
                session.Utility.Reset();
                session.LoadPinMap(tsmContext.PinMapFilePath);
                session.LoadSpecifications(tsmContext.DigitalPatternProjectSpecificationsFilePaths);
                //session.LoadLevels(tsmContext.DigitalPatternProjectLevelsFilePaths);
                session.LoadLevels(tsmContext.DigitalPatternProjectLevelsFilePaths);
                session.LoadTiming(tsmContext.DigitalPatternProjectTimingFilePaths);
                if (tsmContext.DigitalPatternProjectLevelsFilePaths.Count > 0 && tsmContext.DigitalPatternProjectTimingFilePaths.Count > 0)
                {
                    session.ApplyLevelsAndTiming("", tsmContext.DigitalPatternProjectLevelsFilePaths.FirstOrDefault(), tsmContext.DigitalPatternProjectTimingFilePaths.FirstOrDefault());
                }
                foreach (string patternPath in tsmContext.DigitalPatternProjectPatternFilePaths) session.LoadPattern(patternPath);
                foreach (string captureWaveform in tsmContext.DigitalPatternProjectCaptureWaveformFilePaths) session.CaptureWaveforms.CreateFromFile(Path.GetFileNameWithoutExtension(captureWaveform), captureWaveform);
                foreach (string sourceWaveform in tsmContext.DigitalPatternProjectSourceWaveformFilePaths) session.SourceWaveforms.CreateFromFile(Path.GetFileNameWithoutExtension(sourceWaveform), sourceWaveform, false);
            });
        }

        /// <summary>
        /// Closes all NI-Digital sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseDigitalSessions(ISemiconductorModuleContext tsmContext)
        {
            foreach (var session in tsmContext.GetAllNIDigitalPatternSessions()) session.Close();
        }

        /// <summary>
        /// Converts unsigned integer data to a double type.
        /// </summary>
        /// <param name="data">Unsigned integer data to convert.</param>
        /// <returns>
        /// An array of converted data.
        /// </returns>
        public static double[][] ConvertUinttoDouble(uint[][] data)
        {
            if (data == null) return null;

            var output = new double[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] = Array.ConvertAll<uint, double>(data[i], Convert.ToDouble);
            }
            return output;
        }

        /// <summary>
        /// Gets the specified ammount of sample data from the captured waveform.
        /// </summary>
        /// <typeparam name="T">List value with single Capture values</typeparam>
        /// <param name="captureWaveform">Capture Waveform data.</param>
        /// <param name="sampleToGet">Samples of the data to get.</param>
        /// <returns>
        /// Single captured sample data from the captured waveform.
        /// </returns>
        public static T[][] GetSingleSampleFromPerInstData<T>(T[][][] captureWaveform, int sampleToGet = 0)
        {
            var singleCaptures = new T[captureWaveform.Length][];
            for (int i = 0; i < captureWaveform.Length; i++)
            {
                singleCaptures[i] = new T[captureWaveform[i].Length];
                for (int j = 0; j < captureWaveform[i].Length; j++)
                {
                    singleCaptures[i][j] = captureWaveform[i][j][sampleToGet];
                }
            }
            return singleCaptures;
        }
    }

    /// <summary>
    /// Structure reference to for Digital SSC members
    /// </summary>
    public struct DigitalSSC
    {
        /// <summary>
        /// NI-Digital session field property.
        /// </summary>
        public NIDigital Session { get; set; }
        /// <summary>
        /// ChannelList string property.
        /// </summary>
        public string ChannelList { get; set; }
        /// <summary>
        /// Site List string property. 
        /// </summary>
        public string SiteList { get; set; }
        /// <summary>
        /// Site Numbers array.
        /// </summary>
        public int[] SiteNumbers { get; set; }
        /// <summary>
        /// Site index array.
        /// </summary>
        public int[] SiteIndex { get; set; }
        /// <summary>
        /// Digital Pin Set file from session.
        /// </summary>
        public DigitalPinSet PinSet { get; set; }
        /// <summary>
        /// Index of session within object.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// Class definition for Digital Sessions Objects.
    /// </summary>
    public class Digital
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public NIDigitalPatternPinQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// Digital SSC definition.
        /// </summary>
        public DigitalSSC[] SSC { get; set; }
        /// <summary>
        /// Site numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }

        /// <summary>
        /// Stops clock generation on the specified channel(s) or pin(s) and pin group(s) retrieved from the current session.
        /// </summary>
        public void ClockGeneratorAbort() => Parallel.ForEach(SSC, ssc => ssc.PinSet.ClockGenerator.Abort());

        /// <summary>
        /// Initiates clock generation on the specified channel(s) or pin(s) and pin group(s) retrieved from the current session.
        /// </summary>
        public void ClockGeneratorInitiate() => Parallel.ForEach(SSC, ssc => ssc.PinSet.ClockGenerator.Initiate());

        /// <summary>
        /// Configures and initiates clock generation on the specified channel(s), or pin(s) and pin group(s).
        /// </summary>`
        /// <param name="frequencyinHz">Clock frequency value in Hz.</param>
        /// <param name="selectDigitalFunction">If true, sets the Selected Funciton of the pins to Digital.</param>
        public void ClockGeneratorGenerateClock(double frequencyinHz, bool selectDigitalFunction = true) => Parallel.ForEach(SSC, ssc => ssc.PinSet.ClockGenerator.GenerateClock(frequencyinHz, selectDigitalFunction));
        
        /// <summary>
        /// Gets or sets the instrument function of this pin set. The changes take effect immediately. 
        /// </summary>
        /// <param name="selectedFunction">Instrument function for the pins.</param>
        public void SelectFunction(SelectedFunction selectedFunction) => Parallel.ForEach(SSC, ssc => ssc.PinSet.SelectedFunction = selectedFunction);
       
        /// <summary>
        /// Configures the NI DigitalFrequencyCounter measurement time for the frequency measurement.
        /// </summary>
        /// <param name="measureTime">Measurement time for the frequency measurements (Precision Time Span format).</param>
        /// <remarks>
        /// Measurement time is provided as a PrecisionTimeSpan value from IVI driver.
        /// </remarks>
        public void FrequencyCounterConfigureMeasurementTime(Ivi.Driver.PrecisionTimeSpan measureTime) => Parallel.ForEach(SSC, ssc => ssc.PinSet.FrequencyCounter.MeasurementTime = measureTime);

        /// <summary>
        /// Configures the NI DigitalFrequencyCounter measurement time for the frequency measurement.
        /// </summary>
        /// <param name="measureTime">Measurement time for the frequency measurements (decimal format).</param>
        /// <remarks>
        /// Measurement time automatically converted from decimal to a PrecisionTimeSpan value from IVI driver.
        /// </remarks>
        public void FrequencyCounterConfigureMeasurementTime(decimal measureTime) => FrequencyCounterConfigureMeasurementTime(new Ivi.Driver.PrecisionTimeSpan(measureTime));

        /// <summary>
        /// Measures the frequency on the specified pins over the measurement time. All pins in the pin list should have the same measurement time.   
        /// </summary>
        /// <returns>
        /// The measurements taken, ordered according to the pinSetString parameter. If a site is disabled, the method does not return data for that site.
        /// </returns>        
        public double[][] FrequencyCounterMeasureFrequency()
        {
            double[][] measurements = new double[SSC.Length][];
            Parallel.ForEach(SSC, ssc => measurements[ssc.Index] = ssc.PinSet.FrequencyCounter.MeasureFrequency());
            return measurements;
        }

        /// <summary>
        /// Stops bursting the pattern.
        /// </summary>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.Session.PatternControl.Abort());

        /// <summary>
        /// Loads a pattern to the hardware from a pattern file.
        /// </summary>
        /// <param name="patternPath">The absolute file path to the pattern file.</param>
        /// <remarks>
        /// Pattern file is read from the specified path.
        /// </remarks>
        public void LoadPattern(string patternPath) => Parallel.ForEach(SSC, ssc => ssc.Session.LoadPattern(patternPath));

        /// <summary>
        /// Loads a pattern to the hardware from a pattern file list.
        /// </summary>
        /// <param name="patternPaths">List of the absolute file path to the pattern file.</param>
        /// <remarks>
        /// Pattern list contains multiple specified pattern paths.
        /// </remarks>
        public void LoadPatterns(List<string> patternPaths)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                foreach (var pattern in patternPaths) ssc.Session.LoadPattern(pattern);
            });
        }

        /// <summary>
        /// Starts a pattern burst and optionally blocks execution until the executions are completed.
        /// </summary>
        /// <param name="startLabel">The pattern name or exported pattern label from which to start bursting the pattern.</param>
        /// <param name="selectDigitalFunction">If true, sets the Selected Funciton of the pins to Digital.</param>
        /// <param name="waitUntilDone">If true, waits until the pattern burst is completed; otherwise, initiates a pattern burst and returns.</param>
        /// <param name="timeoutinSeconds">The maximum time interval allowed for the pattern burst to complete.</param>
        /// <remarks>
        /// waitUntilDone parameter lets you select whether you want the method to wait until the pattern burst has completed before returning or the specified maxTime has elapsed. To get results after calling this method, you must call NationalInstruments.ModularInstruments.NIDigital.DigitalPatternControl.GetSitePassFail(System.String). For more information, refer to the Session State Model topic of the Digital Pattern help.
        /// </remarks>
        public void BurstPattern(string startLabel, bool selectDigitalFunction = true, bool waitUntilDone = true, double timeoutinSeconds = 5.0) =>
            Parallel.ForEach(SSC, ssc => ssc.Session.PatternControl.BurstPattern(ssc.SiteList, startLabel, selectDigitalFunction, waitUntilDone, TimeSpan.FromSeconds(timeoutinSeconds)));

        /// <summary>
        /// Returns a value indicating whether the specified sites passed the comparisons in the pattern burst. 
        /// </summary>
        /// <returns>
        /// An array of bools descirbing pass/fail result per site per session
        /// </returns>
        public bool[][] GetSitePassFail()
        {
            bool[][] perinstpassfail = new bool[SSC.Length][];
            Parallel.ForEach(SSC, ssc => perinstpassfail[ssc.Index] = ssc.Session.PatternControl.GetSitePassFail(ssc.SiteList));
            return perinstpassfail;
        }

        /// <summary>
        /// Gets an array of strings containing the terminal name of the DigitalPatternOpcodeEvent per site.
        /// </summary>
        /// <param name="patternOpcodeNumber">Pattern Opcode Number.</param>
        /// <returns>
        /// An array of strings with the terminal names of the DigitalPatternOpcodeEvent.
        /// </returns>
        public string[] GetPerSiteTriggerTerminals(int patternOpcodeNumber)
        {
            var triggerTerminals = new string[SiteNumbers.Length];

            foreach (var ssc in SSC)
            {
                foreach (var siteNdx in ssc.SiteIndex)
                {
                    triggerTerminals[siteNdx] = ssc.Session.Event.PatternOpcodeEvents[patternOpcodeNumber].TerminalName;
                }
            }

            return triggerTerminals;
        }

        /// <summary>
        /// Gets an array of strings containing the terminal name of the DigitalStartTrigger per site
        /// </summary>
        /// <returns>
        /// An array of strings with the terminal names of the DigitalStartTrigger.
        /// </returns>
        public string[] GetPerSiteStartTriggerTerminals()
        {
            var triggerTerminals = new string[SiteNumbers.Length];

            foreach (var ssc in SSC)
            {
                foreach (var siteNdx in ssc.SiteIndex)
                {
                    triggerTerminals[siteNdx] = ssc.Session.Trigger.StartTrigger.TerminalName;
                }
            }

            return triggerTerminals;
        }

        /// <summary>
        /// Waits until the pattern burst has completed or the specified "timeoutinSeconds" has expired
        /// </summary>
        /// <param name="timeoutinSeconds">The maximum time interval allowed for the pattern burst to complete.</param>
        public void WaitUntilDone(double timeoutinSeconds = 5.0) => Parallel.ForEach(SSC, ssc => ssc.Session.PatternControl.WaitUntilDone(TimeSpan.FromSeconds(timeoutinSeconds)));

        /// <summary>
        /// Applies digital levels and timing defined in the loaded levels and timing sheets.
        /// Any levels not specified in the levels sheet remain unchanged.
        /// When applying a timing sheet, all existing time sets are deleted before the new
        /// time sets are loaded.
        /// </summary>
        /// <param name="levelsSheetName">The name of the levels sheet to apply.</param>
        /// <param name="timingsSheetName">The name of the timing sheet to apply.</param>
        public void ApplyLevelsandTimings(string levelsSheetName, string timingsSheetName) => Parallel.ForEach(SSC, ssc => ssc.Session.ApplyLevelsAndTiming(ssc.SiteList, levelsSheetName, timingsSheetName));

        /// <summary>
        /// Sets the aperture time for the PPMU measurement
        /// </summary>
        /// <param name="apertureTimeinSeconds">The measurement aperture time for the PPMU (in seconds).</param>
        public void PPMUConfigureApertureTime(double apertureTimeinSeconds) => Parallel.ForEach(SSC, ssc => ssc.PinSet.Ppmu.ConfigureApertureTime(apertureTimeinSeconds, PpmuApertureTimeUnits.Seconds));

        /// <summary>
        /// Specifies the voltage limit high, or high clamp voltage (Vch), and voltage limit low, or low clamp voltage (Vcl) for the pin set when forcing current.
        /// </summary>
        /// <param name="voltageLimitLow"> The nominal voltage at the pin at which the low side voltage clamp activates when the PPMU forces current to the DUT.</param>
        /// <param name="voltageLimitHigh">The nominal voltage at the pin at which the high side voltage clamp activates when the PPMU forces current to the DUT.</param>
        public void PPMUConfigureVoltageLimits(double voltageLimitHigh, double voltageLimitLow) => Parallel.ForEach(SSC, ssc => ssc.PinSet.Ppmu.DCCurrent.ConfigureVoltageLimits(voltageLimitLow, voltageLimitHigh));
        
        /// <summary>
        /// Sets the PPMU output function as DCCurrent, sets the current level/range and starts sourcing current.
        /// </summary>
        /// <param name="currentLevel">The current level, in amps, forced to the DUT.</param>
        /// <param name="currentLevelRange">The valid range for the current level in amps forced to the DUT.</param>
        /// <param name="enableSourcing">If true, PPMU sources after configuration.</param>
        public void PPMUForceCurrent(double currentLevel, double? currentLevelRange = null, bool enableSourcing = true)
        {
            double chosenCurrentLevelRange = currentLevelRange ?? Math.Max(Math.Abs(currentLevel), 2e-6);
            Parallel.ForEach(SSC, ssc =>
            {
                ssc.PinSet.Ppmu.OutputFunction = PpmuOutputFunction.DCCurrent;
                ssc.PinSet.Ppmu.DCCurrent.CurrentLevel = currentLevel;
                ssc.PinSet.Ppmu.DCCurrent.CurrentLevelRange = chosenCurrentLevelRange;
                if (enableSourcing) ssc.PinSet.Ppmu.Source();
            });
        }

        /// <summary>
        /// Sets the PPMU output function as DCVoltage, sets the voltage level, current limit range and starts sourcing voltage.
        /// </summary>
        /// <param name="voltageLevel">The voltage level in volts forced to the DUT.</param>
        /// <param name="currentLimitRange">Maximum current in amps that can be set.</param>
        /// <param name="enableSourcing">If true, PPMU sources after configuration.</param>
        public void PPMUForceVoltage(double voltageLevel, double currentLimitRange, bool enableSourcing = true)
        {
            // Current limit is not configured here
            // The PXIe - 6570 and PXIe-6571 do not support current limits in PPMU voltage mode:
            // http://zone.ni.com/reference/en-XX/help/375145e/nidigitalpropref/pnidigital_ppmucurrentlimit/
            Parallel.ForEach(SSC, ssc =>
            {
                ssc.PinSet.Ppmu.OutputFunction = PpmuOutputFunction.DCVoltage;
                ssc.PinSet.Ppmu.DCVoltage.VoltageLevel = voltageLevel;
                ssc.PinSet.Ppmu.DCVoltage.CurrentLimitRange = currentLimitRange;
                //ssc.PinSet.Ppmu.DCCurrent.CurrentLevelRange = currentLimitRange;
                if (enableSourcing) ssc.PinSet.Ppmu.Source();
            });
        }

        /// <summary>
        /// Starts the sourcing of voltage or current from the PPMU.
        /// </summary>
        public void PPMUSource() => Parallel.ForEach(SSC, ssc => ssc.PinSet.Ppmu.Source());

        /// <summary>
        /// Instructs the PPMU to measure voltage or current.
        /// </summary>
        /// <param name="measurementType">Measurement type: Voltage of Current</param>
        /// <returns>
        /// The measurements taken, ordered according to the pinSetString parameter used. If a site is disabled, the method does not return data for that site (The method returns site numbers in the same order as values read).
        /// </returns>
        public double[][] PPMUMeasure(PpmuMeasurementType measurementType)
        {
            var measurements = new double[SSC.Length][];
            Parallel.ForEach(SSC, ssc => measurements[ssc.Index] = ssc.PinSet.Ppmu.Measure(measurementType));
            return measurements;
        }

        /// <summary>
        /// Reads the bool stated of the pattern sequencer flag. 
        /// </summary>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <returns>
        /// An array of the states of the specified pattern sequencer flag per session.
        /// </returns>
        public bool[] ReadSequencerFlags(string flag)
        {
            var perinstflags = new bool[SSC.Length];
            Parallel.ForEach(SSC, ssc => perinstflags[ssc.Index] = ssc.Session.PatternControl.ReadSequencerFlag(flag));
            return perinstflags;
        }

        /// <summary>
        /// Writes a bool value to a pattern sequencer flag.
        /// </summary>
        /// <param name="flag">The name of the pattern sequencer flag to read. Possible values include "seqflag0", "seqflag1", "seqflag2", or "seqflag3".</param>
        /// <param name="state">The state to assign to the specified pattern sequencer flag.</param>
        public void WriteSequencerFlag(string flag, bool state) => Parallel.ForEach(SSC, ssc => ssc.Session.PatternControl.WriteSequencerFlag(flag, state));

        /// <summary>
        /// Reads the numeric state of a pattern sequencer register. 
        /// </summary>
        /// <param name="register">Specifies pattern sequencer register to read.</param>
        /// <returns>
        /// The value read from the specified pattern sequence register arranged per session.
        /// </returns>
        public int[] ReadSequenceRegister(string register)
        {
            var perinstrumentRegisters = new int[SSC.Length];
            Parallel.ForEach(SSC, ssc => perinstrumentRegisters[ssc.Index] = ssc.Session.PatternControl.ReadSequencerRegister(register));
            return perinstrumentRegisters;
        }

        /// <summary>
        /// Reads the PpmuOutputFunction
        /// </summary>
        /// <returns>
        /// The value read PpmuOutputFunction per session.
        /// </returns>
        public PpmuOutputFunction[] ReadPpmuOutputFunction()
        {
            return SSC.AsParallel().Select(ssc => ssc.PinSet.Ppmu.OutputFunction).ToArray();
        }

        /// <summary>
        /// Writes a value to a pattern sequencer register.
        /// </summary>
        /// <param name="register">pecifies the sequencer register to which you would like to write the specified value.</param>
        /// <param name="regValue">The value to write to the specified pattern sequence register.</param>
        public void WriteSequencerRegister(string register, int regValue) => Parallel.ForEach(SSC, ssc => ssc.Session.PatternControl.WriteSequencerRegister(register, regValue));

        /// <summary>
        /// Fetches a defined number of samples for a specified list of sites. 
        /// </summary>
        /// <param name="captureWaveformName">The name of the waveform to fetch. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="samplesToRead">The number of samples to fetch. Use -1 to fetch all samples after the pattern is finished bursting.</param>
        /// <param name="timeoutInSeconds">The maximum amount of time allowed for this method to complete in seconds. An exception is thrown if the method does not complete within this time span.</param>
        /// <returns>
        /// The captured data for each site specified. If a site is disabled, not enabled for burst, or the current instrument does not include any capture pins, the method does not return data for that site, data is returned by session.
        /// </returns>
        public uint[][][] FetchCaptureWaveform(string captureWaveformName, int samplesToRead, double timeoutInSeconds = 5.0)
        {
            var perinstrumentcaptures = new uint[SSC.Length][][];
            Parallel.ForEach(SSC, ssc => perinstrumentcaptures[ssc.Index] = ssc.Session.CaptureWaveforms.Fetch(ssc.SiteList, captureWaveformName, samplesToRead, TimeSpan.FromSeconds(timeoutInSeconds), ref perinstrumentcaptures[ssc.Index]));
            return perinstrumentcaptures;
        }

        /// <summary>
        /// Writes the same source waveform data to all sites.
        /// </summary>
        /// <param name="waveformName">The name of the waveform to fetch. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="waveformData">A 1D array of waveform data samples to use as the source data to apply to all sites</param>    
        public void WriteSourceWaveformBroadcast(string waveformName, uint[] waveformData) => Parallel.ForEach(SSC, ssc => ssc.Session.SourceWaveforms.WriteBroadcast(waveformName, waveformData));

        /// <summary>
        /// Creates the source waveform settings used to source serial waveforms using the PinSet information from the session (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">The name of the waveform to use in the pattern file. Waveform names must be unique. Use the waveformName with source_start opcode in your pattern.</param>
        /// <param name="dataMapping">Specifies whether the waveform is broadcast to all sites or a unique waveform</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. Valid values are between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance. This can be most significant bit first or least significant bit first</param>
        public void CreateSourceWaveformBroadcast(string waveformName, SourceDataMapping dataMapping, uint sampleWidth, BitOrder bitOrder)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(String.Join(",", Pins));
                ssc.Session.SourceWaveforms.CreateSerial(pinSet, waveformName, dataMapping, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates the source waveform settings used to source serial waveforms using the PinSet information from the specified pin name (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">The name of the waveform to use in the pattern file. Waveform names must be unique. Use the waveformName with source_start opcode in your pattern.</param>
        /// <param name="pin">Pin name string</param>
        /// <param name="dataMapping">Specifies whether the waveform is broadcast to all sites or a unique waveform</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. Valid values are between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance. This can be most significant bit first or least significant bit first</param>
        public void CreateSourceWaveformBroadcast(string waveformName, string pin, SourceDataMapping dataMapping, uint sampleWidth, BitOrder bitOrder)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(pin);
                ssc.Session.SourceWaveforms.CreateSerial(pinSet, waveformName, dataMapping, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates the source waveform settings used to source serial waveforms using the PinSet information from the specified array of pin names (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">The name of the waveform to use in the pattern file. Waveform names must be unique. Use the waveformName with source_start opcode in your pattern.</param>
        /// <param name="pins">Array of Pin names</param>       
        /// <param name="dataMapping">Specifies whether the waveform is broadcast to all sites or a unique waveform</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. Valid values are between 1 and 32.</param>
        /// <param name="bitOrder">The bit order significance. This can be most significant bit first or least significant bit first</param>
        public void CreateSourceWaveformBroadcast(string waveformName, string[] pins, SourceDataMapping dataMapping, uint sampleWidth, BitOrder bitOrder)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(String.Join(",", pins));
                ssc.Session.SourceWaveforms.CreateSerial(pinSet, waveformName, dataMapping, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition using the PinSet information from the session. Settings apply across all sites if multiple sites are configured (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the capture_start opcode in your pattern.</param>
        public void CreateParallelCaptureWaveform(string waveformName)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(String.Join(",", Pins));
                ssc.Session.CaptureWaveforms.CreateParallel(pinSet, waveformName);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition using the PinSet information from the specified array of pin names. Settings apply across all sites if multiple sites are configured (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="pins">Array of Pin names</param>
        public void CreateParallelCaptureWaveform(string waveformName, string[] pins)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(String.Join(",", pins));
                ssc.Session.CaptureWaveforms.CreateParallel(pinSet, waveformName);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings for parallel acquisition using the PinSet information from the specified pin name. Settings apply across all sites if multiple sites are configured (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="pin">Pin name string</param>
        public void CreateParallelCaptureWaveform(string waveformName, string pin)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(pin);
                ssc.Session.CaptureWaveforms.CreateParallel(pinSet, waveformName);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings used to source serial waveforms using the PinSet information from the session (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. Valid values are between 1 and 32.</param>
        /// <param name="bitOrder">The order in which to shift the bits. This can be most significant bit first or least significant bit first.</param>
        public void CreateSerialCaptureWaveform(string waveformName, uint sampleWidth, BitOrder bitOrder = BitOrder.MostSignificantBitFirst)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(String.Join(",", Pins));
                ssc.Session.CaptureWaveforms.CreateSerial(pinSet, waveformName, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Creates the capture waveform settings used to source serial waveforms using the PinSet information from the specified pin name (The number of waveforms is limited to 512 and You cannot reconfigure settings after waveforms are created).
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the capture_start opcode in your pattern.</param>
        /// <param name="pin">Pin name string</param>
        /// <param name="sampleWidth">The width in bits of each serial sample. Valid values are between 1 and 32.</param>
        /// <param name="bitOrder">The order in which to shift the bits. This can be most significant bit first or least significant bit first.</param>
        public void CreateSerialCaptureWaveform(string waveformName, string pin, uint sampleWidth, BitOrder bitOrder = BitOrder.MostSignificantBitFirst)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                var pinSet = ssc.Session.PinAndChannelMap.GetPinSet(pin);
                ssc.Session.CaptureWaveforms.CreateSerial(pinSet, waveformName, sampleWidth, bitOrder);
            });
        }

        /// <summary>
        /// Writes one source waveform to each of the specified sites for each of the sessions.
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the source_start opcode in your pattern.</param>
        /// <param name="perInstrumentWaveformData">A 3D jagged array of waveform samples to use as source data. Each page (2D jagged array) corresponds to each site in the specified siteList in a per-instrument basis.</param>
        public void WriteSourceWaveformSiteUnique(string waveformName, uint[][][] perInstrumentWaveformData) => Parallel.ForEach(SSC, ssc => ssc.Session.SourceWaveforms.WriteSiteUnique(ssc.SiteList, waveformName, perInstrumentWaveformData[ssc.Index]));

        /// <summary>
        /// Writes one source waveform to each of the specified sites for each of the sessions.
        /// </summary>
        /// <param name="waveformName">Specifies the waveform name to use. Use the waveformName with the source_start opcode in your pattern.</param>
        /// <param name="perSiteWaveformData">A 2D jagged array of waveform samples to use as source data. Each row corresponds to the waveform data of each site.</param>
        public void WriteSourceWaveformSiteUnique(string waveformName, uint[][] perSiteWaveformData) => Parallel.ForEach(SSC, ssc => ssc.Session.SourceWaveforms.WriteSiteUnique(ssc.SiteList, waveformName, PerSiteToPerInstrumentData(perSiteWaveformData)[ssc.Index]));

        /// <summary>
        ///  Writes a static state to the channels or pins represented by this pin set. These channels or pins remain in the specified state until the next pattern burst or call to this method.
        /// </summary>
        /// <param name="state">The digital state to write to the channels or pins specified. Valid values are: <br/>
        /// • PinState._0 (0) – Specifies to drive low.<br/>
        /// • PinState._1 (1) – Specifies to drive high.<br/>
        /// • PinState._X (5) – Specifies to put the driver in a non-drive pin state (L, H, X, V, M, E).</param>  
        public void WriteStatic(PinState state) => Parallel.ForEach(SSC, ssc => ssc.PinSet.WriteStatic(state));

        /// <summary>
        /// Reads the current state of comparators for the specified channels or pins. 
        /// </summary>
        /// <returns>
        /// An array of digital states in the order specified by the pinSetString parameter used in each session.
        /// </returns>
        public PinState[][] ReadStatic()
        {
            var state = new PinState[SSC.Length][];
            Parallel.ForEach(SSC, ssc => state[ssc.Index] = ssc.PinSet.ReadStatic());
            return state;
        }

        /// <summary>
        /// Configures the Ioh, Iol and Vcom values
        /// </summary>
        /// <param name="iol">The current in amps that the DUT sinks when one or more pins outputs a voltage greater than Vcom.</param>
        /// <param name="ioh">The current in amps that the DUT sources when one or more pins outputs a voltage greater than Vcom.</param>
        /// <param name="vcom">The commutating voltage at which the active load circuit switches between sourcing current and sinking current.</param>
        public void ConfigureActiveLoadLevels(double iol, double ioh, double vcom)
        {
            Parallel.ForEach(SSC, ssc => ssc.PinSet.DigitalLevels.ConfigureActiveLoadLevels(iol, ioh, vcom));
        }

        /// <summary>
        /// Configures the high and low logic levels for voltage as well as the termination mode input voltage.
        /// </summary>
        /// <param name="vil">The voltage that the instrument will apply to the input of the DUT when the test instrument drives a logic low (0).</param>
        /// <param name="vih">The voltage that the instrument will apply to the input of the DUT when the test instrument drives a logic high (1).</param>
        /// <param name="vol">The output voltage below which the comparator on the pin driver interprets a logic low (L).</param>
        /// <param name="voh">The output voltage above which the comparator on the pin driver interprets a logic high (H).</param>
        /// <param name="vterm">The termination voltage the instrument applies during non-drive cycles when the termination mode is set to Vterm. The instrument applies the termination voltage through a 50 Ω parallel termination resistance.</param>
        public void ConfigureVoltgeLevels(double vil, double vih, double vol, double voh, double vterm)
        {
            Parallel.ForEach(SSC, ssc => ssc.PinSet.DigitalLevels.ConfigureVoltageLevels(vil, vih, vol, voh, vterm));
        }

        /// <summary>
        /// Specifies the behavior of the pin during non-drive cycles. 
        /// </summary>
        /// <param name="terminationMode">Termination mode. Select between: <br/>• HighZ<br/>• ActiveLoad<br/>• Vterm</param>
        public void ConfigureTerminationMode(TerminationMode terminationMode)
        {
            Parallel.ForEach(SSC, ssc => ssc.PinSet.DigitalLevels.TerminationMode = terminationMode);
        }

        /// <summary>
        /// Configures the drive format and the drive edge placement for the specified pins. 
        /// </summary>
        /// <param name="timeSet">Name of the time set to configure.</param>
        /// <param name="driveFormat">The drive format of the time set (NonReturn, ReturnToLow, ReturnToHigh, SurroundByComplement).</param>
        /// <param name="driveOn">The delay (in seconds) from the beginning of the vector period for turning on the pin driver. This option applies only when the prior vector left the pin in a non-drive PinState (L, H, X, V, M, E). For the SurroundByComplement format, this option specifies the delay from the beginning of the vector period at which the complement of the pattern value is driven.</param>
        /// <param name="driveData">The delay (in seconds) from the beginning of the vector period until the pattern data is driven to the pattern value. The ending state from the previous vector persists until this point.</param>
        /// <param name="driveReturn">The delay (in seconds) from the beginning of the vector period until the pin changes from the pattern data to the return value, as specified in the format.</param>
        /// <param name="driveOff">The delay (in seconds) from the beginning of the vector period to turn off the pin driver when the next vector period uses a non-drive PinState (L, H, X, V, M, E).</param>
        /// <remarks>Not all edges apply to all drive formats. </remarks>
        public void ConfigureTimeSetDriveEdges(string timeSet, DriveFormat driveFormat, double driveOn, double driveData, double driveReturn, double driveOff)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Timing.GetTimeSet(timeSet).ConfigureDriveEdges(ssc.PinSet, driveFormat, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveOn)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveData)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveReturn)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveOff))));
        }

        /// <summary>
        /// Configures the drive format and the drive edge placement for the specified pins. 
        /// </summary>
        /// <param name="timeSet">Name of the time set to configure.</param>
        /// <param name="driveFormat">The drive format of the time set (NonReturn, ReturnToLow, ReturnToHigh, SurroundByComplement).</param>
        /// <param name="driveOn">The delay (in seconds) from the beginning of the vector period for turning on the pin driver. This option applies only when the prior vector left the pin in a non-drive PinState (L, H, X, V, M, E). For the SurroundByComplement format, this option specifies the delay from the beginning of the vector period at which the complement of the pattern value is driven.</param>
        /// <param name="driveData">The delay (in seconds) from the beginning of the vector period until the pattern data is driven to the pattern value. The ending state from the previous vector persists until this point.</param>
        /// <param name="driveReturn">The delay (in seconds) from the beginning of the vector period until the pin changes from the pattern data to the return value, as specified in the format.</param>
        /// <param name="driveOff">The delay (in seconds) from the beginning of the vector period to turn off the pin driver when the next vector period uses a non-drive PinState (L, H, X, V, M, E).</param>
        /// <param name="driveData2">The delay (in seconds) from the beginning of the vector period until the pattern data is driven to the second pattern value. The ending state from the previous pattern value persists until this point. To use this edge you must specify a second pin state in the pattern and configure an edge multiplier greater than one in the time set.</param>
        /// <param name="driveReturn2">The delay (in seconds) from the beginning of the vector period until the pin changes from the second pattern data to the return value, as specified in the format. To use this edge you must specify a second pin state in the pattern and configure an edge multiplier greater than one in the time set.</param>
        /// <remarks>Not all edges apply to all drive formats. </remarks>
        public void ConfigureTimeSetDriveEdges(string timeSet, DriveFormat driveFormat, double driveOn, double driveData, double driveReturn, double driveOff, double driveData2, double driveReturn2)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Timing.GetTimeSet(timeSet).ConfigureDriveEdges(ssc.PinSet, driveFormat, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveOn)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveData)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveReturn)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveOff)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveData2)), Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(driveReturn2))));
        }

        /// <summary>
        /// Configures the period of a time set.
        /// </summary>
        /// <param name="timeSet">Name of the time set to configure.</param>
        /// <param name="period">The period of this time set, in seconds</param>
        public void ConfigureTimeSetPeriod(string timeSet, double period)
        {
            Parallel.ForEach(SSC, ssc => ssc.Session.Timing.GetTimeSet(timeSet).ConfigurePeriod(Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(period))));
        }

        /// <summary>
        ///  Configures the different parameters for HRAM Settings and HRAM Triggers.
        /// </summary>
        /// <param name="cyclesToAcquire">Configures which cycles History RAM acquires after the trigger conditions are met. If you configure History RAM to only acquire failed cycles, you must set the pretrigger samples for History RAM to 0.</param>
        /// <param name="preTriggerSamples">The number of samples to acquire before the DigitalHistoryRamTrigger.</param>
        /// <param name="maximumSamplesToAcquirePerSite">Specifies the maximum number of History RAM samples to acquire per site. If the property is set to -1, it will acquire until the History RAM buffer is full.</param>
        /// <param name="numberOfSamplesIsFinite">Specifies whether the digital pattern instrument acquires a finite number of History RAM samples or acquires samples continuously. When acquiring continuously, samples can be fetched during pattern burst.</param>
        /// <param name="bufferSizePerSite">Specifies the size, in samples, of the in-memory History RAM sample buffer. Use this property when the device is configured for continuous History RAM acquisition.</param>
        /// <param name="triggerType">Specifies the History RAM trigger type: FirstFailure, CycleNumber or PatternLabel.</param>
        /// <param name="cycleNumber">The number of cycles to execute before the DigitalHistoryRamTrigger.</param>
        /// <param name="patternLabel">The pattern label to augment by the vector and cycle offsets where History RAM will start acquiring pattern information.</param>
        /// <param name="cycleOffset">The number of cycles following the pattern label specified by Label and vector offset specified by VectorOffset where History RAM will start acquiring pattern information.</param>
        /// <param name="vectorOffset">The number of vectors following the pattern label specified by Label where History RAM will start acquiring pattern information.</param>
        public void ConfigureHRAM(HistoryRamCycle cyclesToAcquire, int preTriggerSamples, int maximumSamplesToAcquirePerSite, bool numberOfSamplesIsFinite, long bufferSizePerSite, HistoryRamTriggerType triggerType, long cycleNumber, string patternLabel, long cycleOffset, long vectorOffset)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                ssc.Session.HistoryRam.CyclesToAcquire = cyclesToAcquire;
                ssc.Session.Trigger.HistoryRamTrigger.PretriggerSamples = preTriggerSamples;
                ssc.Session.HistoryRam.MaximumSamplesToAcquirePerSite = maximumSamplesToAcquirePerSite;
                ssc.Session.HistoryRam.BufferSizePerSite = bufferSizePerSite;
                ssc.Session.HistoryRam.NumberOfSamplesIsFinite = numberOfSamplesIsFinite;
                ssc.Session.Trigger.HistoryRamTrigger.TriggerType = triggerType;
                ssc.Session.Trigger.HistoryRamTrigger.CycleNumber.Number = cycleNumber;
                ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.Label = patternLabel;
                ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.VectorOffset = vectorOffset;
                ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.CycleOffset = cycleOffset;
            });
        }

        /// <summary>
        ///  Get the current con=figuration parameters for HRAM Settings and HRAM Triggers.
        /// </summary>
        /// <param name="cyclesToAcquire">Gets which cycles History RAM acquires after the trigger conditions are met.</param>
        /// <param name="preTriggerSamples">Gets the number of samples to acquire before the DigitalHistoryRamTrigger.</param>
        /// <param name="maximumSamplesToAcquirePerSite">Gets the the maximum number of History RAM samples to acquire per site.</param>
        /// <param name="numberOfSamplesIsFinite">Gets whether the digital pattern instrument acquires a finite number of History RAM samples or acquires samples continuously</param>
        /// <param name="bufferSizePerSite">Gets the size, in samples, of the in-memory History RAM sample buffer.</param>
        /// <param name="triggerType">Gets the History RAM trigger type: FirstFailure, CycleNumber or PatternLabel.</param>
        /// <param name="cycleNumber">Gets the number of cycles to execute before the DigitalHistoryRamTrigger.</param>
        /// <param name="patternLabel">Gets the pattern label to augment by the vector and cycle offsets where History RAM will start acquiring pattern information.</param>
        /// <param name="cycleOffset">Gets the number of cycles following the pattern label specified by Label and vector offset specified by VectorOffset where History RAM will start acquiring pattern information.</param>
        /// <param name="vectorOffset">Gets the number of vectors following the pattern label specified by Label where History RAM will start acquiring pattern information.</param>
        public void GetHRAMConfiguration(out HistoryRamCycle cyclesToAcquire, out int preTriggerSamples, out int maximumSamplesToAcquirePerSite, out bool numberOfSamplesIsFinite, out long bufferSizePerSite, out HistoryRamTriggerType triggerType, out long cycleNumber, out string patternLabel, out long cycleOffset, out long vectorOffset)
        {
            cyclesToAcquire = HistoryRamCycle.All;
            preTriggerSamples = 0;
            maximumSamplesToAcquirePerSite = 0;
            bufferSizePerSite = 0;
            numberOfSamplesIsFinite = false;
            triggerType = HistoryRamTriggerType.FirstFailure;
            cycleNumber = 0;
            patternLabel = "";
            vectorOffset = 0;
            cycleOffset = 0;

            foreach (var ssc in SSC)
            {
                cyclesToAcquire = ssc.Session.HistoryRam.CyclesToAcquire;
                preTriggerSamples = ssc.Session.Trigger.HistoryRamTrigger.PretriggerSamples;
                maximumSamplesToAcquirePerSite = ssc.Session.HistoryRam.MaximumSamplesToAcquirePerSite;
                bufferSizePerSite = ssc.Session.HistoryRam.BufferSizePerSite;
                numberOfSamplesIsFinite = ssc.Session.HistoryRam.NumberOfSamplesIsFinite;
                triggerType = ssc.Session.Trigger.HistoryRamTrigger.TriggerType;
                cycleNumber = ssc.Session.Trigger.HistoryRamTrigger.CycleNumber.Number;
                patternLabel = ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.Label;
                vectorOffset = ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.VectorOffset;
                cycleOffset = ssc.Session.Trigger.HistoryRamTrigger.PatternLabel.CycleOffset;
            }
        }

        /// <summary>
        /// Measures propagation delays through cables, connectors, and load boards using Time-Domain Reflectometry (TDR). Optionally, you can apply the offsets to the pins.Returns the measured TDR offsets specified in seconds.
        /// </summary>
        /// <param name="apply">Specifies whether to apply the measured TDR offsets. The default value is false.</param>
        /// <returns>
        /// The measured TDR offset values.
        /// </returns>
        public Ivi.Driver.PrecisionTimeSpan[][] MeasureTDROffsets(bool apply = false)
        {
            var tdrOffsets = new Ivi.Driver.PrecisionTimeSpan[SSC.Length][];
            //Parallel.ForEach(SSC, ssc => tdrOffsets[ssc.Index] = ssc.PinSet.Tdr(false));
            Parallel.ForEach(SSC, ssc => tdrOffsets[ssc.Index] = ssc.PinSet.Tdr(apply));    //apply bool argument   -adrian

            return tdrOffsets;
        }

        /// <summary>
        /// Applies the correction for propagation delay offsets to a digital pattern instrument.
        /// </summary>
        /// <param name="offsetsToApply">The Time-Domain Reflectometry (TDR) offsets to write to the digital pattern instrument.</param>
        public void ApplyTDROffsets(Ivi.Driver.PrecisionTimeSpan[][] offsetsToApply)
        {
            Parallel.ForEach(SSC, ssc => ssc.PinSet.ApplyTdrOffsets(offsetsToApply[ssc.Index]));
        }

        /// <summary>
        /// Saves TDR offset values into an external File specified using the filePath value.
        /// </summary>
        /// <param name="offsetsToSave">TDS offsets value to record into the external file.</param>
        /// <param name="filePath">Absolute Path value of file to store the offset data.</param>
        public void SaveTDROffsetsToFile(Ivi.Driver.PrecisionTimeSpan[][] offsetsToSave, string filePath)
        {
            var lines = new List<string>();

            for (int instIndex = 0; instIndex < SSC.Length; instIndex++)
            {
                var pins = SSC[instIndex].ChannelList.Split(',');
                for (int pinIndex = 0; pinIndex < pins.Length; pinIndex++)
                {
                    lines.Add(pins[pinIndex].Trim() + ": " + offsetsToSave[instIndex][pinIndex].ToDecimal());
                }
            }
            File.WriteAllLines("C:/Data/ADuM225N_TDR.txt", lines);
        }

        /// <summary>
        /// Reads and applies the TDR offset values from an external File specified using the filePath value. Return the TDR offset values as a precisionTimeSpan array.
        /// </summary>        
        /// <param name="filePath">Absolute Path value of file to read the offset data.</param>
        /// <param name="exceptionWhenMissing">Defines if exception will be raised uppon not finding the file. Default value is set to true.</param>
        public Ivi.Driver.PrecisionTimeSpan[][] LoadTDROffsetsFromFile(string filePath, bool exceptionWhenMissing = true)
        {
            var offsetDictionary = new Dictionary<string, Ivi.Driver.PrecisionTimeSpan>();

            //var lines = File.ReadAllLines("C:/Data/ADuM225N_TDR.txt");
            //var lines = File.ReadAllLines("C:\\Data\\ADuM225N_TDR.txt");
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (!String.IsNullOrWhiteSpace(line))
                {
                    var valuesInLine = line.Split(':');
                    var channel = valuesInLine[0].Trim();
                    var offset = Ivi.Driver.PrecisionTimeSpan.FromSeconds(Convert.ToDouble(valuesInLine[1].Trim()));
                    offsetDictionary.Add(channel, offset);
                }
            }

            var missingOffsets = new List<string>();

            var loadedTDROffsets = new Ivi.Driver.PrecisionTimeSpan[SSC.Length][];
            for (int instIndex = 0; instIndex < SSC.Length; instIndex++)
            {
                var pins = SSC[instIndex].ChannelList.Split(',');

                loadedTDROffsets[instIndex] = new Ivi.Driver.PrecisionTimeSpan[pins.Length];

                for (int pinIndex = 0; pinIndex < pins.Length; pinIndex++)
                {
                    var pin = pins[pinIndex].Trim();
                    if (offsetDictionary.ContainsKey(pin))
                    {
                        loadedTDROffsets[instIndex][pinIndex] = offsetDictionary[pin];
                    }
                    else
                    {
                        missingOffsets.Add(pin);
                    }
                }
            }

            if (exceptionWhenMissing && missingOffsets.Count() > 0)
            {
                throw new Exception("Specified TDR File of " + filePath + " does not contain TDR offsets for pins " + string.Join(", ", missingOffsets));
            }

            return loadedTDROffsets;
        }

        /// <summary>
        /// Publishes captured data from the instrument sessions to the specified publish Data ID.
        /// </summary>
        /// <param name="perSiteCaptureData">Captured data to publish provided per site (double val type).</param>
        /// <param name="publishDataID">Data ID to publisht the data.</param>
        /// <remarks>
        /// Data to publish is a double type.
        /// </remarks>
        public void PublishCaptureData(double[][] perSiteCaptureData, string publishDataID = "")
        {
            PublishCaptureData(PerSiteToPerInstrumentData(perSiteCaptureData), publishDataID);
        }

        /// <summary>
        /// Publishes captured data from the instrument sessions to the specified publish Data ID.
        /// </summary>
        /// <param name="perSiteCaptureData">Captured data to publish provided per site (uint val type).</param>
        /// <param name="publishDataID">Data ID to publisht the data.</param>
        /// <remarks>
        /// Data to publish is an unsigned integer type.
        /// </remarks> 
        public void PublishCaptureData(uint[][] perSiteCaptureData, string publishDataID = "")
        {
            PublishCaptureData(PerSiteToPerInstrumentData(perSiteCaptureData), publishDataID);
        }

        /// <summary>
        /// Publishes multi-site captured data from the instrument sessions to the specified publish Data ID.
        /// </summary>
        /// <param name="perInstrumentCaptures">Captured data to publish provided per site per instrument (double val type).</param>
        /// <param name="publishDataID">Data ID to publisht the data.</param>
        /// <remarks>
        /// Data to publish is a double type.
        /// </remarks>
        public void PublishCaptureData(double[][][] perInstrumentCaptures, string publishDataID = "")
        {
            if (perInstrumentCaptures != null)
            {
                var publishcount = perInstrumentCaptures[0][0].Length;

                if (publishcount == 1)
                {
                    PinQueryContext.Publish(InstrCtrl.GetSingleSampleFromPerInstData(perInstrumentCaptures), publishDataID);
                }
                else if (publishcount > 1)
                {
                    for (int sample = 0; sample < publishcount; sample++)
                    {
                        PinQueryContext.Publish(InstrCtrl.GetSingleSampleFromPerInstData(perInstrumentCaptures, sample), publishDataID + sample);
                    }
                }
            }
        }

        /// <summary>
        /// Publishes multi-site captured data from the instrument sessions to the specified publish Data ID.
        /// </summary>
        /// <param name="perInstrumentCaptures">Per Instrument, per session data to publish.</param>
        /// <param name="publishDataID">Data ID to publisht the data.</param>
        /// <remarks>
        /// Data to publish is an unsigned integer type.
        /// </remarks> 
        public void PublishCaptureData(uint[][][] perInstrumentCaptures, string publishDataID = "")
        {
            if (perInstrumentCaptures != null)
            {
                var publishcount = perInstrumentCaptures[0][0].Length;

                if (publishcount == 1)
                {
                    var results = InstrCtrl.ConvertUinttoDouble(InstrCtrl.GetSingleSampleFromPerInstData(perInstrumentCaptures));
                    PinQueryContext.Publish(results, publishDataID);
                }
                else if (publishcount > 1)
                {
                    for (int i = 0; i < publishcount; i++)
                    {
                        var results = InstrCtrl.ConvertUinttoDouble(InstrCtrl.GetSingleSampleFromPerInstData(perInstrumentCaptures, i));
                        PinQueryContext.Publish(results, publishDataID + i);
                    }
                }
            }
        }

        /// <summary>
        /// This method converts per-intrument data into per-site valid data
        /// </summary>
        /// <typeparam name="T">List of converted data (per-site).</typeparam>
        /// <param name="perInstrumentData">Per Instrument input data to convert.</param>
        /// <returns>
        /// The data in the converted format.
        /// </returns>
        public T[][] PerInstrumentToPerSiteData<T>(T[][][] perInstrumentData)
        {
            var siteNums = SiteNumbers.ToArray();
            T[][] perSiteData = null;
            if (perInstrumentData != null)
            {
                perSiteData = new T[SiteNumbers.Length][];
                for (int instrNdx = 0; instrNdx < perInstrumentData.Length; instrNdx++)
                {
                    for (int siteNdx = 0; siteNdx < perInstrumentData[instrNdx].Length; siteNdx++)
                    {
                        perSiteData[SSC[instrNdx].SiteIndex[siteNdx]] = perInstrumentData[instrNdx][siteNdx];
                    }
                }
            }
            return perSiteData;
        }

        /// <summary>
        /// This method converts per-site data into per-instrument valid data 
        /// </summary>
        /// <typeparam name="T">List of converted data (per-instrument).</typeparam>
        /// <param name="perSiteData">Per Site input data to convert.</param>
        /// <returns>
        /// The data in the converted format.
        /// </returns>
        public T[][][] PerSiteToPerInstrumentData<T>(T[][] perSiteData)
        {
            T[][][] perInstrumentData = null;
            if (perSiteData != null)
            {
                perInstrumentData = new T[SSC.Length][][];
                for (int instrNdx = 0; instrNdx < perInstrumentData.Length; instrNdx++)
                {
                    perInstrumentData[instrNdx] = new T[SSC[instrNdx].SiteIndex.Length][];
                    for (int siteNdx = 0; siteNdx < perInstrumentData[instrNdx].Length; siteNdx++)
                    {
                        perInstrumentData[instrNdx][siteNdx] = perSiteData[SSC[instrNdx].SiteIndex[siteNdx]];
                    }
                }
            }
            return perInstrumentData;
        }
    }

}