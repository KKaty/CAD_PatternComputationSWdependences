using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part_ComposedPatterns
{
    public partial class Functions
    {
        //This function returns the list of paths obtained given the Start point.
        public static void OnePointGivenPaths_ComposedPatterns(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            List<MyPattern> listOfParallelPatterns, List<MyVertex> listCentroid, List<int> listOfExtremePoints, ref List<int> listOfSimplePoints_Copy,
            List<int> listOfMBPoints, ref bool longestPattern, ref List<MyPathOfPoints> listOfPaths, 
            ref List<int> listOfPenultimate, ref List<int> listOfLast, ref StringBuilder fileOutput,
            ref bool toleranceOk, ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface,
            ref List<MyComposedPattern> listOfOutputComposedPattern, ref List<MyComposedPattern> listOfOutputComposedPatternTwo,
            ref List<int> listOfIndicesOfLongestPath, SldWorks SwApplication)
        {
            List<int> BranchesFirst = matrAdjToSee.matr.GetRow(startPointInd).Find(entry => entry == 1).ToList();
            //List<int> BranchesFirst = nInd.FindAll(ind => MatrAdjToSee.matr[StartPointInd, ind] == 1);

            foreach (int branch1 in BranchesFirst)
            {
                fileOutput.AppendLine("\n branch di StartPoint " + startPointInd + ": " + branch1);
                TwoPointsGivenPaths_ComposedPatterns(matrAdjToSee, n, startPointInd, branch1, listOfParallelPatterns, listCentroid, listOfExtremePoints,
                    ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths, ref listOfPenultimate,
                    ref listOfLast, ref fileOutput, ref toleranceOk, ref listOfMatrAdj, ref listOfMyGroupingSurface,
                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo,
                    ref listOfIndicesOfLongestPath, SwApplication);

                if (toleranceOk == false || longestPattern)
                {
                    return;
                }
            }
        }
    }
}
