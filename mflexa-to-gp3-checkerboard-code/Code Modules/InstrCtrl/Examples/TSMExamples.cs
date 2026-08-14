using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Linq;

namespace NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Examples
{
    /// <summary>
    /// Examples of how to use the Instrument Control to develop TestStand sequence file Code Modules.
    /// </summary>
    public class TSMExamples
    {
        /// <summary>
        /// Performs simple NI-DCPower Source, Measure and Publish.
        /// </summary>
        /// <param name="tsmContext">The ISemiconductorModuleContext object.</param>
        /// <param name="pin">Pin name.</param>
        public static void DCPowerSimpleSourceAndMeasure(ISemiconductorModuleContext tsmContext, string pin)
        {
            var dcSession = InstrCtrl.DCPowerPinsToSessions(tsmContext, pin);

            dcSession.ForceVoltage(3, .1);
            dcSession.Measure(out double[] voltageMeasurement, out double[] currentMeasurement);

            dcSession.PinQueryContext.Publish(voltageMeasurement, "Voltage");
            dcSession.PinQueryContext.Publish(currentMeasurement, "Current");

        }

        /// <summary>
        /// Performs simple PPMU Source, Measure and Publish.
        /// </summary>
        /// <param name="tsmContext">The ISemiconductorModuleContext object.</param>
        /// <param name="pin">Pin name.</param>
        public static void PPMUSimpleSourceAndMeasure(ISemiconductorModuleContext tsmContext, string pin)
        {
            var ppmuSession = InstrCtrl.DigitalPinsToSessions(tsmContext, pin);

            ppmuSession.PPMUForceVoltage(1, .032);
            var ppmuVoltageMeasurements = ppmuSession.PPMUMeasure(PpmuMeasurementType.Voltage); // Returns a 2D jagged array of double of measurement type

            ppmuSession.PinQueryContext.Publish(ppmuVoltageMeasurements, "Voltage");
        }

        /// <summary>
        /// Performs NI-Digital capture waveforms.
        /// </summary>
        /// <param name="tsmContext">The ISemiconductorModuleContext object.</param>
        /// <param name="pin">Pin name.</param>
        public static void NIDigitalExample(ISemiconductorModuleContext tsmContext, string pin)
        {
            var digiSession = InstrCtrl.DigitalPinsToSessions(tsmContext, pin);

            digiSession.BurstPattern("example");
            var passFail = digiSession.GetSitePassFail();

            digiSession.PinQueryContext.Publish(passFail);

            var captureData = digiSession.FetchCaptureWaveform("Example Waveform", 32);

            digiSession.PublishCaptureData(captureData, "Capture");

        }

        /// <summary>
        /// Performs relay actions for the relays.
        /// </summary>
        /// <param name="tsmContext">The ISemiconductorModuleContext object.</param>
        /// <param name="pin">Pin name.</param>
        public static void NISwitchControlRelays(ISemiconductorModuleContext tsmContext, string pin) // This might be updated but this is how we use it for now
        {
            var switchSession = InstrCtrl.CustomRelayPinsToSessions(tsmContext, pin);

            switchSession.ControlRelays(closeRelays: true); // closes the relay
            switchSession.ControlRelays(closeRelays: false); // opens the relay
        }

        /// <summary>
        /// Acquires wavefroms from Scope, calculate and publish total waveform average, mininum and maximum values.
        /// </summary>
        /// <param name="semiconductorModuleContext">The ISemiconductorModuleContext object.</param>
        /// <param name="pins">Pin names with connection to NI-Scope devices.</param>
        public static void NIScopeMeasureVref(ISemiconductorModuleContext semiconductorModuleContext, string[] pins)
        {
            var scope = InstrCtrl.ScopePinsToSessions(semiconductorModuleContext, pins);
            var data = scope.Read(PrecisionTimeSpan.FromSeconds(5), -1);

            CalculateWaveformMaxMinAverage(
                data, 
                out double[][] totalMinValues, 
                out double[][] totalMaxValues, 
                out double[][] totalAverageValues);

            scope.PinQueryContext.Publish(totalAverageValues, "DCValue");
            scope.PinQueryContext.Publish(totalMinValues, "Min");
            scope.PinQueryContext.Publish(totalMaxValues, "Max");
        }

        private static void CalculateWaveformMaxMinAverage(AnalogWaveformCollection<double>[] data, out double[][] totalMinValues, out double[][] totalMaxValues, out double[][] totalAverageValues)
        {
            totalMinValues = new double[data.Length][];
            totalMaxValues = new double[data.Length][];
            totalAverageValues = new double[data.Length][];

            for (int i = 0; i < data.Length; i++)
            {
                var waveforms = data[i];
                double[] minValues = new double[waveforms.Count];
                double[] maxValues = new double[waveforms.Count];
                double[] averageValues = new double[waveforms.Count];

                for (int j = 0; j < waveforms.Count; j++)
                {
                    var sampleValues = waveforms[j].Samples.Select(sample => sample.Value);
                    minValues[j] = sampleValues.Min();
                    maxValues[j] = sampleValues.Max();
                    averageValues[j] = sampleValues.Average();
                }
                totalMinValues[i] = minValues;
                totalMaxValues[i] = maxValues;
                totalAverageValues[i] = averageValues;
            }
        }
    }
}
