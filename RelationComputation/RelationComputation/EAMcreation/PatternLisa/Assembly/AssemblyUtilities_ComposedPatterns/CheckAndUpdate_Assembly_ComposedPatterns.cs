using System.Collections.Generic;
using AssemblyRetrieval.Debug;
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
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("             UPDATE STEP", nameFile);
            KLdebug.Print("CURRENT SITUATION:", nameFile);
            KLdebug.Print("listOfOutputComposedPattern.Count = " + listOfOutputComposedPattern.Count, nameFile);
            KLdebug.Print("listOfOutputComposedPatternTwo.Count = " + listOfOutputComposedPatternTwo.Count, nameFile);
            KLdebug.Print(" ", nameFile);

            var lengthOfComposedPattern = newComposedPattern.ListOfMyPatternOfComponents.Count;

            // if lengthOfComposedPattern = 2, I add the newComposedPattern only if there is not another pattern in 
            //listOfOutputComposedPatternTwo containing one of the two patterns in the newComposedPattern.
            if (lengthOfComposedPattern == 2)
            {
                KLdebug.Print("Entrata nel caso lengthOfPattern = " + lengthOfComposedPattern, nameFile);

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
                    KLdebug.Print("AGGIUNTO! Non ho trovato altri Pattern da 2 con intersezione non nulla con il corrente.", nameFile);
                }
                else
                {
                    KLdebug.Print("NON AGGIUNTO! Trovato altro Pattern da 2 che interseca questo.", nameFile);
                }

            }
            // if lengthOfComposedPattern > 2, I add the newComposedPattern and I update the other data
            // (aiming not to find composed pattern containing pattern already set in this newComposedPattern)
            else
            {
                KLdebug.Print("Entrata nel lengthOfPattern = " + lengthOfComposedPattern, nameFile);

                listOfOutputComposedPattern.Add(newComposedPattern);
                KLdebug.Print("AGGIUNTO (senza verifiche..)", nameFile);

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
            KLdebug.Print("--> AGGIORNAMENTO DI TUTTI GLI ALTRI DATI:", nameFile);

            foreach (var pattern in newComposedPattern.ListOfMyPatternOfComponents)
            {
                var indOfThisPattern = listOfParallelPatterns.IndexOf(pattern);
                KLdebug.Print("UPDATE per PATTERN (del composed pattern) n° " + indOfThisPattern, nameFile);

                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyPathOfCentroids(ref listOfPathOfCentroids, indOfThisPattern, nameFile);
                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisPattern, nameFile);
                UpdateListOfPatternTwo_Assembly_ComposedPatterns(pattern, ref listOfOutputComposedPatternTwo, indOfThisPattern);

                KLdebug.Print(" ", nameFile);

            }
        }

        //Update list of MyComposedPattern geometrically verified of length = 2: delete the ones containing
        //a pattern of the newComposedPattern
        public static void UpdateListOfPatternTwo_Assembly_ComposedPatterns(MyPatternOfComponents pattern, 
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, int indOfThisPattern)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            KLdebug.Print("     ---> UpdateListOfPatternTwo", nameFile);

            var indOfFound =
                listOfOutputComposedPatternTwo.FindIndex(
                    composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var found = listOfOutputComposedPatternTwo.Find(
                    composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
                KLdebug.Print(" Trovato composedPattern da 2 contenente il pattern corrente (posiz :" + 
                    indOfThisPattern + "):", nameFile);
                KLdebug.Print(" Lunghezza (deve essere 2): " + found.ListOfMyPatternOfComponents.Count, nameFile);
                KLdebug.Print(" Posizione nella lista: " + listOfOutputComposedPatternTwo.IndexOf(found), nameFile);
                KLdebug.Print(
                    " Centroid 1^ pattern nel composedPattern: (" + found.ListOfMyPatternOfComponents[0].patternCentroid.x + "," +
                    found.ListOfMyPatternOfComponents[0].patternCentroid.y + "," + found.ListOfMyPatternOfComponents[0].patternCentroid.z + ")",
                    nameFile);
                KLdebug.Print(
                   " Centroid 2^ pattern nel composedPattern: (" + found.ListOfMyPatternOfComponents[1].patternCentroid.x + "," +
                   found.ListOfMyPatternOfComponents[1].patternCentroid.y + "," + found.ListOfMyPatternOfComponents[1].patternCentroid.z + ")",
                   nameFile);
                KLdebug.Print("(Uno di questi due centroid deve essere quello del current pattern)", nameFile);

                listOfOutputComposedPatternTwo.Remove(found);
                KLdebug.Print(" RIMOSSO dalla lista listOfOutputComposedPatternTwo!", nameFile);
            }
            KLdebug.Print(" ", nameFile);
        }            
    }
}
