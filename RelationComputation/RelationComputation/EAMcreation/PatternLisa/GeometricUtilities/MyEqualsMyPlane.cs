using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //This function returns true if the 2 MyLine correspond to the same line in 3D space, false otherwise.
        //First the function verifies if the two lines have the same direction versor, then it verifies if
        //the point lying on the first line also lies on the second line, and viceversa.
        //The direction versor of each line is computed by computing the vector product of the two normal
        //vectors of the two planes defining the line.
        public static bool MyEqualsMyPlane(MyLine firstLine, MyLine secondLine, MyVertex firstPoint, MyVertex secondPoint)
        {
            var firstDirection = firstLine.direction;      
            var secondDirection = secondLine.direction;

            var secondDirectionInverted = new double[3];
            secondDirectionInverted.SetValue(-secondDirection[0],0);
            secondDirectionInverted.SetValue(-secondDirection[1],1);
            secondDirectionInverted.SetValue(-secondDirection[2],2);


            if (FunctionsLC.MyEqualsArray(firstDirection, secondDirection) || FunctionsLC.MyEqualsArray(firstDirection, secondDirectionInverted))
            {
                KLdebug.Print("direzioni uguali", "prova.txt");

                var firstVerify = firstPoint.Lieonline(secondLine);
                var secondVerify = secondPoint.Lieonline(firstLine);
                KLdebug.Print("firstVerify of point " + firstVerify, "prova.txt");
                KLdebug.Print("secondVerify of point" + secondVerify, "prova.txt");
                if (firstPoint.Lieonline(secondLine) && secondPoint.Lieonline(firstLine))
                {             
                    return true;
                }
                return false;
            }            
            return false;
        }      
    }
}