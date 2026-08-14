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
    public class vil_vih_4p5_4p5_pd
    {
        public static void Main(ISemiconductorModuleContext tsmContext, Pattern ThePat, PinList InPins, PinList OutPins, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value)
        {
            TheHdw thehdw = new TheHdw();
            TheExec theExec = new TheExec();
            double mV = 0.001, v = 1;
            Globals.tsmContext = tsmContext;
            int tlDCVIConnectHighForce = DCVIConstants.tlDCVIConnectHighForce;
            int tlDCVIConnectHighSense = DCVIConstants.tlDCVIConnectHighSense;
            int tlDCVIComplianceBoth = tlDCVICompliance.Both;
            string scaleNoScaling = "scaleNoScaling", unitVolt = "unitVolt", tlForceFlow = "tlForceFlow";
            TlRelayMode tlPowered = new TlRelayMode();
            ChPinLevel chVil = ChPinLevel.chVil;
            ChPinLevel chVih = ChPinLevel.chVih;

            //dynamic nSite;
            SiteDouble[][] VilVoltage = new SiteDouble[0][];
            SiteDouble[][] VihVoltage = new SiteDouble[0][];
            SiteDouble[][] VilInitVoltage = new SiteDouble[0][];
            SiteDouble[][] VihInitVoltage = new SiteDouble[0][];
            double[] VilCoarseVoltage = null;
            double[] VihCoarseVoltage = null;
            SiteLong[][] FailCount = new SiteLong[0][];
            long i;
            long SiteNum = 0;
            //long MaxLoops;
            //long NumActiveSites;
            long TestPinsNum;
            string[] InPinsArray;
            string[] OutPinsArray;

            theExec.DataManager.DecomposePinList(InPins, out InPinsArray, out TestPinsNum);
            theExec.DataManager.DecomposePinList(OutPins, out OutPinsArray, out TestPinsNum);

            Array.Resize(ref FailCount, (int)TestPinsNum);
            Array.Resize(ref VilVoltage, (int)TestPinsNum);
            Array.Resize(ref VihVoltage, (int)TestPinsNum);
            Array.Resize(ref VilInitVoltage, (int)TestPinsNum);
            Array.Resize(ref VihInitVoltage, (int)TestPinsNum);
            Array.Resize(ref VilCoarseVoltage, (int)TestPinsNum);
            Array.Resize(ref VihCoarseVoltage, (int)TestPinsNum);


            //Set or clear prebody relays

            // Set supplies
            thehdw.DCVI.Pins("VDD1").ComplianceRange[tlDCVIComplianceBoth] = 10;
            thehdw.DCVI.Pins("VDD1").SetVoltageAndRange(VDD1_value, 10);    // 4.500 VDD1 4p5/4p5
            thehdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            thehdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;
            thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            thehdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VDD1").Gate = true;

            thehdw.DCVI.Pins("VDD2").ComplianceRange[tlDCVIComplianceBoth] = 10;
            thehdw.DCVI.Pins("VDD2").SetVoltageAndRange(VDD2_value, 10); // 4.500 VDD2 4p5/4p5
            thehdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            thehdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;
            thehdw.Wait(0.001); //settle wait to prevent hotswitching condition
            thehdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VDD2").Gate = true;

            //Apply levels and timing
            thehdw.Digital.ApplyLevelsTiming(true, true, true, tlPowered);

            // Initialize InPinsArray
            foreach (dynamic nSite in theExec.Sites.Active)
            {
                SiteNum = nSite;
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    VilInitVoltage[i][nSite] = -10 * mV;
                    VihInitVoltage[0][nSite] = VDD1_value * v; // VIA
                    VihInitVoltage[1][nSite] = VDD1_value * v; // VIB
                    FailCount[i][nSite] = 0;
                }
            }

            VilCoarseVoltage[0] = VilInitVoltage[0][SiteNum];
            while (FailCount[0][SiteNum] == 0 && VilCoarseVoltage[0] < VihInitVoltage[0][SiteNum])
            {
                thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(chVil, VilCoarseVoltage[0]);
                thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                FailCount[0][SiteNum] = thehdw.Pins(OutPinsArray[0]).FailCount(SiteNum);
                if (FailCount[0][SiteNum] == 0)
                {
                    VilCoarseVoltage[0] += 250 * mV;
                }
                else
                {
                    thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(chVil, 0);
                    VilCoarseVoltage[0] -= 250 * mV;
                }
            }

            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                VilInitVoltage[i] = new SiteDouble[1]; // Initialize inner array
                VilInitVoltage[i][0] = -10 * mV;
            }

            FailCount[0][SiteNum] = 0;
            VihCoarseVoltage[0] = VihInitVoltage[0][SiteNum] * v;
            while (FailCount[0][SiteNum] == 0 && VihCoarseVoltage[0] > VilInitVoltage[0][SiteNum])
            {
                thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(chVih, VihCoarseVoltage[0]);
                thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                FailCount[0][SiteNum] = thehdw.Pins(OutPinsArray[0]).FailCount(SiteNum);
                if (FailCount[0][SiteNum] == 0)
                {
                    VihCoarseVoltage[0] -= 250 * mV;
                }
                else
                {
                    thehdw.PinLevels.Pins(InPinsArray[0]).ModifyLevel(chVih, VihInitVoltage[0][SiteNum]);
                    VihCoarseVoltage[0] += 250 * mV;
                }
            }
            FailCount[0][SiteNum] = 0;
            // Initialize InPinsArray
            foreach (dynamic nSite in theExec.Sites.Active)
            {
                VilInitVoltage[0][(int)nSite] = VilCoarseVoltage[0] * v; // VIA
                VihInitVoltage[0][(int)nSite] = VihCoarseVoltage[0] * v; // VIA
                VilInitVoltage[1][(int)nSite] = VilCoarseVoltage[0] * v; // VIB
                VihInitVoltage[1][(int)nSite] = VihCoarseVoltage[0] * v; // VIB
            }

            foreach (var nSite in theExec.Sites.Active)
            {
                int NumActiveSites = theExec.Sites.Active.Count;

                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    FailCount[i][(int)nSite] = 0;
                    // VilVoltage[i][nSite] = VilVoltage[i][nSite] + 800 * mV; //Temporary
                    VilVoltage[i][(int)nSite] = VilInitVoltage[i][(int)nSite];
                    while (FailCount[i][(int)nSite] == 0 && VilVoltage[i][(int)nSite] < (VihInitVoltage[i][(int)nSite] + 100 * mV))
                    {
                        thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(chVil, VilVoltage[i][(int)nSite]);
                        thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                        FailCount[i][(int)nSite] = thehdw.Pins(OutPinsArray[i]).FailCount((long)nSite);
                        if (FailCount[i][(int)nSite] == 0)
                        {
                            VilVoltage[i][(int)nSite] = VilVoltage[i][(int)nSite] + 10 * mV;
                        }
                        else
                        {
                            VilVoltage[i][(int)nSite] = VilVoltage[i][(int)nSite] - 10 * mV;
                        }
                    }
                    thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(chVil, 0);
                }
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    VilInitVoltage[i] = new SiteDouble[1]; // Initialize inner array
                    VilInitVoltage[i][0] = -10 * mV;
                }
                for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
                {
                    FailCount[i][(int)nSite] = 0;
                    // VihVoltage[i][nSite] = 1.1 * V; // Temporary
                    VihVoltage[i][(int)nSite] = VihInitVoltage[i][(int)nSite] * v;
                    while (FailCount[i][(int)nSite] == 0 && VihVoltage[i][(int)nSite] > VilInitVoltage[i][(int)nSite])
                    {
                        thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(chVih, VihVoltage[i][(int)nSite]);
                        thehdw.Digital.Patterns.Pat(@".\vectors\vil_vih_search").Run();
                        FailCount[i][(int)nSite] = thehdw.Pins(OutPinsArray[i]).FailCount((long)nSite);
                        if (FailCount[i][(int)nSite] == 0)
                        {
                            VihVoltage[i][(int)nSite] = VihVoltage[i][(int)nSite] - 10 * mV;
                        }
                        else
                        {
                            VihVoltage[i][(int)nSite] = VihVoltage[i][(int)nSite] + 10 * mV;
                        }
                    }
                    thehdw.PinLevels.Pins(InPinsArray[i]).ModifyLevel(chVih, VihInitVoltage[i][(int)nSite]);
                }
            }

            // Set or clear postbody relays
            // Re-Apply original levels and timing
            thehdw.Digital.ApplyLevelsTiming(true, true, true, tlPowered);
            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                theExec.Flow.TestLimit(VilVoltage[i], ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            }
            for (i = 0; i <= InPinsArray.GetUpperBound(0); i++)
            {
                theExec.Flow.TestLimit(VihVoltage[i], ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            }
            // Exit Function
        }
    }
}
