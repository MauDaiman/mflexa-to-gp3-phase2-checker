using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using System.Net.NetworkInformation;
using NationalInstruments.DAQmx;


namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public class TheHdw
    {
        // Properties and Methods
        public void ChanFromPinSite(string pinName, int siteNum, string chanType) { /* Implementation */ }
        public bool DGSConnected { get; set; }
        public object Instrument(string instName) { /* Implementation */ return null; }
        public void PinSiteFromChan(int chanNum, string chanType, out string retPinName, out int retSiteNum) { /* Implementation */ retPinName = ""; retSiteNum = 0; }
        public void ReadStopwatch() { /* Implementation */ }
        public void SetSettlingTimer(int timeVal) { /* Implementation */ }
        public void SettleWait(int timeout) { /* Implementation */ }
        public void StartStopwatch() { /* Implementation */ }
        public void Wait(double timeInSeconds)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            //int milliseconds = (int)(timeInSeconds * 1000);
            //System.Threading.Thread.Sleep(milliseconds);
            // Thread.Sleep() has a resolution over 10ms, so use Stopwatch to support shorter settling times.
            if (timeInSeconds > 0.0)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                double frequency = Stopwatch.Frequency;
                while (true)
                {
                    double elapsedSeconds = stopwatch.ElapsedTicks / frequency;
                    //if (elapsedSeconds < timeInSeconds)
                    if (elapsedSeconds > timeInSeconds) //corrected -adrian
                    {
                        break;
                    }
                }
            }
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        // Objects
        public DCVI DCVI { get; private set; }
        public PPMU PPMU { get; private set; }
        public Digital Digital { get; set; }
        public _Pins Pins { get; private set; }
        public PinLevels PinLevels { get; set; }
        public Utility Utility { get; set; }

        public TheHdw()
        {
            DCVI = new DCVI();
            PPMU = new PPMU();
            Pins = new _Pins(this);
            Digital = new Digital();
            PinLevels = new PinLevels();
            Utility = new Utility();
        }
    }

    public class DCVI
    {
        public bool ComplianceHeadroomEnabled { get; set; }
        public bool EnhancedModeEnabled { get; set; }
        public bool PatternInstrumentUnlock { get; set; }
        public bool PatternRestartOptimizationEnabled { get; set; }
        public bool PowerSupplyModeResetEnabled { get; set; }

        public PinController Pins(string pinName)
        {
            var pinController = new PinController();
            PinList singlePinList = new PinList();
            // Convert the comma-delimited input string to a string array
            string[] pinNamesArray = pinName.Split(',');
            string[] PinGroupsArray = Globals.tsmContext.GetPinsInPinGroups(pinNamesArray);
            singlePinList = PinList.FromStringArray(PinGroupsArray);
            pinController.PinList = singlePinList;
            pinController.Meter.PinList = singlePinList; // Initialize Meter's PinList
            return pinController;
        }

        public PinController Pins(PinList pinList)
        {
            string[] TestPinsArray = pinList.GetAll().ToArray();
            string[] PinGroupsArray = Globals.tsmContext.GetPinsInPinGroups(TestPinsArray);
            pinList = PinList.FromStringArray(PinGroupsArray);
            var pinController = new PinController();
            pinController.PinList = pinList;
            pinController.Meter.PinList = pinList; // Initialize Meter's PinList
            return pinController;
        }

        // Indexer
        public PinController this[string pinName]
        {
            get { return Pins(pinName); }
        }

        public class PinController
        {
            public bool _gate;
            public int _mode;
            public PinList PinList { get; set; }

            // Additional methods from PinListData integrated here
            public void AddPin(string pinName)
            {
                PinList.Add(pinName);
            }

            public PinController()
            {
                ComplianceRange = new Dictionary<int, int>();
                AsynchronousTrigger = new AsynchronousTrigger();
                Capture = new Capture();
                Meter = new Meter();
                PSets = new PSets();
                Source = new Source(PinList);
                PinList = new PinList(); // Initialize PinList
                Meter.PinList = PinList; // Initialize Meter's PinList
                NominalBandwidth.PinList = PinList; // Initialize NominalBandwidth's PinList          
            }

            // Properties and Methods
            public bool Alarm { get; set; }
            //NIDCPower don't have the equivalent functionality as DCVI alarm it is more of an error code which it will throw once it is being encounter
            //Getting of alarm also is being set in teststand
            //hence this method will be left blank under the hood

            public void AlarmClear() { /* Implementation */ }
            //NIDCPower don't have the equivalent functionality as DCVI alarm it is more of an error code which it will throw once it is being encounter
            //Getting of alarm also is being set in teststand
            //hence this method will be left blank under the hood
            public bool AlarmLatching { get; set; }
            public void ApplyMixedSignalTiming() { /* Implementation */ }
            public void ClearCaptureMemory() { /* Implementation */ }

            //ComplianceRange property with implementation for different tlDCVICompliance values
            public Dictionary<int, int> ComplianceRange { get; set; }
            public int this[int complianceType]
            {
                get
                {
                    if (ComplianceRange.ContainsKey(complianceType))
                    {
                        return ComplianceRange[complianceType];
                    }
                    throw new KeyNotFoundException("Compliance type not found.");
                }
                set
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    string[] Pins = PinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    //Parallel.ForEach(PinList, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        Parallel.For(0, dcPowerSessions.Length, i =>
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            switch (complianceType)
                            {
                                case tlDCVICompliance.Positive:
                                    output.Source.Current.VoltageLimitHigh = value;
                                    break;
                                case tlDCVICompliance.Negative:
                                    output.Source.Current.VoltageLimitLow = value;
                                    break;
                                case tlDCVICompliance.Both:
                                    output.Source.Current.VoltageLimitRange = value;
                                    break;
                                default:
                                    throw new ArgumentException("Invalid compliance type.");
                            }
                        });
                    });
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }
            public int ConditionBit { get; set; }
            public void Connect(int connectionType)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                switch (connectionType)
                {
                    case 0:
                        //code here
                        break;
                    case 1:
                        //code here
                        break;
                    case 2:
                        //code here
                        break;
                }
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    NIDCPower[] dcPowerSessions;
                    string[] dcPowerChannelStrings;
                    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    var output = new DCPowerOutput[dcPowerSessions.Length];
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        output[i] = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        output[i].Source.Output.Connected = true;
                    });
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public bool Connected { get; set; }
            public void ConnectToSyncPanel() { /* Implementation */ }
            public string CrossOverType { get; set; }
            public double Current // This property gets or sets the current for the DCVI. Type Double. Use this property to get or set the DCVI's current. 
            {
                get //return the existing current level being set
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            return output.Source.Current.CurrentLevel;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }
                set //change the existing current limit for the pin specified
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    Parallel.ForEach(Pins, (Pin) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            output.Source.Current.CurrentLevel = value;
                        }
                    });
                }
            }
            public string CurrentLimitMode { get; set; }
            public class CurrentRange
            {
                public PinList PinList { get; set; }
                PinList singlePinList = new PinList();

                //Sets or gets autoranging for the current range. If you turn autoranging off, you must program a range. Type Boolean.
                //When autoranging is on, IG-XL adjusts the range to the lowest range that is higher than the programmed value. True: Autoranging on
                public bool Autorange
                {
                    get //return if the SMU is set to autorange or not
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        foreach (var Pin in Pins)
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                if (output.Source.Current.CurrentLevelAutorange == DCPowerSourceCurrentLevelAutorange.On)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                    }
                    set //change the autorange to on or off
                    {

                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        Parallel.ForEach(Pins, (Pin) =>
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                output.Control.Abort();
                                if (value == true)
                                {
                                    output.Source.Current.CurrentLevelAutorange = DCPowerSourceCurrentLevelAutorange.On;
                                }
                                else
                                {
                                    output.Source.Current.CurrentLevelAutorange = DCPowerSourceCurrentLevelAutorange.Off;
                                }
                                output.Control.Initiate();
                            }
                        });
                    }
                }

                public double[] List { get; set; } // there is no direct way to get the list of the current ranges of SMU and this property is not commonly use
                public double Max { get; } //there is no direct way to get the max of the current ranges of SMU for a property that is forcing a current and this property is not commonly use
                public double Min { get; } //there is no direct way to get the max of the current ranges of SMU for a property that is forcing a current and this property is not commonly use
                public double Value
                {
                    get // return the existing current level being set
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        foreach (var Pin in Pins)
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                return output.Source.Current.CurrentLevel;
                            }
                        }
                        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                    }
                    set // change the existing current limit for the pin specified
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        Parallel.ForEach(Pins, (Pin) =>
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                output.Control.Abort();
                                output.Source.Current.CurrentLevel = value;
                                output.Control.Initiate();
                            }
                        });
                    }
                }
            }
            public string DCVIType { get; set; }
            public void Disconnect(int connectionType)
            {
                switch (connectionType)
                {
                    case 0:
                        //code here
                        break;
                    case 1:
                        //code here
                        break;
                    case 2:
                        //code here
                        break;
                    default:
                        break;
                }
                string[] Pins = PinList.GetAll().ToArray();
                if (Pins.Length == 0)
                {
                    throw new InvalidOperationException("Invalid pin: No pins found.");
                }

                Parallel.ForEach(Pins, (Pin) =>
                {
                    NIDCPower[] dcPowerSessions;
                    string[] dcPowerChannelStrings;
                    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    //for (int i = 0; i < dcPowerSessions.Length; i++)
                    //{
                    //    var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                    //    //output.Control.Abort();
                    //    output.Source.Output.Connected = false;
                    //    //output.Control.Initiate();
                    //}
                    var output = new DCPowerOutput[dcPowerSessions.Length];
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        output[i] = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        //output.Control.Abort();
                        output[i].Source.Output.Connected = false;
                        //output.Control.Initiate();
                    });
                });

            }
            public string ExternalModulationInput { get; set; }
            public bool Gate
            {
                get //return the current gate or enabled settings of SMU
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            return output.Source.Output.Enabled;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }
                set
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    _gate = value;
                    string[] Pins = PinList.GetAll().ToArray();
                    //Parallel.ForEach(Pins, (Pin, state, index) =>
                    //{
                    //    NIDCPower[] dcPowerSessions;
                    //    string[] dcPowerChannelStrings;
                    //    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    //    Parallel.For(0, dcPowerSessions.Length, i =>
                    //    {
                    //        var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                    //        output.Control.Abort();
                    //        output.Source.Output.Enabled = _gate;
                    //        output.Control.Initiate();
                    //    });
                    //});
                    Globals.tsmContext.GetNIDCPowerSessions(Pins, out NIDCPower[] dcPowerSessions, out string[] dcPowerChannelStrings);
                    var output = new DCPowerOutput[dcPowerSessions.Length];
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        output[i] = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        //output[i].Control.Abort();
                        output[i].Source.Output.Enabled = _gate;
                        //output[i].Control.Initiate();
                    });
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }
            public bool LimiterEnabled { get; set; }

            public Dictionary<tlDCVILocalKelvin, bool> LocalKelvin { get; set; }

            public bool this[tlDCVILocalKelvin which]
            {
                get
                {
                    if (LocalKelvin.ContainsKey(which))
                    {
                        return LocalKelvin[which];
                    }
                    return false;
                }
                set
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            output.Control.Abort();
                            switch (which)
                            {
                                case tlDCVILocalKelvin.Both:
                                    output.Measurement.Sense = value ? DCPowerMeasurementSense.Remote : DCPowerMeasurementSense.Local;
                                    break;
                                case tlDCVILocalKelvin.High:
                                    output.Measurement.Sense = value ? DCPowerMeasurementSense.Remote : DCPowerMeasurementSense.Local;
                                    break;
                                case tlDCVILocalKelvin.Low:
                                    output.Measurement.Sense = value ? DCPowerMeasurementSense.Remote : DCPowerMeasurementSense.Local;
                                    break;
                                default:
                                    throw new ArgumentException("Invalid local kelvin type.");
                            }
                            output.Control.Initiate();
                        }
                    });

                    // Update the dictionary
                    LocalKelvin[which] = value;
                }
            }





            //TheHdw.DCVI.Pins(PinList).LocalKelvin(which) 
            public int Mode
            {
                get //return the current gate or enabled settings of SMU
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            switch (output.Source.Output.Function)
                            {
                                case DCPowerSourceOutputFunction.DCVoltage:
                                    //set DCVI function to voltage mode
                                    return DCVIMode.tlDCVIModeVoltage;

                                case DCPowerSourceOutputFunction.DCCurrent:
                                    //set DCVI function to current mode
                                    return DCVIMode.tlDCVIModeCurrent;

                                default:
                                    throw new ArgumentException("Invalid mode value.");
                            }
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }
                set
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    _mode = value;
                    string[] Pins = PinList.GetAll().ToArray();
                    //Parallel.ForEach(Pins, (Pin, state, index) =>
                    //foreach (var Pin in Pins)
                    //{
                    //    NIDCPower[] dcPowerSessions;
                    //    string[] dcPowerChannelStrings;
                    //    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    //    //Parallel.ForEach(dcPowerSessions, (dcPowerSession, state, i) =>
                    //    Parallel.For(0, dcPowerSessions.Length, i =>
                    //    {
                    //        var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                    //        output.Control.Abort();
                    //        switch (_mode)
                    //        {
                    //            case DCVIMode.tlDCVIModeVoltage:
                    //                //set DCVI function to voltage mode
                    //                output.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                    //                break;
                    //            case DCVIMode.tlDCVIModeCurrent:
                    //                //set DCVI function to current mode
                    //                output.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                    //                break;
                    //            default:
                    //                throw new ArgumentException("Invalid mode value.");
                    //        }
                    //        output.Control.Initiate();
                    //    });
                    //}
                    //Run in parallel - adrian

                    Globals.tsmContext.GetNIDCPowerSessions(Pins, out NIDCPower[] dcPowerSessions, out string[] dcPowerChannelStrings);
                    var output = new DCPowerOutput[dcPowerSessions.Length];
                    //Parallel.ForEach(dcPowerSessions, (dcPowerSession, state, i) =>
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        output[i] = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        output[i].Control.Abort();
                        switch (_mode)
                        {
                            case DCVIMode.tlDCVIModeVoltage:
                                //set DCVI function to voltage mode
                                output[i].Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                                break;
                            case DCVIMode.tlDCVIModeCurrent:
                                //set DCVI function to current mode
                                output[i].Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                                break;
                            default:
                                throw new ArgumentException("Invalid mode value.");
                        }
                        output[i].Control.Initiate();
                    });
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }
            //public double NominalBandwidth
            //{
            //    //
            //    // Summary:
            //    // gets the value of the transient response being set and return a value of nominal BW based on the transient response
            //    //sets the value of the transient response, >= 50 is slow, 51-500 is Normal and >500 is Fast

            //    get //return the nominal BW value based on the transient response being set
            //    {

            //        string[] Pins = PinList.GetAll().ToArray();
            //        if (Pins.Length == 0)
            //        {
            //            throw new InvalidOperationException("Invalid pin: No pins found.");
            //        }

            //        foreach (var Pin in Pins)
            //        {
            //            NIDCPower[] dcPowerSessions;
            //            string[] dcPowerChannelStrings;
            //            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
            //            for (int i = 0; i < dcPowerSessions.Length; i++)
            //            {
            //                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
            //                if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Normal)
            //                {
            //                    return 500;
            //                }
            //                else if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Fast)
            //                {
            //                    return 5000;
            //                }
            //                else if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Slow)
            //                {
            //                    return 50;
            //                }
            //                else
            //                {
            //                    throw new ArgumentException("Invalid TransientResponse value");
            //                }
            //            }
            //        }
            //        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
            //    }
            //    set //Change the transient response if based on the range of the set nominal BW
            //    {
            //        string[] Pins = PinList.GetAll().ToArray();
            //        if (Pins.Length == 0)
            //        {
            //            throw new InvalidOperationException("Invalid pin: No pins found.");
            //        }

            //        Parallel.ForEach(Pins, (Pin) =>
            //        {
            //            NIDCPower[] dcPowerSessions;
            //            string[] dcPowerChannelStrings;
            //            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
            //            for (int i = 0; i < dcPowerSessions.Length; i++)
            //            {
            //                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
            //                if (value <= 50)
            //                {
            //                    output.Source.TransientResponse = DCPowerSourceTransientResponse.Slow;
            //                }
            //                else if (value >= 51 && value <= 500)
            //                {
            //                    output.Source.TransientResponse = DCPowerSourceTransientResponse.Normal;
            //                }
            //                else if (value > 500)
            //                {
            //                    output.Source.TransientResponse = DCPowerSourceTransientResponse.Fast;
            //                }
            //                else
            //                {
            //                    throw new ArgumentException("Invalid value");
            //                }
            //            }
            //        });
            //    }
            //}
            public NominalBandwidthProperty NominalBandwidth { get; private set; } = new NominalBandwidthProperty();

            public class NominalBandwidthProperty
            {
                private double _value;
                public PinList PinList { get; set; }

                // List: Gets the list of bandwidth ranges
                public List<double> List
                {
                    get
                    {
                        return new List<double> { 50, 500, 5000 }; // Example values for Slow, Normal, and Fast
                    }
                }

                // Max: Gets the maximum value for the bandwidth setting
                public double Max
                {
                    get
                    {
                        return List.Max();
                    }
                }

                // Min: Gets the minimum value for the bandwidth setting
                public double Min
                {
                    get
                    {
                        return List.Min();
                    }
                }

                // Value: Gets or sets the bandwidth for the specified pins
                public double Value
                {
                    get
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        foreach (var Pin in Pins)
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Normal)
                                {
                                    return 500;
                                }
                                else if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Fast)
                                {
                                    return 5000;
                                }
                                else if (output.Source.TransientResponse == DCPowerSourceTransientResponse.Slow)
                                {
                                    return 50;
                                }
                                else
                                {
                                    throw new ArgumentException("Invalid TransientResponse value");
                                }
                            }
                        }
                        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                        //return _value;
                    }
                    set
                    {
                        SetForPins(PinList.GetAll(), value); // Default to all pins
                    }
                }

                // Method to set NominalBandwidth for a specific list of pins
                public void SetForPins(IEnumerable<string> pins, double value)
                {
                    // Find the smallest setting greater than or equal to the programmed value
                    double closestValue = List.FirstOrDefault(bw => bw >= value);
                    if (closestValue == 0)
                    {
                        throw new ArgumentException("The programmed value exceeds the maximum available setting.");
                    }

                    _value = closestValue;

                    Parallel.ForEach(pins, (Pin) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            if (_value == 50)
                            {
                                output.Source.TransientResponse = DCPowerSourceTransientResponse.Slow;
                            }
                            else if (_value == 500)
                            {
                                output.Source.TransientResponse = DCPowerSourceTransientResponse.Normal;
                            }
                            else if (_value == 5000)
                            {
                                output.Source.TransientResponse = DCPowerSourceTransientResponse.Fast;
                            }
                        }
                    });
                }
            }


            public string PowerSupplyMode { get; set; }
            public int ReadTimeout { get; set; }
            public void Reset() { /* Implementation */ }
            public void SetCurrentAndRange(double current, double range)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    NIDCPower[] dcPowerSessions;
                    string[] dcPowerChannelStrings;
                    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        bool RunAbortInitiate = Math.Abs(output.Source.Current.CurrentLevel) > range;
                        if (RunAbortInitiate)
                        {
                            output.Control.Abort(); //added to prevent error on mismatched range and level -adrian
                        }

                        output.Source.Current.CurrentLevelRange = range;
                        output.Source.Current.CurrentLevel = current;

                        if (RunAbortInitiate)
                        {
                            output.Control.Initiate();  //added to prevent error on mismatched range and level -adrian
                        }
                    });
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public bool Settled { get; set; }
            public void SetVoltageAndRange(double voltage, double range)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    NIDCPower[] dcPowerSessions;
                    string[] dcPowerChannelStrings;
                    Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                    Parallel.For(0, dcPowerSessions.Length, i =>
                    {
                        var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                        //output.Source.Voltage.VoltageLevelRange = range;  //SMU-4162 has fixed voltage range of 24V -adrian
                        output.Source.Voltage.VoltageLevel = voltage;
                    });
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public void SourceNextSample() { /* Implementation */ }
            public double Voltage //This property sets or gets the voltage for the DCVI. Type Double. Use this property to program the DCVI's voltage. 
            {
                get //return the existing voltage level being set
                {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            return output.Source.Voltage.VoltageLevel;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
                set //change the existing Voltage level for the pin specified
                {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    Parallel.ForEach(Pins, (Pin) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            output.Source.Voltage.VoltageLevel = value;
                        }
                    });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }
            public class VoltageRange
            {
                public PinList PinList { get; set; }
                PinList singlePinList = new PinList();

                public bool AutoRange
                {
                    get //return if the SMU is set to autorange or not
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        foreach (var Pin in Pins)
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                if (output.Source.Voltage.VoltageLevelAutorange == DCPowerSourceVoltageLevelAutorange.On)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                    }
                    set //change the autorange to on or off
                    {

                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        Parallel.ForEach(Pins, (Pin) =>
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                output.Control.Abort();
                                if (value == true)
                                {
                                    output.Source.Voltage.VoltageLevelAutorange = DCPowerSourceVoltageLevelAutorange.On;
                                }
                                else
                                {
                                    output.Source.Voltage.VoltageLevelAutorange = DCPowerSourceVoltageLevelAutorange.Off;
                                }
                                output.Control.Initiate();
                            }
                        });
                    }
                }
                //Gets or sets the Autorange property for the current range. If you turn autoranging off, you must program a range. Type Boolean.
                public double[] List { get; set; }
                //Gets the list of values for the current range setting. Array of type Double.
                // there is no direct way to get the list of the voltage ranges of SMU and this property is not commonly use

                public double Max { get; set; }
                //Gets the maximum value for the current range setting for the specified pins. Type Double. 
                //there is no direct way to get the max of the voltage ranges of SMU for a property that is forcing a voltage and this property is not commonly use
                public double Min { get; set; }
                //Gets the minimum value for the current range setting for the specified pins. Type Double. 
                //there is no direct way to get the min of the voltage ranges of SMU for a property that is forcing a voltage and this property is not commonly use

                public double Value
                //Gets or sets the current range for the specified pins. Type Double. Default. 
                {
                    get // return the existing voltage level being set
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        foreach (var Pin in Pins)
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                return output.Source.Voltage.VoltageLevel;
                            }
                        }
                        throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                    }
                    set // change the existing current limit for the pin specified
                    {
                        string[] Pins = PinList.GetAll().ToArray();
                        if (Pins.Length == 0)
                        {
                            throw new InvalidOperationException("Invalid pin: No pins found.");
                        }

                        Parallel.ForEach(Pins, (Pin) =>
                        {
                            NIDCPower[] dcPowerSessions;
                            string[] dcPowerChannelStrings;
                            Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                            for (int i = 0; i < dcPowerSessions.Length; i++)
                            {
                                var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                                output.Control.Abort();
                                output.Source.Voltage.VoltageLevel = value;
                                output.Control.Initiate();
                            }
                        });
                    }
                }
            }

            // Interfaces
            public AsynchronousTrigger AsynchronousTrigger { get; set; }
            public Capture Capture { get; set; }
            public Meter Meter { get; set; }
            public PSets PSets { get; set; }
            public Source Source { get; set; }
        }

        // Interfaces
        public HotSwitchDetectInterface HotSwitchDetect { get; private set; }
        public DCVI()
        {
            HotSwitchDetect = new HotSwitchDetectInterface();
        }

        public class HotSwitchDetectInterface
        {
            // Define methods and properties for HotSwitchDetect
        }

        public class AsynchronousTrigger
        {
            // Define methods and properties for AsynchronousTrigger
        }

        public class Capture
        {
            // Define methods and properties for Capture
            // this function will be calling either Digital 6571 or Scope 5172 driver instead as no equivalent function in DCPower driver
            // pls refer to loadboard schematic to know which instrument is being used.

            public PinList PinList { get; set; } // Property to hold the pin list
            public bool IsCaptureDone //This property gets a Boolean indicating if the DCVI is currently capturing.
            {
                //get => _IsCaptureDone;
                get //
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = false; //hardcoded to False since this property is no available for NI DCPower inst driver
                            return output;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }

            }

            public void ClearCaptureMemory()
            {
                //no DCPower driver equivalent to this
            }
            public class Signals
            {
                public void Add(string SignalName)
                {
                    //no NI DCPower/Digital/Scope driver equivalent to this
                }

                public class Item
                {
                    public double[] DSPWave(string SignalName, string Instr = "Dig6571", string patternToBurst = "")
                    {
                        var captureData = new double[Globals.captureSampleSize]; //standardize output data to be 1D array

                        switch (Instr)
                        {
                            case ("Dig6571"):
                                var digiSessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                                var digitalssc = digiSessions.SSC;
                                digiSessions.BurstPattern(patternToBurst); //assume the digicapture has been created/loaded beforehand
                                var capDig = digiSessions.FetchCaptureWaveform(SignalName, Globals.captureSampleSize);
                                Array.Copy(capDig, captureData, Globals.captureSampleSize); //resize the array into 1D
                                break;

                            case ("Scope5172"):
                                var scopeSessions = InstrCtrl.ScopePinsToSessions(Globals.tsmContext, Globals.AllScopePins);
                                var capScope = scopeSessions.Fetch(Globals.captureSampleSize);
                                Array.Copy(capScope, captureData, Globals.captureSampleSize);
                                break;

                            case ("SMU"):
                                break;

                            default:
                                throw new InvalidOperationException("Invalid instrument name");
                                break;

                        }

                        return captureData;
                    }

                    public double Filter(string SignalName)
                    { return 0; }  //no DCPower/Digital/Scope driver equivalent to this

                    public void LoadSettings(string SignalName, string Instr = "Dig6571", double ScopeConfVertical = 10)
                    {
                        switch (Instr)
                        {
                            case ("Dig6571"):
                                var digiSessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

                                break;

                            case ("Scope5172"):
                                var scopeSessions = InstrCtrl.ScopePinsToSessions(Globals.tsmContext, Globals.AllScopePins);
                                scopeSessions.ConfigureVertical(ScopeConfVertical);
                                break;

                            case ("SMU"):
                                break;

                            default:
                                throw new InvalidOperationException("Invalid instrument name");
                                break;

                        }
                    }

                    public string Mode(string SignalName)
                    { return null; }  //no DCPower/Digital/Scope driver equivalent to this

                    public double Range(string SignalName)
                    { return 0; }  //no DCPower/Digital/Scope driver equivalent to this

                    public double SampleRate(string SignalName)
                    { return 0; } //no DCPower/Digital/Scope driver equivalent to this

                    public void SampleSize(string SignalName, int size)
                    {
                        Globals.captureSampleSize = size;
                    }
                }
            }

        }

        public class Meter
        {
            //public int _mode;
            //need mode for every DC -adrian
            public int Mode
            {
                get
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    switch (Pins[0])
                    {
                        case "VDD1":
                            return Globals.VDD1MeterMode;
                        case "VDD2":
                            return Globals.VDD2MeterMode;
                        case "VOA_dc30_da":
                            return Globals.VOAdc30MeterMode;
                        case "VOB_dc30_da":
                            return Globals.VOBdc30MeterMode;
                        default:
                            return 0;
                    }
                }
                set
                {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
                    string[] Pins = PinList.GetAll().ToArray();
                    Parallel.For(0, Pins.Length, i =>
                    {
                        switch (Pins[i])
                        {
                            case "VDD1":
                                Globals.VDD1MeterMode = value;
                                break;
                            case "VDD2":
                                Globals.VDD2MeterMode = value;
                                break;
                            case "VOA_dc30_da":
                                Globals.VOAdc30MeterMode = value;
                                break;
                            case "VOB_dc30_da":
                                Globals.VOBdc30MeterMode = value;
                                break;
                            default:
                                throw new ArgumentException("Invalid pin: No valid power sessions found.");
                        }
                    });
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }  // Property to hold the pin meter mode  -adrian

            public double _voltageRange;
            public PinList PinList { get; set; } // Property to hold the pin list
            public _Filter Filter { get; set; } = new _Filter(); // Property to hold the filter settings

            // Method to read meter values and return double[]
            public (double[], List<string>) Read(tlStrobeOption strobeOption, int sampleSize, double sampleRate, tlDCVIMeterReadingFormat format)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                double[] value = new double[0];
                double[] V_value = new double[0];
                double[] I_value = new double[0];

                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    var SMUSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
                    SMUSession.Measure(out V_value, out I_value);

                    //if (_mode == DCVIMeterMode.tlDCVIMeterVoltage)
                    if (Globals.TheHdw.DCVI.Pins(Pin).Meter.Mode == DCVIMeterMode.tlDCVIMeterVoltage)  //replaced -adrian
                    {
                        value = V_value;
                    }
                    else
                    {
                        value = I_value;
                    }
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
                return (value, Pins.ToList());
            }

            // Method to read meter values and return double[]
            //overload for read -adrian
            public PinListData Read(tlStrobeOption strobeOption, int sampleSize, double sampleRate, tlDCVIMeterReadingFormat format, bool MultiplePins = true)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                double[][] value = new double[Pins.Length][];
                double[][] V_value = new double[Pins.Length][];
                double[][] I_value = new double[Pins.Length][];
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    var SMUSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
                    SMUSession.Measure(out V_value[index], out I_value[index]);

                    //if (_mode == DCVIMeterMode.tlDCVIMeterVoltage)
                    if (Globals.TheHdw.DCVI.Pins(Pin).Meter.Mode == DCVIMeterMode.tlDCVIMeterVoltage)  //replaced -adrian
                    {
                        //value = V_value;  //place all measured value of all pins to array -adrian
                        value[index] = V_value[index];
                    }
                    else
                    {
                        //value = I_value;  //place all measured value of all pins to array -adrian
                        value[index] = I_value[index];
                    }
                });
                PinListData PinListData = ConvertToPinListData(value, Pins);
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
                return PinListData;
            }

            //added for meter class -adrian
            public static PinListData ConvertToPinListData(double[][] data, string[] pinNames)
            {
                if (data.Length != pinNames.Length)
                {
                    throw new ArgumentException("The number of pin names must match the number of rows in the data array.");
                }

                PinListData pinListData = new PinListData();

                for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
                {
                    string pinName = pinNames[pinIndex];
                    for (int siteIndex = 0; siteIndex < data[pinIndex].Length; siteIndex++)
                    {
                        pinListData.Add(pinName, siteIndex, data[pinIndex][siteIndex]); //per pin measurement was placed per row -adrian
                    }
                }
                return pinListData;
            }

            // Property to get or set the mode of the meter
            //public int Mode
            //{
            //    //get => _mode;
            //    get => mode;    //replaced -adrian
            //    set
            //    {
            //        //_mode = value;
            //        string[] Pins = PinList.GetAll().ToArray();
            //        //Parallel.ForEach(Pins, (Pin, state, index) =>
            //        foreach (var Pin in Pins)
            //        {
            //            var DCSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
            //            //DCSession.Abort();
            //            //foreach (var channel in DCSession)
            //            //{
            //            //switch (_mode)
            //            //{
            //            //    case DCVIMeterMode.tlDCVIMeterVoltage:
            //            //        // Set DCVI function to voltage mode
            //            //        _mode = DCVIMeterMode.tlDCVIMeterVoltage;//No need to set Meter Mode for STS.
            //            //        break;
            //            //    case DCVIMeterMode.tlDCVIMeterCurrent:
            //            //        // Set DCVI function to current mode
            //            //        _mode = DCVIMeterMode.tlDCVIMeterCurrent;//No need to set Meter Mode for STS.
            //            //        break;
            //            //    default:
            //            //        throw new ArgumentException("Invalid mode value.");
            //            //}
            //            //}
            //            //DCSession.Initiate();
            //        }//);
            //    }
            //}

            // Property to get or set the voltage range of the meter
            public double VoltageRange
            {
                //get => _voltageRange;
                get // return the existing Voltage range setting of the SMU
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            return output.Measurement.AutorangeMinimumVoltageRange;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }

                set
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        var SMUSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
                        SMUSession.Abort();
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            output.Measurement.AutorangeMinimumVoltageRange = value;
                        }
                        SMUSession.Initiate();
                    });

                }
            }

            public double CurrentRange
            {
                //get => _voltageRange;
                get // return the existing Current range setting of the SMU
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    foreach (var Pin in Pins)
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            return output.Measurement.AutorangeMinimumCurrentRange;
                        }
                    }
                    throw new InvalidOperationException("Invalid pin: No valid power sessions found.");
                }
                set
                {
                    _voltageRange = value;

                    string[] Pins = PinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        var SMUSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
                        SMUSession.Abort();
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        for (int i = 0; i < dcPowerSessions.Length; i++)
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];
                            output.Measurement.AutorangeMinimumCurrentRange = value;
                        }
                        SMUSession.Initiate();
                    });

                }
            }
            // Method to strobe the meter
            public void Strobe(int sampleSize = 1, double sampleRate = 1.0, bool strobeWait = false)
            {
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    var DCSession = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, Pin);
                    foreach (var channel in DCSession.SSC)
                    {
                        // Simulate strobe operation
                        // The actual implementation will depend on the hardware specifics.
                    }
                });

                if (strobeWait)
                {
                    // Wait for strobe to complete
                    // The actual implementation will depend on the hardware specifics.
                }
            }

            public class _Filter
            {
                //Our SMU do not have a meter filter feauture available. Instead we use the transient response as for the similar purpose as the meter filter. 
                //the transient response has been already set at Nominal BW since it also acts the same as the transient response
                //This function will leave as blank to not redundantly call the transient response that will result to increase in test time

                public double _value = 0;
                public bool bypass_filter = false;

                //no equivalent function in NIDCPower see explanation above for reference
                public bool Bypass
                {
                    get => bypass_filter;
                    set
                    {
#if TestTimeMeasure
                        Globals.TestTimeMeasure.TestTimeStart();
#endif
                        bypass_filter = value;
                        //no equivalent function in NIDCPower see explanation above for reference
#if TestTimeMeasure
                        Globals.TestTimeMeasure.TestTimeStop();
#endif
                    }
                }

                //no equivalent function in NIDCPower see explanation above for reference
                public double Value
                {
                    get => _value;
                    set
                    {
                        _value = value;
                        //no equivalent function in NIDCPower see explanation above for reference
                    }
                }

                //no equivalent function in NIDCPower see explanation above for reference
                public double[] List
                {
                    get
                    {
                        //no equivalent function in NIDCPower see explanation above for reference
                        return new double[] { 100, 200, 300 };
                    }
                }

                //no equivalent function in NIDCPower see explanation above for reference
                public double Max
                {
                    get
                    {
                        //no equivalent function in NIDCPower see explanation above for reference
                        return 300;
                    }
                }


                public double Min
                {
                    get
                    {
                        //no equivalent function in NIDCPower see explanation above for reference
                        return 100;
                    }
                }

                // Implicit conversion from double to Filter
                public static implicit operator _Filter(double? value)
                {
                    return new _Filter { Value = value ?? 100 };
                }
            }
        }


        public class PSets
        {
            // Define methods and properties for PSets
        }

        public class Source
        {
            // Define methods and properties for Source
            private PinList pinList;

            public Signal Signals { get; set; }

            // Constructor requires parameters
            public Source(PinList pinList)
            {
                this.pinList = pinList;
                Signals = new Signal(pinList);
            }

            public SignalCollection SignalName(string signalName)
            {
                return new Signal(pinList).Item(signalName);
            }

            public class Signal
            {
                // Define methods and properties for Signal
                private PinList pinList;

                public Dictionary<string, SignalCollection> SignalList = new Dictionary<string, SignalCollection>();

                // Constructor requires parameters
                public Signal(PinList pinList)
                {
                    this.pinList = pinList;
                }

                public void Add(string signalName)
                {
                    if (SignalList.ContainsKey(signalName))
                    {
                        SignalList.Remove(signalName);
                        SignalList.Add(signalName, new SignalCollection { });
                        SignalList[signalName].pinList = this.pinList;
                    }
                    else
                    {
                        SignalList.Add(signalName, new SignalCollection { });
                        SignalList[signalName].pinList = this.pinList;
                    }
                }

                public SignalCollection Item(string signalName)
                {
                    return SignalList[signalName];
                }
            }

            public class SignalCollection
            {
                // Define properties for SignalCollection with default value
                public double Amplitude { get; set; } = 1;
                public string WaveDefinitionName { get; set; } = String.Empty;
                public int SampleSize { get; set; } = 0;

                public Bandwidth Bandwidth { get; set; }
                public double[] Samples { get; set; }
                public SampleRate SampleRate { get; set; }
                public Range Range { get; set; }
                public Modes Mode { get; set; }

                public PinList pinList { get; set; }

                public SignalCollection()
                {
                    Bandwidth = new Bandwidth();
                    SampleRate = new SampleRate();
                    Range = new Range();
                    Mode = new Modes();
                }

                public void LoadSettings()
                {
                    string[] Pins = pinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        Parallel.For(0, dcPowerSessions.Length, i =>
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];

                            switch (Mode.Value)
                            {
                                case Globals.tlDCVIModeVoltage:
                                    output.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                                    switch (Range.Mode)
                                    {
                                        case Globals.tlSignalModeUseValue:   // Set value Range
                                            output.Source.Voltage.VoltageLevelAutorange = DCPowerSourceVoltageLevelAutorange.Off;
                                            output.Source.Voltage.VoltageLevelRange = Range.Value;
                                            break;
                                        case Globals.tlSignalModeUseLoadedValue:    // no change Range
                                            break;
                                        case Globals.tlCSignalModeUseCalculatedValue:    // Set Auto Range
                                            output.Source.Voltage.VoltageLevelAutorange = DCPowerSourceVoltageLevelAutorange.On;
                                            break;
                                        default:
                                            throw new ArgumentException("Invalid Range Mode.");
                                    }
                                    switch (Bandwidth.Mode)
                                    {
                                        case Globals.tlSignalModeUseValue:   // Set value to Gain Bandwidth
                                            output.Source.CustomTransientResponse.Voltage.GainBandwidth = Bandwidth.Value;
                                            break;
                                        case Globals.tlSignalModeUseLoadedValue:    // no change Gain Bandwidth
                                            break;
                                        case Globals.tlCSignalModeUseCalculatedValue:    // Set maximum Gain Bandwidth
                                            output.Source.CustomTransientResponse.Voltage.GainBandwidth = 0;
                                            break;
                                        default:
                                            throw new ArgumentException("Invalid Bandwidth Mode.");
                                    }
                                    break;
                                case Globals.tlDCVIModeCurrent:
                                    output.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                                    switch (Range.Mode)
                                    {
                                        case Globals.tlSignalModeUseValue:   // Set value Range
                                            output.Source.Current.CurrentLevelAutorange = DCPowerSourceCurrentLevelAutorange.Off;
                                            output.Source.Current.CurrentLevelRange = Range.Value;
                                            break;
                                        case Globals.tlSignalModeUseLoadedValue:    // no change Range
                                            break;
                                        case Globals.tlCSignalModeUseCalculatedValue:    // Set Auto Range
                                            output.Source.Current.CurrentLevelAutorange = DCPowerSourceCurrentLevelAutorange.On;
                                            break;
                                        default:
                                            throw new ArgumentException("Invalid Range Mode.");
                                    }
                                    switch (Bandwidth.Mode)
                                    {
                                        case Globals.tlSignalModeUseValue:   // Set value to Gain Bandwidth
                                            output.Source.CustomTransientResponse.Current.GainBandwidth = Bandwidth.Value;
                                            break;
                                        case Globals.tlSignalModeUseLoadedValue:    // no change Gain Bandwidth
                                            break;
                                        case Globals.tlCSignalModeUseCalculatedValue:    // Set maximum Gain Bandwidth
                                            output.Source.CustomTransientResponse.Current.GainBandwidth = 0;
                                            break;
                                        default:
                                            throw new ArgumentException("Invalid Bandwidth Mode.");
                                    }
                                    break;
                                default:
                                    throw new ArgumentException("Invalid source mode.");
                            }
                        });
                    });
                }

                public void LoadSamples()
                {
                    string[] Pins = pinList.GetAll().ToArray();
                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        NIDCPower[] dcPowerSessions;
                        string[] dcPowerChannelStrings;
                        Globals.tsmContext.GetNIDCPowerSessions(Pin, out dcPowerSessions, out dcPowerChannelStrings);
                        Parallel.For(0, dcPowerSessions.Length, i =>
                        {
                            var output = dcPowerSessions[i].Outputs[dcPowerChannelStrings[i]];

                            bool dcSourceModeSinglePoint = Samples.Length > 1 ? false : true;

                            if (dcSourceModeSinglePoint)
                            {
                                dcPowerSessions[i].Source.Mode = DCPowerSourceMode.SinglePoint;
                                switch (this.Mode.Value)
                                {
                                    case Globals.tlDCVIModeVoltage:
                                        output.Source.Voltage.VoltageLevel = Samples[0] * Amplitude;
                                        break;
                                    case Globals.tlDCVIModeCurrent:
                                        output.Source.Current.CurrentLevel = Samples[0] * Amplitude;
                                        break;
                                    default:
                                        throw new ArgumentException("Invalid source mode.");
                                }
                            }
                            else
                            {
                                dcPowerSessions[i].Source.Mode = DCPowerSourceMode.Sequence;
                                output.Source.SetSequence(Samples.Select(e => e * Amplitude).ToArray());
                            }

                            // Set sample rate
                            switch (SampleRate.Mode)
                            {
                                case Globals.tlSignalModeUseValue:   // Set value to Sample Rate (adjust only aperture time)
                                    if (SampleRate.Value > 0)
                                    {
                                        double targetPeriod = 1 / SampleRate.Value;     // Convert sample rate to period
                                        double targetApertureTime = targetPeriod - output.Source.SourceDelay.TotalSeconds;
                                        if (targetApertureTime > 0)
                                        {
                                            output.Measurement.ApertureTime = targetApertureTime;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Cannot set Sample Rate Value.");
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Cannot set Sample Rate Value.");
                                    }
                                    break;
                                case Globals.tlSignalModeUseLoadedValue:    // no change Sample Rate
                                    break;
                                default:
                                    throw new ArgumentException("Invalid Sample Rate Mode.");
                            }
                        });
                    });
                }
            }

            public class Bandwidth
            {
                public double Mode { get; set; } = Globals.tlCSignalModeUseCalculatedValue;
                public double Value { get; set; }
            }

            public class SampleRate
            {
                public double Mode { get; set; }
                public double Value { get; set; }
            }

            public class Range
            {
                public double Mode { get; set; } = Globals.tlCSignalModeUseCalculatedValue;
                public double Value { get; set; }
            }

            public class Modes
            {
                public int Mode { get; set; }
                public int Value { get; set; } = Globals.tlDCVIModeVoltage;
            }
        }
    }

    public class PPMU
    {
        public PinController Pins(PinList pinList)
        {
            string[] TestPinsArray = pinList.GetAll().ToArray();
            string[] PinGroupsArray = Globals.tsmContext.GetPinsInPinGroups(TestPinsArray);
            pinList = PinList.FromStringArray(PinGroupsArray);
            return new PinController(pinList);
        }

        public PinController Pins(string pin)
        {
            PinList singlePinList = new PinList();
            string[] PinGroupsArray = Globals.tsmContext.GetPinsInPinGroup(pin);
            singlePinList = PinList.FromStringArray(PinGroupsArray);
            return Pins(singlePinList);
        }

        public class PinController
        {
            //public CallerType Caller { get; private set; } // Assuming CallerType is defined elsewhere
            public PinList PinList { get; set; }
            public PinController(PinList pinList)
            {
                //Caller = Caller;
                PinList = pinList;
            }

            public void Connect()
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    //PPMU connect
                    var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                    DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Ppmu);
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            //public double Current { get; set; }
            public void Disconnect() 
            { /* Implementation */ //added-adrian
                    #if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    //PPMU connect
                    var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                    DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Digital);
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public void ForceCurrentRange(double range) { /* Implementation */ }
            public void ForceI(double current, double? forceCurrentRange = null, double testLimitVHi = 6, double testLimitVLo = -2)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    // If forceCurrentRange is not specified, set it to the highest force current range supported by the instrument
                    if (!forceCurrentRange.HasValue)
                    {
                        forceCurrentRange = 0.002; // 2 mA for BBAC, HSD200, and VHFAC
                    }
                    var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                    DigiSession.PPMUConfigureVoltageLimits(testLimitVHi, testLimitVLo);
                    DigiSession.PPMUForceCurrent(current, forceCurrentRange, true);
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public void ForceV(double voltage, double? measureCurrentRange = null, double testLimitIHi = 32e-3, double testLimitILo = -32e-3)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                string[] Pins = PinList.GetAll().ToArray();
                Parallel.ForEach(Pins, (Pin, state, index) =>
                {
                    // Adjust the measureCurrentRange based on the test limits if not specified
                    //if (Math.Abs(testLimitIHi) == Math.Abs(testLimitILo))
                    //{
                    //    testLimitILo = testLimitIHi;
                    //}
                    double nonNullmeasureCurrentRange = measureCurrentRange ?? 0.002;// 2 mA default value if nullableDouble is null
                    var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                    //foreach(var channel in DigiSession.SSC)
                    //{
                    //channel.PinSet.Ppmu.DCVoltage.ConfigureCurrentLimit(ModularInstruments.NIDigital.PpmuCurrentLimitBehavior.CurrentRegulate, testLimitIHi);
                    //}
                    DigiSession.PPMUForceVoltage(voltage, nonNullmeasureCurrentRange, true);
                });
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }
            public bool Gate
            {
                // IGXL: TheHdw.PPMU.Pins(PinList).Gate 
                // This property gets or sets the PPMU gate on or off. 
                // STS : true = set selectedfunction to ppmu , false = set selectedfunction to off

                get //return the gate function
                {
                    string[] Pins = PinList.GetAll().ToArray();
                    if (Pins.Length == 0)
                    {
                        throw new InvalidOperationException("Invalid pin: No pins found.");
                    }

                    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pins);
                    var digitalssc = sessions.SSC;
                    Boolean output = false;

                    Parallel.ForEach(digitalssc, SSC =>
                    {
                        switch (SSC.PinSet.SelectedFunction)
                        {
                            case (SelectedFunction.Ppmu):
                                output = true;
                                break;
                            default:
                                output = false;
                                break;
                        }
                    });

                    return output;

                    throw new InvalidOperationException("Invalid pin: No valid ppmu sessions found.");
                }
                set // set the gate function
                {
                    string[] Pins = PinList.GetAll().ToArray();

                    Parallel.ForEach(Pins, (Pin, state, index) =>
                    {
                        //PPMU connect
                        var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                        if (value)
                            DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Ppmu);
                        else
                            DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Off);

                    });

                }
            }
            public double MeasureCurrentRange { get; set; }
            public int Mode { get; set; }
            //public (double[], List<string>) Read(tlPPMUReadWhat? readType = tlPPMUReadWhat.tlPPMUReadMeasurements)
            public PinListData Read(tlPPMUReadWhat? readType = tlPPMUReadWhat.tlPPMUReadMeasurements, PpmuMeasurementType ppmuMeasurementType = PpmuMeasurementType.Voltage)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                double[] value = new double[0];
                //object lockObject = new object(); // Object used for locking
                List<string> pinNames = PinList.GetAll();

                string[] Pins = pinNames.ToArray();

                //string[] Pins = PinList.GetAll().ToArray();
                //int i = 0;
                //int j = 0;

                //Parallel.ForEach(Pins, (Pin, state, index) =>
                ////foreach (var Pin in Pins)
                //{
                //    var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                //    var data = DigiSession.PPMUMeasure(ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

                //    // Flatten the two-dimensional array into a List<double>
                //    //var dataList = data.SelectMany(innerArray => innerArray).ToList();//#debug to verify data results later
                //    //string[] PinArray = { Pin };
                //    //value = ConvertToPinListData(data, PinArray);
                //    //lock (lockObject) // Ensure thread safety when modifying value
                //    //{
                //    //    value.AddPinData(Pin, dataList);
                //    //}
                //    int datalength = data.GetLength(0);
                //    Array.Resize(ref value, (j + 1) * datalength);
                //    for (; i < datalength; i++)
                //    {
                //        value[i] = data[i][j];//2D-to-1D data. Only one pin per session
                //    }
                //    j++;
                //});
                //return (value, pinNames);


                //*************replaced - adrian***************
                PinListData PinListData;
                var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pins);
                var data = DigiSession.PPMUMeasure(ppmuMeasurementType);

                PinListData = ConvertToPinListData(data, Pins);
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
                return PinListData;
            }

            public static PinListData ConvertToPinListData(double[][] data, string[] pinNames)
            {
                //if (data.Length != pinNames.Length)
                if (data.Length != Globals.tsmContext.SiteNumbers.Count) //per site = per row -adrian
                {
                    //throw new ArgumentException("The number of pin names must match the number of rows in the data array.");
                    throw new ArgumentException("The number of site must match the number of rows in the data array.");
                }

                PinListData pinListData = new PinListData();

                for (int pinIndex = 0; pinIndex < pinNames.Length; pinIndex++)
                {
                    string pinName = pinNames[pinIndex];
                    for (int siteIndex = 0; siteIndex < data.Length; siteIndex++)
                    {
                        //pinListData.Add(pinName, siteIndex, data[pinIndex][siteIndex]);
                        pinListData.Add(pinName, siteIndex, data[siteIndex][pinIndex]); //for digital meas -adrian
                    }
                }
                return pinListData;
            }
            public void Reset() { /* Implementation */ }
            public void Test() { /* Implementation */ }
            public double Voltage { get; set; }

            public TestLimits TestLimits { get; private set; }

            public PinController()
            {
                TestLimits = new TestLimits();
            }

        }

        // Properties and Methods
        public void SetClampsV(double value) { /* Implementation */ }
        public void SetClampsVHi(double value)
        {
            /* Implementation */
            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

            Parallel.ForEach(sessions.SSC, ssc => ssc.PinSet.Ppmu.DCCurrent.VoltageLimitHigh = value);
            sessions.PPMUSource();
        }
        public void SetClampsVHiMax(double value) { /* Implementation */ }
        public void SetClampsVLo(double value)
        {
            /* Implementation */
            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

            Parallel.ForEach(sessions.SSC, ssc => ssc.PinSet.Ppmu.DCCurrent.VoltageLimitLow = value);
            sessions.PPMUSource();
        }
        public void SetClampsVLoMin(double value) { /* Implementation */ }

        public class TestLimits
        {
            // Define methods and properties for TestLimits
        }
    }

    public class Digital
    {
        public CallerType Caller { get; private set; } // Assuming CallerType is defined elsewhere
        public PinList PinList { get; set; }

        public void ApplyLevelsTiming(bool ConnectAllPins = false, bool loadLevels = false, bool loadTiming = false,
                                      TlRelayMode RelayMode = TlRelayMode.tlPowered, List<string> InitPinsHi = null,
                                      List<string> InitPinsLo = null, string InitPinsHiz = null,
                                      bool voltagelevels = false, double vil = 0, double vih = 0, double vol = 0, double voh = 0, double vterm = 0)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            // Implementation details:
            // 1. If connectAllPins is true, connect all pins specified on the levels sheet.
            // 2. If loadLevels is true, load levels as specified on the test instances sheet.
            // 3. If loadTiming is true, load timing values specified on the Test Instances sheet.
            // 4. Check relayMode to decide between cold switching (tlUnpowered) and hot switching (tlPowered).
            // 5. Set pins to Drive High, Drive Low, or High Impedance (Hiz) init-states as specified using the lists.

            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
            var digitalssc = sessions.SSC;


            //Optional. If it is tlUnpowered, it first powers down the DUT before closing relays on the digital channels (cold switching).
            if (RelayMode == TlRelayMode.tlUnpowered)
            {
                Parallel.ForEach(digitalssc, SSC =>
                {
                    SSC.PinSet.SelectedFunction = SelectedFunction.Off;
                });
            }

            if (ConnectAllPins)
            {
                Parallel.ForEach(digitalssc, SSC =>
                {
                    SSC.PinSet.SelectedFunction = SelectedFunction.Digital;//set to Digital
                });
            }

            // Set pins to Drive High
            if (InitPinsHi != null)
            {
                ////Parallel.ForEach(InitPinsHi, Pin =>
                //foreach (var Pin in InitPinsHi)
                //{
                //    //SSC.PinSet.WriteStatic(ModularInstruments.NIDigital.PinState.H);
                //    var session = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                //    session.WriteStatic(PinState._1);
                //}//);
                //TTR -adrian
                if (InitPinsHi != null)
                {
                    var session = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, InitPinsHi.ToArray());
                    session.WriteStatic(PinState._1);
                }
            }

            // Set pins to Drive Low
            if (InitPinsLo != null)
            {
                //Parallel.ForEach(InitPinsLo, Pin =>
                //foreach (var Pin in InitPinsLo)
                //{
                //    //SSC.PinSet.WriteStatic(ModularInstruments.NIDigital.PinState.L);
                //    var session = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                //    session.WriteStatic(PinState._0);
                //}//);
                //TTR - adrian
                if (InitPinsLo != null)
                {
                    var session = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, InitPinsLo.ToArray());
                    session.WriteStatic(PinState._0);
                }
            }

            // Set pins to High Impedance (Hiz)
            if (InitPinsHiz != null)
            {
                Parallel.ForEach(digitalssc, SSC =>
                {
                    SSC.PinSet.DigitalLevels.TerminationMode = ModularInstruments.NIDigital.TerminationMode.HighZ;
                });
            }

            //Adjust voltage levels -adrian
            if (voltagelevels)
            {
                Parallel.ForEach(digitalssc, SSC =>
                {
                    SSC.PinSet.DigitalLevels.ConfigureVoltageLevels(vil, vih, vol, voh, vterm);
                });
            }

            // Apply levels and timings to pins
            if (loadLevels || loadTiming)
            {
                //another option to apply levels and timings
                //PinLevels.LoadPinLevels();
                
                Globals.levelSheetName = Globals.levelSheetName.Replace(".digilevels", "");
                Globals.timingSheetName = Globals.timingSheetName.Replace(".digitiming", "");
                Parallel.ForEach(digitalssc, SSC =>
                //foreach (var SSC in digitalssc)
                {
                    SSC.Session.ApplyLevelsAndTiming(SSC.SiteList, Globals.levelSheetName, Globals.timingSheetName);
                });
            }

#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
        }

        public bool ChannelFailed { get; set; }
        public void ConnectPins(string pinList)
        {
            // Split the input string into individual pin names
            var pinNames = pinList.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Create a new PinList and add each pin name to it
            PinList pins = new PinList();
            foreach (var pinName in pinNames)
            {
                pins.Add(pinName);
            }

            // Call the overload of ConnectPins that takes a PinList
            ConnectPins(pinList);
        }
        // Method to connect pins, now accepting a list of pin names
        public void ConnectPins(PinList pinList)
        {
            // Implementation for connecting pins using a list of pin names
            string[] Pins = pinList.GetAll().ToArray();
            Parallel.ForEach(Pins, (Pin, state, index) =>
            {
                // Current implementation is to switch the pins to Digital function
                // Depending on the debug result, this could change to switching the pins from relay instead
                var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Digital);
            });
        }
        public void DisconnectPins(string pins)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            // Split the input string into individual pin names
            var pinNames = pins.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Create a new PinList and add each pin name to it
            PinList pinList = new PinList();
            foreach (var pinName in pinNames)
            {
                pinList.Add(pinName);
            }

            // Call the overload of DisconnectPins that takes a PinList
            DisconnectPins(pinList);
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
        // Method to disconnect pins, now accepting a list of pin names
        public void DisconnectPins(PinList PinList)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            // Implementation for disconnecting pins using a list of pin names
            string[] Pins = PinList.GetAll().ToArray();
            Parallel.ForEach(Pins, (Pin, state, index) =>
            {
                var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Pin);
                DigiSession.SelectFunction(ModularInstruments.NIDigital.SelectedFunction.Disconnect);
            });
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
        }
        public int FailedPinsCount { get; set; }
        public bool ForcedHVPoweredMode { get; set; }
        // Method to get which pins failed the last PatGen burst.
        public void PinsFailedEx()
        {
            // NI Digital does not support this function.
        }
        // Method to read if a given pin at a given site failed the last PatGen burst.
        // True means the pin failed and False means the pin passed.
        public bool PinSiteFailed(string pinName, long siteNum)
        {
            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, new string[] { pinName });
            bool[][] passFailResults = sessions.GetSitePassFail();

            foreach (var ssc in sessions.SSC)
            {
                foreach (var data in ssc.SiteNumbers.Zip(passFailResults[ssc.Index], (x, y) => new { siteNumber = x, value = y }))
                {
                    if (data.siteNumber == siteNum)
                    {
                        bool failPinCheck = !data.value;
                        return failPinCheck;
                    }
                }
            }
            return true;
        }
        public int PinSiteFromChan { get; set; }

        // Propose to use NI Digital trigger export to trigger other instruments in the PXI chassis.
        // Property to get the sync pulse enable state of a channel. Read-only Boolean.
        public bool SyncEnabled
        {
            get; set;
            // NI Digital does not support this property.
        }

        // Method to program a channel's timing and format to be able to produce a sync pulse. 
        public void SyncModeOn()
        {
            // NI Digital does not support this function.
        }

        // Method to disable sync pulse generation. 
        public void SyncPulseOff()
        {
            // NI Digital does not support this function.
        }

        // Method to output a sync pulse on a channel when a pattern is run.
        public void SyncPulseOn()
        {
            // NI Digital does not support this function.
        }

        // Method to specify pins to exclude from UserDib calibration.
        public void UserDibExcludePins()
        {
            // NI Digital does not support this function.
        }

        // Interfaces
        public FreqCtrInterface FreqCtr { get; set; }
        public HRAMInterface HRAM { get; set; }
        public KeepAliveInterface KeepAlive { get; set; }
        public PatgenInterface Patgen { get; set; }
        public PatternsInterface Patterns { get; }
        public TimingInterface Timing { get; set; }

        //Initialize Interfaces
        public Digital()
        {
            FreqCtr = new FreqCtrInterface();
            HRAM = new HRAMInterface();
            KeepAlive = new KeepAliveInterface();
            Patgen = new PatgenInterface();
            Patterns = new PatternsInterface();
            Timing = new TimingInterface();
        }

        // Define the required classes or interfaces
        public class FreqCtrInterface
        {
            // Add properties and methods as needed
        }

        public class HRAMInterface
        {
            // Add properties and methods as needed
        }

        public class KeepAliveInterface
        {
            // Add properties and methods as needed
        }

        public class PatgenInterface
        {
            // Properties

            // Gets or sets the behavior of the ccall opcode. Read/Write Boolean.
            public bool Ccall { get; set; }

            // Gets the states of the CPU flags. Read-only Long.
            public long CpuFlags { get; private set; }

            // Gets the pattern generator cycle count. Read-only Long.
            public long CycleCount { get; private set; }

            // Gets or sets the pattern generator event cycle number. Read/Write Long.
            public long EventCycleCount { get; set; }

            // Enables or disables the pattern generator cycle event. Read/Write Boolean.
            public bool EventCycleEnabled { get; set; }

            // Enables use of PatGen EXT flag. Read/Write Boolean.
            public bool ExtEnable { get; set; }

            // Gets the current pattern generator fail count. Read-only Long.
            public long FailCount { get; private set; }

            // Gets the state of the pattern generator pass/fail flag. Read-only Boolean.
            public bool FailFlag { get; private set; }

            // Gets the number of flag matches. Read-only Boolean.
            public bool FlagMatchCount { get; private set; }

            // Method to check if keepalive is running.
            // Use method instead of property as NI Digital needs pinName to get the sessions.
            public bool IsKeepAlive(InstrumentControl.Digital sessions, long siteNum)
            {
                foreach (var ssc in sessions.SSC)
                {
                    foreach (int siteNumber in ssc.SiteNumbers)
                    {
                        if (siteNumber == siteNum)
                        {
                            bool keepAlive = ssc.Session.PatternControl.IsKeepAliveActive;
                            return keepAlive;
                        }
                    }
                }
                return false;
            }

            // Method to check if a pattern is running.
            // Use method instead of property as NI Digital needs pinName to get the sessions.
            public bool IsRunning(InstrumentControl.Digital sessions, long siteNum)
            {
                foreach (var ssc in sessions.SSC)
                {
                    foreach (int siteNumber in ssc.SiteNumbers)
                    {
                        if (siteNumber == siteNum)
                        {
                            bool isRunning = !ssc.Session.PatternControl.IsDone;
                            return isRunning;
                        }
                    }
                }
                return false;
            }

            // Method to check if a pattern is running on any site.
            // Use method instead of property as NI Digital needs pinName to get the sessions.
            public bool IsRunningAnySite(InstrumentControl.Digital sessions)
            {
                foreach (var ssc in sessions.SSC)
                {
                    foreach (int siteNumber in ssc.SiteNumbers)
                    {
                        bool isRunning = !ssc.Session.PatternControl.IsDone;
                        if (isRunning)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            // Method to check if either a pattern or keepalive is running.
            // Use method instead of property as NI Digital needs pinName to get the sessions.
            public bool IsRunningOrKA(InstrumentControl.Digital sessions, long siteNum)
            {
                foreach (var ssc in sessions.SSC)
                {
                    foreach (int siteNumber in ssc.SiteNumbers)
                    {
                        if (siteNumber == siteNum)
                        {
                            bool isRunning = !ssc.Session.PatternControl.IsDone;
                            bool keepAlive = ssc.Session.PatternControl.IsKeepAliveActive;
                            return isRunning || keepAlive;
                        }
                    }
                }
                return false;
            }

            // Determines whether to keep the hardware setup for the subsequent pattern run. Read/Write Boolean.
            public bool KeepHWSetup { get; set; }

            // Enables or disables PatGen 32-bit loop counter mode. Read/Write Boolean.
            public bool Loop32 { get; set; }

            // Gets or sets the pattern mask opcode inversion state. Read/Write Boolean.
            public bool MaskInvert { get; set; }

            // Overrides fail detection until cycle count event. Read/Write Boolean.
            public bool MaskTilCycle { get; set; }

            // Gets or sets the pattern generator no-halt mode. Read/Write NoHaltMode.
            public NoHaltMode NoHaltMode { get; set; }

            // Method to read the Accumulated Failed Registers for all the channels on this site.
            public bool PatternBurstPassed(InstrumentControl.Digital sessions, long siteNum)
            {
                bool[][] passFailResults = sessions.GetSitePassFail();

                foreach (var ssc in sessions.SSC)
                {
                    foreach (var data in ssc.SiteNumbers.Zip(passFailResults[ssc.Index], (x, y) => new { siteNumber = x, value = y }))
                    {
                        if (data.siteNumber == siteNum)
                        {
                            bool patternBurst = data.value;
                            return patternBurst;
                        }
                    }
                }
                return false;
            }

            // Property to get the current pattern generator read code. Read-only Long.
            public long ReadCode
            {
                get; private set;
                // NI Digital does not support this property.
            }

            // Property to get or set the pattern timeout value. Read/Write Double.
            public double Timeout
            {
                get; set;
                // NI Digital support this property in BurstPattern function.
            }

            // Property to enable or disable the pattern timeout. Read/Write Boolean.
            public bool TimeoutEnable
            {
                get; set;
                // NI Digital does not support this property.
            }

            // Methods

            // Method to clear the pattern generator fail count.
            public void ClearFailCount()
            {
                // NI Digital does not support this function.
            }

            // Method to clear the pattern generator read code.
            public void ClearReadCode()
            {
                // NI Digital does not support this function.
            }

            // Sets the states of the CPU flags.
            public void Continue(long flagsSet, long flagsClear)
            {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
                /* Implementation */
                int[] CpuFlagBit = new int[] { 1, 2, 4, 8 };

                var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

                for (int i = 0; i < CpuFlagBit.Length; i++)
                {
                    // If the cpu flag is in the number Flags then it will return a one.
                    // Otherwise a zero will be returned.
                    long FlagAnswer = flagsSet & CpuFlagBit[i];
                    if (FlagAnswer != 0)
                    {
                        // Set the sequencer flag to true
                        sessions.WriteSequencerFlag("seqflag" + Math.Log(FlagAnswer, 2), true);
                    }
                }

                for (int i = 0; i < CpuFlagBit.Length; i++)
                {
                    // If the cpu flag is in the number Flags then it will return a one.
                    // Otherwise a zero will be returned.
                    long FlagAnswer = flagsClear & CpuFlagBit[i];
                    if (FlagAnswer != 0)
                    {
                        // Set the sequencer flag to false
                        sessions.WriteSequencerFlag("seqflag" + Math.Log(FlagAnswer, 2), false);
                    }
                }
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
            }

            // Gets the pattern generator event loop enable and count.
            public void EventGetLoopCount(out bool retEnable, out long retCount) { /* Implementation */ retEnable = false; retCount = 0; }

            // Gets the pattern generator's vector event setup.
            public void EventGetVector(out bool retEnable, out string retPatname, out string retLabel, out long retOffset) { /* Implementation */ retEnable = false; retPatname = ""; retLabel = ""; retOffset = 0; }

            // Sets the pattern generator event loop enable and count.
            public void EventSetLoopCount(bool enable, long count) { /* Implementation */ }

            // Sets the pattern generator vector event.
            public void EventSetVector(bool enable, string patName, string label, long offset) { /* Implementation */ }

            // Waits for the pattern generator flags to reach a state.
            public void FlagWait(long waitFlagsTrue, long waitFlagsFalse) { /* Implementation */ }

            // Gets the pattern generator's FlagMatch setup.
            public void GetFlagMatch(out bool retEnable, out long retFlagsTrue, out long retFlagsFalse, out bool retMatchAllSites) { /* Implementation */ retEnable = false; retFlagsTrue = 0; retFlagsFalse = 0; retMatchAllSites = false; }

            // Gets the global address register as set by SetGlobalAddr.
            public void GetGlobalAddr(out string retPat, out string retLabel) { /* Implementation */ retPat = ""; retLabel = ""; }

            // Method to stop the pattern generator unconditionally. After this call, neither a pattern nor keepalive is running.
            public void Halt(InstrumentControl.Digital sessions)
            {
                foreach (var ssc in sessions.SSC)
                {
                    // Stops the keep alive pattern if it is currently running. If a pattern burst is in progress, the method aborts the pattern burst.
                    ssc.Session.PatternControl.AbortKeepAlive();
                }
            }

            // Method to wait for the running pattern to halt.
            public void HaltWait()
            {
                // NI Digital does not support this function.
            }

            // Method to read the current pattern generator vector location.
            public void ReadCurrentVector()
            {
                // NI Digital does not support this function.
            }

            // Gets information about the most recently executed pattern or pattern group.
            public void ReadLastStart(out string retBurst, out bool retIsGroup, out string retLabel) { /* Implementation */ retBurst = ""; retIsGroup = false; retLabel = ""; }

            // Repeats the last pattern start.
            public void Restart(string startLabel = "", bool disableAlarmCheck = false, bool keepFailCount = false) { /* Implementation */ }

            // Sets up FlagMatch interpose feature.
            public void SetFlagMatch(bool enable, long flagsTrue, long flagsFalse, bool matchAllSites) { /* Implementation */ }

            // Sets the PatGen global address register.
            public void SetGlobalAddr(string patBurstObj, string label) { /* Implementation */ }

            // Enables or disables halting the pattern generator on a specified cycle.
            public bool StopOnCycleEnabled { get; set; }

            // Enables or disables pattern threading. Read/Write Boolean.
            public bool Threading { get; set; }
        }

        public class PatternsInterface
        {
            public static long[,] UnloadAll(MemType Mem)
            {
                if (Mem == MemType.memSvm)
                {
                    return new long[0, 0];//Modify later.
                }
                else
                {
                    return new long[0, 0];//Modify later.
                }
            }
            public _Pattern Pat(Pattern patName)
            {
                return new _Pattern(patName.Data.ToString());
            }
            public _Pattern Pat(PatternSet patName)
            {
                return new _Pattern(patName.Data.ToString());
            }
            public _Pattern Pat(string patName)
            {
                return new _Pattern(patName.ToString());
            }
            public class _Pattern
            {
                public static string _patName;
                private string pinList;

                public _Pattern(string patName)
                {
                    _patName = patName;
                }

                // Properties and Methods

                // Enables or disables extended validation for a pattern or pattern group.
                public void EnableExtendedValidation(bool enable)
                {
                    // Implementation
                }

                // Loads the pattern files into the tester.
                public void Load()
                {
                    // Implementation
                }

                // Loads the pattern files into a specific memory.
                public void LoadMem(MemType mem)
                {
                    // Implementation
                }

                // Modifies the pattern data in a block of vectors on the specified channels.
                public void ModifyChanVectorBlockData(string label, long offset, int[] chanNumArray, string[] dataBlock1)
                {
                    // Implementation
                }

                // Modifies the SCIO or MUX pattern data in a block of vectors on the specified channels.
                public void ModifyChanVectorBlockDataAll(string label, long offset, int[] chanNumArray, string[] dataBlock1, string[] dataBlock2)
                {
                    // Implementation
                }

                // Modifies the pattern data in a single vector on the specified channels.
                public void ModifyChanVectorData(string label, long offset, int[] chanNumArray, string dataStr)
                {
                    // Implementation
                }

                // Modifies the SCIO or MUX pattern data in a single vector on the specified channels.
                public void ModifyChanVectorDataAll(string label, long offset, int[] chanNumArray, string data1, string data2)
                {
                    // Implementation
                }

                // Modifies pattern data for a block of vectors on the specified pins at all sites.
                public void ModifyPinVectorBlockData(string label, long offset, string pinList, string[] dataBlock1)
                {
                    // Implementation
                }

                // Modifies SCIO or MUX pattern data for a block of vectors on the specified pins at all sites.
                public void ModifyPinVectorBlockDataAll(string label, long offset, string pinList, string[] dataBlock1, string[] dataBlock2)
                {
                    // Implementation
                }

                // Modifies pattern data for a block of vectors on the specified pins at the specified sites.
                public void ModifyPinVectorBlockDataNSite(string label, long offset, string pinList, string[] dataBlock1, long[] nSite)
                {
                    // Implementation
                }

                // Modifies SCIO or MUX pattern data for a block of vectors on the specified pins at the specified sites.
                public void ModifyPinVectorBlockDataNSiteAll(string label, long offset, string pinList, string[] dataBlock1, string[] dataBlock2, long[] nSite)
                {
                    // Implementation
                }

                // Modifies pattern data for a block of vectors on the specified pins at the specified site.
                public void ModifyPinVectorBlockDataSite(string label, long offset, string pinList, string[] dataBlock1, long site)
                {
                    // Implementation
                }

                // Modifies SCIO or MUX pattern data for a block of vectors on the specified pins at a single site.
                public void ModifyPinVectorBlockDataSiteAll(string label, long offset, string pinList, string[] dataBlock1, string[] dataBlock2, long site)
                {
                    // Implementation
                }

                // Modifies pattern data for a single vector on the specified pins at all sites.
                public void ModifyPinVectorData(string label, long offset, string pinList, string dataStr)
                {
                    // Implementation
                }

                // Modifies SCIO or MUX pattern data for a single vector on the specified pins at all sites.
                public void ModifyPinVectorDataAll(string label, long offset, string pinList, string data1, string data2)
                {
                    // Implementation
                }

                // Modifies pattern data for a single vector on the specified pins at the specified site.
                public void ModifyPinVectorDataSite(string label, long offset, string pinList, string dataStr, long site)
                {
                    // Implementation
                }

                // Modifies SCIO or MUX pattern data for a single vector on the specified pins at the specified site.
                public void ModifyPinVectorDataSiteAll(string label, long offset, string pinList, string data1, string data2, long site)
                {
                    // Implementation
                }

                // Modifies pattern scan data on the specified pins at all sites.
                public void ModifyPinVectorScanData(string label, long offset, string pinList, long startScanCycle, string dataStr)
                {
                    // Implementation
                }

                // Modifies pattern scan data on the specified pins at a single site.
                public void ModifyPinVectorScanDataSite(string label, long offset, string pinList, long startScanCycle, string dataStr, long site)
                {
                    // Implementation
                }

                // Modifies a pattern vector numeric operand.
                public void ModifyVectorOperand(string label, long offset, long operand)
                {
                    // Implementation
                }

                // Modifies a pattern vector tset name.
                public void ModifyVectorTset(string label, long offset, string tsetName)
                {
                    // Implementation
                }

                // Modifies a pattern vector tset number.
                public void ModifyVectorTsetNum(string label, long offset, long tsetNum)
                {
                    // Implementation
                }

                // Gets the full file specification of a pattern.
                public string _Path
                {
                    get
                    {
                        // Implementation
                        return "";
                    }
                }

                // Finds the global label name for vector number Vecnum.
                public string VectorLabel(long vecNum)
                {
                    // Implementation
                    return "";
                }

                // Reads the numeric operand for a vector.
                public long VectorOperand(string label, long offset)
                {
                    // Implementation
                    return 0;
                }

                // Reads the tset name for a vector.
                public string VectorTsetName(string label, long offset)
                {
                    // Implementation
                    return "";
                }

                // Reads the tset number for a vector.
                public long VectorTsetNum(string label, long offset)
                {
                    // Implementation
                    return 0;
                }

                // Starts a pattern burst at particular start label and wait for it.
                public void Run(string startLabel = "")
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation to start a pattern burst at the specified start label

                    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                    var digitalssc = sessions.SSC;
                    //Parallel.ForEach(digitalssc, SSC =>
                    if (startLabel == null || startLabel == "") { startLabel = _patName; }
                    Parallel.ForEach(digitalssc, SSC =>
                    {
                        SSC.Session.PatternControl.BurstPattern(SSC.SiteList, startLabel, selectDigitalFunction: false, waitUntilDone: true, maxTime: TimeSpan.MaxValue);
                    });
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }

                // Starts a pattern burst at a particular start label and immediately returns without waiting for the pattern to complete.
                public void Start(string startLabel = null)
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation
                    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                    var digitalssc = sessions.SSC;
                    //foreach (var Pin in Globals.PinGroupMap.Keys)
                    //Parallel.ForEach(digitalssc, SSC =>  
                    if (startLabel == null || startLabel == "") { startLabel = _patName; }
                    //foreach (var SSC in digitalssc)
                    //{
                    //    SSC.Session.PatternControl.BurstPattern(SSC.SiteList, startLabel, selectDigitalFunction: false, waitUntilDone: false, maxTime: TimeSpan.MinValue);
                    //}//);

                    Parallel.ForEach(digitalssc, SSC =>
                    {
                        SSC.Session.PatternControl.BurstPattern(SSC.SiteList, startLabel, selectDigitalFunction: false, waitUntilDone: false, maxTime: TimeSpan.MinValue);
                    });

#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }

                // Runs a pattern from a start to a stop label.
                public void StartStop(string startLabel, string stopLabel, bool disableAlarmCheck = false)
                {
                    // Implementation
                }

                // Executes a functional test and indicates the method of reporting results.
                public SiteDouble[] Test(pfType setPassFail, long stopOnFirstFail, string startLabel = "")
                {
#if TestTimeMeasure
                        Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation
                    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                    var digitalssc = sessions.SSC;

                    List<string> patterns = new List<string>();
                    List<string> labels = new List<string>();
                    int i = 0;

                    if (PatternData.PatternSetDictionary.TryGetValue(_patName, out List<PatternEntry> patternList))
                    {
                        foreach (PatternEntry pattern in patternList)
                        {
                            patterns.Add(Path.GetFileNameWithoutExtension(pattern.FileName));
                            labels.Add(pattern.StartLabel);
                        }
                    }
                    else if (string.IsNullOrEmpty(startLabel))
                    {
                        patterns.Add(_patName);
                    }
                    else
                    {
                        patterns.Add(startLabel);
                    }

                    SiteDouble[] passFail = SiteDouble.New(patternList.Count - 1);//SiteDouble Library is based on 0 index input from VBA.
                                                                                  //Hence it is always adding 1. So for default C# input we should subtract 1.
                                                                                  //bool[] passFailResults = new bool[patternList.Count];

                    i = 0;
                    switch (setPassFail)
                    {
                        case pfType.pfAlways://pfAlways: Always report results 
                            //foreach (var SSC in digitalssc)//This can be parallel
                            //{
                            //    int j = 0;
                            //    foreach (var pattern in patterns)//This is sequential, dont make this parallel
                            //    {
                            //        if (labels[j] != null)//If start label is present.
                            //        {
                            //            SSC.Session.PatternControl.StartLabel = labels[j];
                            //        }
                            //        SSC.Session.PatternControl.BurstPattern(SSC.SiteList, pattern, selectDigitalFunction: false, waitUntilDone: true, maxTime: Globals.TimeOut);
                            //        // Fetch the results on the fly
                            //        bool[] passFailResults = SSC.Session.PatternControl.GetSitePassFail(SSC.SiteList);
                            //        passFail[j].Value[i] = passFailResults[0] ? 1 : 0;
                            //        j++;
                            //    }
                            //    i++;
                            //}
                            //break;

                            Parallel.ForEach(digitalssc, SSC =>
                           {
                               int j = 0;
                               foreach (var pattern in patterns)//This is sequential, dont make this parallel
                               {
                                   if (labels[j] != null)//If start label is present.
                                   {
                                       SSC.Session.PatternControl.StartLabel = labels[j];
                                   }
                                   SSC.Session.PatternControl.BurstPattern(SSC.SiteList, pattern, selectDigitalFunction: false, waitUntilDone: true, maxTime: Globals.TimeOut);
                                   // Fetch the results on the fly
                                   bool[] passFailResults = SSC.Session.PatternControl.GetSitePassFail(SSC.SiteList);
                                   passFail[j].Value[SSC.Index] = passFailResults[0] ? 1 : 0;
                                   j++;
                               }
                               //i++;
                           });
                            break;
                        case pfType.pfFailsOnly://pfFailsOnly: Report failures only
                            foreach (var SSC in digitalssc)//This can be parallel
                            {
                                int j = 0;
                                foreach (var pattern in patterns)//This is sequential, dont make this parallel
                                {
                                    SSC.Session.PatternControl.BurstPattern(SSC.SiteList, startLabel, selectDigitalFunction: false, waitUntilDone: true, maxTime: Globals.TimeOut);
                                    // Fetch the results on the fly
                                    bool[] passFailResults = SSC.Session.PatternControl.GetSitePassFail(SSC.SiteList);
                                    // Convert pass/fail results to long array where pass (true) is 0L and fail (false) is 1L
                                    long[] failResults = passFailResults.Select(b => b ? 0L : 1L).ToArray();
                                    passFail[j].Value[i] = failResults[0];
                                    j++;
                                }
                                i++;
                            }
                            break;
                        case pfType.pfNever://pfNever: Never report results 
                            passFail = null;
                            break;
                        // Add other cases as needed
                        default:
                            throw new ArgumentOutOfRangeException(nameof(setPassFail), setPassFail, null);
                    }

                    for (i = 0; i < passFail.Length; i++)//This datalogging can be parallel
                    {
                        Globals.TheExec.Flow.TestLimit(resultval: passFail[i], ScaleType: null, unit: "FUNC", ForceResults: Globals.tlForceFlow);
                    }
#if TestTimeMeasure
                        Globals.TestTimeMeasure.TestTimeStop();
#endif
                    return passFail;
                }


                //public void Test(out SiteLong[] passFail, out bool TL_C_YES)//temporary implementation
                //{
                //    // Implementation
                //    // Implementation
                //    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                //    var digitalssc = sessions.SSC;
                //    bool[] passFailResults = new bool[sessions.SSC.Length];
                //    passFail = new SiteLong[sessions.SSC.Length];
                //    int i = 0;
                //    //foreach (var Pin in Globals.PinGroupMap.Keys)
                //    //Parallel.ForEach(digitalssc, SSC =>
                //    //if (startLabel == null || startLabel == "") { startLabel = this.patName; }
                //    foreach (var SSC in digitalssc)
                //    {
                //        SSC.Session.PatternControl.BurstPattern(SSC.SiteList, _patName, selectDigitalFunction: false, waitUntilDone: true, maxTime: Globals.TimeOut);
                //        // Fetch the results on the fly
                //        passFailResults = SSC.Session.PatternControl.GetSitePassFail(SSC.SiteList);
                //        passFail[i] = passFailResults.Select(b => b ? 1 : 0).ToArray();
                //        i++;
                //    }//);
                //    //return passFail;
                //    TL_C_YES = true;//temporary implementation
                //}

                // Executes a functional test, waiting for CPU flag match for each burst.
                public void TestFlagWait(pfType setPassFail, long stopOnFirstFail, long waitFlagsTrue, long waitFlagsFalse)
                {
                    // Implementation
                }

                // Unloads and removes patterns.
                public void Unload()
                {
                    // Implementation
                }

                // Unloads a specific pattern resource.
                public void UnloadMem(MemType mem)
                {
                    // Implementation
                }

                // Validates a pattern label.
                public long ValidateLabel(string startLabel, string stopLabel)
                {
                    // Implementation
                    return 0;
                }

                // Validates a list of patterns.
                public long ValidatePatlist()
                {
                    // Implementation
                    return 0;
                }

                // Validates a list of patterns to be used with pattern threading on.
                public long ValidateThreading()
                {
                    // Implementation
                    return 0;
                }
            }
        }

        public class TimingInterface
        {
            private Dictionary<TimeSetEdge, double> edgeTimings = new Dictionary<TimeSetEdge, double>();
            //{
            //            { TimeSetEdge.DriveOn, 10.0 },
            //            { TimeSetEdge.DriveData, 20.0 },
            //            { TimeSetEdge.DriveReturn, 30.0 },
            //            { TimeSetEdge.DriveOff, 40.0 },
            //            { TimeSetEdge.CompareStrobe, 50.0 },
            //            { TimeSetEdge.CompareStrobe2, 60.0 }
            //};

            // Method to read the working copy of the timing values from the hardware.
            public void ReadEdgeTimingRAM()
            {
                // NI Digital does not support this function.
            }

            // Method to write the working copy of the timing values to the hardware.
            public void WriteEdgeTimingRAM()
            {
                // NI Digital does not support this function.
            }

            //// Method to get an edge timing value.
            //// Use method instead of property as NI Digital needs pinName to get the sessions.
            ////public double GetEdgeTime(InstrumentControl.Digital sessions, string pinNameList, string time_set_name, TimeSetEdge edge)
            //public double GetEdgeTime(TimeSetEdge edge)
            //{
            //    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
            //    var digitalssc = sessions.SSC;
            //    foreach (var ssc in digitalssc)
            //    {
            //        // An exception will be thrown if the property value was different for pins in the list.
            //        double edgeTime = ssc.Session.Timing.GetTimeSet(Globals.timingSheetName).GetEdge(ssc.PinSet, edge).TotalSeconds;
            //        return edgeTime;
            //    }
            //    return 0;
            //}

            //// Method to set an edge timing value.
            //// Use method instead of property as NI Digital needs pinName to get the sessions.
            ////public void SetEdgeTime(InstrumentControl.Digital sessions, string pinNameList, string time_set_name, TimeSetEdge edge, double edgeTime)
            //public void SetEdgeTime(TimeSetEdge edge, double edgeTime)
            //{
            //    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
            //    var digitalssc = sessions.SSC;
            //    foreach (var ssc in digitalssc)
            //    {
            //        ssc.Session.Timing.GetTimeSet(Globals.timingSheetName).ConfigureEdge(ssc.PinSet, edge, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(edgeTime)));
            //    }
            //}

            //Method for ConfigureCompareEdgesStrobe() from NI-DigitalTimeSet class which covers Digital Configure Time Set Compare Edge
            public void ConfigureCompareEdgesStrobe(InstrumentControl.Digital sessions, string pinNameList, string time_set_name, double edgeTime)
            {
                // Implementation
                foreach (var ssc in sessions.SSC)
                {
                    ssc.Session.Timing.GetTimeSet(time_set_name).ConfigureCompareEdgesStrobe(pinNameList, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(edgeTime)));
                }
            }

            /// <summary>
            /// Gets or sets a timing value for the specified edge.
            /// This property works on a local copy of the actual hardware values.
            /// Use ReadEdgeTimingRAM to acquire the current hardware values prior to using EdgeTiming to read,
            /// or use WriteEdgeTimingRAM to transfer the modified local values to the hardware after using EdgeTiming to change a value.
            /// </summary>
            public class EdgeTimeIndexer
            {
                private readonly string _pinList;
                private readonly string _timeSetName;
                private readonly TimingInterface _timingInterface;

                public EdgeTimeIndexer(TimingInterface timingInterface)
                {
                    _timingInterface = timingInterface;
                    _pinList = PinsClass._pinList;
                    _timeSetName = PinsClass._timeSetName;
                }

                public double this[TimeSetEdge edge]
                {
                    get
                    {
                        //if (_timingInterface.edgeTimings.ContainsKey(edge))
                        //{
                        // Method to set an edge timing value.
                        var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                        var digitalssc = sessions.SSC;
                        foreach (var ssc in digitalssc)
                        {
                            // An exception will be thrown if the property value was different for pins in the list.
                            _timingInterface.edgeTimings[edge] = ssc.Session.Timing.GetTimeSet(_timeSetName).GetEdge(_pinList, edge).TotalSeconds;
                        }
                        return _timingInterface.edgeTimings[edge];
                        //}
                        //else
                        //{
                        //    throw new ArgumentException("Invalid edge specified.");
                        //}
                    }
                    set
                    {
                        //if (_timingInterface.edgeTimings.ContainsKey(edge))
                        //{
                        // Method to set an edge timing value.
                        var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                        var digitalssc = sessions.SSC;
                        foreach (var ssc in digitalssc)
                        {
                            ssc.Session.Timing.GetTimeSet(_timeSetName).ConfigureEdge(_pinList, edge, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Math.Abs(value)));
                        }
                        //}
                        //else
                        //{
                        //    throw new ArgumentException("Invalid edge specified.");
                        //}
                    }
                }
            }

            // Property that returns an instance of the nested class
            public EdgeTimeIndexer EdgeTime => new EdgeTimeIndexer(this);

            // Property to determine if the timing context has been dirtied since it was loaded into the test system hardware
            public bool Dirty { get; set; }

            // Method to disable a timing edge from occurring
            public void DisableEdge(ChEdge edge)
            {
                // Implementation to disable the specified edge
                HardwareAPI.DisableEdge(edge);
            }

            // Property to determine if a timing edge is currently enabled
            public bool EdgeEnabled(ChEdge edge)
            {
                // Implementation to check if the specified edge is enabled
                return HardwareAPI.IsEdgeEnabled(edge);
            }

            // Method to load timing and format registers from the active time set sheet
            public void Load(string sheet = null, string category = null, string selector = null, string edgeSheet = null)
            {
                // Implementation to load timing and format registers
                HardwareAPI.LoadTiming(sheet, category, selector, edgeSheet);
            }

            // Property to get a comma-separated list of time set names
            public string TimeSetNameList
            {
                get
                {
                    // Implementation to get time set names
                    return HardwareAPI.GetTimeSetNameList();
                }
            }

            // Property to get an array of the time set numbers
            public long[] TimeSetNumberList
            {
                get
                {
                    // Implementation to get time set numbers
                    return HardwareAPI.GetTimeSetNumberList();
                }
            }

            // Method to write the period and CPP for all channels based on time set name
            public void WritePeriodRAMByTSName(string name, double value, long cpp)
            {
                // Implementation to write period RAM by time set name
                HardwareAPI.WritePeriodRAMByTSName(name, value, cpp);
            }

            // Method to write the period and CPP for all channels based on time set number
            public void WritePeriodRAMByTSNumber(long ts, double value, long cpp)
            {
                // Implementation to write period RAM by time set number
                HardwareAPI.WritePeriodRAMByTSNumber(ts, value, cpp);
            }

            // Nested Pins class to handle pin-specific timing operations
            public class PinsClass
            {
                private readonly TimingInterface _timingInterface;
                public static string _pinList;
                public static string _timeSetName;

                public PinsClass(TimingInterface timingInterface, string pinList)
                {
                    _timingInterface = timingInterface;
                    _pinList = pinList;
                }

                public PinsClass(TimingInterface timingInterface, PinList pinList)
                {
                    _timingInterface = timingInterface;
                    _pinList = string.Join(",", pinList.GetAll());
                }

                public PinsClass(TimingInterface timingInterface, string[] pinList)
                {
                    _timingInterface = timingInterface;
                    _pinList = string.Join(",", pinList);
                }

                // Method to disable a timing edge for a list of pins in hardware
                public void DisableEdgeTimingRAM(string timeSetName, ChEdge edge)
                {
                    // Implementation to disable edge timing RAM for the specified pins
                    HardwareAPI.DisableEdgeTimingRAM(_pinList, timeSetName, edge);
                }

                // Method to modify the drive format, and optionally the pin setup, in hardware for the specified pin and time set
                public void ModifyDriveFormat(string timeSetName, ChDriveFormat driveFormat, int setupType = -1)
                {
                    // Implementation to modify drive format for the specified pins
                    HardwareAPI.ModifyDriveFormat(_pinList, timeSetName, driveFormat, setupType);
                }

                // Method to read the working copy of the timing values from the hardware for the specified pins
                public void ReadEdgeTimingRAM(string timeSetName)
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation to read edge timing RAM for the specified pins
                    _timeSetName = timeSetName;
                    HardwareAPI.ReadEdgeTimingRAM(_pinList, timeSetName);
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }

                // Method to get the actual clock period and clocks per period (CPP) factor from the hardware for the specified pins
                public void ReadPeriodRAM(long site, string timeSetName, out double period, out long cpp)
                {
                    // Implementation to read period RAM for the specified pins
                    HardwareAPI.ReadPeriodRAM(_pinList, site, timeSetName, out period, out cpp);
                }

                // Method to write the working copy of the timing values to the hardware for the specified pins
                public void WriteEdgeTimingRAM(string timeSetName)
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation to write edge timing RAM for the specified pins
                    HardwareAPI.WriteEdgeTimingRAM(_pinList, timeSetName);
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }

            // Overloaded method to return an instance of the PinsClass for a string pin list
            public PinsClass Pins(string pinList)
            {
                return new PinsClass(this, pinList);
            }

            // Overloaded method to return an instance of the PinsClass for a string pin list
            public PinsClass Pins(string[] pinList)
            {
                return new PinsClass(this, pinList);
            }

            // Overloaded method to return an instance of the PinsClass for a PinList
            public PinsClass Pins(PinList pinList)
            {
                return new PinsClass(this, pinList);
            }
        }

        // Hypothetical hardware API class
        public static class HardwareAPI
        {
            public static double GetEdgeTiming(ChEdge edge)
            {
                // Replace with actual hardware API call to get edge timing
                return 0.0; // Placeholder value
            }

            public static void SetEdgeTiming(ChEdge edge, double value)
            {
                // Replace with actual hardware API call to set edge timing
            }

            public static void DisableEdge(ChEdge edge)
            {
                // Replace with actual hardware API call to disable edge
            }

            public static bool IsEdgeEnabled(ChEdge edge)
            {
                // Replace with actual hardware API call to check if edge is enabled
                return true; // Placeholder value
            }

            public static void LoadTiming(string sheet, string category, string selector, string edgeSheet)
            {
                // Replace with actual hardware API call to load timing
            }

            public static string GetTimeSetNameList()
            {
                // Replace with actual hardware API call to get time set names
                return "TimeSet1,TimeSet2"; // Placeholder value
            }

            public static long[] GetTimeSetNumberList()
            {
                // Replace with actual hardware API call to get time set numbers
                return new long[] { 1, 2 }; // Placeholder value
            }

            public static void WritePeriodRAMByTSName(string name, double value, long cpp)
            {
                // Replace with actual hardware API call to write period RAM by time set name
            }

            public static void WritePeriodRAMByTSNumber(long ts, double value, long cpp)
            {
                // Replace with actual hardware API call to write period RAM by time set number
            }

            public static void DisableEdgeTimingRAM(string pinList, string timeSetName, ChEdge edge)
            {
                // Replace with actual hardware API call to disable edge timing RAM for the specified pins
            }

            public static void ModifyDriveFormat(string pinList, string timeSetName, ChDriveFormat driveFormat, int setupType)
            {
                // Replace with actual hardware API call to modify drive format for the specified pins
            }

            public static void ReadEdgeTimingRAM(string pinList, string timeSetName)
            {
                // Replace with actual hardware API call to read edge timing RAM for the specified pins

            }

            public static void ReadPeriodRAM(string pinList, long site, string timeSetName, out double period, out long cpp)
            {
                // Replace with actual hardware API call to read period RAM for the specified pins
                period = 0.0; // Placeholder value
                cpp = 0; // Placeholder value
            }

            public static void WriteEdgeTimingRAM(string pinList, string timeSetName)
            {
                var digPins = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, pinList);
                Parallel.ForEach(digPins.SSC, ssc => ssc.Session.Timing.GetTimeSet(timeSetName).ConfigureCompareEdgesStrobe(pinList, Ivi.Driver.PrecisionTimeSpan.FromSeconds(Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0])));
                // Replace with actual hardware API call to write edge timing RAM for the specified pins
            }
        }
    }

    public class _Pins
    {
        private TheHdw _theHdw;

        public _Pins(TheHdw theHdw)
        {
            _theHdw = theHdw;
        }

        public PinController this[string pinList]
        {
            get { return new PinController(_theHdw, pinList); }
        }

        public PinController Pins(PinList pinList)
        {
            return new PinController(_theHdw, string.Join(",", pinList.GetAll()));
        }

        public class PinController
        {
            private TheHdw _theHdw;
            private string[] _pinList;

            public PinController(TheHdw theHdw, string pinList)
            {
                _theHdw = theHdw;
                _pinList = pinList.Split(',');
                FailCount = new FailCountController(this);
            }

            public void ForceStaticLevel(int driveState, double? voltageLevel = null, string siteList = "", bool bIncludeOrExcludeSites = true)
            {
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStart();
#endif
                // Implementation to force static level for the specified pin list

                // Check whether need to operate for ForceStaticLevel
                if (bIncludeOrExcludeSites == false)
                {
                    // Read current site number
                    int siteNumber = (int)Globals.seqContext.AsPropertyObject().GetValNumber("RunState.TestSockets.MyIndex", 0) + 1;

                    // Check if current site contain in the siteLite
                    if (!siteList.Split(',').Select(s => s.Trim()).Contains(siteNumber.ToString()))
                    {
                        // if bIncludeOrExcludeSites is false, and the site number is not in the siteList string, then not operate for ForceStaticLevel
                        return;
                    }
                }

                var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, _pinList);

                switch (driveState)
                {
                    case DriveState.chStaticStateDisable:
                        sessions.WriteStatic(PinState.X);
                        break;
                    case DriveState.chStaticStateHi:
                        // Set voltageLevel if not null
                        if (voltageLevel != null)
                        {
                            //foreach (var ssc in sessions.SSC)
                            //{
                            //    ssc.PinSet.DigitalLevels.Vih = (double)voltageLevel;
                            //}
                            Parallel.ForEach(sessions.SSC, ssc =>   //run in parallel -adrian
                            {
                                ssc.PinSet.DigitalLevels.Vih = (double)voltageLevel;
                            });
                        }
                        sessions.WriteStatic(PinState._1);
                        break;
                    case DriveState.chStaticStateHiZ:
                        // Set voltageLevel if not null
                        if (voltageLevel != null)
                        {
                            //foreach (var ssc in sessions.SSC)
                            //{
                            //    ssc.PinSet.DigitalLevels.Vterm = (double)voltageLevel;
                            //}
                            Parallel.ForEach(sessions.SSC, ssc =>   //run in parallel -adrian
                            {
                                ssc.PinSet.DigitalLevels.Vterm = (double)voltageLevel;
                            });
                        }
                        sessions.WriteStatic(PinState.X);
                        break;
                    case DriveState.chStaticStateLo:
                        // Set voltageLevel if not null
                        if (voltageLevel != null)
                        {
                            //foreach (var ssc in sessions.SSC)
                            //{
                            //    ssc.PinSet.DigitalLevels.Vil = (double)voltageLevel;
                            //}
                            Parallel.ForEach(sessions.SSC, ssc =>   //run in parallel -adrian
                            {
                                ssc.PinSet.DigitalLevels.Vil = (double)voltageLevel;
                            });
                        }
                        sessions.WriteStatic(PinState._0);
                        break;
                    default:
                        throw new ArgumentException($"Invalid driveState value: {driveState}");
                }
#if TestTimeMeasure
                Globals.TestTimeMeasure.TestTimeStop();
#endif
            }

            public void ChanFromSite(int siteNum) { /* Implementation */ }
            public void DisableFails() { /* Implementation */ }
            public void DisableFailsAlways() { /* Implementation */ }
            public void DisableFailsTest() { /* Implementation */ }

            public FailCountController FailCount { get; private set; }

            public class FailCountController
            {
                private PinController _pinController;

                public FailCountController(PinController pinController)
                {
                    _pinController = pinController;
                    Value = new FailCountValueController(this);
                }

                public FailCountValueController Value { get; private set; }

                public class FailCountValueController
                {
                    private FailCountController _failCountController;

                    public FailCountValueController(FailCountController failCountController)
                    {
                        _failCountController = failCountController;
                    }

                    public int this[int siteNum]
                    {
                        get
                        {
                            return _failCountController._pinController.GetFailCount(siteNum);
                        }
                        set
                        {
                            _failCountController._pinController.SetFailCount(siteNum, value);
                        }
                    }
                }
            }

            private int GetFailCount(int siteNum)
            {
                // Implementation to get fail count for the specified site number

                var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, _pinList);
                long TotalFailCount = 0;

                foreach (var ssc in sessions.SSC)
                {
                    foreach (int siteNumber in ssc.SiteNumbers)
                    {
                        if (siteNumber == siteNum)
                        {
                            long[] PinSetFailCount = ssc.PinSet.GetFailCount();

                            TotalFailCount = TotalFailCount + PinSetFailCount[0];
                        }
                    }
                }
                return (int)TotalFailCount;
            }

            private void SetFailCount(int siteNum, int value)
            {
                // Implementation to set fail count for the specified site number
            }

            public void InitState() { /* Implementation */ }
            public void StartState() { /* Implementation */ }
            public bool SyncEnabled { get; set; }
            public bool SyncModeOn { get; set; }
            public void SyncPulseOff() { /* Implementation */ }
            public void SyncPulseOn() { /* Implementation */ }
        }
    }

    public class PinLevels
    {
        public PinController Pins(PinList pinList)
        {
            return new PinController(pinList);
        }

        public PinController Pins(string pin)
        {
            PinList singlePinList = new PinList();
            singlePinList.Add(pin);
            return Pins(singlePinList);
        }

        public class PinController
        {
            private PinList pinList;

            public PinController(PinList pinList)
            {
                this.pinList = pinList;
            }

            public void ModifyLevel(ChPinLevel level, double value)
            {
                // Implementation to set the specified level for the given pins
                Console.WriteLine($"Setting {level} to {value} for pins: {string.Join(", ", pinList.GetAll())}");
            }
        }
        public class DigiPinTypes
        {
            public string LevelSheetName { get; set; }
            public string TimingSheetName { get; set; }
            public double Vil { get; set; }
            public double Vih { get; set; }
            public double Vol { get; set; }
            public double Voh { get; set; }
            public double Iol { get; set; }
            public double Ioh { get; set; }
            public double Vt { get; set; }
            public double Vch { get; set; }
            public double Vcl { get; set; }
            public double Vph { get; set; }
            public double Iph { get; set; }
            public double Tpr { get; set; }
            public double DriverMode { get; set; }
        }
        public static void LoadPinLevels()
        {
            DigiPinTypes Levels = new DigiPinTypes();
            foreach (var pinGroup in Globals.PinGroupMap.Keys)
            {
                string[,] pinLevels = GetPinArray(pinGroup);
                int numRows = pinLevels.GetLength(0);

                for (int i = 0; i < numRows; i++)
                {
                    string levelName = pinLevels[i, 2];
                    string specName = pinLevels[i, 3];

                    PropertyInfo property = typeof(DigiPinTypes).GetProperty(levelName);
                    if (property != null && property.CanWrite)
                    {
                        double specValue = Specs.GetValue(specName);
                        property.SetValue(Levels, specValue);
                    }
                    else
                    {
                        Console.WriteLine($"Property '{levelName}' not found or is not writable.");
                    }
                }

                var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, pinGroup);
                DigiSession.ConfigureVoltgeLevels(Levels.Vil, Levels.Vih, Levels.Vol, Levels.Voh, Levels.Vt);
            }
        }
        public static string[,] GetPinArray(string pinGroup)
        {
            if (Globals.PinGroupMap.TryGetValue(pinGroup, out var pinArray))
            {
                return pinArray;
            }
            else
            {
                throw new ArgumentException("Invalid pin or pin group", nameof(pinGroup));
            }
        }
    }

    // Utility
    // The Utility object provides access to the VBT syntax used to program Utility Data Bits. 
    // For a list of VBT properties, methods, and interfaces used with the Utility object, see TheHdw.Utility. 
    public class Utility
    {
        // Properties and Methods:
        // Alarm
        // This property gets or sets the behaviors of the utility alarm. Read/Write tlAlarmBehavior. 
        public tlAlarmBehavior Alarm { get; set; }

        // Reset
        // This method resets all the utility bits. The reset state is to clear all utility bits and set the threshold level to 1.5 V. 
        public void Reset()
        {
            // Reset all utility data bit pins to the default threshold voltage:
            // All Utility pins now have a threshold value of 1.5 V
        }

        // SettlingTime
        // This property gets or sets the Settling Time flag for the utility bits. Read/Write Boolean. 
        public bool SettlingTime { get; set; }

        // Threshold
        // This property gets or sets the comparator threshold voltage value. Read/Write Double. 
        public double Threshold { get; set; }

        // Interfaces:
        public PinController Pins(PinList pinList)
        {
            return new PinController(pinList);
        }

        // PinController
        // Provides access to the utility pin language node. 
        public class PinController
        {
            private PinList pinList;

            public PinController(PinList pinList)
            {
                this.pinList = pinList;
            }

            // Alarm
            // This property gets the state of the alarms for the specified pins.
            public tlUtilityAlarm Alarm(tlUtilityAlarm ubAlarm)
            {
                // Implementation to get the state of the alarms for the specified pins.
                return tlUtilityAlarm.tlUtilityAlarmUb;
            }

            // State
            // This property turns the utility bits on and off. tlUtilBitState.
            public tlUtilBitState State
            {
                // This property sets the state of a utility bit. It does not read a utility bit state. To get utility bit states, see States.
                set
                {
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStart();
#endif
                    // Implementation to set the state of the utility bits.
                    string[] Relays = pinList.Value.ToArray();
                    var Connect = value == tlUtilBitState.tlUtilBitOn ? true : false;
                    Relay.ControlRelay(Globals.tsmContext, Relays, Connect);
#if TestTimeMeasure
                    Globals.TestTimeMeasure.TestTimeStop();
#endif
                }
            }

            // States
            // This property gets the programmed state of the utility bits.
            public PinListData States(tlUBState rState)
            {
                // Implementation to get the programmed state of the utility bits.
                return new PinListData();
            }
        }
    }

    // DIBAccess
    // The DIBAccess object provides access to the VBT syntax used to control the relays on a primary DIB access instrument.
    // For a list of VBT properties, methods, and interfaces used with the DIBAccess object, see TheHdw.DIBAccess. 
    public class DIBAccess
    {
        // Properties and Methods:

        // Interfaces:
        public PinController Pins(PinList pinList)
        {
            return new PinController(pinList);
        }

        // PinController
        // Provides access to the DIBAccess pin language node. 
        public class PinController
        {
            private PinList pinList;

            public PinController(PinList pinList)
            {
                this.pinList = pinList;
            }

            // Connect
            // This method connects the DIB access in the primary instrument.
            public void Connect()
            {
                // Implementation connects the DIB access.
                var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);
                string connectPin = pinList.ToString();

                // Connect each pin to which HMOD
                // Prepare data to burst in HMOD
                UInt32 HMODWaveformData = 0x0; //Data to burst in HMOD

                switch (connectPin)
                {
                    case "":
                        HMODWaveformData = 0x00000001;
                        break;
                    default:
                        throw new ArgumentException($"Invalid connectPin value: {connectPin}");
                }

                // Burst data to burst in HMOD
                foreach (var ssc in sessions.SSC)
                {
                    //disconnect all other MM connections first
                    control_HMOD(ssc.Session, 0);

                    //connect MM to MP
                    control_HMOD(ssc.Session, HMODWaveformData);
                }
            }

            // DisConnect 
            // This method connects the DIB access in the primary instrument.
            public void DisConnect()
            {
                // Implementation disconnects the DIB access.
                var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

                // Burst data to burst in HMOD
                foreach (var ssc in sessions.SSC)
                {
                    //disconnect all other MM connections first
                    control_HMOD(ssc.Session, 0);
                }
            }

            private void control_HMOD(NIDigital sessions, UInt32 WaveformData)
            {

                string waveformname = "hmod_32bit";
                string waveformfile = String.Concat(Directory.GetParent(Globals.tsmContext.DigitalPatternProjectSourceWaveformFilePaths.First()).ToString(), "\\hmod_32bit.tdms");
                string PatternStartLabel = "hmod_32bit";

                uint[] BroadcastWaveformData = { WaveformData };

                //source the waveformdata in src waveform file
                sessions.SourceWaveforms.CreateFromFile(waveformname, waveformfile, true);
                sessions.SourceWaveforms.WriteBroadcast(waveformname, BroadcastWaveformData);

                //burst pattern
                sessions.PatternControl.BurstPattern(string.Empty, PatternStartLabel, true, true, TimeSpan.FromSeconds(10));

                //cleanup
                sessions.PinAndChannelMap.GetPinSet(string.Empty).SelectedFunction = SelectedFunction.Disconnect;

            }
        }
    }
}


