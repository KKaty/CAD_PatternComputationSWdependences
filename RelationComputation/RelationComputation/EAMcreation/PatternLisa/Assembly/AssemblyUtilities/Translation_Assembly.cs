using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {

        public static bool GetMaximumTranslation_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath, MyPathGeometricObject pathObject,
            ref int i, ref int numOfCompOnThisPath, ref bool noStop, ref MyPatternOfComponents outputPattern)
        {

            var listOfComponentsOfNewMyPattern = new List<MyRepeatedComponent> { listOfComponentsOnThePath[i] };
            var lengthOfCurrentPath = listOfComponentsOfNewMyPattern.Count; 
            var exit = false;

            while (i < (numOfCompOnThisPath - 1) && exit == false) 
            {

                if (IsTranslationTwoRC(listOfComponentsOnThePath[i], listOfComponentsOnThePath[i + 1]))
                {
                    listOfComponentsOfNewMyPattern.Add(listOfComponentsOnThePath[i + 1]);
                    lengthOfCurrentPath += 1;
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    i++;
                }
            }
            
            if (lengthOfCurrentPath > 1)
            {
                outputPattern.listOfMyRCOfMyPattern = listOfComponentsOfNewMyPattern;
                outputPattern.pathOfMyPattern = pathObject;
                if (pathObject.GetType() == typeof(MyLine))
                {
                    outputPattern.typeOfMyPattern = "linear TRANSLATION";
                }
                else
                {
                    outputPattern.typeOfMyPattern = "circular TRANSLATION";
                }
                outputPattern.constStepOfMyPattern = listOfComponentsOfNewMyPattern[0].Origin.Distance(
                    listOfComponentsOfNewMyPattern[1].Origin);
                return true;
            }

            return false;

        }


        public static bool KLGetMaximumTranslation_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath, MyPathGeometricObject pathObject,
           ref int i, ref int numOfCompOnThisPath, ref bool noStop, ref MyPatternOfComponents outputPattern, SldWorks swApplication)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";

            var listOfREOnThePath = new List<MyRepeatedEntity>(listOfComponentsOnThePath.Select(comp => comp.RepeatedEntity));

            var listOfREOfNewMyPattern = new List<MyRepeatedEntity> { listOfREOnThePath[i] };
            var listOfComponetsOfNewMyPattern = new List<MyRepeatedComponent> { listOfComponentsOnThePath[i] };

            var lengthOfCurrentPath = listOfREOfNewMyPattern.Count;   // = number of couple possibly related by translation (= number of repetition of distance d) 
            var exit = false;

            while (i < (numOfCompOnThisPath - 1) && exit == false) //fino alla penultima RE
            {
                if(true)
               //if (Part.PartUtilities.GeometryAnalysis.IsTranslationTwoRE(listOfREOnThePath[i], listOfREOnThePath[i + 1]))
                {
                    listOfREOfNewMyPattern.Add(listOfREOnThePath[i + 1]);
                    listOfComponetsOfNewMyPattern.Add(listOfComponentsOnThePath[i+1]);
                    lengthOfCurrentPath += 1;
                    //KLdebug.Print("Aggiunta " + (i + 1) + "-esima COMP al MYPattern. lengthOfCurrentPath = " + listOfComponetsOfNewMyPattern.Count, nameFile);
                    i++;
                }
                else
                {
                    exit = true;
                    noStop = false;

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("------>>> Interruzione alla posizione: " + i, nameFile);
                    //KLdebug.Print(" ", nameFile);

                    i++;
                } 
            }

            if (lengthOfCurrentPath > 1)
            {
                outputPattern.listOfMyRCOfMyPattern = listOfComponetsOfNewMyPattern;
                outputPattern.pathOfMyPattern = pathObject;
                
                if (pathObject.GetType() == typeof(MyLine))
                {
                    outputPattern.typeOfMyPattern = "linear TRANSLATION";
                    //KLdebug.Print("CREATO PATTERN TRANS lineare DI LUNGHEZZA = " + listOfREOfNewMyPattern.Count, nameFile);

                }
                else
                {
                    outputPattern.typeOfMyPattern = "circular TRANSLATION";
                    //KLdebug.Print("CREATO PATTERN TRANS circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);
                    var teta = FunctionsLC.FindAngle(listOfREOfNewMyPattern[0].centroid,
                        listOfREOfNewMyPattern[1].centroid, ((MyCircumForPath)pathObject).circumcenter);
                    outputPattern.angle = teta;
                }
                outputPattern.constStepOfMyPattern = listOfREOfNewMyPattern[0].centroid.Distance(listOfREOfNewMyPattern[1].centroid);
                return true;
            }
            
            return false;
       }



        //It verifies if a symmetry TRANSLATIONAL relation of translational vector between two MyRepeatedComponent
        public static bool IsTranslationTwoRC(MyRepeatedComponent firstComponent,
            MyRepeatedComponent secondComponent)
        {
            int i = 0;

            //for (var j = 0; j < 3; j++)
            //{
            //    double[] firstVector =
            //    {
            //        firstComponent.Transform.RotationMatrix[0, j],
            //        firstComponent.Transform.RotationMatrix[1, j], 
            //        firstComponent.Transform.RotationMatrix[2, j]
            //    };
            //    double[] secondVector =
            //    {
            //        secondComponent.Transform.RotationMatrix[0, j],
            //        secondComponent.Transform.RotationMatrix[1, j], 
            //        secondComponent.Transform.RotationMatrix[2, j]
            //    };

            //    whatToWrite = string.Format("Versore 1^ componente: ({0},{1},{2})", firstVector[0], firstVector[1], firstVector[2]);
            //    KLdebug.Print(whatToWrite, nameFile);
            //    whatToWrite = string.Format("Versore 2^ componente: ({0},{1},{2})", secondVector[0], secondVector[1], secondVector[2]);
            //    KLdebug.Print(whatToWrite, nameFile);
            //    KLdebug.Print("", nameFile);

            //}

            while (i < 3) 
            {
                double[] firstVector =
                {
                    firstComponent.Transform.RotationMatrix[0, i],
                    firstComponent.Transform.RotationMatrix[1, i], 
                    firstComponent.Transform.RotationMatrix[2, i]
                };
                double[] secondVector =
                {
                    secondComponent.Transform.RotationMatrix[0, i],
                    secondComponent.Transform.RotationMatrix[1, i], 
                    secondComponent.Transform.RotationMatrix[2, i]
                };
               
              
                if (FunctionsLC.MyEqualsArray(firstVector, secondVector))
                {
                    i++;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}
 