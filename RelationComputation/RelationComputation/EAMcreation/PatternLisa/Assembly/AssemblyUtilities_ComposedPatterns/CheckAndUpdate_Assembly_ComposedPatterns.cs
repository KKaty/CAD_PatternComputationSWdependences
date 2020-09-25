using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities_ComposedPatterns
{
    public partial class GeometryAnalysis
    {

        public static void CheckAndUpdate_Assembly_ComposedPatterns(MyComposedPatternOfComponents newComposedPattern, 
            ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyPatternOfComponents> listOfParallelPatterns, ref List<MyMatrAdj> listOfMatrAdj,            
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo)
        {
  
            var lengthOfComposedPattern = newComposedPattern.ListOfMyPatternOfComponents.Count;

            // if lengthOfComposedPattern = 2, I add the newComposedPattern only if there is not another pattern in 
            //listOfOutputComposedPatternTwo containing one of the two patterns in the newComposedPattern.
            if (lengthOfComposedPattern == 2)
            {
                int i = 0;
                var addOrNot = true;
                while (addOrNot == true && i < 2)
                {
                    var currentPattern = newComposedPattern.ListOfMyPatternOfComponents[i];
                    var indOfFound =
                        listOfOutputComposedPatternTwo.FindIndex(
                            composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                                pattern => pattern.idMyPattern == currentPattern.idMyPattern) != -1);
                    if (indOfFound != -1)
                    {
                        addOrNot = false;
                    }
                    i++;
                }

                if (addOrNot == true)
                {
                    listOfOutputComposedPatternTwo.Add(newComposedPattern);
                }

            }
            // if lengthOfComposedPattern > 2, I add the newComposedPattern and I update the other data
            // (aiming not to find composed pattern containing pattern already set in this newComposedPattern)
            else
            {
                listOfOutputComposedPattern.Add(newComposedPattern);
                UpdateOtherData_Assembly_ComposedPatterns(newComposedPattern, ref listOfPathOfCentroids,
                    listOfParallelPatterns, ref listOfMatrAdj, ref listOfOutputComposedPatternTwo);
            }
        }



        //Update data in case on newPattern of length > 2
        public static void UpdateOtherData_Assembly_ComposedPatterns(MyComposedPatternOfComponents newComposedPattern, 
            ref List<MyPathOfPoints> listOfPathOfCentroids, List<MyPatternOfComponents> listOfParallelPatterns,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            foreach (var pattern in newComposedPattern.ListOfMyPatternOfComponents)
            {
                var indOfThisPattern = listOfParallelPatterns.IndexOf(pattern);

                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyPathOfCentroids(ref listOfPathOfCentroids, indOfThisPattern, nameFile);
                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisPattern, nameFile);
                UpdateListOfPatternTwo_Assembly_ComposedPatterns(pattern, ref listOfOutputComposedPatternTwo, indOfThisPattern);
            }
        }

        //Update list of MyComposedPattern geometrically verified of length = 2: delete the ones containing
        //a pattern of the newComposedPattern
        public static void UpdateListOfPatternTwo_Assembly_ComposedPatterns(MyPatternOfComponents pattern, 
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, int indOfThisPattern)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            var indOfFound =
                listOfOutputComposedPatternTwo.FindIndex(
                    composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var found = listOfOutputComposedPatternTwo.Find(
                    composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
               
                listOfOutputComposedPatternTwo.Remove(found);
            }
        }            
    }
}
