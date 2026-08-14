using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class IPATManager
    {
        public long RunIPATTests()
        {
            // Simulate running IPAT tests and returning a result
            // Replace this with the actual implementation
            return 12345L;
        }

        public void TestLimit(
        dynamic resultval,
        double? lowVal = null,
        double? hiVal = null,
        string lowCompareSign = null,
        string highCompareSign = null,
        string ScaleType = null,
        string unit = null,
        string formatStr = null,
        string TName = null,
        string compareMode = null,
        PinList PinNames = null,
        double? forceVal = null,
        string forceunit = null,
        string customUnit = null,
        string customForceunit = null,
        string ForceResults = null
        )
        {
            PinListData results = resultval;
            string[] Pins;
            if (PinNames == null)
            {
                Pins = resultval.GetAllPinNames().ToArray();
            }
            else
            {
                Pins = PinNames.GetAll().ToArray();
            }

            string[] digitalPins = Globals.tsmContext.FilterPinsByInstrumentType(Pins, InstrumentTypeIdConstants.NIDigitalPattern);

            if (TName == null)
            {
                foreach (var PinName in digitalPins)
                {
                    List<double> pinDataList = results.PinData(PinName);
                    double[] pinDataArray = pinDataList.ToArray();
                    var pinQuery = Globals.tsmContext.GetNIDigitalPatternSessionsForPpmu(PinName, out NIDigital[] session, out string[] pinSetString);
                    pinQuery.Publish(pinDataArray, Globals.TNames[0]);
                }
            }
            else
            {
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    List<double> pinDataList = resultval.GetSiteDataForPin(Pin);
                    double[] pinDataArray = pinDataList.ToArray();
                    Globals.tsmContext.PublishPerSite(pinDataArray, TName + "_" + Pin, Pin);
                });
            }
        }


    }
}
