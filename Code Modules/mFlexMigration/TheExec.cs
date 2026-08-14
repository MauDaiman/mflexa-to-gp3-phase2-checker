using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.Interop.API;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class TheExec
    {
        public Datalog Datalog { get; private set; }
        public DataViewManager DataManager { get; private set; }
        public Flow Flow { get; private set; }
        public Sites Sites { get; private set; }

        public string ConfigurationName;

        public TheExec()
        {
            Datalog = new Datalog();
            DataManager = new DataViewManager();
            Flow = new Flow();
            Sites = new Sites();
        }

        // AddOutput
        // This method adds a message to the program output window.
        public void AddOutput(string message, int color = 0, bool bold = false)
        {
            // Implementation
        }

        // CalibrateTDR
        // This method performs TDR calibration from tester Pogo pins to the device contact.
        public bool CalibrateTDR()
        {
            // Implementation
            return true;
        }

        // ClearAllEnableWords
        // This method clears all enable words.
        public void ClearAllEnableWords()
        {
            // Implementation
        }

        // CurrentChanMap
        // This property gets or sets the current Channel Map.
        public string CurrentChanMap { get; set; }

        // CurrentEnv
        // This property gets or sets the current Environment Context.
        public string CurrentEnv { get; set; }

        // CurrentJob
        // This property gets or sets the current job.
        public string CurrentJob 
        {
            get => ConfigurationName;
            set
            {
                ConfigurationName = value;
            }
        }

        // CurrentPart
        // This property gets or sets the current Part Context.
        public string CurrentPart { get; set; }

        // EnableWord
        // This property gets or sets the state of an enable word.
        public bool EnableWord(string word)
        {
            // Implementation
            return true;
        }

        // ErrorCloseLogfile
        // This method closes the Run-Time error logfile.
        public void ErrorCloseLogfile()
        {
            // Implementation
        }

        // ErrorLogfileName
        // This property gets or sets the Error Logfile Filename.
        public string ErrorLogfileName { get; set; }

        // ErrorLogMessage
        // This method logs a Run-Time Error Message to be reported using ErrorReport.
        public void ErrorLogMessage(string message)
        {
            // Implementation
        }

        // ErrorMessageToLogfile
        // This method writes a Run-Time error message directly to the logfile.
        public void ErrorMessageToLogfile(string message)
        {
            // Implementation
        }

        // ErrorOutputMode
        // This property gets or sets the Error Output Mode.
        public int ErrorOutputMode { get; set; }

        // ErrorReport
        // This method reports a previously logged error.
        public void ErrorReport()
        {
            // Implementation
        }

        // ExcelHandle
        // Returns an Excel handle.
        public object ExcelHandle
        {
            get
            {
                // Implementation
                return null;
            }
        }

        // ExecutionCount
        // This property gets or sets the execution count.
        public long ExecutionCount { get; set; }

        // JobIsValid
        // This property returns whether the active job has been successfully validated.
        public bool JobIsValid
        {
            get
            {
                // Implementation
                return true;
            }
        }

        // Rootpath
        // This property gets the installation Root Path.
        public string Rootpath
        {
            get
            {
                // Implementation
                return "";
            }
        }

        // RunMode
        // This property gets or sets the system Run Mode.
        public int RunMode { get; set; }

        // RunTestProgram
        // This method executes the test program.
        public bool RunTestProgram()
        {
            // Implementation
            return true;
        }

        // SoftwareBuild
        // This property gets the IG-XL Software Build identifier.
        public string SoftwareBuild
        {
            get
            {
                // Implementation
                return "";
            }
        }

        // SoftwareVersion
        // This property gets the IG-XL Software Version.
        public string SoftwareVersion
        {
            get
            {
                // Implementation
                return "";
            }
        }

        // StartDataTool
        // This method starts IG-XL DataTool.
        public void StartDataTool()
        {
            // Implementation
        }

        // StopDataTool
        // This method stops IG-XL DataTool.
        public void StopDataTool()
        {
            // Implementation
        }

        // Timer
        // This method reads the time elapsed since the reference time.
        public double Timer(double refTime = 0)
        {
            // Implementation
            return 0.0;
        }

        // Validate
        // This method validates the active job.
        public void Validate()
        {
            // Implementation
        }

        // VariableValue
        // This property gets the value of a variable (spec) in the context of a test.
        public object VariableValue(string varName, string testName = null, long memberNum = -1)
        {
            // Implementation
            return null;
        }
    }

    public class Datalog
    {
        // These properties and methods control datalogging as a whole, as opposed to setting up datalogging. 
        // TheExec.Datalog.Property-or-Method

        // Implementation of Datalog methods and properties would go here.
        public void WriteComment(string CommentText)
        {
            // This method writes a comment to the datalog streams.
            // Usage: This method writes the comment to all datalog output windows and / or files that have been selected using the DataCollect Setup window. If an STDF file is specified, it writes a Datalog Text Record(DTR) to the file.
            // Implementation to write a CommentText to the datalog
        }
    }

    public class DataViewManager
    {
        // AllowInvalidSiteDoubleAssignment
        // This property suppresses the error that occurs when reading an array into a SiteDouble.
        public bool AllowInvalidSiteDoubleAssignment { get; set; }

        // ChannelType
        // This property gets the channel type for a given pin.
        public string ChannelType(string pinName)
        {
            // Implementation to get the channel type for the given pin
            return "chType";
        }

        // DecomposePinList
        // This method parses a list of comma-separated names of pins and pingroups and returns the names as elements of an array of strings.
        public void DecomposePinList(PinList pinList, out string[] pinNames, out int testPinsNum)
        {    
            pinNames = pinList.GetAll().ToArray();
            pinNames = Globals.tsmContext.GetPinsInPinGroups(pinNames);
            testPinsNum = pinNames.Length;
        }

        // EnableExtendedValidation
        // This property enables and disables validation of Compare timing edges.
        public bool EnableExtendedValidation { get; set; }

        // GetArgumentList
        // This method gets all argument values including those that are blank up to the last nonblank argument for the active test instance.
        public void GetArgumentList(out string[] arguments, out long numberArguments, long member = -1)
        {
            // Implementation to get argument list
            arguments = new string[] { };
            numberArguments = 0;
        }

        // GetChannelList
        // This method gets a list of channels by site and type.
        public int GetChannelList(string pinList, long site, long channelType, out string[] channels, out long numberChannels, out long numberSites, out string error)
        {
            // Implementation to get channel list
            channels = new string[] { };
            numberChannels = 0;
            numberSites = 0;
            error = "";
            return 0; // TL_SUCCESS
        }

        // GetChannelListByBoard
        // This method gets a list of channels by type and site sorted by board.
        public int GetChannelListByBoard(string pinList, long site, long channelType, out string[] channels, out long numberChannels, out long numberBoards, out long maxChannelsPerBoard, out string error)
        {
            // Implementation to get channel list by board
            channels = new string[] { };
            numberChannels = 0;
            numberBoards = 0;
            maxChannelsPerBoard = 0;
            error = "";
            return 0; // TL_SUCCESS
        }

        // GetChannelListForSelectedSites
        // This method gets a list of channels by type for active or selected sites.
        public int GetChannelListForSelectedSites(string pinList, long channelType, out string[] channels, out long numberChannels, out long numberSites, out string error)
        {
            // Implementation to get channel list for selected sites
            channels = new string[] { };
            numberChannels = 0;
            numberSites = 0;
            error = "";
            return 0; // TL_SUCCESS
        }

        // GetChannelStringFromPinAndSite
        // This method retrieves the channel string for a given pin name and site.
        public string GetChannelStringFromPinAndSite(string pin, long site)
        {
            // Implementation to get channel string from pin and site
            return "channelString";
        }

        // GetInstanceContext
        // This method gets the category names, selector names, and sheets for the currently executing instance.
        public void GetInstanceContext(out string dcCategory, out string dcSelector, out string acCategory, out string acSelector, out string timeSetSheet, out string edgeSetSheet, out string levelsSheet, out long overlay, long memberNumber = -1)
        {
            // Implementation to get instance context
            dcCategory = "";
            dcSelector = "";
            acCategory = "";
            acSelector = "";
            timeSetSheet = "";
            edgeSetSheet = "";
            levelsSheet = "";
            overlay = 1;
        }

        // GetJobContext
        // This method retrieves the name of the current job, part, and environment.
        public void GetJobContext(out string jobName, out string partName, out string environment)
        {
            // Implementation to get job context
            jobName = "jobName";
            partName = "partName";
            environment = "environment";
        }

        // GetMixedSignalContext
        // This method retrieves the Mixed Signal Context for the current test instance.
        public void GetMixedSignalContext(out string mixedSignalContextName, long memberNumber = -1)
        {
            // Implementation to get mixed signal context
            mixedSignalContextName = "mixedSignalContextName";
        }

        // GetPinSiteFromChannelString
        // This method gets the pin names and site numbers for a given channel.
        public void GetPinSiteFromChannelString(string channel, string chanType, out string retPinNames, out long[] retSiteNums)
        {
            // Implementation to get pin site from channel string
            retPinNames = "pinNames";
            retSiteNums = new long[] { };
        }

        // GetTestNumbers
        // This method gets a list of test numbers associated with flow table entries containing the given instance as the parameter of the test opcode.
        public void GetTestNumbers(string instance, out long[] testNumbers, out long numberTestNumbers)
        {
            // Implementation to get test numbers
            testNumbers = new long[] { };
            numberTestNumbers = 0;
        }

        // InstanceName
        // This property gets the currently active instance name.
        public string InstanceName
        {
            get
            {
                // Implementation to get instance name
                return "instanceName";
            }
        }

        // MemberIndex
        // This property gets the index of the currently active member.
        public long MemberIndex
        {
            get
            {
                // Implementation to get member index
                return 0;
            }
        }

        // PerformEdgeTimeValidation
        // This method specifies whether validation will check for edge timing.
        public void PerformEdgeTimeValidation(bool enable)
        {
            // Implementation to perform edge time validation
        }

        // PinMapHistory
        // This property retrieves the current history number of the active pin map.
        public long PinMapHistory
        {
            get
            {
                // Implementation to get pin map history
                return 0;
            }
        }

        // PinType
        // This property gets the Pin type for a given pin.
        public string PinType(string pinName)
        {
            // Implementation to get pin type
            return "pinType";
        }

        // ReloadInstance
        // This method reloads the arguments for a given instance.
        public void ReloadInstance(string instance, out string[] arguments, long memberNumber)
        {
            // Implementation to reload instance
            arguments = new string[] { };
        }

        // WriteTemplateArgumentError
        // This method writes a validation error message.
        public void WriteTemplateArgumentError(long argument, long messageNumber, string message, string prefix)
        {
            // Implementation to write template argument error
        }
    }

    public class Flow
    {
        //public PinList PinList { get; set; }

        //private static int accessCounter;
        // Static constructor to initialize the static field
        static Flow()
        {
            Globals.accessCounter = 0;
            //Globals.tsmContext.SetGlobalData("accessCounter", 0);
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
            string TName = null,//TName is converted separately to TestStand
            string compareMode = null,
            PinList PinNames = null,
            double? forceVal = null,
            string forceunit = null,
            string customUnit = null,
            string customForceunit = null,
            string ForceResults = null,
            [CallerMemberName] string callerName = null // Caller identifier
        )
        {
            double[][] pinData;
            // Check if resultval is not a 2D array
            if (!(resultval is double[,]))
            {
                pinData = GlobalFunctions.convertPinListDatato2DArray(resultval);
            }
            else
            {
                pinData = resultval;
            }

            if (PinNames != null)
            {
                string[] TestPinsArray = PinNames.GetAll().ToArray();
                string[] Pins = Globals.tsmContext.GetPinsInPinGroups(TestPinsArray);
                int i = 0;
                foreach (var Pin in Pins)
                {
                    Globals.tsmContext.PublishPerSite(pinData[i], Globals.tsmContext.GetGlobalData("publishID").ToString(), Pin);
                    i++;
                    Globals.tsmContext.SetGlobalData("publishID", (int)Globals.tsmContext.GetGlobalData("publishID") + 1);   //added -adrian
                }
            }
            else
            {
                foreach (var Data in pinData)
                {
                    Globals.tsmContext.PublishPerSite(Data, Globals.tsmContext.GetGlobalData("publishID").ToString());
                    Globals.tsmContext.SetGlobalData("publishID", (int)Globals.tsmContext.GetGlobalData("publishID") + 1);  //added -adrian
                }
            }

            // Increment the access counter
            //Globals.tsmContext.SetGlobalData("publishID", (int)Globals.tsmContext.GetGlobalData("publishID") + 1);    //moved -adrian
        }

        public long TestLimitIndex
        {
            //This property gets or sets the test limit index. Read/Write Long.
            //Use this property with the "Test Limits in Flow" feature.
            //It reads or writes the current test limit index, that is,
            //the number of the Use-Limit row (starting with 0) that is to be used by the next call to TestLimit.
            //You can set this value to use a specific Use-Limit row rather than simply the next one. 

            get
            {
                // Implementation to get the test limit index
                return 0;
            }
            set
            {
                // Implementation to set the test limit index
            }
        }

        public void CreateSiteVariable()
        {
            // NI TSM does not support this function.
            // For TSM, assigning a site variable will create the site variable.
        }

        public void AssignSiteVariable<T>(string name, T[] values)
        {
            Globals.tsmContext.SetSiteData(name, values);
        }

        public void IsSiteVariableCreated(string name, out bool result)
        {
            result = Globals.tsmContext.SiteDataExists(name);
        }

        public void GetSiteVariableValue<T>(long siteNum, string name, out T value)
        {
            int[] siteNumbers = Globals.tsmContext.SiteNumbers.ToArray();
            T[] values = Globals.tsmContext.GetSiteData<T>(name);

            foreach (var data in siteNumbers.Zip(values, (x, y) => new { siteNumber = x, value = y }))
            {
                if (data.siteNumber == siteNum)
                {
                    value = data.value;
                    return;
                }
            }
            value = default(T);
        }
    }

    public class Sites 
    {
        private List<int> _existing = new List<int>();
        private List<int> _starting = new List<int>();
        private List<int> _active = new List<int>();
        private List<int> _selected = new List<int>();

        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Pins { get; set; }

        public List<int> Existing
        {
            get => Globals.Existing;
            private set => _existing = Globals.Existing;
        }
        public List<int> Starting
        {
            get => Globals.Starting;
            private set => _starting = Globals.Starting;
        }
        public List<int> Active
        {
            get => Globals.Active;
            private set => _active = Globals.Active;
        }
        public List<int> Selected
        {
            get => _selected;
            set
            {
                _selected = new List<int>(value);
                Globals.tsmContext = Globals.tsmContext.GetSemiconductorModuleContextWithSites(_selected.ToArray());
            }
        }
        //private  List<bool> _existing = new List<bool>();
        //private  List<bool> _starting = new List<bool>();
        //private  List<bool> _active = new List<bool>();
        //private  List<bool> _selected = new List<bool>();

        //public string Name { get; set; }
        //public string Description { get; set; }
        //public string[] Pins { get; set; }

        //public List<bool> Existing
        //{
        //    get => Globals.Existing;
        //    private set => _existing = Globals.Existing;
        //}
        //public List<bool> Starting
        //{
        //    get => Globals.Starting;
        //    private set => _starting = Globals.Starting;
        //}
        //public List<bool> Active
        //{
        //    get => Globals.Active;
        //    private set => _active = Globals.Active;
        //}
        //public List<bool> Selected
        //{
        //    get => _selected;
        //    set
        //    {
        //        _selected = new List<bool>(value);
        //        Globals.tsmContext = Globals.tsmContext.GetSemiconductorModuleContextWithSites(GlobalFunctions.ConvertToIntegerList(_selected).ToArray());
        //    }
        //}
    }

}
