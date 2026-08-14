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
    public class DcviSupplyStatic
    {
        /*public static void Main(Pattern ThePat, double VDD1_value, 
            double idd1_value, double VDD2_value, double idd2_value, 
            PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, 
            PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins, bool do_ipat)
        {
            TheHdw thehdw = new TheHdw();
            TheExec theExec = new TheExec();
            double mS = 0.001;
            tlStrobeOption tlStrobe = new tlStrobeOption();
            tlDCVIMeterReadingFormat tlDCVIMeterReadingFormatAverage = new tlDCVIMeterReadingFormat();
            int chStaticStateHi = DriveState.chStaticStateHi;
            int chStaticStateLo = DriveState.chStaticStateLo;
            string scaleMilli = "scaleMilli", unitAmp = "unitAmp", tlForceFlow = "tlForceFlow";
            TlRelayMode tlPowered = new TlRelayMode();
            bool tlUtilBitOn = true;
            bool tlUtilBitOff = false;

            SiteDouble IDD_VDD1_high_1p9_1p9;
            SiteDouble IDD_VDD1_low_1p9_1p9;
            SiteDouble IDD_VDD2_high_1p9_1p9;
            SiteDouble IDD_VDD2_low_1p9_1p9;

            // Set or clear prebody relays				
            if (PrebodyUtil1Pins != null)
            {
                thehdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
            }
            if (PrebodyUtil0Pins != null)
            {
                thehdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
            }

            PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);

            thehdw.Digital.ApplyLevelsTiming(true, true, true, tlPowered);

            if (ThePat != null)
            {
                thehdw.Digital.Patterns.Pat(ThePat).Run("inputs_hi");
            }
            else
            {
                thehdw.Pins["VIA, VIB"].ForceStaticLevel(chStaticStateHi);
            }

            thehdw.Digital.DisconnectPins("VOB, VOA");
            thehdw.Wait(2 * mS);

            // Measure IDDx Inputs High				
            IDD_VDD1_high_1p9_1p9 = thehdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            IDD_VDD2_high_1p9_1p9 = thehdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            //VOL_20uA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000.00, tlDCVIMeterReadingFormatAverage);

            if (ThePat != null)
            {
                thehdw.Digital.Patterns.Pat[ThePat].Run("inputs_lo");
            }
            else
            {
                thehdw.Pins["VIA, VIB"].ForceStaticLevel(chStaticStateLo);
            }

            // Measure IDDx Inputs Low				
            IDD_VDD1_low_1p9_1p9 = thehdw.DCVI.Pins["VDD1"].Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            IDD_VDD2_low_1p9_1p9 = thehdw.DCVI.Pins["VDD2"].Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);

            // TheExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);				
            // TheExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);				
            // TheExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);				
            // TheExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);				

            switch (TheExec.CurrentJob)
            {
                case "ADuM225_pd_ipat":
                case "ADuM225_char":
                    if (do_ipat)
                    {
                        ipat.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        ipat.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        ipat.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        ipat.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    }
                    else
                    {
                        theExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        theExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        theExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                        theExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    }
                    break;
                case "ADuM225_handtest":
                case "ADuM225_qc":
                    theExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    theExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    theExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    theExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: scaleMilli, unit: unitAmp, ForceResults: tlForceFlow);
                    break;
                case default:
                    break;
            }

            // Set or clear postbody relays				
            if (PostbodyUtil1Pins != null)
            {
                thehdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
            }
            if (PostbodyUtil0Pins != null)
            {
                thehdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
            }
        }*/

    }
}
