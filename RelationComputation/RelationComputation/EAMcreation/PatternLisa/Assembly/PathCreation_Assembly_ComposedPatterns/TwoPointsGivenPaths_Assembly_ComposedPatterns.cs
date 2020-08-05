using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.PathCreation_Assembly_ComposedPatterns
{
    public partial class Functions
    {
        // This function updates the list of paths adding the paths raising from 
        // the given 1st and 2nd seed set points (FOR ASSEMBLY)
        public static void TwoPointsGivenPaths_Assembly_ComposedPatterns(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            int secondPointInd, List<MyPatternOfComponents> listOfPatternsOfComponents, List<MyVertex> listOfCentroids,
            List<int> listOfExtremePoints, ref List<int> listOfSimplePoints_Copy, List<int> listOfMBPoints,
            ref bool longestPattern, ref List<MyPathOfPoints> listOfPaths, ref List<int> listOfPenultimate,
            ref List<int> listOfLast, ref StringBuilder fileOutput, ref bool toleranceOk,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, ref List<int> listOfIndicesOfLongestPath, 
            ModelDoc2 SwModel, SldWorks SwApplication)
        {
            if (listOfExtremePoints.Contains(secondPointInd))
            //If the branch is an ExtremePoint, I treat it in a special way as I have not 
            //the path equation yet. So I guide the 3rd point search.
            {
                //I come back, so I look what are the branches of the StartPoint
                List<int> branchesSecond = matrAdjToSee.matr.GetRow(startPointInd).Find(entry => entry == 1).ToList();
                branchesSecond.Remove(secondPointInd);
                foreach (var branch2 in branchesSecond)
                {
                    var newMyPathOfPoints = new MyPathOfPoints();

                    //if (SecondPointInd(extreme), StartPointInd(MB), branch2 don't belong to an existing path in ListOfPaths)
                    if (listOfPaths.FindIndex(pathObject => (pathObject.path.Contains(secondPointInd) &&
                        pathObject.path.Contains(startPointInd) && pathObject.path.Contains(branch2))) == -1)
                    {
                        List<int> currentPath;
                        MyPathGeometricObject pathCurve;

                        if (listOfCentroids[branch2].Lieonline(FunctionsLC.LinePassingThrough(listOfCentroids[startPointInd], listOfCentroids[secondPointInd])))
                        {
                            currentPath = Part.PathCreation_Part.Functions.ThreePointsGivenPathsLine(matrAdjToSee, listOfCentroids,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
                        }
                        else
                        {
                            currentPath = Part.PathCreation_Part.Functions.ThreePointsGivenPathsCircum(matrAdjToSee, listOfCentroids,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
                        }

                        if (toleranceOk == false)
                        {
                            return;
                        }
                        //If I have found a complete circumference I delete the repeated centroid index:
                        if (currentPath[0] == currentPath[currentPath.Count - 1])
                        {
                            currentPath.RemoveAt(currentPath.Count - 1);
                        }

                        newMyPathOfPoints.path = currentPath;
                        newMyPathOfPoints.pathGeometricObject = pathCurve;

                        listOfPaths.Add(newMyPathOfPoints);

                        fileOutput.AppendLine("Aggiunto alla lista nuovo path: ");
                        fileOutput.AppendLine("  -numero di baricentri:" + newMyPathOfPoints.path.Count);
                        if (pathCurve.GetType() == typeof(MyLine))
                        {
                            fileOutput.AppendLine("  -tipo di path: retta");
                            fileOutput.AppendLine("");
                        }
                        else
                        {
                            fileOutput.AppendLine("  -tipo di path: criconferenza");
                            fileOutput.AppendLine("");
                        }

                        //If a path of length n or n-1 is found, I move up the geometrical verify to see
                        //if the process can be stopped at this point (sufficiently satisfying result on this GS)
                        int m = currentPath.Count;
                        if (m == n || m == n - 1)
                        {
                            fileOutput.AppendLine("\n Trovato path che potrebbe dare un Longest Pattern: VERIFICO");

                            // Removing of the path from the list:
                            //this is done because if the pattern is verified the updating phase (in particular 
                            //referred to the part of list of paths updating) will consider all the paths existing
                            //in the list of paths (so also the current long one) and the updating would be
                            //long and useless (we would try to compare the current path to itself).
                            listOfPaths.RemoveAt(listOfPaths.Count - 1);

                            if (newMyPathOfPoints.pathGeometricObject.GetType() == typeof(MyLine))
                            {
                                if (AssemblyUtilities_ComposedPatterns.LC_AssemblyTraverse.GetComposedPatternsFromPathLine_Assembly(newMyPathOfPoints,
                                    listOfPatternsOfComponents, ref listOfPaths,
                                    ref listOfMatrAdj,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfPoints);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfPoints));
                                }
                            }
                            else //newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyCircumForPath)
                            {
                                if (AssemblyUtilities_ComposedPatterns.LC_AssemblyTraverse.GetComposedPatternsFromPathCircum_Assembly(newMyPathOfPoints,
                                    listOfPatternsOfComponents, ref listOfPaths,
                                    ref listOfMatrAdj,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo, 
                                    SwApplication, ref fileOutput))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfPoints);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfPoints));
                                }
                            }     
                            
                        }
                        else
                        {
                            Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);
                        }
                    }
                }
            }
            else  // cioè listOfMBPoints.Contains(SecondPointInd) || ListOfSimplePoints.Contains(SecondPointInd)
            {
                List<int> branchesSecond = matrAdjToSee.matr.GetRow(secondPointInd).Find(entry => entry == 1).ToList();
                branchesSecond.Remove(startPointInd); //I remove the startingPoint not to come back in the start direction
                foreach (var branch2 in branchesSecond)
                {
                    var newMyPathOfPoints = new MyPathOfPoints();

                    // if (StartPointInd, SecondPointInd, branch2 don't belong to an existing path in ListOfPaths )
                    if (listOfPaths.FindIndex(pathObject => (pathObject.path.Contains(startPointInd) && pathObject.path.Contains(secondPointInd) && pathObject.path.Contains(branch2))) == -1)
                    {
                        List<int> currentPath;
                        MyPathGeometricObject pathCurve;

                        if (listOfCentroids[branch2].Lieonline(FunctionsLC.LinePassingThrough(listOfCentroids[startPointInd], listOfCentroids[secondPointInd])))
                        {
                            currentPath = Part.PathCreation_Part.Functions.ThreePointsGivenPathsLine(matrAdjToSee, listOfCentroids,
                                listOfExtremePoints, startPointInd, secondPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                        }
                        else
                        {
                            currentPath = Part.PathCreation_Part.Functions.ThreePointsGivenPathsCircum(matrAdjToSee, listOfCentroids,
                                listOfExtremePoints, startPointInd, secondPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                        }

                        if (toleranceOk == false)
                        {
                            return;
                        }
                        //If I have found a complete circumference I delete the repeated centroid index:
                        if (currentPath[0] == currentPath[currentPath.Count - 1])
                        {
                            currentPath.RemoveAt(currentPath.Count - 1);
                        }

                        newMyPathOfPoints.path = currentPath;
                        newMyPathOfPoints.pathGeometricObject = pathCurve;

                        listOfPaths.Add(newMyPathOfPoints);

                        fileOutput.AppendLine("Aggiunto alla lista nuovo path: ");
                        fileOutput.AppendLine("  -numero di baricentri:" + newMyPathOfPoints.path.Count);
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
                        int m = currentPath.Count;
                        if (m == n || m == n - 1)
                        {
                            fileOutput.AppendLine("\n Trovato path che potrebbe dare un Longest Pattern: VERIFICO");
        
                            // Removing of the path from the list:
                            listOfPaths.RemoveAt(listOfPaths.Count - 1);

                            if (newMyPathOfPoints.pathGeometricObject.GetType() == typeof(MyLine))
                            {
                                if (AssemblyUtilities_ComposedPatterns.LC_AssemblyTraverse.GetComposedPatternsFromPathLine_Assembly(newMyPathOfPoints,
                                    listOfPatternsOfComponents, ref listOfPaths,
                                    ref listOfMatrAdj,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfPoints);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfPoints));
                                }
                            }
                            else //newMyPathOfCentroids.pathGeometricObject.GetType() == typeof (MyCircumForPath)
                            {
                                if (AssemblyUtilities_ComposedPatterns.LC_AssemblyTraverse.GetComposedPatternsFromPathCircum_Assembly(newMyPathOfPoints,
                                    listOfPatternsOfComponents, ref listOfPaths,
                                    ref listOfMatrAdj,
                                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo,
                                    SwApplication, ref fileOutput))
                                {
                                    fileOutput.AppendLine("\n è longestPattern verificato!! ");
                                    longestPattern = true;
                                    return;
                                }
                                else
                                {
                                    fileOutput.AppendLine("\n longestPattern NON verificato!! ");
                                    listOfPaths.Add(newMyPathOfPoints);

                                    // Updating of the list of Simple points not yet passed through:
                                    var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(
                                        point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                    Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                        listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);

                                    listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfPoints));
                                }
                            }     

                        }
                        else 
                        {
                            Part.PathCreation_Part.Functions.UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                listOfMBPoints, ref listOfPenultimate, ref listOfLast, currentPath);
                        }
                    }
                }
            }

        }

    }
}
