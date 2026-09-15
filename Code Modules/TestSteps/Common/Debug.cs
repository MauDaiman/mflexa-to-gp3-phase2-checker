using System;
using TestSteps.Common;
using NationalInstruments;
using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;

namespace TestSteps.Common
{
    public class Debug
    {
        private const double SettlingTimeSec = 5e-3;
        private const string chmodAcc1 = "SL11_HSD_ACC1";

        public static void SL11Check(ISemiconductorModuleContext tsmContext,
            DCPowerMeasurementSense senseType,
            bool groundMeter = true,
            int meterType = 0)
        {
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, groundMeter); // Grounds the METER LO

            HMODControl.AllHMODReset(tsmContext);

            // Turn ON relays in SL11
            HMODControl.HMOD14to18(tsmContext,
                HMOD_Data_14: HMODControl.RelayRange(1, 32),
                HMOD_Data_15: HMODControl.RelayRange(1, 16));

            Digital ch5 = InstrCtrl.DigitalPinsToSessions(tsmContext, "T_HSD200_SL11_CH5");
            Digital ch6 = InstrCtrl.DigitalPinsToSessions(tsmContext, "T_HSD200_SL11_CH6");

            HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K1, K2"));

            ch5.Abort(); ch6.Abort();
            ch5.SelectFunction(SelectedFunction.Ppmu);
            ch6.SelectFunction(SelectedFunction.Ppmu);

            ch5.PPMUForceVoltage(voltageLevel: 3, currentLimitRange: 10e-3);
            ch6.PPMUForceCurrent(currentLevel: 1e-3, currentLevelRange: 10e-3);
            Globals.TheHdw.Wait(SettlingTimeSec);

            HMODControl.CHMOD1to6(tsmContext, HMOD_Data_2: HMODControl.RelayID72("K2"));

            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType, ForceMode.ForceVoltage);

            ch5.SelectFunction(SelectedFunction.Digital);
            ch6.SelectFunction(SelectedFunction.Digital);

            HMODControl.AllHMODReset(tsmContext);

            meter.Cleanup(tsmContext);
        }
    }
}
