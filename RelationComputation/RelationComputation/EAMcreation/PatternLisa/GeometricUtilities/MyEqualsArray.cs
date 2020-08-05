using System;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        public static bool MyEqualsArray(double[] first, double[] second)
        {
            var tolerance = Math.Pow(10, -2);
            return Math.Abs(Accord.Math.Matrix.InnerProduct(first, second) - 1) < tolerance;

            //var tolerance = Math.Pow(10, -5);
            //return (Math.Abs((double)first.GetValue(0) - (double)second.GetValue(0)) < tolerance &&
            //        Math.Abs((double)first.GetValue(1) - (double)second.GetValue(1)) < tolerance &&
            //        Math.Abs((double)first.GetValue(2) - (double)second.GetValue(2)) < tolerance);
        }

        public static bool KLMyEqualsArray(double[] first, double[] second)
        {
            var tolerance = Math.Pow(10, -2);
            return Math.Abs(Accord.Math.Matrix.InnerProduct(first, second) - 1) < tolerance;
            
        }

        public static bool MyEqualsToZero(double number)
        {
            var tolerance = Math.Pow(10, -6);
            return (Math.Abs(number) < tolerance);
        }
    }
}
