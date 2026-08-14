using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.LTX
{
    public class DUTName_TestSteps
    {

        #region Test 30: AbsMaxMicropwerIq
        public static void AbsMaxMicropwerIq(
            ISemiconductorModuleContext tsmContext
            )
        {
            //Setting these clamps is not part of test 30. It is actually done in a previous test.
            //We needed to add this calls at the top of this test so the default limits (0A, 0V, 0V) are overwritten
            //So we do not get the error thrown when programming the limit to 0.
            //Instrument_VI.SetVIClamps(ref Globals.vi1Limits, 0.01, 51, -2);
            //Instrument_VI.SetVIClamps(ref Globals.vi2Limits, 0.01, 46, -5);
            //Instrument_VI.SetVIClamps(ref Globals.vi3Limits, 0.09, 20, -2);
            /* Expecting AI to generate this code.
             * 
             * Possible challenges with the current AI implimentation
             * 1. How do we want to handle measuremnts?
             *    BIN, limits, and other test info is set as instructions which is significantly different than STS. Likely we
             *    want the AI to look at a group of instructions and parse this appropreately. Note most of the time measured value
             *    and limits are used directly, but there are times things are more complicated.  For example, in lines 11140 and 11210
             *    the measured value is manipulated before the evaluation. We believe this can be handled by having the measurement and
             *    publish at the top level as done in this code allowing the AI to add a line to perform math on the result as needed.
             *    Also note limits are occational calculated in the code, so this will need to be handled in our approach.
             * 2. Goto Statements
             *    The LTX code uses a substancial number of Goto statements. Given there is not direct support for this on STS, we need
             *    to align on the way to handle these.  Note several goto statements appear to jump into the middle of tests which is
             *    unlikely to make this easy in C# or TestStand.
             * 3. LTX does not explictly call out the pin that should be making a measurement
             *    Instead a pin is interally connected to the measure instrument and a measurement is performed. On STS, the measurement
             *    will be made with an SMU, thus it's critical the AI passes in the proper session to the measurement function. Some cases
             *    will be easier than others:
             *      A. Line 4930 the pin used for the test can be seen a few lines above in line 4890
             *      B. Line 4860 we need to refer to the last time the mux was set 4310 in test #20, which is a bit trickier as the AI will
             *         need to keep track of the current state. 
             * 4. Additional changes from LTX code for STS
             *      A. Need pins to sessions at beginning of each LTX test
             *         Avoids having to duplicate this in every function which would be a significant test time increase
             *      B. Need publish IDs to be passed into publish functions
             *         Without unique publish IDs an LTX test would only be able to perform a specific type of measurement once per pin
             *         within a given LTX tests. The would prevent typical tests like leakage (i.e. multiple current measuremnts on the
             *         same pin) from working as expected.
             * 5. Additional Circuitry
             *    ADI is adding additional circuits to the interposer board (ex. pulsing curcuit). These will be needed for certain types
             *    of tests. We do not believe any action is required at this time, but this is something we want to keep a close watch
             *    on as more information is availible.
             * 
             * Toddo
             * 1. Review Instrument_MM.connectMMtoVIAM(vi2Session, 0.0012)
             * 
             * Questions
             * 1. Do we want the LTX code listed as a comment above each line of code so ADI can debug without opening the LTX version?
             * 2. How do we set test numbers as LTX tests can have multiple evaluations and STS cannot have them all share a test number.*/

            // Sessions from pins used in the test
            DCPower vi1Session = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VI1");
            DCPower vi2Session = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VI2");
            DCPower vi3Session = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VI3");
            DCPower vi4Session = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VI4");

            DCPower vs1Session = InstrCtrl.DCPowerPinsToSessions(tsmContext, "VS1");

            // Test #30 - ABS MAX MICROPOWER IQ (I_VIN),(I_SHDN)
            /* Dev tools passes in the voltage and current limits with the force function so setting these values in the drive will
             * result and in them being overwitten.  To overcome this we are caching the voltage and current limts for this instruction
             * and allowing them to easily be passed into SetVIVoltage and SetVICurrent */

            // SET VI 4 CLAMPS TO VMAX 51 VMIN -5 IMAX 50MA */ 
            // Instrument_VI.SetVIClamps(ref Globals.vi4Limits, 0.05, 51, -5);

            Instrument_MM.ChangeDCPowerCurrentMax(vi1Session, 0.00024);
            
            // Recommend we pass the current range from a Teststand so it can be changed without recompiling the code
            Instrument_VI.SetVIVoltage(vi2Session, 10, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 15, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 25, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 35, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 38, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 39, Globals.vi2Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi2Session, 40, Globals.vi2Limits.currentLimit);

            Instrument_VI.SetVIVoltage(vi3Session, 15, Globals.vi3Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi3Session, 5, Globals.vi3Limits.currentLimit);
            Instrument_VI.SetVIVoltage(vi3Session, 0, Globals.vi3Limits.currentLimit);

            //Unsure what pin is VS1 - Changing to vi1Session so code will run
            Instrument_VS.SetVSVoltage(vs1Session, 5, Globals.vi1Limits.currentLimit);

            System.Threading.Thread.Sleep(5);
            
            /* Given math and other things can be performed on meaurement results, we are storing the result in varaible so the AI
             * can manipulate the result if needed and then we are publishing the result. Note if everything were done in the measure
             * fuction 1x set of LTX instructions woulf result in a LTX fuction for every combination. */
            var measurement = Instrument_MM.MeasureDCPowerCurrent(vi1Session, 0.01);
            
            // If the measurment were manipulated the AI would add that code here
            
            vi1Session.PinQueryContext.Publish(measurement, "I_VIN");

            /*The LTX tester makes SMU measurements by connecting the VI to MM; however, on STS the SMU will make the measurement
             * directly and thus no switching is required. For STS, this function would become a range change for the SMU and the AI tools
             * would need to keep track of where MM is connected so the measurement occurs on the correct SMU. From an AI Perspective, not
             * sure if we want to keep the near 1:1 LTX to STS naming or if it makes since to change the name to match what is happening. */
            Instrument_MM.ChangeDCPowerCurrentMax(vi2Session, 0.00012);

            System.Threading.Thread.Sleep(5);

            /* Given math and other things can be performed on meaurement results, we are storing the result in varaible so the AI
             * can manipulate the result if needed and then we are publishing the result. Note if everything were done in the measure
             * fuction 1x set of LTX instructions woulf result in a LTX fuction for every combination. */
            measurement = Instrument_MM.MeasureDCPowerCurrent(vi2Session, 0.01);
            // If the measurment were manipulated the AI would add that code here
            vi2Session.PinQueryContext.Publish(measurement, "I_SHDN");
        }
        #endregion
    }
}