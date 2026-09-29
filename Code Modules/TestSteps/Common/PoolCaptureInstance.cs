using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using Digital = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Digital;
using Scope = NationalInstruments.TestStand.SemiconductorModule.InstrumentControl.Scope;

namespace TestSteps.Common
{
    /// <summary>
    /// Translator board HMOD shift register holding a POOL relay.
    /// </summary>
    public enum PoolRegister
    {
        /// <summary>HMOD11, relays K1 to K32.</summary>
        Hmod11Reg = 11,
        /// <summary>HMOD12, relays K1 to K32.</summary>
        Hmod12Reg = 12,
        /// <summary>HMOD13, relays K1 to K32.</summary>
        Hmod13Reg = 13
    }

    /// <summary>
    /// One relay, identified by its HMOD register and K number within that register.
    /// The three static factories exist so the per-slot relay tables read as literal
    /// tables, for example Hmod11(28), Hmod12(1).
    /// </summary>
    public struct PoolRelay
    {
        private PoolRelay(PoolRegister register, int number)
        {
            if (number < 1 || number > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(number), number, "K number must be 1..32.");
            }

            Register = register;
            Number = number;
        }

        /// <summary>The HMOD register holding the relay.</summary>
        public PoolRegister Register { get; }

        /// <summary>K number within the register, 1 to 32.</summary>
        public int Number { get; }

        /// <summary>A relay on HMOD11.</summary>
        /// <param name="number">K number, 1 to 32.</param>
        public static PoolRelay Hmod11(int number)
        {
            return new PoolRelay(PoolRegister.Hmod11Reg, number);
        }

        /// <summary>A relay on HMOD12.</summary>
        /// <param name="number">K number, 1 to 32.</param>
        public static PoolRelay Hmod12(int number)
        {
            return new PoolRelay(PoolRegister.Hmod12Reg, number);
        }

        /// <summary>A relay on HMOD13.</summary>
        /// <param name="number">K number, 1 to 32.</param>
        public static PoolRelay Hmod13(int number)
        {
            return new PoolRelay(PoolRegister.Hmod13Reg, number);
        }

        /// <summary>Returns a readable identifier such as "HMOD13 K9".</summary>
        public override string ToString()
        {
            return "HMOD" + (int)Register + " K" + Number;
        }
    }

    /// <summary>
    /// Accumulates POOL relay closures and writes every register in a single burst.
    ///
    /// HMOD writes are full chain overwrites, not read modify write: HMOD11to13 always
    /// shifts all three registers and any omitted argument defaults to 0, which opens
    /// every relay in that register. Relays that must be closed together therefore have
    /// to be present in one Apply. Accumulating here and applying once makes that
    /// structural rather than a convention someone has to remember.
    /// </summary>
    public class PoolHmodState
    {
        private readonly SortedSet<int> _hmod11 = new SortedSet<int>();
        private readonly SortedSet<int> _hmod12 = new SortedSet<int>();
        private readonly SortedSet<int> _hmod13 = new SortedSet<int>();
        private readonly SortedSet<int> _chmod6 = new SortedSet<int>();

        /// <summary>Marks translator board relays to be closed.</summary>
        /// <param name="relays">Relays to close.</param>
        public PoolHmodState Close(params PoolRelay[] relays)
        {
            foreach (PoolRelay relay in relays)
            {
                RegisterSet(relay.Register).Add(relay.Number);
            }

            return this;
        }

        /// <summary>Marks checker board CHMOD6 relays to be closed.</summary>
        /// <param name="relayNumbers">K numbers within CHMOD6, 1 to 72.</param>
        public PoolHmodState CloseChecker(params int[] relayNumbers)
        {
            foreach (int relayNumber in relayNumbers)
            {
                // HMODControl.RelayID72 silently discards out of range numbers, which would
                // leave a relay open with no error, so range check here instead.
                if (relayNumber < 1 || relayNumber > 72)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(relayNumbers), relayNumber, "CHMOD6 K number must be 1..72.");
                }

                _chmod6.Add(relayNumber);
            }

            return this;
        }

        private SortedSet<int> RegisterSet(PoolRegister register)
        {
            switch (register)
            {
                case PoolRegister.Hmod11Reg:
                    return _hmod11;
                case PoolRegister.Hmod12Reg:
                    return _hmod12;
                case PoolRegister.Hmod13Reg:
                    return _hmod13;
                default:
                    throw new ArgumentOutOfRangeException(nameof(register));
            }
        }

        private static string Format(SortedSet<int> relayNumbers)
        {
            return string.Join(", ", relayNumbers.Select(k => "K" + k));
        }

        /// <summary>
        /// Writes CHMOD6 and then HMOD11, HMOD12 and HMOD13. The translator board registers
        /// go out in one call because they form a single shift register chain.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public void Apply(ISemiconductorModuleContext tsmContext)
        {
            HMODControl.CHMOD1to6(tsmContext,
                HMOD_Data_6: HMODControl.RelayID72(Format(_chmod6)));

            HMODControl.HMOD11to13(tsmContext,
                HMOD_Data_11: HMODControl.RelayID(Format(_hmod11)),
                HMOD_Data_12: HMODControl.RelayID(Format(_hmod12)),
                HMOD_Data_13: HMODControl.RelayID(Format(_hmod13)));
        }

        /// <summary>
        /// Returns the accumulated closures, for debug logging. Log this before Apply and a
        /// failing measurement tells you exactly which relays were asked for.
        /// </summary>
        public override string ToString()
        {
            return "HMOD11[" + Format(_hmod11) + "] HMOD12[" + Format(_hmod12) +
                   "] HMOD13[" + Format(_hmod13) + "] CHMOD6[" + Format(_chmod6) + "]";
        }
    }

    /// <summary>
    /// Analytical RC model of the POOL PL OUT rise time path, shared by all three slots
    /// because every slot uses an identical 1 kohm / 1 uF network into an identical TFE.
    /// Kept in one place so a correction cannot be applied to one slot and missed on the
    /// other two.
    ///
    /// Derivation, matching Reports/RC_Charge_Times.xlsx. The checker board drives a 1 kohm
    /// series resistor into 1 uF, and the TFE "10 kohm" input option loads that node with
    /// its own 10 kohm input impedance while halving the signal reaching the comparator:
    ///   R in     = R64 15K || (R55 15K + R66 15K)   = 10 kohm
    ///   R eff    = 1 kohm || 10 kohm                = 909.0909 ohm
    ///   tau      = R eff * 1 uF                     = 909.0909 us
    ///   V node   = 5 V * 10k / (1k + 10k)           = 4.5455 V
    ///   V full   = V node * R66 / (R55 + R66)       = 2.2727 V at the comparator
    ///
    /// Thresholds are therefore percentages of 2.2727 V, not of 5 V, and each expected
    /// width is tau * ln((1 - lo) / (1 - hi)).
    ///
    /// Only the 10 kohm input option is usable for rise time. The two 50 ohm options load
    /// the node to about 47.6 us and leave the comparator under 0.25 V full scale, which
    /// the window comparator cannot resolve.
    /// </summary>
    public static class PoolRcModel
    {
        /// <summary>Settled voltage at the window comparator input, in volts.</summary>
        public const double ComparatorFullScaleVolts = 2.2727272727272729;

        /// <summary>Loaded RC time constant, in seconds.</summary>
        public const double TauLoadedSec = 909.0909090909091e-6;

        /// <summary>One low/high threshold percentage pair and its expected pulse width.</summary>
        public struct ThresholdPair
        {
            internal ThresholdPair(string name, double lowFraction, double highFraction)
            {
                Name = name;
                LowFraction = lowFraction;
                HighFraction = highFraction;
            }

            /// <summary>Suffix used in the published result name, such as "10_90".</summary>
            public string Name { get; }

            /// <summary>Lower threshold as a fraction of the settled voltage.</summary>
            public double LowFraction { get; }

            /// <summary>Upper threshold as a fraction of the settled voltage.</summary>
            public double HighFraction { get; }

            /// <summary>VOL setpoint in volts.</summary>
            public double LowVolts
            {
                get { return LowFraction * ComparatorFullScaleVolts; }
            }

            /// <summary>VOH setpoint in volts.</summary>
            public double HighVolts
            {
                get { return HighFraction * ComparatorFullScaleVolts; }
            }

            /// <summary>Expected pulse width in seconds, tau * ln((1 - lo) / (1 - hi)).</summary>
            public double ExpectedWidthSec
            {
                get { return TauLoadedSec * Math.Log((1.0 - LowFraction) / (1.0 - HighFraction)); }
            }
        }

        /// <summary>
        /// The three threshold pairs, measured in this order. Expected widths are
        /// 1997.477 us, 1260.268 us and 770.271 us.
        /// </summary>
        public static readonly ThresholdPair[] Pairs =
        {
            new ThresholdPair("10_90", 0.10, 0.90),
            new ThresholdPair("20_80", 0.20, 0.80),
            new ThresholdPair("30_70", 0.30, 0.70)
        };

        /// <summary>
        /// Expected (90-10)/(80-20) width ratio, ln9 / ln4 = 1.5849625. This and
        /// <see cref="ExpectedRatio8020Over7030"/> depend only on the threshold
        /// percentages, so they are independent of tau, of the divider ratio and of every
        /// component tolerance. They are the tolerance immune part of the check, while the
        /// absolute widths additionally confirm the resistor and capacitor values.
        /// </summary>
        public static double ExpectedRatio9010Over8020
        {
            get { return Pairs[0].ExpectedWidthSec / Pairs[1].ExpectedWidthSec; }
        }

        /// <summary>Expected (80-20)/(70-30) width ratio, ln4 / ln(7/3) = 1.6361358.</summary>
        public static double ExpectedRatio8020Over7030
        {
            get { return Pairs[1].ExpectedWidthSec / Pairs[2].ExpectedWidthSec; }
        }
    }

    /// <summary>
    /// Selects which instrument captures the TFE window comparator output.
    /// </summary>
    public enum PoolCaptureType
    {
        /// <summary>PXIe-6571 digital capture, via the output changeover's NC branch.</summary>
        Digital6571 = 0,
        /// <summary>PXIe-5172 scope, via the output changeover's NO branch.</summary>
        Scope5172 = 1
    }

    /// <summary>
    /// Strategy interface for stepping the RC network and timing the resulting XOR pulse.
    ///
    /// The caller owns the relay table and passes the three output relays in, so all
    /// per-slot and per-channel relay knowledge stays in the slot check file and this
    /// file stays instrument only.
    /// </summary>
    public interface IPoolCaptureStrategy
    {
        /// <summary>The capture type this strategy implements.</summary>
        PoolCaptureType Type { get; }

        /// <summary>Adds the relays needed to route the XOR output to this capture instrument.</summary>
        /// <param name="state">Relay state to add to.</param>
        /// <param name="changeover">The channel's output changeover relay.</param>
        /// <param name="scopeTap">The channel's relay to the 5172.</param>
        /// <param name="digitalTap">The channel's relay to the 6571.</param>
        void SelectOutputPath(PoolHmodState state, PoolRelay changeover, PoolRelay scopeTap, PoolRelay digitalTap);

        /// <summary>Opens the sessions and configures the capture instrument.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        void Configure(ISemiconductorModuleContext tsmContext, int channel);

        /// <summary>
        /// Discharges the RC node, steps it from 0 V to 5 V and returns the width of the
        /// comparator window crossing, per site, in seconds. Returns NaN when no complete
        /// pulse was captured, so a missing edge fails loudly instead of reporting 0.
        /// </summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="dischargeTimeSec">Time to hold the node low before stepping.</param>
        double[] StepAndMeasure(ISemiconductorModuleContext tsmContext, double dischargeTimeSec);

        /// <summary>Returns the drive pins low and aborts the sessions.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        void Cleanup(ISemiconductorModuleContext tsmContext);
    }

    /// <summary>
    /// Pins of the instruments that drive and capture the POOL rise time path.
    /// </summary>
    public static class PoolCapturePins
    {
        /// <summary>
        /// HSD200 step drive pins, one per slot in SL12, SL14, SL21 order. The pattern
        /// drives all three together; only the slot whose CHMOD6 input relay is closed
        /// reaches a buffer, so this is harmless and lets one pattern cover every slot.
        /// </summary>
        public static readonly string[] Drive =
        {
            "T_HSD200_SL11_CH5", "T_HSD200_SL11_CH6", "T_HSD200_SL11_CH7"
        };

        /// <summary>
        /// 6571 capture pins, in the order they must appear in the pattern's capture
        /// waveform. Capture samples pack one bit per captured pin in declaration order,
        /// so this order defines the bit positions the digital strategy unpacks.
        /// </summary>
        public static readonly string[] Dio =
        {
            "P154_6571_DIO_24", "P154_6571_DIO_25", "P154_6571_DIO_26", "P154_6571_DIO_27",
            "P154_6571_DIO_28", "P154_6571_DIO_29", "P154_6571_DIO_30", "P154_6571_DIO_31"
        };

        /// <summary>
        /// 5172 scope pins. The 5172 is declared in the pinmap as instrument
        /// SCOPE_5172_C1_S17 with 8 channels but has no DUTPin or Connection entries, so
        /// these names must be added to the pinmap before the scope path can resolve.
        /// </summary>
        public static readonly string[] Scope =
        {
            "P110_SCOPE_5172_CH0", "P110_SCOPE_5172_CH1", "P110_SCOPE_5172_CH2", "P110_SCOPE_5172_CH3",
            "P110_SCOPE_5172_CH4", "P110_SCOPE_5172_CH5", "P110_SCOPE_5172_CH6", "P110_SCOPE_5172_CH7"
        };
    }

    /// <summary>
    /// Capture strategy using the PXIe-6571 digital capture.
    ///
    /// A single pattern covers all 24 paths. It drives all three HSD200 pins together and
    /// captures all eight DIO pins; only the slot whose CHMOD6 input relay is closed
    /// reaches a buffer, and only the channel whose digital tap is closed drives a DIO pin,
    /// so the relay state alone selects the path under test.
    ///
    /// The XOR output is negative ECL: MC100EL07DR2G runs with VCC at AGND and VEE at
    /// -5V_XOR, terminated 50 ohm to -2 V, so it swings roughly -1.0 V high to -1.7 V low.
    /// The capture pin's compare levels must bracket that swing. The 6571 handles 6 V to
    /// -2 V, so this is reachable, but the shipped Rise_time_levels sheet still specifies
    /// Vol 0.8 V and Voh 2 V, against which the pin reads a constant low and every
    /// measurement returns NaN.
    /// </summary>
    public class PoolDigital6571Strategy : IPoolCaptureStrategy
    {
        // These four must agree with the compiled pattern and its levels/timing sheets.
        private const string PatternLabel = "rise_time_pattern";
        private const string CaptureWaveformName = "rise_time_capture_wfm";
        private const string LevelsSheet = "Rise_time_levels";
        private const string TimingSheet = "Rise_time_timing";

        // Vector period from the timing sheet, and the number of captured vectors between
        // capture_start and capture_stop. 6000 vectors at 1 us covers 6 ms, which is 6.6
        // time constants of the loaded 909 us RC, so the 90% crossing at 2.3 tau sits well
        // inside the record.
        private const double SamplePeriodSec = 1e-6;
        private const int MaxCaptureSamples = 6000;

        private const double CaptureTimeoutSec = 5.0;

        private Digital _drive;
        private Digital _pattern;
        private int _channel;

        /// <summary>The capture type this strategy implements.</summary>
        public PoolCaptureType Type
        {
            get { return PoolCaptureType.Digital6571; }
        }

        /// <summary>
        /// Closes the channel's digital tap. The changeover is left de-energised so it rests
        /// on NC, which is the 6571 branch.
        /// </summary>
        /// <param name="state">Relay state to add to.</param>
        /// <param name="changeover">The channel's output changeover relay, unused here.</param>
        /// <param name="scopeTap">The channel's relay to the 5172, unused here.</param>
        /// <param name="digitalTap">The channel's relay to the 6571.</param>
        public void SelectOutputPath(
            PoolHmodState state,
            PoolRelay changeover,
            PoolRelay scopeTap,
            PoolRelay digitalTap)
        {
            state.Close(digitalTap);
        }

        /// <summary>Opens the drive and pattern sessions and applies levels and timing.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public void Configure(ISemiconductorModuleContext tsmContext, int channel)
        {
            _channel = channel;

            var patternPins = new List<string>(PoolCapturePins.Drive);
            patternPins.AddRange(PoolCapturePins.Dio);

            // A separate drive-only session keeps WriteStatic off the capture pins, which
            // are high impedance inputs and must not be driven.
            _drive = InstrCtrl.DigitalPinsToSessions(tsmContext, PoolCapturePins.Drive);
            _pattern = InstrCtrl.DigitalPinsToSessions(tsmContext, patternPins.ToArray());

            _pattern.Abort();
            _pattern.SelectFunction(SelectedFunction.Digital);
            _pattern.ApplyLevelsandTimings(LevelsSheet, TimingSheet);
        }

        /// <summary>Discharges the RC node, bursts the pattern and returns the pulse width.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="dischargeTimeSec">Time to hold the node low before stepping.</param>
        public double[] StepAndMeasure(ISemiconductorModuleContext tsmContext, double dischargeTimeSec)
        {
            // The pattern's first vector only drives low for one period, far too short to
            // discharge 1 uF through 1 kohm, so hold it low here instead.
            _drive.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(dischargeTimeSec);

            _pattern.BurstPattern(
                PatternLabel,
                selectDigitalFunction: true,
                waitUntilDone: true,
                timeoutinSeconds: CaptureTimeoutSec);

            uint[][][] captureData = _pattern.FetchCaptureWaveform(
                CaptureWaveformName,
                samplesToRead: MaxCaptureSamples,
                timeoutInSeconds: CaptureTimeoutSec);

            uint[][] perSiteData = _pattern.PerInstrumentToPerSiteData(captureData);

            int bit = _channel - 1;
            var widths = new double[perSiteData.Length];
            for (int site = 0; site < perSiteData.Length; site++)
            {
                widths[site] = PulseWidth(perSiteData[site], bit);
            }

            return widths;
        }

        /// <summary>
        /// Width of the first complete high pulse on the channel's captured bit. Edge
        /// indices are differenced rather than counting every high sample, so a glitch
        /// elsewhere in the record cannot inflate the result.
        /// </summary>
        private static double PulseWidth(uint[] samples, int bit)
        {
            if (samples == null)
            {
                return double.NaN;
            }

            uint mask = 1u << bit;
            int firstHigh = -1;
            for (int i = 0; i < samples.Length; i++)
            {
                bool high = (samples[i] & mask) != 0;
                if (firstHigh < 0)
                {
                    if (high)
                    {
                        firstHigh = i;
                    }
                }
                else if (!high)
                {
                    return (i - firstHigh) * SamplePeriodSec;
                }
            }

            // Either the window was never entered or never left, so there is no measurable
            // crossing interval.
            return double.NaN;
        }

        /// <summary>Returns the drive pins low and aborts the pattern session.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            if (_drive != null)
            {
                _drive.WriteStatic(PinState._0);
            }

            if (_pattern != null)
            {
                _pattern.Abort();
            }
        }
    }

    /// <summary>
    /// Capture strategy using the PXIe-5172 scope.
    ///
    /// The scope digitises the ECL XOR output directly and the width is measured by
    /// threshold crossing at the midpoint of the ECL swing. No hardware trigger route is
    /// needed because the measurement is an interval, not a delay from the step, so the
    /// record only has to be long enough to contain the whole pulse.
    /// </summary>
    public class PoolScope5172Strategy : IPoolCaptureStrategy
    {
        // 1 Mohm so the scope does not load the XOR output. The TFE already terminates it
        // 50 ohm to -2 V, and a 50 ohm scope input terminates to ground, which would pull
        // the ECL levels. Bandwidth is irrelevant for millisecond pulses.
        private const double InputImpedanceOhms = 1e6;
        private const double InputFrequencyMaxHz = 100e6;

        // The ECL swing is about -1.0 V high to -1.7 V low, so centre the window on its
        // midpoint with a range wide enough to keep both rails on screen.
        private const double EclMidpointVolts = -1.35;
        private const double VerticalRangeVolts = 2.0;

        // 1 MS/s over 12000 points covers 12 ms, twice the digital record, so the pulse is
        // contained even without a trigger aligned to the step.
        private const double SampleRateHz = 1e6;
        private const int RecordLength = 12000;
        private const double SamplePeriodSec = 1.0 / SampleRateHz;

        private const double CaptureTimeoutSec = 5.0;

        private Digital _drive;
        private Scope _scope;

        /// <summary>The capture type this strategy implements.</summary>
        public PoolCaptureType Type
        {
            get { return PoolCaptureType.Scope5172; }
        }

        /// <summary>Energises the changeover onto its NO branch and closes the scope tap.</summary>
        /// <param name="state">Relay state to add to.</param>
        /// <param name="changeover">The channel's output changeover relay.</param>
        /// <param name="scopeTap">The channel's relay to the 5172.</param>
        /// <param name="digitalTap">The channel's relay to the 6571, unused here.</param>
        public void SelectOutputPath(
            PoolHmodState state,
            PoolRelay changeover,
            PoolRelay scopeTap,
            PoolRelay digitalTap)
        {
            state.Close(changeover, scopeTap);
        }

        /// <summary>Opens the drive session and configures the scope for one channel.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="channel">POOL channel, 1 to 8.</param>
        public void Configure(ISemiconductorModuleContext tsmContext, int channel)
        {
            _drive = InstrCtrl.DigitalPinsToSessions(tsmContext, PoolCapturePins.Drive);
            _drive.SelectFunction(SelectedFunction.Digital);

            _scope = InstrCtrl.ScopePinsToSessions(tsmContext, PoolCapturePins.Scope[channel - 1]);
            _scope.Abort();
            _scope.ConfigureChannelCharacteristics(InputImpedanceOhms, InputFrequencyMaxHz);
            _scope.ConfigureVertical(
                verticalRange: VerticalRangeVolts,
                verticalOffset: EclMidpointVolts,
                verticalCoupling: ScopeVerticalCoupling.DC,
                probeAttenuation: 1.0,
                enabled: true);
            _scope.ConfigureAcquisition(ScopeAcquisitionType.Normal);
            _scope.ConfigureHorizontalTiming(
                sampleRateMin: SampleRateHz,
                numberOfPointsMin: RecordLength,
                referencePosition: 0.0,
                numberOfRecords: 1,
                enforceRealtime: true);
        }

        /// <summary>Discharges the RC node, steps it and returns the pulse width.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        /// <param name="dischargeTimeSec">Time to hold the node low before stepping.</param>
        public double[] StepAndMeasure(ISemiconductorModuleContext tsmContext, double dischargeTimeSec)
        {
            _drive.WriteStatic(PinState._0);
            Globals.TheHdw.Wait(dischargeTimeSec);

            // Arm before stepping so the ramp cannot start before the record does.
            _scope.Initiate();
            _drive.WriteStatic(PinState._1);

            AnalogWaveformCollection<double>[] waveforms = _scope.Fetch(RecordLength, CaptureTimeoutSec);

            var widths = new double[waveforms.Length];
            for (int session = 0; session < waveforms.Length; session++)
            {
                widths[session] = waveforms[session] == null || waveforms[session].Count == 0
                    ? double.NaN
                    : PulseWidth(waveforms[session][0].GetScaledData());
            }

            return widths;
        }

        /// <summary>
        /// Width of the first complete excursion above the ECL midpoint. The XOR output is
        /// high while the ramp sits inside the comparator window, so this is the crossing
        /// interval.
        /// </summary>
        private static double PulseWidth(double[] samples)
        {
            if (samples == null)
            {
                return double.NaN;
            }

            int firstHigh = -1;
            for (int i = 0; i < samples.Length; i++)
            {
                bool high = samples[i] > EclMidpointVolts;
                if (firstHigh < 0)
                {
                    if (high)
                    {
                        firstHigh = i;
                    }
                }
                else if (!high)
                {
                    return (i - firstHigh) * SamplePeriodSec;
                }
            }

            return double.NaN;
        }

        /// <summary>Returns the drive pins low and aborts the scope.</summary>
        /// <param name="tsmContext">The semiconductor module context.</param>
        public void Cleanup(ISemiconductorModuleContext tsmContext)
        {
            if (_drive != null)
            {
                _drive.WriteStatic(PinState._0);
            }

            if (_scope != null)
            {
                _scope.Abort();
            }
        }
    }

    /// <summary>
    /// Creates capture strategy instances, mirroring the meter factory pattern.
    /// </summary>
    public static class PoolCaptureFactory
    {
        /// <summary>Creates a capture strategy for the given type.</summary>
        /// <param name="type">0 for the 6571 digital capture, 1 for the 5172 scope.</param>
        public static IPoolCaptureStrategy Create(PoolCaptureType type)
        {
            switch (type)
            {
                case PoolCaptureType.Digital6571:
                    return new PoolDigital6571Strategy();
                case PoolCaptureType.Scope5172:
                    return new PoolScope5172Strategy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
