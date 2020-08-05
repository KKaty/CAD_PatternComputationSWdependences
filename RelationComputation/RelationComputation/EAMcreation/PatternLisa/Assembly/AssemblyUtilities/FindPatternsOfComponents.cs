using System;
using System.Collections.Generic;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;
using Functions = AssemblyRetrieval.PatternLisa.Part.PathCreation_Part.Functions;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public static class AssemblyPatterns
    {
        //This function takes as input a list of congruent MyComponents and returns 
        public static void FindPatternsOfComponents(List<MyRepeatedComponent> listOfComponents, 
            List<MyVertex> listOfVertexOrigins,
            ref List<MyPatternOfComponents> listOfMyPattern, ref List<MyPatternOfComponents> listOfMyPatternTwo, 
            ModelDoc2 SwModel, SldWorks swApplication, ref StringBuilder fileOutput)
        {
            //per PROVA: parto dal livello delle foglie...
            
            //I create a list of adjacency matrices at constant distance.
            //The constant distance is the distance between the origin of the coordinate systems
            //(it is computed by computing the Euclidean norm of the difference vector between 
            // two origins).
            var numOfComponents = listOfComponents.Count;
            //var listOfVertexOrigins = listOfComponents.Select(component2 => component2.Origin).ToList();

          
            if (numOfComponents > 2)
            {
                fileOutput.AppendLine("CREAZIONE MATRICI DI ADIACENZA:");
                var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.
                var toleranceOk = true;

                var listOfMyMatrAdj = Functions.CreateMatrAdj(listOfVertexOrigins, ref fileOutput);

                while (listOfMyMatrAdj.Count > 0 && maxPath == false && toleranceOk == true)
                {
                    bool onlyShortPath;

                    var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj[0].d, listOfMyMatrAdj[0].matr, listOfMyMatrAdj[0].nOccur);
                    fileOutput.AppendLine("Esamino NUOVA MatrAdj. Al momento sono rimaste " + listOfMyMatrAdj.Count +
                                          " MatrAdj.");
                    listOfMyMatrAdj.Remove(listOfMyMatrAdj[0]);
                    //NOTA: forse la mia MatrAdj non deve essere rimossa ma conservata,
                    //soprattutto nel caso in cui si presenta onlyShortPath = true
                    //(non avrebbe senso cancellarla, ma conservarla per la ricerca di path
                    //di 2 RE).
                    fileOutput.AppendLine("----> Eliminata NUOVA MatrAdj. Ora sono rimaste " + listOfMyMatrAdj.Count +
                                          " MatrAdj.");
                    fileOutput.AppendLine(" ");

                    List<MyPathOfPoints> listOfPathOfPoints;

                    maxPath = PathCreation_Assembly.Functions.FindPaths_Assembly(currentMatrAdj, listOfComponents, ref fileOutput,
                        out listOfPathOfPoints, out onlyShortPath, ref toleranceOk,
                        ref listOfMyMatrAdj, ref listOfMyPattern, ref listOfMyPatternTwo, SwModel, swApplication);


                    fileOutput.AppendLine(" ");
                    fileOutput.AppendLine("PER QUESTA MATRADJ: ");
                    fileOutput.AppendLine("maxPath = " + maxPath);
                    fileOutput.AppendLine("listOfMyPattern.Count = " + listOfMyPattern.Count);
                    fileOutput.AppendLine("listOfMyPatternTwo.Count = " + listOfMyPatternTwo.Count);
                    fileOutput.AppendLine("onlyShortPath = " + onlyShortPath);
                    fileOutput.AppendLine("toleranceOK = " + toleranceOk);

                    if (toleranceOk == true)
                    {
                        if (listOfPathOfPoints != null)
                        {
                            if (maxPath == false)
                            {
                                if (onlyShortPath == false)
                                {
                                    GeometryAnalysis.GetPatternsFromListOfPaths_Assembly(listOfPathOfPoints,
                                        listOfComponents,
                                        ref listOfMyMatrAdj, ref listOfMyPattern, ref listOfMyPatternTwo, SwModel,
                                        swApplication);
                                }
                                else
                                {
                                    //non faccio niente e li rimetto in gioco per 
                                }
                            }
                        }
                        else
                        {
                            fileOutput.AppendLine(" ---> NO PATH FOUND in this adjacency matrix!");
                        }
                    }
                    else
                    {
                        fileOutput.AppendLine("===>>    TOLLERANZA NON SUFFICIENTEMENTE PICCOLA. TERMINATO.");
                    }
                }
                
            }
            else
            {
                if (numOfComponents == 2)
                {
                    fileOutput.AppendLine("LE COMPONENTI DI QUESTO TIPO SONO 2: ");

                    if (GeometryAnalysis.IsTranslationTwoRC(listOfComponents[0], listOfComponents[1]))
                    {
                        fileOutput.AppendLine("Le due COMPONENTI sono legate da TRASLAZIONE!");
                        var listOfPathOfCentroids = new List<MyPathOfPoints>();
                        var listOfMyMatrAdj = new List<MyMatrAdj>();
                        var newPatternRC = new List<MyRepeatedComponent> {listOfComponents[0], listOfComponents[1]};
                        var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1]);
                        var newPatternType = "TRANSLATION of length 2";
                        var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                        GeometryAnalysis.CheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                            listOfVertexOrigins, ref listOfMyMatrAdj,
                            ref listOfMyPattern, ref listOfMyPatternTwo);

                    }
                    else
                    {
                        if (GeometryAnalysis.IsReflectionTwoComp_Assembly(listOfComponents[0], listOfComponents[1]))
                        {
                            fileOutput.AppendLine("Le due COMPONENTI sono legate da RIFLESSIONE!");
                            var listOfPathOfCentroids = new List<MyPathOfPoints>();
                            var listOfMyMatrAdj = new List<MyMatrAdj>();
                            var newPatternRC = new List<MyRepeatedComponent>
                            {
                                listOfComponents[0],
                                listOfComponents[1]
                            };
                            var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1]);
                            var newPatternType = "REFLECTION";
                            var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                            GeometryAnalysis.CheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                                listOfVertexOrigins, ref listOfMyMatrAdj,
                                ref listOfMyPattern, ref listOfMyPatternTwo);
                        }
                        else
                        {
                            fileOutput.AppendLine("PROVO CON LA ROTAZIONE:");
                            double[] axisDirection;
                            if (GeometryAnalysis.IsRotationTwoComp180degrees_Assembly(listOfComponents[0], listOfComponents[1], 
                                out axisDirection, SwModel, swApplication))
                            {
                                fileOutput.AppendLine("Le due COMPONENTI sono legate da ROTAZIONE!");
                                var listOfPathOfCentroids = new List<MyPathOfPoints>();
                                var listOfMyMatrAdj = new List<MyMatrAdj>();
                                var newPatternRC = new List<MyRepeatedComponent>
                                {
                                    listOfComponents[0],
                                    listOfComponents[1]
                                };
                                var angle90degrees = Math.PI / 2;
                                var thirdVertexOnCircum = listOfVertexOrigins[0].Rotate(angle90degrees, axisDirection);
                                var newPatternGeomObject = FunctionsLC.CircumPassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1], thirdVertexOnCircum,  ref fileOutput, SwModel, swApplication);
                                var newPatternType = "ROTATION";
                                var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                                GeometryAnalysis.CheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                                    listOfVertexOrigins, ref listOfMyMatrAdj,
                                    ref listOfMyPattern, ref listOfMyPatternTwo);
                            }
                        }
                    }
                }
            }
        }

        public static void KLFindPatternsOfComponents(List<MyRepeatedComponent> listOfComponents,
          List<MyVertex> listOfVertexOrigins,
          ref List<MyPatternOfComponents> listOfMyPattern, ref List<MyPatternOfComponents> listOfMyPatternTwo,
          ModelDoc2 SwModel, SldWorks swApplication, ref StringBuilder fileOutput)
        {
            //per PROVA: parto dal livello delle foglie...

            //I create a list of adjacency matrices at constant distance.
            //The constant distance is the distance between the origin of the coordinate systems
            //(it is computed by computing the Euclidean norm of the difference vector between 
            // two origins).
            var numOfComponents = listOfComponents.Count;
            //var listOfVertexOrigins = listOfComponents.Select(component2 => component2.Origin).ToList();

            //foreach (MyVertex origin in listOfVertexOrigins)
            //{
            //    var centroidToPrint = string.Format("{0}, {1}, {2}", origin.x, origin.y, origin.z);
            //    //KLdebug.Print(centroidToPrint, "Centroidi.txt");
            //}
            
            if (numOfComponents > 2)
            {
                //fileOutput.AppendLine("CREAZIONE MATRICI DI ADIACENZA:");
                var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.
                var toleranceOk = true;
                var listOfMyMatrAdj = Functions.CreateMatrAdj(listOfVertexOrigins, ref fileOutput);
                while (listOfMyMatrAdj.Count > 0 && maxPath == false && toleranceOk == true)
                {
                    bool onlyShortPath;

                    var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj[0].d, listOfMyMatrAdj[0].matr, listOfMyMatrAdj[0].nOccur);
                    //fileOutput.AppendLine("Esamino NUOVA MatrAdj. Al momento sono rimaste " + listOfMyMatrAdj.Count +
                                         // " MatrAdj.");
                    listOfMyMatrAdj.Remove(listOfMyMatrAdj[0]);
                    //NOTA: forse la mia MatrAdj non deve essere rimossa ma conservata,
                    //soprattutto nel caso in cui si presenta onlyShortPath = true
                    //(non avrebbe senso cancellarla, ma conservarla per la ricerca di path
                    //di 2 RE).
                    //fileOutput.AppendLine("----> Eliminata NUOVA MatrAdj. Ora sono rimaste " + listOfMyMatrAdj.Count +
                    //                      " MatrAdj.");
                    //fileOutput.AppendLine(" ");

                    List<MyPathOfPoints> listOfPathOfPoints;
                    maxPath = PathCreation_Assembly.Functions.KLFindPaths_Assembly(currentMatrAdj, listOfComponents, listOfVertexOrigins, ref fileOutput,
                        out listOfPathOfPoints, out onlyShortPath, ref toleranceOk,
                        ref listOfMyMatrAdj, ref listOfMyPattern, ref listOfMyPatternTwo, SwModel, swApplication);

                    //fileOutput.AppendLine(" ");
                    //fileOutput.AppendLine("PER QUESTA MATRADJ: ");
                    //fileOutput.AppendLine("maxPath = " + maxPath);
                    //fileOutput.AppendLine("listOfMyPattern.Count = " + listOfMyPattern.Count);
                    //fileOutput.AppendLine("listOfMyPatternTwo.Count = " + listOfMyPatternTwo.Count);
                    //fileOutput.AppendLine("onlyShortPath = " + onlyShortPath);
                    //fileOutput.AppendLine("toleranceOK = " + toleranceOk);

                    if (toleranceOk == true)
                    {
                        if (listOfPathOfPoints != null)
                        {
                            if (maxPath == false)
                            {
                                if (onlyShortPath == false)
                                {
                                    GeometryAnalysis.KLGetPatternsFromListOfPaths_Assembly(listOfPathOfPoints,
                                        listOfComponents, listOfVertexOrigins,
                                        ref listOfMyMatrAdj, ref listOfMyPattern, ref listOfMyPatternTwo, SwModel,
                                        swApplication);
                                }
                                else
                                {
                                    //non faccio niente e li rimetto in gioco per 
                                }
                            }
                        }
                        else
                        {
                            fileOutput.AppendLine(" ---> NO PATH FOUND in this adjacency matrix!");
                        }
                    }
                    else
                    {
                        fileOutput.AppendLine("===>>    TOLLERANZA NON SUFFICIENTEMENTE PICCOLA. TERMINATO.");
                    }
                }

            }
            else
            {
                if (numOfComponents == 2)
                {
                    //fileOutput.AppendLine("LE COMPONENTI DI QUESTO TIPO SONO 2: ");

                    if (Part.PartUtilities.GeometryAnalysis.IsTranslationTwoRE(listOfComponents[0].RepeatedEntity, listOfComponents[1].RepeatedEntity))
                    {
                      //  fileOutput.AppendLine("Le due COMPONENTI sono legate da TRASLAZIONE!");
                        var listOfPathOfCentroids = new List<MyPathOfPoints>();
                        var listOfMyMatrAdj = new List<MyMatrAdj>();
                        var newPatternRC = new List<MyRepeatedComponent> { listOfComponents[0], listOfComponents[1] };
                        if (listOfVertexOrigins.Count < 2)
                        {
                            swApplication.SendMsgToUser("Non ho due origini");
                        }
                        var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1]);
                        var newPatternType = "TRANSLATION of length 2";
                        var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                        GeometryAnalysis.KLCheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                            listOfVertexOrigins, ref listOfMyMatrAdj,
                            ref listOfMyPattern, ref listOfMyPatternTwo);
                        //swApplication.SendMsgToUser("Pattern da due " + listOfMyPatternTwo.Count);
                        
                    }
                    else
                    {
                        if (Part.PartUtilities.GeometryAnalysis.IsReflectionTwoRE(listOfComponents[0].RepeatedEntity, listOfComponents[1].RepeatedEntity, swApplication))
                        {
                            //fileOutput.AppendLine("Le due COMPONENTI sono legate da RIFLESSIONE!");
                            var listOfPathOfCentroids = new List<MyPathOfPoints>();
                            var listOfMyMatrAdj = new List<MyMatrAdj>();
                            var newPatternRC = new List<MyRepeatedComponent>
                            {
                                listOfComponents[0],
                                listOfComponents[1]
                            };
                            var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1]);
                            var newPatternType = "REFLECTION";
                            var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                            GeometryAnalysis.KLCheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                                listOfVertexOrigins, ref listOfMyMatrAdj,
                                ref listOfMyPattern, ref listOfMyPatternTwo);
                        }
                        //else
                        //{
                        //    fileOutput.AppendLine("PROVO CON LA ROTAZIONE:");
                        //    double[] axisDirection;
                        //    if (GeometryAnalysis.IsRotationTwoComp180degrees_Assembly(listOfComponents[0], listOfComponents[1],
                        //        out axisDirection, SwModel, swApplication))
                        //    {
                        //        fileOutput.AppendLine("Le due COMPONENTI sono legate da ROTAZIONE!");
                        //        var listOfPathOfCentroids = new List<MyPathOfPoints>();
                        //        var listOfMyMatrAdj = new List<MyMatrAdj>();
                        //        var newPatternRC = new List<MyRepeatedComponent>
                        //        {
                        //            listOfComponents[0],
                        //            listOfComponents[1]
                        //        };
                        //        var angle90degrees = Math.PI / 2;
                        //        var thirdVertexOnCircum = listOfVertexOrigins[0].Rotate(angle90degrees, axisDirection);
                        //        var newPatternGeomObject = FunctionsLC.CircumPassingThrough(listOfVertexOrigins[0], listOfVertexOrigins[1], thirdVertexOnCircum, ref fileOutput);
                        //        var newPatternType = "ROTATION";
                        //        var newPattern = new MyPatternOfComponents(newPatternRC, newPatternGeomObject, newPatternType);
                        //        GeometryAnalysis.KLCheckAndUpdate_Assembly(newPattern, ref listOfPathOfCentroids,
                        //            listOfVertexOrigins, ref listOfMyMatrAdj,
                        //            ref listOfMyPattern, ref listOfMyPatternTwo);
                        //    }
                        //}
                    }
                }
            }

        }
    }
}
