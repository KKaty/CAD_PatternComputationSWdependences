using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        //Given a MyMatrAdj, this function creates a list of paths of length >=3 (line or circumference).
        //Exception: when it finds a path of length n or n-1 (n = number of centroids on the current MyGroupingSurface)
        //.          the function early recalls the geometric check of MyRepeatedEntities corresponding to the centroids.

        // INPUT: a MyMatrAdj and a list of MyRepeatedEntity (on a MyGroupingSurface)
        // OUTPUT: it returns TRUE if it finds a pattern of length = n or length = n -1, FALSE otherwise 
        //         it returns a list of detected MyPathOfPoints (still to verify the geometry)
        //         a bool onlyShortPaths which is TRUE if there are not paths of length > 2, FALSE otherwise
        //         it updates the lists of final pattern (already geometrically verified), the list of MyMatrAdj
        //         of the current MyGroupingSurface, the list of MyGroupingSurface.
        //There is also a Tolerance check: if it is not satisfied it stops the procedure.


        public static bool FindPaths(MyMatrAdj matrAdjToSee, List<MyRepeatedEntity> listOfREOnThisSurface,
            ref StringBuilder fileOutput, out List<MyPathOfPoints> listOfPathOfCentroids, out bool onlyShortPaths,
            ref bool toleranceOk, ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurface> listOfMyGroupingSurface,
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPattern> listOfOutputPattern, ref List<MyPattern> listOfOutputPatternTwo)
        {
            int n = matrAdjToSee.matr.GetLength(0);
            bool longestPattern = false;
            var listOfIndicesOfLongestPath = new List<int>();
            //to keep in mind the index of the LongestPath if it is found during the process: I can't delete it from the list
            //immediately because I need it to remember the points I have already passed through.
            //Then at the end I delete this path from the list because it is geometrically verified in the process
            //(and if I have to delete it, it means that is not a real geometric pattern).
            var listOfPaths = new List<MyPathOfPoints>();

            var listCentroid = listOfREOnThisSurface.Select(re => re.centroid).ToList();

            List<int> listOfExtremePoints = new List<int>(); //list of Extreme points
            List<int> listOfSimplePoints = new List<int>(); //list of Simple points
            List<int> listOfMBPoints = new List<int>(); //list of Multibranch points

            // We classify the points involved in the current adjacency matrix:
            ClassifyComponents(matrAdjToSee, n, ref listOfExtremePoints, ref listOfSimplePoints, ref listOfMBPoints);

            //OSSERVAZIONE: perché non considero i punti simple come particolari punti MB? 
            //Perché nel caso di pochi punti e la maggior parte di loro appartenenti ad un path (es:crf quasi pulita) 
            //mi andrebbe a prendere come Start point anche i punti simple interni al path e farlo mi darebbe di nuovo
            //lo stesso path, ok che individua che tale path esiste già ma fa perdere tempo.

            //Work copies of the point lists:
            List<int> listOfSimplePoints_Copy = new List<int>(listOfSimplePoints);
            List<int> listOfMBPoints_Copy = new List<int>(listOfMBPoints);

            List<int> listOfPenultimate = new List<int>();
            List<int> listOfLast = new List<int>();

            #region Show ListOfExtremePoints, ListOfSimplePoints, ListOfMBPoints information

            //fileOutput.AppendLine("\n Lista dei baricentri Extreme:");
            //foreach (int baricentro in ListOfExtremePoints)
            //{
            //    fileOutput.AppendLine("\n baricentro " + ListOfExtremePoints.IndexOf(baricentro) + ":" + baricentro);
            //}

            //fileOutput.AppendLine("\n Lista dei baricentri Simple:");
            //foreach (int baricentro in ListOfSimplePoints)
            //{
            //    fileOutput.AppendLine("\n baricentro " + ListOfSimplePoints.IndexOf(baricentro) + ":" + baricentro);
            //}

            //fileOutput.AppendLine("\n Lista dei baricentri MB:");
            //foreach (int baricentro in ListOfMBPoints)
            //{
            //    fileOutput.AppendLine("\n baricentro " + ListOfMBPoints.IndexOf(baricentro) + ":" + baricentro);
            //}
            //Console.ReadLine();

            #endregion


            while (listOfMBPoints_Copy.Count > 0)
            {
                int startPointInd = listOfMBPoints_Copy[0];
                fileOutput.AppendLine("\n StartPoint MB:" + startPointInd);
                //Add the list of paths created from this MB start point to ListOfPaths
                OnePointGivenPaths(matrAdjToSee, n, startPointInd, listOfREOnThisSurface,
                    listCentroid, listOfExtremePoints,
                    ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths,
                    ref listOfPenultimate, ref listOfLast, ref fileOutput, ref toleranceOk,
                    ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath);
                if (toleranceOk == false)
                {
                    fileOutput.AppendLine("\n \n WARNING: the program has been interrupted.");
                    fileOutput.AppendLine("The tolerance level is too rough: put a lower tolerance.");
                    listOfPathOfCentroids = null;
                    onlyShortPaths = false;
                    return false;
                }
                if (longestPattern == true)
                {
                    fileOutput.AppendLine("\n LongestPattern = true");
                    listOfPathOfCentroids = listOfPaths;
                    onlyShortPaths = false;
                    return true;
                }
                listOfMBPoints_Copy.Remove(startPointInd);
            }

            //At this point we exit from WHILE because ListOfMBPoints_copy.Count == 0:

            //now the MB points that could be StartPoint are finished or they have never existed
            if (listOfMBPoints.Count == 0 && listOfSimplePoints.Count > 0)
                // i.e. MB points have never existed (never entered the WHILE cycle)
            {
                // and there exists at connected component of at least 3 points (-> there esists a simple point)
                //I must start the "expansion" from a simple point!
                fileOutput.AppendLine("Non esistono MB points.");
                fileOutput.AppendLine("");

                var startPointInd = listOfSimplePoints[0]; //I take the first Simple point of the list
                OnePointGivenPaths(matrAdjToSee, n, startPointInd, listOfREOnThisSurface,
                    listCentroid, listOfExtremePoints,
                    ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths,
                    ref listOfPenultimate, ref listOfLast, ref fileOutput, ref toleranceOk,
                    ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath);
                if (toleranceOk == false)
                {
                    fileOutput.AppendLine("\n \n WARNING: the program has been interrupted.");
                    fileOutput.AppendLine("The tolerance level is too rough: put a lower tolerance.");
                    listOfPathOfCentroids = null;
                    onlyShortPaths = false;
                    return false;
                }
                if (longestPattern == true)
                {
                    fileOutput.AppendLine("\n LongestPattern = true");
                    listOfPathOfCentroids = listOfPaths;
                    onlyShortPaths = false;
                    return true;
                }
            }
            else
            //here there are 3 cases: T+F I have to exit because I only have connected components of 2 centroids;
            //F+T and F+F I am sure I have at least a path created (penultimate points set exists), there is nothing to do in this case.
            {
                if (listOfMBPoints.Count == 0 && listOfSimplePoints.Count == 0) //T+F
                {
                    listOfPathOfCentroids = null;
                    onlyShortPaths = true;
                    return false;
                }
            }
            fileOutput.AppendLine("");
            fileOutput.AppendLine("Lista dei penultimi punti alla fine dei MB come StartPoint:");
            foreach (int ind in listOfPenultimate)
            {
                fileOutput.AppendLine("* " + ind);
            }

            //At this point at least one path has been created. The list of penultimate point is surely not void.

            //I remove all Simple points I already passed through up to now.
            listOfSimplePoints_Copy.RemoveAll(
                point => listOfPaths.FindIndex(pathObject => pathObject.path.Contains(point)) != -1);

            fileOutput.AppendLine("");
            fileOutput.AppendLine("Lista Simple Point per cui non si è ancora passati:");
            foreach (int ind in listOfSimplePoints_Copy)
            {
                fileOutput.AppendLine("* " + ind);
            }

            // Now, if there is a penultimate point available in the list I use it as StartPoint
            // else I use a Simple Point not belonging to any path (the point is in ListOfSimplePoints_copy)

            int NumOfPenultimate = listOfPenultimate.Count;
            fileOutput.AppendLine("");
            fileOutput.AppendLine("Numero di penultimi punti: " + NumOfPenultimate);
            int NumOfSimple = listOfSimplePoints_Copy.Count;
            fileOutput.AppendLine("");
            fileOutput.AppendLine("Numero di Simple punti in cui non si è ancora passati: " + NumOfSimple);

            while (NumOfPenultimate + NumOfSimple > 0)
            {
                if (NumOfPenultimate > 0)
                {
                    // I look for a path with the first available "penultimate point" as StartPoint and I move towards of the relative "last point"
                    fileOutput.AppendLine(" uso un penultimo punto: " + listOfPenultimate[0] + " verso " +
                                            listOfLast[0]);
                    int StartPointInd = listOfPenultimate[0];
                    int SecondPointInd = listOfLast[0];
                    TwoPointsGivenPaths(matrAdjToSee, n, StartPointInd, SecondPointInd, listOfREOnThisSurface,
                        listCentroid, listOfExtremePoints, ref listOfSimplePoints_Copy, listOfMBPoints,
                        ref longestPattern, ref listOfPaths, ref listOfPenultimate, ref listOfLast, ref fileOutput,
                        ref toleranceOk, ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath);

                    listOfPenultimate.Remove(StartPointInd);
                    listOfLast.Remove(SecondPointInd);

                }
                else // => NumOfSimple > 0
                {
                    fileOutput.AppendLine(" uso un Simple punto: " + listOfSimplePoints_Copy[0]);
                    int StartPointInd = listOfSimplePoints_Copy[0];
                    OnePointGivenPaths(matrAdjToSee, n, StartPointInd, listOfREOnThisSurface,
                        listCentroid, listOfExtremePoints,
                        ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths,
                        ref listOfPenultimate, ref listOfLast, ref fileOutput, ref toleranceOk,
                        ref listOfMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath);
                }

                //Check if tolerance level is ok:
                if (toleranceOk == false)
                {
                    fileOutput.AppendLine("\n \n WARNING: the program has been interrupted.");
                    fileOutput.AppendLine("The tolerance level is too rough: put a lower tolerance.");
                    listOfPathOfCentroids = null;
                    onlyShortPaths = false;
                    return false;
                }
                //Check if longestPattern == true:
                if (longestPattern == true)
                {
                    fileOutput.AppendLine("\n LongestPattern = true");
                    listOfPathOfCentroids = listOfPaths;
                    onlyShortPaths = false;
                    return true;
                }

                //I update the list of Simple points I have not passed through yet:
                listOfSimplePoints_Copy.RemoveAll(point => listOfPaths[listOfPaths.Count - 1].path.Contains(point));
                NumOfSimple = listOfSimplePoints_Copy.Count;
                NumOfPenultimate = listOfPenultimate.Count;
            }


            //Last searching: I go through the branches of MB satisfying some characteristics..
            AddPathsFromNewCheckOfMB(matrAdjToSee, listCentroid, ref listOfPaths, listOfExtremePoints,
                listOfMBPoints, ref fileOutput, ref toleranceOk);

            if (toleranceOk == false)
            {
                fileOutput.AppendLine("\n \n WARNING: the program has been interrupted.");
                fileOutput.AppendLine("The tolerance level is too rough: put a lower tolerance.");
                listOfPathOfCentroids = null;
                onlyShortPaths = false;
                return false;
            }

            //I remove the longestPath, which was not geometrically verified but its path was kept in memory
            //not to find it again during the process (in fact tht process, whenever it finds a new seed path
            //of 3 centroids, verifies if a path containing these 3 centroids already exists):
            //listOfPaths.RemoveAt(listOfIndicesOfLongestPath);
            //            fileOutput.AppendLine("Numero di path prima della cancellazione dei longestPattern= " + listOfPaths.Count);
            listOfPaths.RemoveAll(path => listOfIndicesOfLongestPath.Contains(listOfPaths.IndexOf(path)));
            //            fileOutput.AppendLine("Numero di path dopo della cancellazione dei longestPattern= " + listOfPaths.Count);

            // I order the list of MyPathOfPoints by decreasing order respect to the length of the path,
            // if length is the same, first line and then circumference.
            listOfPaths =
                listOfPaths.OrderByDescending(x => x.path.Count)
                    .ThenBy(y => y.pathGeometricObject.GetType() == typeof (MyLine) ? 0 : 1)
                    .ToList();
            listOfPathOfCentroids = listOfPaths;
            onlyShortPaths = false;
            return false;
            
        }
    


        //This function classifies the points involved in a given MyMatrAdj, subdividing them in the respective list
        public static void ClassifyComponents(MyMatrAdj matrAdjToSee, int n, ref List<int> listOfExtremePoints, ref List<int> listOfSimplePoints,
            ref List<int> listOfMBPoints)
        {
            for (var i = 0; i < n; i++)
            {
                var tot = 0;
                for (var j = 0; j < n; j++)
                {
                    tot += matrAdjToSee.matr[i, j];
                }

                if (tot == 1)
                {
                    listOfExtremePoints.Add(i);
                }
                if (tot == 2)
                {
                    listOfSimplePoints.Add(i);
                }
                if (tot > 2)
                {
                    listOfMBPoints.Add(i);
                }
            }
            
        }
        
    }
}
