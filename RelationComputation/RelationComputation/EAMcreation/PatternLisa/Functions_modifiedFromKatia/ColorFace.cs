using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia
{
    public class ColorFace
    {
        public static void KLColorFace(List<Face2> faceList, SldWorks mySwApplication, double t)
        {
            ModelDoc2 myModel = mySwApplication.ActiveDoc;

            double colorRgb1 = t; //red
            double colorRgb2 = 1-t; //green
            double colorRgb3 = 0; //blue

            foreach (Face2 face in faceList)
            {
                var faceMaterial = (Array)face.MaterialPropertyValues;

                if (faceMaterial == null)
                {
                    faceMaterial = (Array)myModel.MaterialPropertyValues;
                }

                faceMaterial.SetValue(colorRgb1, 0);
                faceMaterial.SetValue(colorRgb2, 1);
                faceMaterial.SetValue(colorRgb3, 2);

                face.MaterialPropertyValues = faceMaterial;
                //FolderUtilities.PrintToFile(idNode.ToString(), "colorFace.txt");

            }

        }

        public static void KLColorFace(List<Face2> faceList, SldWorks mySwApplication)
        {
            ModelDoc2 myModel = mySwApplication.ActiveDoc;

            double colorRgb1 = 0; //red
            double colorRgb2 = 1; //green
            double colorRgb3 = 0; //blue

            foreach (Face2 face in faceList)
            {
                var faceMaterial = (Array)face.MaterialPropertyValues;

                if (faceMaterial == null)
                {
                    faceMaterial = (Array)myModel.MaterialPropertyValues;
                }

                faceMaterial.SetValue(colorRgb1, 0);
                faceMaterial.SetValue(colorRgb2, 1);
                faceMaterial.SetValue(colorRgb3, 2);

                face.MaterialPropertyValues = faceMaterial;
                //FolderUtilities.PrintToFile(idNode.ToString(), "colorFace.txt");

            }

        }

        //Given a MyPattern, it colors the MyPattern faces.
        //The second part draw lines and circle on the model (not very ok)
        public static void MyVisualOutput(List<MyPattern> myOutPatterns, SldWorks mySwApplication)
        {
            ModelDoc2 myModel = mySwApplication.ActiveDoc;

            double colorRgb1 = 0;  //red
            double colorRgb2 = 0;  //green
            double colorRgb3 = 0;  //blue

            if (myOutPatterns.Count > 0)
            {
                //mySwApplication.SendMsgToUser("num: " + myOutPatterns.Count);
                double step = 1.0 / myOutPatterns.Count;
                //mySwApplication.SendMsgToUser("step: " + step);

                // If I am in the list of path LINE, I color with the RED range
                if (myOutPatterns[0].pathOfMyPattern.GetType() == typeof(MyLine))
                {
                    foreach (MyPattern pattern in myOutPatterns)
                    {
                        colorRgb1 = step * (myOutPatterns.IndexOf(pattern) + 1);

                        //mySwApplication.SendMsgToUser("COLORE: " + colorRgb1);

                        foreach (MyRepeatedEntity repeatedEntity in pattern.listOfMyREOfMyPattern)
                        {
                            foreach (Face2 face in repeatedEntity.listOfFaces)
                            {
                                var faceMaterial = (Array)face.MaterialPropertyValues;

                                if (faceMaterial == null)
                                {
                                    faceMaterial = (Array)myModel.MaterialPropertyValues;
                                }

                                faceMaterial.SetValue(colorRgb1, 0);
                                faceMaterial.SetValue(colorRgb2, 1);
                                faceMaterial.SetValue(colorRgb3, 2);

                                face.MaterialPropertyValues = faceMaterial;
                                //FolderUtilities.PrintToFile(idNode.ToString(), "colorFace.txt");

                            }
                        }
                    }

                }
                // If I am in the list of path CIRCUMFERENCE, I color with the GREEN range
                else
                {
                    foreach (MyPattern pattern in myOutPatterns)
                    {
                        colorRgb2 = step * (myOutPatterns.IndexOf(pattern) + 1);
                        //mySwApplication.SendMsgToUser("COLORE: " + colorRgb2);

                        foreach (MyRepeatedEntity repeatedEntity in pattern.listOfMyREOfMyPattern)
                        {
                            foreach (Face2 face in repeatedEntity.listOfFaces)
                            {
                                var faceMaterial = (Array)face.MaterialPropertyValues;

                                if (faceMaterial == null)
                                {
                                    faceMaterial = (Array)myModel.MaterialPropertyValues;
                                }

                                faceMaterial.SetValue(colorRgb1, 0);
                                faceMaterial.SetValue(colorRgb2, 1);
                                faceMaterial.SetValue(colorRgb3, 2);

                                face.MaterialPropertyValues = faceMaterial;
                                //FolderUtilities.PrintToFile(idNode.ToString(), "colorFace.txt");

                            }
                        }
                    }
                }

            }

            #region PROVE
            //myModel.Insert3DSketch();
            //myModel.CreateLine2(0, 0, 0, 0.01, 0.01, 0.01);
            //myModel.InsertSketch();
            //myModel.ClearSelection2(true);



            ////myModel.Insert3DSketch();
            ////myModel.CreateCircleByRadius2(0, 0, 0, 0.01);
            ////myModel.InsertSketch();
            ////myModel.ClearSelection2(true);

            


            //var sketchManager = (SketchManager)myModel.SketchManager;
            //mySwApplication.SendMsgToUser("Pausa");
            //sketchManager.Insert3DSketch(true);
            //myModel.ClearSelection2(true);

            //var sketch = (Sketch)sketchManager.ActiveSketch;
            //mySwApplication.SendMsgToUser("Pausa");
            //if (sketch.SetWorkingPlaneOrientation(0, 0, 0,
            //    0, 1, 0,
            //    0, 0, 1,
            //    1, 0, 0))
            //    //Non disegna la circonferenza richiesta sul piano settato!
            //    //In pratica crea lo schizzo 3D ma poi lo fa sparire dalla tendiana a sinistra.
            //    //Disegna invece la circonferenza sul piano def da:
            //    //(0, 0, 0,
            //    //1, 0, 0,
            //    //0, 0, 1,
            //    //0, 1, 0)... PERCHE QUESTO SI??
            //    //Disegna inoltre crf se non si setta il piano e si passa direttamente
            //    //a sketchManager.CreateCircleByRadius(0, 0, 0, 0.001).
            //{
            //    mySwApplication.SendMsgToUser("Sono TRUE");
            //}
            //else
            //{
            //    mySwApplication.SendMsgToUser("Sono FALSE");
            //}

            //mySwApplication.SendMsgToUser("Pausa");
            //sketchManager.CreateCircleByRadius(0, 0, 0, 0.001);
            
            //mySwApplication.SendMsgToUser("Pausa");
            //myModel.ClearSelection2(true);
            //myModel.InsertSketch();
            //myModel.ClearSelection2(true);
            #endregion
        }

        public static void MyVisualOutput_ComposedPatterns(List<MyComposedPattern> listOfOutputComposedPattern,
            List<MyComposedPattern> listOfOutputComposedPatternTwo, SldWorks mySwApplication, ModelDoc2 SwModel)
        {

            SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();
            
            int nOfPatterns = 0;
            var tolerance = Math.Pow(10, -4);
            MyVertex headCentroidPattern;
            MyVertex backCentroidPattern;
            foreach (var composedPattern in listOfOutputComposedPattern)
            {
                nOfPatterns = composedPattern.listOfMyPattern.Count;
                headCentroidPattern = composedPattern.listOfMyPattern[0].patternCentroid;
                backCentroidPattern = composedPattern.listOfMyPattern[nOfPatterns - 1].patternCentroid;
                if (composedPattern.pathOfMyComposedPattern.GetType() == typeof(MyLine))
                {
                    SwModel.GetActiveSketch2();
                    SwModel.CreateLine2(headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                        backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z);
                }
                else //typeof(MyCircumForPath)
                {
                    var pathObject = (MyCircumForPath)composedPattern.pathOfMyComposedPattern;
                    var circumCenter = pathObject.circumcenter;
                    //SwModel.CreateArc2(pathObject.circumcenter.x, pathObject.circumcenter.y, pathObject.circumcenter.z, 
                    //    headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                    //    backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z, -1);   //o +1?

                    if (
                        Math.Abs(headCentroidPattern.Distance(backCentroidPattern) -
                                 composedPattern.constStepOfMyComposedPattern) < tolerance)
                    {
                        //SwModel.ClearSelection2(true);
                        SwModel.Insert3DSketch();
                        SwModel.GetActiveSketch2();
                        SwModel.CreateCircle2(circumCenter.x, circumCenter.y, circumCenter.z,
                        headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z);
                        //SwModel.InsertSketch();
                    }
                    else
                    {
                        //SwModel.ClearSelection2(true);
                        //SwModel.Insert3DSketch();
                        SwModel.GetActiveSketch2();
                        SwModel.Create3PointArc(headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                            backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z,
                            composedPattern.listOfMyPattern[1].patternCentroid.x,
                            composedPattern.listOfMyPattern[1].patternCentroid.y,
                            composedPattern.listOfMyPattern[1].patternCentroid.z);
                        //SwModel.InsertSketch();
                    }                  
                }
            }

            foreach (var composedPattern in listOfOutputComposedPatternTwo)
            {
                headCentroidPattern = composedPattern.listOfMyPattern[0].patternCentroid;
                backCentroidPattern = composedPattern.listOfMyPattern[1].patternCentroid;
                SwModel.GetActiveSketch2();
                SwModel.CreateLine2(headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                    backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z);
                
            }
            SwModel.InsertSketch();

            #region esempio linea funzionante
            //ModelDoc2 SwModel = mySwApplication.ActiveDoc;
            //SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();
            //SwModel.GetActiveSketch2();
            //SwModel.CreateLine2(0, 0, 0, 0, 0.1, 0.1);

            //SwModel.InsertSketch();
            #endregion
        }

        //Given a MyPattern, it colors the MyPattern faces.
        //The second part draw lines and circle on the model (not very ok)
        public static void MyVisualOutput_Assembly(List<MyPatternOfComponents> myOutPatterns, 
            SldWorks mySwApplication, ModelDoc2 SwModel)
        {
            ModelDoc2 myModel = mySwApplication.ActiveDoc;

            double colorRgb1 = 0; //red
            double colorRgb2 = 0; //green
            double colorRgb3 = 0; //blue

            if (myOutPatterns.Count > 0)
            {
                //mySwApplication.SendMsgToUser("num: " + myOutPatterns.Count);
                double step = 1.0/myOutPatterns.Count;
                double stepLinearTRANSLATION = 1.0 /
                                               (myOutPatterns.FindAll(
                                                   pattern => pattern.typeOfMyPattern == "linear TRANSLATION").ToList().Count +
                                                myOutPatterns.FindAll(
                                                   pattern => pattern.typeOfMyPattern == "TRANSLATION of length 2").ToList().Count);

                double stepLinearROTATION = 1.0 /
                                               (myOutPatterns.FindAll(
                                                   pattern => pattern.typeOfMyPattern == "linear ROTATION").ToList())
                                                   .Count;

                double stepcircularTRANSLATION = 1.0 /
                                               (myOutPatterns.FindAll(
                                                   pattern => pattern.typeOfMyPattern == "circular TRANSLATION").ToList())
                                                   .Count;

                double stepREFLECTION = 1.0 /
                                               (myOutPatterns.FindAll(
                                                   pattern => pattern.typeOfMyPattern == "REFLECTION").ToList())
                                                   .Count;
                //mySwApplication.SendMsgToUser("step: " + step);

                // If I am in the list of path LINE, I color with the RED range
               // if (myOutPatterns[0].pathOfMyPattern.GetType() == typeof (MyLine))
                //{
                    foreach (MyPatternOfComponents pattern in myOutPatterns)
                    {
                        colorRgb1 = 0; //red
                        colorRgb2 = 0; //green
                        colorRgb3 = 0; //blue
                        if (pattern.typeOfMyPattern == "linear TRANSLATION" || pattern.typeOfMyPattern == "TRANSLATION of length 2")
                        {
                            colorRgb1 = stepLinearTRANSLATION * (myOutPatterns.IndexOf(pattern) + 1);
                        }
                        else if (pattern.typeOfMyPattern == "linear ROTATION")
                        {
                            colorRgb2 = stepLinearROTATION * (myOutPatterns.IndexOf(pattern) + 1);
                        }
                          else if (pattern.typeOfMyPattern == "circular TRANSLATION")
                        {
                            colorRgb2 = stepcircularTRANSLATION * (myOutPatterns.IndexOf(pattern) + 1);
                        }
                        else if (pattern.typeOfMyPattern == "REFLECTION")
                        {
                            colorRgb3 = stepREFLECTION * (myOutPatterns.IndexOf(pattern) + 1);
                        }
                        //else if (pattern.typeOfMyPattern == "ROTATION")
                        //{
                        //    colorRgb1 = step * (myOutPatterns.IndexOf(pattern) + 0.5);
                        //    colorRgb3 = step * (myOutPatterns.IndexOf(pattern) + 0.5);
                        //}
                        else
                        {
                            colorRgb2 = (myOutPatterns.IndexOf(pattern) + 0.5);
                            colorRgb3 = (myOutPatterns.IndexOf(pattern) + 0.5);
                        }
                       // mySwApplication.SendMsgToUser("COLORE: " + colorRgb1);

                        foreach (var repeatedComponent in pattern.listOfMyRCOfMyPattern)
                        {

                            var currentComp = repeatedComponent.Component;
                            var componentMaterial = (Array) currentComp.MaterialPropertyValues;

                            if (componentMaterial == null)
                            {
                                componentMaterial = (Array) myModel.MaterialPropertyValues;
                            }

                            componentMaterial.SetValue(colorRgb1, 0);
                            componentMaterial.SetValue(colorRgb2, 1);
                            componentMaterial.SetValue(colorRgb3, 2);

                            repeatedComponent.Component.MaterialPropertyValues = componentMaterial;
                        }
                        
                     //   mySwApplication.SendMsgToUser("ADD pattern on LINE of type " + pattern.typeOfMyPattern);
                    }

                //}
                // If I am in the list of path CIRCUMFERENCE, I color with the GREEN range
               // else
                //{
                    //foreach (MyPatternOfComponents pattern in myOutPatterns)
                    //{
                    //    colorRgb2 = step*(myOutPatterns.IndexOf(pattern) + 1);
                    //    //mySwApplication.SendMsgToUser("COLORE: " + colorRgb2);

                    //    foreach (var repeatedComponent in pattern.listOfMyRCOfMyPattern)
                    //    {

                    //        var currentComp = repeatedComponent.Component;
                    //        var componentMaterial = (Array) currentComp.MaterialPropertyValues;

                    //        if (componentMaterial == null)
                    //        {
                    //            componentMaterial = (Array) myModel.MaterialPropertyValues;
                    //        }

                    //        componentMaterial.SetValue(colorRgb1, 0);
                    //        componentMaterial.SetValue(colorRgb2, 1);
                    //        componentMaterial.SetValue(colorRgb3, 2);

                    //        repeatedComponent.Component.MaterialPropertyValues = componentMaterial;


                    //    }
                       
                        //mySwApplication.SendMsgToUser("ADD pattern on CIRCUMFERENCE of type " + pattern.typeOfMyPattern);

                    //}
                }
            }
        

    }
}
