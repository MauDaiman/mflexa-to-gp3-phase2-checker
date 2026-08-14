using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace TestStand.SemiconductorModule.Migration.LTX
{

   
    public static class READ
    {

        //Creates and initializes a new list called DATA
        public static List<int> DATA = new List<int>();

        //Hard code creation of equivalent variables of LTX
        public static int A { get; set; }
        public static int B { get; set; }
        public static int C { get; set; }
        public static int D { get; set; }
        public static int E { get; set; }
        public static int F { get; set; }
        public static int X { get; set; }
        public static int Y { get; set; }
        public static int Z { get; set; }

        public static int incread { get; set; }  = 0;
        public static int incdata { get; set; } = 0;


        //Read method that accepts single variable input and returns single integer
        public static int Read(char str)
        {
            //Keep track of the index of the array, read depending on the current value of incread
            int x = 0;
            switch (str)
            {
                case 'A':
                    x = A = DATA[incread];
                    break;
                case 'B':
                    x = B = DATA[incread];
                    break;
                case 'C':
                    x = C = DATA[incread];
                    break;
                case 'D':
                    x = D = DATA[incread];
                    break;
                case 'E':
                    x = E = DATA[incread];
                    break;
                case 'F':
                    x = F = DATA[incread];
                    break;
            }

            incread++;
            return x;
        }

        //Read method that will accept an array of string input variables and return an array of integers
        public static int[] Read(params string[] str )
        {
            int [] values = new int[str.Length];
            int i = 0;

            foreach (string s in str)
            {
                switch (s)
                {
                    case "A":
                        values[i] = A = DATA[incread];
                        break;
                    case "B":
                        values[i] = B = DATA[incread];
                        break;
                    case "C":
                        values[i] = C = DATA[incread];
                        break;
                    case "D":
                        values[i] = D = DATA[incread];
                        break;
                    case "E":
                        values[i] = E = DATA[incread];
                        break;
                    case "F":
                        values[i] = F = DATA[incread];
                        break;
                }
                i++;
                incread++;
            }



            return values;
        }

        //accepts comma separated values of variables to read
        public static int[] Read(string str1)
        {
            String[] strings = str1.Split(','); //Split csv into array of strings
            for (int i1 = 0; i1 < strings.Length; i1++)  ////Remove spaces from the string
            {
                strings[i1] = strings[i1].Replace(" ", "");
            }

            int[] values = new int[str1.Length]; //Create a new array of integers to store the values
            int i = 0;  //Keep track of the index of the array
            incread++;

            foreach (string s in strings)
            {
                switch (s)
                {
                    case "A":
                        values[i] = A = DATA[incread];
                        break;
                    case "B":
                        values[i] = B = DATA[incread];
                        break;
                    case "C":
                        values[i] = C = DATA[incread];
                        break;
                    case "D":
                        values[i] = D = DATA[incread];
                        break;
                    case "E":
                        values[i] = E = DATA[incread];
                        break;
                    case "F":
                        values[i] = F = DATA[incread];
                        break;
                }
                i++; 
                incread++;
            }
            return values;
        }

        //method that accepts an integer array and creates List equivalent called "DATA"
        public static void SetData(params int[] data_input)

        {
            //Copies the data_input from caller to DATA list
            DATA.AddRange(data_input);
            
        }

    }


    
}
