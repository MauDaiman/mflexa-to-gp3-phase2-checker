using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using HMOD;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.DAQmx;


namespace TestSteps
{
    public class TxChecker_instances
    {

        public static void DMM_4081_S14_check(ISemiconductorModuleContext tsmContext)
        //Check the Ammeter, Voltmeter and Ohmmeter(2W, 4W) functionality of the PXIE-4081 
        {
            // 4081 as current meter checker 
            //configure DMM(METER_4081) as ammeter
            Globals.dmmglobal = InstrCtrl.DmmPinsToSessions(tsmContext, "METER_4081");
            Globals.dmmglobal.Abort();
            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.DCCurrent, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 10e-3);
            Globals.dmmglobal.Initiate();


            // Turn ON K21 of HMOD13 of the TX Board to connect P131_4081_DMM_HI_S_I_C1_S14 to T_GPIO_SL01_I
            HMODCtrl.HMOD11to13(tsmContext, HMOD_Data_13: 1048576); //b0001 0000 0000 0000 0000 0000

            //Turn ON RL3 and RL6 of the Tx Checker board to route the current output of Q1 current source to T_GPIO_SL01_I
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL3", "RL6" }, true);

            Globals.TheHdw.Wait(Globals.SettlingTime);
            double[] Iout = Globals.dmmglobal.Read();

            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL6" }, false); //disconnect T_GPIO_SL01_I from the Drain of Q1. Route Current source output to AGND.

            // 4081 as current meter checker 
            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.DCVolts, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 100); //DDM set as voltmeter
            Globals.dmmglobal.Initiate();
            Globals.TheHdw.Wait(Globals.SettlingTime);

            double[] Pos15V = Globals.dmmglobal.Read();

            // 4081 as ohmmeter checker 
            //Turn ON RL4 to connect T_GPIO_SL01_2W-HI and T_GPIO_SL01_2W-LO accross 6K resistor network load for 2W resistance measurement check
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL4" }, true);
            // Relay.ControlRelay(Globals.tsmContext, "RL4", true);
            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.TwoWireResistance, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 10e3);
            Globals.dmmglobal.Initiate();
            Globals.TheHdw.Wait(Globals.SettlingTime);

            double[] ResMeas_2W = Globals.dmmglobal.Read();

            //Turn ON RL5 to connect T_GPIO_SL01_4W-HI and T_GPIO_SL01_4W-LO accross 1K resistor network load for 4W resistance measurement check           
            Relay.ControlRelay(Globals.tsmContext, "RL5", true);

            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.FourWireResistance, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 10e3);
            Globals.dmmglobal.Initiate();
            Globals.TheHdw.Wait(Globals.SettlingTime);
            double[] ResMeas_4W = Globals.dmmglobal.Read();

            Globals.dmmglobal.Abort();

            // configure back PXIE-4018 to voltmeter mode for future use
            // 4081 as current meter checker 
            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.DCVolts, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 100); //DDM set as voltmeter
            Globals.dmmglobal.Initiate();

            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital HMOD_Reset = InstrCtrl.DigitalPinsToSessions(tsmContext, "HMOD_RESET");
            HMOD_Reset.WriteStatic(PinState._0);  //Reset all HMOD

            //Turn OFF Slot1 checker circuit relays to go back to their default settings, and also isolate the slot1 circuitry from the PXIE-4081 DMM
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL3", "RL4", "RL5", "RL6" }, false);

            //bin out results            
            Globals.dmmglobal.PinQueryContext.Publish(Iout, "DMM_Current_Measurement");
            Globals.dmmglobal.PinQueryContext.Publish(Pos15V, "DMM_Voltage_Measurement");
            Globals.dmmglobal.PinQueryContext.Publish(ResMeas_2W, "DMM_2W_Resistance_Measurement");
            Globals.dmmglobal.PinQueryContext.Publish(ResMeas_4W, "DMM_4W_Resistance_Measurement");
        }
        public static void ChckBrd_SPI_Pins_Checker(ISemiconductorModuleContext tsmContex)
        //Checks if the signal from the checker board SPI pins is able to reach the MFLEX board 
        {
            double[] data_voltage_chk, sclk_voltage_chk, cs_voltage_chk, rst_voltage_chk;
            bool Meteroption = Globals.MeterResource;

            //Turn OFF RL15 to RL18 to connect Checker board SPI Pins resource to the Checker Board HMOD circuit          
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL15", "RL16", "RL17", "RL18" }, false);

            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital ChckrBrd_HMOD_Pins = InstrCtrl.DigitalPinsToSessions(tsmContex, "ChckrBrd_HMOD_Pins"); //initiate Pins to Session

            //At this point, VIH/VIL should have been already set  to 5V/0V during process setup.
            ChckrBrd_HMOD_Pins.ConfigureVoltgeLevels(0, 5, 0, 0, 0); //set VIL=0V, VIH=5V
            ChckrBrd_HMOD_Pins.WriteStatic(PinState._1); //program the SPI pins to force high - vih

            //connect data pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL15" }, true);

            //Check/Measure if checker board SPI pins has reached the MFLEX board
            data_voltage_chk = MeasureVoltage(tsmContex, Meteroption);

            //disconnect data pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL15" }, false);

            //connect scl pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL16" }, true);

            //Check/Measure if checker board SPI pins has reached the MFLEX board
            sclk_voltage_chk = MeasureVoltage(tsmContex, Meteroption);

            //disconnect scl pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL16" }, false);

            //connect cs pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL17" }, true);

            //Check/Measure if checker board SPI pins has reached the MFLEX board
            cs_voltage_chk = MeasureVoltage(tsmContex, Meteroption);

            //disconnect cs pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL17" }, false);

            //connect rst pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL18" }, true);

            //Check/Measure if checker board SPI pins has reached the MFLEX board
            rst_voltage_chk = MeasureVoltage(tsmContex, Meteroption);

            //disconnect rst pin to METER HI
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL18" }, false);

            ChckrBrd_HMOD_Pins.WriteStatic(PinState._0); //program the SPI pins to force low - vil

            //bin out results        
            if (Meteroption) //Using PXIE-4137 resource
            {

                Globals.DC90_1A_Global.PinQueryContext.Publish(data_voltage_chk, "DATA_voltage_check");
                Globals.DC90_1A_Global.PinQueryContext.Publish(sclk_voltage_chk, "SCLK_voltage_check");
                Globals.DC90_1A_Global.PinQueryContext.Publish(cs_voltage_chk, "CS_voltage_check");
                Globals.DC90_1A_Global.PinQueryContext.Publish(rst_voltage_chk, "RESET_voltage_check");
            }
            else // Using PXIE-4081
            {
                Globals.dmmglobal.PinQueryContext.Publish(data_voltage_chk, "DATA_voltage_check");
                Globals.dmmglobal.PinQueryContext.Publish(sclk_voltage_chk, "SCLK_voltage_check");
                Globals.dmmglobal.PinQueryContext.Publish(cs_voltage_chk, "CS_voltage_check");
                Globals.dmmglobal.PinQueryContext.Publish(rst_voltage_chk, "RESET_voltage_check");
            }
        }
        public static double[] MeasureVoltage(ISemiconductorModuleContext tsmContex, bool MeterResource)
        //Measures the voltage at the MFLEx board using either the PXIE-4081(True) or PXIE-4081 DMM(False), meter resource defined in  Globals.cs

        {
            Double[] voltage = { 0 };

            Globals.TheHdw.Wait(Globals.SettlingTime);

            if (MeterResource) //Using PXIE-4137 resource
            {
                //Globals.MeasuredVoltage = 
                Globals.DC90_1A_Global.Measure(out voltage, out _);
            }
            else // Using PXIE-4081
            {
                voltage = Globals.dmmglobal.Read();
            }
            return voltage;

        }
        public static void DC90_SLOT23_check(ISemiconductorModuleContext tsmContext)
        //Checks if SMU-4137 signals are able to reach the MFLEx board. Done by forcing a +1mA on the resistor load located at the MFLEX board checker, and measuring the resulting voltage
        {
            PinListData Measured_1k_voltage = new PinListData();
            Double[] DC901A, DC901B, DC902A, DC902B, DC903A, DC903B, DC904A, DC904B;

            //Initiate Pins to session
            DCPower DC90_PINS = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_PINS");
            Globals.DC90_1A_Global = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_1A");
            DCPower DC90_1B = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_1B");
            DCPower DC90_2A = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_2A");
            DCPower DC90_2B = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_2B");
            DCPower DC90_3A = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_3A");
            DCPower DC90_3B = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_3B");
            DCPower DC90_4A = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_4A");
            DCPower DC90_4B = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_4B");

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn ON Tx Board relays K1 - K16(driven by DAQ I/O) to connect PXI-4137 to DC90 pogo

            int[] index = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            DAQmxRelayDriver.CloseRelays(tsmContext, "DC90_RLY_DRIVER", index);
            DAQmxRelayDriver.CloseAllRelays(tsmContext, "DC90_RLY_DRIVER");

            //Turn ON RL13 and RL14 of the Tx Checker board to connect DC90 1A to  the resistor load on the checker board            
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL13", "RL14" }, true);

            //Configure PXIE-4137 to FIMV STL mode, force +/-1mA on 1Kohms load, measure resulting voltage, configure sessions
            //  Globals.TheHdw.DCVI.Pins("DC90_PINS").Mode = Globals.tlDCVIModeCurrent;
            //  Globals.TheHdw.DCVI.Pins("DC90_PINS").SetCurrentAndRange(+1e-3, 10e-3); //set current load and current range 
            //  Globals.TheHdw.DCVI.Pins("DC90_PINS").Meter.Mode = Globals.tlDCVIMeterVoltage;
            //   Globals.TheHdw.DCVI.Pins("DC90_PINS").Meter.VoltageRange = 600e-3; //set meter voltage range
            //  Globals.TheHdw.Wait(Globals.SettlingTime);

            //   Measured_1k_voltage = Globals.TheHdw.DCVI.Pins("DC90_PINS").Meter.Read(Globals.tlStrobe, 10, 10000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage,true);

            //CHECK DIB ACCESS CONNECTION INTEGRITY, STL coding
            //WRITE CODE HERE


            // CHECK KELVIN CONNECTION INTEGRITY, InstCtrl coding  , configure sessions                  
            DC90_PINS.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            DC90_PINS.ConfigureSense(DCPowerMeasurementSense.Remote, initiateSessionAfter: false);
            DC90_PINS.ForceCurrent(currentLevel: +1e-3, voltageLimit: 10); //force current level and set measure voltage range
            DC90_PINS.ConfigureVoltageLevelRange(6);
            DC90_PINS.Initiate();
            DC90_PINS.ConfigureOutputConnected();
            DC90_PINS.ConfigureOutputEnabled();


            Globals.TheHdw.Wait(Globals.SettlingTime);
            Globals.DC90_1A_Global.Measure(out DC901A, out _);
            DC90_1B.Measure(out DC901B, out _);
            DC90_2A.Measure(out DC902A, out _);
            DC90_2B.Measure(out DC902B, out _);
            DC90_3A.Measure(out DC903A, out _);
            DC90_3B.Measure(out DC903B, out _);
            DC90_4A.Measure(out DC904A, out _);
            DC90_4B.Measure(out DC904B, out _);

            Measured_1k_voltage = Globals.TheHdw.DCVI.Pins("DC90_PINS").Meter.Read(Globals.tlStrobe, 10, 10000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage, true);//measure voltage

            DC90_PINS.ForceCurrent(currentLevel: 0, voltageLimit: 10); //force current level and set measure voltage range
            DC90_PINS.Abort();
            DC90_PINS.ConfigureOutputEnabled(false);
            DC90_PINS.ConfigureOutputConnected(false);

            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL13", "RL14" }, false); //connect DC90 1A for the METER_HI option

            //Bin out results
            Globals.TheExec.Flow.TestLimit(resultval: Measured_1k_voltage, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            Globals.DC90_1A_Global.PinQueryContext.Publish(DC901A, "DC901A MEASURED VOLTAGE");
            DC90_1B.PinQueryContext.Publish(DC901B, "DC901B MEASURED VOLTAGE");
            DC90_2A.PinQueryContext.Publish(DC902A, "DC902A MEASURED VOLTAGE");
            DC90_2B.PinQueryContext.Publish(DC902B, "DC902B MEASURED VOLTAGE");
            DC90_3A.PinQueryContext.Publish(DC903A, "DC903A MEASURED VOLTAGE");
            DC90_3B.PinQueryContext.Publish(DC903B, "DC903B MEASURED VOLTAGE");
            DC90_4A.PinQueryContext.Publish(DC904A, "DC904A MEASURED VOLTAGE");
            DC90_4B.PinQueryContext.Publish(DC904B, "DC904B MEASURED VOLTAGE");

            // Globals.TheExec.Flow.TestLimit(resultval: Measured_1k_voltage, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
        }
        public static void Select_METER_option(ISemiconductorModuleContext tsmContext, bool MeterOption)
        //initiate and configure the meter resource to use, meter resource is defined in the Globals.cs, TRUE - PXIE-4137, FALSE - PXIE-4081
        {
            //Turn OFF RL0, RL1 and RL2 first to disconnect all meter and grounding option of LO side
            Relay.ControlRelay(Globals.tsmContext, new string[] { "RL0", "RL1", "RL2" }, false);

            if (MeterOption) //select PXIE - 4137 as Meter
            {
                Relay.ControlRelay(Globals.tsmContext, new string[] { "RL1" }, true); //connect PXIE-4081 DMM to METER_HI/METER_LO
                Globals.DC90_1A_Global.ConfigureSettings(apertureTime: 10E-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
                Globals.DC90_1A_Global.ConfigureSense(DCPowerMeasurementSense.Remote);
                Globals.DC90_1A_Global.ConfigureVoltageLevelRange(6);
                Globals.DC90_1A_Global.Initiate();
                Globals.DC90_1A_Global.ConfigureOutputConnected(true);
                Globals.DC90_1A_Global.ConfigureOutputEnabled(true);
            }
            else //Select PXIE-4081 as Meter
            {
                Relay.ControlRelay(Globals.tsmContext, new string[] { "RL2" }, true); //connect PXIE-4137(DC90_1A) to METER_HI/METER_LO
                Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.DCCurrent, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 10);
            }
        }
        public static void Slot04_DC30_Check(ISemiconductorModuleContext tsmContext)
        //Checks if SMU-4162/63 signals are able to reach the MFLEx board. Done by forcing a +1mA on the resistor load located at the MFLEX board checker
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL04;  // for pin group voltage measurement 
            double[] SL04CH1, SL04CH2, SL04CH3, SL04CH4, SL04CH5, SL04CH6, SL04CH7, SL04CH8, SL04CH9, SL04CH10,
                     SL04CH11, SL04CH12, SL04CH13, SL04CH14, SL04CH15, SL04CH16, SL04CH17, SL04CH18, SL04CH19, SL04CH20; //for per pin voltage measurement 


            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);
            
            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot04 DC30 channels
            //HMOD1 - DB1 to DB20 -> ON = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575);

            //Initiate Pin to Session
            DCPower SL04_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL04_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH1");
            DCPower SL04_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH2");
            DCPower SL04_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH3");
            DCPower SL04_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH4");
            DCPower SL04_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH5");
            DCPower SL04_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH6");
            DCPower SL04_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH7");
            DCPower SL04_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH8");
            DCPower SL04_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH9");
            DCPower SL04_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH10");
            DCPower SL04_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH11");
            DCPower SL04_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH12");
            DCPower SL04_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH13");
            DCPower SL04_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH14");
            DCPower SL04_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH15");
            DCPower SL04_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH16");
            DCPower SL04_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH17");
            DCPower SL04_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH18");
            DCPower SL04_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH19");
            DCPower SL04_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH20");

            //configure and acquisition SMU's
            SL04_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL04_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL04_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL04_DC30.ConfigureOutputConnected(true);
            SL04_DC30.ConfigureOutputEnabled(true);
            SL04_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            Globals.TheHdw.Wait(Globals.SettlingTime);
            SL04_DC30.Initiate();

            //measure voltage, expected to be +1V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL04_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL04_DC30.Measure(out MeasDC30_SL04, out _); //pin group measurement

            //per pin measurement
            SL04_DC30_CH1.Measure(out SL04CH1, out _);
            SL04_DC30_CH2.Measure(out SL04CH2, out _);
            SL04_DC30_CH3.Measure(out SL04CH3, out _);
            SL04_DC30_CH4.Measure(out SL04CH4, out _);
            SL04_DC30_CH5.Measure(out SL04CH5, out _);
            SL04_DC30_CH6.Measure(out SL04CH6, out _);
            SL04_DC30_CH7.Measure(out SL04CH7, out _);
            SL04_DC30_CH8.Measure(out SL04CH8, out _);
            SL04_DC30_CH9.Measure(out SL04CH9, out _);
            SL04_DC30_CH10.Measure(out SL04CH10, out _);
            SL04_DC30_CH11.Measure(out SL04CH11, out _);
            SL04_DC30_CH12.Measure(out SL04CH12, out _);
            SL04_DC30_CH13.Measure(out SL04CH13, out _);
            SL04_DC30_CH14.Measure(out SL04CH14, out _);
            SL04_DC30_CH15.Measure(out SL04CH15, out _);
            SL04_DC30_CH16.Measure(out SL04CH16, out _);
            SL04_DC30_CH17.Measure(out SL04CH17, out _);
            SL04_DC30_CH18.Measure(out SL04CH18, out _);
            SL04_DC30_CH19.Measure(out SL04CH19, out _);
            SL04_DC30_CH20.Measure(out SL04CH20, out _);


            //return to initial settings
            SL04_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL04_DC30.Abort();
            SL04_DC30.ConfigureOutputEnabled(false);
            SL04_DC30.ConfigureOutputConnected(false);

            // bin out results
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1, "SL04_DC30_CH1");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2, "SL04_DC30_CH2");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3, "SL04_DC30_CH3");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4, "SL04_DC30_CH4");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5, "SL04_DC30_CH5");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6, "SL04_DC30_CH6");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7, "SL04_DC30_CH7");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8, "SL04_DC30_CH8");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9, "SL04_DC30_CH9");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10, "SL04_DC30_CH10");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11, "SL04_DC30_CH11");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12, "SL04_DC30_CH12");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13, "SL04_DC30_CH13");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14, "SL04_DC30_CH14");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15, "SL04_DC30_CH15");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16, "SL04_DC30_CH16");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17, "SL04_DC30_CH17");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18, "SL04_DC30_CH18");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19, "SL04_DC30_CH19");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20, "SL04_DC30_CH20");
        }

        public static void Slot10_DC30_Check(ISemiconductorModuleContext tsmContext)
        //Checks if SMU-4162/63 signals are able to reach the MFLEx board. Done by forcing a +1mA on the resistor load located at the MFLEX board checker
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL10;  // for pin group voltage measurement 
            double[] SL10CH1, SL10CH2, SL10CH3, SL10CH4, SL10CH5, SL10CH6, SL10CH7, SL10CH8, SL10CH9, SL10CH10,
                     SL10CH11, SL10CH12, SL10CH13, SL10CH14, SL10CH15, SL10CH16, SL10CH17, SL10CH18, SL10CH19, SL10CH20; //for per pin voltage measurement 


            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255);

            //Initiate Pin to Session
            DCPower SL10_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL10_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH1");
            DCPower SL10_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH2");
            DCPower SL10_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH3");
            DCPower SL10_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH4");
            DCPower SL10_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH5");
            DCPower SL10_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH6");
            DCPower SL10_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH7");
            DCPower SL10_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH8");
            DCPower SL10_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH9");
            DCPower SL10_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH10");
            DCPower SL10_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH11");
            DCPower SL10_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH12");
            DCPower SL10_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH13");
            DCPower SL10_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH14");
            DCPower SL10_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH15");
            DCPower SL10_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH16");
            DCPower SL10_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH17");
            DCPower SL10_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH18");
            DCPower SL10_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH19");
            DCPower SL10_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH20");

            //configure and acquisition SMU's
            SL10_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL10_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL10_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL10_DC30.ConfigureOutputConnected(true);
            SL10_DC30.ConfigureOutputEnabled(true);
            SL10_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            Globals.TheHdw.Wait(Globals.SettlingTime);
            SL10_DC30.Initiate();

            //measure voltage, expected to be +1V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL10_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL10_DC30.Measure(out MeasDC30_SL10, out _); //pin group measurement

            //per pin measurement
            SL10_DC30_CH1.Measure(out SL10CH1, out _);
            SL10_DC30_CH2.Measure(out SL10CH2, out _);
            SL10_DC30_CH3.Measure(out SL10CH3, out _);
            SL10_DC30_CH4.Measure(out SL10CH4, out _);
            SL10_DC30_CH5.Measure(out SL10CH5, out _);
            SL10_DC30_CH6.Measure(out SL10CH6, out _);
            SL10_DC30_CH7.Measure(out SL10CH7, out _);
            SL10_DC30_CH8.Measure(out SL10CH8, out _);
            SL10_DC30_CH9.Measure(out SL10CH9, out _);
            SL10_DC30_CH10.Measure(out SL10CH10, out _);
            SL10_DC30_CH11.Measure(out SL10CH11, out _);
            SL10_DC30_CH12.Measure(out SL10CH12, out _);
            SL10_DC30_CH13.Measure(out SL10CH13, out _);
            SL10_DC30_CH14.Measure(out SL10CH14, out _);
            SL10_DC30_CH15.Measure(out SL10CH15, out _);
            SL10_DC30_CH16.Measure(out SL10CH16, out _);
            SL10_DC30_CH17.Measure(out SL10CH17, out _);
            SL10_DC30_CH18.Measure(out SL10CH18, out _);
            SL10_DC30_CH19.Measure(out SL10CH19, out _);
            SL10_DC30_CH20.Measure(out SL10CH20, out _);


            //return to initial settings
            SL10_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL10_DC30.Abort();
            SL10_DC30.ConfigureOutputEnabled(false);
            SL10_DC30.ConfigureOutputConnected(false);

            // bin out results
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1, "SL10_DC30_CH1");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2, "SL10_DC30_CH2");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3, "SL10_DC30_CH3");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4, "SL10_DC30_CH4");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5, "SL10_DC30_CH5");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6, "SL10_DC30_CH6");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7, "SL10_DC30_CH7");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8, "SL10_DC30_CH8");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9, "SL10_DC30_CH9");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10, "SL10_DC30_CH10");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11, "SL10_DC30_CH11");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12, "SL10_DC30_CH12");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13, "SL10_DC30_CH13");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14, "SL10_DC30_CH14");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15, "SL10_DC30_CH15");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16, "SL10_DC30_CH16");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17, "SL10_DC30_CH17");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18, "SL10_DC30_CH18");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19, "SL10_DC30_CH19");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20, "SL10_DC30_CH20");

        }

        public static void Slot24_DC30_Check(ISemiconductorModuleContext tsmContext)
        //Checks if SMU-4162/63 signals are able to reach the MFLEx board. Done by forcing a +1mA on the resistor load located at the MFLEX board checker
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL24;  // for pin group voltage measurement 
            double[] SL24CH1, SL24CH2, SL24CH3, SL24CH4, SL24CH5, SL24CH6, SL24CH7, SL24CH8, SL24CH9, SL24CH10,
                     SL24CH11, SL24CH12, SL24CH13, SL24CH14, SL24CH15, SL24CH16, SL24CH17, SL24CH18, SL24CH19, SL24CH20; //for per pin voltage measurement 


            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255);

            //Initiate Pin to Session
            DCPower SL24_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL24_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH1");
            DCPower SL24_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH2");
            DCPower SL24_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH3");
            DCPower SL24_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH4");
            DCPower SL24_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH5");
            DCPower SL24_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH6");
            DCPower SL24_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH7");
            DCPower SL24_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH8");
            DCPower SL24_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH9");
            DCPower SL24_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH10");
            DCPower SL24_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH11");
            DCPower SL24_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH12");
            DCPower SL24_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH13");
            DCPower SL24_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH14");
            DCPower SL24_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH15");
            DCPower SL24_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH16");
            DCPower SL24_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH17");
            DCPower SL24_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH18");
            DCPower SL24_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH19");
            DCPower SL24_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH20");

            //configure and acquisition SMU's
            SL24_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL24_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL24_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL24_DC30.ConfigureOutputConnected(true);
            SL24_DC30.ConfigureOutputEnabled(true);
            SL24_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            Globals.TheHdw.Wait(Globals.SettlingTime);
            SL24_DC30.Initiate();

            //measure voltage, expected to be +1V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL24_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL24_DC30.Measure(out MeasDC30_SL24, out _); //pin group measurement

            //per pin measurement
            SL24_DC30_CH1.Measure(out SL24CH1, out _);
            SL24_DC30_CH2.Measure(out SL24CH2, out _);
            SL24_DC30_CH3.Measure(out SL24CH3, out _);
            SL24_DC30_CH4.Measure(out SL24CH4, out _);
            SL24_DC30_CH5.Measure(out SL24CH5, out _);
            SL24_DC30_CH6.Measure(out SL24CH6, out _);
            SL24_DC30_CH7.Measure(out SL24CH7, out _);
            SL24_DC30_CH8.Measure(out SL24CH8, out _);
            SL24_DC30_CH9.Measure(out SL24CH9, out _);
            SL24_DC30_CH10.Measure(out SL24CH10, out _);
            SL24_DC30_CH11.Measure(out SL24CH11, out _);
            SL24_DC30_CH12.Measure(out SL24CH12, out _);
            SL24_DC30_CH13.Measure(out SL24CH13, out _);
            SL24_DC30_CH14.Measure(out SL24CH14, out _);
            SL24_DC30_CH15.Measure(out SL24CH15, out _);
            SL24_DC30_CH16.Measure(out SL24CH16, out _);
            SL24_DC30_CH17.Measure(out SL24CH17, out _);
            SL24_DC30_CH18.Measure(out SL24CH18, out _);
            SL24_DC30_CH19.Measure(out SL24CH19, out _);
            SL24_DC30_CH20.Measure(out SL24CH20, out _);


            //return to initial settings
            SL24_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL24_DC30.Abort();
            SL24_DC30.ConfigureOutputEnabled(false);
            SL24_DC30.ConfigureOutputConnected(false);

            // bin out results
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1, "SL24_DC30_CH1");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2, "SL24_DC30_CH2");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3, "SL24_DC30_CH3");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4, "SL24_DC30_CH4");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5, "SL24_DC30_CH5");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6, "SL24_DC30_CH6");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7, "SL24_DC30_CH7");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8, "SL24_DC30_CH8");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9, "SL24_DC30_CH9");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10, "SL24_DC30_CH10");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11, "SL24_DC30_CH11");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12, "SL24_DC30_CH12");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13, "SL24_DC30_CH13");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14, "SL24_DC30_CH14");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15, "SL24_DC30_CH15");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16, "SL24_DC30_CH16");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17, "SL24_DC30_CH17");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18, "SL24_DC30_CH18");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19, "SL24_DC30_CH19");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20, "SL24_DC30_CH20");
        }

        public static void Slot04_DC30_DA_Check(ISemiconductorModuleContext tsmContext)
        //Checks DIB Access functionality, by forcing 5V on two 1kohm located at the MFLEx checker board. One 1Kohm is directly connected to the SMU-4162, the other 1Kohm gets connected through DIB Access
        {
            // current status of PXIE-4162/63 SMU's of SLOT 4
            // FIMV, forcing 0A
            // SSR/relays between SMU and DC30 pogos are close
            double[] MeasDC30_SL04_Odd_I, MeasDC30_SL04_Even_I;  // for pin group voltage measurement 
            double[] SL04CH1_I, SL04CH2_I, SL04CH3_I, SL04CH4_I, SL04CH5_I, SL04CH6_I, SL04CH7_I, SL04CH8_I, SL04CH9_I, SL04CH10_I,
                     SL04CH11_I, SL04CH12_I, SL04CH13_I, SL04CH14_I, SL04CH15_I, SL04CH16_I, SL04CH17_I, SL04CH18_I, SL04CH19_I, SL04CH20_I; //for per pin voltage measurement 

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            //HMOD1 - DB1 to DB20 -> ON          = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            //HMOD2 - DB29 and DB30 -> ON        = 0101 0000 0000 0000 0000 0000 0000 0000  = 1342177280
            //HMOD3 - DB1 to DB16 ODD bits -> ON = 0000 0000 0000 0000 0101 0101 0101 0101  = 21845
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575, HMOD_Data_2: 1342177280, HMOD_Data_3: 21845, HMOD_Data_4: 0);

            //Initiate Pin to Session
            DCPower SL04_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30"); //for pin group execution
            DCPower SL04_ODD_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_ODD_CH"); // for odd channels
            DCPower SL04_EVEN_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_EVEN_CH"); // for odd channels

            // for per pin execution
            DCPower SL04_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH1");
            DCPower SL04_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH2");
            DCPower SL04_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH3");
            DCPower SL04_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH4");
            DCPower SL04_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH5");
            DCPower SL04_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH6");
            DCPower SL04_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH7");
            DCPower SL04_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH8");
            DCPower SL04_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH9");
            DCPower SL04_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH10");
            DCPower SL04_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH11");
            DCPower SL04_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH12");
            DCPower SL04_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH13");
            DCPower SL04_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH14");
            DCPower SL04_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH15");
            DCPower SL04_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH16");
            DCPower SL04_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH17");
            DCPower SL04_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH18");
            DCPower SL04_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH19");
            DCPower SL04_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH20");

            //configure and acquisition of SMU's
            SL04_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL04_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL04_DC30.ConfigureVoltageLevelRange(24.0); // set voltage range
            SL04_DC30.ConfigureOutputConnected(true);
            SL04_DC30.ConfigureOutputEnabled(true);
            SL04_DC30.ForceVoltage(voltageLevel: 5, currentLimit: 60e-3); // force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
            Globals.TheHdw.Wait(Globals.SettlingTime);
            SL04_DC30.Initiate();

            //measure current expected to be +10mA, ODD channels
            SL04_ODD_CH.Measure(out _, out MeasDC30_SL04_Odd_I); //pin group measurement

            // per pin measurement, OOD channels
            SL04_DC30_CH1.Measure(out _, out SL04CH1_I);
            SL04_DC30_CH3.Measure(out _, out SL04CH3_I);
            SL04_DC30_CH5.Measure(out _, out SL04CH5_I);
            SL04_DC30_CH7.Measure(out _, out SL04CH7_I);
            SL04_DC30_CH9.Measure(out _, out SL04CH9_I);
            SL04_DC30_CH11.Measure(out _, out SL04CH11_I);
            SL04_DC30_CH13.Measure(out _, out SL04CH13_I);
            SL04_DC30_CH15.Measure(out _, out SL04CH15_I);
            SL04_DC30_CH17.Measure(out _, out SL04CH17_I);
            SL04_DC30_CH19.Measure(out _, out SL04CH19_I);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect EVEN channels to DIB Access
            //HMOD1 - DB1 to DB20 -> ON          = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            //HMOD2 - DB30 and DB32 -> ON        = 1010 0000 0000 0000 0000 0000 0000 0000  = 2684354560
            //HMOD3 - DB1 to DB16 ODD bits -> ON = 0000 0000 0000 0000 1010 1010 1010 1010  = 43690
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575, HMOD_Data_2: 2684354560, HMOD_Data_3: 43690, HMOD_Data_4: 0);

            //measure current expected to be +10mA, ODD channels
            SL04_EVEN_CH.Measure(out _, out MeasDC30_SL04_Even_I); //pin group measurement

            // per pin measurement, Even channels
            SL04_DC30_CH2.Measure(out _, out SL04CH2_I);
            SL04_DC30_CH4.Measure(out _, out SL04CH4_I);
            SL04_DC30_CH6.Measure(out _, out SL04CH6_I);
            SL04_DC30_CH8.Measure(out _, out SL04CH8_I);
            SL04_DC30_CH10.Measure(out _, out SL04CH10_I);
            SL04_DC30_CH12.Measure(out _, out SL04CH12_I);
            SL04_DC30_CH14.Measure(out _, out SL04CH14_I);
            SL04_DC30_CH16.Measure(out _, out SL04CH16_I);
            SL04_DC30_CH18.Measure(out _, out SL04CH18_I);
            SL04_DC30_CH20.Measure(out _, out SL04CH20_I);

            //return to initial settings
            SL04_DC30.ForceVoltage(voltageLevel: 0, currentLimit: 60e-3);

            //Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT4 DC30
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);

            SL04_DC30.Abort();
            SL04_DC30.ConfigureOutputEnabled(false);
            SL04_DC30.ConfigureOutputConnected(false);

            //bin out or Publish data
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04_Odd_I[0], ScaleType: Globals.scaleMilli, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group 
            //pin group bin out
            //  SL04_ODD_CH.PinQueryContext.Publish(MeasDC30_SL04_Odd_I, "SL04_DC30_ODD_DA"); 
            //  SL04_EVEN_CH.PinQueryContext.Publish(MeasDC30_SL04_Even_I, "SL04_DC30_ODD_DA");
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1_I, "SL04_DC30_CH1_DA");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2_I, "SL04_DC30_CH2_DA");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3_I, "SL04_DC30_CH3_DA");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4_I, "SL04_DC30_CH4_DA");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5_I, "SL04_DC30_CH5_DA");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6_I, "SL04_DC30_CH6_DA");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7_I, "SL04_DC30_CH7_DA");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8_I, "SL04_DC30_CH8_DA");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9_I, "SL04_DC30_CH9_DA");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10_I, "SL04_DC30_CH10_DA");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11_I, "SL04_DC30_CH11_DA");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12_I, "SL04_DC30_CH12_DA");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13_I, "SL04_DC30_CH13_DA");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14_I, "SL04_DC30_CH14_DA");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15_I, "SL04_DC30_CH15_DA");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16_I, "SL04_DC30_CH16_DA");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17_I, "SL04_DC30_CH17_DA");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18_I, "SL04_DC30_CH18_DA");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19_I, "SL04_DC30_CH19_DA");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20_I, "SL04_DC30_CH20_DA");
        }

        public static void Slot10_DC30_DA_Check(ISemiconductorModuleContext tsmContext)
        //Checks DIB Access functionality, by forcing 5V on two 1kohm located at the MFLEx checker board. One 1Kohm is directly connected to the SMU-4162, the other 1Kohm gets connected through DIB Access
        {
            // current status of PXIE-4162/63 SMU's of SLOT 4
            // FIMV, forcing 0A
            // SSR/relays between SMU and DC30 pogos are close
            double[] MeasDC30_SL10_Odd_I, MeasDC30_SL10_Even_I;  // for pin group voltage measurement 
            double[] SL10CH1_I, SL10CH2_I, SL10CH3_I, SL10CH4_I, SL10CH5_I, SL10CH6_I, SL10CH7_I, SL10CH8_I, SL10CH9_I, SL10CH10_I,
                     SL10CH11_I, SL10CH12_I, SL10CH13_I, SL10CH14_I, SL10CH15_I, SL10CH16_I, SL10CH17_I, SL10CH18_I, SL10CH19_I, SL10CH20_I; //for per pin voltage measurement 

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            //HMOD3 -  0101 0101 0101 0101 0000 0000 0000 0000 = 1431633920
            //HMOD4 -  0000 0000 0000 0000 0000 0000 0000 0101 = 5
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255, HMOD_Data_3: 1431633920, HMOD_Data_4: 5);

            //Initiate Pin to Session
            DCPower SL10_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30"); //for pin group execution
            DCPower SL10_ODD_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_ODD_CH"); // for odd channels
            DCPower SL10_EVEN_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_EVEN_CH"); // for odd channels

            // for per pin execution
            DCPower SL10_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH1");
            DCPower SL10_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH2");
            DCPower SL10_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH3");
            DCPower SL10_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH4");
            DCPower SL10_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH5");
            DCPower SL10_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH6");
            DCPower SL10_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH7");
            DCPower SL10_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH8");
            DCPower SL10_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH9");
            DCPower SL10_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH10");
            DCPower SL10_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH11");
            DCPower SL10_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH12");
            DCPower SL10_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH13");
            DCPower SL10_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH14");
            DCPower SL10_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH15");
            DCPower SL10_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH16");
            DCPower SL10_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH17");
            DCPower SL10_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH18");
            DCPower SL10_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH19");
            DCPower SL10_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH20");

            //configure and acquisition of SMU's
            SL10_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL10_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL10_DC30.ConfigureVoltageLevelRange(24.0); // set voltage range
            SL10_DC30.ConfigureOutputConnected(true);
            SL10_DC30.ConfigureOutputEnabled(true);
            SL10_DC30.ForceVoltage(voltageLevel: 5, currentLimit: 60e-3); // force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
            Globals.TheHdw.Wait(Globals.SettlingTime); SL10_DC30.Initiate();

            //measure current expected to be +10mA, ODD channels
            SL10_ODD_CH.Measure(out _, out MeasDC30_SL10_Odd_I); //pin group measurement

            // per pin measurement, OOD channels
            SL10_DC30_CH1.Measure(out _, out SL10CH1_I);
            SL10_DC30_CH3.Measure(out _, out SL10CH3_I);
            SL10_DC30_CH5.Measure(out _, out SL10CH5_I);
            SL10_DC30_CH7.Measure(out _, out SL10CH7_I);
            SL10_DC30_CH9.Measure(out _, out SL10CH9_I);
            SL10_DC30_CH11.Measure(out _, out SL10CH11_I);
            SL10_DC30_CH13.Measure(out _, out SL10CH13_I);
            SL10_DC30_CH15.Measure(out _, out SL10CH15_I);
            SL10_DC30_CH17.Measure(out _, out SL10CH17_I);
            SL10_DC30_CH19.Measure(out _, out SL10CH19_I);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect EVEN channels to DIB Access
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            //HMOD3 -  1010 1010 1010 1010 0000 0000 0000 0000 = 2863267840
            //HMOD4 -  0000 0000 0000 0000 0000 0000 0000 1010 = 10
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255, HMOD_Data_3: 2863267840, HMOD_Data_4: 10);

            //measure current expected to be +10mA, ODD channels
            SL10_EVEN_CH.Measure(out _, out MeasDC30_SL10_Even_I); //pin group measurement

            // per pin measurement, Even channels
            SL10_DC30_CH2.Measure(out _, out SL10CH2_I);
            SL10_DC30_CH4.Measure(out _, out SL10CH4_I);
            SL10_DC30_CH6.Measure(out _, out SL10CH6_I);
            SL10_DC30_CH8.Measure(out _, out SL10CH8_I);
            SL10_DC30_CH10.Measure(out _, out SL10CH10_I);
            SL10_DC30_CH12.Measure(out _, out SL10CH12_I);
            SL10_DC30_CH14.Measure(out _, out SL10CH14_I);
            SL10_DC30_CH16.Measure(out _, out SL10CH16_I);
            SL10_DC30_CH18.Measure(out _, out SL10CH18_I);
            SL10_DC30_CH20.Measure(out _, out SL10CH20_I);

            //return to initial settings
            SL10_DC30.ForceVoltage(voltageLevel: 0, currentLimit: 60e-3);

            //Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT4 DC30
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);

            SL10_DC30.Abort();
            SL10_DC30.ConfigureOutputEnabled(false);
            SL10_DC30.ConfigureOutputConnected(false);

            //bin out or Publish data
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10_Odd_I[0], ScaleType: Globals.scaleMilli, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group 
            //pin group bin out
            //  SL10_ODD_CH.PinQueryContext.Publish(MeasDC30_SL10_Odd_I, "SL10_DC30_ODD_DA"); 
            //  SL10_EVEN_CH.PinQueryContext.Publish(MeasDC30_SL10_Even_I, "SL10_DC30_ODD_DA");
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1_I, "SL10_DC30_CH1_DA");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2_I, "SL10_DC30_CH2_DA");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3_I, "SL10_DC30_CH3_DA");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4_I, "SL10_DC30_CH4_DA");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5_I, "SL10_DC30_CH5_DA");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6_I, "SL10_DC30_CH6_DA");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7_I, "SL10_DC30_CH7_DA");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8_I, "SL10_DC30_CH8_DA");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9_I, "SL10_DC30_CH9_DA");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10_I, "SL10_DC30_CH10_DA");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11_I, "SL10_DC30_CH11_DA");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12_I, "SL10_DC30_CH12_DA");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13_I, "SL10_DC30_CH13_DA");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14_I, "SL10_DC30_CH14_DA");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15_I, "SL10_DC30_CH15_DA");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16_I, "SL10_DC30_CH16_DA");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17_I, "SL10_DC30_CH17_DA");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18_I, "SL10_DC30_CH18_DA");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19_I, "SL10_DC30_CH19_DA");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20_I, "SL10_DC30_CH20_DA");


        }

        public static void Slot24_DC30_DA_Check(ISemiconductorModuleContext tsmContext)
        //Checks DIB Access functionality, by forcing 5V on two 1kohm located at the MFLEx checker board. One 1Kohm is directly connected to the SMU-4162, the other 1Kohm gets connected through DIB Access
        {
            // current status of PXIE-4162/63 SMU's of SLOT 4
            // FIMV, forcing 0A
            // SSR/relays between SMU and DC30 pogos are close
            double[] MeasDC30_SL24_Odd_I, MeasDC30_SL24_Even_I;  // for pin group voltage measurement 
            double[] SL24CH1_I, SL24CH2_I, SL24CH3_I, SL24CH4_I, SL24CH5_I, SL24CH6_I, SL24CH7_I, SL24CH8_I, SL24CH9_I, SL24CH10_I,
                     SL24CH11_I, SL24CH12_I, SL24CH13_I, SL24CH14_I, SL24CH15_I, SL24CH16_I, SL24CH17_I, SL24CH18_I, SL24CH19_I, SL24CH20_I; //for per pin voltage measurement 

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect ODD channels to DIB Access
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            //HMOD3 -  0101 0101 0101 0101 0000 0000 0000 0000 = 1431633920
            //HMOD4 -  0000 0000 0000 0000 0000 0000 0000 0101 = 5
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255, HMOD_Data_3: 1431633920, HMOD_Data_4: 5);

            //Initiate Pin to Session
            DCPower SL24_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30"); //for pin group execution
            DCPower SL24_ODD_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_ODD_CH"); // for odd channels
            DCPower SL24_EVEN_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_EVEN_CH"); // for odd channels

            // for per pin execution
            DCPower SL24_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH1");
            DCPower SL24_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH2");
            DCPower SL24_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH3");
            DCPower SL24_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH4");
            DCPower SL24_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH5");
            DCPower SL24_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH6");
            DCPower SL24_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH7");
            DCPower SL24_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH8");
            DCPower SL24_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH9");
            DCPower SL24_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH10");
            DCPower SL24_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH11");
            DCPower SL24_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH12");
            DCPower SL24_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH13");
            DCPower SL24_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH14");
            DCPower SL24_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH15");
            DCPower SL24_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH16");
            DCPower SL24_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH17");
            DCPower SL24_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH18");
            DCPower SL24_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH19");
            DCPower SL24_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH20");

            //configure and acquisition of SMU's
            SL24_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL24_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL24_DC30.ConfigureVoltageLevelRange(24.0); // set voltage range
            SL24_DC30.ConfigureOutputConnected(true);
            SL24_DC30.ConfigureOutputEnabled(true);
            SL24_DC30.ForceVoltage(voltageLevel: 5, currentLimit: 60e-3); // force voltage, expected resulting total current is 10mA = 5V/(1Kohms//1Kohms)
            Globals.TheHdw.Wait(Globals.SettlingTime); SL24_DC30.Initiate();

            //measure current expected to be +10mA, ODD channels
            SL24_ODD_CH.Measure(out _, out MeasDC30_SL24_Odd_I); //pin group measurement

            // per pin measurement, OOD channels
            SL24_DC30_CH1.Measure(out _, out SL24CH1_I);
            SL24_DC30_CH3.Measure(out _, out SL24CH3_I);
            SL24_DC30_CH5.Measure(out _, out SL24CH5_I);
            SL24_DC30_CH7.Measure(out _, out SL24CH7_I);
            SL24_DC30_CH9.Measure(out _, out SL24CH9_I);
            SL24_DC30_CH11.Measure(out _, out SL24CH11_I);
            SL24_DC30_CH13.Measure(out _, out SL24CH13_I);
            SL24_DC30_CH15.Measure(out _, out SL24CH15_I);
            SL24_DC30_CH17.Measure(out _, out SL24CH17_I);
            SL24_DC30_CH19.Measure(out _, out SL24CH19_I);

            //Turn ON HMOD relays to connect PXIE-4162/63, to connect EVEN channels to DIB Access
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255
            //HMOD3 -  1010 1010 1010 1010 0000 0000 0000 0000 = 2863267840
            //HMOD4 -  0000 0000 0000 0000 0000 0000 0000 1010 = 10
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255, HMOD_Data_3: 2863267840, HMOD_Data_4: 10);

            //measure current expected to be +10mA, ODD channels
            SL24_EVEN_CH.Measure(out _, out MeasDC30_SL24_Even_I); //pin group measurement

            // per pin measurement, Even channels
            SL24_DC30_CH2.Measure(out _, out SL24CH2_I);
            SL24_DC30_CH4.Measure(out _, out SL24CH4_I);
            SL24_DC30_CH6.Measure(out _, out SL24CH6_I);
            SL24_DC30_CH8.Measure(out _, out SL24CH8_I);
            SL24_DC30_CH10.Measure(out _, out SL24CH10_I);
            SL24_DC30_CH12.Measure(out _, out SL24CH12_I);
            SL24_DC30_CH14.Measure(out _, out SL24CH14_I);
            SL24_DC30_CH16.Measure(out _, out SL24CH16_I);
            SL24_DC30_CH18.Measure(out _, out SL24CH18_I);
            SL24_DC30_CH20.Measure(out _, out SL24CH20_I);

            //return to initial settings
            SL24_DC30.ForceVoltage(voltageLevel: 0, currentLimit: 60e-3);

            //Disconnect DIB Access, but retain the connection of SMU-4162/63 to SLOT4 DC30
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575, HMOD_Data_2: 0, HMOD_Data_3: 0, HMOD_Data_4: 0);

            SL24_DC30.Abort();
            SL24_DC30.ConfigureOutputEnabled(false);
            SL24_DC30.ConfigureOutputConnected(false);

            //bin out or Publish data
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24_Odd_I[0], ScaleType: Globals.scaleMilli, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group 
            //pin group bin out
            //  SL24_ODD_CH.PinQueryContext.Publish(MeasDC30_SL24_Odd_I, "SL24_DC30_ODD_DA"); 
            //  SL24_EVEN_CH.PinQueryContext.Publish(MeasDC30_SL24_Even_I, "SL24_DC30_ODD_DA");
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1_I, "SL24_DC30_CH1_DA");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2_I, "SL24_DC30_CH2_DA");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3_I, "SL24_DC30_CH3_DA");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4_I, "SL24_DC30_CH4_DA");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5_I, "SL24_DC30_CH5_DA");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6_I, "SL24_DC30_CH6_DA");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7_I, "SL24_DC30_CH7_DA");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8_I, "SL24_DC30_CH8_DA");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9_I, "SL24_DC30_CH9_DA");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10_I, "SL24_DC30_CH10_DA");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11_I, "SL24_DC30_CH11_DA");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12_I, "SL24_DC30_CH12_DA");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13_I, "SL24_DC30_CH13_DA");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14_I, "SL24_DC30_CH14_DA");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15_I, "SL24_DC30_CH15_DA");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16_I, "SL24_DC30_CH16_DA");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17_I, "SL24_DC30_CH17_DA");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18_I, "SL24_DC30_CH18_DA");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19_I, "SL24_DC30_CH19_DA");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20_I, "SL24_DC30_CH20_DA");


        }
        public static void Slot04_DC30_DGS_Check(ISemiconductorModuleContext tsmContext)
        //Checks DGS connectivity of each SMU-4162/63 channels. Each SMU channel forces current on the 1Kohm load located on the Checker board. Resulting voltage is measured at different DGS1/2/3/4 reference. 
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL04_DGS1, MeasDC30_SL04_DGS2, MeasDC30_SL04_DGS3, MeasDC30_SL04_DGS4;  // for pin group voltage measurement 
            double[] SL04CH1_DGS1, SL04CH2_DGS1, SL04CH3_DGS1, SL04CH4_DGS1, SL04CH5_DGS1, SL04CH6_DGS1, SL04CH7_DGS1, SL04CH8_DGS1, SL04CH9_DGS1, SL04CH10_DGS1,
                     SL04CH11_DGS1, SL04CH12_DGS1, SL04CH13_DGS1, SL04CH14_DGS1, SL04CH15_DGS1, SL04CH16_DGS1, SL04CH17_DGS1, SL04CH18_DGS1, SL04CH19_DGS1, SL04CH20_DGS1;
            double[] SL04CH1_DGS2, SL04CH2_DGS2, SL04CH3_DGS2, SL04CH4_DGS2, SL04CH5_DGS2, SL04CH6_DGS2, SL04CH7_DGS2, SL04CH8_DGS2, SL04CH9_DGS2, SL04CH10_DGS2,
                     SL04CH11_DGS2, SL04CH12_DGS2, SL04CH13_DGS2, SL04CH14_DGS2, SL04CH15_DGS2, SL04CH16_DGS2, SL04CH17_DGS2, SL04CH18_DGS2, SL04CH19_DGS2, SL04CH20_DGS2;
            double[] SL04CH1_DGS3, SL04CH2_DGS3, SL04CH3_DGS3, SL04CH4_DGS3, SL04CH5_DGS3, SL04CH6_DGS3, SL04CH7_DGS3, SL04CH8_DGS3, SL04CH9_DGS3, SL04CH10_DGS3,
                     SL04CH11_DGS3, SL04CH12_DGS3, SL04CH13_DGS3, SL04CH14_DGS3, SL04CH15_DGS3, SL04CH16_DGS3, SL04CH17_DGS3, SL04CH18_DGS3, SL04CH19_DGS3, SL04CH20_DGS3;
            double[] SL04CH1_DGS4, SL04CH2_DGS4, SL04CH3_DGS4, SL04CH4_DGS4, SL04CH5_DGS4, SL04CH6_DGS4, SL04CH7_DGS4, SL04CH8_DGS4, SL04CH9_DGS4, SL04CH10_DGS4,
                     SL04CH11_DGS4, SL04CH12_DGS4, SL04CH13_DGS4, SL04CH14_DGS4, SL04CH15_DGS4, SL04CH16_DGS4, SL04CH17_DGS4, SL04CH18_DGS4, SL04CH19_DGS4, SL04CH20_DGS4;

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot04 DC30 channels
            //HMOD1 - DB1 to DB20 -> ON = 0000 0000 0000 1111 1111 1111 1111 11111 = 1048575
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 1048575);

            //Initiate Pin to Session
            DCPower SL04_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL04_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH1");
            DCPower SL04_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH2");
            DCPower SL04_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH3");
            DCPower SL04_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH4");
            DCPower SL04_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH5");
            DCPower SL04_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH6");
            DCPower SL04_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH7");
            DCPower SL04_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH8");
            DCPower SL04_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH9");
            DCPower SL04_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH10");
            DCPower SL04_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH11");
            DCPower SL04_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH12");
            DCPower SL04_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH13");
            DCPower SL04_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH14");
            DCPower SL04_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH15");
            DCPower SL04_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH16");
            DCPower SL04_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH17");
            DCPower SL04_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH18");
            DCPower SL04_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH19");
            DCPower SL04_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL04_DC30_CH20");

            //configure and acquisition SMU's
            SL04_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL04_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL04_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL04_DC30.ConfigureOutputConnected(true);
            SL04_DC30.ConfigureOutputEnabled(true);
            SL04_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            SL04_DC30.Initiate();

            //Connect LO_S of SLOT4 DC30's to DGS1 only, disconnect LO_s channels from DGS2/3/4
            //HMOD5 - 0000 0001 1111 0000 0111 1111 1110 0000 = 32538592
            //HMOD6 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_5: 32538592, HMOD_Data_6: 201328384);

            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 1); //connect SL04 DGS1 to 1V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            //measure voltage, expected to be +1V - 1V(DGS = 1V REF) = 0V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL04_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL04_DC30.Measure(out MeasDC30_SL04_DGS1, out _); //pin group measurement

            //per pin measurement
            SL04_DC30_CH1.Measure(out SL04CH1_DGS1, out _);
            SL04_DC30_CH2.Measure(out SL04CH2_DGS1, out _);
            SL04_DC30_CH3.Measure(out SL04CH3_DGS1, out _);
            SL04_DC30_CH4.Measure(out SL04CH4_DGS1, out _);
            SL04_DC30_CH5.Measure(out SL04CH5_DGS1, out _);
            SL04_DC30_CH6.Measure(out SL04CH6_DGS1, out _);
            SL04_DC30_CH7.Measure(out SL04CH7_DGS1, out _);
            SL04_DC30_CH8.Measure(out SL04CH8_DGS1, out _);
            SL04_DC30_CH9.Measure(out SL04CH9_DGS1, out _);
            SL04_DC30_CH10.Measure(out SL04CH10_DGS1, out _);
            SL04_DC30_CH11.Measure(out SL04CH11_DGS1, out _);
            SL04_DC30_CH12.Measure(out SL04CH12_DGS1, out _);
            SL04_DC30_CH13.Measure(out SL04CH13_DGS1, out _);
            SL04_DC30_CH14.Measure(out SL04CH14_DGS1, out _);
            SL04_DC30_CH15.Measure(out SL04CH15_DGS1, out _);
            SL04_DC30_CH16.Measure(out SL04CH16_DGS1, out _);
            SL04_DC30_CH17.Measure(out SL04CH17_DGS1, out _);
            SL04_DC30_CH18.Measure(out SL04CH18_DGS1, out _);
            SL04_DC30_CH19.Measure(out SL04CH19_DGS1, out _);
            SL04_DC30_CH20.Measure(out SL04CH20_DGS1, out _);

            //Connect LO_S of SLOT4 DC30's to DGS2 only, disconnect LO_s channels from DGS1/3/4
            //HMOD5 - 0011 1110 0000 1111 1000 0000 0001 1111 = 1041203231
            //HMOD6 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_5: 1041203231, HMOD_Data_6: 201328384);

            //measure voltage, expected to be +1V - 2V(DGS = 2V REF) = -1V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 2); //connect SL04 DGS1 to 2V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL04_DC30.Measure(out MeasDC30_SL04_DGS2, out _); //pin group measurement

            //per pin measurement
            SL04_DC30_CH1.Measure(out SL04CH1_DGS2, out _);
            SL04_DC30_CH2.Measure(out SL04CH2_DGS2, out _);
            SL04_DC30_CH3.Measure(out SL04CH3_DGS2, out _);
            SL04_DC30_CH4.Measure(out SL04CH4_DGS2, out _);
            SL04_DC30_CH5.Measure(out SL04CH5_DGS2, out _);
            SL04_DC30_CH6.Measure(out SL04CH6_DGS2, out _);
            SL04_DC30_CH7.Measure(out SL04CH7_DGS2, out _);
            SL04_DC30_CH8.Measure(out SL04CH8_DGS2, out _);
            SL04_DC30_CH9.Measure(out SL04CH9_DGS2, out _);
            SL04_DC30_CH10.Measure(out SL04CH10_DGS2, out _);
            SL04_DC30_CH11.Measure(out SL04CH11_DGS2, out _);
            SL04_DC30_CH12.Measure(out SL04CH12_DGS2, out _);
            SL04_DC30_CH13.Measure(out SL04CH13_DGS2, out _);
            SL04_DC30_CH14.Measure(out SL04CH14_DGS2, out _);
            SL04_DC30_CH15.Measure(out SL04CH15_DGS2, out _);
            SL04_DC30_CH16.Measure(out SL04CH16_DGS2, out _);
            SL04_DC30_CH17.Measure(out SL04CH17_DGS2, out _);
            SL04_DC30_CH18.Measure(out SL04CH18_DGS2, out _);
            SL04_DC30_CH19.Measure(out SL04CH19_DGS2, out _);
            SL04_DC30_CH20.Measure(out SL04CH20_DGS2, out _);

            //Connect LO_S of SLOT4 DC30's to DGS3 only, disconnect LO_s channels from DGS1/2/4
            //HMOD5 - 1100 0001 1111 0000 0000 0000 0001 1111 = 3253731359
            //HMOD6 - 0000 1100 0000 0000 0001 1000 1111 1111 = 201332991
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_5: 3253731359, HMOD_Data_6: 201332991);

            //measure voltage, expected to be +1V - 3V(DGS = 2V REF) = -2V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 4); //connect SL04 DGS3 to 3V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL04_DC30.Measure(out MeasDC30_SL04_DGS3, out _); //pin group measurement

            //per pin measurement
            SL04_DC30_CH1.Measure(out SL04CH1_DGS3, out _);
            SL04_DC30_CH2.Measure(out SL04CH2_DGS3, out _);
            SL04_DC30_CH3.Measure(out SL04CH3_DGS3, out _);
            SL04_DC30_CH4.Measure(out SL04CH4_DGS3, out _);
            SL04_DC30_CH5.Measure(out SL04CH5_DGS3, out _);
            SL04_DC30_CH6.Measure(out SL04CH6_DGS3, out _);
            SL04_DC30_CH7.Measure(out SL04CH7_DGS3, out _);
            SL04_DC30_CH8.Measure(out SL04CH8_DGS3, out _);
            SL04_DC30_CH9.Measure(out SL04CH9_DGS3, out _);
            SL04_DC30_CH10.Measure(out SL04CH10_DGS3, out _);
            SL04_DC30_CH11.Measure(out SL04CH11_DGS3, out _);
            SL04_DC30_CH12.Measure(out SL04CH12_DGS3, out _);
            SL04_DC30_CH13.Measure(out SL04CH13_DGS3, out _);
            SL04_DC30_CH14.Measure(out SL04CH14_DGS3, out _);
            SL04_DC30_CH15.Measure(out SL04CH15_DGS3, out _);
            SL04_DC30_CH16.Measure(out SL04CH16_DGS3, out _);
            SL04_DC30_CH17.Measure(out SL04CH17_DGS3, out _);
            SL04_DC30_CH18.Measure(out SL04CH18_DGS3, out _);
            SL04_DC30_CH19.Measure(out SL04CH19_DGS3, out _);
            SL04_DC30_CH20.Measure(out SL04CH20_DGS3, out _);

            //Connect LO_S of SLOT4 DC30's to DGS4 only, disconnect LO_s channels from DGS1/2/3
            //HMOD5 - 0000 0001 1111 0000 0000 0000 0001 1111 = 32505887
            //HMOD6 - 0000 0011 1111 1111 1110 0111 0000 0000 = 67102464
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_5: 32505887, HMOD_Data_6: 67102464);

            //measure voltage, expected to be +1V - 4V(DGS = 2V REF) = -3V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 8); //connect SL04 DGS4 to 4V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL04_DC30.Measure(out MeasDC30_SL04_DGS4, out _); //pin group measurement

            //per pin measurement
            SL04_DC30_CH1.Measure(out SL04CH1_DGS4, out _);
            SL04_DC30_CH2.Measure(out SL04CH2_DGS4, out _);
            SL04_DC30_CH3.Measure(out SL04CH3_DGS4, out _);
            SL04_DC30_CH4.Measure(out SL04CH4_DGS4, out _);
            SL04_DC30_CH5.Measure(out SL04CH5_DGS4, out _);
            SL04_DC30_CH6.Measure(out SL04CH6_DGS4, out _);
            SL04_DC30_CH7.Measure(out SL04CH7_DGS4, out _);
            SL04_DC30_CH8.Measure(out SL04CH8_DGS4, out _);
            SL04_DC30_CH9.Measure(out SL04CH9_DGS4, out _);
            SL04_DC30_CH10.Measure(out SL04CH10_DGS4, out _);
            SL04_DC30_CH11.Measure(out SL04CH11_DGS4, out _);
            SL04_DC30_CH12.Measure(out SL04CH12_DGS4, out _);
            SL04_DC30_CH13.Measure(out SL04CH13_DGS4, out _);
            SL04_DC30_CH14.Measure(out SL04CH14_DGS4, out _);
            SL04_DC30_CH15.Measure(out SL04CH15_DGS4, out _);
            SL04_DC30_CH16.Measure(out SL04CH16_DGS4, out _);
            SL04_DC30_CH17.Measure(out SL04CH17_DGS4, out _);
            SL04_DC30_CH18.Measure(out SL04CH18_DGS4, out _);
            SL04_DC30_CH19.Measure(out SL04CH19_DGS4, out _);
            SL04_DC30_CH20.Measure(out SL04CH20_DGS4, out _);

            //return to initial settings
            SL04_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL04_DC30.Abort();
            SL04_DC30.ConfigureOutputEnabled(false);
            SL04_DC30.ConfigureOutputConnected(false);

            // bin out results
            //DGS = 1V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL04_DC30.PinQueryContext.Publish(MeasDC30_SL04_DGS1, "SL04_DC30_CHANNELS_DGS1");
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1_DGS1, "SL04_DC30_CH1_DGS1");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2_DGS1, "SL04_DC30_CH2_DGS1");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3_DGS1, "SL04_DC30_CH3_DGS1");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4_DGS1, "SL04_DC30_CH4_DGS1");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5_DGS1, "SL04_DC30_CH5_DGS1");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6_DGS1, "SL04_DC30_CH6_DGS1");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7_DGS1, "SL04_DC30_CH7_DGS1");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8_DGS1, "SL04_DC30_CH8_DGS1");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9_DGS1, "SL04_DC30_CH9_DGS1");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10_DGS1, "SL04_DC30_CH10_DGS1");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11_DGS1, "SL04_DC30_CH11_DGS1");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12_DGS1, "SL04_DC30_CH12_DGS1");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13_DGS1, "SL04_DC30_CH13_DGS1");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14_DGS1, "SL04_DC30_CH14_DGS1");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15_DGS1, "SL04_DC30_CH15_DGS1");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16_DGS1, "SL04_DC30_CH16_DGS1");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17_DGS1, "SL04_DC30_CH17_DGS1");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18_DGS1, "SL04_DC30_CH18_DGS1");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19_DGS1, "SL04_DC30_CH19_DGS1");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20_DGS1, "SL04_DC30_CH20_DGS1");

            //DGS = 2V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL04_DC30.PinQueryContext.Publish(MeasDC30_SL04_DGS2, "SL04_DC30_CHANNELS_DGS2");
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1_DGS2, "SL04_DC30_CH1_DGS2");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2_DGS2, "SL04_DC30_CH2_DGS2");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3_DGS2, "SL04_DC30_CH3_DGS2");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4_DGS2, "SL04_DC30_CH4_DGS2");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5_DGS2, "SL04_DC30_CH5_DGS2");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6_DGS2, "SL04_DC30_CH6_DGS2");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7_DGS2, "SL04_DC30_CH7_DGS2");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8_DGS2, "SL04_DC30_CH8_DGS2");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9_DGS2, "SL04_DC30_CH9_DGS2");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10_DGS2, "SL04_DC30_CH10_DGS2");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11_DGS2, "SL04_DC30_CH11_DGS2");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12_DGS2, "SL04_DC30_CH12_DGS2");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13_DGS2, "SL04_DC30_CH13_DGS2");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14_DGS2, "SL04_DC30_CH14_DGS2");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15_DGS2, "SL04_DC30_CH15_DGS2");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16_DGS2, "SL04_DC30_CH16_DGS2");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17_DGS2, "SL04_DC30_CH17_DGS2");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18_DGS2, "SL04_DC30_CH18_DGS2");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19_DGS2, "SL04_DC30_CH19_DGS2");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20_DGS2, "SL04_DC30_CH20_DGS2");

            //DGS = 3V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL04_DC30.PinQueryContext.Publish(MeasDC30_SL04_DGS3, "SL04_DC30_CHANNELS_DGS3");
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1_DGS3, "SL04_DC30_CH1_DGS3");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2_DGS3, "SL04_DC30_CH2_DGS3");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3_DGS3, "SL04_DC30_CH3_DGS3");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4_DGS3, "SL04_DC30_CH4_DGS3");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5_DGS3, "SL04_DC30_CH5_DGS3");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6_DGS3, "SL04_DC30_CH6_DGS3");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7_DGS3, "SL04_DC30_CH7_DGS3");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8_DGS3, "SL04_DC30_CH8_DGS3");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9_DGS3, "SL04_DC30_CH9_DGS3");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10_DGS3, "SL04_DC30_CH10_DGS3");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11_DGS3, "SL04_DC30_CH11_DGS3");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12_DGS3, "SL04_DC30_CH12_DGS3");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13_DGS3, "SL04_DC30_CH13_DGS3");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14_DGS3, "SL04_DC30_CH14_DGS3");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15_DGS3, "SL04_DC30_CH15_DGS3");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16_DGS3, "SL04_DC30_CH16_DGS3");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17_DGS3, "SL04_DC30_CH17_DGS3");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18_DGS3, "SL04_DC30_CH18_DGS3");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19_DGS3, "SL04_DC30_CH19_DGS3");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20_DGS3, "SL04_DC30_CH20_DGS3");

            //DGS = 4V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL04[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL04_DC30.PinQueryContext.Publish(MeasDC30_SL04_DGS4, "SL04_DC30_CHANNELS_DGS4");
            //per pin bin out
            SL04_DC30_CH1.PinQueryContext.Publish(SL04CH1_DGS4, "SL04_DC30_CH1_DGS4");
            SL04_DC30_CH2.PinQueryContext.Publish(SL04CH2_DGS4, "SL04_DC30_CH2_DGS4");
            SL04_DC30_CH3.PinQueryContext.Publish(SL04CH3_DGS4, "SL04_DC30_CH3_DGS4");
            SL04_DC30_CH4.PinQueryContext.Publish(SL04CH4_DGS4, "SL04_DC30_CH4_DGS4");
            SL04_DC30_CH5.PinQueryContext.Publish(SL04CH5_DGS4, "SL04_DC30_CH5_DGS4");
            SL04_DC30_CH6.PinQueryContext.Publish(SL04CH6_DGS4, "SL04_DC30_CH6_DGS4");
            SL04_DC30_CH7.PinQueryContext.Publish(SL04CH7_DGS4, "SL04_DC30_CH7_DGS4");
            SL04_DC30_CH8.PinQueryContext.Publish(SL04CH8_DGS4, "SL04_DC30_CH8_DGS4");
            SL04_DC30_CH9.PinQueryContext.Publish(SL04CH9_DGS4, "SL04_DC30_CH9_DGS4");
            SL04_DC30_CH10.PinQueryContext.Publish(SL04CH10_DGS4, "SL04_DC30_CH10_DGS4");
            SL04_DC30_CH11.PinQueryContext.Publish(SL04CH11_DGS4, "SL04_DC30_CH11_DGS4");
            SL04_DC30_CH12.PinQueryContext.Publish(SL04CH12_DGS4, "SL04_DC30_CH12_DGS4");
            SL04_DC30_CH13.PinQueryContext.Publish(SL04CH13_DGS4, "SL04_DC30_CH13_DGS4");
            SL04_DC30_CH14.PinQueryContext.Publish(SL04CH14_DGS4, "SL04_DC30_CH14_DGS4");
            SL04_DC30_CH15.PinQueryContext.Publish(SL04CH15_DGS4, "SL04_DC30_CH15_DGS4");
            SL04_DC30_CH16.PinQueryContext.Publish(SL04CH16_DGS4, "SL04_DC30_CH16_DGS4");
            SL04_DC30_CH17.PinQueryContext.Publish(SL04CH17_DGS4, "SL04_DC30_CH17_DGS4");
            SL04_DC30_CH18.PinQueryContext.Publish(SL04CH18_DGS4, "SL04_DC30_CH18_DGS4");
            SL04_DC30_CH19.PinQueryContext.Publish(SL04CH19_DGS4, "SL04_DC30_CH19_DGS4");
            SL04_DC30_CH20.PinQueryContext.Publish(SL04CH20_DGS4, "SL04_DC30_CH20_DGS4");
        }

        public static void Slot10_DC30_DGS_Check(ISemiconductorModuleContext tsmContext)
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL10_DGS1, MeasDC30_SL10_DGS2, MeasDC30_SL10_DGS3, MeasDC30_SL10_DGS4;  // for pin group voltage measurement 
            double[] SL10CH1_DGS1, SL10CH2_DGS1, SL10CH3_DGS1, SL10CH4_DGS1, SL10CH5_DGS1, SL10CH6_DGS1, SL10CH7_DGS1, SL10CH8_DGS1, SL10CH9_DGS1, SL10CH10_DGS1,
                     SL10CH11_DGS1, SL10CH12_DGS1, SL10CH13_DGS1, SL10CH14_DGS1, SL10CH15_DGS1, SL10CH16_DGS1, SL10CH17_DGS1, SL10CH18_DGS1, SL10CH19_DGS1, SL10CH20_DGS1;
            double[] SL10CH1_DGS2, SL10CH2_DGS2, SL10CH3_DGS2, SL10CH4_DGS2, SL10CH5_DGS2, SL10CH6_DGS2, SL10CH7_DGS2, SL10CH8_DGS2, SL10CH9_DGS2, SL10CH10_DGS2,
                     SL10CH11_DGS2, SL10CH12_DGS2, SL10CH13_DGS2, SL10CH14_DGS2, SL10CH15_DGS2, SL10CH16_DGS2, SL10CH17_DGS2, SL10CH18_DGS2, SL10CH19_DGS2, SL10CH20_DGS2;
            double[] SL10CH1_DGS3, SL10CH2_DGS3, SL10CH3_DGS3, SL10CH4_DGS3, SL10CH5_DGS3, SL10CH6_DGS3, SL10CH7_DGS3, SL10CH8_DGS3, SL10CH9_DGS3, SL10CH10_DGS3,
                     SL10CH11_DGS3, SL10CH12_DGS3, SL10CH13_DGS3, SL10CH14_DGS3, SL10CH15_DGS3, SL10CH16_DGS3, SL10CH17_DGS3, SL10CH18_DGS3, SL10CH19_DGS3, SL10CH20_DGS3;
            double[] SL10CH1_DGS4, SL10CH2_DGS4, SL10CH3_DGS4, SL10CH4_DGS4, SL10CH5_DGS4, SL10CH6_DGS4, SL10CH7_DGS4, SL10CH8_DGS4, SL10CH9_DGS4, SL10CH10_DGS4,
                     SL10CH11_DGS4, SL10CH12_DGS4, SL10CH13_DGS4, SL10CH14_DGS4, SL10CH15_DGS4, SL10CH16_DGS4, SL10CH17_DGS4, SL10CH18_DGS4, SL10CH19_DGS4, SL10CH20_DGS4;

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255            
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255);

            //Initiate Pin to Session
            DCPower SL10_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL10_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH1");
            DCPower SL10_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH2");
            DCPower SL10_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH3");
            DCPower SL10_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH4");
            DCPower SL10_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH5");
            DCPower SL10_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH6");
            DCPower SL10_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH7");
            DCPower SL10_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH8");
            DCPower SL10_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH9");
            DCPower SL10_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH10");
            DCPower SL10_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH11");
            DCPower SL10_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH12");
            DCPower SL10_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH13");
            DCPower SL10_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH14");
            DCPower SL10_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH15");
            DCPower SL10_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH16");
            DCPower SL10_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH17");
            DCPower SL10_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH18");
            DCPower SL10_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH19");
            DCPower SL10_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL10_DC30_CH20");

            //configure and acquisition SMU's
            SL10_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL10_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL10_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL10_DC30.ConfigureOutputConnected(true);
            SL10_DC30.ConfigureOutputEnabled(true);
            SL10_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            SL10_DC30.Initiate();

            //Connect LO_S of SLOT10 DC30's to DGS1 only, disconnect LO_s channels from DGS2/3/4
            //HMOD7 - 0000 0001 1111 0000 0111 1111 1110 0000 = 32538592
            //HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 32538592, HMOD_Data_8: 201328384);

            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 1); //connect SL10 DGS1 to 1V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            //measure voltage, expected to be +1V - 1V(DGS = 1V REF) = 0V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL10_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL10_DC30.Measure(out MeasDC30_SL10_DGS1, out _); //pin group measurement

            //per pin measurement
            SL10_DC30_CH1.Measure(out SL10CH1_DGS1, out _);
            SL10_DC30_CH2.Measure(out SL10CH2_DGS1, out _);
            SL10_DC30_CH3.Measure(out SL10CH3_DGS1, out _);
            SL10_DC30_CH4.Measure(out SL10CH4_DGS1, out _);
            SL10_DC30_CH5.Measure(out SL10CH5_DGS1, out _);
            SL10_DC30_CH6.Measure(out SL10CH6_DGS1, out _);
            SL10_DC30_CH7.Measure(out SL10CH7_DGS1, out _);
            SL10_DC30_CH8.Measure(out SL10CH8_DGS1, out _);
            SL10_DC30_CH9.Measure(out SL10CH9_DGS1, out _);
            SL10_DC30_CH10.Measure(out SL10CH10_DGS1, out _);
            SL10_DC30_CH11.Measure(out SL10CH11_DGS1, out _);
            SL10_DC30_CH12.Measure(out SL10CH12_DGS1, out _);
            SL10_DC30_CH13.Measure(out SL10CH13_DGS1, out _);
            SL10_DC30_CH14.Measure(out SL10CH14_DGS1, out _);
            SL10_DC30_CH15.Measure(out SL10CH15_DGS1, out _);
            SL10_DC30_CH16.Measure(out SL10CH16_DGS1, out _);
            SL10_DC30_CH17.Measure(out SL10CH17_DGS1, out _);
            SL10_DC30_CH18.Measure(out SL10CH18_DGS1, out _);
            SL10_DC30_CH19.Measure(out SL10CH19_DGS1, out _);
            SL10_DC30_CH20.Measure(out SL10CH20_DGS1, out _);

            //Connect LO_S of SLOT10 DC30's to DGS2 only, disconnect LO_s channels from DGS1/3/4
            //HMOD7 - 0011 1110 0000 1111 1000 0000 0001 1111 = 1041203231
            //HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 1041203231, HMOD_Data_8: 201328384);

            //measure voltage, expected to be +1V - 2V(DGS = 2V REF) = -1V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 2); //connect SL10 DGS1 to 2V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS2, out _); //pin group measurement

            //per pin measurement
            SL10_DC30_CH1.Measure(out SL10CH1_DGS2, out _);
            SL10_DC30_CH2.Measure(out SL10CH2_DGS2, out _);
            SL10_DC30_CH3.Measure(out SL10CH3_DGS2, out _);
            SL10_DC30_CH4.Measure(out SL10CH4_DGS2, out _);
            SL10_DC30_CH5.Measure(out SL10CH5_DGS2, out _);
            SL10_DC30_CH6.Measure(out SL10CH6_DGS2, out _);
            SL10_DC30_CH7.Measure(out SL10CH7_DGS2, out _);
            SL10_DC30_CH8.Measure(out SL10CH8_DGS2, out _);
            SL10_DC30_CH9.Measure(out SL10CH9_DGS2, out _);
            SL10_DC30_CH10.Measure(out SL10CH10_DGS2, out _);
            SL10_DC30_CH11.Measure(out SL10CH11_DGS2, out _);
            SL10_DC30_CH12.Measure(out SL10CH12_DGS2, out _);
            SL10_DC30_CH13.Measure(out SL10CH13_DGS2, out _);
            SL10_DC30_CH14.Measure(out SL10CH14_DGS2, out _);
            SL10_DC30_CH15.Measure(out SL10CH15_DGS2, out _);
            SL10_DC30_CH16.Measure(out SL10CH16_DGS2, out _);
            SL10_DC30_CH17.Measure(out SL10CH17_DGS2, out _);
            SL10_DC30_CH18.Measure(out SL10CH18_DGS2, out _);
            SL10_DC30_CH19.Measure(out SL10CH19_DGS2, out _);
            SL10_DC30_CH20.Measure(out SL10CH20_DGS2, out _);

            //Connect LO_S of SLOT10 DC30's to DGS3 only, disconnect LO_s channels from DGS1/2/4
            //HMOD7 - 1100 0001 1111 0000 0000 0000 0001 1111 = 3253731359
            //HMOD8 - 0000 1100 0000 0000 0001 1000 1111 1111 = 201332991
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 3253731359, HMOD_Data_8: 201332991);

            //measure voltage, expected to be +1V - 3V(DGS = 3V REF) = -2V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 4); //connect SL10 DGS3 to 3V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS3, out _); //pin group measurement

            //per pin measurement
            SL10_DC30_CH1.Measure(out SL10CH1_DGS3, out _);
            SL10_DC30_CH2.Measure(out SL10CH2_DGS3, out _);
            SL10_DC30_CH3.Measure(out SL10CH3_DGS3, out _);
            SL10_DC30_CH4.Measure(out SL10CH4_DGS3, out _);
            SL10_DC30_CH5.Measure(out SL10CH5_DGS3, out _);
            SL10_DC30_CH6.Measure(out SL10CH6_DGS3, out _);
            SL10_DC30_CH7.Measure(out SL10CH7_DGS3, out _);
            SL10_DC30_CH8.Measure(out SL10CH8_DGS3, out _);
            SL10_DC30_CH9.Measure(out SL10CH9_DGS3, out _);
            SL10_DC30_CH10.Measure(out SL10CH10_DGS3, out _);
            SL10_DC30_CH11.Measure(out SL10CH11_DGS3, out _);
            SL10_DC30_CH12.Measure(out SL10CH12_DGS3, out _);
            SL10_DC30_CH13.Measure(out SL10CH13_DGS3, out _);
            SL10_DC30_CH14.Measure(out SL10CH14_DGS3, out _);
            SL10_DC30_CH15.Measure(out SL10CH15_DGS3, out _);
            SL10_DC30_CH16.Measure(out SL10CH16_DGS3, out _);
            SL10_DC30_CH17.Measure(out SL10CH17_DGS3, out _);
            SL10_DC30_CH18.Measure(out SL10CH18_DGS3, out _);
            SL10_DC30_CH19.Measure(out SL10CH19_DGS3, out _);
            SL10_DC30_CH20.Measure(out SL10CH20_DGS3, out _);

            //Connect LO_S of SLOT10 DC30's to DGS4 only, disconnect LO_s channels from DGS1/2/3
            //HMOD7 - 0000 0001 1111 0000 0000 0000 0001 1111 = 32505887
            //HMOD8 - 0000 0011 1111 1111 1110 0111 0000 0000 = 67102464
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 32505887, HMOD_Data_8: 67102464);

            //measure voltage, expected to be +1V - 4V(DGS = 4V REF) = -3V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 8); //connect SL10 DGS4 to 4V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL10_DC30.Measure(out MeasDC30_SL10_DGS4, out _); //pin group measurement

            //per pin measurement
            SL10_DC30_CH1.Measure(out SL10CH1_DGS4, out _);
            SL10_DC30_CH2.Measure(out SL10CH2_DGS4, out _);
            SL10_DC30_CH3.Measure(out SL10CH3_DGS4, out _);
            SL10_DC30_CH4.Measure(out SL10CH4_DGS4, out _);
            SL10_DC30_CH5.Measure(out SL10CH5_DGS4, out _);
            SL10_DC30_CH6.Measure(out SL10CH6_DGS4, out _);
            SL10_DC30_CH7.Measure(out SL10CH7_DGS4, out _);
            SL10_DC30_CH8.Measure(out SL10CH8_DGS4, out _);
            SL10_DC30_CH9.Measure(out SL10CH9_DGS4, out _);
            SL10_DC30_CH10.Measure(out SL10CH10_DGS4, out _);
            SL10_DC30_CH11.Measure(out SL10CH11_DGS4, out _);
            SL10_DC30_CH12.Measure(out SL10CH12_DGS4, out _);
            SL10_DC30_CH13.Measure(out SL10CH13_DGS4, out _);
            SL10_DC30_CH14.Measure(out SL10CH14_DGS4, out _);
            SL10_DC30_CH15.Measure(out SL10CH15_DGS4, out _);
            SL10_DC30_CH16.Measure(out SL10CH16_DGS4, out _);
            SL10_DC30_CH17.Measure(out SL10CH17_DGS4, out _);
            SL10_DC30_CH18.Measure(out SL10CH18_DGS4, out _);
            SL10_DC30_CH19.Measure(out SL10CH19_DGS4, out _);
            SL10_DC30_CH20.Measure(out SL10CH20_DGS4, out _);

            //return to initial settings
            SL10_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL10_DC30.Abort();
            SL10_DC30.ConfigureOutputEnabled(false);
            SL10_DC30.ConfigureOutputConnected(false);

            // bin out results
            //DGS = 1V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS1, "SL10_DC30_CHANNELS_DGS1");
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1_DGS1, "SL10_DC30_CH1_DGS1");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2_DGS1, "SL10_DC30_CH2_DGS1");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3_DGS1, "SL10_DC30_CH3_DGS1");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4_DGS1, "SL10_DC30_CH4_DGS1");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5_DGS1, "SL10_DC30_CH5_DGS1");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6_DGS1, "SL10_DC30_CH6_DGS1");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7_DGS1, "SL10_DC30_CH7_DGS1");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8_DGS1, "SL10_DC30_CH8_DGS1");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9_DGS1, "SL10_DC30_CH9_DGS1");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10_DGS1, "SL10_DC30_CH10_DGS1");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11_DGS1, "SL10_DC30_CH11_DGS1");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12_DGS1, "SL10_DC30_CH12_DGS1");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13_DGS1, "SL10_DC30_CH13_DGS1");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14_DGS1, "SL10_DC30_CH14_DGS1");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15_DGS1, "SL10_DC30_CH15_DGS1");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16_DGS1, "SL10_DC30_CH16_DGS1");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17_DGS1, "SL10_DC30_CH17_DGS1");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18_DGS1, "SL10_DC30_CH18_DGS1");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19_DGS1, "SL10_DC30_CH19_DGS1");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20_DGS1, "SL10_DC30_CH20_DGS1");

            //DGS = 2V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS2, "SL10_DC30_CHANNELS_DGS2");
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1_DGS2, "SL10_DC30_CH1_DGS2");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2_DGS2, "SL10_DC30_CH2_DGS2");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3_DGS2, "SL10_DC30_CH3_DGS2");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4_DGS2, "SL10_DC30_CH4_DGS2");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5_DGS2, "SL10_DC30_CH5_DGS2");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6_DGS2, "SL10_DC30_CH6_DGS2");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7_DGS2, "SL10_DC30_CH7_DGS2");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8_DGS2, "SL10_DC30_CH8_DGS2");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9_DGS2, "SL10_DC30_CH9_DGS2");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10_DGS2, "SL10_DC30_CH10_DGS2");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11_DGS2, "SL10_DC30_CH11_DGS2");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12_DGS2, "SL10_DC30_CH12_DGS2");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13_DGS2, "SL10_DC30_CH13_DGS2");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14_DGS2, "SL10_DC30_CH14_DGS2");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15_DGS2, "SL10_DC30_CH15_DGS2");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16_DGS2, "SL10_DC30_CH16_DGS2");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17_DGS2, "SL10_DC30_CH17_DGS2");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18_DGS2, "SL10_DC30_CH18_DGS2");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19_DGS2, "SL10_DC30_CH19_DGS2");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20_DGS2, "SL10_DC30_CH20_DGS2");

            //DGS = 3V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS3, "SL10_DC30_CHANNELS_DGS3");
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1_DGS3, "SL10_DC30_CH1_DGS3");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2_DGS3, "SL10_DC30_CH2_DGS3");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3_DGS3, "SL10_DC30_CH3_DGS3");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4_DGS3, "SL10_DC30_CH4_DGS3");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5_DGS3, "SL10_DC30_CH5_DGS3");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6_DGS3, "SL10_DC30_CH6_DGS3");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7_DGS3, "SL10_DC30_CH7_DGS3");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8_DGS3, "SL10_DC30_CH8_DGS3");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9_DGS3, "SL10_DC30_CH9_DGS3");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10_DGS3, "SL10_DC30_CH10_DGS3");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11_DGS3, "SL10_DC30_CH11_DGS3");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12_DGS3, "SL10_DC30_CH12_DGS3");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13_DGS3, "SL10_DC30_CH13_DGS3");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14_DGS3, "SL10_DC30_CH14_DGS3");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15_DGS3, "SL10_DC30_CH15_DGS3");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16_DGS3, "SL10_DC30_CH16_DGS3");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17_DGS3, "SL10_DC30_CH17_DGS3");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18_DGS3, "SL10_DC30_CH18_DGS3");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19_DGS3, "SL10_DC30_CH19_DGS3");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20_DGS3, "SL10_DC30_CH20_DGS3");

            //DGS = 4V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL10[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL10_DC30.PinQueryContext.Publish(MeasDC30_SL10_DGS4, "SL10_DC30_CHANNELS_DGS4");
            //per pin bin out
            SL10_DC30_CH1.PinQueryContext.Publish(SL10CH1_DGS4, "SL10_DC30_CH1_DGS4");
            SL10_DC30_CH2.PinQueryContext.Publish(SL10CH2_DGS4, "SL10_DC30_CH2_DGS4");
            SL10_DC30_CH3.PinQueryContext.Publish(SL10CH3_DGS4, "SL10_DC30_CH3_DGS4");
            SL10_DC30_CH4.PinQueryContext.Publish(SL10CH4_DGS4, "SL10_DC30_CH4_DGS4");
            SL10_DC30_CH5.PinQueryContext.Publish(SL10CH5_DGS4, "SL10_DC30_CH5_DGS4");
            SL10_DC30_CH6.PinQueryContext.Publish(SL10CH6_DGS4, "SL10_DC30_CH6_DGS4");
            SL10_DC30_CH7.PinQueryContext.Publish(SL10CH7_DGS4, "SL10_DC30_CH7_DGS4");
            SL10_DC30_CH8.PinQueryContext.Publish(SL10CH8_DGS4, "SL10_DC30_CH8_DGS4");
            SL10_DC30_CH9.PinQueryContext.Publish(SL10CH9_DGS4, "SL10_DC30_CH9_DGS4");
            SL10_DC30_CH10.PinQueryContext.Publish(SL10CH10_DGS4, "SL10_DC30_CH10_DGS4");
            SL10_DC30_CH11.PinQueryContext.Publish(SL10CH11_DGS4, "SL10_DC30_CH11_DGS4");
            SL10_DC30_CH12.PinQueryContext.Publish(SL10CH12_DGS4, "SL10_DC30_CH12_DGS4");
            SL10_DC30_CH13.PinQueryContext.Publish(SL10CH13_DGS4, "SL10_DC30_CH13_DGS4");
            SL10_DC30_CH14.PinQueryContext.Publish(SL10CH14_DGS4, "SL10_DC30_CH14_DGS4");
            SL10_DC30_CH15.PinQueryContext.Publish(SL10CH15_DGS4, "SL10_DC30_CH15_DGS4");
            SL10_DC30_CH16.PinQueryContext.Publish(SL10CH16_DGS4, "SL10_DC30_CH16_DGS4");
            SL10_DC30_CH17.PinQueryContext.Publish(SL10CH17_DGS4, "SL10_DC30_CH17_DGS4");
            SL10_DC30_CH18.PinQueryContext.Publish(SL10CH18_DGS4, "SL10_DC30_CH18_DGS4");
            SL10_DC30_CH19.PinQueryContext.Publish(SL10CH19_DGS4, "SL10_DC30_CH19_DGS4");
            SL10_DC30_CH20.PinQueryContext.Publish(SL10CH20_DGS4, "SL10_DC30_CH20_DGS4");
        }

        public static void Slot24_DC30_DGS_Check(ISemiconductorModuleContext tsmContext)
        {
            PinListData D30_Voltages = new PinListData();
            double[] MeasDC30_SL24_DGS1, MeasDC30_SL24_DGS2, MeasDC30_SL24_DGS3, MeasDC30_SL24_DGS4;  // for pin group voltage measurement 
            double[] SL24CH1_DGS1, SL24CH2_DGS1, SL24CH3_DGS1, SL24CH4_DGS1, SL24CH5_DGS1, SL24CH6_DGS1, SL24CH7_DGS1, SL24CH8_DGS1, SL24CH9_DGS1, SL24CH10_DGS1,
                     SL24CH11_DGS1, SL24CH12_DGS1, SL24CH13_DGS1, SL24CH14_DGS1, SL24CH15_DGS1, SL24CH16_DGS1, SL24CH17_DGS1, SL24CH18_DGS1, SL24CH19_DGS1, SL24CH20_DGS1;
            double[] SL24CH1_DGS2, SL24CH2_DGS2, SL24CH3_DGS2, SL24CH4_DGS2, SL24CH5_DGS2, SL24CH6_DGS2, SL24CH7_DGS2, SL24CH8_DGS2, SL24CH9_DGS2, SL24CH10_DGS2,
                     SL24CH11_DGS2, SL24CH12_DGS2, SL24CH13_DGS2, SL24CH14_DGS2, SL24CH15_DGS2, SL24CH16_DGS2, SL24CH17_DGS2, SL24CH18_DGS2, SL24CH19_DGS2, SL24CH20_DGS2;
            double[] SL24CH1_DGS3, SL24CH2_DGS3, SL24CH3_DGS3, SL24CH4_DGS3, SL24CH5_DGS3, SL24CH6_DGS3, SL24CH7_DGS3, SL24CH8_DGS3, SL24CH9_DGS3, SL24CH10_DGS3,
                     SL24CH11_DGS3, SL24CH12_DGS3, SL24CH13_DGS3, SL24CH14_DGS3, SL24CH15_DGS3, SL24CH16_DGS3, SL24CH17_DGS3, SL24CH18_DGS3, SL24CH19_DGS3, SL24CH20_DGS3;
            double[] SL24CH1_DGS4, SL24CH2_DGS4, SL24CH3_DGS4, SL24CH4_DGS4, SL24CH5_DGS4, SL24CH6_DGS4, SL24CH7_DGS4, SL24CH8_DGS4, SL24CH9_DGS4, SL24CH10_DGS4,
                     SL24CH11_DGS4, SL24CH12_DGS4, SL24CH13_DGS4, SL24CH14_DGS4, SL24CH15_DGS4, SL24CH16_DGS4, SL24CH17_DGS4, SL24CH18_DGS4, SL24CH19_DGS4, SL24CH20_DGS4;

            //Reset all HMODs(Tx Board and Checker Board)
            Reset_All_HMODs(tsmContext);

            //Turn HMOD relays to connect PXIE-4162/63 to their corresponding Slot10 DC30 channels
            //HMOD1 -  1111 1111 1111 0000 0000 0000 0000 0000 = 4293918720
            //HMOD2 -  0000 0000 0000 0000 0000 0000 1111 1111 = 255            
            HMODCtrl.HMOD1to4(tsmContext, HMOD_Data_1: 4293918720, HMOD_Data_2: 255);

            //Initiate Pin to Session
            DCPower SL24_DC30 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30"); //for pin group execution

            // for per pin execution
            DCPower SL24_DC30_CH1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH1");
            DCPower SL24_DC30_CH2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH2");
            DCPower SL24_DC30_CH3 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH3");
            DCPower SL24_DC30_CH4 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH4");
            DCPower SL24_DC30_CH5 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH5");
            DCPower SL24_DC30_CH6 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH6");
            DCPower SL24_DC30_CH7 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH7");
            DCPower SL24_DC30_CH8 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH8");
            DCPower SL24_DC30_CH9 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH9");
            DCPower SL24_DC30_CH10 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH10");
            DCPower SL24_DC30_CH11 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH11");
            DCPower SL24_DC30_CH12 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH12");
            DCPower SL24_DC30_CH13 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH13");
            DCPower SL24_DC30_CH14 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH14");
            DCPower SL24_DC30_CH15 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH15");
            DCPower SL24_DC30_CH16 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH16");
            DCPower SL24_DC30_CH17 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH17");
            DCPower SL24_DC30_CH18 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH18");
            DCPower SL24_DC30_CH19 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH19");
            DCPower SL24_DC30_CH20 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "SL24_DC30_CH20");

            //configure and acquisition SMU's
            SL24_DC30.ConfigureSettings(apertureTime: 10e-3, apertureTimeUnitsinSeconds: DCPowerMeasureApertureTimeUnits.Seconds);
            SL24_DC30.ConfigureSense(sense: DCPowerMeasurementSense.Remote, initiateSessionAfter: true);
            SL24_DC30.ConfigureCurrentLevelRange(10e-3); // set current range
            SL24_DC30.ConfigureOutputConnected(true);
            SL24_DC30.ConfigureOutputEnabled(true);
            SL24_DC30.ForceCurrent(currentLevel: 1e-3, voltageLimit: 24); // force current on 1Kohms resistor
            SL24_DC30.Initiate();

            //Connect LO_S of SLOT10 DC30's to DGS1 only, disconnect LO_s channels from DGS2/3/4
            //HMOD7 - 0000 0001 1111 0000 0111 1111 1110 0000 = 32538592
            //HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 32538592, HMOD_Data_8: 201328384);

            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 1); //connect SL24 DGS1 to 1V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            //measure voltage, expected to be +1V - 1V(DGS = 1V REF) = 0V
            //  At this point, the LO_S of the SMU4162/63 channels are connected to their default DGS(1/2/3/4). All DGS are also connected to GND by default. 
            //  D30_Voltages = Globals.TheHdw.DCVI.Pins("SL24_DC30").Meter.Read(Globals.tlStrobe, 10, 1000, tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage);
            SL24_DC30.Measure(out MeasDC30_SL24_DGS1, out _); //pin group measurement

            //per pin measurement
            SL24_DC30_CH1.Measure(out SL24CH1_DGS1, out _);
            SL24_DC30_CH2.Measure(out SL24CH2_DGS1, out _);
            SL24_DC30_CH3.Measure(out SL24CH3_DGS1, out _);
            SL24_DC30_CH4.Measure(out SL24CH4_DGS1, out _);
            SL24_DC30_CH5.Measure(out SL24CH5_DGS1, out _);
            SL24_DC30_CH6.Measure(out SL24CH6_DGS1, out _);
            SL24_DC30_CH7.Measure(out SL24CH7_DGS1, out _);
            SL24_DC30_CH8.Measure(out SL24CH8_DGS1, out _);
            SL24_DC30_CH9.Measure(out SL24CH9_DGS1, out _);
            SL24_DC30_CH10.Measure(out SL24CH10_DGS1, out _);
            SL24_DC30_CH11.Measure(out SL24CH11_DGS1, out _);
            SL24_DC30_CH12.Measure(out SL24CH12_DGS1, out _);
            SL24_DC30_CH13.Measure(out SL24CH13_DGS1, out _);
            SL24_DC30_CH14.Measure(out SL24CH14_DGS1, out _);
            SL24_DC30_CH15.Measure(out SL24CH15_DGS1, out _);
            SL24_DC30_CH16.Measure(out SL24CH16_DGS1, out _);
            SL24_DC30_CH17.Measure(out SL24CH17_DGS1, out _);
            SL24_DC30_CH18.Measure(out SL24CH18_DGS1, out _);
            SL24_DC30_CH19.Measure(out SL24CH19_DGS1, out _);
            SL24_DC30_CH20.Measure(out SL24CH20_DGS1, out _);

            //Connect LO_S of SLOT10 DC30's to DGS2 only, disconnect LO_s channels from DGS1/3/4
            //HMOD7 - 0011 1110 0000 1111 1000 0000 0001 1111 = 1041203231
            //HMOD8 - 0000 1100 0000 0000 0000 0111 0000 0000 = 201328384
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 1041203231, HMOD_Data_8: 201328384);

            //measure voltage, expected to be +1V - 2V(DGS = 2V REF) = -1V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 2); //connect SL24 DGS1 to 2V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL24_DC30.Measure(out MeasDC30_SL24_DGS2, out _); //pin group measurement

            //per pin measurement
            SL24_DC30_CH1.Measure(out SL24CH1_DGS2, out _);
            SL24_DC30_CH2.Measure(out SL24CH2_DGS2, out _);
            SL24_DC30_CH3.Measure(out SL24CH3_DGS2, out _);
            SL24_DC30_CH4.Measure(out SL24CH4_DGS2, out _);
            SL24_DC30_CH5.Measure(out SL24CH5_DGS2, out _);
            SL24_DC30_CH6.Measure(out SL24CH6_DGS2, out _);
            SL24_DC30_CH7.Measure(out SL24CH7_DGS2, out _);
            SL24_DC30_CH8.Measure(out SL24CH8_DGS2, out _);
            SL24_DC30_CH9.Measure(out SL24CH9_DGS2, out _);
            SL24_DC30_CH10.Measure(out SL24CH10_DGS2, out _);
            SL24_DC30_CH11.Measure(out SL24CH11_DGS2, out _);
            SL24_DC30_CH12.Measure(out SL24CH12_DGS2, out _);
            SL24_DC30_CH13.Measure(out SL24CH13_DGS2, out _);
            SL24_DC30_CH14.Measure(out SL24CH14_DGS2, out _);
            SL24_DC30_CH15.Measure(out SL24CH15_DGS2, out _);
            SL24_DC30_CH16.Measure(out SL24CH16_DGS2, out _);
            SL24_DC30_CH17.Measure(out SL24CH17_DGS2, out _);
            SL24_DC30_CH18.Measure(out SL24CH18_DGS2, out _);
            SL24_DC30_CH19.Measure(out SL24CH19_DGS2, out _);
            SL24_DC30_CH20.Measure(out SL24CH20_DGS2, out _);

            //Connect LO_S of SLOT10 DC30's to DGS3 only, disconnect LO_s channels from DGS1/2/4
            //HMOD7 - 1100 0001 1111 0000 0000 0000 0001 1111 = 3253731359
            //HMOD8 - 0000 1100 0000 0000 0001 1000 1111 1111 = 201332991
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 3253731359, HMOD_Data_8: 201332991);

            //measure voltage, expected to be +1V - 3V(DGS = 3V REF) = -2V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 4); //connect SL24 DGS3 to 3V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL24_DC30.Measure(out MeasDC30_SL24_DGS3, out _); //pin group measurement

            //per pin measurement
            SL24_DC30_CH1.Measure(out SL24CH1_DGS3, out _);
            SL24_DC30_CH2.Measure(out SL24CH2_DGS3, out _);
            SL24_DC30_CH3.Measure(out SL24CH3_DGS3, out _);
            SL24_DC30_CH4.Measure(out SL24CH4_DGS3, out _);
            SL24_DC30_CH5.Measure(out SL24CH5_DGS3, out _);
            SL24_DC30_CH6.Measure(out SL24CH6_DGS3, out _);
            SL24_DC30_CH7.Measure(out SL24CH7_DGS3, out _);
            SL24_DC30_CH8.Measure(out SL24CH8_DGS3, out _);
            SL24_DC30_CH9.Measure(out SL24CH9_DGS3, out _);
            SL24_DC30_CH10.Measure(out SL24CH10_DGS3, out _);
            SL24_DC30_CH11.Measure(out SL24CH11_DGS3, out _);
            SL24_DC30_CH12.Measure(out SL24CH12_DGS3, out _);
            SL24_DC30_CH13.Measure(out SL24CH13_DGS3, out _);
            SL24_DC30_CH14.Measure(out SL24CH14_DGS3, out _);
            SL24_DC30_CH15.Measure(out SL24CH15_DGS3, out _);
            SL24_DC30_CH16.Measure(out SL24CH16_DGS3, out _);
            SL24_DC30_CH17.Measure(out SL24CH17_DGS3, out _);
            SL24_DC30_CH18.Measure(out SL24CH18_DGS3, out _);
            SL24_DC30_CH19.Measure(out SL24CH19_DGS3, out _);
            SL24_DC30_CH20.Measure(out SL24CH20_DGS3, out _);

            //Connect LO_S of SLOT10 DC30's to DGS4 only, disconnect LO_s channels from DGS1/2/3
            //HMOD7 - 0000 0001 1111 0000 0000 0000 0001 1111 = 32505887
            //HMOD8 - 0000 0011 1111 1111 1110 0111 0000 0000 = 67102464
            HMODCtrl.HMOD5to10(tsmContext, HMOD_Data_7: 32505887, HMOD_Data_8: 67102464);

            //measure voltage, expected to be +1V - 4V(DGS = 4V REF) = -3V
            HMODCtrl.ChckrBoardHMOD1to13(tsmContext, ChckrBoardHMOD_Data_1: 8); //connect SL24 DGS4 to 4V reference
            Globals.TheHdw.Wait(Globals.SettlingTime);

            SL24_DC30.Measure(out MeasDC30_SL24_DGS4, out _); //pin group measurement

            //per pin measurement
            SL24_DC30_CH1.Measure(out SL24CH1_DGS4, out _);
            SL24_DC30_CH2.Measure(out SL24CH2_DGS4, out _);
            SL24_DC30_CH3.Measure(out SL24CH3_DGS4, out _);
            SL24_DC30_CH4.Measure(out SL24CH4_DGS4, out _);
            SL24_DC30_CH5.Measure(out SL24CH5_DGS4, out _);
            SL24_DC30_CH6.Measure(out SL24CH6_DGS4, out _);
            SL24_DC30_CH7.Measure(out SL24CH7_DGS4, out _);
            SL24_DC30_CH8.Measure(out SL24CH8_DGS4, out _);
            SL24_DC30_CH9.Measure(out SL24CH9_DGS4, out _);
            SL24_DC30_CH10.Measure(out SL24CH10_DGS4, out _);
            SL24_DC30_CH11.Measure(out SL24CH11_DGS4, out _);
            SL24_DC30_CH12.Measure(out SL24CH12_DGS4, out _);
            SL24_DC30_CH13.Measure(out SL24CH13_DGS4, out _);
            SL24_DC30_CH14.Measure(out SL24CH14_DGS4, out _);
            SL24_DC30_CH15.Measure(out SL24CH15_DGS4, out _);
            SL24_DC30_CH16.Measure(out SL24CH16_DGS4, out _);
            SL24_DC30_CH17.Measure(out SL24CH17_DGS4, out _);
            SL24_DC30_CH18.Measure(out SL24CH18_DGS4, out _);
            SL24_DC30_CH19.Measure(out SL24CH19_DGS4, out _);
            SL24_DC30_CH20.Measure(out SL24CH20_DGS4, out _);

            //return to initial settings
            SL24_DC30.ForceCurrent(currentLevel: 0, voltageLimit: 24); // force current on 1Kohms resistor
            SL24_DC30.Abort();
            SL24_DC30.ConfigureOutputEnabled(false);
            SL24_DC30.ConfigureOutputConnected(false);

            // bin out results
            //DGS = 1V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL24_DC30.PinQueryContext.Publish(MeasDC30_SL24_DGS1, "SL24_DC30_CHANNELS_DGS1");
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1_DGS1, "SL24_DC30_CH1_DGS1");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2_DGS1, "SL24_DC30_CH2_DGS1");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3_DGS1, "SL24_DC30_CH3_DGS1");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4_DGS1, "SL24_DC30_CH4_DGS1");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5_DGS1, "SL24_DC30_CH5_DGS1");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6_DGS1, "SL24_DC30_CH6_DGS1");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7_DGS1, "SL24_DC30_CH7_DGS1");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8_DGS1, "SL24_DC30_CH8_DGS1");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9_DGS1, "SL24_DC30_CH9_DGS1");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10_DGS1, "SL24_DC30_CH10_DGS1");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11_DGS1, "SL24_DC30_CH11_DGS1");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12_DGS1, "SL24_DC30_CH12_DGS1");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13_DGS1, "SL24_DC30_CH13_DGS1");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14_DGS1, "SL24_DC30_CH14_DGS1");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15_DGS1, "SL24_DC30_CH15_DGS1");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16_DGS1, "SL24_DC30_CH16_DGS1");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17_DGS1, "SL24_DC30_CH17_DGS1");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18_DGS1, "SL24_DC30_CH18_DGS1");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19_DGS1, "SL24_DC30_CH19_DGS1");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20_DGS1, "SL24_DC30_CH20_DGS1");

            //DGS = 2V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL24_DC30.PinQueryContext.Publish(MeasDC30_SL24_DGS2, "SL24_DC30_CHANNELS_DGS2");
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1_DGS2, "SL24_DC30_CH1_DGS2");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2_DGS2, "SL24_DC30_CH2_DGS2");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3_DGS2, "SL24_DC30_CH3_DGS2");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4_DGS2, "SL24_DC30_CH4_DGS2");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5_DGS2, "SL24_DC30_CH5_DGS2");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6_DGS2, "SL24_DC30_CH6_DGS2");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7_DGS2, "SL24_DC30_CH7_DGS2");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8_DGS2, "SL24_DC30_CH8_DGS2");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9_DGS2, "SL24_DC30_CH9_DGS2");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10_DGS2, "SL24_DC30_CH10_DGS2");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11_DGS2, "SL24_DC30_CH11_DGS2");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12_DGS2, "SL24_DC30_CH12_DGS2");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13_DGS2, "SL24_DC30_CH13_DGS2");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14_DGS2, "SL24_DC30_CH14_DGS2");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15_DGS2, "SL24_DC30_CH15_DGS2");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16_DGS2, "SL24_DC30_CH16_DGS2");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17_DGS2, "SL24_DC30_CH17_DGS2");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18_DGS2, "SL24_DC30_CH18_DGS2");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19_DGS2, "SL24_DC30_CH19_DGS2");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20_DGS2, "SL24_DC30_CH20_DGS2");

            //DGS = 3V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL24_DC30.PinQueryContext.Publish(MeasDC30_SL24_DGS3, "SL24_DC30_CHANNELS_DGS3");
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1_DGS3, "SL24_DC30_CH1_DGS3");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2_DGS3, "SL24_DC30_CH2_DGS3");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3_DGS3, "SL24_DC30_CH3_DGS3");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4_DGS3, "SL24_DC30_CH4_DGS3");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5_DGS3, "SL24_DC30_CH5_DGS3");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6_DGS3, "SL24_DC30_CH6_DGS3");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7_DGS3, "SL24_DC30_CH7_DGS3");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8_DGS3, "SL24_DC30_CH8_DGS3");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9_DGS3, "SL24_DC30_CH9_DGS3");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10_DGS3, "SL24_DC30_CH10_DGS3");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11_DGS3, "SL24_DC30_CH11_DGS3");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12_DGS3, "SL24_DC30_CH12_DGS3");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13_DGS3, "SL24_DC30_CH13_DGS3");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14_DGS3, "SL24_DC30_CH14_DGS3");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15_DGS3, "SL24_DC30_CH15_DGS3");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16_DGS3, "SL24_DC30_CH16_DGS3");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17_DGS3, "SL24_DC30_CH17_DGS3");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18_DGS3, "SL24_DC30_CH18_DGS3");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19_DGS3, "SL24_DC30_CH19_DGS3");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20_DGS3, "SL24_DC30_CH20_DGS3");

            //DGS = 4V REF
            // Globals.TheExec.Flow.TestLimit(resultval: MeasDC30_SL24[0], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //pin group bin out
            SL24_DC30.PinQueryContext.Publish(MeasDC30_SL24_DGS4, "SL24_DC30_CHANNELS_DGS4");
            //per pin bin out
            SL24_DC30_CH1.PinQueryContext.Publish(SL24CH1_DGS4, "SL24_DC30_CH1_DGS4");
            SL24_DC30_CH2.PinQueryContext.Publish(SL24CH2_DGS4, "SL24_DC30_CH2_DGS4");
            SL24_DC30_CH3.PinQueryContext.Publish(SL24CH3_DGS4, "SL24_DC30_CH3_DGS4");
            SL24_DC30_CH4.PinQueryContext.Publish(SL24CH4_DGS4, "SL24_DC30_CH4_DGS4");
            SL24_DC30_CH5.PinQueryContext.Publish(SL24CH5_DGS4, "SL24_DC30_CH5_DGS4");
            SL24_DC30_CH6.PinQueryContext.Publish(SL24CH6_DGS4, "SL24_DC30_CH6_DGS4");
            SL24_DC30_CH7.PinQueryContext.Publish(SL24CH7_DGS4, "SL24_DC30_CH7_DGS4");
            SL24_DC30_CH8.PinQueryContext.Publish(SL24CH8_DGS4, "SL24_DC30_CH8_DGS4");
            SL24_DC30_CH9.PinQueryContext.Publish(SL24CH9_DGS4, "SL24_DC30_CH9_DGS4");
            SL24_DC30_CH10.PinQueryContext.Publish(SL24CH10_DGS4, "SL24_DC30_CH10_DGS4");
            SL24_DC30_CH11.PinQueryContext.Publish(SL24CH11_DGS4, "SL24_DC30_CH11_DGS4");
            SL24_DC30_CH12.PinQueryContext.Publish(SL24CH12_DGS4, "SL24_DC30_CH12_DGS4");
            SL24_DC30_CH13.PinQueryContext.Publish(SL24CH13_DGS4, "SL24_DC30_CH13_DGS4");
            SL24_DC30_CH14.PinQueryContext.Publish(SL24CH14_DGS4, "SL24_DC30_CH14_DGS4");
            SL24_DC30_CH15.PinQueryContext.Publish(SL24CH15_DGS4, "SL24_DC30_CH15_DGS4");
            SL24_DC30_CH16.PinQueryContext.Publish(SL24CH16_DGS4, "SL24_DC30_CH16_DGS4");
            SL24_DC30_CH17.PinQueryContext.Publish(SL24CH17_DGS4, "SL24_DC30_CH17_DGS4");
            SL24_DC30_CH18.PinQueryContext.Publish(SL24CH18_DGS4, "SL24_DC30_CH18_DGS4");
            SL24_DC30_CH19.PinQueryContext.Publish(SL24CH19_DGS4, "SL24_DC30_CH19_DGS4");
            SL24_DC30_CH20.PinQueryContext.Publish(SL24CH20_DGS4, "SL24_DC30_CH20_DGS4");
        }

       // public static void Select_METER_option(ISemiconductorModuleContext tsmContext, bool MeterOption)
        public static void Reset_All_HMODs(ISemiconductorModuleContext tsmContext)
       //toggle reset pins of all HMODs(Tx Board and Checker Board)
        {
            //Reset all HMODs(Tx Board and Checker Board)
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital RESET_PINS = InstrCtrl.DigitalPinsToSessions(tsmContext, "RESET_PINS"); //initiate pin to session
            RESET_PINS.WriteStatic(PinState._0); //toggle low
            Globals.TheHdw.Wait(Globals.SettlingTime);
            RESET_PINS.WriteStatic(PinState._1); 
        }

        public static void PowerDown_Checker(ISemiconductorModuleContext tsmContext)
        {
            Globals.dmmglobal = InstrCtrl.DmmPinsToSessions(tsmContext, "METER_4081");
            Globals.dmmglobal.Abort();
            Globals.dmmglobal.ConfigureDmmSessions(DmmMeasurementFunction.DCVolts, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 10);
            Globals.dmmglobal.Initiate();

            DCPower DC90_PINS = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "DC90_PINS");
            DC90_PINS.Abort();
            DC90_PINS.ForceVoltage(voltageLevel: 0, currentLimit: 10e-3);
            DC90_PINS.ConfigureVoltageLevelRange(6);
            DC90_PINS.Initiate();
            DC90_PINS.ConfigureOutputConnected();
            DC90_PINS.ConfigureOutputEnabled();

            DCPower ALL_DC30_CH = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "ALL_DC30_CH");
 
            ALL_DC30_CH.ForceVoltage(voltageLevel: 0, currentLimit: 10e-3); 



        }

    }



    /*    public class DC90_Relay
        {
            private readonly VbtApplicationImpl _application;
            private readonly DAQMxConfig _setUpDaqMxConfig;
            private readonly DAQMxConfig _cleanUpDaqMxConfig;

            private static bool[] BitsToArrayLSB(uint value, int width)
            {
                // bit 0 -> first channel in the task, bit 1 -> second, etc.
                var bits = new bool[width];
                for (int i = 0; i < width; i++)
                    bits[i] = ((value >> i) & 1u) != 0;
                return bits;
            }


            public static void DC90_relay_on(string pinName)
            {

                //DAQMxConfig relay_on = new DAQMxConfig
                //{
                //    PinGroup = new string[] { pinName },
                //    LineData = new uint[] { 0b_0 }
                //};
                //DAQMxConfig relay_off = new DAQMxConfig
                //{
                //    PinGroup = new string[] { pinName },
                //    LineData = new uint[] { 0b_1 }
                //};
                DAQMxConfig relay_on = new DAQMxConfig
                {
                    PinGroup = new string[] { pinName },
                    LineData = new uint[] { 0xFFFFFFFF }
                };
                DAQMxConfig relay_off = new DAQMxConfig
                {
                    PinGroup = new string[] { pinName },
                    LineData = new uint[] { 0b_0000 }
                };

                // Get the packed 11-bit pattern from your config (LineData[0])
                uint value11 = 0u;
                if (relay_on.LineData != null && relay_on.LineData.Length > 0)
                    value11 = (uint)relay_on.LineData[0];

                // Constrain to 11 bits (lines 0..10)
                value11 &= 0x1111;

                for (int i = 0; i < relay_on.PinGroup.Length; i++)
                {


                    var platform = Platform.PlatformGetters.get_vbtflexni();
                    var tasksBundle = platform.sessionManager.DAQmx(relay_on.PinGroup[i]);

                    tasksBundle.Do(taskInfo =>
                    {
                        int chCount = taskInfo.Task.DOChannels.Count;
                        if (chCount <= 0)
                            throw new InvalidOperationException("No DO channels in the task for this pin group.");

                        if (chCount == 1)
                        {
                            // Single-channel task: decide line vs. port by the physical name
                            var ch = taskInfo.Task.DOChannels[0];
                            string phys = ch.PhysicalName ?? string.Empty;

                            var singleWriter = new DigitalSingleChannelWriter(taskInfo.Task.Stream);

                            if (phys.IndexOf("/line", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // Single **line** channel ? one boolean
                                bool bit0 = (value11 & 0x1u) != 0;
                                singleWriter.WriteSingleSampleSingleLine(true, bit0);
                            }
                            else
                            {
                                // Single **port** channel ? one uint (lower 11 bits used)
                                singleWriter.WriteSingleSamplePort(true, value11);
                            }
                        }
                        else
                        {
                            // Multi-channel task (your pinmap case: 11 line channels)
                            // Use DigitalMultiChannelWriter and provide one bool per channel.
                            var multiWriter = new DigitalMultiChannelWriter(taskInfo.Task.Stream);

                            bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...
                            multiWriter.WriteSingleSampleSingleLine(true, bits);
                        }
                    });


                }
            }

            public static void DC90_relay_off(string pinName)
            {
                MaximTest_Module.Maxim_Test_DisplayStored();
                DAQMxConfig relay_off = new DAQMxConfig
                {
                    PinGroup = new string[] { pinName },
                    LineData = new uint[] { 0xFFFFFFFF }
                };

                // Get the packed 11-bit pattern from your config (LineData[0])
                uint value11 = 0u;

                value11 &= 0x00000000;

                for (int i = 0; i < relay_off.PinGroup.Length; i++)
                {


                    var platform = Platform.PlatformGetters.get_vbtflexni();
                    var tasksBundle = platform.sessionManager.DAQmx(relay_off.PinGroup[i]);

                    tasksBundle.Do(taskInfo =>
                    {
                        int chCount = taskInfo.Task.DOChannels.Count;

                        // Multi-channel task (your pinmap case: 11 line channels)
                        // Use DigitalMultiChannelWriter and provide one bool per channel.
                        var multiWriter = new DigitalMultiChannelWriter(taskInfo.Task.Stream);

                        bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...
                        multiWriter.WriteSingleSampleSingleLine(true, bits);
                        //taskInfo.Task.Stop();

                    });


                }
            }

            public static void DC90_relay_continuity(string pinName)
            {
                uint value11 = 0u;
                value11 = 0x0000FFFF;
                //value11 = 0xFFFFFFFF;
                var platform = Platform.PlatformGetters.get_vbtflexni();
                var tasksBundle = platform.sessionManager.DAQmx(pinName);

                tasksBundle.Do(taskInfo =>
                {
                    taskInfo.Task.Control(TaskAction.Verify);
                    int chCount = taskInfo.Task.DOChannels.Count;
                    string device = taskInfo.Task.Devices[0];

                    if (device == "DAQ_P119_P115_P123_P127")
                    {
                        // Use DigitalMultiChannelWriter and provide one bool per channel.
                        var Writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                        bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...                  
                                                                         // taskInfo.Task.Start();
                        Writer.WriteSingleSampleSingleLine(true, bits);

                    }
                    if (device == "DAQ_P117_P113_P105_109")
                    {
                        value11 = 0x00000000;
                        // Use DigitalMultiChannelWriter and provide one bool per channel.
                        var Writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                        bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...                  
                                                                         // taskInfo.Task.Start();
                        Writer.WriteSingleSampleSingleLine(true, bits);

                    }

                    //// Use DigitalMultiChannelWriter and provide one bool per channel.
                    //var Writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                    //bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...                  
                    //// taskInfo.Task.Start();
                    //Writer.WriteSingleSampleSingleLine(true, bits);

                });
            }

            public static void DC90_relay_toggle(string pinName)
            {
                uint value11 = 0u;
                value11 = 0b_1;
                //value11 = 0xFFFFFFFF;
                var platform = Platform.PlatformGetters.get_vbtflexni();
                var tasksBundle = platform.sessionManager.DAQmx(pinName);

                tasksBundle.Do(taskInfo =>
                {
                    int chCount = taskInfo.Task.DOChannels.Count;

                    // Use DigitalMultiChannelWriter and provide one bool per channel.
                    var Writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                    bool[] bits = BitsToArrayLSB(value11, chCount);  // bit0 -> CH[0], bit1 -> CH[1], ...                  
                                                                     // taskInfo.Task.Start();
                    Writer.WriteSingleSampleSingleLine(true, bits);

                });
            }
        } */ // DC90_relay
}
