using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //It returns the array correspondig to a point resulting from the intersection of a given MyPlane a given MyLine
        public static MyVertex PointIntersectionOfPlaneAndLine(MyPlane plane, MyLine line)
        {
            //... missing a check: not valid if the line lies on the plane!!!

            double[,] coefficientsMatrix = 
                {
                    { plane.a, plane.b, plane.c },
                    { line.plane1.a, line.plane1.b, line.plane1.c },
                    { line.plane2.a, line.plane2.b, line.plane2.c }
                };


            double[,] knownTerms = { { -plane.d }, { -line.plane1.d }, { -line.plane2.d } };

            double[,] outputPointColumn = Matrix.Solve(coefficientsMatrix, knownTerms, leastSquares: true);
            
            //I transform it in MyVertex:
            MyVertex outputPoint = new MyVertex(outputPointColumn[0, 0], outputPointColumn[1, 0], outputPointColumn[2, 0]);
            return outputPoint;
     
        }
    }
}
