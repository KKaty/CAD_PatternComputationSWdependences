using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part_ComposedPatterns
{
    public partial class Functions
    {
        // This function updates the list of paths adding the paths raising from the given 1st and 2nd seed set points.
        public static void TwoPointsGivenPaths_ComposedPatterns(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            int secondPointInd, List<MyPattern> listOfParallelPatterns, 
            List<MyVertex> listCentroid,
            List<int> listOfExtremePoints, ref List<int> listOfSimplePoints_Copy, List<int> listOfMBPoints,
            ref bool longestPattern, ref List<MyPathOfPoints> listOfPaths, ref List<int> listOfPenultimate,
            ref List<int> listOfLast, ref StringBuilder fileOutput, ref bool toleranceOk, 
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface,
            ref List<MyComposedPattern> listOfOutputComposedPattern, ref List<MyComposedPattern> listOfOutputComposedPatternTwo,
            ref List<int> listOfIndicesOfLongestPath, SldWorks SwApplication)
        {

            if (listOfExtremePoints.Contains(secondPointInd))
            //If the branch is an ExtremePoint, I treat it in a special way as I have not 
            //the path equation yet. So I guide the 3rd point search.
            {
                //I come back, so I look what are the branches of the StartPoint
                List<int> BranchesSecond = matrAdjToSee.matr.GetRow(startPointInd).Find(entry => entry == 1).ToList();
                BranchesSecond.Remove(secondPointInd);
                foreach (int branch2 in BranchesSecond)
                {
                    var newMyPathOfCentroids = new MyPathOfPoints();

                    //if (SecondPointInd(extreme), StartPointInd(MB), branch2 don't belong to an existing path in ListOfPaths)
                    if (listOfPaths.FindIndex(pathObject => (pathObject.path.Contains(secondPointInd) &&
                        pathObject.path.Contains(startPointInd) && pathObject.path.Contains(branch2))) == -1)
                    {
                        List<int> Path = new List<int>();
                        var pathCurve = new MyPathGeometricObject();

                        if (listCentroid[branch2].Lieonline(FunctionsLC.LinePassingThrough(
                            listCentroid[startPointInd], listCentroid[secondPointInd])))
                        {
                            Path = PathCreation_Part.Functions.ThreePointsGivenPathsLine(matrAdjToSee, listCentroid,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
                        }
                        else
                        {
                            Path = PathCreation_Part.Functions.ThreePointsGivenPathsCircum(matrAdjToSee, listCentroid,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
                        }

                        if (toleranceOk == false)
                        {
                            return;
                        }

                        //---> Storing of the new path found:
                        //First, if I have found a complete circumference I delete the repeated centroid index:
                        if (Path[0] == Path[Path.Count - 1])
                        {
                            Path.RemoveAt(Path.Count - 1);
                        }
                        newMyPathOfCentroids.path = Path;
                        newMyPathOfCentroids.pathGeometricObject = pathCurve;

                        listOfPaths.Add(newMyPathOfCentroids);

                        fileOutput.AppendLine("Aggiunto alla lista nuovo path: ");
                        fileOutput.AppendLine("  -numero di baricentri:" + newMyPathOfCentroids.path.Count);
                        if (pathCurve.GetType() == typeof(MyLine))
                        {
                            fileOutput.AppendLine("  -tipo di path: retta");
                            fileOutput.AppendLine("");
                        }
                        else
                        {
                            fileOutput.AppendLine("  -tipo di path: circonferenza");
                            fileOutput.AppendLine("");
                        }
      
                        //If a path of length n or n-1 is found, I move up the geometrical verify to see
                        //if the process can be stopped at this point (sufficiently satisfying result on this GS)
                        int m = Path.Count;
                        if (m == n || m == n - 1)
                        {
                            fileOutput.AppendLine("\n Trovato path che potrebbe dare un Longest Pattern: VERIFICO");
                     
                            // Removing of the path from the list:
                            listOfPaths.RemoveAt(listOfPaths.Count - 1);

                            if (newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyLine))
                            {
                                if (PartUtilities_ComposedPatterns.GeometryAnalysis.GetComposedPatternsFromPathLine(newMyPathOfCentroids,
                                    listOfParallelPatterns, ref listOfPaths,
                                    ref listOfMatrAdj, ref listOfMyGroupingSurface,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfCentroids);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));
                                }
                            }
                            else //newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyCircumForPath)
                            {
                                if (PartUtilities_ComposedPatterns.GeometryAnalysis.GetComposedPatternsFromPathCircum(newMyPathOfCentroids,
                                    listOfParallelPatterns, ref listOfPaths,
                                    ref listOfMatrAdj, ref listOfMyGroupingSurface,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo, SwApplication, ref fileOutput))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfCentroids);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));
                                }
                            }     

                        }
                        else //lunghezza path <n-1
                        {
                            PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints, listOfMBPoints,
                                ref listOfPenultimate, ref listOfLast, Path);
                        }

                    }
                }
            }


            else  // cioè listOfMBPoints.Contains(SecondPointInd) || ListOfSimplePoints.Contains(SecondPointInd)
            {
                List<int> BranchesSecond = matrAdjToSee.matr.GetRow(secondPointInd).Find(entry => entry == 1).ToList(); 
                //trasforma in lista la selezione sull'array (SecondPointInd,:) 

                BranchesSecond.Remove(startPointInd); //levo il punto di partenza (che è sicuramente un branch) per non tornare indietro
                foreach (int branch2 in BranchesSecond)
                {
                    var newMyPathOfCentroids = new MyPathOfPoints();

                    // if (StartPointInd, SecondPointInd, branch2 don't belong to an existing path in ListOfPaths )
                    if (listOfPaths.FindIndex(pathObject => (pathObject.path.Contains(startPointInd) &&
                        pathObject.path.Contains(secondPointInd) && pathObject.path.Contains(branch2))) == -1)
                    {
                        List<int> Path = new List<int>();
                        var pathCurve = new MyPathGeometricObject();

                        if (listCentroid[branch2].Lieonline(FunctionsLC.LinePassingThrough(
                            listCentroid[startPointInd], listCentroid[secondPointInd])))
                        {
                            Path = PathCreation_Part.Functions.ThreePointsGivenPathsLine(matrAdjToSee, listCentroid,
                                listOfExtremePoints, startPointInd, secondPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                        }
                        else
                        {
                            Path = PathCreation_Part.Functions.ThreePointsGivenPathsCircum(matrAdjToSee, listCentroid,
                                listOfExtremePoints, startPointInd, secondPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                        }

                        if (toleranceOk == false)
                        {
                            return;
                        }


                        //If I have found a complete circumference I delete the repeated centroid index:
                        if (Path[0] == Path[Path.Count - 1])
                        {
                            Path.RemoveAt(Path.Count - 1);
                        }
                        newMyPathOfCentroids.path = Path;
                        newMyPathOfCentroids.pathGeometricObject = pathCurve;

                        listOfPaths.Add(newMyPathOfCentroids);

                        fileOutput.AppendLine("Aggiunto alla lista nuovo path: ");
                        fileOutput.AppendLine("  -numero di baricentri:" + newMyPathOfCentroids.path.Count);
                        if (pathCurve.GetType() == typeof(MyLine))
                        {
                            fileOutput.AppendLine("  -tipo di path: retta");
                        }
                        else
                        {
                            fileOutput.AppendLine("  -tipo di path: circonferenza");
                        }
         
                        //If a path of length n or n-1 is found, I move up the geometrical verify to see
                        //if the process can be stopped at this point (sufficiently satisfying result on this GS)
                        int m = Path.Count;
                        if (m == n || m == n - 1)
                        {
                            fileOutput.AppendLine("\n Trovato path che potrebbe dare un Longest Pattern: VERIFICO");

                            
                            // Removing of the path from the list:
                            listOfPaths.RemoveAt(listOfPaths.Count - 1);

                            if (newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyLine))
                            {
                                if (PartUtilities_ComposedPatterns.GeometryAnalysis.GetComposedPatternsFromPathLine(newMyPathOfCentroids,
                                    listOfParallelPatterns, ref listOfPaths,
                                    ref listOfMatrAdj, ref listOfMyGroupingSurface,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfCentroids);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));
                                }
                            }
                            else //newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyCircumForPath)
                            {
                                if (PartUtilities_ComposedPatterns.GeometryAnalysis.GetComposedPatternsFromPathCircum(newMyPathOfCentroids,
                                    listOfParallelPatterns, ref listOfPaths,
                                    ref listOfMatrAdj, ref listOfMyGroupingSurface,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo, SwApplication, ref fileOutput))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfCentroids);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));
                                }
                            }     
                        }
                        else
                        {
                            PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints, listOfMBPoints,
                                ref listOfPenultimate, ref listOfLast, Path);
                        }
                    }
                }
            }
        } //fine TwoPointsGivenPaths
    }
}