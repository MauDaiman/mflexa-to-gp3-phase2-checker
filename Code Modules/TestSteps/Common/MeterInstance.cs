using System;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;

namespace TestSteps.Common
{
    public enum MeterType
    {
        Smu4137,
        Dmm4081
    }

    public interface IMeterStrategy
    {
        void Configure(ISemiconductorModuleContext tsmContext);
        double[] Measure(ISemiconductorModuleContext tsmContext);
        void PublishResult(double[] values, string name);
    }

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
    }

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
    }

    public static class MeterFactory
    {
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

    public static class MeterInstance
    {
        public static void PowerDown_Checker(ISemiconductorModuleContext tsmContext)
        {
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, "P131_4081_DMM");
            dmm.Abort();
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                apertureTime: 1e-3,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: 10);
            dmm.Initiate();

            DCPower smuDC90 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DC90_PINS");
            smuDC90.Abort();
            smuDC90.ForceVoltage(voltageLevel: 0, currentLimit: 10e-3);
            smuDC90.ConfigureVoltageLevelRange(6);
            smuDC90.Initiate();
            smuDC90.ConfigureOutputConnected();
            smuDC90.ConfigureOutputEnabled();

            DCPower smuDC30 = InstrCtrl.DCPowerPinsToSessions(tsmContext,
                new string[] { "SL24_DC30", "SL10_DC30", "SL04_DC30" });
            smuDC30.ForceVoltage(voltageLevel: 0, currentLimit: 10e-3);
        }
    }
}
