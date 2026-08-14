using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using HMOD;
using NationalInstruments.ModularInstruments.NIDigital;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class Checker
    {
        public static void PowerSupply(ISemiconductorModuleContext tsmContext)
        {
            Dmm DMM = InstrCtrl.DmmPinsToSessions(tsmContext, "P143_4081_DMM");
            DMM.Abort();
            DMM.ConfigureDmmSessions(DmmMeasurementFunction.DCVolts, DmmApertureTimeUnits.Seconds, 1e-3, DmmAuto.Off, DmmAdcCalibration.Off, settleTimeSeconds: 0, voltageRange: 100);
            DMM.Initiate();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 65536); //HMOD18:K17
            Globals.TheHdw.Wait(.005);  //Allow relay connection before measure
            double[] DIB5V_1 = DMM.Read();


            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);  //allow relay disconnection
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 131072);   //HMOD18:K18
            Globals.TheHdw.Wait(.005);
            double[] DIB5V_2 = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 262144);   //HMOD18:K19
            Globals.TheHdw.Wait(.005);
            double[] DIB5V_3 = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 524288);   //HMOD18:K20
            Globals.TheHdw.Wait(.005);
            double[] DIB12V_Slave = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 1048576);   //HMOD18:K21
            Globals.TheHdw.Wait(.005);
            double[] MASTER15V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 2097152);   //HMOD18:K22
            Globals.TheHdw.Wait(.005);
            double[] SLAVE15V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 4194304);   //HMOD18:K23
            Globals.TheHdw.Wait(.005);
            double[] RELAY12V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 8388608);   //HMOD18:K24
            Globals.TheHdw.Wait(.005);
            double[] TFE5V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 16777216);   //HMOD18:K25
            Globals.TheHdw.Wait(.005);
            double[] HMOD5V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 33554432);   //HMOD18:K26
            Globals.TheHdw.Wait(.005);
            double[] COMPn5p2V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 67108864);   //HMOD18:K27
            Globals.TheHdw.Wait(.005);
            double[] XORn5V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);    //Disconnect before connecting to other supply
            Globals.TheHdw.Wait(.005);
            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 134217728);   //HMOD18:K28
            Globals.TheHdw.Wait(.005);
            double[] n2V = DMM.Read();

            HMODCtrl.HMOD14to18(tsmContext, HMOD_Data_18: 0);

            //tsmContext.PublishPerSite(DIB5V_1, "DIB_Supply_5V_1");
            //tsmContext.PublishPerSite(DIB5V_2, "DIB_Supply_5V_2");
            //tsmContext.PublishPerSite(DIB5V_3, "DIB_Supply_5V_3");
            //tsmContext.PublishPerSite(DIB12V_Slave, "DIB_Supply_12V_Slave");
            //tsmContext.PublishPerSite(MASTER15V, "Master_15V");
            //tsmContext.PublishPerSite(SLAVE15V, "Slave_15V");
            //tsmContext.PublishPerSite(RELAY12V, "Relay_Supply_12V");
            //tsmContext.PublishPerSite(TFE5V, "TFE_5V");
            //tsmContext.PublishPerSite(HMOD5V, "HMOD_5V");
            //tsmContext.PublishPerSite(COMPn5p2V, "COMP_n5p2V");
            //tsmContext.PublishPerSite(XORn5V, "XOR_n5V");
            //tsmContext.PublishPerSite(n2V, "n2V");
               
            tsmContext.PublishPerSite(new double[] { DIB5V_1[0], DIB5V_1[0], DIB5V_1[0], DIB5V_1[0] }, "DIB_Supply_5V_1");
            tsmContext.PublishPerSite(new double[] { DIB5V_2[0], DIB5V_2[0], DIB5V_2[0], DIB5V_2[0] }, "DIB_Supply_5V_2");
            tsmContext.PublishPerSite(new double[] { DIB5V_3[0], DIB5V_3[0], DIB5V_3[0], DIB5V_3[0] }, "DIB_Supply_5V_3");
            tsmContext.PublishPerSite(new double[] { DIB12V_Slave[0], DIB12V_Slave[0], DIB12V_Slave[0], DIB12V_Slave[0] }, "DIB_Supply_12V_Slave");
            tsmContext.PublishPerSite(new double[] { MASTER15V[0], MASTER15V[0], MASTER15V[0], MASTER15V[0] }, "Master_15V");
            tsmContext.PublishPerSite(new double[] { SLAVE15V[0], SLAVE15V[0], SLAVE15V[0], SLAVE15V[0] }, "Slave_15V");
            tsmContext.PublishPerSite(new double[] { RELAY12V[0], RELAY12V[0], RELAY12V[0], RELAY12V[0] }, "Relay_Supply_12V");
            tsmContext.PublishPerSite(new double[] { TFE5V[0], TFE5V[0], TFE5V[0], TFE5V[0] }, "TFE_5V");
            tsmContext.PublishPerSite(new double[] { HMOD5V[0], HMOD5V[0], HMOD5V[0], HMOD5V[0] }, "HMOD_5V");
            tsmContext.PublishPerSite(new double[] { COMPn5p2V[0], COMPn5p2V[0], COMPn5p2V[0], COMPn5p2V[0] }, "COMP_n5p2V");
            tsmContext.PublishPerSite(new double[] { XORn5V[0], XORn5V[0], XORn5V[0], XORn5V[0] }, "XOR_n5V");
            tsmContext.PublishPerSite(new double[] { n2V[0], n2V[0], n2V[0], n2V[0] }, "n2V");

            DMM.Abort();
        }
        public static void MeasureApplySaveTDR(ISemiconductorModuleContext tsmContext)
        {
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital pinSet = InstrCtrl.DigitalPinsToSessions(tsmContext, "digital");
            var tdrvalues = pinSet.MeasureTDROffsets(true);

            //pinSet.SaveTDROffsetsToFile(tdrvalues, "Code Modules/TDR/TDR.txt");
            pinSet.SaveTDROffsetsToFile(tdrvalues, "C:/Data/ADuM225N_TDR.txt");

            //Globals.tdrvalues = pinSet.LoadTDROffsetsFromFile("Code Modules/TDR/TDR.txt");
            Globals.tdrvalues = pinSet.LoadTDROffsetsFromFile("C:/Data/ADuM225N_TDR.txt");
            pinSet.ApplyTDROffsets(Globals.tdrvalues);
            double[][] TDRperPin = { new double[4], new double[4], new double[4], new double[4] };
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < tsmContext.SiteNumbers.Count; k++)
                {
                    TDRperPin[k][i] = Globals.tdrvalues[i][k].TotalSeconds;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                tsmContext.PublishPerSite(TDRperPin[i], "" + i);
            }
        }
        public static void ApplySavedTDR(ISemiconductorModuleContext tsmContext)
        {
            NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital pinSet = InstrCtrl.DigitalPinsToSessions(tsmContext, "digital");
            //Globals.tdrvalues = pinSet.LoadTDROffsetsFromFile("Code Modules/TDR/TDR.txt");
            Globals.tdrvalues = pinSet.LoadTDROffsetsFromFile("C:/Data/ADuM225N_TDR.txt");
            pinSet.ApplyTDROffsets(Globals.tdrvalues);
        }
    }
}
