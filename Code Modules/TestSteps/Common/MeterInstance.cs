using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;

namespace TestSteps.Common
{
    /// <summary>
    /// Selects which physical meter resource to use for voltage measurement.
    /// </summary>
    public enum MeterType
    {
        /// <summary>PXIe-4137 SMU used as voltage meter via RL1.</summary>
        Smu4137 = 0,
        /// <summary>PXIe-4081 DMM used as current meter via RL2.</summary>
        Dmm4081 = 1
    }

    /// <summary>
    /// Strategy interface for configuring, measuring, publishing, and cleaning up a meter resource.
    /// </summary>
    public interface IMeterStrategy
    {
        /// <summary>Configures the meter instrument and connects the appropriate relay path.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        void Configure(ISemiconductorModuleContext tsmContext);

        /// <summary>Performs a measurement and returns the result array.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        double[] Measure(ISemiconductorModuleContext tsmContext);

        /// <summary>Publishes measured values to the TSM results.</summary>
        /// <param name="values">Measured data array.</param>
        /// <param name="name">Published result name.</param>
        void PublishResult(double[] values, string name);

        /// <summary>Aborts the meter session and disconnects the relay path.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        void Cleanup(ISemiconductorModuleContext tsmContext);
    }

    /// <summary>
    /// Meter strategy using PXIe-4137 SMU as a voltage meter. Routes signal via RL1.
    /// </summary>
    public class Smu4137Strategy : IMeterStrategy
    {
        private const string Dc901A = "DC90V_SL23_1A";
        private DCPower _smu;

        public void Configure(ISemiconductorModuleContext tsmContext)
        {
            _smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, Dc901A);
            Relay.ControlRelay(tsmContext, new string[] { "RL0", "RL1", "RL2" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL1" }, true);
            _smu.ConfigureSettings(
                apertureTime: 10e-3,
                apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            _smu.ConfigureSense(DCPowerMeasurementSense.Remote);
            _smu.ConfigureVoltageLevelRange(6);
            _smu.Initiate();
            _smu.ConfigureOutputConnected(true);
            _smu.ConfigureOutputEnabled(true);
        }

        public double[] Measure(ISemiconductorModuleContext tsmContext)
        {
            _smu.Measure(out double[] voltages, out _);
            return voltages;
        }

        public void PublishResult(double[] values, string name)
        {
            _smu.PinQueryContext.Publish(values, name);
        }

        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            _smu.Abort();
            _smu.ConfigureOutputEnabled(false);
            _smu.ConfigureOutputConnected(false);
            Relay.ControlRelay(tsmContext, new string[] { "RL1" }, false);
        }
    }

    /// <summary>
    /// Meter strategy using PXIe-4081 DMM as a current meter. Routes signal via RL2.
    /// </summary>
    public class Dmm4081Strategy : IMeterStrategy
    {
        private const string DmmP131 = "P131_4081_DMM";
        private Dmm _dmm;

        public void Configure(ISemiconductorModuleContext tsmContext)
        {
            _dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmP131);
            Relay.ControlRelay(tsmContext, new string[] { "RL0", "RL1", "RL2" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL2" }, true);
            _dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCCurrent,
                DmmApertureTimeUnits.Seconds,
                apertureTime: 1e-3,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: 10);
            _dmm.Initiate();
        }

        public double[] Measure(ISemiconductorModuleContext tsmContext)
        {
            return _dmm.Read();
        }

        public void PublishResult(double[] values, string name)
        {
            _dmm.PinQueryContext.Publish(values, name);
        }

        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            _dmm.Abort();
            Relay.ControlRelay(tsmContext, new string[] { "RL2" }, false);
        }
    }

    /// <summary>
    /// Factory for creating meter strategy instances based on MeterType.
    /// </summary>
    public static class MeterFactory
    {
        /// <summary>
        /// Creates an <see cref="IMeterStrategy"/> instance for the specified meter type.
        /// </summary>
        /// <param name="type">The meter type (0 = Smu4137, 1 = Dmm4081).</param>
        public static IMeterStrategy Create(MeterType type)
        {
            switch (type)
            {
                case MeterType.Smu4137:
                    return new Smu4137Strategy();
                case MeterType.Dmm4081:
                    return new Dmm4081Strategy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
