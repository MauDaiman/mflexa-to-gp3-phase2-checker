using NationalInstruments.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;
using System.Diagnostics;
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
        /// Initializes all "AI","AO", "DI" and "DO" taskType tasks for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        public static void SetDAQmxTasks(ISemiconductorModuleContext semiconductorModuleContext)
        {
            ClearDAQmxTasks(semiconductorModuleContext);
            // For Each DAQ Board (Analog Input)
            var AI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("AI", out var AIchannelLists);
            Parallel.For(0, AI_taskNames.Length, i =>
            {
                var AI_task = new NationalInstruments.DAQmx.Task("");
                AI_task.AIChannels.CreateVoltageChannel(AIchannelLists[i], "", AITerminalConfiguration.Differential, -10, 10, AIVoltageUnits.Volts);
                semiconductorModuleContext.SetNIDAQmxTask(AI_taskNames[i], AI_task);
            });
            // For Each DAQ Board (Analog Output)
            var AO_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("AO", out var AOchannelLists);
            Parallel.For(0, AO_taskNames.Length, i =>
            {
                var AO_task = new NationalInstruments.DAQmx.Task("");
                AO_task.AOChannels.CreateVoltageChannel(AOchannelLists[i], "", -10, 10, AOVoltageUnits.Volts);
                semiconductorModuleContext.SetNIDAQmxTask(AO_taskNames[i], AO_task);
            });
            // For Each DAQ Board (Digital Input)
            var DI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("DI", out var DIchannelLists);
            Parallel.For(0, DI_taskNames.Length, i =>
            {
                var DI_task = new NationalInstruments.DAQmx.Task("");
                DI_task.DIChannels.CreateChannel(DIchannelLists[i], "", ChannelLineGrouping.OneChannelForEachLine);
                semiconductorModuleContext.SetNIDAQmxTask(DI_taskNames[i], DI_task);
            });
            // For Each DAQ Board (Digital Output)
            var DO_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("DO", out var DOchannelLists);
            Parallel.For(0, DO_taskNames.Length, i =>
            {
                var DO_task = new NationalInstruments.DAQmx.Task("");
                DO_task.DOChannels.CreateChannel(DOchannelLists[i], "", ChannelLineGrouping.OneChannelForEachLine);
                semiconductorModuleContext.SetNIDAQmxTask(DO_taskNames[i], DO_task);
            });
        }

        /// <summary>
        /// Initializes "AI" taskType tasks for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="samplingRate">Specifies the sampling rate in samples per channel per second. If you use an external source for the Sample Clock, set this input to the maximum expected rate of that clock.</param>
        /// <param name="sampleSize">Specifies the number of samples to acquire or generate for each channel.</param>
        public static void SetDAQmxAITasks(ISemiconductorModuleContext semiconductorModuleContext, double samplingRate = 100, int sampleSize = 1)
        {
            var AI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("AI", out var AIchannelLists);

            Parallel.For(0, AI_taskNames.Length, i =>
            {
                var AI_task = new NationalInstruments.DAQmx.Task(AI_taskNames[i]);
                AI_task.AIChannels.CreateVoltageChannel(AIchannelLists[i], "", AITerminalConfiguration.Differential, -1, 1, AIVoltageUnits.Volts);
                AI_task.Timing.SampleClockRate = samplingRate;
                AI_task.Timing.SampleQuantityMode = SampleQuantityMode.FiniteSamples;
                AI_task.Timing.SamplesPerChannel = sampleSize;
                AI_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(AI_taskNames[i], AI_task);
            });
        }

        /// <summary>
        /// Initializes "CI" taskType tasks (Frequency Channel) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="minimumValue">The minimum value expected from the measurement, in units (as specified in units parameter).</param>
        /// <param name="maximumValue">The maximum value expected from the measurement, in units (as specified in units parameter).></param>
        /// <param name="measurementTime">The length of time to measure the frequency of a digital signal. Measurement accuracy increases with increased measurement time and with increased signal frequency. If you measure a high-frequency signal for too long, the count register might roll over, resulting in an incorrect measurement.</param>
        /// <param name="divisor">The value by which to divide the input signal, the larger the divisor, the more accurate the measurement. However, too large a value might cause the count register to roll over, resulting in an incorrect measurement.</param>
        /// <param name="edge">The edges between which to measure the frequency.</param>
        /// <param name="method">The method to use to calculate the frequency of the signal.</param>
        /// <param name="units">The units to use to return the measurement (Default value is set to Hertz).</param>
        public static void SetDAQmxCIFrequencyChannelTasks(ISemiconductorModuleContext semiconductorModuleContext, double minimumValue, double maximumValue, double measurementTime, long divisor, CIFrequencyStartingEdge edge, CIFrequencyMeasurementMethod method, CIFrequencyUnits units = CIFrequencyUnits.Hertz)
        {
            var CI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CI", out var CIchannelLists);

            Parallel.For(0, CI_taskNames.Length, i =>
            {
                var CI_task = new NationalInstruments.DAQmx.Task(CI_taskNames[i]);
                CI_task.CIChannels.CreateFrequencyChannel(CIchannelLists[i], "", minimumValue, maximumValue, edge, method, measurementTime, divisor, units);
                CI_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CI_taskNames[i], CI_task);
            });
        }

        /// <summary>
        /// Initializes "CI" taskType tasks (Period Channel) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="minimumValue">The minimum value expected from the measurement, in units (as specified in units parameter).</param>
        /// <param name="maximumValue">The maximum value expected from the measurement, in units (as specified in units parameter).></param>
        /// <param name="measurementTime">The length of time to measure the period of a digital signal. Measurement accuracy increases with increased measurement time and with increased signal frequency. If you measure a high-frequency signal for too long, the count register might roll over, resulting in an incorrect measurement.</param>
        /// <param name="divisor">The value by which to divide the input signal, the larger the divisor, the more accurate the measurement. However, too large a value might cause the count register to roll over, resulting in an incorrect measurement.</param>
        /// <param name="edge">The edges between which to measure the period of the signal.</param>
        /// <param name="method">The method to use to calculate the period of the signal.</param>
        /// <param name="units">The units to use to return the measurement (Default value is set to seconds).</param>
        public static void SetDAQmxCIPeriodChannelTasks(ISemiconductorModuleContext semiconductorModuleContext, double minimumValue, double maximumValue, double measurementTime, long divisor, CIPeriodStartingEdge edge, CIPeriodMeasurementMethod method, CIPeriodUnits units = CIPeriodUnits.Seconds)
        {
            var CI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CI", out var CIchannelLists);

            Parallel.For(0, CI_taskNames.Length, i =>
            {
                var CI_task = new NationalInstruments.DAQmx.Task(CI_taskNames[i]);
                CI_task.CIChannels.CreatePeriodChannel(CIchannelLists[i], "", minimumValue, maximumValue, edge, method, measurementTime, divisor, units);
                CI_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CI_taskNames[i], CI_task);
            });
        }

        /// <summary>
        /// Initializes "CI" taskType tasks (Pulse Channel Frequency) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="minimumValue">The minimum value expected from the measurement, in units (as specified in units parameter).</param>
        /// <param name="maximumValue">The maximum value expected from the measurement, in units (as specified in units parameter).></param>
        /// <param name="units">The units to use to define pulse frequency (default values is Hertz).</param>
        /// <remarks>Generates Counter Input channel tasks to measure digital pulses defined by frequency and duty cycle.</remarks>
        public static void SetDAQmxCIPulseChannelFrequencyTasks(ISemiconductorModuleContext semiconductorModuleContext, double minimumValue, double maximumValue, CIPulseFrequencyUnits units = CIPulseFrequencyUnits.Hertz)
        {
            var CI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CI", out var CIchannelLists);

            Parallel.For(0, CI_taskNames.Length, i =>
            {
                var CI_task = new NationalInstruments.DAQmx.Task(CI_taskNames[i]);
                CI_task.CIChannels.CreatePulseChannelFrequency(CIchannelLists[i], "", minimumValue, maximumValue, units);
                CI_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CI_taskNames[i], CI_task);
            });
        }

        /// <summary>
        /// Initializes "CO" taskType tasks (Pulse Channel Frequency) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="initialDelay">The amount of time in seconds to wait before generating the first pulse.</param>
        /// <param name="frequency">The frequency at which to generate the pulse.</param>
        /// <param name="dutyCycle">The width of the pulse divided by pulse period. NI-DAQmx uses this ratio, combined with frequency, to determine both pulse width and the interval between pulses.</param>
        /// <param name="idleState">The resting state of the output terminal.</param>
        /// <param name="units">The units to use to define pulse frequency (default values is Hertz).</param>
        /// <remarks>Generates Counter Output channel tasks to generate digital pulses defined by frequency and duty cycle.</remarks>
        public static void SetDAQmxCOPulseChannelFrequencyTasks(ISemiconductorModuleContext semiconductorModuleContext, double initialDelay, double frequency, double dutyCycle, COPulseIdleState idleState = COPulseIdleState.Low, COPulseFrequencyUnits units = COPulseFrequencyUnits.Hertz)
        {
            var CO_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CO", out var COchannelLists);

            Parallel.For(0, CO_taskNames.Length, i =>
            {
                var CO_task = new NationalInstruments.DAQmx.Task(CO_taskNames[i]);
                CO_task.COChannels.CreatePulseChannelFrequency(COchannelLists[i], "", units, idleState, initialDelay, frequency, dutyCycle);
                CO_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CO_taskNames[i], CO_task);
            });
        }

        /// <summary>
        /// Initializes "CI" taskType tasks (Pulse Channel Time) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="minimumValue">The minimum value expected from the measurement, in seconds.</param>
        /// <param name="maximumValue">The maximum value expected from the measurement, in seconds.></param>
        /// <remarks>Generates Counter Input channel tasks to measure digital pulses defined by the amount of time the pulse is at a high and low state. </remarks>
        public static void SetDAQmxCIPulseChannelTimeTasks(ISemiconductorModuleContext semiconductorModuleContext, double minimumValue, double maximumValue)
        {
            var CI_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CI", out var CIchannelLists);

            Parallel.For(0, CI_taskNames.Length, i =>
            {
                var CI_task = new NationalInstruments.DAQmx.Task(CI_taskNames[i]);
                CI_task.CIChannels.CreatePulseChannelTime(CIchannelLists[i], "", minimumValue, maximumValue, CIPulseTimeUnits.Seconds);
                CI_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CI_taskNames[i], CI_task);
            });
        }

        /// <summary>
        /// Initializes "CO" taskType tasks (Pulse Channel Time) for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="initialDelay">The amount of time in seconds to wait before generating the first pulse.</param>
        /// <param name="lowTime">The amount of time that the pulse is low, in seconds.</param>
        /// <param name="highTime">The amount of time that the pulse is high, in seconds.></param>
        /// <param name="idleState">The resting state of the output terminal.</param>
        public static void SetDAQmxCOPulseChannelTimeTasks(ISemiconductorModuleContext semiconductorModuleContext, double initialDelay, double lowTime, double highTime, COPulseIdleState idleState = COPulseIdleState.Low)
        {
            var CO_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("CO", out var COchannelLists);

            Parallel.For(0, CO_taskNames.Length, i =>
            {
                var CO_task = new NationalInstruments.DAQmx.Task(CO_taskNames[i]);
                CO_task.COChannels.CreatePulseChannelTime(COchannelLists[i], "", COPulseTimeUnits.Seconds, idleState, initialDelay, lowTime, highTime);
                CO_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(CO_taskNames[i], CO_task);
            });
        }

        /// <summary>
        /// Initializes "AO" taskType tasks for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="samplingRate">Specifies the sampling rate in samples per channel per second. If you use an external source for the Sample Clock, set this input to the maximum expected rate of that clock.</param>
        /// <param name="sampleSize">Specifies the number of samples to acquire or generate for each channel.</param>
        /// <param name="idleOutputBehavior">Specifies the state of the channel when no generation is in progress.</param>
        public static void SetDAQmxAOTasks(ISemiconductorModuleContext semiconductorModuleContext, double samplingRate = 100, int sampleSize = 1, AOIdleOutputBehavior idleOutputBehavior = AOIdleOutputBehavior.ZeroVolts)
        {
            var AO_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("AO", out var AOchannelLists);

            Parallel.For(0, AO_taskNames.Length, i =>
            {
                var AO_task = new NationalInstruments.DAQmx.Task(AO_taskNames[i]);
                AO_task.AOChannels.CreateVoltageChannel(AOchannelLists[i], "", -1, 1, AOVoltageUnits.Volts);
                Parallel.For(0, AO_task.AOChannels.Count, index => AO_task.AOChannels[index].IdleOutputBehavior = idleOutputBehavior);
                AO_task.Timing.SampleClockRate = samplingRate;
                AO_task.Timing.SampleQuantityMode = SampleQuantityMode.ContinuousSamples;
                AO_task.Timing.SamplesPerChannel = sampleSize;
                AO_task.Control(TaskAction.Commit);
                semiconductorModuleContext.SetNIDAQmxTask(AO_taskNames[i], AO_task);
            });
        }

        /// <summary>
        /// Initializes "AOFuncGen" taskType tasks for all NI-DAQmx instruments in the pin map associated with the test program.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="offset">The desired offset of the output waveformType.</param>
        /// <param name="amplitude">The desired amplitude of the output waveformType, in units of volts zero-to-peak. Zero and negative values are valid.</param>
        /// <param name="frequency">The desired frequency of the output waveformType.</param>
        /// <param name="type">The AOFunctionGenerationType waveformType to generate.</param>
        public static void SetDAQmxAOFuncGenTasks(ISemiconductorModuleContext semiconductorModuleContext, double offset, double amplitude, double frequency, AOFunctionGenerationType type)
        {
            var AOFuncGen_taskNames = semiconductorModuleContext.GetNIDAQmxTaskNames("AOFuncGen", out var AOchannelLists);

            Parallel.For(0, AOFuncGen_taskNames.Length, i =>
            {
                var AOFuncGen_task = new NationalInstruments.DAQmx.Task(AOFuncGen_taskNames[i]);
                AOFuncGen_task.AOChannels.CreateFunctionGenerationChannel(AOchannelLists[i], "", type, frequency, amplitude, offset);
                semiconductorModuleContext.SetNIDAQmxTask(AOFuncGen_taskNames[i], AOFuncGen_task);
            });
        }

        /// <summary>
        /// Stops and Disposes all initiated tasks for all NI-DAQmx instruments in the Semiconductor Module Context.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        public static void ClearDAQmxTasks(ISemiconductorModuleContext semiconductorModuleContext)
        {
            Parallel.ForEach(semiconductorModuleContext.GetAllNIDAQmxTasks(""), task =>
            {
                task.Stop();
                task.Dispose();
            });
        }

        /// <summary>
        /// Populates the TSM SSC DAQmx cluster for the specified pin.
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="pin">Pin name.</param>
        /// <returns>A DAQmx task object containing all tasks associated with the given pin.</returns>
        public static DAQmx PinsToDAQmxTasks(ISemiconductorModuleContext semiconductorModuleContext, string pin)
        {
            string[] pins = { pin };
            return PinsToDAQmxTasks(semiconductorModuleContext, pins);
        }

        /// <summary>
        /// Populates the TSM SSC DAQmx cluster for the specified pin(s) and pin group(s).
        /// </summary>
        /// <param name="semiconductorModuleContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="pins">Array of Pin names.</param>
        /// <returns>A DAQmx task object containing all tasks associated with the given pin(s) and pin group(s).</returns>
        public static DAQmx PinsToDAQmxTasks(ISemiconductorModuleContext semiconductorModuleContext, string[] pins)
        {
            var pqc = semiconductorModuleContext.GetNIDAQmxTasks(pins, out var tasks, out var channelLists);

            var DAQmxssc = new DAQmxSSC[tasks.Length];

            var siteNumbers = semiconductorModuleContext.SiteNumbers.ToArray();

            for (int taskNdx = 0; taskNdx < tasks.Length; taskNdx++)
            {
                DAQmxssc[taskNdx].DAQmxTask = tasks[taskNdx];
                DAQmxssc[taskNdx].ChannelLists = channelLists[taskNdx];
            }

            return new DAQmx() { PinQueryContext = pqc, Pins = pins, SiteNumbers = siteNumbers, SSC = DAQmxssc };
        }
    }

    /// <summary>
    /// Structure reference to for DAQmx SSC members.
    /// </summary>
    public struct DAQmxSSC
    {
        /// <summary>
        /// NI-DAQmx Task field property.
        /// </summary>
        public NationalInstruments.DAQmx.Task DAQmxTask { get; set; }
        /// <summary>
        /// ChannelLists string property.
        /// </summary>
        public string ChannelLists { get; set; }
    }

    /// <summary>
    /// Class definition for DAQmx Tasks Objects.
    /// </summary>
    public class DAQmx
    {
        /// <summary>
        /// Multi-session pin query context.
        /// </summary>
        public NIDAQmxMultiplePinMultipleTaskQueryContext PinQueryContext { get; set; }
        /// <summary>
        /// DAQmx SSC definition.
        /// </summary>
        public DAQmxSSC[] SSC { get; set; }
        /// <summary>
        /// Site Numbers.
        /// </summary>
        public int[] SiteNumbers { get; set; }
        /// <summary>
        ///  Pin names.
        /// </summary>
        public string[] Pins { get; set; }

        /// <summary>
        /// Alters the state of the task to a commit state.
        /// </summary>
        /// <remarks>
        /// Programs the hardware with all parameters of the task.
        /// </remarks>
        public void Commit() => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Control(TaskAction.Commit));

        /// <summary>
        /// Alters the state of the task to a Start state.
        /// </summary>
        /// <remarks>
        /// Transitions the task to the running state, which begins device input or output.
        /// </remarks>
        public void Start() => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Control(TaskAction.Start));

        /// <summary>
        /// Alters the state of the task to a Stop state.
        /// </summary>
        /// <remarks>
        /// Transitions the task from the running state to the committed state, which ends device input or output.
        /// </remarks>
        public void Stop() => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Control(TaskAction.Stop));

        /// <summary>
        /// Waits for the measurement or generation to complete and returns if it has completed.
        /// </summary>
        /// <param name="timeout">The maximum amount of time in milliseconds to wait for the measurement or generation to complete. This method returns an error if the time elapses. If you set the timeout to -1, the method always waits for the task to complete, regardless of the amount of time needed.</param>
        public void WaitUntilDone(int timeout) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.WaitUntilDone(timeout));

        /// <summary>
        /// Waits for the measurement or generation to complete, regardless of the amount of time needed, and returns if it has completed execution.
        /// </summary>
        public void WaitUntilDone() => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.WaitUntilDone(-1));

        /// <summary>
        /// Waits for the measurement or generation to complete and returns if it has completed execution before the specified System.TimeSpan elapses.
        /// </summary>
        /// <param name="timeout">The maximum System.TimeSpan to wait for the measurement or generation to complete. This method returns an error if the time elapses.</param>
        public void WaitUntilDone(TimeSpan timeout) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.WaitUntilDone(timeout));

        /// <summary>
        /// Checks wheter the tasks contained in the DAQmx Tasks Objects have completed execution.
        /// </summary>
        /// <returns>A boolean array indicating wheter the tasks completed execution.</returns>
        public bool[] IsTaskDone()
        {
            var boolCheck = new bool[SSC.Length];
            Parallel.For(0, SSC.Length, index =>
            {
                boolCheck[index] = SSC[index].DAQmxTask.IsDone;
            });
            return boolCheck;
        }

        /// <summary>
        /// Routes a control signal to the specified terminal.
        /// </summary>
        /// <param name="signal">The trigger, clock, or event to export.</param>
        /// <param name="outputTerminal">The destination of the exported signal. You can also specify a comma-delimited list for multiple terminal names.</param>
        /// <remarks>Because the output terminal can reside on the device that generates the control signal or on a different device, you can use this method to share clocks and triggers between multiple devices.</remarks>
        public void ExportSignal(ExportSignal signal, string outputTerminal) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.ExportSignals.ExportHardwareSignal(signal, outputTerminal));


        /// <summary>
        /// Sets only the duration of the task and the number of samples to acquire or generate without specifying timing.
        /// </summary>
        /// <param name="sampleMode">The duration of the task. A task is either finite and stops once the specified number of samples have been acquired or generated, or it is continuous and continues to acquire or generate samples until the task is explicitly stopped.</param>
        /// <param name="samplesPerChannel">The number of samples to acquire or generate for each channel in the task.</param>
        /// <remarks>
        /// Typically, you use implicit timing when sample timing is not required for the task, such as for a task that uses counters for buffered frequency measurement, buffered period measurement, or pulse train generation. Use this method if the task requires a finite number of samples. The NI-DAQmx driver does not determine if the requested settings are possible until the task is verified. This method does not throw an exception for parameter values that are not compatible with your hardware or other settings in your task.
        /// </remarks>
        public void ConfigureImplicit(SampleQuantityMode sampleMode, int samplesPerChannel) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Timing.ConfigureImplicit(sampleMode, samplesPerChannel));

        /// <summary>
        /// Sets the source of the sample clock, the rate of the sample clock, and the number of samples to acquire or generate.
        /// </summary>
        /// <param name="signalSource">The source terminal of the clock. To use the internal clock of the device, leave this value empty.</param>
        /// <param name="sampleRate">The sampling rate in samples per second. If you use an external source for the sample clock, set this input to the maximum expected rate of that clock.</param>
        /// <param name="activeEdge">The edges of sample clock pulses on which to acquire or generate samples.</param>
        /// <param name="sampleMode">The duration of the task. A task is either finite and stops once the specified number of samples have been acquired or generated, or it is continuous and continues to acquire or generate samples until the task is explicitly stopped.</param>
        /// <param name="samplesPerChannel">The number of samples to acquire or generate if sampleMode is "FiniteSamples". If sample mode is "ContinuousSamples", NI-DAQmx uses this value to determine the buffer size.</param>
        /// <param name="setRefClkSource">Boolean to set if specific reference clock source will be used.</param>
        /// <param name="refClkSource">Specifies the terminal of the signal to use as the Reference Clock.</param>
        /// <remarks>
        /// The NI-DAQmx driver does not determine if the requested settings are possible until the task is verified. this method does not throw an exception for parameter values that are not compatible with your hardware or other settings in your task.
        /// </remarks>
        public void ConfigureTiming(string signalSource, double sampleRate, SampleClockActiveEdge activeEdge, SampleQuantityMode sampleMode, int samplesPerChannel, bool setRefClkSource = false, string refClkSource = "PXIe_Clk100")
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].DAQmxTask.Timing.ConfigureSampleClock(signalSource, sampleRate, activeEdge, sampleMode, samplesPerChannel);
                if (setRefClkSource)
                {
                    SSC[index].DAQmxTask.Timing.ReferenceClockSource = refClkSource;
                }
            });
        }

        /// <summary>
        /// Specifies the terminal configuration of the output channel.
        /// </summary>
        /// <param name="terminalConfig">Output channel terminal configuration (Rse, Differential, Pseudodifferential)</param>
        public void SetAOTermConfig(AOTerminalConfiguration terminalConfig)
        {
            Parallel.ForEach(SSC, ssc =>
            {
                Parallel.For(0, ssc.DAQmxTask.AOChannels.Count, index =>
                {
                    ssc.DAQmxTask.AOChannels[index].TerminalConfiguration = terminalConfig;
                });
            });
        }

        /// <summary>
        /// Reads one or more floating point samples from the single channel Analog Input tasks defined in the Tasks object.
        /// </summary>
        /// <param name="samples">Number of samples to return. Default value is set to 1 sample.</param>
        /// <returns>An array of 1D arrays containing the samples per task. Each element in the 1D array corresponds to a sample from the channel.</returns>
        /// <remarks>
        /// Note that the function is valid only for single channel defined Analog Input tasks.
        /// </remarks>
        public double[][] ReadAnalog(int samples = 1)
        {
            var measurements = new double[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new AnalogSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSample(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes a floating point sample to the single channel Analog Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="data">A 1D array of sample values to write to the tasks (one sample per task).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for single channel defined Analog Output tasks.
        /// </remarks>
        public void WriteAnalog(double[] data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new AnalogSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSample(autoStart, data[index]);
            });
        }

        /// <summary>
        /// Reads one or more floating point samples from the multi-channel Analog Input tasks defined in the Tasks object.
        /// </summary>
        /// <param name="samples">Number of samples to return. Default value is set to 1 sample.</param>
        /// <returns>An array of 2D arrays containing the samples per channel per task. Each element in the first dimension of the 2D array corresponds to to a channel in the task. Each element in the second dimension of the 2D array corresponds to a sample from each of the channels.</returns>
        /// <remarks>
        /// Note that the function is valid only for multi-channel defined tasks.
        /// </remarks>
        public double[][,] ReadAnalogMultiChannel(int samples = 1)
        {
            var measurements = new double[SSC.Length][,];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new AnalogMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSample(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes a floating point sample for each channel of the Analog Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="data">An array of 2D of sample values to write to the each channel in each tasks (Each element in the first dimension of the 2D array corresponds to a channel in the task. Each element in the second dimension of the 2D array corresponds to a sample to write to each channel.).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for multi-channel defined Analog Output tasks.
        /// </remarks>
        public void WriteAnalogMultiChannel(double[][,] data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new AnalogMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteMultiSample(autoStart, data[index]);
            });
        }

        /// <summary>
        /// Reads one or more analog waveform samples from a single channel in a Analog Input tasks in the Tasks object.
        /// </summary>
        /// <param name="samples">The number of samples to read. If you set numberOfSamples to -1 for a continuous acquisition, the read retrieves all samples available in the buffer at the time of the read.</param>
        /// <returns>An array of AnalogWaveform containing samples from each single channel task.</returns>
        public AnalogWaveform<double>[] ReadAnalogWaveform(int samples)
        {
            var analogWaveform = new AnalogWaveform<double>[SSC.Length];

            Parallel.For(0, SSC.Length, index =>
            {
                analogWaveform[index] = new AnalogSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadWaveform(samples);
            });

            return analogWaveform;
        }

        /// <summary>
        /// Writes a waveform to the single channel Analog Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="analogWaveforms">A 1D array of AnalogWaveform (double) values to write to the tasks (one AnalogWaveform per task).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for single channel defined Analog Output tasks.
        /// </remarks>
        public void WriteAnalogWaveform(AnalogWaveform<double>[] analogWaveforms, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new AnalogSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteWaveform(autoStart, analogWaveforms[index]);
            });
        }

        /// <summary>
        /// Reads one or more analog waveform samples from a multi channel in a Analog Input tasks in the Tasks object.
        /// </summary>
        /// <param name="samples">The number of samples to read. If you set numberOfSamples to -1 for a continuous acquisition, the read retrieves all samples available in the buffer at the time of the read.</param>
        /// <returns>An array of AnalogWaveform arrays containing samples from each channel defined in each task.</returns>
        public AnalogWaveform<double>[][] ReadAnalogWaveformMultiChannel(int samples)
        {
            var analogWaveform = new AnalogWaveform<double>[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                analogWaveform[index] = new AnalogMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadWaveform(samples);
            });

            return analogWaveform;
        }

        /// <summary>
        /// Writes a waveform for each channel of the Analog Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="analogWaveforms">An array of AnalogWaveform (double) array values to write to each channel in each of the tasks (one AnalogWaveform per channel per task).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for multi-channel defined Analog Output tasks.
        /// </remarks>
        public void WriteAnalogWaveformMultiChannel(AnalogWaveform<double>[][] analogWaveforms, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new AnalogMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteWaveform(autoStart, analogWaveforms[index]);
            });
        }

        /// <summary>
        /// Reads one or more int samples from a Counter Input task.
        /// </summary>
        /// <param name="samples">The number of samples to read. If you set numberOfSamples to -1 for a continuous acquisition, the read retrieves all samples available in the buffer at the time of the read.</param>
        /// <returns>An array of 1D arrays containing the samples per task. Each element in the 1D array corresponds to a sample from the channel.</returns>
        /// <remarks>
        /// Note that the function is valid only for single channel defined Counter tasks. Use this method when counter samples are returned unscaled, such as for event counting.
        /// </remarks>
        public double[][] ReadCounter(int samples = 1)
        {
            var measurements = new double[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new CounterSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSampleDouble(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes one or more ticks samples to a single Counter Output Channel in the counter output tasks contained in the object.
        /// </summary>
        /// <param name="sample_data">An array of 1D arrays with samples to write to each task. Each element of the 1D array corresponds to a sample to write to the channel.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// NI-DAQmx scales the generated data to the units of the measurement, including any custom scaling you apply to the channel. You specify these units with the create channel methods or the DAQ Assistant.
        /// </remarks>
        public void WriteCounter(CODataTicks[][] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new CounterSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteMultiSample(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads one or more int samples from the multi-channel Counter Input tasks defined in the Tasks object.
        /// </summary>
        /// <param name="samples">Number of samples to return. Default value is set to 1 sample.</param>
        /// <returns>An array of 2D arrays containing the int samples per channel per task. Each element in the first dimension of the 2D array corresponds to to a channel in the task. Each element in the second dimension of the 2D array corresponds to a sample from each of the channels.</returns>
        /// <remarks>
        /// Note that the function is valid only for multi-channel defined Counter tasks. NI-DAQmx scales the returned data to the units of the measurement, including any custom scaling you apply to the channel. You specify these units with the create channel methods or the DAQ Assistant.
        /// </remarks>
        public double[][,] ReadCounterMultiChannel(int samples = 1)
        {
            var measurements = new double[SSC.Length][,];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new CounterMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSampleDouble(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes a single ticks sample to one or more Counter Output Channel objects in the counter output tasks contained in the Tasks object.
        /// </summary>
        /// <param name="sample_data">An array of 1D arrays with samples to write to each task. Each element of the 1D array corresponds to a channel in the task.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// NI-DAQmx scales the generated data to the units of the measurement, including any custom scaling you apply to the channel. You specify these units with the create channel methods or the DAQ Assistant.
        /// </remarks>
        public void WriteCounterMultiChannel(CODataTicks[][] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new CounterMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSample(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads a single Boolean sample from a single Digital Input channel for each of the tasks contained in the Tasks object. The task can contain only a single digital line.
        /// </summary>
        /// <returns>
        /// An array of booleans containing a sample from each task (Single Line).
        /// </returns>
        public bool[] ReadDigital()
        {
            var measurements = new bool[SSC.Length];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new DigitalSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadSingleSampleSingleLine();
            });

            return measurements;
        }

        /// <summary>
        /// Writes a single Boolean sample to a single digital line in each Digital Output task contained in the Tasks object. Each task can contain only a single digital line.
        /// </summary>
        /// <param name="sample_data">An array of Boolean samples to write to each task.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        public void WriteDigital(bool[] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSampleSingleLine(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads a single Boolean sample from a one or more Digital Input channels for each tasks contained in the Tasks object. Each channel can contain a single digital line.
        /// </summary>
        /// <returns>
        /// An array of 1D boolean arrays containing a sample from each channel of each task (Each element of the 1D array corresponds to a channel in the task).
        /// </returns>
        public bool[][] ReadDigitalMultiChannel()
        {
            var measurements = new bool[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new DigitalMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadSingleSampleSingleLine();
            });

            return measurements;
        }

        /// <summary>
        /// Writes a single Boolean sample to one or more Digital Output Channels in each task contained in the Tasks object. Each channel can contain only a single digital line.
        /// </summary>
        /// <param name="sample_data">An array of 1D Boolean array samples to write to each channel of each task. Each element of the 1D array corresponds to a channel within the task.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        public void WriteDigitalMultiChannel(bool[][] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSampleSingleLine(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads one or more 32-bit unsigned integer samples from a single Digital Input Channel in each task contained in the Tasks object (One or more samples per task).
        /// </summary>
        /// <param name="samples">Number of samples to return. Default value is set to 1 sample.</param>
        /// <returns>An array of 1D arrays of 32-bit unsigned integer samples from each of the tasks. Each element in the 1D array corresponds to a sample from the task.</returns>
        /// <remarks>
        /// Use this method for devices with up to 32 lines per port.
        /// </remarks>
        public uint[][] ReadDigitalU32(int samples = 1)
        {
            var measurements = new uint[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new DigitalSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSamplePortUInt32(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes a single 8-bit unsigned integer sample to a single Digital Output Channel for each task contained in the Tasks object (one sample per task).
        /// </summary>
        /// <param name="sample_data">An array of single sample data to write to each task.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>Use this method for devices with up to eight lines per port.</remarks>
        public void WriteDigitalU32(byte[] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSamplePort(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads one or more 32-bit unsigned integer samples from one or more Digital Input Channels for each of the task contained in the Tasks object (One or more samples per channel per task).
        /// </summary>
        /// <param name="samples">Number of samples to return. Default value is set to 1 sample.</param>
        /// <returns>An array of 2D arrays of 32-bit unsigned integer samples from each channel of each tasks. Each element in the first dimension of the 2D array corresponds to a channel from each task. Each element in the second dimension of the 2D array corresponds to a sample from each channel.</returns>
        /// <remarks>
        /// Use this method for devices with up to 32 lines per port.
        /// </remarks>
        public uint[][,] ReadDigitalMultiChannelU32(int samples = 1)
        {
            var measurements = new uint[SSC.Length][,];

            Parallel.For(0, SSC.Length, index =>
            {
                measurements[index] = new DigitalMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadMultiSamplePortUInt32(samples);
            });

            return measurements;
        }

        /// <summary>
        /// Writes a single 8-bit unsigned integer sample to one or more Digital Output Channels for each of the tasks contained in the Tasks object (one sample per channel per task).
        /// </summary>
        /// <param name="sample_data">An array of 1D arrays of 8-bit unsigned integer samples to write to each channel of each task. Each element in the array corresponds to a channel in the task.</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>Use this method for devices with up to eight lines per port.</remarks>
        public void WriteDigitalMultiChannelU32(byte[][] sample_data, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteSingleSamplePort(autoStart, sample_data[index]);
            });
        }

        /// <summary>
        /// Reads one or more digital waveform samples from a single Digital Input Channel from each of the tasks contained in the Tasks object (One or more waveform samples per task).
        /// </summary>
        /// <param name="samples">The number of samples to read. If you set numberOfSamples to -1 for a continuous acquisition, the read retrieves all samples available in the buffer at the time of the read.</param>
        /// <returns> An array of DigitalWaveforms containing samples from the task.</returns>
        public DigitalWaveform[] ReadDigitalWaveform(int samples)
        {
            var digitalWaveform = new DigitalWaveform[SSC.Length];

            Parallel.For(0, SSC.Length, index =>
            {
                digitalWaveform[index] = new DigitalSingleChannelReader(SSC[index].DAQmxTask.Stream).ReadWaveform(samples);
            });

            return digitalWaveform;
        }

        /// <summary>
        /// Reads one or more digital waveform samples from a one or more Digital Input Channels for each of the tasks contained in the Tasks object (One or more waveform samples per channel per task).
        /// </summary>
        /// <param name="samples">The number of samples to read. If you set numberOfSamples to -1 for a continuous acquisition, the read retrieves all samples available in the buffer at the time of the read.</param>
        /// <returns> An array of 1D DigitalWaveform arrays containing samples from each channel of each task.</returns>
        public DigitalWaveform[][] ReadDigitalWaveformMultiChannel(int samples)
        {
            var digitalWaveform = new DigitalWaveform[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                digitalWaveform[index] = new DigitalMultiChannelReader(SSC[index].DAQmxTask.Stream).ReadWaveform(samples);
            });

            return digitalWaveform;
        }

        /// <summary>
        /// Writes a digital waveform for the Digital Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="digitalWaveforms">An array of DigitalWaveform array values to write to each task (one DigitalWaveform per task).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for single-channel defined Digital Output tasks.
        /// </remarks>
        public void WriteDigitalWaveform(DigitalWaveform[] digitalWaveforms, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalSingleChannelWriter(SSC[index].DAQmxTask.Stream).WriteWaveform(autoStart, digitalWaveforms[index]);
            });
        }

        /// <summary>
        /// Writes a digital waveform for each channel of the Digital Output tasks defined in the Tasks object.
        /// </summary>
        /// <param name="digitalWaveforms">An array of 1D DigitalWaveform array values to write to each channel in each of the tasks (one DigitalWaveform per channel per task).</param>
        /// <param name="autoStart">If set to true this method automatically calls "DAQmx.Task.Start" if you do not explicitly call it. You cannot set this parameter to true if you`have installed events on the task. Default value is true.</param>
        /// <remarks>
        /// Note that the function is valid only for multi-channel defined Digital Output tasks.
        /// </remarks>
        public void WriteDigitalWaveformMultiChannel(DigitalWaveform[][] digitalWaveforms, bool autoStart = true)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                new DigitalMultiChannelWriter(SSC[index].DAQmxTask.Stream).WriteWaveform(autoStart, digitalWaveforms[index]);
            });
        }

        /// <summary>
        /// Specifies the sampling rate in samples per channel per second. If you use an external source for the Sample Clock, set this input to the maximum expected rate of that clock.
        /// </summary>
        /// <param name="sampleClkRate">Sampling rate in samples per channel per second</param>
        public void SetSampleClkRate(double sampleClkRate) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Timing.SampleClockRate = sampleClkRate);

        /// <summary>
        /// Reads the sampling rate in samples per channel per second for all channels in the tasks.
        /// </summary>
        /// <returns>An array of sample rate values per channel per task.</returns>
        public double[] GetSampleClkRate()
        {
            var sampleClkRates = new double[SSC.Length];

            Parallel.For(0, SSC.Length, index =>
            {
                sampleClkRates[index] = SSC[index].DAQmxTask.Timing.SampleClockRate;
            });

            return sampleClkRates;
        }

        /// <summary>
        /// Reads the counter output duty cycle value of the pulses for all channels in the tasks.
        /// </summary>
        /// <returns>An array of duty cycle values per channel per task.</returns>
        public double[][] GetCounterOutputPulseDutyCycle()
        {
            var dutyCycles = new double[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                dutyCycles[index] = new double[SSC[index].DAQmxTask.COChannels.Count];

                Parallel.For(0, SSC[index].DAQmxTask.COChannels.Count, i =>
                {
                    dutyCycles[index][i] = SSC[index].DAQmxTask.COChannels[i].PulseDutyCycle;
                });
            });

            return dutyCycles;
        }

        /// <summary>
        /// Specifies the duty cycle of the pulses. The duty cycle of a signal is the width of the pulse divided by period. NI-DAQmx uses this ratio and the pulse frequency to determine the width of the pulses and the delay between pulses.
        /// </summary>
        /// <param name="dutyCycle">Duty cycle value of the pulses.</param>
        public void SetCounterOutputPulseDutyCycle(double dutyCycle)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                Parallel.For(0, SSC[index].DAQmxTask.COChannels.Count, i =>
                {
                    SSC[index].DAQmxTask.COChannels[i].PulseDutyCycle = dutyCycle;
                });
            });
        }

        /// <summary>
        /// Reads the counter output pulse frequency value for all channels in the tasks.
        /// </summary>
        /// <returns>An array of frequencies per channel per task.</returns>
        public double[][] GetCounterOutputPulseFrequency()
        {
            var frequencies = new double[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                frequencies[index] = new double[SSC[index].DAQmxTask.COChannels.Count];

                Parallel.For(0, SSC[index].DAQmxTask.COChannels.Count, i =>
                {
                    frequencies[index][i] = SSC[index].DAQmxTask.COChannels[i].PulseFrequency;
                });
            });

            return frequencies;
        }

        /// <summary>
        /// Specifies the frequency of the pulses to generate for all counter output channels in the tasks.
        /// </summary>
        /// <param name="frequency">Frequency of the pulses to generate.</param>
        public void SetCounterOutputPulseFrequency(double frequency)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                Parallel.For(0, SSC[index].DAQmxTask.COChannels.Count, i =>
                {
                    SSC[index].DAQmxTask.COChannels[i].PulseFrequency = frequency;
                });
            });
        }

        /// <summary>
        /// Reads the counter output pulse terminal value for all channels in the tasks.
        /// </summary>
        /// <returns>An array of output pulse terminal value per channel per task</returns>
        public string[][] GetCounterOutputPulseTerminal()
        {
            var terminals = new string[SSC.Length][];

            Parallel.For(0, SSC.Length, index =>
            {
                terminals[index] = new string[SSC[index].DAQmxTask.COChannels.Count];

                Parallel.For(0, SSC[index].DAQmxTask.COChannels.Count, i =>
                {
                    terminals[index][i] = SSC[index].DAQmxTask.COChannels[i].PulseTerminal;
                });
            });

            return terminals;
        }

        /// <summary>
        /// Specifies whether to enable averaging mode for Sample Clock-timed frequency measurements.
        /// </summary>
        /// <param name="enable">Enabler flag, to enable averagin mode set to true.</param>
        public void SetCIFrequencyEnableAveraging(bool enable)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                Parallel.For(0, SSC[index].DAQmxTask.CIChannels.Count, i =>
                {
                    SSC[index].DAQmxTask.CIChannels[i].FrequencyEnableAveraging = enable;
                });
            });
        }

        /// <summary>
        /// Stops the running tasks and updates the function generation parameters (waveform type, frequency, amplitude, offset).
        /// </summary>
        /// <param name="waveformType">Specifies the waveform to generate.</param>
        /// <param name="frequency">Specifies the frequency of the waveform to generate in hertz.</param>
        /// <param name="amplitude">Specifies the zero-to-peak amplitude of the waveform to generate in volts. Zero and negative values are valid.</param>
        /// <param name="offset">Specifies the voltage offset of the waveform to generate.</param>
        public void ConfigureAOFuncGen(AOFunctionGenerationType waveformType, double frequency, double amplitude, double offset)
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].DAQmxTask.Stop();

                Parallel.For(0, SSC[index].DAQmxTask.AOChannels.Count, i =>
                {
                    SSC[index].DAQmxTask.AOChannels[i].FunctionGenerationType = waveformType;
                    SSC[index].DAQmxTask.AOChannels[i].FunctionGenerationFrequency = frequency;
                    SSC[index].DAQmxTask.AOChannels[i].FunctionGenerationAmplitude = amplitude;
                    SSC[index].DAQmxTask.AOChannels[i].FunctionGenerationOffset = offset;
                });
            });
        }

        /// <summary>
        ///Transitions the task to the running state to begin the analog output function generation. 
        /// </summary>
        /// <remarks>
        /// It will block for the appropriate amount of settling time required by the insturment. 
        /// </remarks>
        public void StartAOFuncGen()
        {
            Parallel.For(0, SSC.Length, index =>
            {
                SSC[index].DAQmxTask.Control(TaskAction.Start);

                var frequencies = new double[SSC[index].DAQmxTask.AOChannels.Count];

                Parallel.For(0, SSC[index].DAQmxTask.AOChannels.Count, i =>
                {
                    frequencies[i] = SSC[index].DAQmxTask.AOChannels[i].FunctionGenerationFrequency;
                });

                // Waits for 80 cycles + 30ms for function to settle
                Wait(80/frequencies.Max()+0.03);
            });
        }

        /// <summary>
        /// Configures the task to start acquiring or generating samples immediately upon starting the task.
        /// </summary>
        public void DisableStartTrigger() => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Triggers.StartTrigger.ConfigureNone());

        /// <summary>
        /// Configures the tasks to start acquiring or generating samples on a rising or falling edge of a digital signal.
        /// </summary>
        /// <param name="triggerSource">The name of a terminal where there is a digital signal to use as the source of the trigger.</param>
        /// <param name="startTriggerEdge">The edge of the digital signal to start acquiring or generating samples.</param>
        /// <remarks>
        /// The NI-DAQmx driver does not determine if the requested settings are possible until the task is verified.
        /// </remarks>
        public void ConfigureDigitalEdgeStartTrigger(string triggerSource, DigitalEdgeStartTriggerEdge startTriggerEdge) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(triggerSource, startTriggerEdge));

        /// <summary>
        /// Configures the tasks to stop the acquisition when the device acquires all pretrigger samples, detects a rising or falling edge of a digital signal, and acquires all post-trigger samples.
        /// </summary>
        /// <param name="triggerSource">The name of the terminal where there is a digital signal to use as the source of the trigger.</param>
        /// <param name="refTriggerEdge">The edge of the digital signal on which the reference trigger occurs.</param>
        /// <param name="preTriggerSamples">The minimum number of samples to acquire per channel before recognizing the reference trigger.</param>
        /// <remarks>
        /// The number of post-trigger samples is equal to the value of SamplesPerChannel minus the value of pretriggerSamples. If pretriggerSamples equals SamplesPerChannel, the measurement or generation stops when the reference trigger occurs. The NI-DAQmx driver does not determine if the requested settings are possible until the task is verified.
        /// </remarks>
        public void ConfigureReferenceTrigger(string triggerSource, DigitalEdgeReferenceTriggerEdge refTriggerEdge, long preTriggerSamples) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Triggers.ReferenceTrigger.ConfigureDigitalEdgeTrigger(triggerSource, refTriggerEdge, preTriggerSamples));

        /// <summary>
        /// Configures the terminal of the signal to use as the synchronization pulse. The synchronization pulse resets the clock dividers and the ADCs/DACs on the device.
        /// </summary>
        /// <param name="syncPulseSource">Specifies the terminal of the signal to use as the synchronization pulse.</param>
        public void ConfigureSyncPulseSource(string syncPulseSource) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Timing.SynchronizationPulseSource = syncPulseSource);

        /// <summary>
        /// Specifies whether to allow NI-DAQmx to generate the same data multiple times.
        /// </summary>
        /// <param name="regenerationMode">Select between AllowRegeneration or DoNotAllowRegeneration</param>
        public void ConfigureRegeneration(WriteRegenerationMode regenerationMode) => Parallel.ForEach(SSC, ssc => ssc.DAQmxTask.Stream.WriteRegenerationMode = regenerationMode);

        /// <summary>
        /// High resolution time delay helper function.
        /// </summary>
        /// <param name="timeInSeconds">Delay time in seconds.</param>
        private static void Wait(double timeInSeconds)
        {
            // Thread.Sleep() has a resolution over 10ms, so use
            // tch to support shorter settling times
            if (timeInSeconds > 0.0)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                double frequency = Stopwatch.Frequency;
                while (true)
                {
                    double elapsedSeconds = stopwatch.ElapsedTicks / frequency;
                    if (elapsedSeconds > timeInSeconds)
                    {
                        break;
                    }
                }
            }
        }
    }
}