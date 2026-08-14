using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.LTX
{
    public static class Instrument_GPIB
    {
        public static bool CheckIfDevicePresetAtGPIBAddress(long GPIBAddress)
        {
            return true;
        }

        public static bool CheckIfFilePresent(string FileName)
        {
            return true;
        }

        public static bool CheckIfGPIBHeadIsController()
        {
            return true;
        }

        public static bool CheckGPIBServiceRequestFromAddress(long GPIBAddress)
        {
            return true;
        }

        public static bool CheckGPIBServiceRequest()
        {
            return true;
        }
    }
}
