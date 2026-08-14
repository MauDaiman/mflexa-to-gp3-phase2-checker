using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Numerics;
using System.IO;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.InstrumentControl;

namespace NationalInstruments.TestStand.SemiconductorModule.Migration.mFlex
{
    public abstract class SiteVariant<T>
    {
        protected T[,] data;
        protected int numberOfSites;
        protected int numberOfPins;

        public SiteVariant(int numberOfPins, int numberOfSites)
        {
            this.numberOfPins = numberOfPins;
            this.numberOfSites = numberOfSites;
            data = new T[numberOfPins, numberOfSites];
        }

        public SiteVariant(int numberOfPins)
        {
            this.numberOfPins = numberOfPins;
            this.numberOfSites = Globals.tsmContext.SiteNumbers.Count();
            data = new T[numberOfPins, numberOfSites];
        }

        public SiteVariant()
        {
            this.numberOfPins = 1;
            this.numberOfSites = Globals.tsmContext.SiteNumbers.Count();
            data = new T[numberOfPins, numberOfSites];
        }

        public T this[int pinIndex, int siteIndex]
        {
            get => data[pinIndex, siteIndex];
            set => data[pinIndex, siteIndex] = value;
        }

        public T this[int siteIndex]
        {
            get => data[0, siteIndex]; 
            set
            {
                for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
                {
                    data[pinIndex, siteIndex] = value;
                }
            }
        }

        public SiteValueProxy Value => new SiteValueProxy(this);

        public class SiteValueProxy
        {
            private readonly SiteVariant<T> _siteVariant;

            public SiteValueProxy(SiteVariant<T> siteVariant)
            {
                _siteVariant = siteVariant;
            }

            public T this[int siteIndex]
            {
                get => _siteVariant[siteIndex];
                set => _siteVariant[siteIndex] = value;
            }
        }

        public void SetAll(T value)
        {
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    data[pinIndex, siteIndex] = value;
                }
            }
        }

        public int Length => numberOfSites;

        public SiteVariant<T> Operate(SiteVariant<T> other, Func<T, T, T> operation)
        {
            var result = (SiteVariant<T>)Activator.CreateInstance(this.GetType(), numberOfSites, numberOfPins);
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    result[pinIndex, siteIndex] = operation(this[pinIndex, siteIndex], other[pinIndex, siteIndex]);
                }
            }
            return result;
        }


        public List<T> ToList()
        {
            var list = new List<T>();
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    list.Add(data[pinIndex, siteIndex]);
                }
            }
            return list;
        }
    }

    public class SiteDouble : SiteVariant<double>
    {
        public List<string> PinNames { get; private set; }

        //public SiteDouble(int numberOfSites, int numberOfPins) : base(numberOfPins+1, numberOfSites)
        public SiteDouble(int numberOfSites, int numberOfPins) : base(numberOfPins, numberOfSites)  //Changed due to inaccurate pin number -adrian
        {
            PinNames = new List<string>();
            //InitializeArray(numberOfPins+1, numberOfSites);
        }

        public SiteDouble(int numberOfPins) : base(numberOfPins+1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
            //InitializeArray(numberOfPins+1, Globals.tsmContext.SiteNumbers.Count());
        }

        public SiteDouble() : base(1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
            //InitializeArray(1, Globals.tsmContext.SiteNumbers.Count());
        }

        //public SiteDouble(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins+1, numberOfSites)
        public SiteDouble(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins, numberOfSites)   //Changed due to inaccurate pin number -adrian
        {
            PinNames = pinNames;
            //InitializeArray(numberOfPins+1, numberOfSites);
        }
        /*
        private void InitializeArray(int numberOfPins, int numberOfSites)
        {
            data = new double[numberOfPins, numberOfSites];
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    data[pinIndex, siteIndex] = 0.0; // Initialize each element with a default value
                }
            }
        }
        */
        public static SiteDouble[] New(int size)
        {
            var array = new SiteDouble[size+1];
            //for (int i = 0; i < array.Length; i++)
            //{
            //    array[i] = new SiteDouble();
            //}
            Parallel.For(0, array.Length, i =>
            {
                array[i] = new SiteDouble();
            });
            return array;
        }

        public static implicit operator SiteDouble(double value)
        {
            var siteDouble = new SiteDouble();
            siteDouble.SetAll(value);
            return siteDouble;
        }

        public static implicit operator SiteDouble((double[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteDouble = new SiteDouble(numSite, values.Length / numSite, pinNames);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteDouble[i, j] = values[m];
                    m++;
                }
            }
            return siteDouble;
        }

        public new SiteValueProxy Value => new SiteValueProxy(this);

        public new class SiteValueProxy : SiteVariant<double>.SiteValueProxy
        {
            private readonly SiteDouble _siteDouble;

            public SiteValueProxy(SiteDouble siteDouble) : base(siteDouble)
            {
                _siteDouble = siteDouble;
            }

            public new double this[int siteIndex]
            {
                get => _siteDouble[siteIndex];
                set => _siteDouble[siteIndex] = value;
            }
        }

        public SiteDouble Add(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a + b);
        }

        public SiteDouble Subtract(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a - b);
        }

        public SiteDouble Divide(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a / b);
        }

        public SiteDouble Invert()
        {
            return (SiteDouble)Operate(this, (a, b) => 1 / a);
        }

        public SiteDouble Log10()
        {
            return (SiteDouble)Operate(this, (a, b) => Math.Log10(a));
        }

        public SiteDouble Maximum(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a > b ? a : b);
        }

        public SiteDouble Minimum(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a < b ? a : b);
        }

        public SiteDouble Multiply(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => a * b);
        }

        public SiteDouble Negate()
        {
            return (SiteDouble)Operate(this, (a, b) => -a);
        }

        public SiteDouble Power(SiteDouble other)
        {
            return (SiteDouble)Operate(other, (a, b) => Math.Pow(a, b));
        }

        public SiteDouble SquareRoot()
        {
            return (SiteDouble)Operate(this, (a, b) => Math.Sqrt(a));
        }

        public SiteDouble Truncate()
        {
            return (SiteDouble)Operate(this, (a, b) => Math.Truncate(a));
        }
    }
    public class SiteComplexDouble : SiteVariant<Complex>
    {
        public List<string> PinNames { get; private set; }

        public SiteComplexDouble(int numberOfPins) : base(numberOfPins+1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public SiteComplexDouble(int numberOfSites, int numberOfPins) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = new List<string>();
        }

        public SiteComplexDouble(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = pinNames;
        }

        public SiteComplexDouble() : base(1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public static SiteComplexDouble[] New(int size)
        {
            var array = new SiteComplexDouble[size+1];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteComplexDouble();
            }
            return array;
        }

        public SiteDouble Real => ConvertToDoubleVariant(c => c.Real);

        public SiteDouble Imag => ConvertToDoubleVariant(c => c.Imaginary);

        public SiteDouble Magnitude => ConvertToDoubleVariant(c => c.Magnitude);

        public SiteDouble Phase => ConvertToDoubleVariant(c => c.Phase);

        private SiteDouble ConvertToDoubleVariant(Func<Complex, double> selector)
        {
            var result = new SiteDouble(numberOfSites, numberOfPins);
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    result[pinIndex, siteIndex] = selector(data[pinIndex, siteIndex]);
                }
            }
            return result;
        }

        public SiteComplexDouble Add(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a + b);
        }

        public SiteComplexDouble Subtract(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a - b);
        }

        public SiteComplexDouble Divide(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a / b);
        }

        public SiteComplexDouble Invert()
        {
            return (SiteComplexDouble)Operate(this, (a, b) => 1 / a);
        }

        public SiteComplexDouble Log10()
        {
            return (SiteComplexDouble)Operate(this, (a, b) => Complex.Log10(a));
        }

        public SiteComplexDouble Maximum(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a.Magnitude > b.Magnitude ? a : b);
        }

        public SiteComplexDouble Minimum(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a.Magnitude < b.Magnitude ? a : b);
        }

        public SiteComplexDouble Multiply(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => a * b);
        }

        public SiteComplexDouble Negate()
        {
            return (SiteComplexDouble)Operate(this, (a, b) => -a);
        }

        public SiteComplexDouble Power(SiteComplexDouble other)
        {
            return (SiteComplexDouble)Operate(other, (a, b) => Complex.Pow(a, b));
        }

        public SiteComplexDouble SquareRoot()
        {
            return (SiteComplexDouble)Operate(this, (a, b) => Complex.Sqrt(a));
        }

        public SiteComplexDouble Truncate()
        {
            return (SiteComplexDouble)Operate(this, (a, b) => new Complex(Math.Truncate(a.Real), Math.Truncate(a.Imaginary)));
        }

    }
    public class SiteLong : SiteVariant<long>
    {
        public List<string> PinNames { get; private set; }

        public SiteLong(int numberOfSites, int numberOfPins) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = new List<string>();
        }

        public SiteLong(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = pinNames;
        }

        public SiteLong(int numberOfPins) : base(numberOfPins+1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public SiteLong() : base(1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public static SiteLong[] New(int size)
        {
            var array = new SiteLong[size+1];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteLong();
            }
            return array;
        }

        public static implicit operator SiteLong(long value)
        {
            var siteLong = new SiteLong();
            siteLong.SetAll(value);
            return siteLong;
        }

        public static implicit operator SiteLong((long[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteLong = new SiteLong(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteLong[i, j] = values[m];
                    m++;
                }
            }
            return siteLong;
        }

        public static implicit operator SiteLong(int[] values)
        {
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteLong = new SiteLong(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteLong[i, j] = values[m];
                    m++;
                }
            }
            return siteLong;
        }

        public SiteLong BitwiseAnd(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a & b);
        }

        public SiteLong BitwiseOr(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a | b);
        }

        public SiteLong BitwiseXor(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a ^ b);
        }

        public SiteLong Complement()
        {
            return (SiteLong)Operate(this, (a, b) => ~a);
        }

        public SiteLong ShiftLeft(int count)
        {
            return (SiteLong)Operate(this, (a, b) => a << count);
        }

        public SiteLong ShiftRight(int count)
        {
            return (SiteLong)Operate(this, (a, b) => a >> count);
        }

        public SiteLong Add(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a + b);
        }

        public SiteLong Subtract(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a - b);
        }

        public SiteLong Divide(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a / b);
        }

        public SiteLong Invert()
        {
            return (SiteLong)Operate(this, (a, b) => 1 / a);
        }

        public SiteLong Log10()
        {
            return (SiteLong)Operate(this, (a, b) => (long)Math.Log10(a));
        }

        public SiteLong Maximum(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a > b ? a : b);
        }

        public SiteLong Minimum(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a < b ? a : b);
        }

        public SiteLong Multiply(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a * b);
        }

        public SiteLong Negate()
        {
            return (SiteLong)Operate(this, (a, b) => -a);
        }

        public SiteLong Power(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => (long)Math.Pow(a, b));
        }

        public SiteLong SquareRoot()
        {
            return (SiteLong)Operate(this, (a, b) => (long)Math.Sqrt(a));
        }

        public SiteLong Truncate()
        {
            return (SiteLong)Operate(this, (a, b) => a);
        }

    }
    public class SiteBoolean : SiteVariant<bool>
    {
        public List<string> PinNames { get; private set; }

        public SiteBoolean(int numberOfSites, int numberOfPins) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = new List<string>();
        }

        public SiteBoolean(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins+1, numberOfSites)
        {
            PinNames = pinNames;
        }

        public SiteBoolean(int numberOfPins) : base(numberOfPins+1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public SiteBoolean() : base(1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public static SiteBoolean[] New(int size)
        {
            var array = new SiteBoolean[size+1];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteBoolean();
            }
            return array;
        }

        public static implicit operator SiteBoolean(bool value)
        {
            var siteBoolean = new SiteBoolean();
            siteBoolean.SetAll(value);
            return siteBoolean;
        }

        public static implicit operator SiteBoolean((bool[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteBoolean = new SiteBoolean(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteBoolean[i, j] = values[m];
                    m++;
                }
            }
            return siteBoolean;
        }

        public bool All(bool condition)
        {
            for (int i = 0; i < numberOfSites; i++)
            {
                if (data[0, i] != condition)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Any(bool condition)
        {
            for (int i = 0; i < numberOfSites; i++)
            {
                if (data[0, i] == condition)
                {
                    return true;
                }
            }
            return false;
        }

        public SiteBoolean LogicalAnd(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a && b);
        }

        public SiteBoolean LogicalNot()
        {
            return (SiteBoolean)Operate(this, (a, b) => !a);
        }

        public SiteBoolean LogicalOr(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a || b);
        }

        public SiteBoolean LogicalXor(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a ^ b);
        }

    }
    /*
    public abstract class SiteVariant<T>
    {
        protected T[,] data;
        protected int numberOfSites;// = Globals.tsmContext.SiteNumbers.Count();//update this using TSM API
        protected int numberOfPins;

        public SiteVariant(int numberOfPins, int numberOfSites)
        {
            this.numberOfPins = numberOfPins; 
            this.numberOfSites = numberOfSites;
            data = new T[numberOfPins, numberOfSites];
        }

        public SiteVariant(int numberOfPins)
        {
            this.numberOfPins = numberOfPins; // Add 1 to numberOfPins
            this.numberOfSites = Globals.tsmContext.SiteNumbers.Count(); // Initialize number of sites
            data = new T[numberOfPins, numberOfSites];
        }

        public SiteVariant()
        {
            this.numberOfPins = 1; // Default to 1 pin
            this.numberOfSites = Globals.tsmContext.SiteNumbers.Count(); // Initialize number of sites
            data = new T[numberOfPins, numberOfSites];
        }

        public T this[int pinIndex, int siteIndex]
        {
            get => data[pinIndex, siteIndex];
            set => data[pinIndex, siteIndex] = value;
        }

        public T this[int siteIndex]
        {
            get => data[0, siteIndex];
            set
            {
                for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
                {
                    data[pinIndex, siteIndex] = value;
                }
            }
        }

        public SiteValueProxy Value => new SiteValueProxy(this);

        public class SiteValueProxy
        {
            private readonly SiteVariant<T> _siteVariant;

            public SiteValueProxy(SiteVariant<T> siteVariant)
            {
                _siteVariant = siteVariant;
            }

            public T this[int siteIndex]
            {
                get => _siteVariant[siteIndex];
                set => _siteVariant[siteIndex] = value;
            }
        }

        public void SetAll(T value)
        {
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    data[pinIndex, siteIndex] = value;
                }
            }
        }

        // Length property to get the number of sites
        public int Length => numberOfSites;

        public SiteVariant<T> Operate(SiteVariant<T> other, Func<T, T, T> operation)
        {
            var result = (SiteVariant<T>)Activator.CreateInstance(this.GetType(), numberOfSites, numberOfPins);
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    result[pinIndex, siteIndex] = operation(this[pinIndex, siteIndex], other[pinIndex, siteIndex]);
                }
            }
            return result;
        }

        public SiteVariant<T> Abs()
        {
            return Operate(this, (a, b) => Math.Abs((dynamic)a));
        }

        public SiteVariant<T> Add(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a + (dynamic)b);
        }

        public SiteBoolean Compare(SiteVariant<T> other, Func<T, T, bool> comparison)
        {
            var result = new SiteBoolean(numberOfSites, numberOfPins);
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    result[pinIndex, siteIndex] = comparison(this[pinIndex, siteIndex], other[pinIndex, siteIndex]);
                }
            }
            return result;
        }

        public SiteVariant<T> Divide(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a / (dynamic)b);
        }

        public SiteVariant<T> Invert()
        {
            return Operate(this, (a, b) => 1 / (dynamic)a);
        }

        public SiteVariant<T> Log10()
        {
            return Operate(this, (a, b) => Math.Log10((dynamic)a));
        }

        public SiteVariant<T> Maximum(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a > (dynamic)b ? a : b);
        }

        public SiteVariant<T> Minimum(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a < (dynamic)b ? a : b);
        }

        public SiteVariant<T> Multiply(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a * (dynamic)b);
        }

        public SiteVariant<T> Negate()
        {
            return Operate(this, (a, b) => -(dynamic)a);
        }

        public SiteVariant<T> Power(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => Math.Pow((dynamic)a, (dynamic)b));
        }

        public SiteVariant<T> SquareRoot()
        {
            return Operate(this, (a, b) => Math.Sqrt((dynamic)a));
        }

        public SiteVariant<T> Subtract(SiteVariant<T> other)
        {
            return Operate(other, (a, b) => (dynamic)a - (dynamic)b);
        }

        public SiteVariant<T> Truncate()
        {
            return Operate(this, (a, b) => Math.Truncate((dynamic)a));
        }
        // ToList method to list all data
        public List<T> ToList()
        {
            var list = new List<T>();
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    list.Add(data[pinIndex, siteIndex]);
                }
            }
            return list;
        }
    }

    public class SiteBoolean : SiteVariant<bool>
    {
        public List<string> PinNames { get; private set; }
        public SiteBoolean(int numberOfSites, int numberOfPins) : base(numberOfPins, numberOfSites) { PinNames = new List<string>(); }
        public SiteBoolean(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins, numberOfSites) { PinNames = pinNames; }
        public SiteBoolean(int numberOfPins) : base(numberOfPins, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }
        public SiteBoolean() : base(1, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }

        public static implicit operator SiteBoolean(bool value)
        {
            var siteBoolean = new SiteBoolean();
            siteBoolean.SetAll(value);
            return siteBoolean;
        }

        public static implicit operator SiteBoolean((bool[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteBoolean = new SiteBoolean(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteBoolean[i, j] = values[m];
                    m++;
                }
            }
            return siteBoolean;
        }

        public bool All(bool condition)
        {
            for (int i = 0; i < numberOfSites; i++)
            {
                if (data[0, i] != condition)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Any(bool condition)
        {
            for (int i = 0; i < numberOfSites; i++)
            {
                if (data[0, i] == condition)
                {
                    return true;
                }
            }
            return false;
        }

        public SiteBoolean LogicalAnd(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a && b);
        }

        public SiteBoolean LogicalNot()
        {
            return (SiteBoolean)Operate(this, (a, b) => !a);
        }

        public SiteBoolean LogicalOr(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a || b);
        }

        public SiteBoolean LogicalXor(SiteBoolean other)
        {
            return (SiteBoolean)Operate(other, (a, b) => a ^ b);
        }
    }

    public class SiteComplexDouble : SiteVariant<Complex>
    {
        public List<string> PinNames { get; private set; }
        public SiteComplexDouble(int numberOfPins) : base(numberOfPins, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }
        public SiteComplexDouble(int numberOfSites, int numberOfPins) : base(numberOfPins, numberOfSites) { PinNames = new List<string>(); }
        public SiteComplexDouble(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins, numberOfSites) { PinNames = pinNames; }
        public SiteComplexDouble() : base(1, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }

        public SiteDoubleI Imag => ConvertToDoubleVariant(c => c.Imaginary);

        public SiteDoubleI Magnitude => ConvertToDoubleVariant(c => c.Magnitude);

        public SiteDoubleI Phase => ConvertToDoubleVariant(c => c.Phase);

        public SiteDoubleI Real => ConvertToDoubleVariant(c => c.Real);

        private SiteDoubleI ConvertToDoubleVariant(Func<Complex, double> selector)
        {
            var result = new SiteDoubleI(numberOfSites, numberOfPins);
            for (int pinIndex = 0; pinIndex < numberOfPins; pinIndex++)
            {
                for (int siteIndex = 0; siteIndex < numberOfSites; siteIndex++)
                {
                    result[pinIndex, siteIndex] = selector(data[pinIndex, siteIndex]);
                }
            }
            return result;
        }
    }

    public class SiteDouble
    {
        private SiteDoubleI[] array;

        public SiteDouble(int size)
        {
            array = new SiteDoubleI[size + 1]; // Always add 1 to the size
            InitializeArray(size + 1);
        }
        public SiteDouble()
        {
            int siteNum = Globals.tsmContext.SiteNumbers.Count();
            array = new SiteDoubleI[siteNum]; // Always follow number of site size for single pin
            InitializeArray(siteNum);
        }

        private void InitializeArray(int size)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteDoubleI(); // Initialize each element with a new SiteDoubleInstance
            }
        }

        public static SiteDouble[] New(int size)
        {
            var array = new SiteDouble[size + 1];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteDouble();
            }
            return array;
        }

        public static SiteDouble[] ReDim(int size)
        {
            var array = new SiteDouble[size+1];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new SiteDouble();
            }
            return array;
        }


        public SiteDoubleI this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public int Length => array.Length;

        public SiteDoubleI.SiteValueProxy Value => new SiteDoubleI.SiteValueProxy(array[0]);

        public static implicit operator SiteDouble(double value)
        {
            var siteDouble = new SiteDouble(1);
            siteDouble[0] = new SiteDoubleI();
            siteDouble[0].SetAll(value);
            return siteDouble;
        }

        public static implicit operator SiteDouble((double[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteDouble = new SiteDouble(numSite);
            for (int i = 0; i < numSite; i++)
            {
                siteDouble[i] = new SiteDoubleI(numSite, values.Length / numSite, pinNames);
                for (int j = 0; j < values.Length / numSite; j++)
                {
                    siteDouble[i][j] = values[i * (values.Length / numSite) + j];
                }
            }
            return siteDouble;
        }

        public SiteDouble Subtract(SiteDouble other)
        {
            var result = new SiteDouble(this.Length - 1);
            for (int i = 0; i < this.Length; i++)
            {
                result[i] = (SiteDoubleI)this[i].Subtract(other[i]);
            }
            return result;
        }

        public List<double> ToList()
        {
            var list = new List<double>();
            for (int i = 0; i < array.Length; i++)
            {
                list.AddRange(array[i].ToList());
            }
            return list;
        }
    }


    public class SiteDoubleI : SiteVariant<double>
    {
        public List<string> PinNames { get; private set; }

        public SiteDoubleI(int numberOfSites, int numberOfPins) : base(numberOfPins, numberOfSites)
        {
            PinNames = new List<string>();
        }

        public SiteDoubleI(int numberOfPins) : base(numberOfPins, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public SiteDoubleI() : base(1, Globals.tsmContext.SiteNumbers.Count())
        {
            PinNames = new List<string>();
        }

        public SiteDoubleI(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins, numberOfSites)
        {
            PinNames = pinNames;
        }

        public static implicit operator SiteDoubleI(double value)
        {
            var siteDouble = new SiteDoubleI();
            siteDouble.SetAll(value);
            return siteDouble;
        }

        public static implicit operator SiteDoubleI((double[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteDouble = new SiteDoubleI(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteDouble[i, j] = values[m];
                    m++;
                }
            }
            return siteDouble;
        }

        public new SiteValueProxy Value => new SiteValueProxy(this);

        public new class SiteValueProxy : SiteVariant<double>.SiteValueProxy
        {
            private readonly SiteDoubleI _siteDoubleI;

            public SiteValueProxy(SiteDoubleI siteDoubleI) : base(siteDoubleI)
            {
                _siteDoubleI = siteDoubleI;
            }

            public new double this[int siteIndex]
            {
                get => _siteDoubleI[siteIndex];
                set => _siteDoubleI[siteIndex] = value;
            }
        }
    }

    public class SiteLong : SiteVariant<long>
    {
        public List<string> PinNames { get; private set; }
        public SiteLong(int numberOfSites, int numberOfPins) : base(numberOfPins, numberOfSites) { PinNames = new List<string>(); }
        public SiteLong(int numberOfSites, int numberOfPins, List<string> pinNames) : base(numberOfPins, numberOfSites) { PinNames = pinNames; }
        public SiteLong(int numberOfPins) : base(numberOfPins, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }
        public SiteLong() : base(1, Globals.tsmContext.SiteNumbers.Count()) { PinNames = new List<string>(); }

        public static implicit operator SiteLong(long value)
        {
            var siteLong = new SiteLong();
            siteLong.SetAll(value);
            return siteLong;
        }

        public static implicit operator SiteLong((long[] values, List<string> pinNames) input)
        {
            var (values, pinNames) = input;
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            var siteLong = new SiteLong(numSite, values.Length / numSite);
            int m = 0;
            for (int i = 0; i < values.Length / numSite; i++)
            {
                for (int j = 0; j < numSite; j++)
                {
                    siteLong[i, j] = values[m];
                    m++;
                }
            }
            return siteLong;
        }

        public SiteLong BitwiseAnd(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a & b);
        }

        public SiteLong BitwiseOr(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a | b);
        }

        public SiteLong BitwiseXor(SiteLong other)
        {
            return (SiteLong)Operate(other, (a, b) => a ^ b);
        }

        public SiteLong Complement()
        {
            return (SiteLong)Operate(this, (a, b) => ~a);
        }

        public SiteLong ShiftLeft(int count)
        {
            return (SiteLong)Operate(this, (a, b) => a << count);
        }

        public SiteLong ShiftRight(int count)
        {
            return (SiteLong)Operate(this, (a, b) => a >> count);
        }
    }

    /*public class PinListData : SiteVariant<PinData<double>>
    {
        public List<string> PinNames { get; private set; }
        public PinListData(int numberOfPins) : base(numberOfPins) { PinNames = new List<string>(); }

        // Dictionary to store pin data for each site
        private Dictionary<string, Dictionary<int, double>> pinData;

        public PinListData(PinList pinList)
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            PinNames = pinList.GetAll();
        }

        // Constructor to initialize the pin data and pin names
        public PinListData() : base()
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            PinNames = new List<string>();
        }

        // Constructor to initialize from a double value
        public PinListData(double value)
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            PinNames = new List<string>();

            // Assuming a single pin name for simplicity
            string pinName = "Pin1";
            PinNames.Add(pinName);

            // Assuming a single site for simplicity
            Add(pinName, 0, value);
        }

        // Constructor to initialize from a double array
        public PinListData(double[] values, List<string> pinNames)
        {
            this.PinNames = pinNames;

            // Assign values to different sites
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            for (int site = 0; site < numSite; site++)
            {
                Add(pinNames[site], site, values[site]);
            }
        }

        public static implicit operator PinListData((double[] values, List<string> pinNames) data)
        {
            return new PinListData(data.values, data.pinNames);
        }

        // Adds data for a specific pin and site
        public void Add(string pin, int site, double value)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                PinNames.Add(pin);
            }
            pinData[pin][site] = value;
        }

        // Declares or defines a single pin with default values for each site
        public void DeclarePin(string pin, List<int> sites, double defaultValue)
        {
            foreach (var site in sites)
            {
                Add(pin, site, defaultValue);
            }
        }

        // Adds a group of pins with default values for each site
        public void AddGroup(List<string> pins, List<int> sites, double defaultValue)
        {
            foreach (var pin in pins)
            {
                DeclarePin(pin, sites, defaultValue);
            }
        }

        // Adds a pin without specifying sites and data
        public void AddPin(string pin)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                PinNames.Add(pin);
            }
        }

        // Extracts data for a single pin across all sites
        public List<double> PinData(string pinName)
        {
            if (pinData.ContainsKey(pinName))
            {
                return pinData[pinName].Values.ToList();
            }
            throw new ArgumentException($"Pin {pinName} not found in the list.");
        }

        // Extracts a subset of pins
        public PinListData Copy(string extractList)
        {
            var extractedPins = new PinListData();
            var pinNames = extractList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(pin => pin.Trim());

            foreach (var pin in pinNames)
            {
                if (pinData.ContainsKey(pin))
                {
                    foreach (var site in pinData[pin])
                    {
                        extractedPins.Add(pin, site.Key, site.Value);
                    }
                }
                else
                {
                    throw new ArgumentException($"Pin {pin} not found in the list.");
                }
            }
            return extractedPins;
        }

        // Returns all pin names
        public List<string> GetAllPinNames()
        {
            return new List<string>(PinNames);
        }

        // Returns all pin data
        public Dictionary<string, Dictionary<int, double>> GetAllPinData()
        {
            return new Dictionary<string, Dictionary<int, double>>(pinData);
        }

        // Returns all site numbers
        public List<int> GetAllSiteNumbers()
        {
            return pinData.Values.SelectMany(dict => dict.Keys).Distinct().ToList();
        }

        // Returns the count of site data for a specific pin
        public int GetSiteDataCount(string pin)
        {
            if (pinData.ContainsKey(pin))
            {
                return pinData[pin].Count;
            }
            throw new ArgumentException($"Pin {pin} not found in the list.");
        }

        // Adds pin data from a list of values
        public void AddPinData(string pin, List<double> values)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                PinNames.Add(pin);
            }

            for (int i = 0; i < values.Count; i++)
            {
                pinData[pin][i] = values[i];
            }
        }

        // Decomposes pin groups by site numbers
        public List<PinListData> DecomposeBySite()
        {
            var siteGroups = new Dictionary<int, PinListData>();

            foreach (var pin in pinData)
            {
                foreach (var site in pin.Value)
                {
                    if (!siteGroups.ContainsKey(site.Key))
                    {
                        siteGroups[site.Key] = new PinListData();
                    }
                    siteGroups[site.Key].Add(pin.Key, site.Key, site.Value);
                }
            }

            return siteGroups.Values.ToList();
        }

        // Returns the entire dictionary of site data for a specific pin
        public Dictionary<int, double> GetPinSiteData(string pinName)
        {
            if (pinData.ContainsKey(pinName))
            {
                return new Dictionary<int, double>(pinData[pinName]);
            }
            throw new ArgumentException($"Pin {pinName} not found in the list.");
        }

        // Method to convert double[] to PinData<double>[]
        private PinData<double>[] ConvertToPinDataArray(double[] values, List<string> pinNames)
        {
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            PinData<double>[] pinDataArray = new PinData<double>[numSite];

            for (int site = 0; site < numSite; site++)
            {
                pinDataArray[site] = new PinData<double>(pinNames[site])
                {
                    SiteData = values.Skip(site * values.Length / numSite).Take(values.Length / numSite).ToList()
                };
            }

            return pinDataArray;
        }


    }*/

    public enum PinLevelNames
    {
        Vil,
        Vih,
        Vol,
        Voh,
        Iol,
        Ioh,
        Vt,
        Vch,
        Vcl
    }

    public class tlDCVICompliance
    {
        public const int Positive = 0;
        public const int Negative = 1;
        public const int Both = 2;
    }

    public enum tlDCVILocalKelvin
    {
        Both = 0,
        High = 1,
        Low = 2
    }

    public enum ChPinLevel
    {
        chIOh,
        chIOl,
        chVch,
        chVcl,
        chVih,
        chVil,
        chVoh,
        chVol,
        chVt
    }

    public class DCVIConstants
    {
        public const int tlDCVIConnectHighForce = 0b01; // Equivalent to decimal 1
        public const int tlDCVIConnectHighSense = 0b10; // Equivalent to decimal 2
                                                        // Add other connection constants here as needed
    }

    public class DCVIMode
    {
        public const int tlDCVIModeVoltage = 0;
        public const int tlDCVIModeCurrent = 1;
    }

    public class DCVIMeterMode
    {
        public const int tlDCVIMeterVoltage = 0;
        public const int tlDCVIMeterCurrent = 1;
        // Add other modes as necessary
    }

    public enum tlPPMUReadWhat
    {
        // Get the measurements. Default.
        tlPPMUReadMeasurements,

        // Get the pass/fail results.
        tlPPMUReadPassFailResults
    }

    public class Pattern
    {
        public string Data { get; set; }

        public Pattern(string data)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(data);
            Data = fileNameWithoutExtension;
        }

        // Parameterless constructor
        //public Pattern()
        //{
        //}
    
        // Implicit conversion from string to Pattern
        public static implicit operator Pattern(string data)
        {
            // Get the file name with extension
            //string fileNameWithExtension = Path.GetFileName(data);
            // Remove the extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(data);
            return new Pattern(fileNameWithoutExtension);
        }
    }
    public class PatternSet
    {
        public string Data { get; set; }

        public PatternSet(string data)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(data);
            Data = fileNameWithoutExtension;
        }

        // Parameterless constructor
        //public Pattern()
        //{
        //}

        // Implicit conversion from string to Pattern
        public static implicit operator PatternSet(string data)
        {
            // Get the file name with extension
            //string fileNameWithExtension = Path.GetFileName(data);
            // Remove the extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(data);
            return new PatternSet(fileNameWithoutExtension);
        }
    }

    //public class PatternSet
    //{
    //    public List<Pattern> Patterns { get; private set; }

    //    public PatternSet()
    //    {
    //        Patterns = new List<Pattern>();
    //    }

    //    public bool Add(Pattern pattern)
    //    {
    //        if (!Patterns.Contains(pattern))
    //        {
    //            Patterns.Add(pattern);
    //            Console.WriteLine($"Added pattern: {pattern.Data}"); // Debug output
    //            return true;
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Failed to add pattern: {pattern.Data}"); // Debug output
    //        }
    //        return false;
    //    }

    //    // Overload of Add method to accept string
    //    public bool Add(string patternData)
    //    {
    //        return Add((Pattern)patternData);
    //    }

    //    public bool Remove(Pattern pattern)
    //    {
    //        return Patterns.Remove(pattern);
    //    }

    //    public bool Contains(Pattern pattern)
    //    {
    //        return Patterns.Contains(pattern);
    //    }

    //    public List<Pattern> GetAll()
    //    {
    //        return Patterns;
    //    }

    //    public void Clear()
    //    {
    //        Patterns.Clear();
    //    }

    //    public int Count
    //    {
    //        get { return Patterns.Count; }
    //    }

    //    public static PatternSet FromPatternArray(Pattern[] patternArray)
    //    {
    //        PatternSet patternSet = new PatternSet();
    //        foreach (var pattern in patternArray)
    //        {
    //            patternSet.Add(pattern);
    //        }
    //        return patternSet;
    //    }

    //    // Implicit conversion from Pattern to PatternSet
    //    public static implicit operator PatternSet(Pattern pattern)
    //    {
    //        PatternSet patternSet = new PatternSet();
    //        patternSet.Add(pattern);
    //        return patternSet;
    //    }

    //    // Implicit conversion from string to PatternSet
    //    public static implicit operator PatternSet(string patternData)
    //    {
    //        PatternSet patternSet = new PatternSet();
    //        patternSet.Add(patternData);
    //        return patternSet;
    //    }
    //}

    public class DriveState
    {
        public const int chStaticStateDisable = 0;
        public const int chStaticStateHi = 1;
        public const int chStaticStateHiZ = 2;
        public const int chStaticStateLo = 3;
    }
    public enum ChEdge
    {
        chEdgeD0, // Drive On
        chEdgeD1, // Drive Data
        chEdgeD2, // Drive Return
        chEdgeD3, // Drive Off
        chEdgeR0, // Compare Open
        chEdgeR1  // Compare Close
    }

    // Enumeration for ChDriveFormat
    public enum ChDriveFormat
    {
        chDrvFmtCLKHI,
        chDrvFmtCLKLO,
        chDrvFmtCS,
        chDrvFmtCSC,
        chDrvFmtDrvClkHi,
        chDrvFmtDrvClkLo,
        chDrvFmtDrvHi,
        chDrvFmtDrvLo,
        chDrvFmtDrvOS,
        chDrvFmtFHi,
        chDrvFmtFLo,
        chDrvFmtFreqCnt,
        chDrvFmtNRZ,
        chDrvFmtNRZC,
        chDrvFmtOFF,
        chDrvFmtOSC,
        chDrvFmtRO,
        chDrvFmtROC,
        chDrvFmtRZ,
        chDrvFmtRZC,
        chDrvFmtST,
        chDrvFmtUnknown,
        chDrvFmtZS,
        chDrvFmtZSC
    }

    public enum CallerType
    {
        DCVI,
        PPMU,
        Digital
    }

    public enum TlRelayMode
    {
        tlUnpowered, // Cold Switching
        tlPowered    // Hot Switching
    }

    public enum tlStrobeOption
    {
        tlStrobe,
        tlNoStrobe
    }

    public enum tlDCVIMeterReadingFormat
    {
        tlDCVIMeterReadingFormatAverage,
        tlDCVIMeterReadingFormatArray
    }
    public class tlDCVIConnectWhat
    {
        // Not used to program connections. Only used when retrieving connection status using the Connected property. Use pin.disconnect to disconnect pins.
        public const int tlDCVIConnectNone = 0b00; // Equivalent to decimal 0

        // Disconnects only the high force.
        public const int tlDCVIConnectHighForce = 0b001;

        // Disconnects only the high sense.
        public const int tlDCVIConnectHighSense = 0b010;

        // Disconnects only high guard.
        public const int tlDCVIConnectHighGuard = 0b011;

        // Disconnects only the low force. Using this produces a run-time error for the DC30 as the low side is always connected to ground.
        public const int tlDCVIConnectLowForce = 0b100;

        // Disconnect low sense. Using this produces a run-time error for the DC30 as the low side is always connected to ground.
        public const int tlDCVIConnectLowSense = 0b101;

        // Disconnects force sense and guard for the channel. Default.
        public const int tlDCVIConnectDefault = 0b110;
    }

    // Enumerations and other classes
    public enum tlUtilityAlarm
    {
        tlUtilityAlarmUb // Raises alarms for all Utility alarm types. Default.
    }

    public enum tlAlarmBehavior
    {
        tlAlarmForceFail, // Forces the test to fail if an alarm of the specified type occurs. Default.
        tlAlarmForceBin,  // Forces the test to fail and bins the part to the error bin if an alarm of the specified type occurs.
        tlAlarmOff,       // Disables the specified alarm in hardware. Removes the time overhead normally required to process alarms.
        tlAlarmDefault,   // Sets the alarm to the default action for the BBAC Capture, which is tlAlarmForceFail.
        tlAlarmContinue   // Records and reports an alarm of the specified type but does not affect the pass/fail state or binning. Processes alarms so processing time occurs.
    }

    public enum tlUtilBitState
    {
        tlUtilBitOff, // Relay is open (off).
        tlUtilBitOn   // Relay is closed (on).
    }

    public enum tlUBState
    {
        tlUBStateProgrammed, // Read back the programmed state. Default.
        tlUBStateCompared    // Read back the compared (Comparator output) state.
    }

    // Enum for NoHaltMode
    public enum NoHaltMode
    {
        noHaltAlways,
        noHaltOff,
        noHaltTilCycle
    }
    // Enum for MemType
    public enum MemType
    {
        memAll,
        memAsioDnld,
        memCtoDnld,
        memLvm,
        memLvmScan,
        memLvmSvm,
        memMtoDnld,
        memNone,
        memSvm
    }

    // Enum for pfType
    public enum pfType
    {
        pfAlways,
        pfFailsOnly,
        pfNever
    }

    public class PinData<T>
    {
        public string PinName { get; set; }
        public List<T> SiteData { get; set; }

        public PinData(string pinName)
        {
            PinName = pinName;
            SiteData = new List<T>();
        }
    }

    public class PinListData
    {
        // Dictionary to store pin data for each site
        private Dictionary<string, Dictionary<int, double>> pinData;

        // List to store pin names
        private List<string> pinNames;
        // Constructor to initialize from a PinList

        public PinListData(PinList pinList)
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            pinNames = pinList.GetAll();
        }

        // Constructor to initialize the pin data and pin names
        public PinListData()
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            pinNames = new List<string>();
        }

        // Constructor to initialize from a double value
        public PinListData(double value)
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            pinNames = new List<string>();

            // Assuming a single pin name for simplicity
            string pinName = "Pin1";
            pinNames.Add(pinName);

            // Assuming a single site for simplicity
            Add(pinName, 0, value);
        }

        // Constructor to initialize from a double array
        // Constructor to initialize from a double array
        public PinListData(double[] values, List<string> pinNames)
        {
            pinData = new Dictionary<string, Dictionary<int, double>>();
            this.pinNames = pinNames;

            // Assign values to different sites
            int numSite = Globals.tsmContext.SiteNumbers.Count();
            for (int site = 0; site < numSite; site++)
            {
                Add(pinNames[site], site, values[site]);
            }
        }

        // Adds data for a specific pin and site
        public void Add(string pin, int site, double value)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                pinNames.Add(pin);
            }
            pinData[pin][site] = value;
        }

        // Declares or defines a single pin with default values for each site
        public void DeclarePin(string pin, List<int> sites, double defaultValue)
        {
            foreach (var site in sites)
            {
                Add(pin, site, defaultValue);
            }
        }

        // Adds a group of pins with default values for each site
        public void AddGroup(List<string> pins, List<int> sites, double defaultValue)
        {
            foreach (var pin in pins)
            {
                DeclarePin(pin, sites, defaultValue);
            }
        }

        // Adds a pin without specifying sites and data
        public void AddPin(string pin)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                pinNames.Add(pin);
            }
        }

        // Extracts data for a single pin across all sites
        public List<double> PinData(string pinName)
        {
            if (pinData.ContainsKey(pinName))
            {
                return pinData[pinName].Values.ToList();
            }
            throw new ArgumentException($"Pin {pinName} not found in the list.");
        }

        // Extracts a subset of pins
        public PinListData Copy(string extractList)
        {
            var extractedPins = new PinListData();
            var pinNames = extractList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(pin => pin.Trim());

            foreach (var pin in pinNames)
            {
                if (pinData.ContainsKey(pin))
                {
                    foreach (var site in pinData[pin])
                    {
                        extractedPins.Add(pin, site.Key, site.Value);
                    }
                }
                else
                {
                    throw new ArgumentException($"Pin {pin} not found in the list.");
                }
            }
            return extractedPins;
        }

        // Returns all pin names
        public List<string> GetAllPinNames()
        {
            return new List<string>(pinNames);
        }

        // Returns all pin data
        public Dictionary<string, Dictionary<int, double>> GetAllPinData()
        {
            return new Dictionary<string, Dictionary<int, double>>(pinData);
        }

        // Returns all site numbers
        public List<int> GetAllSiteNumbers()
        {
            return pinData.Values.SelectMany(dict => dict.Keys).Distinct().ToList();
        }

        // Returns the count of site data for a specific pin
        public int GetSiteDataCount(string pin)
        {
            if (pinData.ContainsKey(pin))
            {
                return pinData[pin].Count;
            }
            throw new ArgumentException($"Pin {pin} not found in the list.");
        }

        // Adds pin data from a list of values
        public void AddPinData(string pin, List<double> values)
        {
            if (!pinData.ContainsKey(pin))
            {
                pinData[pin] = new Dictionary<int, double>();
                pinNames.Add(pin);
            }

            for (int i = 0; i < values.Count; i++)
            {
                pinData[pin][i] = values[i];
            }
        }

        // Decomposes pin groups by site numbers
        public List<PinListData> DecomposeBySite()
        {
            var siteGroups = new Dictionary<int, PinListData>();

            foreach (var pin in pinData)
            {
                foreach (var site in pin.Value)
                {
                    if (!siteGroups.ContainsKey(site.Key))
                    {
                        siteGroups[site.Key] = new PinListData();
                    }
                    siteGroups[site.Key].Add(pin.Key, site.Key, site.Value);
                }
            }

            return siteGroups.Values.ToList();
        }

        // Returns the entire dictionary of site data for a specific pin
        public Dictionary<int, double> GetPinSiteData(string pinName)
        {
            if (pinData.ContainsKey(pinName))
            {
                return new Dictionary<int, double>(pinData[pinName]);
            }
            throw new ArgumentException($"Pin {pinName} not found in the list.");
        }

        // Implicit conversion from double to PinListData
        public static implicit operator PinListData(double value)
        {
            return new PinListData(value);
        }

        // Implicit conversion from double[] to PinListData
        public static implicit operator PinListData(double[] values)
        {
            List<string> pinNames = new List<string>(); // Retrieve pin names from context or other source
            return new PinListData(values, pinNames);
        }

        public static implicit operator PinListData((double[] values, List<string> pinNames) data)
        {
            return new PinListData(data.values, data.pinNames);
        }

        // Method to flatten pin data to a List of doubles
        public List<double> ToList()
        {
            return pinData.Values.SelectMany(siteData => siteData.Values).ToList();
        }

        // Method to convert the flattened List of doubles to an array
        public double[] ToArray()
        {
            return ToList().ToArray();
        }
    }

    public class PinList
    {
        public List<string> pins;
        public Regex pinFormatRegex = new Regex(@"^[\w\W]+$", RegexOptions.IgnoreCase);

        public PinList()
        {
            pins = new List<string>();
        }

        public bool Add(string pin)
        {
            if (pinFormatRegex.IsMatch(pin) && !pins.Contains(pin))
            {
                pins.Add(pin);
                Console.WriteLine($"Added pin: {pin}"); // Debug output
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to add pin: {pin}"); // Debug output
            }
            return false;
        }

        public bool Add(Pattern pattern)
        {
            return Add(pattern.Data);
        }

        //public bool Add(PatternSet patternSet)
        //{
        //    bool allAdded = true;
        //    foreach (var pattern in patternSet.Patterns)
        //    {
        //        if (!Add(pattern))
        //        {
        //            allAdded = false;
        //        }
        //    }
        //    return allAdded;
        //}

        public bool Remove(string pin)
        {
            return pins.Remove(pin);
        }

        public bool Remove(Pattern pattern)
        {
            return Remove(pattern.Data);
        }

        //public bool Remove(PatternSet patternSet)
        //{
        //    bool allRemoved = true;
        //    foreach (var pattern in patternSet.Patterns)
        //    {
        //        if (!Remove(pattern))
        //        {
        //            allRemoved = false;
        //        }
        //    }
        //    return allRemoved;
        //}

        public bool Contains(string pin)
        {
            return pins.Contains(pin);
        }

        public bool Contains(Pattern pattern)
        {
            return Contains(pattern.Data);
        }

        //public bool Contains(PatternSet patternSet)
        //{
        //    foreach (var pattern in patternSet.Patterns)
        //    {
        //        if (!Contains(pattern))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public List<string> GetAll()
        {
            return pins;
        }

        public void Clear()
        {
            pins.Clear();
        }

        public List<string> Value
        {
            get { return pins; }
        }

        public int Count
        {
            get { return pins.Count; }
        }

        public static PinList FromStringArray(string[] pinArray)
        {
            PinList pinList = new PinList();
            foreach (var pin in pinArray)
            {
                pinList.Add(pin);
            }
            return pinList;
        }

        public static PinList FromPatternArray(Pattern[] patternArray)
        {
            PinList pinList = new PinList();
            foreach (var pattern in patternArray)
            {
                pinList.Add(pattern);
            }
            return pinList;
        }

        // Implicit conversion from string to PinList
        public static implicit operator PinList(string pin)
        {
            PinList pinList = new PinList();
            string[] pins = pin.Split(',');

            foreach (string p in pins)
            {
                pinList.Add(p.Trim());
            }

            return pinList;
        }

        // Implicit conversion from Pattern to PinList
        public static implicit operator PinList(Pattern pattern)
        {
            PinList pinList = new PinList();
            pinList.Add(pattern);
            return pinList;
        }

        //// Implicit conversion from PatternSet to PinList
        //public static implicit operator PinList(PatternSet patternSet)
        //{
        //    PinList pinList = new PinList();
        //    pinList.Add(patternSet);
        //    return pinList;
        //}
    }

}
