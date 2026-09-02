using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
using static TestSteps.Common.DAQmxRelay;

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
        void Configure(ISemiconductorModuleContext tsmContext, DCPowerMeasurementSense senseType);

        /// <summary>Performs a voltage measurement and returns the result array.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        double[] MeasureVoltage(ISemiconductorModuleContext tsmContext);
        
        /// <summary>Performs a current measurement and returns the result array.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        double[] MeasureCurrent(ISemiconductorModuleContext tsmContext);

        /// <summary>Publishes measured values using tsmContext.PublishPerSite so no Pin input is needed in TestStand.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="values">Measured data array.</param>
        /// <param name="name">Published result name.</param>
        void PublishResult(ISemiconductorModuleContext tsmContext, double[] values, string name);

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

        public void Configure(ISemiconductorModuleContext tsmContext, DCPowerMeasurementSense senseType)
        {
            _smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, Dc901A);
            Relay.ControlRelay(tsmContext, new string[] { "RL0", "RL1", "RL2", "RL13", "RL14" }, false);

            _smu.Abort();
            _smu.ConfigureSense(
                sense: senseType,
                initiateSessionAfter: false);
            _smu.ConfigureSettings(
                apertureTime: 10e-3,
                apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            _smu.ConfigureCurrentLevelRange(currentLevelRange: 10e-3);
            _smu.ConfigureVoltageLimitRange(voltageLimitRange: 20);
            _smu.ConfigureOutputConnected();
            _smu.ConfigureOutputEnabled();

            _smu.ForceCurrent(currentLevel: 0, voltageLimit: 20);

            DaqRelayDrive(tsmContext, new string[] { 
                "P127_6368_DIG0_P0_0", 
                "P127_6368_DIG0_P0_1" }, 
                true);

            Relay.ControlRelay(tsmContext, new string[] { "RL2" }, true);
            Globals.TheHdw.Wait(5e-3);
        }

        public double[] MeasureVoltage(ISemiconductorModuleContext tsmContext)
        {
            _smu.Measure(out double[] voltages, out _);
            return voltages;
        }
        
        public double[] MeasureCurrent(ISemiconductorModuleContext tsmContext)
        {
            _smu.Measure(out _, out double[] currents);
            return currents;
        }

        public void PublishResult(ISemiconductorModuleContext tsmContext, double[] values, string name)
        {
            tsmContext.PublishPerSite(values, name);
        }

        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            _smu.Abort();
            _smu.ConfigureOutputEnabled(false);
            _smu.ConfigureOutputConnected(false);
            DaqRelayDrive(tsmContext, new string[] { 
                "P127_6368_DIG0_P0_0", 
                "P127_6368_DIG0_P0_1" }, 
                false);
            Relay.ControlRelay(tsmContext, new string[] { "RL2" }, false);
        }
    }

    /// <summary>
    /// Meter strategy using PXIe-4081 DMM as a current meter. Routes signal via RL2.
    /// </summary>
    public class Dmm4081Strategy : IMeterStrategy
    {
        private const string DmmP131 = "P131_4081_DMM";
        private Dmm _dmm;

        public void Configure(ISemiconductorModuleContext tsmContext, DCPowerMeasurementSense senseType)
        {
            _dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmP131);
            Relay.ControlRelay(tsmContext, new string[] { "RL0", "RL1", "RL2", "RL13", "RL14" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL1" }, true);
        }

        public double[] MeasureVoltage(ISemiconductorModuleContext tsmContext)
        {
            ConfigureDmm(_dmm, DmmMeasurementFunction.DCVolts, range: 100);
            return _dmm.Read();
        }
        
        public double[] MeasureCurrent(ISemiconductorModuleContext tsmContext)
        {
            ConfigureDmm(_dmm, DmmMeasurementFunction.DCCurrent, range: 10e-3);
            return _dmm.Read();
        }

        public void PublishResult(ISemiconductorModuleContext tsmContext, double[] values, string name)
        {
            tsmContext.PublishPerSite(values, name);
        }

        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            _dmm.Abort();
            Relay.ControlRelay(tsmContext, new string[] { "RL1" }, false);
        }

        /// <summary>
        /// Configures DMM measurement function and range with standard aperture settings.
        /// </summary>
        /// <param name="dmm">The DMM session wrapper.</param>
        /// <param name="function">Measurement function (DCCurrent, DCVolts, TwoWireResistance, FourWireResistance).</param>
        /// <param name="range">Measurement range value.</param>
        private static void ConfigureDmm(Dmm dmm, DmmMeasurementFunction function, double range)
        {
            dmm.Abort();
            dmm.ConfigureDmmSessions(
                function,
                DmmApertureTimeUnits.Seconds,
                apertureTime: 1e-3,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: range);
            dmm.Initiate();
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
