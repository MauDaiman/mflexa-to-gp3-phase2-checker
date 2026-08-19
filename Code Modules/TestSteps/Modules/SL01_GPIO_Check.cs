using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDmm;

namespace TestSteps.Modules
{
    public class SL01_GPIO_Check
    {
        private const string DmmP131 = "P131_4081_DMM";
        private const double SettlingTimeSec = 1e-3;
        private const double ApertureTimeSec = 1e-3;

        /// <summary>
        /// Check the Ammeter, Voltmeter and Ohmmeter (2W, 4W) functionality of the PXIE-4081.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public static void SL01Check(ISemiconductorModuleContext tsmContext)
        {
            Dmm dmm = InstrCtrl.DmmPinsToSessions(tsmContext, DmmP131);
            dmm.Abort();

            // Configure DMM (METER_4081) as ammeter
            ConfigureDmm(dmm, DmmMeasurementFunction.DCCurrent, range: 10e-3);
            dmm.Initiate();

            // Turn ON K21 of HMOD13 of the TXBoard
            // to connect P131_4081_DMM to T_GPIO_SL01_I
            HMODControl.HMOD11to13(tsmContext, HMOD_Data_13: HMODControl.RelayID("K21"));

            // Turn ON RL3 and RL6 of the CXboard to route
            // the current output of Q1 current source to T_GPIO_SL01_I
            Relay.ControlRelay(tsmContext, new string[] { "RL3", "RL6" }, true);

            Globals.TheHdw.Wait(SettlingTimeSec);
            double[] iout = dmm.Read();

            // Disconnect T_GPIO_SL01_I from the Drain of Q1.
            // Route Current source output to AGND.
            Relay.ControlRelay(tsmContext, new string[] { "RL6" }, false);

            // DMM set as voltmeter
            dmm.Abort();
            ConfigureDmm(dmm, DmmMeasurementFunction.DCVolts, range: 100);
            dmm.Initiate();
            Globals.TheHdw.Wait(SettlingTimeSec);

            double[] pos15V = dmm.Read();

            // Turn ON RL4 to connect T_GPIO_SL01_2W-HI and T_GPIO_SL01_2W-LO
            // across 6K resistor network load for 2W resistance measurement check
            Relay.ControlRelay(tsmContext, "RL4", true);

            // DMM as ohmmeter (2-wire)
            dmm.Abort();
            ConfigureDmm(dmm, DmmMeasurementFunction.TwoWireResistance, range: 10e3);
            dmm.Initiate();
            Globals.TheHdw.Wait(SettlingTimeSec);

            double[] resMeas2W = dmm.Read();

            // Turn ON RL5 to connect T_GPIO_SL01_4W-HI and T_GPIO_SL01_4W-LO
            // across 1K resistor network load for 4W resistance measurement check
            Relay.ControlRelay(tsmContext, "RL5", true);

            // DMM as ohmmeter (4-wire)
            dmm.Abort();
            ConfigureDmm(dmm, DmmMeasurementFunction.FourWireResistance, range: 10e3);
            dmm.Initiate();
            Globals.TheHdw.Wait(SettlingTimeSec);

            double[] resMeas4W = dmm.Read();

            dmm.Abort();

            // Configure back PXIE-4081 to voltmeter mode for future use
            ConfigureDmm(dmm, DmmMeasurementFunction.DCVolts, range: 100);
            dmm.Initiate();

            // Reset all HMODs
            HMODControl.AllHMODReset(tsmContext);

            // Turn OFF Slot1 checker circuit relays to go back to their default settings,
            // and also isolate the slot1 circuitry from the PXIE-4081 DMM
            Relay.ControlRelay(tsmContext, new string[] { "RL3", "RL4", "RL5", "RL6" }, false);

            // Publish results
            dmm.PinQueryContext.Publish(iout, "DMM_Current_Measurement");
            dmm.PinQueryContext.Publish(pos15V, "DMM_Voltage_Measurement");
            dmm.PinQueryContext.Publish(resMeas2W, "DMM_2W_Resistance_Measurement");
            dmm.PinQueryContext.Publish(resMeas4W, "DMM_4W_Resistance_Measurement");
        }

        /// <summary>
        /// Configures DMM measurement function and range with standard aperture settings.
        /// </summary>
        /// <param name="dmm">The DMM session wrapper.</param>
        /// <param name="function">Measurement function (DCCurrent, DCVolts, TwoWireResistance, FourWireResistance).</param>
        /// <param name="range">Measurement range value.</param>
        private static void ConfigureDmm(Dmm dmm, DmmMeasurementFunction function, double range)
        {
            dmm.ConfigureDmmSessions(
                function,
                DmmApertureTimeUnits.Seconds,
                apertureTime: ApertureTimeSec,
                DmmAuto.Off,
                DmmAdcCalibration.Off,
                settleTimeSeconds: 0,
                voltageRange: range);
        }
    }
}
