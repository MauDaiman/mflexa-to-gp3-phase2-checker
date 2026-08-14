using NationalInstruments.ModularInstruments.NIDmm;
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
        /// Initializes all NI-Dmm sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void InitDmmSessions(ISemiconductorModuleContext tsmContext)
        {
            var DmmInstrumentsNames = tsmContext.GetNIDmmInstrumentNames();

            Parallel.ForEach(DmmInstrumentsNames, DmmInstrumentsName =>
            {
                var session = new NIDmm(DmmInstrumentsName, false, true);
                tsmContext.SetNIDmmSession(DmmInstrumentsName, session);
            });
        }

        /// <summary>
        /// Closes all active NI-Dmm sessions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        public static void CloseDmmSessions(ISemiconductorModuleContext tsmContext)
        {
            Parallel.ForEach(tsmContext.GetAllNIDmmSessions(), session =>
            {
                session.Close();
            });
        }

        /// <summary>
        /// Generates a NI-Dmm session based on the provided Pin name and session context.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pin">Pin name.</param>
        /// <returns>
        /// A single NI-Dmm session.
        /// </returns>
        public static Dmm DmmPinsToSessions(ISemiconductorModuleContext tsmContext, string pin)
        {
            string[] pins = { pin };
            return DmmPinsToSessions(tsmContext, pins);
        }
        /// <summary>
        /// Generates a series of NI-Dmm session based on the provided Pins names array and session context. 
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context</param>
        /// <param name="pins">Array of Pin names</param>
        /// <returns>
        /// A NI-Dmm object containing all Pin defined sessions.
        /// </returns>
        public static Dmm DmmPinsToSessions(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var pqc = tsmContext.GetNIDmmSessions(pins, out var dmmSessions);

            var dmmssc = new DmmSSC[dmmSessions.Length];

            var siteNumbers = tsmContext.SiteNumbers.ToArray();

            for (int sessionNdx = 0; sessionNdx < dmmSessions.Length; sessionNdx++)
            {
                dmmssc[sessionNdx].Session = dmmSessions[sessionNdx];
                dmmssc[sessionNdx].PinIndex = Array.IndexOf(pins, dmmssc[sessionNdx].Pin);
                dmmssc[sessionNdx].SiteIndex = Array.IndexOf(siteNumbers, dmmssc[sessionNdx].SiteNumber);
            }

            return new Dmm() { PinQueryContext = pqc, Pins = pins, SiteNumbers = siteNumbers, SSC = dmmssc };
        }

        /// <summary>
        /// Configures all available Dmm sessions to share the same measurement configuration (Measurement type, Range and Resolution).
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context</param>
        /// <param name="measurementType">Specifies measurement function to be used in the session.</param>
        /// <param name="voltageRange">Measurement voltage range value in volts.</param>
        /// <param name="resolutionDigits">Measurement resolution defined in digits.</param>
        /// <param name="error">Error out message string.</param>
        public static void DmmConfigureMeasurementVoltageRange(ISemiconductorModuleContext semiconductorModuleContext, out string error, DmmMeasurementFunction measurementType = DmmMeasurementFunction.DCVolts, double voltageRange = 0, double resolutionDigits = 5.5)
        {
            string exception = "";

            Parallel.ForEach(semiconductorModuleContext.GetAllNIDmmSessions(), session =>
            {
                //session.ConfigureMeasurementDigits(measurementType, voltageRange, resolutionDigits);
                session.MeasurementFunction = DmmMeasurementFunction.DCVolts;
                session.Range = voltageRange;
                try
                {
                    // Try to set resolution to provided numeric value
                    session.DigitsResolution = resolutionDigits;
                }
                catch (Exception e)
                {
                    // Catch error exception and print out, set resolution digits to a default value
                    exception = "Fault Exception Thrown: " + e.Message;
                    session.DigitsResolution = 3.5;
                }
            });

            error = String.IsNullOrEmpty(exception) ? "" : exception + "\\nSetting resolution to 3.5 digits";
        }

        /// <summary>
        /// Configures all available Dmm sessions to share the same aperture time configuration. 
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context</param>
        /// <param name="apertureTimeUnits">Unit of measurement used for aperture time for the current configuration (seconds or PLCs).</param>
        /// <param name="apertureTime">Aperture time value (must match aperture time units).</param>
        /// <param name="powerLineFrequency">Power Line frequency value in Hertz.</param>
        public static void DmmConfigureApertureTime(ISemiconductorModuleContext semiconductorModuleContext, DmmApertureTimeUnits apertureTimeUnits, double apertureTime, double powerLineFrequency = 60)
        {
            Parallel.ForEach(semiconductorModuleContext.GetAllNIDmmSessions(), session =>
            {
                session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                session.Advanced.ApertureTime = apertureTime;
                session.Advanced.PowerlineFrequency = powerLineFrequency;
            });
        }

        /// <summary>
        /// Configures all available Dmm sessions to share the same configuration parameters in the method and acquires a single voltage measurement
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context</param>
        /// <param name="voltageRange">Measurement voltage range value in volts.</param>
        /// <param name="resolutionDigits">Measurement resolution defined in digits.</param>
        /// <param name="voltageMeasurements">Array containing all voltage measurements.</param>
        /// <param name="voltageOverRange">Boolean result from OverRange comparison (TRUE means comparison condition was met).</param>
        public static void DmmConfigureAndMeasureVoltage(ISemiconductorModuleContext semiconductorModuleContext, double voltageRange, double resolutionDigits, out double[] voltageMeasurements, out bool[] voltageOverRange)
        {
            var sessions = semiconductorModuleContext.GetAllNIDmmSessions();
            var measurements = new double[sessions.Length];
            var overRange = new bool[sessions.Length];

            Parallel.For(0, sessions.Length, i =>
            {
                sessions[i].ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, voltageRange, resolutionDigits);
                measurements[i] = sessions[i].Measurement.Read();
                overRange[i] = sessions[i].Measurement.IsOverRange(sessions[i].Measurement.Read());
            });

            voltageMeasurements = measurements;
            voltageOverRange = overRange;
        }

        /// <summary>
        /// Configures all available Dmm sessions to share the same configuration parameters in the method.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context</param>
        /// <param name="measurementType">Specifies measurement function to be used in the session.</param>
        /// <param name="apertureTimeUnits">Unit of measurement used for aperture time for the current configuration (seconds or PLCs).</param>
        /// <param name="apertureTime">Aperture time value (must match aperture time units).</param>
        /// <param name="settleTimeSeconds">Specifies settling time value for measurements (in seconds).</param>
        /// <param name="powerLineFrequency">Power Line frequency value in Hertz.</param>
        /// <param name="voltageRange">Measurement voltage range value in volts.</param>
        /// <param name="resolutionDigits">Measurement resolution defined in digits.</param>
        public static void DmmConfigureAllSessions(ISemiconductorModuleContext semiconductorModuleContext, DmmMeasurementFunction measurementType, DmmApertureTimeUnits apertureTimeUnits, double apertureTime, double settleTimeSeconds = 0.01, double powerLineFrequency = 60, double voltageRange = 0.02, double resolutionDigits = 5.5)
        {
            Parallel.ForEach(semiconductorModuleContext.GetAllNIDmmSessions(), session =>
            {
                session.ConfigureMeasurementDigits(measurementType, voltageRange, resolutionDigits);
                session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                session.Advanced.ApertureTime = apertureTime;
                session.Advanced.PowerlineFrequency = powerLineFrequency;
                session.Advanced.SettleTime = settleTimeSeconds;
            });
        }
    }

    /// <summary>
    /// Structure reference to for Dmm SSC members
    /// </summary>
    public struct DmmSSC
    {
        /// <summary>
        /// NI-Dmm session field property.
        /// </summary>
        public NIDmm Session { get; set; }
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
    /// Class definition for Dmm Sessions Objects.
    /// </summary>
    public class Dmm
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public NIDmmMultiplePinMultipleSessionQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// Dmm SSC definition.
        /// </summary>
        public DmmSSC[] SSC { get; set; }
        /// <summary>
        /// Pin names.
        /// </summary>
        public string[] Pins { get; set; }
        /// <summary>
        /// Site numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }

        /// <summary>
        ///  Aborts a previously initiated measurement and returns the DMM to the idle state.
        /// </summary>
        public void Abort() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Abort());

        /// <summary>
        /// Sends a Control action the DMM (DmmControlAction)
        /// </summary>
        /// <param name="action">Control Action for Dmm</param>
        public void Control(DmmControlAction action) => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Control(action));

        /// <summary>
        /// Frees the resources held by the Dmm session
        /// </summary>
        public void Dispose() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Dispose());

        /// <summary>
        /// Initiates an acquisition. After calling this method, the DMM leaves the Idle state and enters the Wait-for-Trigger state.
        /// </summary>
        public void Initiate() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.Initiate());


        /// <summary>
        /// Takes a measurement value and determines if the value is a valid measurement
        /// or a value indicating that an overrange condition occurred.
        /// </summary>
        /// <param name="measurementValue">Measure value to check over range function.</param>
        /// <returns>
        /// A bool array stating result for each Over Range comparison within the session.
        /// </returns>
        public bool[] IsOverRange(double measurementValue)
        {
            var boolCheck = new bool[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                boolCheck[index] = SSC[index].Session.Measurement.IsOverRange(measurementValue);
            });
            return boolCheck;
        }

        /// <summary>
        /// Takes a measurement value and determines if the value is a valid measurement
        /// or a value indicating that an underrange condition occurred.
        /// </summary>
        /// <param name="measurementValue">Measure value to check Under range function.</param>
        /// <returns>
        /// A bool array stating result for each Under Range comparison within the session.
        /// </returns>
        public bool[] IsUnderRange(double measurementValue)
        {
            var boolCheck = new bool[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                boolCheck[index] = SSC[index].Session.Measurement.IsUnderRange(measurementValue);
            });
            return boolCheck;
        }

        /// <summary>
        /// Acquires a single measurement and fetches the measured value. 
        /// </summary>
        /// <returns>
        /// The measured value returned from the DMM.
        /// </returns>
        public double[] Read()
        {
            var measurements = new double[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = SSC[index].Session.Measurement.Read();
            });

            return measurements;
        }

        /// <summary>
        /// Acquires multiple measurements.
        /// </summary>
        /// <param name="pointsToFetch">Specifies the number of measurements to acquire. For continuous acquisitions, up to 100,000 points can be returned at once. The number of measurements can be a subset. The valid range is any positive integer. The default value is 1.</param>
        /// <returns>
        /// An array of measured values returned from the DMM
        /// </returns>
        public double[][] ReadMultiPoint(int pointsToFetch = 1)
        {
            var measurements = new double[SSC.Length][];
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.Trigger.MultiPoint.SampleCount = pointsToFetch;
                measurements[index] = new double[pointsToFetch];
                measurements[index] = SSC[index].Session.Measurement.ReadMultiPoint(pointsToFetch);
            });
            return measurements;
        }

        /// <summary>
        /// Fetches a value from a previously initiated measurement.
        /// </summary>
        /// <returns>The measured value returned from the DMM.</returns>
        /// <remarks>
        /// You must call Initiate before calling this method. The maximum time allowed for this method to complet is set to PrecisionTimeSpan.MaxValue.
        /// </remarks>
        public double[] Fetch()
        {
            var measurements = new double[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = SSC[index].Session.Measurement.Fetch();
            });

            return measurements;
        }

        /// <summary>
        /// Fetches values from a previously initiated multipoint measurement.
        /// </summary>
        /// <param name="pointsToFetch">Specifies the number of measurements to acquire. The maximum number of measurements for a finite acquisition is the (triggerCount x sampleCount). For continuous acquisitions, up to 100,000 points can be returned at once. The number of measurements can be a subset. The valid range is any positive integer. Default value is set to 1.</param>
        /// <returns>A Double[][] array of measured values per session.</returns>
        /// <remarks>
        /// You must call Initiate before calling this method. The number of measurements the DMM makes is determined by the values you specify for the triggerCount and sampleCount parameters. The maximum time allowed for this method to complet is set to PrecisionTimeSpan.MaxValue.
        /// </remarks>
        public double[][] FetchMultipoint(int pointsToFetch = 1)
        {
            var measurements = new double[SSC.Length][];
            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new double[pointsToFetch];
                measurements[index] = SSC[index].Session.Measurement.FetchMultiPoint(pointsToFetch);
            });

            return measurements;
        }

        /// <summary>
        /// Acquires multiple measurements and returns an array of values (Trigger is configured as inmediate).
        /// </summary>
        /// <param name="measurementDelay">Specifies the time that the DMM waits after it has received a trigger before taking a measurement.</param>
        /// <param name="pointsToFetch">Specifies the number of measurements to acquire. For continuous acquisitions, up to 100,000 points can be returned at once. The number of measurements can be a subset. The valid range is any positive integer. The default value is 1.</param>
        /// <returns>
        ///  An array of measured values returned from the DMM
        /// </returns>
        public double[][] ReadMultiPointInmediate(double measurementDelay, int pointsToFetch = 1)
        {
            var measurements = new double[SSC.Length][];
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.Trigger.Configure(DmmTriggerSource.Immediate, PrecisionTimeSpan.FromSeconds(measurementDelay));
                SSC[index].Session.Trigger.MultiPoint.SampleCount = pointsToFetch;
                measurements[index] = new double[pointsToFetch];
                measurements[index] = SSC[index].Session.Measurement.ReadMultiPoint(pointsToFetch);
            });
            return measurements;
        }

        /// <summary>
        /// Sends a software command to trigger the DMM.
        /// </summary>
        public void SendSoftwareTrigger() => Parallel.ForEach(SSC, ssc => ssc.Session.Measurement.SendSoftwareTrigger());

        /// <summary>
        /// Configures the settings for multipoint signal acquisition (Trigger source, sample count and edge slope)
        /// </summary>
        /// <param name="triggerSource">Specifies the trigger that initiates the acquisition.</param>
        /// <param name="triggerEdge">Edge of the signal from the specified trigger source on which the DMM is triggered.</param>
        /// <param name="triggerDelay">Specifies the time that the DMM waits after it has received a trigger before taking a measurement.</param>
        /// <param name="sampleCount">The number of measurements the DMM makes in each measurement sequence initiated by a trigger. The default is 1.</param>
        public void ConfigureMultipoint(DmmTriggerSource triggerSource, DmmSlope triggerEdge, double triggerDelay, int sampleCount = 1)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.Trigger.Configure(triggerSource, PrecisionTimeSpan.FromSeconds(triggerDelay));
                SSC[index].Session.Trigger.MultiPoint.SampleCount = sampleCount;
                SSC[index].Session.Trigger.Slope = triggerEdge;
            });
        }

        /// <summary>
        /// Configures different properties for the Dmm sessions
        /// </summary>
        public void ConfigureDmmSessions(DmmMeasurementFunction measurementType, DmmApertureTimeUnits apertureTimeUnits, double apertureTime, DmmAuto AutoZero = DmmAuto.Auto, DmmAdcCalibration ADC_calibraion = DmmAdcCalibration.Auto, int numberOfAverages = 1, double settleTimeSeconds = 0.01, double powerLineFrequency = 60, double voltageRange = 0.02, double resolutionDigits = 5.5, DmmDCNoiseRejection DCNoiseRejection = DmmDCNoiseRejection.Auto)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.ConfigureMeasurementDigits(measurementType, voltageRange, resolutionDigits);
                SSC[index].Session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                SSC[index].Session.Advanced.ApertureTime = apertureTime;
                SSC[index].Session.Advanced.PowerlineFrequency = powerLineFrequency;
                SSC[index].Session.Advanced.SettleTime = settleTimeSeconds;
                SSC[index].Session.Advanced.AutoZero = AutoZero;
                SSC[index].Session.Advanced.AdcCalibration = ADC_calibraion;
                SSC[index].Session.Advanced.NumberOfAverages = numberOfAverages;
                SSC[index].Session.Advanced.DCNoiseRejection = DCNoiseRejection;
            });
        }

        /// <summary>
        /// Configures frequency Bandwith for sessions when selecting AC measuring functions.
        /// </summary>
        /// <param name="minACFrequency">Minimum frequency component of the input signal for AC measurements.</param>
        /// <param name="maxACFrequency">Maximum frequency component of the input signal for AC measurements.</param>
        public void ConfigureDmmSessionsACBandwidth(double minACFrequency, double maxACFrequency)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.AC.ConfigureBandwidth(minACFrequency, maxACFrequency);
                SSC[index].Session.AC.FrequencyMin = minACFrequency;
                SSC[index].Session.AC.FrequencyMax = maxACFrequency;
            });
        }

        /// <summary>
        /// Enumerator to list available waveform functions.
        /// </summary>
        public enum WaveformFunction
        {
            /// <summary>
            /// Current Waveform.
            /// </summary>
            CurrentWaveform,
            /// <summary>
            /// Voltage Waveform.
            /// </summary>
            VoltageWaveform,
        }

        /// <summary>
        /// Configures and acquires waveform measurements for the sessions.
        /// </summary>   
        /// <param name="waveformtype">Waveform type: select between Voltage or Current</param>
        /// <param name="range">Specifies the expected maximum amplitude of the input signal and sets the range for the measurementFunction.</param>
        /// <param name="rate">Specifies the rate of the acquisition in samples per second. Range values are coerced up to the closest input range.</param>
        /// <param name="sampleCount">The number of measurements the DMM makes in each measurement sequence initiated by a trigger. The default is 1.</param>
        /// <returns>
        /// An analog waveform array with all measurements.
        /// </returns>
        public AnalogWaveform<double>[] ConfigureAndAcquireWaveform(WaveformFunction waveformtype, double range, double rate, int sampleCount = 1)
        {
            var analogWaveform = new AnalogWaveform<double>[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].Session.ConfigureWaveformAcquisition((waveformtype == WaveformFunction.VoltageWaveform) ? DmmMeasurementFunction.WaveformVoltage : DmmMeasurementFunction.WaveformCurrent, range, rate, sampleCount);
                analogWaveform[index] = SSC[index].Session.WaveformAcquisition.ReadWaveform(sampleCount, PrecisionTimeSpan.MaxValue);
            });
            return analogWaveform;
        }

        /// <summary>
        /// Acquires the current mesurement session status.
        /// </summary>
        /// <returns>
        /// The measurement backlog and acquisition status.
        /// </returns>
        public DmmAcquisitionStatus[] ReadSessionStatus(out int[] acquisitionBacklog)
        {
            var acquisitionStatus = new DmmAcquisitionStatus[SSC.Length];
            var acquisitionCounts = new int[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                acquisitionStatus[index] = SSC[index].Session.Measurement.ReadStatus(out var count);
                acquisitionCounts[index] = count;
            });
            acquisitionBacklog = acquisitionCounts;

            return acquisitionStatus;
        }

    }
}