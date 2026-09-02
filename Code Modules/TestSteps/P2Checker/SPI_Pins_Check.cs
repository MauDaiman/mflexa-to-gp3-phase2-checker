using System;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDCPower;
using TestSteps.Common;

namespace TestSteps.P2Checker
{
    public class SPI_Pins_Check
    {
        private const string AllChmodDigiPins = "CHMOD_DIGITAL_PINS";
        private const double SettlingTimeSec = 1e-3;
        private const double RelaySettleSec = 5e-3;

        /// <summary>
        /// Checks if the signal from the checker board SPI pins is able to reach the MFlex board.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="meterType">Meter selection: 0 = PXIe-4137 SMU, 1 = PXIe-4081 DMM.</param>
        public static void CheckerBoardSPICheck(ISemiconductorModuleContext tsmContext, 
            DCPowerMeasurementSense senseType, 
            int meterType = 0)
        {
            // Configure the selected meter resource (PXIE-4137 or PXIE-4081)
            IMeterStrategy meter = MeterFactory.Create((MeterType)meterType);
            meter.Configure(tsmContext, senseType);

            // Reset all HMODs (Tx Board and Checker Board)
            HMODControl.AllHMODReset(tsmContext);
            // DaqRelayDrive(tsmContext, dc90RelayPinGroup, true);

            // Turn OFF RL15 to RL18 to connect Checker board
            // SPI Pins resource to the Checker Board HMOD circuit
            Relay.ControlRelay(tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);
            Relay.ControlRelay(tsmContext, new string[] { "RL0" }, true); // Grounds the METER LO

            // Turn ON relays to connect DIO pins to HSD
            HMODControl.HMOD14to18(tsmContext, HMOD_Data_14: HMODControl.RelayID("K1, K2, K3, K4"));

            // Initiate Pins to Session
            Digital chmodPins = InstrCtrl.DigitalPinsToSessions(tsmContext, AllChmodDigiPins);
            chmodPins.Abort();
            // At this point, VIH/VIL should have been already set to 5V/0V during process setup.
            chmodPins.SelectFunction(SelectedFunction.Digital);
            chmodPins.ApplyLevelsandTimings(levelsSheetName: "HMOD", timingsSheetName: "HMOD");
            // Program the SPI pins to force high - vih
            chmodPins.WriteStatic(PinState._1);

            try
            {
                // Connect data pin to METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL15" }, true);
                Globals.TheHdw.Wait(RelaySettleSec);
                // Check/Measure if checker board SPI pins has reached the MFlex board
                double[] dataVoltageChk = meter.MeasureVoltage(tsmContext);
                // Disconnect data pin from METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL15" }, false);
                Globals.TheHdw.Wait(RelaySettleSec);

                // Connect sclk pin to METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL16" }, true);
                Globals.TheHdw.Wait(RelaySettleSec);
                double[] sclkVoltageChk = meter.MeasureVoltage(tsmContext);
                // Disconnect sclk pin from METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL16" }, false);
                Globals.TheHdw.Wait(RelaySettleSec);

                // Connect cs pin to METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL17" }, true);
                Globals.TheHdw.Wait(RelaySettleSec);
                double[] csVoltageChk = meter.MeasureVoltage(tsmContext);
                // Disconnect cs pin from METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL17" }, false);
                Globals.TheHdw.Wait(RelaySettleSec);

                // Connect rst pin to METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL18" }, true);
                Globals.TheHdw.Wait(RelaySettleSec);
                double[] rstVoltageChk = meter.MeasureVoltage(tsmContext);
                // Disconnect rst pin from METER HI
                Relay.ControlRelay(tsmContext, new string[] { "RL18" }, false);
                Globals.TheHdw.Wait(RelaySettleSec);

                // Publish results
                meter.PublishResult(tsmContext, dataVoltageChk, "DATA_Voltage_Check");
                meter.PublishResult(tsmContext, sclkVoltageChk, "SCLK_Voltage_Check");
                meter.PublishResult(tsmContext, csVoltageChk, "CS_Voltage_Check");
                meter.PublishResult(tsmContext, rstVoltageChk, "RESET_Voltage_Check");
            }
            finally
            {
                chmodPins.WriteStatic(PinState._0);
                chmodPins.Abort();
                meter.Cleanup(tsmContext);
                Relay.ControlRelay(tsmContext, new string[] { "RL0" }, false);
            }
        }
    }
}
