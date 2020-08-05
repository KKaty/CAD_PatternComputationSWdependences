using System;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //Restituisce l'equazione cartesiana della sfera passante per V1, V2, V3, V4
        //[DAI TEST: i 4 vertici stanno sulla sfera con precisione dell'ordine in media di 10^(-6) se 
        // le coordinate dei vertici sono numeri con al più 3 cifre dopo la virgola e 3 cifre prima della virgola.
        // Inoltre ho osservato che più aumenta la differenza tra l'ordine di grandezza tra tutte le coordinate dei vari
        // vertici e più diminuisce la precisione...o no???]
        public static MySphere SpherePassingThrough(MyVertex V1, MyVertex V2, MyVertex V3, MyVertex V4, ref StringBuilder fileOutput, ModelDoc2 SwModel)
        {
            //MySphere OutputMySphere = new MySphere();
            
            //First, the sphere does not exist if 3 of the 4 points lie on a line. Four possible line: V1-V2-V3, V1-V3-V4, V1-V2-V4, V2-V3-V4
            //Second, the sphere does not exist if the 4 points lie on the same plane.
            //(MANCA IL CONTROLLO CHE NON SIANO PUNTI COINCIDENTI...)

           if (V1.Lieonline(LinePassingThrough(V2, V3)) || V1.Lieonline(LinePassingThrough(V3, V4)) || V1.Lieonline(LinePassingThrough(V2, V4)) || V2.Lieonline(LinePassingThrough(V3, V4)))
            {
                fileOutput.AppendLine("Sistema non risolvibile");

                MySphere OutputMySphere = new MySphere();
                return OutputMySphere; // MySphere does not exist
            }
            else
            {
                double[,] CoefficientsMatrix = 
                {
                    { V1.x, V1.y, V1.z, 1 },
                    { V2.x, V2.y, V2.z, 1 },
                    { V3.x, V3.y, V3.z, 1 },
                    { V4.x, V4.y, V4.z, 1 },
                };

                double alfa = Math.Pow(V1.x, 2) + Math.Pow(V1.y, 2) + Math.Pow(V1.z, 2);
                double beta = Math.Pow(V2.x, 2) + Math.Pow(V2.y, 2) + Math.Pow(V2.z, 2);
                double gamma = Math.Pow(V3.x, 2) + Math.Pow(V3.y, 2) + Math.Pow(V3.z, 2);
                double delta = Math.Pow(V4.x, 2) + Math.Pow(V4.y, 2) + Math.Pow(V4.z, 2);

                double[,] KnownTerms = { { -alfa }, { -beta }, { -gamma }, { -delta } };

                double[,] SphereCoef = Matrix.Solve(CoefficientsMatrix, KnownTerms, leastSquares: true);

                //OutputMySphere.a = SphereCoef[0, 0]; OutputMySphere.b = SphereCoef[1, 0]; OutputMySphere.c = SphereCoef[2, 0]; OutputMySphere.d = SphereCoef[3, 0];
                MySphere OutputMySphere = new MySphere(SphereCoef[0, 0], SphereCoef[1, 0], SphereCoef[2, 0], SphereCoef[3, 0]);
                return OutputMySphere;
            }
            
        }
   
    }
}
