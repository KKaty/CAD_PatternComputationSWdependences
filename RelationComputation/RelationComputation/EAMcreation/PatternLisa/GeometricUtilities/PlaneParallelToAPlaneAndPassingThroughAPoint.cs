using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        // It creates the MyPlane passing through a given MyVertex and parallel to a given MyPlane    ------------>>>>>>> AL MOMENTO NON SERVE!
        public static MyPlane PlaneParallelToAPlaneAndPassingThroughAPoint(MyVertex givenPoint, MyPlane givenPlane)
        {
            double[] outputNormal = { givenPlane.a, givenPlane.b, givenPlane.c };
            double[] outputPointOfApplication = { givenPoint.x, givenPoint.y, givenPoint.z };
            var outputPlane = new MyPlane(outputNormal, outputPointOfApplication);
            return outputPlane;
        }
    }
}
