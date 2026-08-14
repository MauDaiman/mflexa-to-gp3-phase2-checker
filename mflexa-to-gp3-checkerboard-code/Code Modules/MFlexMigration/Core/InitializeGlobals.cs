using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class InitializeGlobals
    {
        private const string jobName = "TBChecker";

        public static void Initialize(ISemiconductorModuleContext semiconductorModuleContext)
        {
            Globals.tsmContext = semiconductorModuleContext;

            List<int> siteNumbers = Globals.tsmContext.SiteNumbers.ToList();
            //Globals.Existing = GlobalFunctions.ConvertToBooleanList(siteNumbers);
            Globals.Existing = siteNumbers;

            Globals.TheExec.CurrentJob = jobName;
#if TestTimeMeasure
            // Creates Test Time Measure to a file
            Globals.TestTimeMeasure.CreateTestTimeResult();
#endif
        }

        public static void SetSatrtingSites(ISemiconductorModuleContext semiconductorModuleContext)
        {
            Globals.tsmContext = semiconductorModuleContext;

            List<int> siteNumbers = Globals.tsmContext.SiteNumbers.ToList();
            //Globals.Starting = GlobalFunctions.ConvertToBooleanList(siteNumbers);
            Globals.Starting = siteNumbers;
        }

    }
}
