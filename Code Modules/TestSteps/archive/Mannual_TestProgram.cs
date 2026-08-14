using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;


namespace TestSteps
{
    class TestProgram
    {
        public static void continuity_pd(PinList TestPins)
        {
            PinListData pos_diode;
            PinListData neg_diode;

            Globals.thehdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeVoltage;
            Globals.thehdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;
            Globals.thehdw.DCVI.Pins("VDD1").SetVoltageAndRange(0, 5);    //'VDD1 cty
            Globals.thehdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.thehdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;
            Globals.thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            Globals.thehdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);
            Globals.thehdw.Wait(0.001);
            Globals.thehdw.DCVI.Pins("VDD1").Gate = true;

            Globals.thehdw.DCVI.Pins("VDD2").Mode = Globals.tlDCVIModeVoltage;
            Globals.thehdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;
            Globals.thehdw.DCVI.Pins("VDD2").SetVoltageAndRange(0, 5);    //VDD2 cty
            Globals.thehdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.thehdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;
            Globals.thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            Globals.thehdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);
            Globals.thehdw.Wait(0.001);
            Globals.thehdw.DCVI.Pins("VDD2").Gate = true;

            //Apply Levels and Timing
            Globals.thehdw.Digital.ApplyLevelsTiming(false, true, false, Globals.tlPowered, initPinsHiz: TestPins.Value);

            //Connect PPMU's 
            Globals.thehdw.PPMU.Pins(TestPins).Connect();

            Globals.thehdw.PPMU.Pins(TestPins).ForceI(0.0005, 2 * Globals.mA);
            pos_diode = Globals.thehdw.PPMU.Pins(TestPins).Read();

            Globals.thehdw.PPMU.Pins(TestPins).ForceI(-0.0005, 2 * Globals.mA);
            Globals.thehdw.Wait(0.001);
            neg_diode = Globals.thehdw.PPMU.Pins(TestPins).Read();

            Globals.thehdw.PPMU.Pins(TestPins).ForceI(0);
            Globals.thehdw.PPMU.Pins(TestPins).ForceV(0);
            Globals.thehdw.PPMU.Pins(TestPins).Disconnect();
            Globals.thehdw.Wait(0.001);

            Globals.thehdw.Digital.DisconnectPins(TestPins);

            Globals.theExec.Flow.TestLimit(resultval: pos_diode, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.theExec.Flow.TestLimit(resultval: neg_diode, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
        }

        public static void vil_vih_1p7_1p7_pd(string ThePat, PinList InPins, PinList OutPins, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value)
        {
            double v = 1.0;

            //var nSite = 0;
            long i;
            long SiteNum = 0;
            long MaxLoops;
            long NumActiveSites;
            long TestPinsNum;
            string[] InPinsArray;
            string[] OutPinsArray;

            Globals.theExec.DataManager.DecomposePinList(InPins, out InPinsArray, out TestPinsNum);
            Globals.theExec.DataManager.DecomposePinList(OutPins, out OutPinsArray, out TestPinsNum);

            // Resize arrays
            SiteLong[] FailCount = new SiteLong[TestPinsNum];
            SiteDouble[] VilVoltage = new SiteDouble[TestPinsNum];
            SiteDouble[] VihVoltage = new SiteDouble[TestPinsNum];
            SiteDouble[] VilInitVoltage = new SiteDouble[TestPinsNum];
            SiteDouble[] VihInitVoltage = new SiteDouble[TestPinsNum];
            double[] VilCoarseVoltage = new double[TestPinsNum];
            double[] VihCoarseVoltage = new double[TestPinsNum];

            //Set or clear prebody relays

            // Set supplies
            Globals.thehdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;
            Globals.thehdw.DCVI.Pins("VDD1").SetVoltageAndRange(VDD1_value, 10);    // 1.700 VDD1 1p7/1p7
            Globals.thehdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.thehdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;
            Globals.thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            Globals.thehdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);
            Globals.thehdw.Wait(0.001);
            Globals.thehdw.DCVI.Pins("VDD1").Gate = true;

            Globals.thehdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;
            Globals.thehdw.DCVI.Pins("VDD2").SetVoltageAndRange(VDD2_value, 10);    // 1.700 VDD2 1p7/1p7
            Globals.thehdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.thehdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;
            Globals.thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            Globals.thehdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);
            Globals.thehdw.Wait(0.001);
            Globals.thehdw.DCVI.Pins("VDD2").Gate = true;

            //Apply levels and timing
            Globals.thehdw.Digital.ApplyLevelsTiming(true, true, true, Globals.tlPowered);

            // Initialize InPinsArray
            foreach (dynamic nSite in Globals.theExec.Sites.Active)
            //for (nSite = 0; nSite < Globals.theExec.Sites.Active.Count; nSite++)
            {
                SiteNum = nSite;
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    VilInitVoltage[(int)i][nSite] = -10 * Globals.mV;
                    VihInitVoltage[0][nSite] = VDD1_value * v; // VIA
                    VihInitVoltage[1][nSite] = VDD1_value * v; // VIB
                    FailCount[(int)i][nSite] = 0;
                }
            }

            VilCoarseVoltage[0] = VilInitVoltage[0][(int)SiteNum];

            while (FailCount[0][(int)SiteNum] == 0 && VilCoarseVoltage[0] < VihInitVoltage[0][(int)SiteNum])
            {
                Globals.thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(Globals.chVil, VilCoarseVoltage[0]);
                Globals.thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                FailCount[0][(int)SiteNum] = Globals.thehdw.Pins(OutPinsArray[0]).FailCount(SiteNum);
                if (FailCount[0][(int)SiteNum] == 0)
                {
                    VilCoarseVoltage[0] = VilCoarseVoltage[0] + 250 * Globals.mV;
                }
                else
                {
                    Globals.thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(Globals.chVil, 0);
                    VilCoarseVoltage[0] = VilCoarseVoltage[0] - 250 * Globals.mV;
                }
            }

            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                VilInitVoltage[i] = -10 * Globals.mV;
            }

            FailCount[0][(int)SiteNum] = 0;
            VihCoarseVoltage[0] = VihInitVoltage[0][(int)SiteNum] * v;

            while (FailCount[0][(int)SiteNum] == 0 && VihCoarseVoltage[0] > VilInitVoltage[0][(int)SiteNum])
            {
                Globals.thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(Globals.chVih, VihCoarseVoltage[0]);
                Globals.thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                FailCount[0][(int)SiteNum] = Globals.thehdw.Pins(OutPinsArray[0]).FailCount(SiteNum);
                if (FailCount[0][(int)SiteNum] == 0)
                {
                    VihCoarseVoltage[0] = VihCoarseVoltage[0] - 250 * Globals.mV;
                }
                else
                {
                    Globals.thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(Globals.chVih, VihInitVoltage[0][(int)SiteNum]);
                    VihCoarseVoltage[0] = VihCoarseVoltage[0] + 250 * Globals.mV;
                }
            }

            FailCount[0][(int)SiteNum] = 0;

            // Initialize InPinsArray
            foreach (dynamic nSite in Globals.theExec.Sites.Active)
            {
                VilInitVoltage[0][(int)nSite] = VilCoarseVoltage[0] * v; // VIA
                VihInitVoltage[0][(int)nSite] = VihCoarseVoltage[0] * v; // VIA
                VilInitVoltage[1][(int)nSite] = VilCoarseVoltage[0] * v; // VIB
                VihInitVoltage[1][(int)nSite] = VihCoarseVoltage[0] * v; // VIB
            }

            foreach (var nSite in Globals.theExec.Sites.Active)
            {
                NumActiveSites = Globals.theExec.Sites.Active.Count;

                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    FailCount[i][(int)nSite] = 0;
                    // VilVoltage[i][nSite] = VilVoltage[i][nSite] + 800 * mV; //Temporary
                    VilVoltage[i][(int)nSite] = VilInitVoltage[i][(int)nSite];
                    while (FailCount[i][(int)nSite] == 0 && VilVoltage[i][(int)nSite] < (VihInitVoltage[i][(int)nSite] + 100 * Globals.mV))
                    {
                        Globals.thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(Globals.chVil, VilVoltage[i][(int)nSite]);
                        Globals.thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                        FailCount[i][(int)nSite] = Globals.thehdw.Pins(OutPinsArray[i]).FailCount((long)nSite);
                        if (FailCount[i][(int)nSite] == 0)
                        {
                            VilVoltage[i][(int)nSite] = VilVoltage[i][(int)nSite] + 10 * Globals.mV;
                        }
                        else
                        {
                            VilVoltage[i][(int)nSite] = VilVoltage[i][(int)nSite] - 10 * Globals.mV;
                        }
                    }
                    Globals.thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(Globals.chVil, 0);
                }
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    VilInitVoltage[i] = -10 * Globals.mV;
                }
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    FailCount[i][(int)nSite] = 0;
                    // VihVoltage[i][nSite] = 1.1 * V; // Temporary
                    VihVoltage[i][(int)nSite] = VihInitVoltage[i][(int)nSite] * v;
                    while (FailCount[i][(int)nSite] == 0 && VihVoltage[i][(int)nSite] > VilInitVoltage[i][(int)nSite])
                    {
                        Globals.thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(Globals.chVih, VihVoltage[i][(int)nSite]);
                        Globals.thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                        FailCount[i][(int)nSite] = Globals.thehdw.Pins(OutPinsArray[i]).FailCount((long)nSite);
                        if (FailCount[i][(int)nSite] == 0)
                        {
                            VihVoltage[i][(int)nSite] = VihVoltage[i][(int)nSite] - 10 * Globals.mV;
                        }
                        else
                        {
                            VihVoltage[i][(int)nSite] = VihVoltage[i][(int)nSite] + 10 * Globals.mV;
                        }
                    }
                    Globals.thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(Globals.chVih, VihInitVoltage[i][(int)nSite]);
                }
            }

            // Set or clear postbody relays
            // Re-Apply original levels and timing
            Globals.thehdw.Digital.ApplyLevelsTiming(true, true, true, Globals.tlPowered);
            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                Globals.theExec.Flow.TestLimit(VilVoltage[i], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            }
            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                Globals.theExec.Flow.TestLimit(VihVoltage[i], ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            }
            // Exit Function */
        }
    }
}
