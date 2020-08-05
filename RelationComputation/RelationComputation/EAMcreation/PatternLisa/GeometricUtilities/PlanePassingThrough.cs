using System;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //Restituisce l'equazione cartesiana del piano passante per V1, V2, V3
        public static MyPlane PlanePassingThrough(MyVertex V1, MyVertex V2, MyVertex V3 /*, ref StringBuilder fileOutput*/)
        {
            //manca il controllo se i punti se i 3 punti sono coincidenti...

            MyPlane OutputPlane = new MyPlane();
            if (V1.Lieonline(LinePassingThrough(V2, V3)))
            {
                return OutputPlane; // Plane does not exist
            }
            else
            {
                var aa = (V2.y - V1.y) * (V3.z - V1.z) - (V3.y - V1.y) * (V2.z - V1.z);
                var bb = -((V2.x - V1.x) * (V3.z - V1.z) - (V3.x - V1.x) * (V2.z - V1.z));
                var cc = (V2.x - V1.x) * (V3.y - V1.y) - (V3.x - V1.x) * (V2.y - V1.y);
                var norm = Math.Sqrt(Math.Pow(aa, 2) + Math.Pow(bb, 2) + Math.Pow(cc, 2));
                OutputPlane.a = aa/ norm;
                OutputPlane.b = bb/ norm;
                OutputPlane.c = cc/ norm;
                OutputPlane.d = -OutputPlane.a * V1.x - OutputPlane.b * V1.y - OutputPlane.c * V1.z;

                //VERSIONE VECCHIA CON CUI FUNZIONAVA: SE IN FUTURO SORGONO PROBLEMI TORNARE A QUESTA
                //OutputPlane.a = (V2.y - V1.y) * (V3.z - V1.z) - (V3.y - V1.y) * (V2.z - V1.z);
                //OutputPlane.b = -((V2.x - V1.x) * (V3.z - V1.z) - (V3.x - V1.x) * (V2.z - V1.z));
                //OutputPlane.c = (V2.x - V1.x) * (V3.y - V1.y) - (V3.x - V1.x) * (V2.y - V1.y);
                //OutputPlane.d = -OutputPlane.a * V1.x - OutputPlane.b * V1.y - OutputPlane.c * V1.z;
                
            }

            return OutputPlane;
        }

    }
}
