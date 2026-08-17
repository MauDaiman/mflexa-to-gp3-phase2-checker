using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;


namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class Setup_CleanUp
    {

        public static void PowerSupply_Setup(ISemiconductorModuleContext tsmContext)
        {
            DCPower DIB_POS5V_1 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DIB_POS5V_1"); // So redundant
            DCPower DIB_POS5V_2 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DIB_POS5V_2");
            DCPower DIB_POS5V_3 = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DIB_POS5V_3");
            //DCPower DIB_SMU4147_POS5V = InstrCtrl.DCPowerPinsToSessions(tsmcontext, "DIB_SMU4147_POS5V");
            DCPower SMU4139_POS5V = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SMU4139_POS5V");

            DIB_POS5V_1.ConfigureSense();
            DIB_POS5V_2.ConfigureSense();
            DIB_POS5V_3.ConfigureSense();
            SMU4139_POS5V.ConfigureSense();

            Globals.TheHdw.Wait(1 * Globals.mS);

            //MFLEX DIB SUPPLY +5V (1, 2, 3)
            DIB_POS5V_1.ForceVoltage(6.5, 2);
            DIB_POS5V_2.ForceVoltage(6.5, 2);
            DIB_POS5V_3.ForceVoltage(6.5, 2);

            //-5 XOR and -2V out
            SMU4139_POS5V.ForceVoltage(-6.5, 3);

            bool enablePos6v = true;   //HMOD +5V
            bool enablePos20v = true;  //TFE +5V
            bool enableNeg20v = true;  //Comp -5.2V
            bool enablePos12vAtP143 = false;
            bool enablePos12vAtP179 = true; //Relay Supply +12V PS2?
            bool enablePos24vAtP102Ch0 = true; //Master Support Bd +15V PS1?
            bool enablePos24vAtP102Ch1 = true; //Slave Support Bd +15V PS2?
            bool enablePos48vAtP179 = true; //DIB User supply +12V
            bool enablePos48vAtP143 = false;
            LoadBoardCtrl.EnableLoadBoardSupplies(tsmContext, enablePos6v, enablePos20v, enableNeg20v, enablePos12vAtP143, enablePos12vAtP179, enablePos24vAtP102Ch0, enablePos24vAtP102Ch1, enablePos48vAtP179, enablePos48vAtP143);

        }

        public static void DCSetup(ISemiconductorModuleContext tsmContext, 
            DCPowerMeasurementSense DCSense, 
            Double VDD_ApertureTime = .001,
            //Double VO_dc30__ApertureTime = .001,
            DCPowerSourceTransientResponse dCPowerSourceTransientResponse = DCPowerSourceTransientResponse.Normal)
        {
            DCPower AllDC = InstrCtrl.DCPowerPinsToSessions(tsmContext, "ALLDC");
            AllDC.ForceVoltage(0, 100e-3);
            Globals.TheHdw.Wait(3e-3);
            AllDC.ConfigureOutputEnabled(false);
            AllDC.ConfigureSense(DCSense, true);

            //DCPower VDD = InstrCtrl.DCPowerPinsToSessions(tsmContext, new string[] { "VDD1", "VDD2"});
            //DCPower VO_dc30 = InstrCtrl.DCPowerPinsToSessions(tsmContext, new string[] { "VOA_dc30_da", "VOB_dc30_da" });

            AllDC.ConfigureSettings(apertureTime: VDD_ApertureTime, initiateSessionAfter: true, transientResponse: dCPowerSourceTransientResponse);
            //VO_dc30.ConfigureSettings(apertureTime: VO_dc30__ApertureTime, initiateSessionAfter: true, transientResponse: dCPowerSourceTransientResponse);

            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
            var digitalssc = sessions.SSC;

            sessions.Abort();
            sessions.PPMUConfigureApertureTime(0.000004);   //set to lowest aperture time
        }

            public static void PowerSupply_CleanUp(ISemiconductorModuleContext tsmContext)
        {
            DCPower DIB_SMU4147_POS5V = InstrCtrl.DCPowerPinsToSessions(tsmContext, "DIB_SMU4147_POS5V");
            DCPower SMU4139_POS5V = InstrCtrl.DCPowerPinsToSessions(tsmContext, "SMU4139_POS5V");

            //MFLEX DIB SUPPLY +5V (1, 2, 3)
            DIB_SMU4147_POS5V.ForceVoltage(0, 3);

            //-5 XOR and -2V out
            SMU4139_POS5V.ForceVoltage(0, 3);

            Globals.TheHdw.Wait(1 * Globals.mS);

            DIB_SMU4147_POS5V.ConfigureSense(DCPowerMeasurementSense.Local);
            SMU4139_POS5V.ConfigureSense(DCPowerMeasurementSense.Local);

            bool enablePos6v = false;   //HMOD +5V
            bool enablePos20v = false;  //TFE +5V
            bool enableNeg20v = false;  //Comp -5.2V
            bool enablePos12vAtP143 = false;
            bool enablePos12vAtP179 = false; //Relay Supply +12V PS2?
            bool enablePos24vAtP102Ch0 = false; //Master Support Bd +15V PS1?
            bool enablePos24vAtP102Ch1 = false; //Slave Support Bd +15V PS2?
            bool enablePos48vAtP179 = false; //DIB User supply +12V
            bool enablePos48vAtP143 = false;
            LoadBoardCtrl.EnableLoadBoardSupplies(tsmContext, enablePos6v, enablePos20v, enableNeg20v, enablePos12vAtP143, enablePos12vAtP179, enablePos24vAtP102Ch0, enablePos24vAtP102Ch1, enablePos48vAtP179, enablePos48vAtP143);
        }
    }
}
