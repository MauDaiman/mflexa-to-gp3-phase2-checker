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
    public class input_output_levels_1p7_1p7_pd
    {
        public static void Main(ISemiconductorModuleContext tsmContext, PinList TestPins)
        {
            TheHdw thehdw = new TheHdw();
            TheExec theExec = new TheExec();
            double mA = 0.001, uA = 0.000001;
            Globals.tsmContext = tsmContext;
            int tlDCVIConnectHighForce = DCVIConstants.tlDCVIConnectHighForce;
            int tlDCVIConnectHighSense = DCVIConstants.tlDCVIConnectHighSense;
            int tlDCVIModeCurrent = DCVIMode.tlDCVIModeCurrent;
            int tlDCVIModeVoltage = DCVIMode.tlDCVIModeVoltage;
            int tlDCVIMeterVoltage = DCVIMeterMode.tlDCVIMeterVoltage;
            int tlDCVIComplianceBoth = tlDCVICompliance.Both;
            string scaleNoScaling = "scaleNoScaling", unitVolt = "unitVolt", tlForceFlow = "tlForceFlow";
            TlRelayMode tlPowered = new TlRelayMode();
            tlStrobeOption tlStrobe = new tlStrobeOption();
            tlDCVIMeterReadingFormat tlDCVIMeterReadingFormatAverage = new tlDCVIMeterReadingFormat();
            int chStaticStateHi = DriveState.chStaticStateHi;
            int chStaticStateLo = DriveState.chStaticStateLo;

            PinListData VOL_20uA;
            PinListData VOL_2mA;
            PinListData VOL_4mA;
            PinListData VOH_20uA;
            PinListData VOH_2mA;
            PinListData VOH_4mA;

            thehdw.DCVI.Pins("VDD1").Mode = tlDCVIModeVoltage;
            thehdw.DCVI.Pins("VDD1").ComplianceRange[tlDCVIComplianceBoth] = 10;
            thehdw.DCVI.Pins("VDD1").SetVoltageAndRange(1.7, 10);    //'VDD1 4p5/4p5
            thehdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            thehdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;
            thehdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VDD1").Gate = true;

            thehdw.DCVI.Pins("VDD2").Mode = tlDCVIModeVoltage;
            thehdw.DCVI.Pins("VDD2").ComplianceRange[tlDCVIComplianceBoth] = 10;
            thehdw.DCVI.Pins("VDD2").SetVoltageAndRange(1.7, 10);    //'VDD2 4p5/4p5
            thehdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            thehdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;
            thehdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VDD2").Gate = true;

            //'Apply Levels and Timing ***What should be the fields set below***
            thehdw.Digital.ApplyLevelsTiming(true, true, true, tlPowered);

            thehdw.Digital.DisconnectPins("VOB_dc30_da, VOA_dc30_da");
            thehdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(20 * uA, 2 * mA);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(10, 10);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = false;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Value = 500;
            thehdw.Wait(0.001);//settle wait to prevent hotswitching
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = true;

            thehdw.Wait(0.001);
            VOL_20uA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000.00, tlDCVIMeterReadingFormatAverage);

            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(2 * mA, 20 * mA);
            thehdw.Wait(0.001);
            VOL_2mA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);

            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(4 * mA, 20 * mA);
            thehdw.Wait(0.001);
            VOL_4mA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);

            thehdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-20 * uA, 2 * mA);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(-10, 10);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = false;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Value = 500;
            thehdw.Wait(0.001);//settle wait to prevent hotswitching
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            thehdw.Wait(0.001);
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = true;

            thehdw.Wait(0.001);
            VOH_20uA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);

            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-2 * mA, 20 * mA);
            thehdw.Wait(0.001);
            VOH_2mA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);

            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-4 * mA, 20 * mA);
            thehdw.Wait(0.001);
            VOH_4mA = thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);

            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = false;
            thehdw.Wait(0.001);//settle wait to prevent hotswitching
            thehdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Disconnect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);

            thehdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);

            theExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            theExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            theExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            theExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            theExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
            theExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: scaleNoScaling, unit: unitVolt, ForceResults: tlForceFlow);
        }
    }
}
