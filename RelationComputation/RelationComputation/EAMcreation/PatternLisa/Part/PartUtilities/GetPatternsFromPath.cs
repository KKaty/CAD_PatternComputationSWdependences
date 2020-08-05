using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        //It detects all the symmetry relations in a set of MyRepeatedEntity 
        //on a given MyPathOfPoints.
        //(symmetry types considered: translation)  <-- reflection? rotation?       
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static bool GetPatternsFromPath(MyPathOfPoints myPathOfPoints,
           List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyPathOfPoints> listOfPathOfCentroids, 
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurface> listOfMyGroupingSurface,
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPattern> listOfOutputPattern,
            ref List<MyPattern> listOfOutputPatternTwo)
        {
            var listOfREOnThePath = myPathOfPoints.path.Select(ind => listOfREOnThisSurface[ind]).ToList();

            if (myPathOfPoints.pathGeometricObject.GetType() == typeof (MyLine))
            {
                const string nameFile = "GetLinearPatterns.txt";
                KLdebug.Print(" ", nameFile);
                KLdebug.Print("POSSIBILE TRASLAZIONE retta. AVVIO", nameFile);
                return GetPatternsFromLinearPath(listOfREOnThePath, myPathOfPoints.pathGeometricObject,
                    ref listOfPathOfCentroids, listOfREOnThisSurface, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                    listOfInitialGroupingSurface, ref listOfOutputPattern, ref listOfOutputPatternTwo);
            }
            else
            {
                const string nameFile = "GetCircularPatterns.txt";
                KLdebug.Print(" ", nameFile);
                KLdebug.Print("POSSIBILE TRASLAZIONE o ROTAZIONE su circonferenza. AVVIO", nameFile);
                return GetPatternsFromCircularPath(listOfREOnThePath, myPathOfPoints.pathGeometricObject,
                    ref listOfPathOfCentroids, listOfREOnThisSurface, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                    listOfInitialGroupingSurface, ref listOfOutputPattern, ref listOfOutputPatternTwo);
 
            }
        }

        //It detects all the TRANSLATIONAL relations in a set of MyRepeatedEntity 
        //whose centroids lie on a given line.
        //it saves patterns of length = 2 in a list;
        //it saves patterns of length > 2 in a list.
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static bool GetPatternsFromLinearPath(List<MyRepeatedEntity> listOfREOnThePath,
            MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyGroupingSurface> listOfMyGroupingSurface,
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPattern> listOfOutputPattern,
            ref List<MyPattern> listOfOutputPatternTwo)
        {
            var numOfRE = listOfREOnThePath.Count;
            var noStop = true;

            const string nameFile = "GetLinearPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("VERIFICA DELLE POSSIBILI TRASLAZIONI TRA " + numOfRE + " REPEATED ENTITY:", nameFile);
            KLdebug.Print(" ", nameFile);

            var i = 0;
            while (i < (numOfRE - 1))
            {
                var newPattern = new MyPattern();
                var foundNewPattern = GetMaximumTranslation(listOfREOnThePath, pathObject, ref i, ref numOfRE, 
                    ref noStop, ref newPattern, listOfInitialGroupingSurface);

                if (foundNewPattern)
                {
                    //if (newPattern.listOfMyREOfMyPattern.Count == numOfRE || newPattern.listOfMyREOfMyPattern.Count == numOfRE - 1)
                    //if (newPattern.listOfMyREOfMyPattern.Count == numOfRE)
                    //{
                    //    noStop = true;
                    //}

                    CheckAndUpdate(newPattern, ref listOfPathOfCentroids,
                        listOfREOnThisSurface, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }
            }
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
                KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            KLdebug.Print("C'E STATA ALMENO UNA INTERRUZIONE: PATTERN DI LUNGHEZZA non MASSIMA SU QUESTO PATH!", nameFile);
            return false;
        }

        public static bool GetPatternsFromCircularPath(List<MyRepeatedEntity> listOfREOnThePath,
            MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyGroupingSurface> listOfMyGroupingSurface, 
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPattern> listOfOutputPattern,
            ref List<MyPattern> listOfOutputPatternTwo)
        {
            var numOfRE = listOfREOnThePath.Count;
            var noStop = true;

            const string nameFile = "GetCircularPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("VERIFICA DELLE POSSIBILI TRASLAZIONI SU CIRCONFERENZA O ROTAZIONI TRA " + numOfRE + " REPEATED ENTITY:", nameFile);
            KLdebug.Print(" ", nameFile);

            var i = 0;
            while (i < (numOfRE - 1))
            {
                var newPattern = new MyPattern();
                var j = i;
                var foundNewPattern = GetMaximumTranslation(listOfREOnThePath, pathObject, ref j, ref numOfRE, ref noStop, 
                    ref newPattern, listOfInitialGroupingSurface);

                if (foundNewPattern == false)
                {
                    var pathCircumference = (MyCircumForPath)pathObject;
                    foundNewPattern = GetMaximumRotation(listOfREOnThePath, pathCircumference, ref i, ref numOfRE, ref noStop, 
                        ref newPattern, listOfInitialGroupingSurface);
                }
                else
                {
                    i = j;
                }

                if (foundNewPattern)
                {
                    if (newPattern.listOfMyREOfMyPattern.Count == numOfRE || newPattern.listOfMyREOfMyPattern.Count == numOfRE - 1)
                    {
                        noStop = true;
                    }

                    CheckAndUpdate(newPattern, ref listOfPathOfCentroids,
                        listOfREOnThisSurface, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }

            }
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
                KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            return false;
        }
    }
}
