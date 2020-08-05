using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {
        //It detects all the symmetry relations in a set of MyRepeatedComponent 
        //it saves patterns of length = 1 in a list;
        //it saves patterns of length = 2 in a list;
        //it saves patterns of length > 2 in a list.
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static void GetPatternsFromListOfPaths_Assembly(List<MyPathOfPoints> listOfMyPathsOfPoints,
            List<MyRepeatedComponent> listOfComponents, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            Part.PartUtilities.GeometryAnalysis.ReorderListOfPaths(ref listOfMyPathsOfPoints);

            while (listOfMyPathsOfPoints.Count > 0)
            {
                var currentPathOfPoints = new MyPathOfPoints(listOfMyPathsOfPoints[0].path,
                    listOfMyPathsOfPoints[0].pathGeometricObject);
                listOfMyPathsOfPoints.RemoveAt(0);
                //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                var maxLength = GetPatternsFromPath_Assembly(currentPathOfPoints,
                    listOfComponents, ref listOfMyPathsOfPoints, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, SwApplication);
            }
        }



        public static void KLGetPatternsFromListOfPaths_Assembly(List<MyPathOfPoints> listOfMyPathsOfPoints,
            List<MyRepeatedComponent> listOfComponents, List<MyVertex> listCentroid, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            Part.PartUtilities.GeometryAnalysis.ReorderListOfPaths(ref listOfMyPathsOfPoints);
            while (listOfMyPathsOfPoints.Count > 0)
            {
                var firstIndex = listOfMyPathsOfPoints.IndexOf(listOfMyPathsOfPoints.First());
                var currentPathOfPoints = new MyPathOfPoints(listOfMyPathsOfPoints[firstIndex].path,
                    listOfMyPathsOfPoints[firstIndex].pathGeometricObject);
                listOfMyPathsOfPoints.RemoveAt(firstIndex);
                //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                //listOfOutputPatternTwo.Clear();
                //listOfOutputPattern.Clear();

                var maxLength = KLGetPatternsFromPath_Assembly(currentPathOfPoints,
                    listOfComponents, listCentroid, ref listOfMyPathsOfPoints, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, SwApplication);

            }
        }
    }
}
