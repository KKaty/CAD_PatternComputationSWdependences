using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.PathCreation_Assembly_ComposedPatterns
{
    public partial class Functions
    {
        //This function returns the list of paths obtained given the Start point.
        public static void OnePointGivenPaths_Assembly_ComposedPatterns(MyMatrAdj matrAdjToSee, int n, int startPointInd,
            List<MyPatternOfComponents> listOfPatternsOfComponents, List<MyVertex> listOfCentroids, List<int> listOfExtremePoints,
            ref List<int> listOfSimplePoints_Copy, List<int> listOfMBPoints, ref bool longestPattern, 
            ref List<MyPathOfPoints> listOfPaths, ref List<int> listOfPenultimate, ref List<int> listOfLast,  
            ref StringBuilder fileOutput, ref bool toleranceOk, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyComposedPatternOfComponents> listOfOutputPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputPatternTwo, ref List<int> listOfIndicesOfLongestPath, 
            ModelDoc2 SwModel, SldWorks SwApplication)
        {
            var branchesFirst = matrAdjToSee.matr.GetRow(startPointInd).Find(entry => entry == 1).ToList();

            foreach (int branch1 in branchesFirst)
            {
                fileOutput.AppendLine("\n branch di StartPoint " + startPointInd + ": " + branch1);
                TwoPointsGivenPaths_Assembly_ComposedPatterns(matrAdjToSee, n, startPointInd, branch1, listOfPatternsOfComponents, listOfCentroids, 
                    listOfExtremePoints, ref listOfSimplePoints_Copy, listOfMBPoints, ref longestPattern, ref listOfPaths,
                    ref listOfPenultimate, ref listOfLast, ref fileOutput, ref toleranceOk, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, ref listOfIndicesOfLongestPath, SwModel, SwApplication);
               

                if (toleranceOk == false)
                {
                    return;
                }

                if (longestPattern == true)
                {
                    return;
                }
            }
        }
    }
}
