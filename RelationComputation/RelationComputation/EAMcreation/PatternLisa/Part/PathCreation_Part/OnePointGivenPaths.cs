using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        //This function returns the list of paths obtained given the Start point.
        public static void OnePointGivenPaths(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            List<MyRepeatedEntity> listOfReOnThisSurface,
            List<MyVertex> listCentroid, List<int> listOfExtremePoints, ref List<int> listOfSimplePoints_Copy,
            List<int> listOfMBPoints, ref bool longestPattern, ref List<MyPathOfPoints> listOfPaths, 
            ref List<int> listOfPenultimate, ref List<int> listOfLast, ref StringBuilder fileOutput,
            ref bool toleranceOk, ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurface> listOfMyGroupingSurface,
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPattern> listOfOutputPattern, ref List<MyPattern> listOfOutputPatternTwo,
            ref List<int> listOfIndicesOfLongestPath)       
        {

            List<int> BranchesFirst = matrAdjToSee.matr.GetRow(startPointInd).Find(entry => entry == 1).ToList();
            //List<int> BranchesFirst = nInd.FindAll(ind => MatrAdjToSee.matr[StartPointInd, ind] == 1);

            foreach (int branch1 in BranchesFirst)
            {
                fileOutput.AppendLine("\n branch di StartPoint " + startPointInd + ": " + branch1);
                //List<List<int>> ListOfPathsThisBranch = 
                TwoPointsGivenPaths(matrAdjToSee, n, startPointInd, branch1, listOfReOnThisSurface, listCentroid, listOfExtremePoints, 
                    ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths, ref listOfPenultimate,
                    ref listOfLast, ref fileOutput, ref toleranceOk, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                    listOfInitialGroupingSurface, ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath);

                //ListOfPathsOnePoint.AddRange(ListOfPathsThisBranch);

                if (toleranceOk == false)
                {
                    return;
                }

                if (longestPattern == true)
                {
                    return;
                }
            }
            
            //return ListOfPathsOnePoint;

        }//fine OnePointGivenPaths
    }
}
