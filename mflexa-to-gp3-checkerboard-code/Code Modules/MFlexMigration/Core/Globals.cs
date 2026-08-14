using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Numerics;
using System.IO;
using System.Threading.Tasks;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public static class Globals
    {
        public static ISemiconductorModuleContext tsmContext;
        public static SequenceContext seqContext;

        public static TheHdw TheHdw = new TheHdw();
        public static TheExec TheExec = new TheExec();
        public static IPATManager ipat = new IPATManager();
        public static int accessCounter = 0;

        //public static List<bool> Existing;
        //public static List<bool> Starting;
        //public static List<bool> Active;
        //public static List<bool> Selected;
        public static List<int> Existing;
        public static List<int> Starting;
        public static List<int> Active;
        public static List<int> Selected;

        public static double mA = 0.001;
        public static double uA = 0.000001;
        public static double nA = 0.000000001;

        public static double mV = 0.001;
        public static double uV = 0.000001;
        public static double nV = 0.000000001;

        public static double mS = 0.001;
        public static double uS = 0.000001;
        public static double nS = 0.000000001;
        public static double V = 1;
        public static string scaleMilli = "scaleMilli";
        public static string scaleMicro = "scaleMicro";
        public static string scaleNano = "scaleNano";
        public static string unitAmp = "unitAmp";
        public static string unitTime = "unitTime";
        public static int captureSampleSize = 0;

        public static string[] strbNames = { "strb0", "strb1", "strb2", "strb3", "strb4", "strb5", "strb6", "strb7", "strb8", "strb9", "strb10", "strb11", "strb12", "strb13", "strb14", "strb15", "strb16", "strb17", "strb18", "strb19" };

        public const int tlDCVIModeVoltage = DCVIMode.tlDCVIModeVoltage;
        public const int tlDCVIModeCurrent = DCVIMode.tlDCVIModeCurrent;
        public const int tlSignalModeUseValue = 0;
        public const int tlSignalModeUseLoadedValue = 1;
        public const int tlCSignalModeUseCalculatedValue = 2;
        public static int tlDCVIMeterVoltage = DCVIMeterMode.tlDCVIMeterVoltage;
        public static int tlDCVIMeterCurrent = DCVIMeterMode.tlDCVIMeterCurrent;
        public static int tlDCVIComplianceBoth = tlDCVICompliance.Both;
        public static int tlDCVICompliancePositive = tlDCVICompliance.Positive;
        public static int tlDCVIComplianceNegative = tlDCVICompliance.Negative;
        public static tlDCVILocalKelvin tlDCVILocalKelvinBoth = tlDCVILocalKelvin.Both;
        public static tlDCVILocalKelvin tlDCVILocalKelvinHigh = tlDCVILocalKelvin.High;
        public static tlDCVILocalKelvin tlDCVILocalKelvinLow = tlDCVILocalKelvin.Low;
        public static tlStrobeOption tlStrobe = tlStrobeOption.tlStrobe;
        public static tlStrobeOption tlNoStrobe = tlStrobeOption.tlNoStrobe;
        public static tlDCVIMeterReadingFormat tlDCVIMeterReadingFormatAverage = tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatAverage;
        public static tlDCVIMeterReadingFormat tlDCVIMeterReadingFormatArray = tlDCVIMeterReadingFormat.tlDCVIMeterReadingFormatArray;
        public static int tlDCVIConnectNone = tlDCVIConnectWhat.tlDCVIConnectNone;
        public static int tlDCVIConnectHighForce = tlDCVIConnectWhat.tlDCVIConnectHighForce;
        public static int tlDCVIConnectHighSense = tlDCVIConnectWhat.tlDCVIConnectHighSense;
        public static int tlDCVIConnectHighGuard = tlDCVIConnectWhat.tlDCVIConnectHighGuard;
        public static int tlDCVIConnectLowForce = tlDCVIConnectWhat.tlDCVIConnectLowForce;
        public static int tlDCVIConnectLowSense = tlDCVIConnectWhat.tlDCVIConnectLowSense;
        public static int tlDCVIConnectDefault = tlDCVIConnectWhat.tlDCVIConnectDefault;

        public static tlPPMUReadWhat tlPPMUReadMeasurements = tlPPMUReadWhat.tlPPMUReadMeasurements;
        public static tlPPMUReadWhat tlPPMUReadPassFailResults = tlPPMUReadWhat.tlPPMUReadPassFailResults;

        public static string scaleNoScaling = "scaleNoScaling", unitVolt = "unitVolt", tlForceFlow = "tlForceFlow";
        //public static TlRelayMode tlPowered = new TlRelayMode();
        public static TlRelayMode tlPowered = TlRelayMode.tlPowered; //corrected -adrian

        public static ChPinLevel chVil = ChPinLevel.chVil;
        public static ChPinLevel chVih = ChPinLevel.chVih;
        public static TimeSetEdge chEdgeD0 = TimeSetEdge.DriveOn;//chEdgeD0: Drive On 
        public static TimeSetEdge chEdgeD1 = TimeSetEdge.DriveData;//chEdgeD1: Drive Data 
        public static TimeSetEdge chEdgeD2 = TimeSetEdge.DriveReturn;//chEdgeD2: Drive Return
        public static TimeSetEdge chEdgeD3 = TimeSetEdge.DriveOff;//chEdgeD3: Drive Off 
        public static TimeSetEdge chEdgeR0 = TimeSetEdge.CompareStrobe;//chEdgeR0: Compare Open --> No Direct Equivalent
        public static TimeSetEdge chEdgeR1 = TimeSetEdge.CompareStrobe2;//chEdgeR1: Compare Close --> No Direct Equivalent

        public static pfType pfAlways = pfType.pfAlways;
        public static pfType pfFailsOnly = pfType.pfFailsOnly;
        public static pfType pfNever = pfType.pfNever;

        // Not able to find the actual value (cpuA, cpuB, cpuC, cpuD) from IGXL help file. 
        // These values are assumed based on understanding of code.

        public static int cpuA = 1; // 2^0
        public static int cpuB = 2; // 2^1
        public static int cpuC = 4; // 2^2
        public static int cpuD = 8; // 2^3

        public static int chStaticStateDisable = DriveState.chStaticStateDisable;
        public static int chStaticStateHi = DriveState.chStaticStateHi;
        public static int chStaticStateHiZ = DriveState.chStaticStateHiZ;
        public static int chStaticStateLo = DriveState.chStaticStateLo;

        public static tlUtilityAlarm tlUtilityAlarmUb = tlUtilityAlarm.tlUtilityAlarmUb;
        public static tlAlarmBehavior tlAlarmForceFail = tlAlarmBehavior.tlAlarmForceFail;
        public static tlAlarmBehavior tlAlarmForceBin = tlAlarmBehavior.tlAlarmForceBin;
        public static tlAlarmBehavior tlAlarmOff = tlAlarmBehavior.tlAlarmOff;
        public static tlAlarmBehavior tlAlarmDefault = tlAlarmBehavior.tlAlarmDefault;
        public static tlAlarmBehavior tlAlarmContinue = tlAlarmBehavior.tlAlarmContinue;
        public static tlUtilBitState tlUtilBitOff = tlUtilBitState.tlUtilBitOff;
        public static tlUtilBitState tlUtilBitOn = tlUtilBitState.tlUtilBitOn;
        public static tlUBState tlUBStateProgrammed = tlUBState.tlUBStateProgrammed;
        public static tlUBState tlUBStateCompared = tlUBState.tlUBStateCompared;

        public static SubSpecs DCSpecs = new SubSpecs();
        public static SubSpecs ACSpecs = new SubSpecs();
        public static SubSpecs Environment = new SubSpecs();
        public static string levelSheetName = "ADuM225_lvl_dc_levels", timingSheetName = "ADuM225_master_timeset";
        public static string[] TNames = null;
        public static string edgeSetSheetName = null, MixedSignalTiming = null;
        public static string LevelsTimingsPins = null;
        public static string AllDigitalPins = "Digital";
        public static string AllScopePins = "Scope";

        public static Dictionary<string, string[,]> PinGroupMap = new Dictionary<string, string[,]>();

        public static double VDD1;
        public static double VDD1_alt;
        public static double VDD2;
        public static double VDD2_alt;
        public static double VDD1_vih;
        public static double VDD1_vil;
        public static double VDD1_voh;
        public static double VDD1_vol;
        public static double VDD1_ioh_20;
        public static double VDD1_iol_20;
        public static double VDD1_ioh_4000;
        public static double VDD1_iol_4000;
        public static double VDD1_vt;
        public static double VDD2_vih;
        public static double VDD2_vil;
        public static double VDD2_voh;
        public static double VDD2_vol;
        public static double VDD2_ioh_20;
        public static double VDD2_iol_20;
        public static double VDD2_ioh_4000;
        public static double VDD2_iol_4000;
        public static double VDD2_vt;
        public static double idd_VDD1;
        public static double idd_VDD2;

        public static double period;
        public static double TpdPos1RuStepSize;
        public static double output_strobe;
        public static double ph1_strobe;
        public static double ph2_strobe;
        public static double ph3_strobe;
        public static double ph4_strobe;
        public static double mux_ph1_strobe;
        public static double mux_ph2_strobe;
        public static double mux_ph3_strobe;
        public static double mux_ph4_strobe;

        public static TimeSpan TimeOut = TimeSpan.FromSeconds(10);//Patternburst timeout

        //public static string[] Pin_Group = null;
        public static string[,] GetPinArray = null;

        public static string[,] VIA_mux = null;
        public static string[,] VIB_mux = null;
        public static string[,] VOA_mux = null;
        public static string[,] VOB_mux = null;


        //OriginalCode: Public FIRST_RUN_ONLY As Boolean;
        //PseudoCode  : GlobalVariableDeclaration: Name: FIRST_RUN_ONLY; Type: Boolean;
        public static bool FIRST_RUN_ONLY;

        //OriginalCode: Public Const HKEY_CLASSES_ROOT = &H80000000;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CLASSES_ROOT; Value: 0x80000000;
        public const long HKEY_CLASSES_ROOT = 0x80000000;

        //OriginalCode: Public Const HKEY_CURRENT_CONFIG = &H80000005;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CURRENT_CONFIG; Value: 0x80000005;
        public const long HKEY_CURRENT_CONFIG = 0x80000005;

        //OriginalCode: Public Const HKEY_CURRENT_USER = &H80000001;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CURRENT_USER; Value: 0x80000001;
        public const long HKEY_CURRENT_USER = 0x80000001;

        //OriginalCode: Public Const HKEY_DYN_DATA = &H80000006;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_DYN_DATA; Value: 0x80000006;
        public const long HKEY_DYN_DATA = 0x80000006;

        //OriginalCode: Public Const HKEY_LOCAL_MACHINE = &H80000002;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_LOCAL_MACHINE; Value: 0x80000002;
        public const long HKEY_LOCAL_MACHINE = 0x80000002;

        //OriginalCode: Public Const HKEY_PERFORMANCE_DATA = &H80000004;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_PERFORMANCE_DATA; Value: 0x80000004;
        public const long HKEY_PERFORMANCE_DATA = 0x80000004;

        //OriginalCode: Public Const HKEY_USERS = &H80000003;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_USERS; Value: 0x80000003;
        public const long HKEY_USERS = 0x80000003;

        //OriginalCode: Public Const ERROR_SUCCESS As Long = 0&;
        //PseudoCode  : GlobalConstantDeclaration: Name: ERROR_SUCCESS As Long; Value: 0&;
        public const double ERROR_SUCCESS = 0.0;

        //OriginalCode: Public ipat As IPATManager;
        //PseudoCode  : GlobalVariableDeclaration: Name: ipat; Type: IPATManager;
        //public static IPATManager ipat;

        //OriginalCode: Public Const pF = 0.000000000001;
        //PseudoCode  : GlobalConstantDeclaration: Name: pF; Value: 0.000000000001;
        public const double pF = 0.000000000001;

        //OriginalCode: Public PropDelayRise() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: PropDelayRise(); Type: New SiteDouble;
        public static SiteDouble[] PropDelayRise;

        //OriginalCode: Public PropDelayFall() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: PropDelayFall(); Type: New SiteDouble;
        public static SiteDouble[] PropDelayFall;

        //OriginalCode: Public RiseCal() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: RiseCal(); Type: New SiteDouble;
        public static SiteDouble[] RiseCal;

        //OriginalCode: Public FallCal() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: FallCal(); Type: New SiteDouble;
        public static SiteDouble[] FallCal;

        //OriginalCode: Public HvstPreCurrent() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: HvstPreCurrent(); Type: New SiteDouble;
        public static SiteDouble[] HvstPreCurrent;

        //OriginalCode: Public HvstPostCurrent() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: HvstPostCurrent(); Type: New SiteDouble;
        public static SiteDouble[] HvstPostCurrent;

        //OriginalCode: Public HvstRatio() As New SiteDouble;
        //PseudoCode  : GlobalVariableDeclaration: Name: HvstRatio(); Type: New SiteDouble;
        public static SiteDouble[] HvstRatio;

        //OriginalCode: Public Const HKEY_CLASSES_ROOT = &H80000000;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CLASSES_ROOT; Value: 0x80000000;
        //public const long HKEY_CLASSES_ROOT = 0x80000000;

        //OriginalCode: Public Const HKEY_CURRENT_CONFIG = &H80000005;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CURRENT_CONFIG; Value: 0x80000005;
        //public const long HKEY_CURRENT_CONFIG = 0x80000005;

        //OriginalCode: Public Const HKEY_CURRENT_USER = &H80000001;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_CURRENT_USER; Value: 0x80000001;
        //public const long HKEY_CURRENT_USER = 0x80000001;

        //OriginalCode: Public Const HKEY_DYN_DATA = &H80000006;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_DYN_DATA; Value: 0x80000006;
        //public const long HKEY_DYN_DATA = 0x80000006;

        //OriginalCode: Public Const HKEY_LOCAL_MACHINE = &H80000002;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_LOCAL_MACHINE; Value: 0x80000002;
        //public const long HKEY_LOCAL_MACHINE = 0x80000002;

        //OriginalCode: Public Const HKEY_PERFORMANCE_DATA = &H80000004;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_PERFORMANCE_DATA; Value: 0x80000004;
        //public const long HKEY_PERFORMANCE_DATA = 0x80000004;

        //OriginalCode: Public Const HKEY_USERS = &H80000003;
        //PseudoCode  : GlobalConstantDeclaration: Name: HKEY_USERS; Value: 0x80000003;
        //public const long HKEY_USERS = 0x80000003;

        //OriginalCode: Public Const ERROR_SUCCESS As Long = 0&;
        //PseudoCode  : GlobalConstantDeclaration: Name: ERROR_SUCCESS As Long; Value: 0&;
        //public const double ERROR_SUCCESS = 0.0;

        //add for meter mode  -adrian
        public static int VDD1MeterMode;
        public static int VDD2MeterMode;
        public static int VOAdc30MeterMode;
        public static int VOBdc30MeterMode;

        //HMOD globals -adrian
        public static uint HMOD_Data_1;
        public static uint HMOD_Data_2;
        public static uint HMOD_Data_3;
        public static uint HMOD_Data_4;
        public static uint HMOD_Data_5;
        public static uint HMOD_Data_6;
        public static uint HMOD_Data_7;
        public static uint HMOD_Data_8;
        public static uint HMOD_Data_9;
        public static uint HMOD_Data_10;
        public static uint HMOD_Data_11;
        public static uint HMOD_Data_12;
        public static uint HMOD_Data_13;
        public static uint HMOD_Data_14;
        public static uint HMOD_Data_15;
        public static uint HMOD_Data_16;
        public static uint HMOD_Data_17;
        public static uint HMOD_Data_18;
        public static uint HMOD_Data_19;
        public static uint HMOD_Data_20;
        public static uint HMOD_Data_21;
        public static uint HMOD_Data_22;
        public static uint HMOD_Data_23;

        //TDR Globals -adrian
        public static Ivi.Driver.PrecisionTimeSpan[][] tdrvalues;
        /*
        #if TestTimeMeasure
                /// <summary>
                /// Benchmarking code using stopwatch.
                /// </summary>
                public class TestTimeMeasure
                {
                    private static Stopwatch stopwatch;
                    private static List<TestTimeResult> results = new List<TestTimeResult>();
                    private static List<TestTimeResult> resultsOld = new List<TestTimeResult>();
                    private static string filePath;
                    private static string delimiter;

                    private struct TestTimeResult
                    {
                        public string TestName;
                        public string LibraryName;
                        public long ElapsedTimeMs;
                        public long Index;

                        /// <summary>
                        /// Assign value and default to Test Time Reult when create.
                        /// </summary>
                        /// <param name="testName"></param>
                        /// <param name="libraryName"></param>
                        /// <param name="ElapsedTimeMs"></param>
                        /// <param name="Index"></param>
                        public TestTimeResult(string testName, string libraryName = "", long index = -1, long elapsedTimeMs = -1)
                        {
                            TestName = testName;
                            LibraryName = libraryName;
                            ElapsedTimeMs = elapsedTimeMs;
                            Index = index;
                        }
                    }

                    public static void TestTimeStart()
                    {
                        // Get testName and libraryName
                        StackTrace trace = new StackTrace();
                        StackFrame testMethod = trace.GetFrame(2); // Get method who call the function 2 step
                        StackFrame libraryMethod = trace.GetFrame(1); // Get method who call this function;
                        string testName = testMethod.GetMethod().DeclaringType.FullName + "." + testMethod.GetMethod().Name;
                        string libraryName = libraryMethod.GetMethod().DeclaringType.FullName + "." + libraryMethod.GetMethod().Name;

                        // Get index
                        int index = results.Count(results => results.LibraryName == libraryName) + resultsOld.Count(results => results.LibraryName == libraryName) + 1;

                        // Pre-record data to results
                        results.Add(new TestTimeResult(testName, libraryName, index));

                        // Start stopwatch at the end of method
                        stopwatch = Stopwatch.StartNew();
                    }

                    public static long TestTimeStop()
                    {
                        // Get stopwatch data at the beginning of method
                        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        stopwatch.Stop();

                        int resultIndex = results.Count - 1;

                        if (resultIndex < 0)
                        {
                            throw new ArgumentException("Number of result in TestTimeMeasure Error. Make sure TestTimeStart() method is called before TestTimeStop() method");
                        }

                        TestTimeResult lastResult = results[resultIndex];
                        lastResult.ElapsedTimeMs = elapsedMilliseconds;
                        results[resultIndex] = lastResult;

                        return elapsedMilliseconds;
                    }

                    /// <summary>
                    /// Create Test Time Result file at C:\data folder.
                    /// </summary>
                    public static void CreateTestTimeResult(string delimiter = ",", string fileExtension = ".csv")
                    {
                        string folderPath = "C:\\data";
                        //string fileName = System.IO.Path.GetFileNameWithoutExtension(Globals.seqContext.SequenceFile.Path) + "_Test Time Result Log_Site" + Globals.seqContext.AsPropertyObject().GetValNumber("RunState.TestSockets.MyIndex", 0).ToString() +"_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileExtension;
                        string fileName = "Functions_Profiler.txt";
                        filePath = Path.Combine(folderPath, fileName);
                        Globals.TestTimeMeasure.delimiter = delimiter;

                        using (StreamWriter writer = new StreamWriter(filePath, append: true))
                        {
                            writer.WriteLine("Test Name" + delimiter + "Library Name" + delimiter + "Elapsed Time (ms)" + delimiter + "Index");
                        }

                        // Clear all data
                        results.Clear();
                        resultsOld.Clear();
                    }

                    /// <summary>
                    /// Write Test Time Result data to the file which created from CreateTestTimeResult().
                    /// </summary>
                    public static void ExportTestTimeResult()
                    {
                        if (filePath == null)
                        {
                            throw new ArgumentException("Please called CreateTestTimeResult() method first");
                        }

                        // write data to file
                        using (StreamWriter writer = new StreamWriter(filePath, append: true))
                        {
                            string str;
                            foreach (var result in results)
                            {
                                str = result.TestName + delimiter + result.LibraryName + delimiter + result.ElapsedTimeMs.ToString() + delimiter + result.Index.ToString();
                                writer.WriteLine(str);
                            }
                        } // file close automatically here

                        resultsOld.AddRange(results);
                        results.Clear();
                    }
                }
        #endif
        */

#if TestTimeMeasure
        public class TestTimeMeasure
        {
            private static Stopwatch stopwatch;
            private static List<TestTimeResult> results = new List<TestTimeResult>();
            private static List<TestTimeResult> resultsOld = new List<TestTimeResult>();
            private static string filePath;
            private static string delimiter;

            private struct TestTimeResult
            {
                public string TestName;
                public string LibraryName;
                public long ElapsedTimeUs;
                public long Index;

                /// <summary>
                /// Assign value and default to Test Time Reult when create.
                /// </summary>
                /// <param name="testName"></param>
                /// <param name="libraryName"></param>
                /// <param name="ElapsedTimeMs"></param>
                /// <param name="Index"></param>
                public TestTimeResult(string testName, string libraryName = "", long index = -1, long elapsedTimeMs = -1)
                {
                    TestName = testName;
                    LibraryName = libraryName;
                    ElapsedTimeUs = elapsedTimeMs;
                    Index = index;
                }
            }

            public static void TestTimeStart()
            {
                // Get testName and libraryName
                StackTrace trace = new StackTrace();
                StackFrame testMethod = trace.GetFrame(2); // Get method who call the function 2 step
                StackFrame libraryMethod = trace.GetFrame(1); // Get method who call this function;
                string testName = testMethod.GetMethod().DeclaringType.FullName + "." + testMethod.GetMethod().Name;
                string libraryName = libraryMethod.GetMethod().DeclaringType.FullName + "." + libraryMethod.GetMethod().Name;

                // Get index
                int index = results.Count(results => results.LibraryName == libraryName) + resultsOld.Count(results => results.LibraryName == libraryName) + 1;

                // Pre-record data to results
                results.Add(new TestTimeResult(testName, libraryName, index));

                // Start stopwatch at the end of method
                stopwatch = Stopwatch.StartNew();
            }

            public static long TestTimeStop()
            {
                // Get stopwatch data at the beginning of method
                long elapsedTicks = stopwatch.ElapsedTicks;
                stopwatch.Stop();

                long elapsedMicroseconds = (elapsedTicks * 1000000) / Stopwatch.Frequency;

                int resultIndex = results.Count - 1;

                if (resultIndex < 0)
                {
                    throw new ArgumentException("Number of result in TestTimeMeasure Error. Make sure TestTimeStart() method is called before TestTimeStop() method");
                }

                TestTimeResult lastResult = results[resultIndex];
                lastResult.ElapsedTimeUs = elapsedMicroseconds;
                results[resultIndex] = lastResult;

                return elapsedMicroseconds;
            }

            /// <summary>
            /// Create Test Time Result file at C:\data folder.
            /// </summary>
            public static void CreateTestTimeResult(string delimiter = ",", string fileExtension = ".csv")
            {
                string folderPath = "C:\\data";
                //string fileName = System.IO.Path.GetFileNameWithoutExtension(Globals.seqContext.SequenceFile.Path) + "_Test Time Result Log_Site" + Globals.seqContext.AsPropertyObject().GetValNumber("RunState.TestSockets.MyIndex", 0).ToString() +"_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + fileExtension;
                string fileName = "Functions_Profiler.txt";
                filePath = Path.Combine(folderPath, fileName);
                Globals.TestTimeMeasure.delimiter = delimiter;

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine("Test Name" + delimiter + "Library Name" + delimiter + "Elapsed Time (us)" + delimiter + "Index");
                }

                // Clear all data
                results.Clear();
                resultsOld.Clear();
            }

            /// <summary>
            /// Write Test Time Result data to the file which created from CreateTestTimeResult().
            /// </summary>
            public static void ExportTestTimeResult()
            {
                if (filePath == null)
                {
                    throw new ArgumentException("Please called CreateTestTimeResult() method first");
                }

                // write data to file
                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    string str;
                    foreach (var result in results)
                    {
                        str = result.TestName + delimiter + result.LibraryName + delimiter + result.ElapsedTimeUs.ToString() + delimiter + result.Index.ToString();
                        writer.WriteLine(str);
                    }
                } // file close automatically here

                resultsOld.AddRange(results);
                results.Clear();
            }
        }
#endif
    }

}

