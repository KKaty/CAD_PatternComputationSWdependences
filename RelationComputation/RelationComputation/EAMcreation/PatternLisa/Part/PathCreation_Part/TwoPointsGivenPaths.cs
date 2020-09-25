using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        // This function updates the list of paths adding the paths raising from the given 1st and 2nd seed set points.
        public static void TwoPointsGivenPaths(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            int secondPointInd, List<MyRepeatedEntity> listOfREOnThisSurface, List<MyVertex> listCentroid,
            List<int> listOfExtremePoints, ref List<int> listOfSimplePoints_Copy, List<int> listOfMBPoints,
            ref bool longestPattern, ref List<MyPathOfPoints> listOfPaths, ref List<int> listOfPenultimate,
            ref List<int> listOfLast, ref StringBuilder fileOutput, ref bool toleranceOk,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurface> listOfMyGroupingSurface,
            List<MyGroupingSurface> listOfInitialGroupingSurface,
            ref List<MyPattern> listOfOutputPattern, ref List<MyPattern> listOfOutputPatternTwo, 
            ref List<int> listOfIndicesOfLongestPath)
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
                            Path = ThreePointsGivenPathsLine(matrAdjToSee, listCentroid,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
                        }
                        else
                        {
                            Path = ThreePointsGivenPathsCircum(matrAdjToSee, listCentroid,
                                listOfExtremePoints, secondPointInd, startPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                            // Notice that expansion will procede only in one direction
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
                            fileOutput.AppendLine("");
                        }
                        else
                        {
                            fileOutput.AppendLine("  -tipo di path: criconferenza");
                            fileOutput.AppendLine("");
                        }

                        //If a path of length n or n-1 is found, I move up the geometrical verify to see
                        //if the process can be stopped at this point (sufficiently satisfying result on this GS)
                        int m = Path.Count;
                        if (m == n || m == n - 1)
                        {
                            fileOutput.AppendLine("\n Trovato path che potrebbe dare un Longest Pattern: VERIFICO");

                            // Removing of the path from the list:
                            //this is done because if the pattern is verified the updating phase (in particular 
                            //referred to the part of list of paths updating) will consider all the paths existing
                            //in the list of paths (so also the current long one) and the updating would be
                            //long and useless (we would try to compare the current path to itself).
                            listOfPaths.RemoveAt(listOfPaths.Count - 1);

                            if (PartUtilities.GeometryAnalysis.GetPatternsFromPath(newMyPathOfCentroids, listOfREOnThisSurface,
                                ref listOfPaths, ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                                ref listOfOutputPattern, ref listOfOutputPatternTwo))
                            {
                                longestPattern = true;
                                return;
                            }
                            else
                            {
                                listOfPaths.Add(newMyPathOfCentroids);
                                // Updating of the list of Simple points not yet passed through:
                                var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                    listOfSimplePoints_Copy.RemoveAll(point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                    listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);
                                
                                listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));

                            }
                        }
                        else
                        {
                            UpdateListsOfPenultimateAndLast(listOfExtremePoints, 
                                listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);
                        }

                    }
                }
            }


            else  // cioè listOfMBPoints.Contains(SecondPointInd) || ListOfSimplePoints.Contains(SecondPointInd)
            {
                //trasforma in lista la selezione sull'array (SecondPointInd,:) 
                List<int> BranchesSecond = matrAdjToSee.matr.GetRow(secondPointInd).Find(entry => entry == 1).ToList();

                //levo il punto di partenza (che è sicuramente un branch) per non tornare indietro
                BranchesSecond.Remove(startPointInd); 
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
                            Path = ThreePointsGivenPathsLine(matrAdjToSee, listCentroid,
                                listOfExtremePoints, startPointInd, secondPointInd, branch2,
                                ref fileOutput, ref toleranceOk, out pathCurve);
                        }
                        else
                        {
                            Path = ThreePointsGivenPathsCircum(matrAdjToSee, listCentroid,
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

                            if (PartUtilities.GeometryAnalysis.GetPatternsFromPath(newMyPathOfCentroids, listOfREOnThisSurface,
                                ref listOfPaths, ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                                ref listOfOutputPattern, ref listOfOutputPatternTwo))
                            {
                                longestPattern = true;
                                return;
                            }
                            else
                            {
                                listOfPaths.Add(newMyPathOfCentroids);

                                // Updating of the list of Simple points not yet passed through:
                                var ListOfPaths_copy = new List<MyPathOfPoints>(listOfPaths);
                                listOfSimplePoints_Copy.RemoveAll(point => ListOfPaths_copy[ListOfPaths_copy.Count - 1].path.Contains(point));

                                UpdateListsOfPenultimateAndLast(listOfExtremePoints,
                                    listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);

                                listOfIndicesOfLongestPath.Add(listOfPaths.IndexOf(newMyPathOfCentroids));
                            }
                        }
                        else
                        {
                            UpdateListsOfPenultimateAndLast(listOfExtremePoints, 
                                listOfMBPoints, ref listOfPenultimate, ref listOfLast, Path);
                        }
                    }
                }
            }
        }

        public static void UpdateListsOfPenultimateAndLast(List<int> listOfExtremePoints, 
            List<int> listOfMBPoints, ref List<int> listOfPenultimate, ref List<int> listOfLast, List<int> Path)
        {
            try
            {

            int Penultimate1 = Path[1];                     // 1 penultimate point of the Path
            int Penultimate2 = Path[Path.Count - 2];        // 2 penultimate point of the Path
            int LastOfPenultimate1 = Path[0];               // 1 last point of the Path
            int LastOfPenultimate2 = Path[Path.Count - 1];  // 2 last point of the Path

            //Un penultimo punto non può essere il penultimo punto di più path, perché:
            //-se avessero in comune il penultimo e non i comune l'ultimo, il penultimo sarebbe un MB
            //-se avessero in comune il penultimo e anche l'ultimo, o i due path si diramerebbero e il penultimo sarebbe MB
            //  oppure coinciderebbero
            //Inoltre l'ultimo punto non deve essere un estremo, quindi un simple
            if (!(listOfMBPoints.Contains(Penultimate1)) && !(listOfExtremePoints.Contains(LastOfPenultimate1)))
            {
                listOfPenultimate.Add(Penultimate1);
                listOfLast.Add(LastOfPenultimate1);
            }
            if (!(listOfMBPoints.Contains(Penultimate2)) && !(listOfExtremePoints.Contains(LastOfPenultimate2)))
            {
                listOfPenultimate.Add(Penultimate2);
                listOfLast.Add(LastOfPenultimate2);
            }

            }
            catch (Exception exception)
            {
            }
        }
    }
}
