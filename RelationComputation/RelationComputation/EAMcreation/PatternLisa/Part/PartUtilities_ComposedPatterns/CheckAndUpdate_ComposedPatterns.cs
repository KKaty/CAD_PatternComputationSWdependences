using System.Collections.Generic;
using AssemblyRetrieval.Debug;
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
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("             UPDATE STEP", nameFile);
            KLdebug.Print("CURRENT SITUATION:", nameFile);
            KLdebug.Print("listOfOutputComposedPattern.Count = " + listOfOutputComposedPattern.Count, nameFile);
            KLdebug.Print("listOfOutputComposedPatternTwo.Count = " + listOfOutputComposedPatternTwo.Count, nameFile);
            KLdebug.Print(" ", nameFile);

            var lengthOfComposedPattern = newComposedPattern.listOfMyPattern.Count;

            // if lengthOfComposedPattern = 2, I add the newComposedPattern only if there is not another pattern in 
            //listOfOutputComposedPatternTwo containing one of the two patterns in the newComposedPattern.
            if (lengthOfComposedPattern == 2)
            {
                KLdebug.Print("Entrata nel caso lengthOfPattern = " + lengthOfComposedPattern, nameFile);

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
            KLdebug.Print("--> AGGIORNAMENTO DI TUTTI GLI ALTRI DATI:", nameFile);

            foreach (var pattern in newComposedPattern.listOfMyPattern)
            {
                var indOfThisPattern = listOfParallelPatterns.IndexOf(pattern);
                KLdebug.Print("UPDATE per PATTERN (del composed pattern) n° " + indOfThisPattern, nameFile);

                PartUtilities.GeometryAnalysis.UpdateListOfMyPathOfCentroids(ref listOfPathOfCentroids, indOfThisPattern, nameFile);
                PartUtilities.GeometryAnalysis.UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisPattern, nameFile);
                UpdateListOfMyGroupingSurface_ComposedPatterns(pattern, ref listOfMyGroupingSurface, indOfThisPattern);
                UpdateListOfPatternTwo_ComposedPatterns(pattern, ref listOfOutputComposedPatternTwo, indOfThisPattern);

                KLdebug.Print(" ", nameFile);

            }
        }

        //Update list of MyComposedPattern geometrically verified of length = 2: delete the ones containing
        //a pattern of the newComposedPattern
        public static void UpdateListOfPatternTwo_ComposedPatterns(MyPattern pattern, 
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo, int indOfThisPattern)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            KLdebug.Print("     ---> UpdateListOfPatternTwo", nameFile);

            var indOfFound =
                listOfOutputComposedPatternTwo.FindIndex(
                    composedPattern => composedPattern.listOfMyPattern.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var found = listOfOutputComposedPatternTwo.Find(
                    composedPattern => composedPattern.listOfMyPattern.FindIndex(
                        patternInComposedPattern => patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1);
                KLdebug.Print(" Trovato composedPattern da 2 contenente il pattern corrente (posiz :" + 
                    indOfThisPattern + "):", nameFile);
                KLdebug.Print(" Lunghezza (deve essere 2): " + found.listOfMyPattern.Count, nameFile);
                KLdebug.Print(" Posizione nella lista: " + listOfOutputComposedPatternTwo.IndexOf(found), nameFile);
                KLdebug.Print(
                    " Centroid 1^ pattern nel composedPattern: (" + found.listOfMyPattern[0].patternCentroid.x + "," +
                    found.listOfMyPattern[0].patternCentroid.y + "," + found.listOfMyPattern[0].patternCentroid.z + ")",
                    nameFile);
                KLdebug.Print(
                   " Centroid 2^ pattern nel composedPattern: (" + found.listOfMyPattern[1].patternCentroid.x + "," +
                   found.listOfMyPattern[1].patternCentroid.y + "," + found.listOfMyPattern[1].patternCentroid.z + ")",
                   nameFile);
                KLdebug.Print("(Uno di questi due centroid deve essere quello del current pattern)", nameFile);

                listOfOutputComposedPatternTwo.Remove(found);
                KLdebug.Print(" RIMOSSO dalla lista listOfOutputComposedPatternTwo!", nameFile);
            }
            KLdebug.Print(" ", nameFile);
        }
        
        //Update the list of MyGroupingSurface, deleting the MyRepeatedEntity already set in the newPattern
        public static void UpdateListOfMyGroupingSurface_ComposedPatterns(MyPattern pattern,
            ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface, int indOfThisPattern)
        {
            const string nameFile = "CheckAndUpdate_ComposedPatterns.txt";
            KLdebug.Print("     ---> UpdateListOfMyGroupingSurface", nameFile);

            var indOfFound =
                listOfMyGroupingSurface.FindIndex(
                    gs => gs.listOfPatternsLine.FindIndex(patternInGS => 
                        patternInGS.idMyPattern == pattern.idMyPattern) != -1);
            if (indOfFound != -1)
            {
                var listOfGroupingSurfaceToUpdate = listOfMyGroupingSurface.FindAll(
                    gs => gs.listOfPatternsLine.FindIndex(patternInGS =>
                        patternInGS.idMyPattern == pattern.idMyPattern) != -1);
                KLdebug.Print(" Trovate GS contenenti il pattern corrente (" + 
                    indOfThisPattern + "^ pattern):", nameFile);

                foreach (var gs in listOfGroupingSurfaceToUpdate)
                {
                    KLdebug.Print(" -aggiorno superficie in posizione: " + listOfMyGroupingSurface.IndexOf(gs), nameFile);
                    KLdebug.Print(" -tipo di superficie: " + gs.groupingSurface.Identity(), nameFile);
                    KLdebug.Print(" -numero di pattern ancora su questa GS (cioè in composedPattern da 2 o in nessun composedPattern ancora): " +
                        gs.listOfPatternsLine.Count, nameFile);

                    gs.listOfPatternsLine.Remove(pattern);
                    KLdebug.Print(" -Rimossa il current pattern. numero di pattern ancora su questa GS:" +
                        gs.listOfPatternsLine.Count, nameFile);

                    if (gs.listOfPatternsLine.Count < 2)
                    {
                        listOfMyGroupingSurface.Remove(gs);
                        KLdebug.Print(" RIMOSSA GS dalla lista di GS: era rimasta solo un pattern.", nameFile);
                    }
                    KLdebug.Print(" ", nameFile);
                }
            }
            KLdebug.Print(" ", nameFile);
        }
        
    }
}
