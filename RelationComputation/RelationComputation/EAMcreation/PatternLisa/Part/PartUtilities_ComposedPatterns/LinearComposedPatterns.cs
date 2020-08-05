using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities_ComposedPatterns
{
    public partial class GeometryAnalysis
    {
        //This function takes as input a list of coherent parallel patterns of type line, in relation because
        //their centroids are on a line or a circumference
        //It detects if they constitutes a COMPOSED PATTERN OF TYPE LINE (TRANSLATIONAL)
        //or a COMPOSED PATTERN OF TYPE CIRCUMFERENCE (ROTATIONAL)
        public static void GetComposedPatternsFromListOfPathsLine(List<MyPathOfPoints> listOfPathsOfCentroids,
            List<MyPattern> listOfParallelPatterns, ref List<MyMatrAdj> listOfMyMatrAdj,
            ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface,
            ref List<MyComposedPattern> listOfOutputComposedPattern,
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            var nameFile = "ComposedPatterns.txt";
            KLdebug.Print("", nameFile);
            KLdebug.Print(">>> VERIFICA DEI PATH TROVATI: " + listOfPathsOfCentroids.Count + " paths", nameFile);

            while (listOfPathsOfCentroids.Count > 0)
            {
                var currentPathOfCentroids = new MyPathOfPoints(listOfPathsOfCentroids[0].path,
                    listOfPathsOfCentroids[0].pathGeometricObject);
                listOfPathsOfCentroids.RemoveAt(0);
                //    //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                bool maxLength;
                if (currentPathOfCentroids.pathGeometricObject.GetType() == typeof(MyLine))
                {
                    KLdebug.Print("PATH DI TIPO LINEA!", nameFile);
                    maxLength = GetComposedPatternsFromPathLine(currentPathOfCentroids,
                    listOfParallelPatterns, ref listOfPathsOfCentroids, ref listOfMyMatrAdj,
                    ref listOfMyGroupingSurface, ref listOfOutputComposedPattern,
                    ref listOfOutputComposedPatternTwo);
                }
                else
                {
                    //composed rotational pattern of patterns line
                    KLdebug.Print("PATH DI TIPO CIRCONFERENZA!", nameFile);
                    maxLength = GetComposedPatternsFromPathCircum(currentPathOfCentroids,
                    listOfParallelPatterns, ref listOfPathsOfCentroids, ref listOfMyMatrAdj,
                    ref listOfMyGroupingSurface, ref listOfOutputComposedPattern,
                    ref listOfOutputComposedPatternTwo, SwApplication, ref fileOutput);
                }
            }
        }

        #region LINEAR TRANSLATIONAL COMPOSED PATTERN
        public static bool GetComposedPatternsFromPathLine(MyPathOfPoints currentPathOfCentroids, 
            List<MyPattern> listOfParallelPatterns, ref List<MyPathOfPoints> listOfPathsOfCentroids,
            ref List<MyMatrAdj> listOfMyMatrAdj, ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface, 
            ref List<MyComposedPattern> listOfOutputComposedPattern, 
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo)
        {
            var numOfPatterns = currentPathOfCentroids.path.Count;
            var noStop = true;

            const string nameFile = "ComposedPatterns_Linear.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("VERIFICA DELLE POSSIBILI TRASLAZIONI TRA " + numOfPatterns + " REPEATED ENTITY:", nameFile);
            KLdebug.Print(" ", nameFile);

            var listOfPatternOnThePath = currentPathOfCentroids.path.Select(ind => listOfParallelPatterns[ind]).ToList();

            var i = 0;
            while (i < (numOfPatterns - 1))
            {
                var newComposedPattern = new MyComposedPattern();
                var foundNewComposedPattern = GetMaximumTranslation_ComposedPatterns(listOfPatternOnThePath, 
                    currentPathOfCentroids.pathGeometricObject, ref i,
                    ref numOfPatterns, ref noStop, ref newComposedPattern);

                if (foundNewComposedPattern)
                {
                    if (newComposedPattern.listOfMyPattern.Count == numOfPatterns ||
                        newComposedPattern.listOfMyPattern.Count == numOfPatterns - 1)
                    {
                        noStop = true;
                    }

                    CheckAndUpdate_ComposedPatterns(newComposedPattern, ref listOfPathsOfCentroids,
                        listOfParallelPatterns, ref listOfMyMatrAdj, ref listOfMyGroupingSurface,
                        ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo);

                    // RICORDA: modificare anche il FindPaths_ComposedPatterns caso Maximum
                }
            }
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
                KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            return false;
        }

        public static bool GetMaximumTranslation_ComposedPatterns(List<MyPattern> listOfPatternOnThePath, 
            MyPathGeometricObject pathObject, ref int i, ref int numOfPatterns, ref bool noStop, 
            ref MyComposedPattern outputComposedPattern)
        {
            const string nameFile = "ComposedPatterns_Linear.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("NUOVO AVVIO DI RICERCA TRASLAZIONE per pattern composti:", nameFile);
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";

            var listOfPatternsOfNew = new List<MyPattern> { listOfPatternOnThePath[i] };
            var lengthOfNew = 1; 
            var exit = false;

            while (i < (numOfPatterns - 1) && exit == false)
            {
                KLdebug.Print(" ", nameFile);
                KLdebug.Print(" ", nameFile);
                whatToWrite = string.Format("         Confronto {0}^ pattern e la {1}^ pattern: ", i, i + 1);
                KLdebug.Print(whatToWrite, nameFile);

                if (IsTranslationTwoPatterns(listOfPatternOnThePath[i], listOfPatternOnThePath[i + 1]))
                {
                    listOfPatternsOfNew.Add(listOfPatternOnThePath[i + 1]);
                    lengthOfNew += 1;
                    KLdebug.Print("Aggiunta " + (i + 1) + "-esimo pattern. lengthOfNew = "
                        + lengthOfNew, nameFile);
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    KLdebug.Print(" ", nameFile);
                    KLdebug.Print("------>>> Interruzione alla posizione: " + i, nameFile);
                    KLdebug.Print(" ", nameFile);

                    i++;
                }
            }
            KLdebug.Print(" ", nameFile);

            if (lengthOfNew > 1)
            {
                outputComposedPattern.listOfMyPattern = listOfPatternsOfNew;
                outputComposedPattern.pathOfMyComposedPattern = pathObject;

                KLdebug.Print("lengthOfCurrentPath = " + lengthOfNew, nameFile);

                outputComposedPattern.typeOfMyComposedPattern = "(linear) TRANSLATION";
                KLdebug.Print("CREATO composed PATTERN TRANS (lineare) DI LUNGHEZZA = " + lengthOfNew, nameFile);

                outputComposedPattern.constStepOfMyComposedPattern = listOfPatternsOfNew[0].patternCentroid.Distance(
                    listOfPatternsOfNew[1].patternCentroid);
                return true;
            }

            KLdebug.Print("lengthOfNew = " + lengthOfNew, nameFile);
            KLdebug.Print("IL TENTATIVO DI composed PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            KLdebug.Print(" ", nameFile);

            return false;

        }

        //This function verifies if two given patterns are related by translation, with the difference
        //vector between the two pattern centroids as candidate translational vector.
        public static bool IsTranslationTwoPatterns(MyPattern firstMyPattern, MyPattern secondMyPattern)
        {
            const string nameFile = "ComposedPatterns_Linear.txt";
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";

            //check of the length correspondence:
            var firstPatternLength = firstMyPattern.listOfMyREOfMyPattern.Count;
            var secondPatternLength = secondMyPattern.listOfMyREOfMyPattern.Count;
            if (firstPatternLength != secondPatternLength)
            {
                return false;
            }

            //candidate translational array computation:
            double[] candidateTranslationArray =
            {
                secondMyPattern.patternCentroid.x-firstMyPattern.patternCentroid.x,
                secondMyPattern.patternCentroid.y-firstMyPattern.patternCentroid.y,
                secondMyPattern.patternCentroid.z-firstMyPattern.patternCentroid.z,
            };
            whatToWrite = string.Format("Candidate translational array: ({0}, {1}, {2})", candidateTranslationArray[0],
                            candidateTranslationArray[1], candidateTranslationArray[2]);
            KLdebug.Print(whatToWrite, nameFile);

            KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la 0^ RE del 2° pattern? " + 
                PartUtilities.GeometryAnalysis.IsTranslationTwoREGivenCandidateVector(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[0], candidateTranslationArray), nameFile);
            KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la (n-1)^ RE del 2° pattern? " +
                PartUtilities.GeometryAnalysis.IsTranslationTwoREGivenCandidateVector(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[secondPatternLength-1], candidateTranslationArray), nameFile);

            if (PartUtilities.GeometryAnalysis.IsTranslationTwoREGivenCandidateVector(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[0], candidateTranslationArray) ||
                PartUtilities.GeometryAnalysis.IsTranslationTwoREGivenCandidateVector(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[secondPatternLength - 1], candidateTranslationArray))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region CIRCULAR ROTATIONAL COMPOSED PATTERN
        public static bool GetComposedPatternsFromPathCircum(MyPathOfPoints currentPathOfCentroids,
            List<MyPattern> listOfCoherentPatterns, ref List<MyPathOfPoints> listOfPathsOfCentroids,
            ref List<MyMatrAdj> listOfMyMatrAdj, ref List<MyGroupingSurfaceForPatterns> listOfMyGroupingSurface,
            ref List<MyComposedPattern> listOfOutputComposedPattern,
            ref List<MyComposedPattern> listOfOutputComposedPatternTwo, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            var numOfPatterns = currentPathOfCentroids.path.Count;
            var noStop = true;

            const string nameFile = "ComposedPatterns_LinearOnCircumference.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("VERIFICA DELLE POSSIBILI Rotazioni TRA " + numOfPatterns + " PATTERNS lineari:", nameFile);
            KLdebug.Print(" ", nameFile);

            var listOfPatternOnThePath = currentPathOfCentroids.path.Select(ind => listOfCoherentPatterns[ind]).ToList();
            var pathCircumference = (MyCircumForPath)currentPathOfCentroids.pathGeometricObject;

            var i = 0;
            while (i < (numOfPatterns - 1))
            {
                var newComposedPattern = new MyComposedPattern();
                var j = i;
                var foundNewComposedPattern = GetMaximumTranslation_ComposedPatterns(listOfPatternOnThePath,
                    pathCircumference, ref j, ref numOfPatterns, ref noStop, ref newComposedPattern);

                if (foundNewComposedPattern == false)
                {
                    foundNewComposedPattern = GetMaximumRotation_ComposedPatterns(listOfPatternOnThePath,
                       pathCircumference, ref i, ref numOfPatterns, ref noStop, ref newComposedPattern, SwApplication, ref fileOutput);
                }
                else
                {
                    i = j;
                }

                if (foundNewComposedPattern)
                {
                    if (newComposedPattern.listOfMyPattern.Count == numOfPatterns ||
                        newComposedPattern.listOfMyPattern.Count == numOfPatterns - 1)
                    {
                        noStop = true;
                    }

                    CheckAndUpdate_ComposedPatterns(newComposedPattern, ref listOfPathsOfCentroids,
                        listOfCoherentPatterns, ref listOfMyMatrAdj, ref listOfMyGroupingSurface,
                        ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo);
                }             
            }
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
                KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            return false;
        }


        public static bool GetMaximumRotation_ComposedPatterns(List<MyPattern> listOfPatternOnThePath,
            MyCircumForPath pathObject, ref int i, ref int numOfPatterns, ref bool noStop,
            ref MyComposedPattern outputComposedPattern, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            const string nameFile = "ComposedPatterns_Linear.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE per pattern composti:", nameFile);
            KLdebug.Print(" ", nameFile);

            var listOfPatternsOfNew = new List<MyPattern> { listOfPatternOnThePath[i] };
            var lengthOfNew = 1;
            var exit = false;

            //Computation of the rotation angle:
            double[] planeNormal =
            {
                pathObject.circumplane.a, 
                pathObject.circumplane.b,
                pathObject.circumplane.c
            };

            var teta = FunctionsLC.FindAngle(listOfPatternOnThePath[0].patternCentroid,
                listOfPatternOnThePath[1].patternCentroid, pathObject.circumcenter);

            var axisDirection = PartUtilities.GeometryAnalysis.establishAxisDirection(listOfPatternOnThePath[0].patternCentroid,
                listOfPatternOnThePath[1].patternCentroid,
                pathObject.circumcenter, planeNormal);

            while (i < (numOfPatterns - 1) && exit == false)
            {
                KLdebug.Print(" ", nameFile);
                KLdebug.Print(" ", nameFile);
                var whatToWrite = string.Format("         Confronto {0}^ pattern e la {1}^ pattern: ", i, i + 1);
                KLdebug.Print(whatToWrite, nameFile);

                if (IsRotationTwoPatterns(listOfPatternOnThePath[i], listOfPatternOnThePath[i + 1], teta, axisDirection,
                    pathObject.circumcenter, SwApplication, ref fileOutput))
                {
                    listOfPatternsOfNew.Add(listOfPatternOnThePath[i + 1]);
                    lengthOfNew += 1;
                    KLdebug.Print("Aggiunta " + (i + 1) + "-esimo pattern. lengthOfNew = "
                        + lengthOfNew, nameFile);
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    KLdebug.Print(" ", nameFile);
                    KLdebug.Print("------>>> Interruzione alla posizione: " + i, nameFile);
                    KLdebug.Print(" ", nameFile);

                    i++;
                }
            }
            KLdebug.Print(" ", nameFile);

            if (lengthOfNew > 1)
            {
                outputComposedPattern.listOfMyPattern = listOfPatternsOfNew;
                outputComposedPattern.pathOfMyComposedPattern = pathObject;

                KLdebug.Print("lengthOfCurrentPath = " + lengthOfNew, nameFile);

                outputComposedPattern.typeOfMyComposedPattern = "ROTATION";
                KLdebug.Print("CREATO composed PATTERN ROTATIONAL DI LUNGHEZZA = " + lengthOfNew, nameFile);

                outputComposedPattern.constStepOfMyComposedPattern = listOfPatternsOfNew[0].patternCentroid.Distance(
                    listOfPatternsOfNew[1].patternCentroid);
                return true;
            }

            KLdebug.Print("lengthOfNew = " + lengthOfNew, nameFile);
            KLdebug.Print("IL TENTATIVO DI composed PATTERN ROTATIONAL ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            KLdebug.Print(" ", nameFile);

            return false;
        }

        public static bool IsRotationTwoPatterns(MyPattern firstMyPattern, MyPattern secondMyPattern, double teta,
            double[] axisDirection, MyVertex circumCenter, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            const string nameFile = "ComposedPatterns_LinearOnCircumference.txt";
            KLdebug.Print(" ", nameFile);

            //check of the length correspondence:
            var firstPatternLength = firstMyPattern.listOfMyREOfMyPattern.Count;
            var secondPatternLength = secondMyPattern.listOfMyREOfMyPattern.Count;
            if (firstPatternLength != secondPatternLength)
            {
                return false;
            }

            //KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la 0^ RE del 2° pattern? " +
            //    IsRotationTwoRE(firstMyPattern.listOfMyREOfMyPattern[0],
            //    secondMyPattern.listOfMyREOfMyPattern[0], teta, axisDirection, circumCenter), nameFile);
            //KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la (n-1)^ RE del 2° pattern? " +
            //    IsRotationTwoRE(firstMyPattern.listOfMyREOfMyPattern[0],
            //    secondMyPattern.listOfMyREOfMyPattern[secondPatternLength - 1], teta, axisDirection, circumCenter), nameFile);

            if (PartUtilities.GeometryAnalysis.IsRotationTwoRE(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[0], teta, axisDirection, circumCenter) ||
                PartUtilities.GeometryAnalysis.IsRotationTwoRE(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[secondPatternLength - 1], teta, axisDirection, circumCenter))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region REFLECTIONAL COMPOSED PATTERN
        //This function verifies if two given patterns are related by translation, with the difference
        //vector between the two pattern centroids as candidate translational vector.
        public static bool IsReflectionTwoPatterns(MyPattern firstMyPattern, MyPattern secondMyPattern)
        {
            const string nameFile = "ComposedPatterns_Linear.txt";
            KLdebug.Print(" ", nameFile);
         
            //check of the length correspondence:
            var firstPatternLength = firstMyPattern.listOfMyREOfMyPattern.Count;
            var secondPatternLength = secondMyPattern.listOfMyREOfMyPattern.Count;
            if (firstPatternLength != secondPatternLength)
            {
                return false;
            }

            //candidate reflectional plane computation:
            var candidateReflMyPlane = PartUtilities.GeometryAnalysis.GetCandidateReflectionalMyPlane(firstMyPattern.patternCentroid,
             secondMyPattern.patternCentroid, null);

            KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la 0^ RE del 2° pattern? " +
                PartUtilities.GeometryAnalysis.IsReflectionTwoREGivenCandidateReflPlane(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[0], candidateReflMyPlane), nameFile);
            KLdebug.Print("La 0^ RE del 1° pattern è compatibile con la (n-1)^ RE del 2° pattern? " +
                PartUtilities.GeometryAnalysis.IsReflectionTwoREGivenCandidateReflPlane(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[secondPatternLength - 1], candidateReflMyPlane), nameFile);

            if (PartUtilities.GeometryAnalysis.IsReflectionTwoREGivenCandidateReflPlane(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[0], candidateReflMyPlane) ||
                PartUtilities.GeometryAnalysis.IsReflectionTwoREGivenCandidateReflPlane(firstMyPattern.listOfMyREOfMyPattern[0],
                secondMyPattern.listOfMyREOfMyPattern[secondPatternLength - 1], candidateReflMyPlane))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
