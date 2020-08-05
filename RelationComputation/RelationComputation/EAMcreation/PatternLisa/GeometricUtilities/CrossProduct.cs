using System;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        public static double[] CrossProduct(double[] first, double[] second)
        {
            var x1 = (double) first.GetValue(0);
            var x2 = (double) first.GetValue(1);
            var x3 = (double) first.GetValue(2);

            var y1 = (double) second.GetValue(0);
            var y2 = (double) second.GetValue(1);
            var y3 = (double) second.GetValue(2);

            double[] vectorProduct = new double[3];
            vectorProduct.SetValue(x2*y3 - x3*y2, 0);
            vectorProduct.SetValue(x3*y1 - x1*y3, 1);
            vectorProduct.SetValue(x1*y2 - x2*y1, 2);
            return vectorProduct;
        }

        public static double[] Normalize(double[] vector)
        {
            var tolerance = Math.Pow(10, -10);
            var x1 = (double)vector.GetValue(0);
            var x2 = (double)vector.GetValue(1);
            var x3 = (double)vector.GetValue(2);

            if (Math.Abs(x1) < tolerance && Math.Abs(x2) < tolerance && Math.Abs(x3) < tolerance)
            {
                return vector;
            }
           
            var norm = Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(x2, 2) + Math.Pow(x3, 2));
            double[] vectorProduct = new double[3];
            vectorProduct.SetValue(x1 / norm, 0);
            vectorProduct.SetValue(x2 / norm, 1);
            vectorProduct.SetValue(x3 / norm, 2);
            return vectorProduct;
        }
    }
}
