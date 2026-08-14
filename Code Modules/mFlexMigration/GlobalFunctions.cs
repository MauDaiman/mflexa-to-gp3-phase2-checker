using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.TestStand.Interop.API;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public static class GlobalFunctions
    {
        //public static SiteDouble[] PropDelayPatternBurst(string[] TestPinsArray, double StartEdge, double StepSize, Pattern ThePat, SiteDouble[] Edges, int nSite)
        //{
        //    double resolution = 250e-12;
        //    StepSize = 500e-12;
        //    StartEdge = 0;//From Russel's code

        //    var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, TestPinsArray);
        //    for (int i = 0; i < 2; i++)
        //    {
        //        //1. Set compare strobe(edge) for all tsets(strb0-strb19)
        //        for (int j = 0; j < Globals.strbNames.Length; j++)
        //        {
        //            //Implement walking strobe
        //            Globals.TheHdw.Digital.Timing.ConfigureCompareEdgesStrobe(sessions, TestPinsArray[i], Globals.strbNames[j], StartEdge);
        //            StartEdge = +StepSize;
        //        }
        //        StartEdge = 0;

        //        //2. Burst pattern, 1st burst captures every 500ps step
        //        Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run();

        //        //3. Count fail
        //        Edges[i].Value[nSite] = resolution * Globals.TheHdw.Pins.Pins(TestPinsArray[i]).FailCount.Value[nSite];

        //        //Add 250ps on each compare strobe location, then repeat step 1 - 3
        //        StartEdge = +resolution;
        //    }

        //debug -adrian
        //public static SiteDouble[] PropDelayPatternBurst(string[] TestPinsArray, double StartEdge, double StepSize, Pattern ThePat, SiteDouble[] Edges, int nSite)
        public static SiteDouble[] PropDelayPatternBurst(string[] TestPinsArray, double StartEdge, double StepSize, Pattern ThePat, SiteDouble[] Edges)
        {
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStart();
#endif
            StepSize = 500e-12;
            StartEdge = 250e-12;
            double Resolution = 250e-12;
            int i = 0;

            var sessions = InstrCtrl.DigitalPinsToSessions(Globals.tsmContext, TestPinsArray);
            //Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run();

            SiteDouble[] PinEdges = new SiteDouble[TestPinsArray.Length]; //temporary var to hold delay per pin -adrian

            //---------------------------------------------------
            Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run();

            //Added to improve resolution from 500ps to 250ps
            //Bursting pattern twice with 250ps starting edge difference with the same 500ps step size
            var failcount_0Edge = new double[TestPinsArray.Length][];
            for (i = 0; i < TestPinsArray.Length; i++)
            {
                failcount_0Edge[i] = new double[Globals.tsmContext.SiteNumbers.Count];
            }
            i = 0;
            foreach (dynamic nSite in Globals.TheExec.Sites.Active)
            {
                GlobalFunctions.BeginSiteLoop(nSite);
                Parallel.For(0, TestPinsArray.Length, k =>  //added -adrian
                {
                    failcount_0Edge[k][i] = Globals.TheHdw.Pins.Pins(TestPinsArray[k]).FailCount.Value[nSite];
                });
                i++;
            }

            //adjust compare strobes by 250ps -adrian
            for (i = 0; i < TestPinsArray.Length; i++)
            {
                StartEdge = 250e-12;
                for (int j = 0; j < Globals.strbNames.Length; j++)
                {
                    //Implement walking strobe
                    Globals.TheHdw.Digital.Timing.ConfigureCompareEdgesStrobe(sessions, TestPinsArray[i], Globals.strbNames[j], StartEdge);
                    StartEdge = StartEdge + StepSize;
                }
            }

            //---------------------------------------------------

            Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run();

            i = 0;
            foreach (dynamic nSite in Globals.TheExec.Sites.Active)
            {
                GlobalFunctions.BeginSiteLoop(nSite);
                Parallel.For(0, TestPinsArray.Length, k =>  //added -adrian
               {
                   //StartEdge = 0;  //Back to 0 for next pin -adrian
                   //for (int i = 0; i < 2; i++)
                   //{
                   //Compare strobe was already set in digitiming
                   //1. Set compare strobe(edge) for all tsets(strb0-strb19)
                   //for (int j = 0; j < Globals.strbNames.Length; j++)
                   //{
                   //    //Implement walking strobe
                   //    Globals.TheHdw.Digital.Timing.ConfigureCompareEdgesStrobe(sessions, TestPinsArray[i], Globals.strbNames[j], StartEdge);
                   //    StartEdge = +StepSize;
                   //}
                   //StartEdge = 0;

                   //2. Burst pattern, 1st burst captures every 500ps step
                   //Globals.TheHdw.Digital.Patterns.Pat(ThePat).Run();    //run pattern once -adrian

                   //3. Count fail
                   //Edges[i].Value[nSite] = resolution * Globals.TheHdw.Pins.Pins(TestPinsArray[i]).FailCount.Value[nSite];
                   //Edges[k].Value[nSite] = (StepSize * Globals.TheHdw.Pins.Pins(TestPinsArray[k]).FailCount.Value[nSite]) - (Globals.TDRperPin[k]+ Globals.TDRperPin[k+2]);   //use stepsize as resolution; subtract delay obtained from TDR (Resource > Pin > Resource; In/Out) -adrian
                   //Edges[k].Value[i] = (StepSize * Globals.TheHdw.Pins.Pins(TestPinsArray[k]).FailCount.Value[nSite]) - (Globals.TDRperPin[k] + Globals.TDRperPin[k + 2]);   //use stepsize as resolution; subtract delay obtained from TDR (Resource > Pin > Resource; In/Out) -adrian
                   //Edges[k].Value[i] = (StepSize * Globals.TheHdw.Pins.Pins(TestPinsArray[k]).FailCount.Value[nSite]) - StepSize;   //use stepsize as resolution; subtracted Stepsize to cancel-out 1 fail from T0 compare -adrian
                   Edges[k].Value[i] = (Resolution * (Globals.TheHdw.Pins.Pins(TestPinsArray[k]).FailCount.Value[nSite] + failcount_0Edge[k][i] - 1));

                    //Add 250ps on each compare strobe location, then repeat step 1 - 3
                    //St artEdge = +resolution;
                    //}
                    //Add(burst1 + burst2) fail counts then multiplied by resolution(250ps), this will be the measured propagation delay.
                    //PinEdges[k]= Edges[0].Add(Edges[1]); //added -adrian
                   PinEdges = Edges;
               });
                i++;
                GlobalFunctions.EndSiteLoop(nSite);
            }
            //Add(burst1 + burst2) fail counts then multiply by resolution(250ps), this will be the measured propagation delay.
            //SiteDouble pin_A_B_sum = Edges[0].Add(Edges[1]);
            Edges = PinEdges;
#if TestTimeMeasure
            Globals.TestTimeMeasure.TestTimeStop();
#endif
            return Edges;

            //Notes:
            //On the index array, increase the n-dimension with respect to the number of loop burst.
            //Don’t forget to add the delay on search strobe on the final datalog.This is based on the pattern start delay on 1st cycle.
        }

        // UBound function to get the upper bound of an array
        public static int UBound(Array array)
        {

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            return array.Length - 1;
        }

        public static double[][] convertPinListDatato2DArray(dynamic oneDarray)
        {
            List<double> pinDataList = oneDarray.ToList();
            double[] result = pinDataList.ToArray();
            int cols = Globals.tsmContext.SiteNumbers.Count();
            int rows = result.Length / cols;
            double[][] pinData = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                pinData[i] = new double[cols];
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    pinData[i][j] = result[i * cols + j];
                }
            }
            return pinData;
        }

        public static List<bool> ConvertToBooleanList(List<int> numbers)
        {
            if (numbers == null || numbers.Count == 0)
            {
                return new List<bool>();
            }

            int maxNumber = numbers.Max();
            List<bool> booleans = Enumerable.Repeat(false, maxNumber + 1).ToList();

            foreach (int number in numbers)
            {
                booleans[number] = true;
            }

            return booleans;
        }

        public static List<int> ConvertToIntegerList(List<bool> booleans)
        {
            List<int> numbers = new List<int>();

            for (int i = 0; i < booleans.Count; i++)
            {
                if (booleans[i])
                {
                    numbers.Add(i);
                }
            }

            return numbers;
        }

        public static bool Is2DArray(object obj)
        {
            if (obj == null)
                return false;

            Type type = obj.GetType();
            return type.IsArray && type.GetArrayRank() == 2;
        }
        public static bool BeginSiteLoop(int nSite)
        {
            //Add implementatio here
            if (nSite == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EndSiteLoop(int nSite)
        {
            //Add implementatio here
            if (nSite == Globals.tsmContext.SiteNumbers.Count())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
