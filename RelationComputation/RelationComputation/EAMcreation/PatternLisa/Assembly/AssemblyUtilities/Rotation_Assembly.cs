using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {

        //It detect the maximum symmetry relation in a set of MyRepeatedEntity, starting from index i of the list of MyRE
        public static bool GetMaximumRotation_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
            MyCircumForPath pathObject,
            ref int i, ref int numOfComp, ref bool noStop, ref MyPatternOfComponents outputPattern, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE", nameFile);
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            //var newMyPattern = new MyPattern();

            var listOfCompOfNewMyPattern = new List<MyRepeatedComponent> {listOfComponentsOnThePath[i]};
            var lengthOfCurrentPath = 1; // = it represents the number of couples related by translation
            var exit = false;

            //Computation of the rotation angle:            
            double[] planeNormal =
            {
                pathObject.circumplane.a,
                pathObject.circumplane.b,
                pathObject.circumplane.c
            };

            var teta = FunctionsLC.FindAngle(listOfComponentsOnThePath[0].Origin, listOfComponentsOnThePath[1].Origin,
                pathObject.circumcenter);
            //KLdebug.Print("   --->>> TETA = " + teta, nameFile);


            var axisDirection =
                Part.PartUtilities.GeometryAnalysis.establishAxisDirection(listOfComponentsOnThePath[0].Origin,
                    listOfComponentsOnThePath[1].Origin,
                    pathObject.circumcenter, planeNormal);

            while (i < (numOfComp - 1) && exit == false) //fino alla penultima Comp
            {
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print(" ", nameFile);
                //whatToWrite = string.Format("         Confronto {0}^ COMP e la {1}^ COMP: ", i, i + 1);
                //KLdebug.Print(whatToWrite, nameFile);

                //KLdebug.PrintTransformMatrix(listOfComponentsOnThePath[i].Transform, nameFile, SwApplication);
                //KLdebug.PrintTransformMatrix(listOfComponentsOnThePath[i + 1].Transform, nameFile, SwApplication);

                if (IsRotationTwoComp_Assembly(listOfComponentsOnThePath[i], listOfComponentsOnThePath[i + 1], teta, axisDirection))
                {
                    listOfCompOfNewMyPattern.Add(listOfComponentsOnThePath[i + 1]);
                    lengthOfCurrentPath += 1;
                    //KLdebug.Print(
                    //    "Aggiunta " + (i + 1) + "-esima COMP al MYPattern. lengthOfCurrentPath = " + lengthOfCurrentPath,
                    //    nameFile);
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
            //KLdebug.Print(" ", nameFile);

            if (lengthOfCurrentPath > 1)
            {
                outputPattern.listOfMyRCOfMyPattern = listOfCompOfNewMyPattern;
                outputPattern.pathOfMyPattern = pathObject;
                //KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);

                outputPattern.typeOfMyPattern = "ROTATION";
                outputPattern.angle = teta;
               // KLdebug.Print("CREATO PATTERN Rotazionale circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);
                return true;
            }
            //KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
            //KLdebug.Print("IL PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            //KLdebug.Print(" ", nameFile);
            return false;

        }

        public static bool KLGetMaximumRotation_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
           MyCircumForPath pathObject,
           ref int i, ref int numOfComp, ref bool noStop, ref MyPatternOfComponents outputPattern, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE", nameFile);
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            var listOfREOnThePath =
                new List<MyRepeatedEntity>(listOfComponentsOnThePath.Select(comp => comp.RepeatedEntity));

            var listOfREOfNewMyPattern = new List<MyRepeatedEntity> { listOfREOnThePath[i] };
            var listOfComponetsOfNewMyPattern = new List<MyRepeatedComponent> { listOfComponentsOnThePath[i] };

            var lengthOfCurrentPath = 1;   // = it represents the number of couples related by translation
            var exit = false;

            if (pathObject.circumplane == null)
            {
                exit = true;
                noStop = false;
                i++;
                return false;
            }
            //Computation of the rotation angle:
            double[] planeNormal =
            {
                pathObject.circumplane.a, 
                pathObject.circumplane.b,
                pathObject.circumplane.c
            };
            var teta = FunctionsLC.FindAngle(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid, pathObject.circumcenter);

            var axisDirection = Part.PartUtilities.GeometryAnalysis.establishAxisDirection(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid,
                pathObject.circumcenter, planeNormal);

            while (i < (numOfComp - 1) && exit == false) //fino alla penultima RE
            {
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print(" ", nameFile);
                //whatToWrite = string.Format("         Confronto {0}^ RE e la {1}^ RE: ", i, i + 1);
                //KLdebug.Print(whatToWrite, nameFile);

                if (true)
                //if (Part.PartUtilities.GeometryAnalysis.IsRotationTwoRE(listOfREOnThePath[i], listOfREOnThePath[i + 1], teta, axisDirection, pathObject.circumcenter))
                {
                    listOfREOfNewMyPattern.Add(listOfREOnThePath[i + 1]);
                    listOfComponetsOfNewMyPattern.Add(listOfComponentsOnThePath[i + 1]);
                    lengthOfCurrentPath += 1;
                    //KLdebug.Print("Aggiunta " + (i + 1) + "-esima RE al MYPattern. lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
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
           // KLdebug.Print(" ", nameFile);

            if (lengthOfCurrentPath > 1)
            {
                outputPattern.listOfMyRCOfMyPattern = listOfComponetsOfNewMyPattern;

                outputPattern.listOfMyRCOfMyPattern = listOfComponentsOnThePath;
                outputPattern.pathOfMyPattern = pathObject;
              
                outputPattern.typeOfMyPattern = "ROTATION";
                outputPattern.constStepOfMyPattern = listOfREOfNewMyPattern[0].centroid.Distance(listOfREOfNewMyPattern[1].centroid);
                //KLdebug.Print("CREATO PATTERN Rotazionale circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);

                outputPattern.angle = teta;
                return true;
            }
            //KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
            //KLdebug.Print("IL PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            //KLdebug.Print(" ", nameFile);
            return false;

            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE", nameFile);
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            ////var newMyPattern = new MyPattern();

            //var listOfCompOfNewMyPattern = new List<MyRepeatedComponent> { listOfComponentsOnThePath[i] };
            //var lengthOfCurrentPath = 1; // = it represents the number of couples related by translation
            //var exit = false;

            ////Computation of the rotation angle:            
            //double[] planeNormal =
            //{
            //    pathObject.circumplane.a,
            //    pathObject.circumplane.b,
            //    pathObject.circumplane.c
            //};

            //var teta = FunctionsLC.FindAngle(listOfComponentsOnThePath[0].Origin, listOfComponentsOnThePath[1].Origin,
            //    pathObject.circumcenter);
            //KLdebug.Print("   --->>> TETA = " + teta, nameFile);


            //var axisDirection =
            //    BRepUtilities.GeometryAnalysis.establishAxisDirection(listOfComponentsOnThePath[0].Origin,
            //        listOfComponentsOnThePath[1].Origin,
            //        pathObject.circumcenter, planeNormal);

            //while (i < (numOfComp - 1) && exit == false) //fino alla penultima Comp
            //{
            //    KLdebug.Print(" ", nameFile);
            //    KLdebug.Print(" ", nameFile);
            //    whatToWrite = string.Format("         Confronto {0}^ COMP e la {1}^ COMP: ", i, i + 1);
            //    KLdebug.Print(whatToWrite, nameFile);

            //    KLdebug.PrintTransformMatrix(listOfComponentsOnThePath[i].Transform, nameFile, SwApplication);
            //    KLdebug.PrintTransformMatrix(listOfComponentsOnThePath[i + 1].Transform, nameFile, SwApplication);

            //    if (IsRotationTwoComp_Assembly(listOfComponentsOnThePath[i], listOfComponentsOnThePath[i + 1], teta, axisDirection))
            //    {
            //        listOfCompOfNewMyPattern.Add(listOfComponentsOnThePath[i + 1]);
            //        lengthOfCurrentPath += 1;
            //        KLdebug.Print(
            //            "Aggiunta " + (i + 1) + "-esima COMP al MYPattern. lengthOfCurrentPath = " + lengthOfCurrentPath,
            //            nameFile);
            //        i++;
            //    }
            //    else
            //    {
            //        exit = true;
            //        noStop = false;

            //        KLdebug.Print(" ", nameFile);
            //        KLdebug.Print("------>>> Interruzione alla posizione: " + i, nameFile);
            //        KLdebug.Print(" ", nameFile);

            //        i++;
            //    }
            //}
            //KLdebug.Print(" ", nameFile);

            //if (lengthOfCurrentPath > 1)
            //{
            //    outputPattern.listOfMyRCOfMyPattern = listOfCompOfNewMyPattern;
            //    outputPattern.pathOfMyPattern = pathObject;
            //    KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);

            //    outputPattern.typeOfMyPattern = "ROTATION";
            //    outputPattern.angle = teta;
            //    KLdebug.Print("CREATO PATTERN Rotazionale circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);
            //    return true;
            //}
            //KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
            //KLdebug.Print("IL PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            //KLdebug.Print(" ", nameFile);
            //return false;

        }

        public static bool IsRotationTwoComp_Assembly(MyRepeatedComponent firstComponent,
            MyRepeatedComponent secondComponent, double teta, double[] axisDirection)
        {
            const string nameFile = "GetRotationalPatterns.txt";
            int i = 0;

            while (i < 3)
            {
                var firstVector = new MyVertex(
                    //firstComponent.Transform.RotationMatrix[i, 0],
                    //firstComponent.Transform.RotationMatrix[i, 1],
                    //firstComponent.Transform.RotationMatrix[i, 2]
                    firstComponent.Transform.RotationMatrix[0, i],
                    firstComponent.Transform.RotationMatrix[1, i],
                    firstComponent.Transform.RotationMatrix[2, i]
                );
                var secondVector = new MyVertex(
                    //secondComponent.Transform.RotationMatrix[i, 0],
                    //secondComponent.Transform.RotationMatrix[i, 1],
                    //secondComponent.Transform.RotationMatrix[i, 2]
                    secondComponent.Transform.RotationMatrix[0, i],
                    secondComponent.Transform.RotationMatrix[1, i],
                    secondComponent.Transform.RotationMatrix[2, i]
                );

                

                if (secondVector.IsRotationOf(firstVector, teta, axisDirection))
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

        public static bool IsRotationTwoComp180degrees_Assembly(MyRepeatedComponent firstComponent,
            MyRepeatedComponent secondComponent, out double[] axisDirectionVersor, ModelDoc2 SwModel, SldWorks SwApplication)
        {

            //Rotation axis computation:
            //we need to find 2 vertices for each of the 2 components: 
            //one is the origin, the second is a chosen point/versor from x,y, or z.
            //Then we need to compute the mean point between the origins 
            //and the mean point between the chosen versors.
            //The rotation axis corresponds to the line passing through the two mean points.

            var originMeanPoint = new double[]{(firstComponent.Origin.x + secondComponent.Origin.x) / 2, 
                (firstComponent.Origin.y + secondComponent.Origin.y) / 2, 
                (firstComponent.Origin.z + secondComponent.Origin.z) / 2};
            
            //SwModel = (ModelDoc2)SwApplication.ActiveDoc;
            //SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();
            //SwModel.CreatePoint2(originMeanPoint[0], originMeanPoint[1], originMeanPoint[2]);
            //SwModel.InsertSketch();

            var xVersorCorrespondingPointFirst = new double[] {
                firstComponent.Origin.x + firstComponent.Transform.RotationMatrix[0, 0],
                firstComponent.Origin.y + firstComponent.Transform.RotationMatrix[1, 0],
                firstComponent.Origin.z + firstComponent.Transform.RotationMatrix[2, 0]
            };

            var xVersorCorrespondingPointSecond = new double[] {
                secondComponent.Origin.x + secondComponent.Transform.RotationMatrix[0, 0],
                secondComponent.Origin.y + secondComponent.Transform.RotationMatrix[1, 0],
                secondComponent.Origin.z + secondComponent.Transform.RotationMatrix[2, 0]
            };

            var versorMeanPoint = new double[]{(xVersorCorrespondingPointFirst[0] + xVersorCorrespondingPointSecond[0]) / 2,
                (xVersorCorrespondingPointFirst[1] + xVersorCorrespondingPointSecond[1]) / 2,
                (xVersorCorrespondingPointFirst[2] + xVersorCorrespondingPointSecond[2]) / 2};

            // if the mean point of the origins and the mean point of the x versors correspond
            // I compute the mean point referred to the y coordinate versors:
            var i = 0;
            while (FunctionsLC.MyEqualsArray(versorMeanPoint, originMeanPoint) && i<2)
            {
                i++;

                xVersorCorrespondingPointFirst.SetValue(firstComponent.Origin.x + firstComponent.Transform.RotationMatrix[0, i],0);
                xVersorCorrespondingPointFirst.SetValue(firstComponent.Origin.y + firstComponent.Transform.RotationMatrix[1, i],1);
                xVersorCorrespondingPointFirst.SetValue(firstComponent.Origin.z + firstComponent.Transform.RotationMatrix[2, i],2);

                xVersorCorrespondingPointSecond.SetValue(secondComponent.Origin.x + secondComponent.Transform.RotationMatrix[0, i],0);
                xVersorCorrespondingPointSecond.SetValue(secondComponent.Origin.y + secondComponent.Transform.RotationMatrix[1, i],1);
                xVersorCorrespondingPointSecond.SetValue(secondComponent.Origin.z + secondComponent.Transform.RotationMatrix[2, i],2);

                versorMeanPoint.SetValue((xVersorCorrespondingPointFirst[0] + xVersorCorrespondingPointSecond[0]) / 2,0);
                versorMeanPoint.SetValue((xVersorCorrespondingPointFirst[1] + xVersorCorrespondingPointSecond[1]) / 2,1);
                versorMeanPoint.SetValue((xVersorCorrespondingPointFirst[2] + xVersorCorrespondingPointSecond[2]) / 2,2);
            };
            //SwModel = (ModelDoc2)SwApplication.ActiveDoc;
            //SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();
            //SwModel.CreatePoint2(versorMeanPoint[0], versorMeanPoint[1], versorMeanPoint[2]);
            //SwModel.InsertSketch();

            var axisDirectionVector = new double[] { originMeanPoint[0] - versorMeanPoint[0], originMeanPoint[1] - versorMeanPoint[1], 
                originMeanPoint[2] - versorMeanPoint[2] };
          
            axisDirectionVersor = GeometricUtilities.FunctionsLC.Normalize(axisDirectionVector);
            var teta = Math.PI; // the angle is fixed at 180°

            // Now i got the direction axis and the the rotation angle: I use the existing verifying function
            return IsRotationTwoComp_Assembly(firstComponent, secondComponent, teta, axisDirectionVersor);
        }
    }
}