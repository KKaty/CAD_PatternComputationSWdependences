using System;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //INPUT: MyVertex V1, MyVertex V2, MyVertex C
        //It returns the angle teta between the two vectors C V1 and C V2
        public static double FindAngle(MyVertex V1, MyVertex V2, MyVertex C)
        {
            //var whatToWrite = string.Format("PROVA: ");
            //KLdebug.Print(whatToWrite, "AngleComputation.txt");

            double outputAngle = 0.0;
            if (V1.Equals(V2) || V1.Equals(C) || V1.Equals(V2))
            {
                return outputAngle;
            }
            if (C.Lieonline(LinePassingThrough(V1,V2)))
            {
                outputAngle = Math.PI;
                return outputAngle;
            }
            //KLdebug.Print("calcolo:", "prova.txt");

            if (Math.Abs(C.x) < Math.Pow(10, -10))
            {
                C.x = 0;
            }
            if (Math.Abs(C.y) < Math.Pow(10, -10))
            {
                C.y = 0;
            }
            if (Math.Abs(C.z) < Math.Pow(10, -10))
            {
                C.z = 0;
            }
            //whatToWrite = string.Format("centro ({0},{1},{2}) ", C.x, C.y, C.z);
            //KLdebug.Print(whatToWrite, "prova.txt");

            double[] vectorV1 = { V1.x - C.x, V1.y - C.y, V1.z - C.z };
            if (Math.Abs(vectorV1[0]) < Math.Pow(10, -10))
            {
                vectorV1[0] = 0;
            }
            if (Math.Abs(vectorV1[1]) < Math.Pow(10, -10))
            {
                vectorV1[1] = 0;
            }
            if (Math.Abs(vectorV1[2]) < Math.Pow(10, -10))
            {
                vectorV1[2] = 0;
            }
            //whatToWrite = string.Format("vec1 ({0},{1},{2}) ", vectorV1[0], vectorV1[1], vectorV1[2]);
            //KLdebug.Print(whatToWrite, "prova.txt");

            double vectorV1Norm =
                Math.Sqrt(Math.Pow(vectorV1[0], 2) + Math.Pow(vectorV1[1], 2) + Math.Pow(vectorV1[2], 2));
            //KLdebug.Print("norma vec1 " + vectorV1Norm, "prova.txt");

            double[] vectorV2 = { V2.x - C.x, V2.y - C.y, V2.z - C.z };
            if (Math.Abs(vectorV2[0]) < Math.Pow(10, -10))
            {
                vectorV2[0] = 0;
            }
            if (Math.Abs(vectorV2[1]) < Math.Pow(10, -10))
            {
                vectorV2[1] = 0;
            }
            if (Math.Abs(vectorV2[2]) < Math.Pow(10, -10))
            {
                vectorV2[2] = 0;
            }
            //whatToWrite = string.Format("vec2 ({0},{1},{2}) ", vectorV2[0], vectorV2[1], vectorV2[2]);
            //KLdebug.Print(whatToWrite, "prova.txt");

            double vectorV2Norm =
                Math.Sqrt(Math.Pow(vectorV2[0], 2) + Math.Pow(vectorV2[1], 2) + Math.Pow(vectorV2[2], 2));
            //KLdebug.Print("norma vec2 " + vectorV2Norm, "prova.txt");

            double scalarProduct = vectorV1[0]*vectorV2[0] + vectorV1[1]*vectorV2[1] + vectorV1[2]*vectorV2[2];
            //KLdebug.Print("scalarProduct " + scalarProduct, "prova.txt");

            //KLdebug.Print("faccio l'arcos di: " + scalarProduct / (vectorV1Norm * vectorV2Norm), "prova.txt");

            if (vectorV1Norm*vectorV2Norm != 0)
            {
                outputAngle = Math.Acos(scalarProduct/(vectorV1Norm*vectorV2Norm));
            }
            //KLdebug.Print("outputAngle " + outputAngle, "prova.txt");

            if (!Double.IsNaN(outputAngle))
            {
                return outputAngle;
            }
            return -1;
        }
    }
}
