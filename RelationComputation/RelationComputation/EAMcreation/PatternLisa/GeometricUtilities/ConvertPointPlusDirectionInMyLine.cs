using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //I create MyLine corresponding to the axis of the cylinder, using two points passing through this line:
        //(the origin of the cylinder) and (the origin of the cylinder + direction vector)

        public static MyLine ConvertPointPlusDirectionInMyLine(double[] appPoint, double[] direction)
        {
            MyVertex firstPoint = new MyVertex(appPoint[0], appPoint[1], appPoint[2]);
            MyVertex secondPoint = new MyVertex(appPoint[0] + direction[0], appPoint[1] + direction[1], appPoint[2] + direction[2]);

            MyLine outputLine = LinePassingThrough(firstPoint, secondPoint);
            return outputLine;
        }
    }
}
