using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;
using Functions = AssemblyRetrieval.PatternLisa.Part.PathCreation_Part.Functions;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities_ComposedPatterns
{
    public partial class LC_AssemblyTraverse
    {
        public static void FindComposedPatternsOfComponents(
            List<MyPatternOfComponents> listOfPatternsOfComponentsLine,
            List<MyPatternOfComponents> listOfPatternsOfComponentsCircum,
            out List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            out List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo,
            ModelDoc2 SwModel, SldWorks mySwApplication, ref StringBuilder fileOutput)
        {
            var nameFile = "ComposedPatterns_Assembly.txt";
            KLdebug.Print("", nameFile);
            KLdebug.Print("-- RAGGRUPPAMENTO DEI PATTERN TROVATI:", nameFile);

            var toleranceOk = true;
            var listOfComposedPattern = new List<MyComposedPatternOfComponents>();
            var listOfComposedPatternTwo = new List<MyComposedPatternOfComponents>();

            
            // >>>>>>> LINEAR CASE:
            //first I group patterns by same length and same distance:
            var listOfListsOfCoherentPatternsLine = GroupFoundPatternsOfComponentsOfTypeLine(
                listOfPatternsOfComponentsLine, mySwApplication);
            if (listOfListsOfCoherentPatternsLine.Count == 0)
            {
                KLdebug.Print("-- Nessun pattern LINEA da esaminare per pattern composti!", nameFile);
            }

            //Then I group coherent patterns in subgroups of parallel patterns:
            foreach (var list in listOfListsOfCoherentPatternsLine)
            {
                //Grouping in lists of parallel patterns
                var listOfListsOfParallelPatterns = GroupPatternsOfComponentsInParallel(list);
                var numOfListsOfParallelPatterns = listOfListsOfParallelPatterns.Count;

                if (list.Count == 2)
                {
                    //I verify if a composed pattern with these 2 patterns has already been created
                    //in another GS:
                    if (ComposedPatternOfComponentsOfLength2AlreadyExists(listOfComposedPatternTwo, list) == false)
                    {
                        //if a composed pattern does not exist yet AND
                        //the 2 patterns are not parallel, I verify if it is REFLECTION:
                        if (numOfListsOfParallelPatterns == 0)
                        {
                            KLdebug.Print("   >>>> SOLO 2 PATTERN su questa GS: POTREBBE ESSERE RIFLESSIONE",
                                nameFile);
                            KLdebug.Print("       INFATTI HO " + numOfListsOfParallelPatterns +
                                                "LISTA DI PATTERN PARALLELI", nameFile);
                            //if (IsReflectionTwoPatterns(list[0], list[1]))
                            //{
                            //    KLdebug.Print("I 2 Pattern sono legate da RIFLESSIONE!", nameFile);

                            //    var typeOfNewComposedPattern = "Composed REFLECTION of length 2";
                            //    BuildNewComposedPatternOfLength2(fileOutput, typeOfNewComposedPattern,
                            //        ref listOfComposedPattern, ref listOfComposedPatternTwo,
                            //        ref listOfGroupingSurfaceForPatterns, list);
                            //}

                        }
                        else
                        //if a composed pattern does not exist yet AND
                        //the 2 patterns are parallel, I verify if it is TRANSLATION:
                        {
                                        KLdebug.Print("   >>>> SOLO 2 PATTERN su questa GS: POTREBBE ESSERE TRASLAZIONE",
                                            nameFile);
                                        KLdebug.Print("       INFATTI HO " + numOfListsOfParallelPatterns +
                                                            "LISTA DI PATTERN PARALLELI", nameFile);
                                        if (IsTranslationTwoPatternsOfComponents(list[0], list[1]))
                                        {
                                            KLdebug.Print("I 2 Pattern sono legate da TRASLAZIONE!", nameFile);

                                            var typeOfNewComposedPattern = "Composed TRANSLATION of length 2";
                                            BuildNewComposedPatternOfComponentsOfLength2(fileOutput, typeOfNewComposedPattern,
                                                ref listOfComposedPattern, ref listOfComposedPatternTwo, list);
                                        }
                                        else
                                        {
                                            KLdebug.Print("   >>>> NON è TRASLAZIONE: POTREBBE ESSERE RIFLESSIONE O ROTAZIONE", nameFile);
                            
                                        }

                        }
                    }
                    else
                    {
                        KLdebug.Print("Un composed pattern con questi pattern era già stato trovato!", nameFile);
                        KLdebug.Print("NON FACCIO NIENTE!", nameFile);
                    }
                }
                else
                {
                    KLdebug.Print(
                        "   >>>> PIU' DI 2 PATTERN su questa GS: APPLICO IL PATH DETECTION ALGORITHM", nameFile);

                    foreach (var listOfParallelPatterns in listOfListsOfParallelPatterns)
                    {
                        KLdebug.Print(
                            "   ---> NUOVO SET DI " + listOfParallelPatterns.Count + " PATTERN LINEA PARALLELI",
                            nameFile);

                        if (listOfParallelPatterns.Count == 2)
                        {
                            //I verify if a composed pattern with these 2 patterns has already been created
                            //in another GS:
                            if (ComposedPatternOfComponentsOfLength2AlreadyExists(listOfComposedPatternTwo, list))
                            {
                                KLdebug.Print(
                                    "Un composed pattern con questi pattern era già stato trovato!", nameFile);
                                KLdebug.Print("NON FACCIO NIENTE!", nameFile);
                            }
                            else
                            {
                                KLdebug.Print(
                                    "        >>>> QUESTO GRUPPO CONTIENE SOLO 2 PATTERN PARALLELI:", nameFile);
                                if (IsTranslationTwoPatternsOfComponents(listOfParallelPatterns[0], listOfParallelPatterns[1]))
                                {
                                    KLdebug.Print("I 2 Pattern sono legate da TRASLAZIONE!", nameFile);

                                    var typeOfNewComposedPattern = "Composed TRANSLATION of length 2";
                                    BuildNewComposedPatternOfComponentsOfLength2(fileOutput, typeOfNewComposedPattern,
                                        ref listOfComposedPattern, ref listOfComposedPatternTwo,
                                        listOfParallelPatterns);
                                }
                                else
                                {
                                    KLdebug.Print("   >>>> NON è TRASLAZIONE: POTREBBE ESSERE RIFLESSIONE O ROTAZIONE", nameFile);
                               
                                }
                            }
                        }
                        else //(listOfParallelPatterns.Count > 2)
                        {
                            var listOfPatternCentroids = listOfParallelPatterns.Select(pattern => pattern.patternCentroid).ToList();
                            fileOutput.AppendLine("");
                            fileOutput.AppendLine("        >>>> CREATION OF ADJECENCY MATRICES FOR COMPOSED PATTERNS:");
                            var listOfMyMatrAdj = Functions.CreateMatrAdj(listOfPatternCentroids, ref fileOutput);

                            var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.
                            while (listOfMyMatrAdj.Count > 0 && maxPath == false && toleranceOk)
                            {
                                var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj[0].d, listOfMyMatrAdj[0].matr,
                                    listOfMyMatrAdj[0].nOccur);
                                listOfMyMatrAdj.Remove(listOfMyMatrAdj[0]);
                                //NOTA: forse la mia MatrAdj non deve essere rimossa ma conservata,
                                //soprattutto nel caso in cui si presenta onlyShortPath = true
                                //(non avrebbe senso cancellarla, ma conservarla per la ricerca di path
                                //di 2 RE).
                                KLdebug.Print("----> Considero NUOVA MatrAdj. Sono rimaste ancora " +
                                                        listOfMyMatrAdj.Count +
                                                        " MatrAdj da controllare.", nameFile);
                                KLdebug.Print(" ", nameFile);
                                List<MyPathOfPoints> listOfPathsOfCentroids;
                                bool onlyShortPaths;
                                maxPath = PathCreation_Assembly_ComposedPatterns.Functions.FindPaths_Assembly_ComposedPatterns(currentMatrAdj,
                                    listOfParallelPatterns,
                                    ref fileOutput, out listOfPathsOfCentroids, out onlyShortPaths, ref toleranceOk,
                                    ref listOfMyMatrAdj,ref listOfComposedPattern,
                                    ref listOfComposedPatternTwo, SwModel, mySwApplication);
                                KLdebug.Print(" ", nameFile);
                                KLdebug.Print("PER QUESTA MATRADJ prima della ricerca 'ufficiale' di pattern': ", nameFile);
                                KLdebug.Print("maxPath = " + maxPath, nameFile);
                                KLdebug.Print("listOfMyPattern.Count = " + listOfComposedPattern.Count, nameFile);
                                KLdebug.Print("listOfMyPatternTwo.Count = " + listOfComposedPatternTwo.Count, nameFile);
                                KLdebug.Print("onlyShortPath = " + onlyShortPaths, nameFile);
                                KLdebug.Print("toleranceOK = " + toleranceOk, nameFile);
                                //KLdebug.Print("listOfPathOfCentroids.Count = " + listOfPathsOfCentroids.Count, nameFile);

                                if (toleranceOk)
                                {
                                    if (listOfPathsOfCentroids != null)
                                    {
                                        if (maxPath == false)
                                        {
                                            if (onlyShortPaths == false)
                                            {
                                                GetComposedPatternsFromListOfPathsLine_Assembly(listOfPathsOfCentroids,
                                                    listOfParallelPatterns, ref listOfMyMatrAdj, 
                                                    ref listOfComposedPattern, ref listOfComposedPatternTwo, mySwApplication, ref fileOutput);
                                            }
                                            else
                                            {
                                                //non faccio niente e li rimetto in gioco per 
                                            }
                                        }
                                        else
                                        {
                                            KLdebug.Print(" !!! sono nel maxPath= true", nameFile);
                                        }
                                    }
                                    else
                                    {
                                        KLdebug.Print("NO PATH FOUND!", nameFile);
                                    }
                                }
                                else
                                {
                                    KLdebug.Print(
                                        "===>>    TOLLERANZA NON SUFFICIENTEMENTE PICCOLA. TERMINATO.", nameFile);
                                    listOfOutputComposedPattern = listOfComposedPattern;
                                    listOfOutputComposedPatternTwo = listOfComposedPatternTwo;
                                    return;
                                }
                            }
                        }
                    }

                    //Now coherent linear patterns that have not been set in a composed pattern yet
                    //are examined to see if they consitute any rotation composed pattern:
                    KLdebug.Print("", nameFile);
                    KLdebug.Print("--->> FINE DEL CONTROLLO SU PATTERN LINEARI PARALLELI", nameFile);
                    KLdebug.Print("", nameFile);
                    KLdebug.Print("", nameFile);
                    KLdebug.Print("--->> PASSO ALLA RICERCA DI PATTERN LINEARI IN PATTERN COMPOSTO ROTAZIONALE", nameFile);
                    KLdebug.Print("lista di pattern coerenti corrente. Count = " + list.Count, nameFile);

                    list.RemoveAll(
                        pattern =>
                            listOfComposedPattern.FindIndex(
                                composedPattern =>
                                    composedPattern.ListOfMyPatternOfComponents.FindIndex(
                                        patternInComposedPattern =>
                                            patternInComposedPattern.idMyPattern == pattern.idMyPattern) != -1) != -1);
                    KLdebug.Print("Rimuovo dalla lista di pattern coerenti quelli già sistemati. Nuovo Count = " + list.Count, nameFile);
                    if (list.Count != 0)
                    {
                        if (list.Count == 2)
                        {
                            //  >>> POSSIBLE REFLECTION CAN EXIST

                            //I verify if a composed pattern with these 2 patterns has already been created
                            //in another GS:
                            if (ComposedPatternOfComponentsOfLength2AlreadyExists(listOfComposedPatternTwo, list))
                            {
                                KLdebug.Print(
                                    "Un composed pattern con questi pattern era già stato trovato!", nameFile);
                                KLdebug.Print("NON FACCIO NIENTE!", nameFile);
                            }
                            else
                            {
                                KLdebug.Print(
                                    "        >>>> QUESTO GRUPPO CONTIENE SOLO 2 PATTERN: potrebbe essere rotazione", nameFile);
                                
                            }
                        }
                        else
                        {
                            var listOfPatternCentroids1 = list.Select(pattern => pattern.patternCentroid).ToList();
                            fileOutput.AppendLine("");
                            fileOutput.AppendLine("        >>>> CREATION OF ADJECENCY MATRICES FOR COMPOSED PATTERNS:");
                            var listOfMyMatrAdj1 = Functions.CreateMatrAdj(listOfPatternCentroids1, ref fileOutput);

                            var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.
                            while (listOfMyMatrAdj1.Count > 0 && maxPath == false && toleranceOk)
                            {
                                var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj1[0].d, listOfMyMatrAdj1[0].matr,
                                    listOfMyMatrAdj1[0].nOccur);
                                listOfMyMatrAdj1.Remove(listOfMyMatrAdj1[0]);
                                //NOTA: forse la mia MatrAdj non deve essere rimossa ma conservata,
                                //soprattutto nel caso in cui si presenta onlyShortPath = true
                                //(non avrebbe senso cancellarla, ma conservarla per la ricerca di path
                                //di 2 RE).
                                KLdebug.Print("----> Considero NUOVA MatrAdj. Sono rimaste ancora " +
                                                  listOfMyMatrAdj1.Count +
                                                  " MatrAdj da controllare.", nameFile);
                                KLdebug.Print(" ", nameFile);
                                List<MyPathOfPoints> listOfPathsOfCentroids1;
                                bool onlyShortPaths1;

                                var maxPath1 = PathCreation_Assembly_ComposedPatterns.Functions.FindPaths_Assembly_ComposedPatterns(currentMatrAdj,
                                    list, ref fileOutput, out listOfPathsOfCentroids1, out onlyShortPaths1,
                                    ref toleranceOk, ref listOfMyMatrAdj1,
                                    ref listOfComposedPattern, ref listOfComposedPatternTwo, SwModel, mySwApplication);
                                
                                if (toleranceOk)
                                {
                                    if (listOfPathsOfCentroids1 != null)
                                    {
                                        //I ignore every linear path (I look for composed rotational patterns now):
                                        var listOfCircularPaths =
                                            listOfPathsOfCentroids1.FindAll(
                                                path =>
                                                    path.pathGeometricObject.GetType() == typeof(MyCircumForPath))
                                                .ToList();

                                        if (maxPath1 == false)
                                        {
                                            if (onlyShortPaths1 == false)
                                            {
                                                GetComposedPatternsFromListOfPathsLine_Assembly(listOfCircularPaths,
                                                    list, ref listOfMyMatrAdj1,
                                                    ref listOfComposedPattern, ref listOfComposedPatternTwo, mySwApplication, ref fileOutput);
                                            }
                                            else
                                            {
                                                //non faccio niente e li rimetto in gioco per 
                                            }
                                        }
                                    }
                                    else
                                    {
                                        KLdebug.Print("NO PATH FOUND!", nameFile);
                                    }
                                }
                                else
                                {
                                    KLdebug.Print(
                                        "===>>    TOLLERANZA NON SUFFICIENTEMENTE PICCOLA. TERMINATO.",
                                        nameFile);
                                }
                            }

                        }
                    }
                    else
                    {
                        KLdebug.Print("--> NON CI SONO ALTRI PATTERN DA CONSIDERARE ", nameFile);
                    }
                }
            }




            listOfOutputComposedPattern = listOfComposedPattern;
            listOfOutputComposedPatternTwo = listOfComposedPatternTwo;
        }

        public static void BuildNewComposedPatternOfComponentsOfLength2(StringBuilder fileOutput, string typeOfNewComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfComposedPattern, ref List<MyComposedPatternOfComponents> listOfComposedPatternTwo,
            List<MyPatternOfComponents> listOfPatternsToConsider)
        {
            var listOfPathOfCentroids = new List<MyPathOfPoints>();
            var listOfMyMatrAdj = new List<MyMatrAdj>();
            var newListOfPatterns = new List<MyPatternOfComponents>();
            newListOfPatterns.Add(listOfPatternsToConsider[0]);
            newListOfPatterns.Add(listOfPatternsToConsider[1]);
            var newComposedPatternGeomObject = FunctionsLC.LinePassingThrough(
                listOfPatternsToConsider[0].patternCentroid, listOfPatternsToConsider[1].patternCentroid);
            var newComposedPatternType = typeOfNewComposedPattern;
            var newComposedPattern = new MyComposedPatternOfComponents(newListOfPatterns,
                newComposedPatternGeomObject, newComposedPatternType);
            GeometryAnalysis.CheckAndUpdate_Assembly_ComposedPatterns(newComposedPattern, ref listOfPathOfCentroids,
                listOfPatternsToConsider, ref listOfMyMatrAdj,
                ref listOfComposedPattern, ref listOfComposedPatternTwo);
        }       

        public static bool ComposedPatternOfComponentsOfLength2AlreadyExists(
            List<MyComposedPatternOfComponents> listOfComposedPatternTwo,
            List<MyPatternOfComponents> listOfPatterns)
        {
            var firstPattern = listOfPatterns[0];
            var secondPattern = listOfPatterns[1];
            var indOfFound =
                listOfComposedPatternTwo.FindIndex(
                    composedPattern => composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        pattern => pattern.idMyPattern == firstPattern.idMyPattern) != -1 &&
                        composedPattern.ListOfMyPatternOfComponents.FindIndex(
                        pattern => pattern.idMyPattern == secondPattern.idMyPattern) != -1);
            if (indOfFound == -1)
            {
                return false;
            }
            return true;
        }

        //This function subdivides a list of patterns line in patterns with parallel lines
        public static List<List<MyPatternOfComponents>> GroupPatternsOfComponentsInParallel(
            List<MyPatternOfComponents> listsOfLinePatterns)
        {
            var nameFile = "ComposedPatterns_Assembly.txt";

            var listOfListsOfParallelPatterns = new List<List<MyPatternOfComponents>>();
            foreach (var pattern in listsOfLinePatterns)
            {
                KLdebug.Print("", nameFile);

                var lineThisPattern = (MyLine)pattern.pathOfMyPattern;
                var invertedDirectionThisPattern = new double[3];
                invertedDirectionThisPattern.SetValue(-lineThisPattern.direction[0], 0);
                invertedDirectionThisPattern.SetValue(-lineThisPattern.direction[1], 1);
                invertedDirectionThisPattern.SetValue(-lineThisPattern.direction[2], 2);

                var indexOfFind = listOfListsOfParallelPatterns.FindIndex(list =>
                    (FunctionsLC.MyEqualsArray(((MyLine)list[0].pathOfMyPattern).direction,
                    ((MyLine)pattern.pathOfMyPattern).direction) || FunctionsLC.MyEqualsArray(((MyLine)list[0].pathOfMyPattern).direction,
                    invertedDirectionThisPattern)));
                if (indexOfFind != -1)
                {
                    var whatToWrite = string.Format("     AGGIORNATA LISTA di pattern ESISTENTE con direzione ({0},{1},{2})",
                        ((MyLine)pattern.pathOfMyPattern).direction[0], ((MyLine)pattern.pathOfMyPattern).direction[1],
                        ((MyLine)pattern.pathOfMyPattern).direction[2]);
                    KLdebug.Print(whatToWrite, nameFile);
                    //The list already exists. I add it to the corresponding list:
                    listOfListsOfParallelPatterns[indexOfFind].Add(pattern);
                }
                else
                {
                    //The list does not exist yet. I create it:
                    List<MyPatternOfComponents> newListOfPatterns = new List<MyPatternOfComponents> { pattern };
                    listOfListsOfParallelPatterns.Add(newListOfPatterns);
                    var whatToWrite = string.Format("     CREATA NUOVA LISTA di pattern linea paralleli con direzione ({0},{1},{2})",
                        ((MyLine)pattern.pathOfMyPattern).direction[0], ((MyLine)pattern.pathOfMyPattern).direction[1],
                        ((MyLine)pattern.pathOfMyPattern).direction[2]);
                    KLdebug.Print(whatToWrite, nameFile);
                }
            }

            KLdebug.Print("", nameFile);
            KLdebug.Print("TERMINE della creazione delle liste di pattern paralleli.", nameFile);
            KLdebug.Print("Ci sono " + listOfListsOfParallelPatterns.Count + " liste", nameFile);
            KLdebug.Print("Ora elimino quelle con solo una occorrenza:", nameFile);
            listOfListsOfParallelPatterns.RemoveAll(list => list.Count < 2);
            KLdebug.Print("Ora ci sono " + listOfListsOfParallelPatterns.Count + " liste.", nameFile);
            KLdebug.Print("TERMINE.", nameFile);
            KLdebug.Print("", nameFile);

            return listOfListsOfParallelPatterns;
        }

        //This function takes as input a list of patterns with MyPathGeometricObject line,
        //it subdivides the patterns by same number of RC on the pattern, then by constant distance.
        public static List<List<MyPatternOfComponents>> GroupFoundPatternsOfComponentsOfTypeLine(
            List<MyPatternOfComponents> listOfPatternsOfTypeLine, SldWorks mySwApplication)
        {
            var nameFile = "ComposedPatterns_Assembly.txt";
            var tolerance = Math.Pow(10, -4);
            var listOfListsOfPatterns = new List<List<MyPatternOfComponents>>();
            foreach (var pattern in listOfPatternsOfTypeLine)
            {
                KLdebug.Print("", nameFile);
                KLdebug.Print("--- Classifico pattern di tipo " + pattern.typeOfMyPattern +
                    ", lunghezza = " + pattern.listOfMyRCOfMyPattern.Count +
                    ", constDistance = " + pattern.constStepOfMyPattern, nameFile);

                var indexOfFind = listOfListsOfPatterns.FindIndex(list => (
                    list[0].listOfMyRCOfMyPattern.Count == pattern.listOfMyRCOfMyPattern.Count &&
                    Math.Abs(list[0].constStepOfMyPattern - pattern.constStepOfMyPattern) < tolerance));

                if (indexOfFind != -1)
                {
                    KLdebug.Print("     AGGIORNATA LISTA ESISTENTE che conteneva già " + 
                        listOfListsOfPatterns[indexOfFind].Count + " elementi.", nameFile);
                    //The list referred to patterns with same RC number and same constant step already exists. I add it to the corresponding list:
                    listOfListsOfPatterns[indexOfFind].Add(pattern);
                }
                else
                {
                    //The list referred to patterns with same RE number and same constant step does not exist yet. I create it:
                    List<MyPatternOfComponents> newListOfPatterns = new List<MyPatternOfComponents> { pattern };
                    listOfListsOfPatterns.Add(newListOfPatterns);
                    KLdebug.Print("     CREATA NUOVA LISTA", nameFile);
                }
            }
            KLdebug.Print("", nameFile);
            KLdebug.Print("TERMINE della creazione delle liste di pattern linea.", nameFile);
            KLdebug.Print("Ci sono " + listOfListsOfPatterns.Count + " liste", nameFile);
            KLdebug.Print("Ora elimino quelle con solo una occorrenza:", nameFile);
            listOfListsOfPatterns.RemoveAll(list => list.Count < 2);
            KLdebug.Print("Ora ci sono " + listOfListsOfPatterns.Count + " liste.", nameFile);
            KLdebug.Print("TERMINE.", nameFile);
            KLdebug.Print("", nameFile);

            return listOfListsOfPatterns;
        }

        //This function takes as input a list of patterns with MyPathGeometricObject circumference,
        //it subdivides the patterns by same number of RC on the pattern, then by angle.
        public static List<List<MyPatternOfComponents>> GroupFoundPatternsOfComponentsOfTypeCircum(
            List<MyPatternOfComponents> listOfPatternsOfTypeCircum, SldWorks mySwApplication)
        {
            var nameFile = "ComposedPatterns_Assembly.txt";
            var tolerance = Math.Pow(10, -6);
            var listOfListsOfPatterns = new List<List<MyPatternOfComponents>>();
            foreach (var pattern in listOfPatternsOfTypeCircum)
            {
                KLdebug.Print("", nameFile);
                KLdebug.Print(
                    "--- Classifico pattern di tipo " + pattern.typeOfMyPattern + ", lunghezza = " +
                    pattern.listOfMyRCOfMyPattern.Count + " angle = " + pattern.angle, nameFile);

                var indexOfFind = listOfListsOfPatterns.FindIndex(list => (list[0].listOfMyRCOfMyPattern.Count ==
                    pattern.listOfMyRCOfMyPattern.Count && Math.Abs(list[0].angle - pattern.angle) < tolerance));
                if (indexOfFind != -1)
                {
                    KLdebug.Print("     AGGIORNATA LISTA ESISTENTE che conteneva già " + 
                        listOfListsOfPatterns[indexOfFind].Count + " elementi.", nameFile);
                    //The list referred to patterns with same RE number and same constant step already exists. I add it to the corresponding list:
                    listOfListsOfPatterns[indexOfFind].Add(pattern);
                }
                else
                {
                    //The list referred to patterns with same RE number and same constant step does not exist yet. I create it:
                    List<MyPatternOfComponents> newListOfPatterns = new List<MyPatternOfComponents> { pattern };
                    listOfListsOfPatterns.Add(newListOfPatterns);
                    KLdebug.Print("     CREATA NUOVA LISTA", nameFile);
                }
            }
            KLdebug.Print("", nameFile);
            KLdebug.Print("TERMINE della creazione delle liste di pattern circonferenza.", nameFile);
            KLdebug.Print("Ci sono " + listOfListsOfPatterns.Count + " liste", nameFile);
            KLdebug.Print("Ora elimino quelle con solo una occorrenza:", nameFile);
            listOfListsOfPatterns.RemoveAll(list => list.Count < 2);
            KLdebug.Print("Ora ci sono " + listOfListsOfPatterns.Count + " liste.", nameFile);
            KLdebug.Print("", nameFile);

            return listOfListsOfPatterns;
        }

    }
}
