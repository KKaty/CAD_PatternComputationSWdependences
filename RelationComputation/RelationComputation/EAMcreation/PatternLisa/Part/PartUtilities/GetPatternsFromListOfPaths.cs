using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {
        //It detects all the symmetry relations in a set of MyRepeatedEntity 
        //(symmetry types considered: translation, reflection,rotation)
        //it saves patterns of length = 1 in a list;
        //it saves patterns of length = 2 in a list;
        //it saves patterns of length > 2 in a list.
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static void GetPatternsFromListOfPaths(List<MyPathOfPoints> listOfMyPathsOfCentroids,
            List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyGroupingSurface> listOfMyGroupingSurface, List<MyGroupingSurface> listOfInitialGroupingSurface,
            ref List<MyPattern> listOfOutputPattern,
            ref List<MyPattern> listOfOutputPatternTwo)
        {
            ReorderListOfPaths(ref listOfMyPathsOfCentroids);


            while (listOfMyPathsOfCentroids.Count > 0)
            {
                var currentPathOfCentroids = new MyPathOfPoints(listOfMyPathsOfCentroids[0].path,
                    listOfMyPathsOfCentroids[0].pathGeometricObject);
                listOfMyPathsOfCentroids.RemoveAt(0);
                //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                var maxLength = GetPatternsFromPath(currentPathOfCentroids,
                    listOfREOnThisSurface, ref listOfMyPathsOfCentroids, ref listOfMatrAdj,
                    ref listOfMyGroupingSurface, listOfInitialGroupingSurface, ref listOfOutputPattern,
                    ref listOfOutputPatternTwo); 
            }
        }

        public static void ReorderListOfPaths(ref List<MyPathOfPoints> listOfMyPathsOfCentroids)
        {
            //const string nameFile = "ReorderTheListOfFoundPaths.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("LISTA DEI PATH PRIMA DELLO SPOSTAMENTO DEI 'PATH DA 3 LINEA' PRIMA DEI 'PATH DA 4 CRF':", nameFile);
            //foreach (var path in listOfMyPathsOfCentroids)
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print(listOfMyPathsOfCentroids.IndexOf(path) + "° path:", nameFile);
            //    foreach (var centroid in path.path)
            //    {
            //        KLdebug.Print("-" + centroid, nameFile);
            //    }
            //}

            // I move the set of paths of length=3 type Line before the set of paths of length=4 type circum.
            var firstIndPathLineThree = listOfMyPathsOfCentroids.FindIndex(
                pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyLine) && pathObj.path.Count == 3));
            //KLdebug.Print("firstIndPathLineThree=" + firstIndPathLineThree, nameFile);

            var firstIndPathCircumFour = listOfMyPathsOfCentroids.FindIndex(
                pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyCircumForPath) && pathObj.path.Count == 4));
            //KLdebug.Print("firstIndPathCircumFour= " + firstIndPathCircumFour, nameFile);

            if (firstIndPathLineThree != -1 && firstIndPathCircumFour != -1)
            {
                var listOfPathLineThree =
                    listOfMyPathsOfCentroids.FindAll(
                        pathObj => (pathObj.pathGeometricObject.GetType() == typeof (MyLine) && pathObj.path.Count == 3));
                //KLdebug.Print("LISTA DEI PATH DA 3 LINEA:", nameFile);
                //foreach (var path in listOfPathLineThree)
                //{
                //    KLdebug.Print(" ", nameFile);
                //    foreach (var centroid in path.path)
                //    {
                //        KLdebug.Print("-" + centroid, nameFile);
                //    }

                //}
                listOfMyPathsOfCentroids.RemoveRange(firstIndPathLineThree, listOfPathLineThree.Count);
                //KLdebug.Print("RIMOSSI I PATH DA 3 LINEA, SONO RIMASTI:", nameFile);
                //foreach (var path in listOfMyPathsOfCentroids)
                //{
                //    KLdebug.Print(" ", nameFile);
                //    foreach (var centroid in path.path)
                //    {
                //        KLdebug.Print("-" + centroid, nameFile);
                //    }

                //}
                listOfMyPathsOfCentroids.InsertRange(firstIndPathCircumFour, listOfPathLineThree);
            }


            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("LISTA DEI PATH DOPO lo SPOSTAMENTO DEI 'PATH DA 3 LINEA' PRIMA DEI 'PATH DA 4 CRF':", nameFile);
            //foreach (var path in listOfMyPathsOfCentroids)
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print(listOfMyPathsOfCentroids.IndexOf(path) + "° path:", nameFile);
            //    foreach (var centroid in path.path)
            //    {
            //        KLdebug.Print("-" + centroid, nameFile);
            //    }

            //}
        }
    }
}
