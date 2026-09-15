using System;
using System.Linq;
using static TestSteps.Common.DAQmxRelay;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;

namespace TestSteps.P1Checker
{
    public class All_DC90_Check
    {
        private const string DmmPin = "P143_4081_DMM";
        private const double CurrentForceLevel = 1e-3;
        private const double CurrentLevelRange = 10e-3;
        private const double VoltageComplianceLimit = 5.0;
        private const double VoltageLimitRange = 6.0;
        private const double DmmApertureSeconds = 1e-3;
        private const double DmmVoltageRange = 100.0;

        private static readonly string[] _dc90Pins = new[]
        {
            "DC90V_SL23_1A", "DC90V_SL23_2A", "DC90V_SL23_3A", "DC90V_SL23_4A",
            "DC90V_SL23_1B", "DC90V_SL23_2B", "DC90V_SL23_3B", "DC90V_SL23_4B"
        };

        private static readonly string[] _onBoardRelayD0Pins = new[]
        {
            "P127_6368_DIG0_P0_0", "P127_6368_DIG0_P0_1", "P127_6368_DIG0_P0_2", "P127_6368_DIG0_P0_3",
            "P127_6368_DIG0_P0_4", "P127_6368_DIG0_P0_5", "P127_6368_DIG0_P0_6", "P127_6368_DIG0_P0_7",
            "P115_6368_DIG0_P0_8", "P115_6368_DIG0_P0_9", "P115_6368_DIG0_P0_10", "P115_6368_DIG0_P0_11",
            "P115_6368_DIG0_P0_12", "P115_6368_DIG0_P0_13", "P115_6368_DIG0_P0_14", "P115_6368_DIG0_P0_15"
        };

        private static readonly string[] _onBoardRelayD1Pins = new[]
        {
            "P115_6368_DIG1_P0_16", "P115_6368_DIG1_P0_17", "P115_6368_DIG1_P0_18", "P115_6368_DIG1_P0_19",
            "P115_6368_DIG1_P0_20", "P115_6368_DIG1_P0_21", "P115_6368_DIG1_P0_22", "P115_6368_DIG1_P0_23",
            "P115_6368_DIG1_P0_24", "P115_6368_DIG1_P0_25", "P115_6368_DIG1_P0_26", "P115_6368_DIG1_P0_27",
            "P115_6368_DIG1_P0_28", "P115_6368_DIG1_P0_29", "P115_6368_DIG1_P0_30", "P115_6368_DIG1_P0_31"
        };

        public static void DC90VCheck(ISemiconductorModuleContext tsmContext)
        {
            DCPower smu = InstrCtrl.DCPowerPinsToSessions(tsmContext, _dc90Pins);
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmPin);

            dmm.Abort();
            dmm.ConfigureDmmSessions(
                DmmMeasurementFunction.DCVolts,
                DmmApertureTimeUnits.Seconds,
                DmmApertureSeconds, DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: DmmVoltageRange);
            dmm.Initiate();

            smu.Abort();
            smu.ConfigureOutputFunction(DCPowerSourceOutputFunction.DCCurrent);
            smu.ConfigureCurrentLevel(CurrentForceLevel);
            smu.ConfigureCurrentLevelRange(CurrentLevelRange);
            smu.ConfigureVoltageLimit(VoltageComplianceLimit);
            smu.ConfigureVoltageLimitRange(VoltageLimitRange);
            smu.ConfigureSense(DCPowerMeasurementSense.Remote);
            smu.ConfigureOutputEnabled(true);
            smu.Initiate();

            try
            {
                HMODControl.THMODReset(tsmContext);
                HMODControl.HMOD14to18(tsmContext, HMOD_Data_18: HMODControl.RelayID("K23"));

                DaqRelayDrive(tsmContext, _onBoardRelayD0Pins, true);

                smu.Measure(out double[] voltages, out double[] currents);
                smu.PinQueryContext.Publish(voltages, "Voltage");

                DaqRelayDrive(tsmContext, _onBoardRelayD0Pins, false);

                for (int i = 0; i < _onBoardRelayD0Pins.Length; i++)
                {
                    double[] baseline = dmm.Read();
                    DaqRelayDrive(tsmContext, _onBoardRelayD0Pins[i], true);
                    double[] loaded = dmm.Read();
                    DaqRelayDrive(tsmContext, _onBoardRelayD0Pins[i], false);
                    dmm.PinQueryContext.Publish(new double[] { baseline[0] - loaded[0] }, $"Relay{i + 1}_Droop");
                }

                for (int i = 0; i < _onBoardRelayD1Pins.Length; i++)
                {
                    double[] baseline = dmm.Read();
                    DaqRelayDrive(tsmContext, _onBoardRelayD1Pins[i], true);
                    double[] loaded = dmm.Read();
                    DaqRelayDrive(tsmContext, _onBoardRelayD1Pins[i], false);
                    dmm.PinQueryContext.Publish(new double[] { baseline[0] - loaded[0] }, $"Relay{i + 17}_Droop");
                }
            }
            finally
            {
                DaqRelayDrive(tsmContext, _onBoardRelayD0Pins, false);
                DaqRelayDrive(tsmContext, _onBoardRelayD1Pins, false);
                smu.ConfigureOutputEnabled(false);
            }
        }
    }
}
