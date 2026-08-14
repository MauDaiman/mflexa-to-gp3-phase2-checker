using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;
using NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex;
using System;
using System.Threading.Tasks;

namespace TestSteps
{
    class TestProgram
    {
        //OriginalCode: Attribute VB_Name = "Exec_IP_Module";
        //OriginalCode: Option Explicit;
        //PseudoCode  : UntranslatedVBA: Option Explicit;
        //***NOT CONVERTED***

        //OriginalCode: Public FIRST_RUN_ONLY As Boolean;
        //PseudoCode  : GlobalVariableDeclaration: Name: FIRST_RUN_ONLY; Type: Boolean;
        bool FIRST_RUN_ONLY;

        //***********************************************************

        //*  C:\\Users\\gmykulow\\Documents\\adum3190\\progen_01_00_03.c *

        //*                 Compile date Sep 21 2015                *

        //*                 Compile time 11:03:04                   *

        //*                      generated on                       *

        //*                 Wed Sep 30 11:39:57 2015

        //*                 Generating ADuM225                     *

        //*                                                         *

        //***********************************************************

        //This module contains empty Exec Interpose functions (see online help

        //for details).  These are here for convenience and are completely optional.

        //It is not necessary to delete them if they are not being used, nor is it

        //necessary that they exist in the program.

        //Immediately at the conclusion of the initialization process.

        //Do not program test system hardware from this function.


        //***********************************************************

        //*  C:\\Users\\gmykulow\\Documents\\adum3190\\progen_01_00_03.c *

        //*                 Compile date Sep 21 2015                *

        //*                 Compile time 11:03:04                   *

        //*                      generated on                       *

        //*                 Wed Sep 30 11:39:57 2015

        //*                 Generating ADuM225                     *

        //*                                                         *

        //***********************************************************

        //Test a string argument value to see if it's empty or not.

        //Return True if not empty, else False.

        //OriginalCode: Public Function NonBlank(ByVal ArgStr As String) As Boolean;
        //PseudoCode  : FunctionDeclaration: Name: NonBlank; ArgumentList: ByVal ArgStr As String; Type: Boolean;
        public static bool NonBlank(PinList input)
        {
            return input != null && input.Count > 0;
        }
        public static bool NonBlank(string argStr)
        {
            //OriginalCode: NonBlank = Len(Trim$(ArgStr)) <> 0;
            //PseudoCode  : Return: Expression: Len(Trim$(ArgStr)) <> 0;
            // Trim the input string and check if its length is not equal to 0
            return argStr.Trim().Length != 0;

            //OriginalCode: End Function;
            //PseudoCode  : FunctionEnd;
        }

        //IPAT implementation

        //OriginalCode: Public ipat As IPATManager;
        //PseudoCode  : GlobalVariableDeclaration: Name: ipat; Type: IPATManager;
        IPATManager ipat;

        //Location of your Statistical Limits Files Directory

        //Public Const SPAT_STAT_LIMITS_DIR = "\\\\prodserv\\supp\\spat_dir"

        ///********************************************************/

        //RunIPAPTests_t Function

        ///********************************************************/

        //OriginalCode: Public Function RunIPATTests_t() As Long;
        //PseudoCode  : FunctionDeclaration: Name: RunIPATTests_t; ArgumentList: None; Type: Long;
        public static void RunIPATTests_t()
        {
            //OriginalCode: ipat.RunIPATTests;
            //PseudoCode  : UntranslatedIGXL: ipat.RunIPATTests;
            //***NOT CONVERTED***
            Globals.ipat.RunIPATTests();

            //OriginalCode: End Function;
            //PseudoCode  : FunctionEnd;
        }

        //OriginalCode: Public Function PowerOn(VDD1_value As Double, idd1_value As Double, VDD2_value As Double, idd2_value As Double);
        //PseudoCode  : FunctionDeclaration: Name: PowerOn; ArgumentList: VDD1_value As Double, idd1_value As Double, VDD2_value As Double, idd2_value As Double; Type: void;
        static void PowerOn_Old(double VDD1_value, double idd1_value, double VDD2_value, double idd2_value)
        {
            //Set DUT supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .Mode = tlDCVIModeVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Mode = tlDCVIModeVoltage;
            Globals.TheHdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeVoltage;

            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //OriginalCode: .SetVoltageAndRange VDD1_value, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(VDD1_value, 10);
            Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(VDD1_value, 10);

            //OriginalCode: .SetCurrentAndRange idd1_value, idd1_value;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(idd1_value, idd1_value);
            Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(idd1_value, idd1_value);

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;

            //OriginalCode: .Meter.Mode = tlDCVIMeterCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Mode = tlDCVIMeterCurrent;
            Globals.TheHdw.DCVI.Pins("VDD1").Meter.Mode = Globals.tlDCVIMeterCurrent;

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            Globals.TheHdw.DCVI.Pins("VDD1").Gate = true;

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .Mode = tlDCVIModeVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Mode = tlDCVIModeVoltage;
            Globals.TheHdw.DCVI.Pins("VDD2").Mode = Globals.tlDCVIModeVoltage;

            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //OriginalCode: .SetVoltageAndRange VDD2_value, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(VDD2_value, 10);
            Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(VDD2_value, 10);

            //OriginalCode: .SetCurrentAndRange idd2_value, idd2_value;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(idd2_value, idd2_value);
            Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(idd2_value, idd2_value);

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;

            //OriginalCode: .Meter.Mode = tlDCVIMeterCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Mode = tlDCVIMeterCurrent;
            Globals.TheHdw.DCVI.Pins("VDD2").Meter.Mode = Globals.tlDCVIMeterCurrent;

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            Globals.TheHdw.DCVI.Pins("VDD2").Gate = true;

            //Parallel.Invoke(
            //() => Globals.TheHdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeVoltage,
            //() => Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10,
            //() => Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(VDD1_value, 10),
            //() => Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(idd1_value, idd1_value),
            //() => Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false,
            //() => Globals.TheHdw.DCVI.Pins("VDD1").Meter.Mode = Globals.tlDCVIMeterCurrent,
            //() => Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense),

            ////Globals.TheHdw.Wait(0.001);

            ////Globals.TheHdw.DCVI.Pins("VDD1").Gate = true;

            //() => Globals.TheHdw.DCVI.Pins("VDD2").Mode = Globals.tlDCVIModeVoltage,
            //() => Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10,
            //() => Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(VDD2_value, 10),
            //() => Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(idd2_value, idd2_value),
            //() => Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false,
            //() => Globals.TheHdw.DCVI.Pins("VDD2").Meter.Mode = Globals.tlDCVIMeterCurrent,
            //() => Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense)
            //);

            //Globals.TheHdw.Wait(0.001);
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true;

            //OriginalCode: End With;
            //OriginalCode: End Function;
            //PseudoCode  : FunctionEnd;
#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
        }

        static void PowerOn(double VDD1_value, double idd1_value, double VDD2_value, double idd2_value)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            DCPower VDD1 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "VDD1");
            DCPower VDD2 = InstrCtrl.DCPowerPinsToSessions(Globals.tsmContext, "VDD2");

            Parallel.Invoke(
                () => VDD1.ForceVoltage(VDD1_value, idd1_value),
                () => VDD2.ForceVoltage(VDD2_value, idd2_value)
                );

            Globals.VDD1MeterMode = Globals.tlDCVIMeterCurrent;
            Globals.VDD2MeterMode = Globals.tlDCVIMeterCurrent;
           

            //Parallel.Invoke(  //Gate is already on from continuity test -adrian
            //    () => VDD1.ConfigureOutputEnabled(),
            //    () => VDD2.ConfigureOutputEnabled()
            //    );
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
        }

        ///********************************************************/

        //DcviSupplyStatic Function

        ///********************************************************/

        public static void DcviSupplyStatic(Pattern ThePat, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins, bool do_ipat, bool VOConnectDisconnect = true)
        {
            //OriginalCode: Dim IDD_VDD1_high_1p9_1p9 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD1_high_1p9_1p9; Type: New SiteDouble;
            SiteDouble IDD_VDD1_high_1p9_1p9 = new SiteDouble();

            //OriginalCode: Dim IDD_VDD1_low_1p9_1p9 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD1_low_1p9_1p9; Type: New SiteDouble;
            SiteDouble IDD_VDD1_low_1p9_1p9 = new SiteDouble();

            //OriginalCode: Dim IDD_VDD2_high_1p9_1p9 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD2_high_1p9_1p9; Type: New SiteDouble;
            SiteDouble IDD_VDD2_high_1p9_1p9 = new SiteDouble();

            //OriginalCode: Dim IDD_VDD2_low_1p9_1p9 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD2_low_1p9_1p9; Type: New SiteDouble;
            SiteDouble IDD_VDD2_low_1p9_1p9 = new SiteDouble();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB on HMOD

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
            Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: VOConnectDisconnect, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null); //only connect DigPins on 1st IDD Static -adrian

            //OriginalCode: If NonBlank(ThePat) Then;
            //PseudoCode  : If: Condition: NonBlank(ThePat);
            if (NonBlank(ThePat))
            {
                //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_hi");
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_hi");
                Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_hi");

                //OriginalCode: Else;
                //PseudoCode  : Else;
            }
            else
            {
                //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateHi;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
                // ttb                Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            if (VOConnectDisconnect)    //only disconnect VOpins on 1st IDD Static -adrian
            {
                //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB, VOA");
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB, VOA");
                Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");  //For review to remove
            }


            //OriginalCode: TheHdw.Wait 2 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(2 * mS);
            //Globals.TheHdw.Wait(2 * Globals.mS);    //For review to remove
            Globals.TheHdw.Wait(1 * Globals.mS);

            //Measure IDDx Inputs High

            Parallel.Invoke(

            ////OriginalCode: IDD_VDD1_high_1p9_1p9 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            ////PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD1_high_1p9_1p9 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD1_high_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage),
            //IDD_VDD1_high_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);

            //OriginalCode: IDD_VDD2_high_1p9_1p9 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD2_high_1p9_1p9 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD2_high_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage)
            //IDD_VDD2_high_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);
            );

            //OriginalCode: If NonBlank(ThePat) Then;
            //PseudoCode  : If: Condition: NonBlank(ThePat);
            if (NonBlank(ThePat))
            {
                //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_lo");
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_lo");
                Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run("inputs_lo");

                //OriginalCode: Else;
                //PseudoCode  : Else;
            }
            else
            {
                //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
                // ttb                Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: TheHdw.Wait 2 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(2 * mS);
            //Globals.TheHdw.Wait(2 * Globals.mS);
            Globals.TheHdw.Wait(2 * Globals.mS);

            //Measure IDDx Inputs Low

            Parallel.Invoke(    //Measure IDD1 and IDD2 in parallel -adrian

            ////OriginalCode: IDD_VDD1_low_1p9_1p9 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            ////PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD1_low_1p9_1p9 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD1_low_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage),
            //IDD_VDD1_low_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);

            //OriginalCode: IDD_VDD2_low_1p9_1p9 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD2_low_1p9_1p9 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD2_low_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage)
            //IDD_VDD2_low_1p9_1p9 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);
            );

            //TheExec.Flow.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow

            //TheExec.Flow.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow

            //TheExec.Flow.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow

            //TheExec.Flow.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow

            //OriginalCode: If TheExec.CurrentJob = "ADuM225_pd_ipat" Then;
            //PseudoCode  : If: Condition: TheExec.CurrentJob = "ADuM225_pd_ipat";
            if (Globals.TheExec.CurrentJob == "ADuM225_pd_ipat")
            {
                //OriginalCode: If do_ipat Then;
                //PseudoCode  : If: Condition: do_ipat;
                if (do_ipat)
                {
                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: Else;
                    //PseudoCode  : Else;
                }
                else
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: ElseIf TheExec.CurrentJob = "ADuM225_handtest" Then;
                //PseudoCode  : ElseIf: Condition: TheExec.CurrentJob = "ADuM225_handtest";
            }
            else if (Globals.TheExec.CurrentJob == "ADuM225_handtest")
            {
                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: ElseIf TheExec.CurrentJob = "ADuM225_qc" Then;
                //PseudoCode  : ElseIf: Condition: TheExec.CurrentJob = "ADuM225_qc";
            }
            else if (Globals.TheExec.CurrentJob == "ADuM225_qc")
            {
                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                //OriginalCode: ElseIf TheExec.CurrentJob = "ADuM225_char" Then;
                //PseudoCode  : ElseIf: Condition: TheExec.CurrentJob = "ADuM225_char";
            }
            else if (Globals.TheExec.CurrentJob == "ADuM225_char")
            {
                //OriginalCode: If do_ipat Then;
                //PseudoCode  : If: Condition: do_ipat;
                if (do_ipat)
                {
                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: ipat.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: ipat.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.ipat.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: Else;
                    //PseudoCode  : Else;
                }
                else
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_high_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_high_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD1_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD1_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD2_low_1p9_1p9, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD2_low_1p9_1p9, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void DcviSupplyDynamic(Pattern ThePat, double Period, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins, bool do_ipat)
        {
            //OriginalCode: Dim IDD_VDD145 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD145; Type: New SiteDouble;
            SiteDouble IDD_VDD145 = new SiteDouble();

            //OriginalCode: Dim IDD_VDD146 As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: IDD_VDD146; Type: New SiteDouble;
            SiteDouble IDD_VDD146 = new SiteDouble();

            //OriginalCode: Dim Frequency As Double;
            //PseudoCode  : VariableDeclaration: Name: Frequency; Type: Double;
            double Frequency = 0;

            //OriginalCode: Dim Supply_CLoad() As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: Supply_CLoad(); Type: New SiteDouble;
            SiteDouble[] Supply_CLoad;

            //OriginalCode: Dim Primary_CLoad As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: Primary_CLoad; Type: New SiteDouble;
            //SiteDouble Primary_CLoad = new SiteDouble();	//change to fix error -adrian
            SiteDouble Primary_CLoad = new SiteDouble(4, 1);

            //OriginalCode: Dim Secondary_CLoad As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: Secondary_CLoad; Type: New SiteDouble;
            //SiteDouble Secondary_CLoad = new SiteDouble();	//change to fix error -adrian
            SiteDouble Secondary_CLoad = new SiteDouble(4, 1);

            //OriginalCode: Dim FlagsSet As Long;
            //PseudoCode  : VariableDeclaration: Name: FlagsSet; Type: Long;
            int FlagsSet = 0;

            //OriginalCode: Dim FlagsClear As Long;
            //PseudoCode  : VariableDeclaration: Name: FlagsClear; Type: Long;
            int FlagsClear = 0;

            //OriginalCode: Dim ChNum_Supply() As Long;
            //PseudoCode  : VariableDeclaration: Name: ChNum_Supply(); Type: Long;
            int[] ChNum_Supply;

            //OriginalCode: Dim nSite As Variant;
            //PseudoCode  : VariableDeclaration: Name: nSite; Type: Variant;
            //dynamic nSite;

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: ReDim Supply_CLoad(2);
            //PseudoCode  : ResizeArray: Name: Supply_CLoad; LowerBound: 0; UpperBound: 2;
            Supply_CLoad = SiteDouble.New(2);

            //OriginalCode: ReDim ChNum_Supply(2);
            //PseudoCode  : ResizeArray: Name: ChNum_Supply; LowerBound: 0; UpperBound: 2;
            ChNum_Supply = new int[2];

            //OriginalCode: Frequency = 1 / Period;
            //PseudoCode  : VariableAssignment: Name: Frequency; AssignedValue: 1 / Period;
            Frequency = 1 / Period;

            //VDD1

            //OriginalCode: ChNum_Supply(0) = 0;
            //PseudoCode  : VariableAssignment: Name: ChNum_Supply(0); AssignedValue: 0;
            ChNum_Supply[0] = 0;

            //VDD2

            //OriginalCode: ChNum_Supply(1) = 2;
            //PseudoCode  : VariableAssignment: Name: ChNum_Supply(1); AssignedValue: 2;
            ChNum_Supply[1] = 2;

            //OriginalCode: If (Frequency <= 5 * MHz) Then;
            //PseudoCode  : If: Condition: (Frequency <= 5 * 1000000);
            if ((Frequency <= 5 * 1000000))
            {
                //OriginalCode: Primary_CLoad(0) = 7.35 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(0); AssignedValue: 7.35 * pF;
                Primary_CLoad[0] = 7.35 * Globals.pF;

                //OriginalCode: Secondary_CLoad(0) = 7.3 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(0); AssignedValue: 7.3 * pF;
                Secondary_CLoad[0] = 7.3 * Globals.pF;

                //OriginalCode: Primary_CLoad(1) = 7.55 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(1); AssignedValue: 7.55 * pF;
                Primary_CLoad[1] = 7.55 * Globals.pF;

                //OriginalCode: Secondary_CLoad(1) = 7.25 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(1); AssignedValue: 7.25 * pF;
                Secondary_CLoad[1] = 7.25 * Globals.pF;

                //OriginalCode: Primary_CLoad(2) = 7.35 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(2); AssignedValue: 7.35 * pF;
                Primary_CLoad[2] = 7.35 * Globals.pF;

                //OriginalCode: Secondary_CLoad(2) = 7.2 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(2); AssignedValue: 7.2 * pF;
                Secondary_CLoad[2] = 7.2 * Globals.pF;

                //OriginalCode: Primary_CLoad(3) = 7.45 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(3); AssignedValue: 7.45 * pF;
                Primary_CLoad[3] = 7.45 * Globals.pF;

                //OriginalCode: Secondary_CLoad(3) = 7.25 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(3); AssignedValue: 7.25 * pF;
                Secondary_CLoad[3] = 7.25 * Globals.pF;

                //OriginalCode: Else;
                //PseudoCode  : Else;
            }
            else
            {
                //OriginalCode: Primary_CLoad(0) = 7.35 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(0); AssignedValue: 7.35 * pF;
                Primary_CLoad[0] = 7.35 * Globals.pF;

                //OriginalCode: Secondary_CLoad(0) = 7.3 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(0); AssignedValue: 7.3 * pF;
                Secondary_CLoad[0] = 7.3 * Globals.pF;

                //OriginalCode: Primary_CLoad(1) = 7.55 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(1); AssignedValue: 7.55 * pF;
                Primary_CLoad[1] = 7.55 * Globals.pF;

                //OriginalCode: Secondary_CLoad(1) = 7.25 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(1); AssignedValue: 7.25 * pF;
                Secondary_CLoad[1] = 7.25 * Globals.pF;

                //OriginalCode: Primary_CLoad(2) = 7.35 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(2); AssignedValue: 7.35 * pF;
                Primary_CLoad[2] = 7.35 * Globals.pF;

                //OriginalCode: Secondary_CLoad(2) = 7.2 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(2); AssignedValue: 7.2 * pF;
                Secondary_CLoad[2] = 7.2 * Globals.pF;

                //OriginalCode: Primary_CLoad(3) = 7.45 * pF;
                //PseudoCode  : VariableAssignment: Name: Primary_CLoad(3); AssignedValue: 7.45 * pF;
                Primary_CLoad[3] = 7.45 * Globals.pF;

                //OriginalCode: Secondary_CLoad(3) = 7.25 * pF;
                //PseudoCode  : VariableAssignment: Name: Secondary_CLoad(3); AssignedValue: 7.25 * pF;
                Secondary_CLoad[3] = 7.25 * Globals.pF;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            //PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);  //Already set on the previous test -adrian
            if (Period == 20.0E-09)
            {
                PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            }

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
            Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: false, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);   //no need to connect all digital pins -adrian

            //OriginalCode: TheHdw.Digital.Patgen.TimeoutEnable = False;
            //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB, VOA");
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB, VOA");
            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");    //VOA and VOB is already disconnected -adrian

            //Run the Pattern

            //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat).Start;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat).Start();
            Globals.TheHdw.Digital.Patterns.Pat(ThePat).Start();

            //OriginalCode: TheHdw.Wait 2 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(2 * mS);
            //Globals.TheHdw.Wait(2 * Globals.mS);
            Globals.TheHdw.Wait(1 * Globals.mS);

            Parallel.Invoke(    //Measure IDD1 and IDD2 in parallel -adrian

            ////OriginalCode: IDD_VDD145 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            ////PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD145 = TheHdw.DCVI.Pins("VDD1").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD145 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage),
            //IDD_VDD145 = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);

            ////OriginalCode: IDD_VDD146 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            ////PseudoCode  : AsIsLineOfCode: LineOfCode: IDD_VDD146 = TheHdw.DCVI.Pins("VDD2").Meter.Read(tlStrobe, 10, 100000, tlDCVIMeterReadingFormatAverage);
            () => IDD_VDD146 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage)
            //IDD_VDD146 = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 100000, Globals.tlDCVIMeterReadingFormatAverage);
            );

            //Power Supply Current (excluding load current), Where ILoad = #CH * CVF

            int i = 0;  //correct indexing even sites was mixed caused by disabled site -adrian
            //OriginalCode: For Each nSite In TheExec.Sites.Active;
            //PseudoCode  : ForEachLoop: ObjectName: nSite; CollectionName: TheExec.Sites.Active;
            foreach (dynamic nSite in Globals.TheExec.Sites.Active)
            {
                GlobalFunctions.BeginSiteLoop(nSite);

                //OriginalCode: IDD_VDD145(nSite) = IDD_VDD145(nSite) - (ChNum_Supply(0) * Primary_CLoad(nSite) * VDD1_value * Frequency);
                //PseudoCode  : VariableAssignment: Name: IDD_VDD145(nSite); AssignedValue: IDD_VDD145(nSite) - (ChNum_Supply(0) * Primary_CLoad(nSite) * VDD1_value * Frequency);
                //IDD_VDD145.Value[nSite] = IDD_VDD145.Value[nSite] - (ChNum_Supply[0] * Primary_CLoad.Value[nSite] * VDD1_value * Frequency);  //causes error when some site was disabled -adrian
                IDD_VDD145.Value[i] = IDD_VDD145.Value[i] - (ChNum_Supply[0] * Primary_CLoad.Value[nSite] * VDD1_value * Frequency);

                //OriginalCode: IDD_VDD146(nSite) = IDD_VDD146(nSite) - (ChNum_Supply(1) * Secondary_CLoad(nSite) * VDD2_value * Frequency);
                //PseudoCode  : VariableAssignment: Name: IDD_VDD146(nSite); AssignedValue: IDD_VDD146(nSite) - (ChNum_Supply(1) * Secondary_CLoad(nSite) * VDD2_value * Frequency);
                //IDD_VDD146.Value[nSite] = IDD_VDD146.Value[nSite] - (ChNum_Supply[1] * Secondary_CLoad.Value[nSite] * VDD2_value * Frequency);    //causes error when some site was disabled -adrian
                IDD_VDD146.Value[i] = IDD_VDD146.Value[i] - (ChNum_Supply[1] * Secondary_CLoad.Value[nSite] * VDD2_value * Frequency);

                //OriginalCode: Next nSite;
                //PseudoCode  : EndForLoop: VariableName: nSite;
                GlobalFunctions.EndSiteLoop(nSite);

                i++;
            }

            //Clear condition flags set in pattern

            //OriginalCode: FlagsSet = cpuB + cpuC + cpuD;
            //PseudoCode  : VariableAssignment: Name: FlagsSet; AssignedValue: cpuB + cpuC + cpuD;
            FlagsSet = Globals.cpuB + Globals.cpuC + Globals.cpuD;

            //OriginalCode: FlagsClear = cpuA;
            //PseudoCode  : VariableAssignment: Name: FlagsClear; AssignedValue: cpuA;
            FlagsClear = Globals.cpuA;

            //Stop Looping Pattern

            //OriginalCode: Call TheHdw.Digital.Patgen.Continue(FlagsSet, FlagsClear);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patgen.Continue(FlagsSet, FlagsClear);
            Globals.TheHdw.Digital.Patgen.Continue(FlagsSet, FlagsClear);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD145, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD145, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD145, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=IDD_VDD146, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=IDD_VDD146, ScaleType:=scaleMilli, unit:=unitAmp, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: IDD_VDD146, ScaleType: Globals.scaleMilli, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void PropDelay(PinList TestPins, double StartEdge, double StepSize, string StartLabel, Pattern ThePat_B, Pattern ThePat_A, string MeasEdge, int VoltageIndex, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins, bool ConnectAllDigPins)
        {
            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Reconnect VOA/VOB on HMOD

            //OriginalCode: Dim i As Long;
            //PseudoCode  : VariableDeclaration: Name: i; Type: Long;
            int i = 0;

            //OriginalCode: Dim nSite As Variant;
            //PseudoCode  : VariableDeclaration: Name: nSite; Type: Variant;
            //dynamic nSite;

            //OriginalCode: Dim PartNum As String;
            //PseudoCode  : VariableDeclaration: Name: PartNum; Type: String;
            string PartNum;

            //OriginalCode: Dim TestPinsNum As Long;
            //PseudoCode  : VariableDeclaration: Name: TestPinsNum; Type: Long;
            int TestPinsNum = 0;

            //OriginalCode: Dim TestPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: TestPinsArray(); Type: String;
            string[] TestPinsArray;

            //OriginalCode: Dim Edges() As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: Edges(); Type: New SiteDouble;
            SiteDouble[] Edges;

            //OriginalCode: Dim DCCategory As String;
            //PseudoCode  : VariableDeclaration: Name: DCCategory; Type: String;
            string DCCategory;

            //OriginalCode: Dim Verbose As Boolean;
            //PseudoCode  : VariableDeclaration: Name: Verbose; Type: Boolean;
            bool Verbose;

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //OriginalCode: Verbose = True;
            //PseudoCode  : VariableAssignment: Name: Verbose; AssignedValue: True;
            Verbose = true;

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            //PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);


            //----------------------------------VOA per site VOH_VOL---------------------------------------

            var VOH = new double[4];
            var VOL = new double[4];

            //Apply Levels and Timing
            switch (MeasEdge) 
            {
                case "Rise":
                    PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
                    //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
                    //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
                    Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: ConnectAllDigPins, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
                    //Set VOH_VOL per site value based on VDD value
                    switch (VDD2_value)
                    {
                        case 1.7:
                            VOH = new double[] { 0.69, 0.85, 0.74, 0.85 };
                            VOL = VOH;
                            break;
                        case 2.25:
                            VOH = new double[] { .95, 1.125, 1, 1.125 };
                            VOL = VOH;
                            break;
                        case 3:
                            VOH = new double[] { 1.3, 1.5, 1.37, 1.6 };
                            VOL = VOH;
                            break;
                        case 4.5:
                            VOH = new double[] { 2, 2.12, 2.05, 2.3};
                            VOL = VOH;
                            break;
                    }
                    break;
                case "Fall":
                    //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: ConnectAllDigPins, voltagelevels: true, vil: 0, vih: VDD1_value, vol: (VDD1_value * .5), voh: (VDD1_value * .5), vterm: (VDD1_value / 2));
                    //Set VOH_VOL per site value based on VDD value
                    switch (VDD2_value)
                    {
                        case 1.7:
                            VOH = new double[] { 0.95, 0.85, 0.95, 0.85 };
                            VOL = VOH;
                            break;
                        case 2.25:
                            VOH = new double[] { 1.25, 1.125, 1.25, 1.125 };
                            VOL = VOH;
                            break;
                        case 3:
                            VOH = new double[] { 1.75, 1.6, 1.75, 1.5 };
                            VOL = VOH;
                            break;
                        case 4.5:
                            VOH = new double[] { 2.5, 2.25, 2.5, 2.25 };
                            VOL = VOH;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentException("Incorrect MeasEdge");
            }

            //Per site VOH_VOL
            ConfigureVOHandVOLperSite(VOH, VOL, "VOA");


            //----------------------------------VOB per site VOH_VOL---------------------------------------
            switch (MeasEdge)
            {
                case "Rise":
                    //Set VOH_VOL per site value based on VDD value
                    switch (VDD2_value)
                    {
                        case 1.7:
                            VOH = new double[] {0.72, 0.85, 0.74, 0.85};
                            VOL = VOH;
                            break;
                        case 2.25:
                            VOH = new double[] { 1, 1.125, 1, 1.15 };
                            VOL = VOH;
                            break;
                        case 3:
                            VOH = new double[] { 1.35, 1.43, 1.43, 1.55 };
                            VOL = VOH;
                            break;
                        case 4.5:
                            VOH = new double[] { 1.95, 2.25, 1.95, 2.35 };
                            VOL = VOH;
                            break;
                    }
                    break;
                case "Fall":
                    //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: ConnectAllDigPins, voltagelevels: true, vil: 0, vih: VDD1_value, vol: (VDD1_value * .5), voh: (VDD1_value * .5), vterm: (VDD1_value / 2));
                    //Set VOH_VOL per site value based on VDD value
                    switch (VDD2_value)
                    {
                        case 1.7:
                            VOH = new double[] {0.96, 0.82, 0.95, 0.85};
                            VOL = VOH;
                            break;
                        case 2.25:
                            VOH = new double[] { 1.3, 1.125, 1.25, 1.125 };
                            VOL = VOH;
                            break;
                        case 3:
                            VOH = new double[] { 1.7, 1.5, 1.65, 1.5 };
                            VOL = VOH;
                            break;
                        case 4.5:
                            VOH = new double[] { 2.7, 2.25, 2.5, 2.35 };
                            VOL = VOH;
                            break;
                    }
                    break;
                default:
                    throw new ArgumentException("Incorrect MeasEdge");
            }

            //Per site VOH_VOL
            ConfigureVOHandVOLperSite(VOH, VOL, "VOB");


            //OriginalCode: TheExec.DataManager.DecomposePinList TestPins, TestPinsArray(), TestPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(TestPins, TestPinsArray(), TestPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(TestPins, out TestPinsArray, out TestPinsNum);

            //OriginalCode: ReDim Edges(TestPinsNum - 1);
            //PseudoCode  : ResizeArray: Name: Edges; LowerBound: 0; UpperBound: TestPinsNum - 1;
            Edges = SiteDouble.New(TestPinsNum - 1);

            //Initialize Edges Array

            //OriginalCode: For i = 0 To UBound(TestPinsArray);
            //PseudoCode  : ForLoop: VariableName: i; Start: 0; Stop: UBound(TestPinsArray); StepSize: 1;
            /*
            for (i = 0; i <= GlobalFunctions.UBound(TestPinsArray); i = i + 1)
            {
                //OriginalCode: For Each nSite In TheExec.Sites.Active;
                //PseudoCode  : ForEachLoop: ObjectName: nSite; CollectionName: TheExec.Sites.Active;
                foreach (dynamic nSite in Globals.TheExec.Sites.Active)
                {
                    GlobalFunctions.BeginSiteLoop(nSite);

                    //OriginalCode: Edges(i)(nSite) = 0;
                    //PseudoCode  : VariableAssignment: Name: Edges(i)(nSite); AssignedValue: 0;
                    Edges[i].Value[nSite] = 0;

                    //OriginalCode: Next nSite;
                    //PseudoCode  : EndForLoop: VariableName: nSite;
                    GlobalFunctions.EndSiteLoop(nSite);
                }

                //OriginalCode: Next i;
                //PseudoCode  : EndForLoop: VariableName: i;
            }
            */

            //Run the Pattern

            //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat_B).Run;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat_B).Run();
            //Globals.TheHdw.Digital.Patterns.Pat(ThePat_B).Run(); --> New Implementation inside siteloop

            //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat_B).Run;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat_B).Run();
            //Globals.TheHdw.Digital.Patterns.Pat(ThePat_B).Run(); --> New Implementation inside siteloop

            //Look at failcounts to find edges

            //OriginalCode: For Each nSite In TheExec.Sites.Active;
            //PseudoCode  : ForEachLoop: ObjectName: nSite; CollectionName: TheExec.Sites.Active;
            //foreach (dynamic nSite in Globals.TheExec.Sites.Active)
            //{
            //    GlobalFunctions.BeginSiteLoop(nSite);

            //Call NI STS PropDelay
            //Edges = GlobalFunctions.PropDelayPatternBurst(TestPinsArray, StepSize, StartEdge, ThePat_B, Edges, nSite);
            //Edges = GlobalFunctions.PropDelayPatternBurst(TestPinsArray, StepSize, StartEdge, ThePat_B, Edges, nSite);
            Edges = GlobalFunctions.PropDelayPatternBurst(TestPinsArray, StepSize, StartEdge, ThePat_B, Edges); //adjust to output complete sitedouble per pin -adrian

            //OriginalCode: Edges(0)(nSite) = StartEdge + StepSize * TheHdw.Pins(TestPinsArray(0)).FailCount(nSite);
            //PseudoCode  : VariableAssignment: Name: Edges(0)(nSite); AssignedValue: StartEdge + StepSize * TheHdw.Pins(TestPinsArray(0)).FailCount(nSite);
            //Edges[0].Value[nSite] = StartEdge + StepSize * Globals.TheHdw.Pins.Pins(TestPinsArray[0]).FailCount.Value[nSite]; --> New Implementation inside PropDelayPatternBurst

            //OriginalCode: Edges(1)(nSite) = StartEdge + StepSize * TheHdw.Pins(TestPinsArray(1)).FailCount(nSite);
            //PseudoCode  : VariableAssignment: Name: Edges(1)(nSite); AssignedValue: StartEdge + StepSize * TheHdw.Pins(TestPinsArray(1)).FailCount(nSite);
            //Edges[1].Value[nSite] = StartEdge + StepSize * Globals.TheHdw.Pins.Pins(TestPinsArray[1]).FailCount.Value[nSite]; --> New Implementation inside PropDelayPatternBurst

            //OriginalCode: Next nSite;
            //PseudoCode  : EndForLoop: VariableName: nSite;
            //GlobalFunctions.EndSiteLoop(nSite);
            //        }

            //OriginalCode: Call TheExec.DataManager.GetInstanceContext(DCCategory, "", "", "", "", "", "", "", -1);
            //TheExec.DataManager.GetInstanceContext(DCCategory, "", "", "", "", "", "", "", -1)

            //OriginalCode: Select Case MeasEdge;
            //PseudoCode  : SelectCaseHeader: VariableName: MeasEdge;
            switch (MeasEdge)
            {
                //OriginalCode: Case "Rise";
                //PseudoCode  : CaseCheck: CaseVariableValue: "Rise";
                case "Rise":

                    //OriginalCode: ReDim PropDelayRise(TestPinsNum - 1);
                    //PseudoCode  : ResizeArray: Name: PropDelayRise; LowerBound: 0; UpperBound: TestPinsNum - 1;
                    Globals.PropDelayRise = SiteDouble.New(TestPinsNum - 1);

                    //OriginalCode: For i = 0 To UBound(TestPinsArray);
                    //PseudoCode  : ForLoop: VariableName: i; Start: 0; Stop: UBound(TestPinsArray); StepSize: 1;
                    for (i = 0; i <= GlobalFunctions.UBound(TestPinsArray); i = i + 1)
                    {
                        //OriginalCode: PropDelayRise(i) = Edges(i);
                        //PseudoCode  : VariableAssignment: Name: PropDelayRise(i); AssignedValue: Edges(i);
                        Globals.PropDelayRise[i] = Edges[i];

                        //OriginalCode: TheExec.Flow.TestLimit resultval:=Edges(i), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Edges(i), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
                        Globals.TheExec.Flow.TestLimit(resultval: Edges[i], ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

                        //OriginalCode: Next i;
                        //PseudoCode  : EndForLoop: VariableName: i;
                    }

                    //OriginalCode: Case "Fall";
                    break;

                //PseudoCode  : CaseCheck: CaseVariableValue: "Fall";
                case "Fall":

                    //OriginalCode: ReDim PropDelayFall(TestPinsNum - 1);
                    //PseudoCode  : ResizeArray: Name: PropDelayFall; LowerBound: 0; UpperBound: TestPinsNum - 1;
                    Globals.PropDelayFall = SiteDouble.New(TestPinsNum - 1);

                    //OriginalCode: For i = 0 To UBound(TestPinsArray);
                    //PseudoCode  : ForLoop: VariableName: i; Start: 0; Stop: UBound(TestPinsArray); StepSize: 1;
                    for (i = 0; i <= GlobalFunctions.UBound(TestPinsArray); i = i + 1)
                    {
                        //OriginalCode: PropDelayFall(i) = Edges(i);
                        //PseudoCode  : VariableAssignment: Name: PropDelayFall(i); AssignedValue: Edges(i);
                        Globals.PropDelayFall[i] = Edges[i];

                        //OriginalCode: TheExec.Flow.TestLimit resultval:=Edges(i), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Edges(i), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
                        Globals.TheExec.Flow.TestLimit(resultval: Edges[i], ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

                        //OriginalCode: Next i;
                        //PseudoCode  : EndForLoop: VariableName: i;
                    }

                    //OriginalCode: End Select;
                    //PseudoCode  : EndSelectCase;
                    break;
                default:
                    Console.WriteLine("Reached the default condition of switch statement");
                    break;
            }

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void PulseWidthDistortion(PinList TestPins, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins)
        {
            //OriginalCode: Dim TestPinsNum As Long;
            //PseudoCode  : VariableDeclaration: Name: TestPinsNum; Type: Long;
            int TestPinsNum = 0;

            //OriginalCode: Dim TestPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: TestPinsArray(); Type: String;
            string[] TestPinsArray;

            //OriginalCode: Dim i As Long;
            //PseudoCode  : VariableDeclaration: Name: i; Type: Long;
            int i = 0;

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            //if (NonBlank(PrebodyUtil1Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
            //    Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            ////OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            ////PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            //if (NonBlank(PrebodyUtil0Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
            //    Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            //OriginalCode: TheExec.DataManager.DecomposePinList TestPins, TestPinsArray(), TestPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(TestPins, TestPinsArray(), TestPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(TestPins, out TestPinsArray, out TestPinsNum);

            //OriginalCode: For i = 0 To UBound(TestPinsArray);
            //PseudoCode  : ForLoop: VariableName: i; Start: 0; Stop: UBound(TestPinsArray); StepSize: 1;
            for (i = 0; i <= GlobalFunctions.UBound(TestPinsArray); i = i + 1)
            {
                //OriginalCode: TheExec.Flow.TestLimit resultval:=PropDelayRise(i).Subtract(PropDelayFall(i)), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=PropDelayRise(i).Subtract(PropDelayFall(i)), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
                Globals.TheExec.Flow.TestLimit(resultval: Globals.PropDelayRise[i].Subtract(Globals.PropDelayFall[i]), ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

                //OriginalCode: Next i;
                //PseudoCode  : EndForLoop: VariableName: i;
            }

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            //if (NonBlank(PostbodyUtil1Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
            //    Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            ////OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            ////PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            //if (NonBlank(PostbodyUtil0Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
            //    Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void ChChMatch(PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins)
        {
            //OriginalCode: Dim i As Long;
            //PseudoCode  : VariableDeclaration: Name: i; Type: Long;
            int i = 0;

            //OriginalCode: Dim j As Long;
            //PseudoCode  : VariableDeclaration: Name: j; Type: Long;
            int j = 0;

            //OriginalCode: Dim nSite As Variant;
            //PseudoCode  : VariableDeclaration: Name: nSite; Type: Variant;
            //dynamic nSite;

            //OriginalCode: Dim TestPinsNum As Long;
            //PseudoCode  : VariableDeclaration: Name: TestPinsNum; Type: Long;
            int TestPinsNum = 0;

            //OriginalCode: Dim VOA_VOB_error As Double;
            //PseudoCode  : VariableDeclaration: Name: VOA_VOB_error; Type: Double;
            double VOA_VOB_error = 0;

            //OriginalCode: Dim TempError(0) As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: TempError(0); Type: New SiteDouble;
            SiteDouble[] TempError = SiteDouble.New(0);

            //OriginalCode: Dim CO_DIR_CH_CH_RiseMaxError(0) As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: CO_DIR_CH_CH_RiseMaxError(0); Type: New SiteDouble;
            SiteDouble[] CO_DIR_CH_CH_RiseMaxError = SiteDouble.New(0);

            //OriginalCode: Dim CO_DIR_CH_CH_FallMaxError(0) As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: CO_DIR_CH_CH_FallMaxError(0); Type: New SiteDouble;
            SiteDouble[] CO_DIR_CH_CH_FallMaxError = SiteDouble.New(0);

            //OriginalCode: Dim CO_DIR_CH_CH_RiseFallMaxError(0) As New SiteDouble;
            //PseudoCode  : VariableDeclaration: Name: CO_DIR_CH_CH_RiseFallMaxError(0); Type: New SiteDouble;
            SiteDouble[] CO_DIR_CH_CH_RiseFallMaxError = SiteDouble.New(0);

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            //if (NonBlank(PrebodyUtil1Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
            //    Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            ////OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            ////PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            //if (NonBlank(PrebodyUtil0Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
            //    Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            //Initialize i

            //OriginalCode: i = 0;
            //PseudoCode  : VariableAssignment: Name: i; AssignedValue: 0;
            i = 0;

            //Calculate CH_CH Matching Rise vs Rise Edge

            //OriginalCode: For Each nSite In TheExec.Sites.Active;
            //PseudoCode  : ForEachLoop: ObjectName: nSite; CollectionName: TheExec.Sites.Active;
            //foreach (dynamic nSite in Globals.TheExec.Sites.Active)   //causes error when some sites were disabled -adrian
            for (int nSite = 0; nSite < Globals.tsmContext.SiteNumbers.Count; nSite++)
            {
                GlobalFunctions.BeginSiteLoop(nSite);

                //OriginalCode: VOA_VOB_error = Abs(PropDelayRise(i)(nSite) - PropDelayRise(i + 1)(nSite));
                //PseudoCode  : VariableAssignment: Name: VOA_VOB_error; AssignedValue: Abs(PropDelayRise(i)(nSite) - PropDelayRise(i + 1)(nSite));
                VOA_VOB_error = Math.Abs(Globals.PropDelayRise[i].Value[nSite] - Globals.PropDelayRise[i + 1].Value[nSite]);

                //Stuff Temp Array

                //OriginalCode: TempError(i)(nSite) = VOA_VOB_error;
                //PseudoCode  : VariableAssignment: Name: TempError(i)(nSite); AssignedValue: VOA_VOB_error;
                TempError[i].Value[nSite] = VOA_VOB_error;

                //Find worst case co-directional CH_CH Matching, Rise vs Rise

                //OriginalCode: CO_DIR_CH_CH_RiseMaxError(0)(nSite) = TempError(0)(nSite);
                //PseudoCode  : VariableAssignment: Name: CO_DIR_CH_CH_RiseMaxError(0)(nSite); AssignedValue: TempError(0)(nSite);
                CO_DIR_CH_CH_RiseMaxError[0].Value[nSite] = TempError[0].Value[nSite];

                //Calculate CH_CH Matching Fall vs Fall Edge

                //OriginalCode: VOA_VOB_error = Abs(PropDelayFall(i)(nSite) - PropDelayFall(i + 1)(nSite));
                //PseudoCode  : VariableAssignment: Name: VOA_VOB_error; AssignedValue: Abs(PropDelayFall(i)(nSite) - PropDelayFall(i + 1)(nSite));
                VOA_VOB_error = Math.Abs(Globals.PropDelayFall[i].Value[nSite] - Globals.PropDelayFall[i + 1].Value[nSite]);

                //Stuff Temp Array

                //OriginalCode: TempError(i)(nSite) = VOA_VOB_error;
                //PseudoCode  : VariableAssignment: Name: TempError(i)(nSite); AssignedValue: VOA_VOB_error;
                TempError[i].Value[nSite] = VOA_VOB_error;

                //Find worst case co-directional CH_CH Matching, Fall vs Fall

                //OriginalCode: CO_DIR_CH_CH_FallMaxError(0)(nSite) = TempError(0)(nSite);
                //PseudoCode  : VariableAssignment: Name: CO_DIR_CH_CH_FallMaxError(0)(nSite); AssignedValue: TempError(0)(nSite);
                CO_DIR_CH_CH_FallMaxError[0].Value[nSite] = TempError[0].Value[nSite];

                //Calculate CH_CH Matching Rise vs Fall Edge

                //OriginalCode: VOA_VOB_error = Abs(PropDelayRise(i)(nSite) - PropDelayFall(i + 1)(nSite));
                //PseudoCode  : VariableAssignment: Name: VOA_VOB_error; AssignedValue: Abs(PropDelayRise(i)(nSite) - PropDelayFall(i + 1)(nSite));
                VOA_VOB_error = Math.Abs(Globals.PropDelayRise[i].Value[nSite] - Globals.PropDelayFall[i + 1].Value[nSite]);

                //Stuff Temp Array

                //OriginalCode: TempError(i)(nSite) = VOA_VOB_error;
                //PseudoCode  : VariableAssignment: Name: TempError(i)(nSite); AssignedValue: VOA_VOB_error;
                TempError[i].Value[nSite] = VOA_VOB_error;

                //Find worst case co-directional CH_CH Matching, Rise vs Fall

                //OriginalCode: CO_DIR_CH_CH_RiseFallMaxError(0)(nSite) = TempError(0)(nSite);
                //PseudoCode  : VariableAssignment: Name: CO_DIR_CH_CH_RiseFallMaxError(0)(nSite); AssignedValue: TempError(0)(nSite);
                CO_DIR_CH_CH_RiseFallMaxError[0].Value[nSite] = TempError[0].Value[nSite];

                //OriginalCode: Next nSite;
                //PseudoCode  : EndForLoop: VariableName: nSite;
                GlobalFunctions.EndSiteLoop(nSite);
            }

            //OriginalCode: TheExec.Flow.TestLimit resultval:=CO_DIR_CH_CH_FallMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=CO_DIR_CH_CH_FallMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: CO_DIR_CH_CH_FallMaxError[0], ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=CO_DIR_CH_CH_RiseFallMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=CO_DIR_CH_CH_RiseFallMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: CO_DIR_CH_CH_RiseFallMaxError[0], ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=CO_DIR_CH_CH_RiseMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=CO_DIR_CH_CH_RiseMaxError(0), ScaleType:=scaleNano, unit:=unitTime, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: CO_DIR_CH_CH_RiseMaxError[0], ScaleType: Globals.scaleNano, unit: Globals.unitTime, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            //if (NonBlank(PostbodyUtil1Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
            //    Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

            ////OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            ////PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            //if (NonBlank(PostbodyUtil0Pins))
            //{
            //    //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
            //    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
            //    Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

            //    //OriginalCode: End If;
            //    //PseudoCode  : EndIf;
            //}

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void Functional(PatternSet ThePat, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins)
        {
            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //Set supplies

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            //PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);  //Already set on the previous test -adrian

            //Apply levels and timing

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
            Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: false, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);   //DigPins are already connected

            //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat).Test(pfAlways, 1);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat).Test(pfAlways, 1);
            Globals.TheHdw.Digital.Patterns.Pat(ThePat).Test(Globals.pfAlways, 1);

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void FunctionalModifyTiming(PatternSet ThePat, PinList InPins, PinList OutPins, PinList MuxInPins, PinList MuxOutPins, string TsetNamePhase1, string TsetNamePhase2, string TsetNamePhase3, string TsetNamePhase4, double Period, int MuxType, double VDD1_value, double idd1_value, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins)
        {
            //OriginalCode: Dim i As Long;
            //PseudoCode  : VariableDeclaration: Name: i; Type: Long;
            int i = 0;

            //OriginalCode: Dim InPinsNum As Long;
            //PseudoCode  : VariableDeclaration: Name: InPinsNum; Type: Long;
            int InPinsNum = 0;

            //OriginalCode: Dim OutPinsNum As Long;
            //PseudoCode  : VariableDeclaration: Name: OutPinsNum; Type: Long;
            int OutPinsNum = 0;

            //OriginalCode: Dim InPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: InPinsArray(); Type: String;
            string[] InPinsArray;

            //OriginalCode: Dim OutPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: OutPinsArray(); Type: String;
            string[] OutPinsArray;

            //OriginalCode: Dim MuxInPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: MuxInPinsArray(); Type: String;
            string[] MuxInPinsArray;

            //OriginalCode: Dim MuxOutPinsArray() As String;
            //PseudoCode  : VariableDeclaration: Name: MuxOutPinsArray(); Type: String;
            string[] MuxOutPinsArray;

            //OriginalCode: Dim SiteNum As Long;
            //PseudoCode  : VariableDeclaration: Name: SiteNum; Type: Long;
            int SiteNum = 0;

            //OriginalCode: Dim nSite As Variant;
            //PseudoCode  : VariableDeclaration: Name: nSite; Type: Variant;
            //dynamic nSite;

            //OriginalCode: Dim NumActiveSites As Long;
            //PseudoCode  : VariableDeclaration: Name: NumActiveSites; Type: Long;
            int NumActiveSites = 0;

            //Array Declarations

            //OriginalCode: Dim D1Edge_Odd_Phase1_2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase1_2; Type: Double;
            double D1Edge_Odd_Phase1_2 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase1_2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase1_2; Type: Double;
            double D2Edge_Odd_Phase1_2 = 0;

            //OriginalCode: Dim D1Edge_Phase1_2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase1_2; Type: Double;
            double D1Edge_Phase1_2 = 0;

            //OriginalCode: Dim D2Edge_Phase1_2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase1_2; Type: Double;
            double D2Edge_Phase1_2 = 0;

            //OriginalCode: Dim D1Edge_Odd_Phase3_4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase3_4; Type: Double;
            double D1Edge_Odd_Phase3_4 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase3_4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase3_4; Type: Double;
            double D2Edge_Odd_Phase3_4 = 0;

            //OriginalCode: Dim D1Edge_Phase3_4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase3_4; Type: Double;
            double D1Edge_Phase3_4 = 0;

            //OriginalCode: Dim D2Edge_Phase3_4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase3_4; Type: Double;
            double D2Edge_Phase3_4 = 0;

            //OriginalCode: Dim D1Edge_Odd_Phase1 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase1; Type: Double;
            double D1Edge_Odd_Phase1 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase1 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase1; Type: Double;
            double D2Edge_Odd_Phase1 = 0;

            //OriginalCode: Dim D1Edge_Phase1 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase1; Type: Double;
            double D1Edge_Phase1 = 0;

            //OriginalCode: Dim D2Edge_Phase1 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase1; Type: Double;
            double D2Edge_Phase1 = 0;

            //OriginalCode: Dim D1Edge_Odd_Phase2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase2; Type: Double;
            double D1Edge_Odd_Phase2 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase2; Type: Double;
            double D2Edge_Odd_Phase2 = 0;

            //OriginalCode: Dim D1Edge_Phase2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase2; Type: Double;
            double D1Edge_Phase2 = 0;

            //OriginalCode: Dim D2Edge_Phase2 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase2; Type: Double;
            double D2Edge_Phase2 = 0;

            //OriginalCode: Dim D1Edge_Odd_Phase3 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase3; Type: Double;
            double D1Edge_Odd_Phase3 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase3 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase3; Type: Double;
            double D2Edge_Odd_Phase3 = 0;

            //OriginalCode: Dim D1Edge_Phase3 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase3; Type: Double;
            double D1Edge_Phase3 = 0;

            //OriginalCode: Dim D2Edge_Phase3 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase3; Type: Double;
            double D2Edge_Phase3 = 0;

            //OriginalCode: Dim D1Edge_Odd_Phase4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Odd_Phase4; Type: Double;
            double D1Edge_Odd_Phase4 = 0;

            //OriginalCode: Dim D2Edge_Odd_Phase4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Odd_Phase4; Type: Double;
            double D2Edge_Odd_Phase4 = 0;

            //OriginalCode: Dim D1Edge_Phase4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D1Edge_Phase4; Type: Double;
            double D1Edge_Phase4 = 0;

            //OriginalCode: Dim D2Edge_Phase4 As Double;
            //PseudoCode  : VariableDeclaration: Name: D2Edge_Phase4; Type: Double;
            double D2Edge_Phase4 = 0;

            //OriginalCode: Dim Delay As Double;
            //PseudoCode  : VariableDeclaration: Name: Delay; Type: Double;
            double Delay = 0;

            //OriginalCode: TheExec.DataManager.DecomposePinList InPins, InPinsArray(), InPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(InPins, InPinsArray(), InPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(InPins, out InPinsArray, out InPinsNum);

            //OriginalCode: TheExec.DataManager.DecomposePinList OutPins, OutPinsArray(), OutPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(OutPins, OutPinsArray(), OutPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(OutPins, out OutPinsArray, out OutPinsNum);

            //OriginalCode: TheExec.DataManager.DecomposePinList MuxInPins, MuxInPinsArray(), InPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(MuxInPins, MuxInPinsArray(), InPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(MuxInPins, out MuxInPinsArray, out InPinsNum);

            //OriginalCode: TheExec.DataManager.DecomposePinList MuxOutPins, MuxOutPinsArray(), OutPinsNum;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.DataManager.DecomposePinList(MuxOutPins, MuxOutPinsArray(), OutPinsNum);
            Globals.TheExec.DataManager.DecomposePinList(MuxOutPins, out MuxOutPinsArray, out OutPinsNum);

            //Dimension / Initialize All Arrays

            //OriginalCode: i = 0;
            //PseudoCode  : VariableAssignment: Name: i; AssignedValue: 0;
            i = 0;

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //Set supplies

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            //PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);  //Already set on previous test -adrian

            //Read timing Array

            //OriginalCode: For Each nSite In TheExec.Sites.Active;
            //PseudoCode  : ForEachLoop: ObjectName: nSite; CollectionName: TheExec.Sites.Active;
            //foreach (dynamic nSite in Globals.TheExec.Sites.Active)   //causes error when some sites were disabled -adrian
            //for (int nSite = 0; nSite < Globals.tsmContext.SiteNumbers.Count; nSite++)
            for (int nSite = 0; nSite < 1; nSite++) //drive edge is just the same on all sites -adrian
            {
                GlobalFunctions.BeginSiteLoop(nSite);

                //OriginalCode: If MuxType = 0 Then;
                //PseudoCode  : If: Condition: MuxType = 0;
                if (MuxType == 0)
                {
                    //Apply levels and timing

                    //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
                    Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If MuxType = 1 Then;
                //PseudoCode  : If: Condition: MuxType = 1;
                if (MuxType == 1)
                {
                    //Apply levels and timing

                    //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
                    //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);
                    Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: false, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);   //DigPins already connected -adrian

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Odd_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Odd_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase3);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase3);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase3);

                    //OriginalCode: D1Edge_Odd_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase3);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase3);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase3);

                    //OriginalCode: D1Edge_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase4);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase4);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase4);

                    //OriginalCode: D1Edge_Odd_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase4);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase4);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase4);

                    //OriginalCode: D1Edge_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If MuxType = 2 Then;
                //PseudoCode  : If: Condition: MuxType = 2;
                if (MuxType == 2)
                {
                    //Apply levels and timing

                    //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
                    Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Odd_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase1 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase1; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Odd_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase3);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase3);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase3);

                    //OriginalCode: D1Edge_Odd_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase3);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase3);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase3);

                    //OriginalCode: D1Edge_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase3 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase3; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase4);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase4);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase4);

                    //OriginalCode: D1Edge_Odd_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase4);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase4);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase4);

                    //OriginalCode: D1Edge_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If MuxType = 3 Then;
                //PseudoCode  : If: Condition: MuxType = 3;
                if (MuxType == 3)
                {
                    //Apply levels and timing

                    //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
                    Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Odd_Phase1_2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase1_2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase1_2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase1_2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase1_2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase1_2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: D1Edge_Phase1_2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase1_2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase1_2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase1_2 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase1_2; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase1_2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxInPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(MuxInPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Odd_Phase3_4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Odd_Phase3_4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Odd_Phase3_4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Odd_Phase3_4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Odd_Phase3_4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Odd_Phase3_4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(InPinsArray[i]).ReadEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: D1Edge_Phase3_4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    //PseudoCode  : VariableAssignment: Name: D1Edge_Phase3_4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD1);
                    D1Edge_Phase3_4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD1];

                    //OriginalCode: D2Edge_Phase3_4 = TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    //PseudoCode  : VariableAssignment: Name: D2Edge_Phase3_4; AssignedValue: TheHdw.Digital.Timing.EdgeTime(chEdgeD2);
                    D2Edge_Phase3_4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeD2];

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                double minDelay = 1;
                //thehdw.Digital.Timing.Pins(InPinsArray(i)).ReadEdgeTimingRAM (TsetNamePhase2) 'remove

                //D1Edge_In_H = thehdw.Digital.Timing.EdgeTime(chEdgeD1) 'remove

                //D2Edge_In_H = thehdw.Digital.Timing.EdgeTime(chEdgeD2) 'remove

                //OriginalCode: For i = 0 To UBound(OutPinsArray);
                //PseudoCode  : ForLoop: VariableName: i; Start: 0; Stop: UBound(OutPinsArray); StepSize: 1;
                for (i = 0; i <= GlobalFunctions.UBound(OutPinsArray); i = i + 1)
                {
                    for (nSite = 0; nSite < Globals.tsmContext.SiteNumbers.Count; nSite++)
                    {
                        //OriginalCode: If i = 0 Then;
                        //PseudoCode  : If: Condition: i = 0;
                        //if (i == 0)
                        //{
                        //    //OriginalCode: Delay = PropDelayRise(i);
                        //    //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayRise(i);
                        //    Delay = Globals.PropDelayRise[i].Value[nsite];

                        //    //OriginalCode: End If;
                        //    //PseudoCode  : EndIf;
                        //}

                        //OriginalCode: If PropDelayRise(i) > Delay Then;
                        //PseudoCode  : If: Condition: PropDelayRise(i) > Delay;
                        if (Globals.PropDelayRise[i].Value[nSite] > Delay)
                        {
                            //OriginalCode: Delay = PropDelayRise(i);
                            //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayRise(i);
                            Delay = Globals.PropDelayRise[i].Value[nSite];

                            //OriginalCode: End If;
                            //PseudoCode  : EndIf;
                        }

                        //OriginalCode: If PropDelayFall(i) > Delay Then;
                        //PseudoCode  : If: Condition: PropDelayFall(i) > Delay;
                        if (Globals.PropDelayFall[i].Value[nSite] > Delay)
                        {
                            //OriginalCode: Delay = PropDelayFall(i);
                            //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayFall(i);
                            Delay = Globals.PropDelayFall[i].Value[nSite];

                            //OriginalCode: End If;
                            //PseudoCode  : EndIf;
                        }

                        //OriginalCode: Next i;
                        //PseudoCode  : EndForLoop: VariableName: i;

                        //Debug - adrian
                        //if (i == 0)
                        //{
                        //    //OriginalCode: Delay = PropDelayRise(i);
                        //    //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayRise(i);
                        //    minDelay = Globals.PropDelayRise[i].Value[nsite];

                        //    //OriginalCode: End If;
                        //    //PseudoCode  : EndIf;
                        //}

                        //OriginalCode: If PropDelayRise(i) > Delay Then;
                        //PseudoCode  : If: Condition: PropDelayRise(i) > Delay;
                        if (Globals.PropDelayRise[i].Value[nSite] < minDelay)
                        {
                            //OriginalCode: Delay = PropDelayRise(i);
                            //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayRise(i);
                            minDelay = Globals.PropDelayRise[i].Value[nSite];

                            //OriginalCode: End If;
                            //PseudoCode  : EndIf;
                        }

                        //OriginalCode: If PropDelayFall(i) > Delay Then;
                        //PseudoCode  : If: Condition: PropDelayFall(i) > Delay;
                        if (Globals.PropDelayFall[i].Value[nSite] < minDelay)
                        {
                            //OriginalCode: Delay = PropDelayFall(i);
                            //PseudoCode  : VariableAssignment: Name: Delay; AssignedValue: PropDelayFall(i);
                            minDelay = Globals.PropDelayFall[i].Value[nSite];

                            //OriginalCode: End If;
                            //PseudoCode  : EndIf;
                        }
                    }
                }


                Delay = (minDelay + Delay) / 2;


                //OriginalCode: i = 0;
                //PseudoCode  : VariableAssignment: Name: i; AssignedValue: 0;
                i = 0;

                //OriginalCode: With TheHdw.Digital.Timing;
                //No Muxing

                //OriginalCode: If MuxType = 0 Then;
                //PseudoCode  : If: Condition: MuxType = 0;
                if (MuxType == 0)
                {
                    //OriginalCode: If (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0);
                    if ((Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: If (Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0);
                    if ((Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns + Period / 2

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //Muxing Inputs

                /* Change of compare timing computation due to change in timing and pattern on 75MHz Func -adrian
                
                //OriginalCode: If MuxType = 1 Then;
                //PseudoCode  : If: Condition: MuxType = 1;
                if (MuxType == 1)
                {
                    //OriginalCode: If (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 < 0);
                    if ((Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period / 4;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: If (Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0);
                    if ((Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2);
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2);
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase2 - ((D2Edge_Phase2 - D1Edge_Phase2) / 2);

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: If (Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 < 0);
                    if ((Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase3 - ((D2Edge_Phase3 - D1Edge_Phase3) / 2) + Period / 4;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase3);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase3);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase3);

                    //OriginalCode: If (Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 < 0);
                    if ((Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase4 - ((D2Edge_Phase4 - D1Edge_Phase4) / 2) - Period / 2;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase4);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase4);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase4);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                */

                //Debug; Monitor compare strobe -adrian
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase1);
                //double VOA_TP1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase2);
                //double VOA_TP2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase3);
                //double VOA_TP3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase4);
                //double VOA_TP4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];

                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase1);
                //double VOB_TP1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase2);
                //double VOB_TP2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase3);
                //double VOB_TP3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase4);
                //double VOB_TP4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];

                //New compare timing computation -adrian
                if (MuxType == 1)
                {
                    //Initialize to avoid unwanted change in compare strobe timing value
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //Set tset_phase1 data_out compare strobe timing
                    if ((Delay + ((D2Edge_Phase1 - D1Edge_Phase1) / 2)) > Period)
                    {
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase1 - D1Edge_Phase1) / 2) - Period;
                    }
                    else
                    {
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase1 - D1Edge_Phase1) / 2);
                    }
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase3); //Ph1 and Ph3 are now the same

                    //Set tset_phase2 data_out compare strobe timing
                    if ((Delay + ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2) > Period)
                    {
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2 - Period;
                    }
                    else
                    {
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase2 - D1Edge_Phase2) / 2) + Period / 2;
                    }
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase4); //Ph2 and Ph4 are now the same

                    ////Set tset_phase3 data_out compare strobe timing
                    //if ((Delay + ((D2Edge_Phase3 - D1Edge_Phase3) / 2)) > Period)
                    //{
                    //    Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase3 - D1Edge_Phase3) / 2) - Period;
                    //}
                    //else
                    //{
                    //    Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase3 - D1Edge_Phase3) / 2);
                    //}
                    //Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase3);

                    ////Set tset_phase4 data_out compare strobe timing
                    //if ((Delay + ((D2Edge_Phase4 - D1Edge_Phase4) / 2) + Period / 2) > Period)
                    //{
                    //    Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase4 - D1Edge_Phase4) / 2) + Period / 2 - Period;
                    //}
                    //else
                    //{
                    //    Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + ((D2Edge_Phase4 - D1Edge_Phase4) / 2) + Period / 2;
                    //}
                    //Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase4);
                }

                //Debug; Monitor compare strobe -adrian
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase1);
                //double post_VOA_TP1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase2);
                //double post_VOA_TP2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase3);
                //double post_VOA_TP3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOA").ReadEdgeTimingRAM(TsetNamePhase4);
                //double post_VOA_TP4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];

                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase1);
                //double post_VOB_TP1 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase2);
                //double post_VOB_TP2 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase3);
                //double post_VOB_TP3 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];
                //Globals.TheHdw.Digital.Timing.Pins("VOB").ReadEdgeTimingRAM(TsetNamePhase4);
                //double post_VOB_TP4 = Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0];


                //Muxing Outputs

                //OriginalCode: If MuxType = 2 Then;
                //PseudoCode  : If: Condition: MuxType = 2;
                if (MuxType == 2)
                {
                    //OriginalCode: If (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0);
                    if ((Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2);

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(MuxOutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: If (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0);
                    if ((Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns + Period / 2

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1 - ((D2Edge_Phase1 - D1Edge_Phase1) / 2) + Period / 2;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //Muxing Input and Outputs. Appears to work!

                //OriginalCode: If MuxType = 3 Then;
                //PseudoCode  : If: Condition: MuxType = 3;
                if (MuxType == 3)
                {
                    //OriginalCode: If (Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) < 0);
                    if ((Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns + Period / 4

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Odd_Phase1_2 - ((D2Edge_Odd_Phase1_2 - D1Edge_Odd_Phase1_2) / 2) + Period / 4;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(MuxOutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: If (Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) < 0);
                    if ((Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2) + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2);
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2);
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase1_2 - ((D2Edge_Phase1_2 - D1Edge_Phase1_2) / 2);

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase1);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase1);

                    //OriginalCode: If (Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 < 0);
                    if ((Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Odd_Phase3_4 - ((D2Edge_Odd_Phase3_4 - D1Edge_Odd_Phase3_4) / 2) - Period / 4;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(MuxOutPins).WriteEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(MuxOutPins).WriteEdgeTimingRAM(TsetNamePhase2);

                    //OriginalCode: If (Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 < 0) Then;
                    //PseudoCode  : If: Condition: (Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 < 0);
                    if ((Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 < 0))
                    {
                        //.EdgeTime(chEdgeR0) = 0 * ns

                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 + Period;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 + Period;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2 + Period;

                        //OriginalCode: Else;
                        //PseudoCode  : Else;
                    }
                    else
                    {
                        //OriginalCode: .EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2;
                        //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.EdgeTime(chEdgeR0) = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2;
                        Globals.TheHdw.Digital.Timing.EdgeTime[Globals.chEdgeR0] = Delay + D1Edge_Phase3_4 - ((D2Edge_Phase3_4 - D1Edge_Phase3_4) / 2) - Period / 2;

                        //OriginalCode: End If;
                        //PseudoCode  : EndIf;
                    }

                    //OriginalCode: .Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase2);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);
                    Globals.TheHdw.Digital.Timing.Pins(OutPins).WriteEdgeTimingRAM(TsetNamePhase2);

                    //.EdgeTime(chEdgeR0) = Delay + D1Edge_In_H + ((D2Edge_In_H - D1Edge_In_H) / 2)

                    //.Pins(OutPins).WriteEdgeTimingRAM (TsetNamePhase2)

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End With;
                //OriginalCode: Call TheHdw.Digital.Patterns.Pat(ThePat).Test(pfAlways, 1);
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.Patterns.Pat(ThePat).Test(pfAlways, 1);
                Globals.TheHdw.Digital.Patterns.Pat(ThePat).Test(Globals.pfAlways, 1);

                //OriginalCode: Next nSite;
                //PseudoCode  : EndForLoop: VariableName: nSite;
                GlobalFunctions.EndSiteLoop(nSite);
            }

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);  //not needed -adrian

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void PpmuForcevMeasi(PinList TestPins, string MeasureType, PinList DriveLoPins, PinList DriveHiPins, PinList VDD1TestPins, double VDD1_value, double idd1_value, PinList VDD2TestPins, double VDD2_value, double idd2_value, PinList PrebodyUtil1Pins, PinList PrebodyUtil0Pins, PinList PostbodyUtil1Pins, PinList PostbodyUtil0Pins)
        {
            //OriginalCode: Dim Iil_VDD1 As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: Iil_VDD1; Type: New PinListData;
            PinListData Iil_VDD1 = new PinListData();

            //OriginalCode: Dim Iih_VDD1 As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: Iih_VDD1; Type: New PinListData;
            PinListData Iih_VDD1 = new PinListData();

            //OriginalCode: Dim Iil_VDD2 As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: Iil_VDD2; Type: New PinListData;
            PinListData Iil_VDD2 = new PinListData();

            //OriginalCode: Dim Iih_VDD2 As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: Iih_VDD2; Type: New PinListData;
            PinListData Iih_VDD2 = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //OriginalCode: If NonBlank(PrebodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil1Pins);
            if (NonBlank(PrebodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PrebodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PrebodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PrebodyUtil0Pins);
            if (NonBlank(PrebodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PrebodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PrebodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //Set supplies

            //OriginalCode: Call PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);
            //PseudoCode  : FunctionCall: Name: PowerOn; ArgumentList: VDD1_value, idd1_value, VDD2_value, idd2_value;
            PowerOn(VDD1_value, idd1_value, VDD2_value, idd2_value);

            //Apply Levels and Timing ***What should be the fields set below***

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming False, True, False, tlPowered, initpinsHiz:=TestPins.Value;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(False, True, False, tlPowered, initpinsHiz:=TestPins.Value);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: false, loadLevels: true, loadTiming: false, RelayMode: Globals.tlPowered, InitPinsHi: TestPins.Value, InitPinsLo: null, InitPinsHiz: null);  //not needed PPMU will be used for test -adrian

            //Set DUT inputs to Hi-Z

            //OriginalCode: TheHdw.Pins(TestPins).ForceStaticLevel chStaticStateHiZ;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins(TestPins).ForceStaticLevel(chStaticStateHiZ);
            //Globals.TheHdw.Pins.Pins(TestPins).ForceStaticLevel(Globals.chStaticStateHiZ);    //not needed PPMU will be used for test -adrian

            //OriginalCode: If NonBlank(DriveLoPins) Then;
            //PseudoCode  : If: Condition: NonBlank(DriveLoPins);
            if (NonBlank(DriveLoPins))
            {
                //OriginalCode: TheHdw.Pins(DriveLoPins).ForceStaticLevel chStaticStateLo;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins(DriveLoPins).ForceStaticLevel(chStaticStateLo);
                Globals.TheHdw.Pins.Pins(DriveLoPins).ForceStaticLevel(Globals.chStaticStateLo);

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(DriveHiPins) Then;
            //PseudoCode  : If: Condition: NonBlank(DriveHiPins);
            if (NonBlank(DriveHiPins))
            {
                //OriginalCode: TheHdw.Pins(DriveHiPins).ForceStaticLevel chStaticStateHi;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pi ns(DriveHiPins).ForceStaticLevel(chStaticStateHi);
                Globals.TheHdw.Pins.Pins(DriveHiPins).ForceStaticLevel(Globals.chStaticStateHi);

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: TheHdw.Digital.DisconnectPins (TestPins);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins (TestPins);
            //Globals.TheHdw.Digital.DisconnectPins(TestPins);

            //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iil") Then;
            //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iil");
            if ((MeasureType == "Both") || (MeasureType == "Iil"))
            {
                //OriginalCode: If NonBlank(VDD1TestPins) Then;
                //PseudoCode  : If: Condition: NonBlank(VDD1TestPins);
                if (NonBlank(VDD1TestPins))
                {
                    //OriginalCode: With TheHdw.PPMU.Pins(VDD1TestPins);
                    //OriginalCode: .ForceV 0, 0.00002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).ForceV(0, 0.00002);
                    //Globals.TheHdw.PPMU.Pins(VDD1TestPins).ForceV(0, 0.00002);
                    Globals.TheHdw.PPMU.Pins(VDD1TestPins).ForceV(0, 0.000002); //corr -adrian

                    //OriginalCode: TheHdw.Wait 0.001;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
                    Globals.TheHdw.Wait(0.001);

                    //OriginalCode: .Connect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).Connect;
                    //Globals.TheHdw.PPMU.Pins(VDD1TestPins).Connect();

                    //OriginalCode: TheHdw.Wait 0.002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.002);
                    //Globals.TheHdw.Wait(0.002);

                    //OriginalCode: Iil_VDD1 = .Read(tlPPMUReadMeasurements);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: Iil_VDD1 = TheHdw.PPMU.Pins(VDD1TestPins).Read(tlPPMUReadMeasurements);
                    //Iil_VDD1 = Globals.TheHdw.PPMU.Pins(VDD1TestPins).Read(Globals.tlPPMUReadMeasurements);   //measure current -adrian
                    Iil_VDD1 = Globals.TheHdw.PPMU.Pins(VDD1TestPins).Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Current);

                    //OriginalCode: .Disconnect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).Disconnect;
                    //Globals.TheHdw.PPMU.Pins(VDD1TestPins).Disconnect();

                    //OriginalCode: End With;
                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If NonBlank(VDD2TestPins) Then;
                //PseudoCode  : If: Condition: NonBlank(VDD2TestPins);
                if (NonBlank(VDD2TestPins))
                {
                    //OriginalCode: With TheHdw.PPMU.Pins(VDD2TestPins);
                    //OriginalCode: .ForceV 0, 0.00002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).ForceV(0, 0.00002);
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).ForceV(0, 0.00002);

                    //OriginalCode: TheHdw.Wait 0.001;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
                    Globals.TheHdw.Wait(0.001);

                    //OriginalCode: .Connect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).Connect;
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).Connect();

                    //OriginalCode: TheHdw.Wait 0.002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.002);
                    Globals.TheHdw.Wait(0.002);

                    //OriginalCode: Iil_VDD2 = .Read(tlPPMUReadMeasurements);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: Iil_VDD2 = TheHdw.PPMU.Pins(VDD2TestPins).Read(tlPPMUReadMeasurements);
                    //Iil_VDD2 = Globals.TheHdw.PPMU.Pins(VDD2TestPins).Read(Globals.tlPPMUReadMeasurements);   //measure current -adrian
                    Iil_VDD2 = Globals.TheHdw.PPMU.Pins(VDD2TestPins).Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Current);

                    //OriginalCode: .Disconnect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).Disconnect;
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).Disconnect();

                    //OriginalCode: End With;
                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iih") Then;
            //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iih");
            if ((MeasureType == "Both") || (MeasureType == "Iih"))
            {
                //OriginalCode: If NonBlank(VDD1TestPins) Then;
                //PseudoCode  : If: Condition: NonBlank(VDD1TestPins);
                if (NonBlank(VDD1TestPins))
                {
                    //OriginalCode: With TheHdw.PPMU.Pins(VDD1TestPins);
                    //OriginalCode: .ForceV VDD1_value, 0.00002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).ForceV(VDD1_value, 0.00002);
                    //Globals.TheHdw.PPMU.Pins(VDD1TestPins).ForceV(VDD1_value, 0.00002);
                    Globals.TheHdw.PPMU.Pins(VDD1TestPins).ForceV(VDD1_value, 0.000002);    //corr -adrian

                    //OriginalCode: TheHdw.Wait 0.001;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
                    Globals.TheHdw.Wait(0.001);

                    //OriginalCode: .Connect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).Connect;
                    //Globals.TheHdw.PPMU.Pins(VDD1TestPins).Connect();

                    //OriginalCode: TheHdw.Wait 0.002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.002);
                    //Globals.TheHdw.Wait(0.002);

                    //OriginalCode: Iih_VDD1 = .Read(tlPPMUReadMeasurements);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: Iih_VDD1 = TheHdw.PPMU.Pins(VDD1TestPins).Read(tlPPMUReadMeasurements);
                    //Iih_VDD1 = Globals.TheHdw.PPMU.Pins(VDD1TestPins).Read(Globals.tlPPMUReadMeasurements);   //measure current -adrian
                    Iih_VDD1 = Globals.TheHdw.PPMU.Pins(VDD1TestPins).Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Current);

                    //OriginalCode: .Disconnect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD1TestPins).Disconnect;
                    Globals.TheHdw.PPMU.Pins(VDD1TestPins).Disconnect();

                    //OriginalCode: End With;
                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If NonBlank(VDD2TestPins) Then;
                //PseudoCode  : If: Condition: NonBlank(VDD2TestPins);
                if (NonBlank(VDD2TestPins))
                {
                    //OriginalCode: With TheHdw.PPMU.Pins(VDD2TestPins);
                    //OriginalCode: .ForceV VDD2_value, 0.00002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).ForceV(VDD2_value, 0.00002);
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).ForceV(VDD2_value, 0.00002);

                    //OriginalCode: TheHdw.Wait 0.001;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
                    Globals.TheHdw.Wait(0.001);

                    //OriginalCode: .Connect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).Connect;
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).Connect();

                    //OriginalCode: TheHdw.Wait 0.002;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.002);
                    Globals.TheHdw.Wait(0.002);

                    //OriginalCode: Iih_VDD2 = .Read(tlPPMUReadMeasurements);
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: Iih_VDD2 = TheHdw.PPMU.Pins(VDD2TestPins).Read(tlPPMUReadMeasurements);
                    //Iih_VDD2 = Globals.TheHdw.PPMU.Pins(VDD2TestPins).Read(Globals.tlPPMUReadMeasurements);   //measure current - adrian
                    Iih_VDD2 = Globals.TheHdw.PPMU.Pins(VDD2TestPins).Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Current);

                    //OriginalCode: .Disconnect;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(VDD2TestPins).Disconnect;
                    Globals.TheHdw.PPMU.Pins(VDD2TestPins).Disconnect();

                    //OriginalCode: End With;
                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: TheHdw.Digital.ConnectPins (TestPins);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ConnectPins (TestPins);
            //Globals.TheHdw.Digital.ConnectPins(TestPins);

            //OriginalCode: If NonBlank(VDD1TestPins) Then;
            //PseudoCode  : If: Condition: NonBlank(VDD1TestPins);
            if (NonBlank(VDD1TestPins))
            {
                //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iil") Then;
                //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iil");
                if ((MeasureType == "Both") || (MeasureType == "Iil"))
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=Iil_VDD1, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Iil_VDD1, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: Iil_VDD1, ScaleType: Globals.scaleMicro, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iih") Then;
                //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iih");
                if ((MeasureType == "Both") || (MeasureType == "Iih"))
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=Iih_VDD1, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Iih_VDD1, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: Iih_VDD1, ScaleType: Globals.scaleMicro, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(VDD2TestPins) Then;
            //PseudoCode  : If: Condition: NonBlank(VDD2TestPins);
            if (NonBlank(VDD2TestPins))
            {
                //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iil") Then;
                //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iil");
                if ((MeasureType == "Both") || (MeasureType == "Iil"))
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=Iil_VDD2, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Iil_VDD2, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: Iil_VDD2, ScaleType: Globals.scaleMicro, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: If (MeasureType = "Both") Or (MeasureType = "Iih") Then;
                //PseudoCode  : If: Condition: (MeasureType = "Both") Or (MeasureType = "Iih");
                if ((MeasureType == "Both") || (MeasureType == "Iih"))
                {
                    //OriginalCode: TheExec.Flow.TestLimit resultval:=Iih_VDD2, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow;
                    //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=Iih_VDD2, ScaleType:=scaleMicro, unit:=unitAmp, ForceResults:=tlForceFlow);
                    Globals.TheExec.Flow.TestLimit(resultval: Iih_VDD2, ScaleType: Globals.scaleMicro, unit: Globals.unitAmp, ForceResults: Globals.tlForceFlow);

                    //OriginalCode: End If;
                    //PseudoCode  : EndIf;
                }

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //Set or clear postbody relays

            //OriginalCode: If NonBlank(PostbodyUtil1Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil1Pins);
            if (NonBlank(PostbodyUtil1Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil1Pins).State = tlUtilBitOn;
                Globals.TheHdw.Utility.Pins(PostbodyUtil1Pins).State = Globals.tlUtilBitOn;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

            //OriginalCode: If NonBlank(PostbodyUtil0Pins) Then;
            //PseudoCode  : If: Condition: NonBlank(PostbodyUtil0Pins);
            if (NonBlank(PostbodyUtil0Pins))
            {
                //OriginalCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Utility.Pins(PostbodyUtil0Pins).State = tlUtilBitOff;
                Globals.TheHdw.Utility.Pins(PostbodyUtil0Pins).State = Globals.tlUtilBitOff;

                //OriginalCode: End If;
                //PseudoCode  : EndIf;
            }

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void PowerDown()
        {
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            //Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB,VOA,VOB").ForceStaticLevel(Globals.chStaticStateLo);
            // ttb            Globals.TheHdw.PPMU.Pins((PinList)"VIA,VIB,VOB,VOA").Disconnect();

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1, VDD2");
            //OriginalCode: .Voltage = 0;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1, VDD2").Voltage = 0;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Voltage = 0;

            //OriginalCode: TheHdw.Wait 0.01;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.01);
            //Globals.TheHdw.Wait(0.01);
            Globals.TheHdw.Wait(0.003);

            //OriginalCode: .Gate = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1, VDD2").Gate = False;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = false;

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Disconnect (tlDCVIConnectDefault);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1, VDD2").Disconnect (tlDCVIConnectDefault);
            //Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Disconnect(Globals.tlDCVIConnectDefault);   //no need to disconnect on Main Seq -adrian

            Relay.ControlRelay(Globals.tsmContext, new string[] { "K1", "K2", "K3", "K4" }, false); //disconnect all relays -adrian

            //OriginalCode: End With;
            //OriginalCode: End Function;
            //PseudoCode  : FunctionEnd;
#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
        }
        public static void continuity_pd(PinList TestPins)
        {
            //OriginalCode: Dim pos_diode As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: pos_diode; Type: New PinListData;
            PinListData pos_diode = new PinListData();

            //OriginalCode: Dim neg_diode As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: neg_diode; Type: New PinListData;
            PinListData neg_diode = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //Set DUT supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .Mode = tlDCVIModeVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Mode = tlDCVIModeVoltage;
            //Globals.TheHdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeVoltage;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Mode = Globals.tlDCVIModeVoltage; //Run VDD1 and VDD2 setting in parallel -adrian

            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD1 cty

            //OriginalCode: .SetVoltageAndRange 0#, 5;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(0, 5);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(0, 5);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").SetVoltageAndRange(0, 5); //Run VDD1 and VDD2 setting in parallel -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);	//PXIe-4162 max current is only 100mA -adrian
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").SetCurrentAndRange(0.1, 0.1); //Run VDD1 and VDD2 setting in parallel -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false;

            //settle wait to prevent hotswitching condition

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = true;  //Run VDD1 and VDD2 setting in parallel -adrian

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .Mode = tlDCVIModeVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Mode = tlDCVIModeVoltage;
            //Globals.TheHdw.DCVI.Pins("VDD2").Mode = Globals.tlDCVIModeVoltage;

            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD2 cty

            //OriginalCode: .SetVoltageAndRange 0#, 5;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(0, 5);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(0, 5);

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);	//PXIe-4162 max current is only 100mA -adrian
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.1, 0.1);

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false;

            //settle wait to prevent hotswitching condition

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true;

            //OriginalCode: End With;
            //Apply Levels and Timing

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming False, True, False, tlPowered, initpinsHiz:=TestPins.Value;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(False, True, False, tlPowered, initpinsHiz:=TestPins.Value);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: false, loadLevels: true, loadTiming: false, RelayMode: Globals.tlPowered, InitPinsHi: TestPins.Value, InitPinsLo: null, InitPinsHiz: null);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: false, RelayMode: Globals.tlPowered, InitPinsHi: TestPins.Value, InitPinsLo: null, InitPinsHiz: null); //connect all digital pins -adrian

            //Connect PPMU's

            //OriginalCode: With TheHdw.PPMU.Pins(TestPins);
            //OriginalCode: .Connect;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).Connect;
            Globals.TheHdw.PPMU.Pins(TestPins).Connect();

            //OriginalCode: End With;
            //OriginalCode: TheHdw.PPMU.Pins(TestPins).ForceI 0.0005, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).ForceI(0.0005, 2 * mA);
            Globals.TheHdw.PPMU.Pins(TestPins).ForceI(0.0005, 2 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.002);

            //OriginalCode: pos_diode = TheHdw.PPMU.Pins(TestPins).Read;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: pos_diode = TheHdw.PPMU.Pins(TestPins).Read();
            pos_diode = Globals.TheHdw.PPMU.Pins(TestPins).Read();

            //OriginalCode: TheHdw.PPMU.Pins(TestPins).ForceI -0.0005, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).ForceI(-0.0005, 2 * mA);
            Globals.TheHdw.PPMU.Pins(TestPins).ForceI(-0.0005, 2 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: neg_diode = TheHdw.PPMU.Pins(TestPins).Read;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: neg_diode = TheHdw.PPMU.Pins(TestPins).Read();
            neg_diode = Globals.TheHdw.PPMU.Pins(TestPins).Read();

            //OriginalCode: With TheHdw.PPMU.Pins(TestPins);
            //OriginalCode: .ForceI (0);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).ForceI (0);
            Globals.TheHdw.PPMU.Pins(TestPins).ForceI(0);

            //OriginalCode: .ForceV (0);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).ForceV (0);
            //Globals.TheHdw.PPMU.Pins(TestPins).ForceV(0);

            //OriginalCode: .Disconnect;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.PPMU.Pins(TestPins).Disconnect;
            //Globals.TheHdw.PPMU.Pins(TestPins).Disconnect();

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Digital.DisconnectPins (TestPins);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins (TestPins);
            Globals.TheHdw.Digital.DisconnectPins(TestPins);


            //VDD continuity test; added test for supply isolation and mis-orientation 
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = false;
            Globals.TheHdw.Wait(0.001);

            Globals.VDD1MeterMode = Globals.tlDCVIMeterVoltage;
            Globals.VDD2MeterMode = Globals.tlDCVIMeterVoltage;

            Globals.TheHdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeCurrent;
            Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(-0.003, 0.1);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = true;
            Globals.TheHdw.Wait(0.001);

            Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(-0.0005, 0.1);
            Globals.TheHdw.Wait(0.001);

            PinListData VDD1_cont = Globals.TheHdw.DCVI.Pins("VDD1").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);


            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = false;

            Globals.TheHdw.Wait(0.001);

            Globals.TheHdw.DCVI.Pins("VDD1").Mode = Globals.tlDCVIModeVoltage;
            Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(0, 5);
            Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);

            Globals.TheHdw.DCVI.Pins("VDD2").Mode = Globals.tlDCVIModeCurrent;
            Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(-0.003, 0.1);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = true;
            Globals.TheHdw.Wait(0.001);

            Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(-0.0005, 0.1);
            Globals.TheHdw.Wait(0.001);

            PinListData VDD2_cont = Globals.TheHdw.DCVI.Pins("VDD2").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = false;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Mode = Globals.tlDCVIModeVoltage;
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").SetVoltageAndRange(0, 5);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").SetCurrentAndRange(0.1, 0.1);

            Globals.TheHdw.Wait(0.003);
            Globals.TheHdw.DCVI.Pins("VDD1,VDD2").Gate = true;


            //OriginalCode: TheExec.Flow.TestLimit resultval:=pos_diode, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=pos_diode, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: pos_diode, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=neg_diode, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=neg_diode, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: neg_diode, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //VDD continuity test; added test for supply isolation and mis-orientation 
            Globals.TheExec.Flow.TestLimit(resultval: VDD1_cont, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VDD2_cont, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow); //Corrected Datalogging for VDD2_cont
            

            //Set or clear postbody relays

#if TestTimeMeasure
                    Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_1p7_1p7_pd()
        {
            //OriginalCode: Dim VOL_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_20uA; Type: New PinListData;
            PinListData VOL_20uA = new PinListData();

            //OriginalCode: Dim VOL_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_2mA; Type: New PinListData;
            PinListData VOL_2mA = new PinListData();

            //OriginalCode: Dim VOL_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_4mA; Type: New PinListData;
            PinListData VOL_4mA = new PinListData();

            //OriginalCode: Dim VOH_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_20uA; Type: New PinListData;
            PinListData VOH_20uA = new PinListData();

            //OriginalCode: Dim VOH_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_2mA; Type: New PinListData;
            PinListData VOH_2mA = new PinListData();

            //OriginalCode: Dim VOH_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_4mA; Type: New PinListData;
            PinListData VOH_4mA = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //Set supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;    //NA on GP3 -adrian

            //VDD1 1p7/1p7

            //OriginalCode: .SetVoltageAndRange 1.7, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(1.7, 10);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(1.7, 10);   //Already set on the last test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);	//PXIe-4162 max current is only 100mA -adrian
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);    //Already set on the last test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false; //no equivalent function in GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;    //NA on GP3 -adrian

            //VDD2 1p7/1p7

            //OriginalCode: .SetVoltageAndRange 1.7, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(1.7, 10);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(1.7, 10); //Already set on the last test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);  //PXIe-4162 max current is only 100mA -adrian
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.1, 0.1);    //Already set on the last test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false; //no equivalent function in GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //Apply Levels and Timing ***What should be the fields set below***

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

            //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent;

            //OriginalCode: .SetCurrentAndRange 20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 2 * Globals.mA);	// can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);


            //OriginalCode: .SetVoltageAndRange 10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(10, 10);   //PXIe-4162 has fixed 24V range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;    //no equivalent function in GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500; //no equivalent function in GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //connection will be handled by HMOD -adrian

            //HMOD Control; Connect VI/VO dc30 connection -adrian
            //HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_23: 34816); //S0 only for debug -adrian
            //HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_2: 263936);    //S0 only for debug -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);    //added bool argument for meter.read overload for pinlistdata -adrian
            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOL_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateHi;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set before measuring VOL -adrian

            //OriginalCode: .SetCurrentAndRange -20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 2 * Globals.mA);	// can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange -10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(-10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(-10, 10);  //Forcing Current, change in voltage is not needed -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //already set before measuring VOL -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //no equivalent function in GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //no equivalent function in GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;  //gate is already on -adrian

            //OriginalCode: TheHdw.Wait 1 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(1 * mS);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);	//added bool argument for meter.read overload for pinlistdata -adrian
            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOH_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Gate = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = False;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Disconnect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Disconnect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Disconnect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);  //disconnection will be handled by HMOD -adrian

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_1p7_1p7_pd_DOE1()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;


            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_1p7_1p7_pd_DOE2()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);


            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_1p7_1p7_pd_DOE3()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(0 * Globals.uA, 10 * Globals.uA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection

            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
            return;
        }
        public static void input_output_levels_2p25_2p25_pd()
        {
            //OriginalCode: Dim VOL_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_20uA; Type: New PinListData;
            PinListData VOL_20uA = new PinListData();

            //OriginalCode: Dim VOL_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_2mA; Type: New PinListData;
            PinListData VOL_2mA = new PinListData();

            //OriginalCode: Dim VOL_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_4mA; Type: New PinListData;
            PinListData VOL_4mA = new PinListData();

            //OriginalCode: Dim VOH_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_20uA; Type: New PinListData;
            PinListData VOH_20uA = new PinListData();

            //OriginalCode: Dim VOH_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_2mA; Type: New PinListData;
            PinListData VOH_2mA = new PinListData();

            //OriginalCode: Dim VOH_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_4mA; Type: New PinListData;
            PinListData VOH_4mA = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //Set supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD1 2p25/2p25

            //OriginalCode: .SetVoltageAndRange 2.25, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(2.25, 10);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(2.25, 10);    //Already set on the previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);    //Already set on the previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already set on the previous test -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD2 2p25/2p25

            //OriginalCode: .SetVoltageAndRange 2.25, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(2.25, 10);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(2.25, 10);    //Already set on the previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.1, 0.1);    //Already set on the previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);  //Already set on the previous test -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //Apply Levels and Timing ***What should be the fields set below***

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

            //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set -adrian

            //OriginalCode: .SetCurrentAndRange 20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 2 * Globals.mA);    // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);    // can't set 20uA level range on 10mA range -adrian

            //OriginalCode: .SetVoltageAndRange 10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(10, 10);

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not Applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not Applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //Connection will be handled by HMOD -adrian

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_23: 34816); //S0 only for debug -adrian
            //HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_2: 263936);    //S0 only for debug -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOL_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateHi;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set -adrian

            //OriginalCode: .SetCurrentAndRange -20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 2 * Globals.mA);   // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange -10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(-10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(-10, 10);  //4162/4163 has fix 24V voltage range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;  //gate is already on -adrian

            //OriginalCode: TheHdw.Wait 1 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(1 * mS);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOH_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Gate = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = False;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Disconnect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Disconnect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Disconnect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);  //disconnection will be handled by HMOD -adrian

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_2p25_2p25_pd_DOE1()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData();
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);


            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_2p25_2p25_pd_DOE2()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_2p25_2p25_pd_DOE3()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(0 * Globals.uA, 10 * Globals.uA);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection

            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
            return;
        }
        public static void input_output_levels_3p0_3p0_pd()
        {
            //OriginalCode: Dim VOL_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_20uA; Type: New PinListData;
            PinListData VOL_20uA = new PinListData();

            //OriginalCode: Dim VOL_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_2mA; Type: New PinListData;
            PinListData VOL_2mA = new PinListData();

            //OriginalCode: Dim VOL_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_4mA; Type: New PinListData;
            PinListData VOL_4mA = new PinListData();

            //OriginalCode: Dim VOH_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_20uA; Type: New PinListData;
            PinListData VOH_20uA = new PinListData();

            //OriginalCode: Dim VOH_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_2mA; Type: New PinListData;
            PinListData VOH_2mA = new PinListData();

            //OriginalCode: Dim VOH_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_4mA; Type: New PinListData;
            PinListData VOH_4mA = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //Set supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD1 3p0/3p0

            //OriginalCode: .SetVoltageAndRange 3#, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(3, 10);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(3, 10);   //Already set on the previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);    //Already set on the previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD2 3p0/3p0

            //OriginalCode: .SetVoltageAndRange 3#, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(3, 10);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(3, 10);   //Already set on the previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.1, 0.1);    //Already set on the previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //Apply Levels and Timing ***What should be the fields set below***

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

            //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set -adrian

            //OriginalCode: .SetCurrentAndRange 20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 2 * Globals.mA);    // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange 10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(10, 10);   //4162/4163 has fixed 24V range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;    //already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //Connection will be handled by HMOD -adrian

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_23: 34816); //S0 only for debug -adrian
            //HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_2: 263936);    //S0 only for debug -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOL_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateHi;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set -adrian

            //OriginalCode: .SetCurrentAndRange -20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 2 * Globals.mA);   // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange -10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(-10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(-10, 10);  //4162/4163 has fixed 24V range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;  //Gate is alrady on -adrian

            //OriginalCode: TheHdw.Wait 1 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(1 * mS);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOH_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Gate = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = False;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Disconnect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Disconnect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Disconnect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);  //disconnection will be handled by HMOD -adrian

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_3p0_3p0_pd_DOE1()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData();
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);


            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_3p0_3p0_pd_DOE2()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_3p0_3p0_pd_DOE3()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(0 * Globals.uA, 10 * Globals.uA);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection

            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
            return;
        }
        public static void input_output_levels_4p5_4p5_pd()
        {
            //OriginalCode: Dim VOL_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_20uA; Type: New PinListData;
            PinListData VOL_20uA = new PinListData();

            //OriginalCode: Dim VOL_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_2mA; Type: New PinListData;
            PinListData VOL_2mA = new PinListData();

            //OriginalCode: Dim VOL_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOL_4mA; Type: New PinListData;
            PinListData VOL_4mA = new PinListData();

            //OriginalCode: Dim VOH_20uA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_20uA; Type: New PinListData;
            PinListData VOH_20uA = new PinListData();

            //OriginalCode: Dim VOH_2mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_2mA; Type: New PinListData;
            PinListData VOH_2mA = new PinListData();

            //OriginalCode: Dim VOH_4mA As New PinListData;
            //PseudoCode  : VariableDeclaration: Name: VOH_4mA; Type: New PinListData;
            PinListData VOH_4mA = new PinListData();

            //OriginalCode: On Error GoTo errHandler;
            //PseudoCode  : UntranslatedVBA: On Error GoTo errHandler;
            //***NOT CONVERTED***

            //Set or clear prebody relays

            //Set supplies

            //OriginalCode: With TheHdw.DCVI.Pins("VDD1");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD1").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD1 4p5/4p5

            //OriginalCode: .SetVoltageAndRange 4.5, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(4.5, 10);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetVoltageAndRange(4.5, 10); //Already set on previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD1").SetCurrentAndRange(0.1, 0.1);    //Already set on previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD1").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD1").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD1").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD1").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VDD2");
            //OriginalCode: .ComplianceRange(tlDCVIComplianceBoth) = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").ComplianceRange(tlDCVIComplianceBoth) = 10;
            //Globals.TheHdw.DCVI.Pins("VDD2").ComplianceRange[Globals.tlDCVIComplianceBoth] = 10;

            //VDD2 4p5/4p5

            //OriginalCode: .SetVoltageAndRange 4.5, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(4.5, 10);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetVoltageAndRange(4.5, 10); //Already set on the previous test -adrian

            //OriginalCode: .SetCurrentAndRange 0.2, 0.2;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.2, 0.2);
            //Globals.TheHdw.DCVI.Pins("VDD2").SetCurrentAndRange(0.1, 0.1);    //Already set on the previous test -adrian

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VDD2").Meter.Filter.Bypass = false; //Not applicable on GP3 -adrian

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VDD2").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VDD2").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VDD2").Gate = true; //Gate is already on -adrian

            //OriginalCode: End With;
            //Apply Levels and Timing ***What should be the fields set below***

            //OriginalCode: TheHdw.Digital.ApplyLevelsTiming True, True, True, tlPowered;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.ApplyLevelsTiming(True, True, True, tlPowered);
            //Globals.TheHdw.Digital.ApplyLevelsTiming(ConnectAllPins: true, loadLevels: true, loadTiming: true, RelayMode: Globals.tlPowered, InitPinsHi: null, InitPinsLo: null, InitPinsHiz: null);

            //OriginalCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Digital.DisconnectPins ("VOB_dc30_da, VOA_dc30_da");
            Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //already set -adrian

            //OriginalCode: .SetCurrentAndRange 20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 2 * Globals.mA);    // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange 10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(10, 10);   //4162/4163 has fixed 24V range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //Connection will be handled by HMOD -adrian

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_23: 34816); //S0 only for debug -adrian
            //HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_2: 263936);    //S0 only for debug -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange 4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOL_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOL_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateHi;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateHi);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Mode = tlDCVIModeCurrent;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Mode = tlDCVIModeCurrent;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Mode = Globals.tlDCVIModeCurrent; //Already set -adrian

            //OriginalCode: .SetCurrentAndRange -20 * uA, 2 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-20 * uA, 2 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 2 * Globals.mA);   // can't set 20uA level range on 10mA range -adrian
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);

            //OriginalCode: .SetVoltageAndRange -10, 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetVoltageAndRange(-10, 10);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetVoltageAndRange(-10, 10);  //4162/4163 has fixed 24V range -adrian

            //OriginalCode: .Meter.Mode = tlDCVIMeterVoltage;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Mode = tlDCVIMeterVoltage;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Mode = Globals.tlDCVIMeterVoltage;  //Already set -adrian

            //OriginalCode: .Meter.VoltageRange = 10;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.VoltageRange = 10;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.VoltageRange = 10;  //4162/4163 has fixed voltage range of 24V

            //OriginalCode: .Meter.Filter.Bypass = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter.Bypass = False;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter.Bypass = false;  //Not applicable on GP3 -adrian

            //OriginalCode: .Meter.Filter.Value = 500;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Filter = 500;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Filter = 500;   //Not applicable on GP3 -adrian

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.001);

            //OriginalCode: .Connect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Connect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Connect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense); //Already connected -adrian

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            //Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Gate = True;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = True;
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            //OriginalCode: TheHdw.Wait 1 * mS;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(1 * mS);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_20uA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_20uA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -2 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-2 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: VOH_2mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_2mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: .SetCurrentAndRange -4 * mA, 20 * mA;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").SetCurrentAndRange(-4 * mA, 20 * mA);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 20 * Globals.mA);
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.001);

            //OriginalCode: VOH_4mA = .Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //PseudoCode  : AsIsLineOfCode: LineOfCode: VOH_4mA = TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Meter.Read(tlStrobe, 10, 10000, tlDCVIMeterReadingFormatAverage);
            //VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage);
            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            //OriginalCode: End With;
            //OriginalCode: With TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da");
            //OriginalCode: .Gate = False;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Gate = False;
            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            //settle wait to prevent hotswitching

            //OriginalCode: TheHdw.Wait 0.001;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Wait(0.001);
            Globals.TheHdw.Wait(0.0005);

            //OriginalCode: .Disconnect tlDCVIConnectHighForce + tlDCVIConnectHighSense;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.DCVI.Pins("VOA_dc30_da, VOB_dc30_da").Disconnect(tlDCVIConnectHighForce + tlDCVIConnectHighSense);
            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Disconnect(Globals.tlDCVIConnectHighForce + Globals.tlDCVIConnectHighSense);    //disconnection will be handled by HMOD -adrian

            //OriginalCode: End With;
            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_4p5_4p5_pd_DOE1()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData();
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(20 * Globals.uA, 1 * Globals.mA);


            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection
            HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 2290614272, 34952, 0, 2290614272, 34952);  //VOA/VOAB back to digital connection

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_4p5_4p5_pd_DOE2()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            //Globals.TheHdw.Digital.DisconnectPins("VOB, VOA");
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            //HMOD Control -adrian
            //HMOD.HMODCtrl.HMOD14to18(Globals.tsmContext, 8912896, 136, 0, 8912896, 136);    //Disconnect VOA/VOB Digitals on HMOD

            //OriginalCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel chStaticStateLo;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheHdw.Pins("VIA, VIB").ForceStaticLevel(chStaticStateLo);
            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-20 * Globals.uA, 1 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-2 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(-4 * Globals.mA, 10 * Globals.mA);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.PPMU.Pins((PinList)"VOA,VOB").Read(Globals.tlPPMUReadMeasurements, NationalInstruments.ModularInstruments.NIDigital.PpmuMeasurementType.Voltage);

            //Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB,VOA,VOB").ForceStaticLevel(Globals.chStaticStateLo);
            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").Disconnect();

            Globals.TheHdw.Wait(0.001);

            Globals.TheHdw.Pins.Pins("VOA,VOB").ForceStaticLevel(Globals.chStaticStateHiZ);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOL_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_20uA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_2mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //OriginalCode: TheExec.Flow.TestLimit resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow;
            //PseudoCode  : AsIsLineOfCode: LineOfCode: TheExec.Flow.TestLimit(resultval:=VOH_4mA, ScaleType:=scaleNoScaling, unit:=unitVolt, ForceResults:=tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

            //Set or clear postbody relays

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif

            //OriginalCode: Exit Function;
            //PseudoCode  : FunctionExit;
            return;

            //OriginalCode: errHandler:;

            //PseudoCode  : FunctionEnd;
        }
        public static void input_output_levels_4p5_4p5_pd_DOE3()
        {
            PinListData VOL_20uA = new PinListData();
            PinListData VOL_2mA = new PinListData();
            PinListData VOL_4mA = new PinListData(); ;
            PinListData VOH_20uA = new PinListData();
            PinListData VOH_2mA = new PinListData();
            PinListData VOH_4mA = new PinListData();

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(0);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").SetCurrentAndRange(0 * Globals.uA, 10 * Globals.uA);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(20e-6, 2e-3);

            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext, HMOD_Data_19: 2281701376, HMOD_Data_20: 34816, HMOD_Data_22: 2281701376, HMOD_Data_23: 34816); //all sites
            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808462116, HMOD_Data_2: 2369280); //all sites

            Globals.TheHdw.Wait(0.0005);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = true;

            Globals.TheHdw.Wait(0.001);

            VOL_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOL_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOL_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            // ttb            Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateHi);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-20e-6, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_20uA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-2e-3, 2e-3);
            Globals.TheHdw.Wait(0.0005);

            VOH_2mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.PPMU.Pins((PinList)"VOB,VOA").ForceI(-4e-3, 32e-3);
            Globals.TheHdw.Wait(0.001);

            VOH_4mA = Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Meter.Read(Globals.tlStrobe, 10, 10000, Globals.tlDCVIMeterReadingFormatAverage, true);

            Globals.TheHdw.DCVI.Pins("VOA_dc30_da,VOB_dc30_da").Gate = false;

            Globals.TheHdw.Wait(0.0005);

            // ttb           Globals.TheHdw.Pins.Pins("VIA,VIB").ForceStaticLevel(Globals.chStaticStateLo);

            //HMOD Control -adrian
            HMOD.HMODCtrl.HMOD19to23(Globals.tsmContext); //Disconnect VOA/VOB to DC30 connection

            HMOD.HMODCtrl.HMOD1to4(Globals.tsmContext, HMOD_Data_1: 808452864, HMOD_Data_2: 768);   //Disconnect VOA/VOB to DC30 connection

            Globals.TheExec.Flow.TestLimit(resultval: VOL_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOL_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_20uA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_2mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);
            Globals.TheExec.Flow.TestLimit(resultval: VOH_4mA, ScaleType: Globals.scaleNoScaling, unit: Globals.unitVolt, ForceResults: Globals.tlForceFlow);

#if TestTimeMeasure
            Globals.TestTimeMeasure.ExportTestTimeResult();
#endif
            return;
        }

        public static void ConfigureVOHandVOLperSite(double[] VOH, double[] VOL, string pin)
        {
            var DigiSession = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, pin);

            // Start a loop that will go through each testing site one by one
            for (int i = 0; i < DigiSession.SSC.Length; i++)
            {
                // Get the specific pin controls for the current testing site we're looking at
                DigitalPinSet PinperSite = DigiSession.SSC[i].Session.PinAndChannelMap.GetPinSet(pin);

                // Find out which site number we're currently working with
                int currentSite = DigiSession.SSC[i].SiteNumbers[0];

                // Set the "low voltage" level for this site using the site number to pick the right value
                PinperSite.DigitalLevels.Vol = VOL[currentSite];

                // Set the "high voltage" level for this site using the site number to pick the right value  
                PinperSite.DigitalLevels.Voh = VOH[currentSite];
            }
        }
    }

}
