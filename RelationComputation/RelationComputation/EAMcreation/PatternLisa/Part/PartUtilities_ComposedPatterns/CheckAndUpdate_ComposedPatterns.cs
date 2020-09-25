using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities_ComposedPatterns
{
    public partial class GeometryAnalysis
    {

        public static void CheckAndUpdate_ComposedPatterns(MyComposedPattern newComposedPattern, 
            ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyPattern> listOfParallelPatterns, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface,
            ref List<MyComposedPattern> listOfOutputComposedPattern, 
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo)
        {
            var lengthOfComposedPattern = newComposedPattern.listOfMyPattern.Count;

            // if lengthOfComposedPattern = 2, I add the newComposedPattern only if there is not another pattern in 
            //listOfOutputComposedPatternTwo containing one of the two patterns in the newComposedPattern.
            if (lengthOfComposedPattern == 2)
            {
                int i = 0;
                var addOrNot = true;
                while (addOrNot == true && i < 2)
                {
                    var currentPattern = newComposedPattern.listOfMyPattern[i];
                    var indOfFound =
                        listOfOutputComposedPatternTwo.FindIndex(
                            composedPattern => composedPattern.listOfMyPattern.FindIndex(
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
                
                UpdateOtherData_ComposedPatterns(newComposedPattern, ref listOfPathOfCentroids,
                    listOfParallelPatterns, ref listOfMatrAdj,
                    ref listOfMyGroupingSurface, ref listOfOutputComposedPatternTwo);
            }

        }



        //Update data in case on newPattern of length > 2
        public static void UpdateOtherData_ComposedPatterns(MyComposedPattern newComposedPattern, 
            ref List<MyPathOfPoints> listOfPathOfCentroids, List<MyPattern> listOfParallelPatterns,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface, 
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";

            foreach (var pattern in newComposedPattern.listOfMyPattern)
            {
                var indOfThisPattern = listOfParallelPatterns.IndexOf(pattern);
            
                PartUtilities.GeometryAnalysis.UpdateListOfMyPathOfCentroids(ref listOfPathOfCentroids, indOfThisPattern, nameFile);
                PartUtilities.GeometryAnalysis.UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisPattern, nameFile);
                UpdateListOfMyGroupingSurface_ComposedPatterns(pattern, ref listOfMyGroupingSurface, indOfThisPattern);
                UpdateListOfPatternTwo_ComposedPatterns(pattern, ref listOfOutputComposedPatternTwo, indOfThisPattern);
            }
        }

        //Update list of MyComposedPattern geometrically verified of length = 2: delete the ones containing
        //a pattern of the newComposedPattern
        public static void UpdateListOfPatternTwo_ComposedPatterns(MyPattern pattern, 
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo, int indOfThisPattern)
        {
            var indOfFound =
                listOfOutputComposedPatternTwo.FindIndex(
                    composedPattern => composedPattern.listOfMyPattern.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var found = listOfOutputComposedPatternTwo.Find(
                    composedPattern => composedPattern.listOfMyPattern.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
            
                listOfOutputComposedPatternTwo.Remove(found);
                
            }
        }
        
        //Update the list of MyGroupingSurface, deleting the MyRepeatedEntity already set in the newPattern
        public static void UpdateListOfMyGroupingSurface_ComposedPatterns(MyPattern pattern,
            ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface, int indOfThisPattern)
        {
            var indOfFound =
                listOfMyGroupingSurface.FindIndex(
                    gs => gs.listOfPatternsLine.FindIndex(patternInGS => 
                        patternInGS.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var listOfGroupingSurfaceToUpdate = listOfMyGroupingSurface.FindAll(
                    gs => gs.listOfPatternsLine.FindIndex(patternInGS =>
                        patternInGS.idMyPattern == pattern.idMyPattern) != -1);
            
                foreach (var gs in listOfGroupingSurfaceToUpdate)
                {
                    gs.listOfPatternsLine.Remove(pattern);
                    
                    if (gs.listOfPatternsLine.Count < 2)
                    {
                        listOfMyGroupingSurface.Remove(gs);
                    }
                }
            }
        }
        
    }
}
