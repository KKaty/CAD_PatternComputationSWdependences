using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities_ComposedPatterns
{
    public partial class LC_AssemblyTraverse
    {
        public static void GetComposedPatternsFromListOfPathsLine_Assembly(List<MyPathOfPoints> listOfPathsOfCentroids,
            List<MyPatternOfComponents> listOfParallelPatterns, ref List<MyMatrAdj> listOfMyMatrAdj,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            var nameFile = "ComposedPatterns_Assembly.txt";
            
            while (listOfPathsOfCentroids.Count > 0)
            {
                var currentPathOfCentroids = new MyPathOfPoints(listOfPathsOfCentroids[0].path,
                    listOfPathsOfCentroids[0].pathGeometricObject);
                listOfPathsOfCentroids.RemoveAt(0);
                //I remove it immediately so in the update phase there is not it in the listOfMyPathsOfCentroids

                bool maxLength;
                if (currentPathOfCentroids.pathGeometricObject.GetType() == typeof(MyLine))
                {
                    maxLength = GetComposedPatternsFromPathLine_Assembly(currentPathOfCentroids,
                    listOfParallelPatterns, ref listOfPathsOfCentroids, ref listOfMyMatrAdj,
                    ref listOfOutputComposedPattern,
                    ref listOfOutputComposedPatternTwo);
                }
                else
                {
                    //composed rotational pattern of patterns line
                    maxLength = GetComposedPatternsFromPathCircum_Assembly(currentPathOfCentroids,
                    listOfParallelPatterns, ref listOfPathsOfCentroids, ref listOfMyMatrAdj,
                    ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo,
                    SwApplication, ref fileOutput);
                }
            }
        }

        #region LINEAR TRANSLATIONAL COMPOSED PATTERN
        public static bool GetComposedPatternsFromPathLine_Assembly(MyPathOfPoints currentPathOfCentroids, 
            List<MyPatternOfComponents> listOfParallelPatterns, ref List<MyPathOfPoints> listOfPathsOfCentroids,
            ref List<MyMatrAdj> listOfMyMatrAdj,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo)
        {
            var numOfPatterns = currentPathOfCentroids.path.Count;
            var noStop = true;

           var listOfPatternOnThePath = currentPathOfCentroids.path.Select(ind => listOfParallelPatterns[ind]).ToList();

            var i = 0;
            while (i < (numOfPatterns - 1))
            {
                var newComposedPattern = new MyComposedPatternOfComponents();
                var foundNewComposedPattern = GetMaximumTranslation_Assembly_ComposedPatterns(listOfPatternOnThePath, 
                    currentPathOfCentroids.pathGeometricObject, ref i,
                    ref numOfPatterns, ref noStop, ref newComposedPattern);

                if (foundNewComposedPattern)
                {
                    if (newComposedPattern.ListOfMyPatternOfComponents.Count == numOfPatterns ||
                        newComposedPattern.ListOfMyPatternOfComponents.Count == numOfPatterns - 1)
                    {
                        noStop = true;
                    }

                    GeometryAnalysis.CheckAndUpdate_Assembly_ComposedPatterns(newComposedPattern, ref listOfPathsOfCentroids,
                        listOfParallelPatterns, ref listOfMyMatrAdj,
                        ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo);
                }
            }
         
            if (noStop)
            {
                return true;
            }
            return false;
        }

        public static bool GetMaximumTranslation_Assembly_ComposedPatterns(List<MyPatternOfComponents> listOfPatternOnThePath, 
            MyPathGeometricObject pathObject, ref int i, ref int numOfPatterns, ref bool noStop, 
            ref MyComposedPatternOfComponents outputComposedPattern)
        {
            var listOfPatternsOfNew = new List<MyPatternOfComponents> { listOfPatternOnThePath[i] };
            var lengthOfNew = 1; 
            var exit = false;

            while (i < (numOfPatterns - 1) && exit == false)
            {

                if (IsTranslationTwoPatternsOfComponents(listOfPatternOnThePath[i], listOfPatternOnThePath[i + 1]))
                {
                    listOfPatternsOfNew.Add(listOfPatternOnThePath[i + 1]);
                    lengthOfNew += 1;
                    
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    i++;
                }
            }

            if (lengthOfNew > 1)
            {
                outputComposedPattern.ListOfMyPatternOfComponents = listOfPatternsOfNew;
                outputComposedPattern.pathOfMyComposedPatternOfComponents = pathObject;

                outputComposedPattern.typeOfMyComposedPatternOfComponents = "(linear) TRANSLATION";

                outputComposedPattern.constStepOfMyComposedPatternOfComponents = 
                    listOfPatternsOfNew[0].patternCentroid.Distance(
                    listOfPatternsOfNew[1].patternCentroid);
                return true;
            }

            return false;

        }

        //This function verifies if two given patterns are related by translation, with the difference
        //vector between the two pattern centroids as candidate translational vector.
        public static bool IsTranslationTwoPatternsOfComponents(MyPatternOfComponents firstMyPattern,
            MyPatternOfComponents secondMyPattern)
        {
            const string nameFile = "ComposedPatterns_Linear.txt";

            //check of the length correspondence:
            var firstPatternLength = firstMyPattern.listOfMyRCOfMyPattern.Count;
            var secondPatternLength = secondMyPattern.listOfMyRCOfMyPattern.Count;
            if (firstPatternLength != secondPatternLength)
            {
                return false;
            }           


            if (AssemblyUtilities.GeometryAnalysis.IsTranslationTwoRC(firstMyPattern.listOfMyRCOfMyPattern[0],
                secondMyPattern.listOfMyRCOfMyPattern[0]) ||
                AssemblyUtilities.GeometryAnalysis.IsTranslationTwoRC(firstMyPattern.listOfMyRCOfMyPattern[0],
                secondMyPattern.listOfMyRCOfMyPattern[secondPatternLength - 1]))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region CIRCULAR ROTATIONAL COMPOSED PATTERN
        public static bool GetComposedPatternsFromPathCircum_Assembly(MyPathOfPoints currentPathOfCentroids,
            List<MyPatternOfComponents> listOfCoherentPatterns, ref List<MyPathOfPoints> listOfPathsOfCentroids,
            ref List<MyMatrAdj> listOfMyMatrAdj, 
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            ref List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, 
            SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            var numOfPatterns = currentPathOfCentroids.path.Count;
            var noStop = true;

            var listOfPatternOnThePath = currentPathOfCentroids.path.Select(ind => listOfCoherentPatterns[ind]).ToList();
            var pathCircumference = (MyCircumForPath)currentPathOfCentroids.pathGeometricObject;

            var i = 0;
            while (i < (numOfPatterns - 1))
            {
                var newComposedPattern = new MyComposedPatternOfComponents();
                var foundNewComposedPattern = GetMaximumRotation_Assembly_ComposedPatterns(listOfPatternOnThePath,
                    pathCircumference, ref i, ref numOfPatterns, ref noStop, ref newComposedPattern, SwApplication, ref fileOutput);

                if (foundNewComposedPattern)
                {
                    if (newComposedPattern.ListOfMyPatternOfComponents.Count == numOfPatterns ||
                        newComposedPattern.ListOfMyPatternOfComponents.Count == numOfPatterns - 1)
                    {
                        noStop = true;
                    }

                    GeometryAnalysis.CheckAndUpdate_Assembly_ComposedPatterns(newComposedPattern, ref listOfPathsOfCentroids,
                        listOfCoherentPatterns, ref listOfMyMatrAdj,
                        ref listOfOutputComposedPattern, ref listOfOutputComposedPatternTwo);

                }
            }
            
            if (noStop)
            {
                return true;
            }
            return false;
        }


        public static bool GetMaximumRotation_Assembly_ComposedPatterns(List<MyPatternOfComponents> listOfPatternOnThePath,
            MyCircumForPath pathObject, ref int i, ref int numOfPatterns, ref bool noStop,
            ref MyComposedPatternOfComponents outputComposedPattern, SldWorks SwApplication, ref StringBuilder fileOutput)
        {
            var listOfPatternsOfNew = new List<MyPatternOfComponents> { listOfPatternOnThePath[i] };
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

            var axisDirection = Part.PartUtilities.GeometryAnalysis.establishAxisDirection(listOfPatternOnThePath[0].patternCentroid,
                listOfPatternOnThePath[1].patternCentroid,
                pathObject.circumcenter, planeNormal);

            while (i < (numOfPatterns - 1) && exit == false)
            {
                
                if (IsRotationTwoPatterns_Assembly(listOfPatternOnThePath[i], listOfPatternOnThePath[i + 1], teta, axisDirection,
                    SwApplication, ref fileOutput))
                {
                    listOfPatternsOfNew.Add(listOfPatternOnThePath[i + 1]);
                    lengthOfNew += 1;
                    
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    i++;
                }
            }

            if (lengthOfNew > 1)
            {
                outputComposedPattern.ListOfMyPatternOfComponents = listOfPatternsOfNew;
                outputComposedPattern.pathOfMyComposedPatternOfComponents = pathObject;

                outputComposedPattern.typeOfMyComposedPatternOfComponents = "ROTATION";
                
                outputComposedPattern.constStepOfMyComposedPatternOfComponents = listOfPatternsOfNew[0].patternCentroid.Distance(
                    listOfPatternsOfNew[1].patternCentroid);
                return true;
            }

            return false;
        }

        public static bool IsRotationTwoPatterns_Assembly(MyPatternOfComponents firstMyPattern, 
            MyPatternOfComponents secondMyPattern, double teta,
            double[] axisDirection, 
            SldWorks SwApplication, ref StringBuilder fileOutput)
        {

            //check of the length correspondence:
            var firstPatternLength = firstMyPattern.listOfMyRCOfMyPattern.Count;
            var secondPatternLength = secondMyPattern.listOfMyRCOfMyPattern.Count;
            if (firstPatternLength != secondPatternLength)
            {
                return false;
            }

            if (AssemblyUtilities.GeometryAnalysis.IsRotationTwoComp_Assembly(firstMyPattern.listOfMyRCOfMyPattern[0],
                secondMyPattern.listOfMyRCOfMyPattern[0], teta, axisDirection) ||
                AssemblyUtilities.GeometryAnalysis.IsRotationTwoComp_Assembly(firstMyPattern.listOfMyRCOfMyPattern[0],
                secondMyPattern.listOfMyRCOfMyPattern[secondPatternLength - 1], teta, axisDirection))
            {
                return true;
            }
            return false;
        }
        #endregion     
    }
}
