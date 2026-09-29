using System;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    /// <summary>
    /// Power supply setup and cleanup for the GP3 translator board, covering both board revisions.
    /// Only <see cref="PowerSupply_Setup"/>, <see cref="DCSetup"/> and
    /// <see cref="PowerSupply_CleanUp"/> are public; they are the TestStand ProcessSetup and
    /// ProcessCleanup entry points. Everything else is an implementation detail.
    ///
    /// The two revisions generate the MFLEX +5V_1/2/3 DIB user supplies from completely different
    /// hardware, which is the reason this class is board-aware:
    ///
    /// 02-084291-01-c: LT1529CQ-5 LDOs (U12/U13/U14) whose VIN is driven by PXIe-4147 CH0, CH1 and
    /// CH2, so setup must force LDO headroom onto those channels. All four 4147 channels share one
    /// LO, so the rails cannot be stacked. CH3 is unused.
    ///
    /// 02-089357-01-a: isolated CCG15-24-05S DC/DC converters (M2/M3/M4) fed from the P102 AUX 24 V
    /// and P179 AUX 12 V rails. No SMU sits in that path and no relay can switch it, so the rails
    /// come up with the AUX supplies and nothing is forced. PXIe-4147 CH0-CH3 instead serve the BBAC
    /// SOURCE common-mode nodes (CH0, CH1) and BBAC CAPTURE ACC_CAP+/- (CH2, CH3), so they are
    /// parked at a safe idle here.
    /// </summary>
    public class Setup_CleanUp
    {
        /// <summary>
        /// Pin driving the -5 V XOR / -2 V output rail. Present on both board revisions.
        /// </summary>
        private const string Neg5VSupplyPin = "SMU4139_POS5V";

        /// <summary>
        /// Voltage applied to <see cref="Neg5VSupplyPin"/> to generate the -5 V XOR / -2 V output.
        /// </summary>
        private const double Neg5VSupplyVoltage = -6.5;

        /// <summary>
        /// Current limit applied to <see cref="Neg5VSupplyPin"/>, in amperes.
        /// </summary>
        private const double Neg5VSupplyCurrentLimit = 3;

        /// <summary>
        /// 02-084291 only. Pin group holding the three PXIe-4147 channels that drive the +5 V LDO
        /// inputs.
        /// </summary>
        private const string Pos5VSupplyPinGroup = "DIB_SMU4147_POS5V";

        /// <summary>
        /// 02-084291 only. Headroom voltage forced onto each LT1529CQ-5 VIN to regulate +5 V out.
        /// </summary>
        private const double Pos5VSupplyInputVoltage = 6.5;

        /// <summary>
        /// 02-084291 only. Current limit applied to each +5 V LDO input, in amperes.
        /// </summary>
        private const double Pos5VSupplyCurrentLimit = 2;

        /// <summary>
        /// 02-089357 only. Current limit the PXIe-4147 channels are parked at, in amperes. This
        /// matches the range the BBAC checkers configure, so no stale wide limit is left to
        /// conflict with it.
        /// </summary>
        private const double BbacPpmuIdleCurrentLimit = 10e-3;

        /// <summary>
        /// 02-089357 only. The four PXIe-4147 channels owned by the BBAC source and capture
        /// checkers. CH0 and CH1 drive the BBAC SOURCE CH1 / CH2 common-mode nodes; CH2 and CH3 are
        /// the ACC_CAP+/- PPMUs, shared by both capture channels.
        /// </summary>
        private static readonly string[] BbacPpmuPins = new string[]
        {
            "P103_4147_SMU_CH0",
            "P103_4147_SMU_CH1",
            "P103_4147_SMU_CH2",
            "P103_4147_SMU_CH3"
        };

        /// <summary>
        /// Identifies which translator board revision is installed.
        /// </summary>
        private enum BoardVariant
        {
            /// <summary>Translator board 02-084291-01-c.</summary>
            Board084291 = 0,

            /// <summary>Translator board 02-089357-01-a.</summary>
            Board089357 = 1
        }

        /// <summary>
        /// Brings up the power supplies for the installed translator board revision.
        /// Call this from the TestStand ProcessSetup sequence.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="boardVariant">
        /// Translator board revision installed. Bind this to FileGlobals.TestInformation.TranslatorBoard,
        /// a String containing either "084291" or "089357".
        /// </param>
        public static void PowerSupply_Setup(ISemiconductorModuleContext tsmContext, string boardVariant)
        {
            BoardVariant board = ParseBoardVariant(boardVariant);

            switch (board)
            {
                case BoardVariant.Board084291:
                    Setup084291(tsmContext);
                    break;
                case BoardVariant.Board089357:
                    Setup089357(tsmContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(board), board, "Unsupported translator board revision.");
            }
        }

        /// <summary>
        /// Configures the DC sources and the digital PPMU aperture.
        /// No 02-084291 versus 02-089357 difference has been identified for this step yet, so both
        /// revisions share one body. The board parameter is present so a divergence can be
        /// dispatched here without changing the TestStand step signature again.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="boardVariant">
        /// Translator board revision installed. Bind this to FileGlobals.TestInformation.TranslatorBoard,
        /// a String containing either "084291" or "089357". Validated here so a typo fails at the
        /// first setup step rather than silently later.
        /// </param>
        /// <param name="DCSense">Measurement sense mode applied to the ALLDC pin group.</param>
        /// <param name="VDD_ApertureTime">Aperture time applied to the ALLDC pin group, in seconds.</param>
        /// <param name="dCPowerSourceTransientResponse">Transient response applied to the ALLDC pin group.</param>
        public static void DCSetup(ISemiconductorModuleContext tsmContext,
            string boardVariant,
            DCPowerMeasurementSense DCSense,
            Double VDD_ApertureTime = .001,
            DCPowerSourceTransientResponse dCPowerSourceTransientResponse = DCPowerSourceTransientResponse.Normal)
        {
            ParseBoardVariant(boardVariant);

            DCPower AllDC = InstrCtrl.DCPowerPinsToSessions(tsmContext, "ALLDC");
            AllDC.ForceVoltage(0, 100e-3);
            Globals.TheHdw.Wait(3e-3);
            AllDC.ConfigureOutputEnabled(false);
            AllDC.ConfigureSense(DCSense, true);

            AllDC.ConfigureSettings(apertureTime: VDD_ApertureTime, initiateSessionAfter: true, transientResponse: dCPowerSourceTransientResponse);

            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, Globals.AllDigitalPins);

            sessions.Abort();
            sessions.PPMUConfigureApertureTime(0.000004);   //set to lowest aperture time
        }

        /// <summary>
        /// Takes the power supplies back down for the installed translator board revision.
        /// Call this from the TestStand ProcessCleanup sequence.
        ///
        /// Unlike setup, this method never refuses to run on a bad board revision string. Cleanup
        /// may be reached before the Get Test Settings step has populated the FileGlobal, or after
        /// a failure anywhere earlier in the run, and throwing here would leave the AUX 24 V / 12 V
        /// / 48 V rails and the -5 V supply energized while masking the original failure. An
        /// unrecognized value therefore falls back to a board-agnostic teardown that de-energizes
        /// everything both revisions have in common.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="boardVariant">
        /// Translator board revision installed. Bind this to FileGlobals.TestInformation.TranslatorBoard,
        /// a String containing either "084291" or "089357".
        /// </param>
        public static void PowerSupply_CleanUp(ISemiconductorModuleContext tsmContext, string boardVariant)
        {
            BoardVariant board;

            if (!TryParseBoardVariant(boardVariant, out board))
            {
                CleanUpUnknownBoard(tsmContext);
                return;
            }

            switch (board)
            {
                case BoardVariant.Board084291:
                    CleanUp084291(tsmContext);
                    break;
                case BoardVariant.Board089357:
                    CleanUp089357(tsmContext);
                    break;
                default:
                    CleanUpUnknownBoard(tsmContext);
                    break;
            }
        }

        /// <summary>
        /// 02-084291 setup: brings up the load board supplies and the three MFLEX +5 V DIB user
        /// supplies by forcing LDO headroom onto PXIe-4147 CH0-CH2.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void Setup084291(ISemiconductorModuleContext tsmContext)
        {
            DCPower pos5VSupplies = InstrCtrl.DCPowerPinsToSessions(tsmContext, Pos5VSupplyPinGroup);

            pos5VSupplies.ConfigureSense();
            Globals.TheHdw.Wait(1 * Globals.mS);

            // All three channels share the single PXIe-4147 LO, so the rails cannot be stacked.
            pos5VSupplies.ForceVoltage(Pos5VSupplyInputVoltage, Pos5VSupplyCurrentLimit);

            EnableNeg5VSupply(tsmContext);
            SetLoadBoardSupplies(tsmContext, true);
        }

        /// <summary>
        /// 02-084291 cleanup: takes the MFLEX +5 V DIB user supplies and the load board supplies
        /// back down.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void CleanUp084291(ISemiconductorModuleContext tsmContext)
        {
            DCPower pos5VSupplies = InstrCtrl.DCPowerPinsToSessions(tsmContext, Pos5VSupplyPinGroup);

            try
            {
                pos5VSupplies.ForceVoltage(0, Pos5VSupplyCurrentLimit);
                DisableNeg5VSupply(tsmContext);
            }
            finally
            {
                Globals.TheHdw.Wait(1 * Globals.mS);
                pos5VSupplies.ConfigureSense(DCPowerMeasurementSense.Local);
                SetLoadBoardSupplies(tsmContext, false);
            }
        }

        /// <summary>
        /// 02-089357 setup: brings up the load board supplies. The MFLEX +5 V DIB user supplies
        /// follow the P102 AUX 24 V and P179 AUX 12 V rails and need no explicit forcing here.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void Setup089357(ISemiconductorModuleContext tsmContext)
        {
            EnableNeg5VSupply(tsmContext);
            SetLoadBoardSupplies(tsmContext, true);

            Park4147Channels(tsmContext);
        }

        /// <summary>
        /// 02-089357 cleanup: takes the load board supplies down and returns the BBAC PXIe-4147
        /// channels to idle.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void CleanUp089357(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                Park4147Channels(tsmContext);
                DisableNeg5VSupply(tsmContext);
            }
            finally
            {
                SetLoadBoardSupplies(tsmContext, false);
            }
        }

        /// <summary>
        /// Parks all four PXIe-4147 channels at 0 V with a 10 mA limit and the output both disabled
        /// and disconnected. Abort runs first, so narrowing the limit range cannot trip a channel
        /// that was still sourcing.
        ///
        /// Safe on either revision: on 02-089357 it leaves the BBAC checkers to configure the
        /// channels themselves, and on 02-084291 CH0-CH2 are the +5 V LDO inputs, so parking them
        /// at 0 V is exactly what cleanup wants.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void Park4147Channels(ISemiconductorModuleContext tsmContext)
        {
            DCPower ppmus = InstrCtrl.DCPowerPinsToSessions(tsmContext, BbacPpmuPins);

            ppmus.Abort();
            ppmus.ConfigureSense(DCPowerMeasurementSense.Local, initiateSessionAfter: false);
            ppmus.ConfigureCurrentLimitRange(BbacPpmuIdleCurrentLimit);
            ppmus.ForceVoltage(0, BbacPpmuIdleCurrentLimit, initiateSessionAfter: false);
            ppmus.ConfigureOutputEnabled(false);
            ppmus.ConfigureOutputConnected(false);
        }

        /// <summary>
        /// Board-agnostic teardown used when the board revision is unknown. Parks the PXIe-4147
        /// channels, drops the -5 V rail and opens every load board supply. Each stage is in its own
        /// try/finally so a fault in one still lets the remaining rails come down, and the load
        /// board supplies go last because they feed the rest.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void CleanUpUnknownBoard(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                try
                {
                    Park4147Channels(tsmContext);
                }
                finally
                {
                    DisableNeg5VSupply(tsmContext);
                }
            }
            finally
            {
                SetLoadBoardSupplies(tsmContext, false);
            }
        }

        /// <summary>
        /// Brings up the -5 V XOR / -2 V output rail. Identical on both board revisions.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void EnableNeg5VSupply(ISemiconductorModuleContext tsmContext)
        {
            DCPower supply = InstrCtrl.DCPowerPinsToSessions(tsmContext, Neg5VSupplyPin);

            supply.ConfigureSense();
            Globals.TheHdw.Wait(1 * Globals.mS);
            supply.ForceVoltage(Neg5VSupplyVoltage, Neg5VSupplyCurrentLimit);
        }

        /// <summary>
        /// Returns the -5 V XOR / -2 V output rail to 0 V and restores local sense.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        private static void DisableNeg5VSupply(ISemiconductorModuleContext tsmContext)
        {
            DCPower supply = InstrCtrl.DCPowerPinsToSessions(tsmContext, Neg5VSupplyPin);

            try
            {
                supply.ForceVoltage(0, Neg5VSupplyCurrentLimit);
            }
            finally
            {
                Globals.TheHdw.Wait(1 * Globals.mS);
                supply.ConfigureSense(DCPowerMeasurementSense.Local);
            }
        }

        /// <summary>
        /// Enables or disables the system and testhead fixed-voltage load board supplies.
        /// Both board revisions need the same set: the P143 system supplies for the HMOD +5 V,
        /// TFE +5 V and comparator -5.2 V regulators, and the P102 AUX 24 V / P179 AUX 12 V and
        /// 48 V rails. On 02-089357 the AUX 24 V and 12 V rails additionally feed the M2/M3/M4
        /// DC/DC converters that generate the MFLEX +5 V DIB user supplies.
        /// </summary>
        /// <param name="tsmContext">Test Stand Semiconductor Module Context.</param>
        /// <param name="enable">True to bring the supplies up, false to take them down.</param>
        private static void SetLoadBoardSupplies(ISemiconductorModuleContext tsmContext, bool enable)
        {
            LoadBoardCtrl.EnableLoadBoardSupplies(
                tsmContext,
                enablePos6v: enable,                // HMOD +5V
                enablePos20v: enable,               // TFE +5V
                enableNeg20v: enable,               // Comparator -5.2V
                enablePos12vAtP143: false,
                enablePos12vAtP179: enable,         // AuxPs1/2, +12V @ P179: on 02-089357 makes +5V_3
                enablePos24vAtP102Ch0: enable,      // AuxPs1/0, +24V @ P102: on 02-089357 makes +5V_1
                enablePos24vAtP102Ch1: enable,      // AuxPs1/1, +24V @ P102: on 02-089357 makes +5V_2
                enablePos48vAtP179: enable,         // AuxPs1/3, +48V @ P179: on 02-089357 makes +12V
                enablePos48vAtP143: false);
        }

        /// <summary>
        /// Resolves the board revision string supplied by TestStand into a <see cref="BoardVariant"/>.
        /// Matching is by drawing number anywhere in the string, so "089357", "02-089357-01-a" and
        /// "Board089357" are all accepted. An unrecognized or empty value throws rather than
        /// defaulting, because guessing the wrong revision would force the PXIe-4147 onto the BBAC
        /// nets of 02-089357 or leave the +5 V DIB rails dead on 02-084291.
        /// </summary>
        /// <param name="boardVariant">
        /// Board revision string, normally FileGlobals.TestInformation.TranslatorBoard.
        /// </param>
        /// <returns>The matching board variant.</returns>
        private static BoardVariant ParseBoardVariant(string boardVariant)
        {
            BoardVariant board;

            if (TryParseBoardVariant(boardVariant, out board))
            {
                return board;
            }

            if (string.IsNullOrWhiteSpace(boardVariant))
            {
                throw new ArgumentException(
                    "Board variant is empty. Check that the Get Test Settings step runs before this "
                    + "one and that the load file configuration sets TranslatorBoard to \"084291\" "
                    + "or \"089357\".",
                    nameof(boardVariant));
            }

            throw new ArgumentException(
                "Unrecognized board variant \"" + boardVariant + "\". Expected a string containing "
                + "\"084291\" for translator board 02-084291-01-c or \"089357\" for 02-089357-01-a.",
                nameof(boardVariant));
        }

        /// <summary>
        /// Non-throwing form of <see cref="ParseBoardVariant"/>, for cleanup paths that must keep
        /// de-energizing hardware even when the revision cannot be determined.
        /// </summary>
        /// <param name="boardVariant">Board revision string, which may be null or empty.</param>
        /// <param name="board">The matching board variant, or the default when unrecognized.</param>
        /// <returns>True when the string named a known revision.</returns>
        private static bool TryParseBoardVariant(string boardVariant, out BoardVariant board)
        {
            board = default(BoardVariant);

            if (string.IsNullOrWhiteSpace(boardVariant))
            {
                return false;
            }

            string normalized = boardVariant.Trim();

            if (normalized.Contains("084291"))
            {
                board = BoardVariant.Board084291;
                return true;
            }

            if (normalized.Contains("089357"))
            {
                board = BoardVariant.Board089357;
                return true;
            }

            return false;
        }
    }
}
