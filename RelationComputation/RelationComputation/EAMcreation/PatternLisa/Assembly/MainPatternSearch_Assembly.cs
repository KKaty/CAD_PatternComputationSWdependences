using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.EAMcreation;
using AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using AssemblyRetrieval.PatternLisa.Part.PartUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly
{
    public partial class LC_AssemblyTraverse
    {
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void MainPatternSearch_Assembly(List<MyListOfInstances> ListOfMyListOfInstances,
            ModelDoc2 SwModel, SldWorks swApplication, ref StringBuilder fileOutput,
            out List<MyPatternOfComponents> listOfOutputPatternOut,
            out List<MyPatternOfComponents> listOfOutputPatternTwoOut)
        {
            //ListOfMyListOfInstances.Clear();
            var identityTransformMatrix = new MyTransformMatrix();

            //Update ListOfMyListOfInstances:
            //-delete list containing only one occurrence
            //-reorder the list by decreasing ordering respect to the number of occurrences
            ListOfMyListOfInstances.RemoveAll(list => list.ListOfMyComponent.Count == 1);
            var numOfListOfInstances = ListOfMyListOfInstances.Count;
            ListOfMyListOfInstances = ListOfMyListOfInstances.OrderByDescending(x => x.ListOfMyComponent.Count).ToList();

            listOfOutputPatternOut = new List<MyPatternOfComponents>(); //list of output patterns found
            listOfOutputPatternTwoOut = new List<MyPatternOfComponents>(); //list of output patterns of length 2 found
           
            for (var k = 0; k < numOfListOfInstances; k++)
            {
                var listOfOutputPattern = new List<MyPatternOfComponents>(); //list of output patterns found
                var listOfOutputPatternTwo = new List<MyPatternOfComponents>(); //list of output patterns of length 2 found

                var listOfComponents = new List<MyRepeatedComponent>(ListOfMyListOfInstances[k].ListOfMyComponent);
                //Verify if there exist coinciding origins: in that case that component is skipped
                //Otherwise the pexisting patterns are identified:
                //var listOfVertexOrigins = listOfComponents.Select(component2 => component2.Origin).ToList();
               
                //var found = false;
                //var indexRepEntity = 0;
                //while (indexRepEntity < listOfVertexOrigins.Count - 1 && found == false)
                //{
                //    if (listOfVertexOrigins.FindIndex(origin => origin.Equals(listOfVertexOrigins[indexRepEntity]) &&
                //                                                listOfVertexOrigins.IndexOf(origin) != indexRepEntity) != -1)
                //    {
                //        found = true;
                //    }
                //    else
                //    {
                //        indexRepEntity++;
                //    }
                //}

                //// Lo commento perché vorrei che non usasse indexRepEntity sistemi di riferimento per il riconoscimento, ma indexRepEntity vertici delle parti

                //if (found == false)
                //{
                //    AssemblyPatterns.FindPatternsOfComponents(listOfComponents, listOfVertexOrigins,
                //        ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, swApplication, ref fileOutput);
                //}
                //else
                //{

                //    swApplication.SendMsgToUser("Component '" +
                //                                ListOfMyListOfInstances[k].Name + "' cannot be processed. SKIPPED.");
                //}

                // Modifica aggiunta da Katia per il controllo delle facce

                var listCentroidWordRF = new List<MyVertex>();
                //swApplication.SendMsgToUser("Analizzo " + ListOfMyListOfInstances[k].Name);
                //swApplication.SendMsgToUser("Nome istanza " + instance.Name);
                //swApplication.SendMsgToUser("Ho " + listOfComponents.Count + " componenti");

                foreach (MyRepeatedComponent repeatedComponent in listOfComponents)
                {
                    var sameIndex = listOfComponents.IndexOf(repeatedComponent);
                    repeatedComponent.RepeatedEntity.idRE = sameIndex;

                    var newRepeatedEntity = repeatedComponent.RepeatedEntity;

                    if (newRepeatedEntity.listOfFaces.Any() && !repeatedComponent.IsSphere)
                    {
                        //swApplication.SendMsgToUser("Ci sono facce");

                        double[] centroidAffine =
                        {
                            newRepeatedEntity.centroid.x, newRepeatedEntity.centroid.y,
                            newRepeatedEntity.centroid.z, 1
                        };

                        double scaleFactor = 1;

                        double[,] compositionMatrixOfComponentPart =
                            new double[4, 4]
                            {
                                {
                                    (double) repeatedComponent.Transform.RotationMatrix[0, 0],
                                    (double) repeatedComponent.Transform.RotationMatrix[0, 1],
                                    (double) repeatedComponent.Transform.RotationMatrix[0, 2],
                                    (double) repeatedComponent.Transform.TranslationVector[0]
                                },
                                {
                                    (double) repeatedComponent.Transform.RotationMatrix[1, 0],
                                    (double) repeatedComponent.Transform.RotationMatrix[1, 1],
                                    (double) repeatedComponent.Transform.RotationMatrix[1, 2],
                                    (double) repeatedComponent.Transform.TranslationVector[1]
                                },
                                {
                                    (double) repeatedComponent.Transform.RotationMatrix[2, 0],
                                    (double) repeatedComponent.Transform.RotationMatrix[2, 1],
                                    (double) repeatedComponent.Transform.RotationMatrix[2, 2],
                                    (double) repeatedComponent.Transform.TranslationVector[2]
                                },
                                {0.0, 0.0, 0.0, 1}
                            };

                        var newCentroid = Matrix.Multiply(compositionMatrixOfComponentPart, centroidAffine);

                        var centroid = new MyVertex(newCentroid[0], newCentroid[1], newCentroid[2]);
                        // Ho dovuto cambiare l'origine perché il find pattern a volte prende indexRepEntity centri dalla lista in input a volte lo estrae dalla repeated component.
                        //repeatedComponent.Origin.x = newCentroid[0];
                        //repeatedComponent.Origin.y = newCentroid[1];
                        //repeatedComponent.Origin.z = newCentroid[2];
                        //listCentroidWordRF.Add(centroid);

                        repeatedComponent.Origin.x = newRepeatedEntity.centroid.x;
                        repeatedComponent.Origin.y = newRepeatedEntity.centroid.y;
                        repeatedComponent.Origin.z = newRepeatedEntity.centroid.z;
                        listCentroidWordRF.Add(newRepeatedEntity.centroid);

                        //SwModel = swApplication.ActiveDoc;
                        //SwModel.ClearSelection2(true);
                        //SwModel.Insert3DSketch();
                        //SwModel.CreatePoint2(newRepeatedEntity.centroid.x, newRepeatedEntity.centroid.y,
                        //    newRepeatedEntity.centroid.z);
                        //SwModel.InsertSketch();

                        // listOfOutputPattern.Clear();
                        // listOfOutputPatternTwo.Clear();

                    }
                    // Se non ci sono facce verifico se è tutta freeform e uguale ad una sfera
                    else if (repeatedComponent.IsSphere)
                    {
                        // Calcolo del centro di massa per trovare il pattern
                        var body = (Body2)repeatedComponent.Component.GetBody();
                        var mass = (double[])body.GetMassProperties(0);

                        var centerOfMass = new MyVertex(mass[0], mass[1], mass[2]);
                        repeatedComponent.Origin.x = centerOfMass.x;
                        repeatedComponent.Origin.y = centerOfMass.y;
                        repeatedComponent.Origin.z = centerOfMass.z;
                        listCentroidWordRF.Add(centerOfMass);

                        //SwModel = swApplication.ActiveDoc;
                        //SwModel.ClearSelection2(true);
                        //SwModel.Insert3DSketch();
                        //SwModel.CreatePoint2(mass[0], mass[1], mass[2]);
                        //SwModel.InsertSketch();


                    }
                }
                //swApplication.SendMsgToUser("Calcolo pattern " + listOfComponents[0].Name + "\ncomponenti " + listOfComponents.Count);
                if (listCentroidWordRF.Count == listOfComponents.Count)
                {
                    AssemblyPatterns.KLFindPatternsOfComponents(listOfComponents, listCentroidWordRF,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, swApplication, ref fileOutput);
                    
                }
                if (listOfOutputPattern.Any())
                        {
                            foreach (var patternOfComponentse in listOfOutputPattern)
                            {
                                listOfOutputPatternOut.Add(patternOfComponentse);
                                //swApplication.SendMsgToUser("Patern " + patternOfComponentse.typeOfMyPattern + " lunghezza " + patternOfComponentse.listOfMyRCOfMyPattern.Count);

                            }
                            
                        }
                        if (listOfOutputPatternTwo.Any())
                        {
                            foreach (var patternOfComponentse in listOfOutputPatternTwo)
                            {
                                listOfOutputPatternTwoOut.Add(patternOfComponentse);
                                //swApplication.SendMsgToUser("Patern " + patternOfComponentse.typeOfMyPattern + " lunghezza " + patternOfComponentse.listOfMyRCOfMyPattern.Count);

                            }
                        }
                        }
                  //}
                        //It shows and print the detected patterns:
                        //ShowAndPrintResults_Assembly(listOfOutputPatternOut, listOfOutputPatternTwoOut, SwModel, swApplication);

                        ////Assigning of id numbers to the found MyPatternOfComponents
                        ////(the id will be used in composed pattern search phase)
                        ////At the same time, we draw the pattern origins
                        //SwModel = swApplication.ActiveDoc;
                        //SwModel.ClearSelection2(true);
                        //SwModel.Insert3DSketch();
                        //var j = 0;

                        //foreach (var pattern in listOfOutputPattern)
                        //{
                        //    pattern.idMyPattern = j;
                        //    j++;
                        //    var listOfOriginsThisPattern =
                        //        pattern.listOfMyRCOfMyPattern.Select(rc => rc.Origin).ToList();
                        //    var patternCentroid =
                        //        ExtractInfoFromBRep.computeCentroidsOfVertices(listOfOriginsThisPattern);
                        //    pattern.patternCentroid = patternCentroid;
                        //    SwModel.CreatePoint2(pattern.patternCentroid.x, pattern.patternCentroid.y,
                        //        pattern.patternCentroid.z);
                        //}
                        //foreach (var pattern in listOfOutputPatternTwo)
                        //{
                        //    pattern.idMyPattern = j;
                        //    j++;
                        //    var listOfOriginsThisPattern =
                        //        pattern.listOfMyRCOfMyPattern.Select(rc => rc.Origin).ToList();
                        //    var patternCentroid =
                        //        ExtractInfoFromBRep.computeCentroidsOfVertices(listOfOriginsThisPattern);
                        //    pattern.patternCentroid = patternCentroid;
                        //    SwModel.CreatePoint2(pattern.patternCentroid.x, pattern.patternCentroid.y,
                        //        pattern.patternCentroid.z);
                        //}
                        //SwModel.InsertSketch();

                        ////Patterns are subdivided in Line patterns and in Circle patterns:
                        //var listOfOutputPatternLine = new List<MyPatternOfComponents>();
                        //var listOfOutputPatternCircum = new List<MyPatternOfComponents>();
                        //var nameFile = "ComposedPatterns_Assembly.txt";
                        //foreach (var pattern in listOfOutputPattern)
                        //{
                        //    if (pattern.pathOfMyPattern.GetType() == typeof (MyLine))
                        //    {
                        //        listOfOutputPatternLine.Add(pattern);
                        //        KLdebug.Print("- pattern di tipo " + pattern.typeOfMyPattern, nameFile);
                        //        KLdebug.Print("  di lunghezza " + pattern.listOfMyRCOfMyPattern.Count, nameFile);
                        //    }
                        //    else
                        //    {
                        //        listOfOutputPatternCircum.Add(pattern);
                        //        KLdebug.Print("- pattern di tipo " + pattern.typeOfMyPattern, nameFile);
                        //        KLdebug.Print("  di lunghezza " + pattern.listOfMyRCOfMyPattern.Count, nameFile);
                        //    }
                        //}

                        ////      >>>>>> COMPOSED PATTERN SEARCH:
                        //List<MyComposedPatternOfComponents> listOfOutputComposedPattern;
                        //List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo;
                        //FindComposedPatternsOfComponents(
                        //    listOfOutputPatternLine,
                        //    listOfOutputPatternCircum,
                        //    out listOfOutputComposedPattern,
                        //    out listOfOutputComposedPatternTwo,
                        //    SwModel, swApplication, ref fileOutput);

                        //ShowAndPrintResults_Assembly_ComposedPatterns(listOfOutputComposedPattern,
                        //    listOfOutputComposedPatternTwo,
                        //    SwModel, swApplication);
                    
                
            

        }


        //This function takes a component and decomposes it in all the existing children. 
        //For every children a MyTransformMatrix is computed. The computed transform matrix refers 
        //to the given children component respect to its position in the original file
        //(such a matrix is obtained composing the MyTransformMatrix referred to the component
        //position respect to its father and the father's MyTransformMatrix).
        //If a children has other children it decomposes it too.
        public static void TraversAssemblyComponents(Component2 swComponent2, MyTransformMatrix fatherTransformMatrix,
            SldWorks swApplication)
        {
            //List of the lists of instances of the same file object:
            var ListOfMyListOfInstances = new List<MyListOfInstances>();

            if (swComponent2 == null) return;

            var nameFile = "ClassifyComponents.txt";
            
            //I take the children of the root component
            var swChildrenComponent = (Array)swComponent2.GetChildren();
           
            int i = 0;

            foreach (Component2 component2 in swChildrenComponent)
            {
                //for each son I create a new ComputeNewRepeatedComponent
                var newRelativeTransformMatrix = AssemblyTraverse.GetTransformMatrix(component2, swApplication);
                var newTransformMatrix = fatherTransformMatrix.ComposeTwoTransformMatrix(newRelativeTransformMatrix,
                        swApplication);              
                bool newIsLeaf = component2.IGetChildrenCount() == 0;
                //If the current son is not a "leaf" component the function is recalled again:
                if (component2.IGetChildrenCount() != 0)
                {
                    TraversAssemblyComponents(component2, newTransformMatrix, swApplication);
                }
                else
                {
                    var newMyComponent = ComputeNewRepeatedComponent(swApplication, component2, 0, newRelativeTransformMatrix, i, newIsLeaf);

                    //I verify if the list corresponding to this component already exists:        
                    //KLdebug.Print("-Componente # " + indexRepEntity + " di " + swComponent2.Name2, nameFile);
                    string namePath = component2.GetPathName();
                    string nameFileComponent = namePath.Split('\\').Last(); //to get the last name after the last "\"
                    //namePath = namePath.TrimEnd('-');
                    //string nameFileComponent = namePath.Remove((namePath.LastIndexOf('-') + 1));


                    //KLdebug.Print("   --->> namePath: " + namePath, "nomi.txt");
                    //KLdebug.Print("   --->> nameFileComponent: " + nameFileComponent, "nomi.txt");
                    //KLdebug.Print("     Matrice relativa", nameFile);
                    //KLdebug.PrintTransformMatrix(newRelativeTransformMatrix, nameFile, swApplication);
                    //KLdebug.Print("     Matrice assoluta", nameFile);
                    //KLdebug.PrintTransformMatrix(newTransformMatrix, nameFile, swApplication);

         
                    var indexOfFind = ListOfMyListOfInstances.FindIndex(list => list.Name == nameFileComponent);
                    if (indexOfFind != -1)
                    {
                        //The list referred to this component already exists. I add it to the corresponding list
                        //var newMyComponent = new ComputeNewRepeatedComponent();

                        //var newIndex =
                        //    ListOfMyListOfInstances[indexOfFind].ListOfMyComponent.Count + 1;
                        //newMyComponent.RepeatedEntity.idRE = newIndex;
                        ListOfMyListOfInstances[indexOfFind].ListOfMyComponent.Add(newMyComponent);
                        //KLdebug.Print("       AGGIORNATA LISTA ESISTENTE: " + newMyComponent.Name + " con id" +newMyComponent.RepeatedEntity.idRE, nameFile);
                    }

                    //else
                    //{
                    //    //Check of the component satisfy other shape criteria (volume and percentage of type of surfaces)
                    //    var shapeComparison = AssemblyTraverse.KL_GetShapePart(component2, swApplication);
                    //    var statisticComparison = AssemblyTraverse.KL_GetStatisticPart(component2,
                    //        shapeComparison.Surface, swApplication);

                    //    var addCompForShape = false;
                    //    foreach (MyListOfInstances instance in ListOfMyListOfInstances)
                    //    {
                    //        var component = (Component2)instance.ListOfMyComponent.First().Component;
                    //        var shapeOriginal = AssemblyTraverse.KL_GetShapePart(component, swApplication);
                    //        var statisticOriginal = AssemblyTraverse.KL_GetStatisticPart(component,
                    //            shapeOriginal.Surface,
                    //            swApplication);

                    //        if (KLcriteriaCheck.Size.Volume(shapeOriginal, shapeComparison, swApplication))
                    //        {
                    //            if (KLcriteriaCheck.Size.PercentageSurfacesType(statisticOriginal, statisticComparison,
                    //                swApplication))
                    //            {
                    //                namePath = component.GetPathName();
                    //                nameFileComponent = namePath.Split('\\').Last();
                    //                //nameFileComponent = namePath;
                    //                indexOfFind =
                    //                    ListOfMyListOfInstances.FindIndex(list => list.Name == nameFileComponent);
                    //                ListOfMyListOfInstances[indexOfFind].ListOfMyComponent.Add(newMyComponent);
                    //                addCompForShape = true;
                    //                break;
                    //            }
                    //        }
                    //    }
                        
                    //    //The list referred to this component does not exist yet. I create it
                    //    if (!addCompForShape)
                    //    {
                    //        List<MyRepeatedComponent> newListOfComponentsOfListOfInstances = new List
                    //            <MyRepeatedComponent>
                    //        {
                    //            newMyComponent
                    //        };
                    //        var newListOfInstances = new MyListOfInstances(nameFileComponent,
                    //            newListOfComponentsOfListOfInstances);
                    //        ListOfMyListOfInstances.Add(newListOfInstances);
                    //        //KLdebug.Print("       CREATA NUOVA LISTA nome:" + newListOfInstances.Name, nameFile);
                    //    }
                    //}

                    
                    i++;
                }

            }

        }

        public static MyRepeatedComponent ComputeNewRepeatedComponent(SldWorks swApplication, Component2 component2, int idCorrespondingNode,
            MyTransformMatrix newRelativeTransformMatrix, int indexRepEntity, bool newIsLeaf)
        {
            // Aggiunto calcolo dell'entità ripetuta alla parte.
            var currentModel = component2.GetModelDoc2();
            var entityList =
                (List<Entity>) AssemblyTraverse.KL_GetPartFaces(currentModel, component2.Name2,
                    swApplication);

            double[,] compositionMatrixOfComponentPart =
                new double[4, 4]
                {
                    {
                        (double) newRelativeTransformMatrix.RotationMatrix[0, 0],
                        (double) newRelativeTransformMatrix.RotationMatrix[0, 1],
                        (double) newRelativeTransformMatrix.RotationMatrix[0, 2],
                        (double) newRelativeTransformMatrix.TranslationVector[0]
                    },
                    {
                        (double) newRelativeTransformMatrix.RotationMatrix[1, 0],
                        (double) newRelativeTransformMatrix.RotationMatrix[1, 1],
                        (double) newRelativeTransformMatrix.RotationMatrix[1, 2],
                        (double) newRelativeTransformMatrix.TranslationVector[1]
                    },
                    {
                        (double) newRelativeTransformMatrix.RotationMatrix[2, 0],
                        (double) newRelativeTransformMatrix.RotationMatrix[2, 1],
                        (double) newRelativeTransformMatrix.RotationMatrix[2, 2],
                        (double) newRelativeTransformMatrix.TranslationVector[2]
                    },
                    {0.0, 0.0, 0.0, 1}
                };

            //swApplication.SendMsgToUser("Calcolo repeated entity di " + component2.Name2 + "\nnumero facce " + entityList.Count);
            MyRepeatedEntity newRepeatedEntity = ExtractInfoFromBRep.KLBuildRepeatedEntity(
                entityList, indexRepEntity, compositionMatrixOfComponentPart, swApplication);
            //swApplication.SendMsgToUser("Ha " + newRepeatedEntity.listOfVertices.Count + " vertici\n" + newRepeatedEntity.listOfAddedVertices.Count + " vertici aggiunti");

            // Fine calcolo entità rip da aggiungere alla componente.
            var newMyComponent = new MyRepeatedComponent(component2, idCorrespondingNode, newRelativeTransformMatrix, newIsLeaf,
                newRepeatedEntity);
            return newMyComponent;
        }

        //This function prints the detected patterns of components and colors coherently the components in the model.   
        public static void ShowAndPrintResults_Assembly(List<MyPatternOfComponents> listOfOutputPattern, 
            List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks swApplication)
        {
           

            if (listOfOutputPattern != null && listOfOutputPatternTwo != null)
            {
                if (listOfOutputPattern.Count == 0 && listOfOutputPatternTwo.Count == 0)
                {
                    //swApplication.SendMsgToUser("NO PATTERN FOUND.");
                    return;
                }
            }

                var listOfOutputPatternLine = new List<MyPatternOfComponents>();
                var listOfOutputPatternCircum = new List<MyPatternOfComponents>();
                var listOfOutputPatternTwoLine = new List<MyPatternOfComponents>();
                var listOfOutputPatternTwoCircum = new List<MyPatternOfComponents>();
    
                foreach (var pattern in listOfOutputPattern)
                {
                    if (pattern.pathOfMyPattern.GetType() == typeof(MyLine))
                    {
                        listOfOutputPatternLine.Add(pattern);
                        
                    }
                    else
                    {
                        listOfOutputPatternCircum.Add(pattern);
                        
                    }
                }
            if (listOfOutputPatternLine != null)
            {
                ColorFace.MyVisualOutput_Assembly(listOfOutputPatternLine, swApplication, SwModel);
            }
            if (listOfOutputPatternCircum != null)
            {
                ColorFace.MyVisualOutput_Assembly(listOfOutputPatternCircum, swApplication, SwModel);
            }

            foreach (var pattern in listOfOutputPatternTwo)
            {
                if (pattern.pathOfMyPattern.GetType() == typeof (MyLine))
                {
                    listOfOutputPatternTwoLine.Add(pattern);
                   
                }
                else
                {
                    listOfOutputPatternTwoCircum.Add(pattern);
                   
                }
            }
            if (listOfOutputPatternTwoLine != null)
            {
                ColorFace.MyVisualOutput_Assembly(listOfOutputPatternTwoLine, swApplication, SwModel);
            }
            if (listOfOutputPatternTwoCircum != null)
            {
                ColorFace.MyVisualOutput_Assembly(listOfOutputPatternTwoCircum, swApplication, SwModel);
            }
        }

        //This function prints the detected COMPOSED patterns of components (lines and circles).   
        public static void ShowAndPrintResults_Assembly_ComposedPatterns(
            List<MyComposedPatternOfComponents> listOfOutputComposedPattern,
            List<MyComposedPatternOfComponents> listOfOutputComposedPatternTwo, ModelDoc2 SwModel, SldWorks swApplication)
        {
            
            SwModel.ClearSelection2(true);
            SwModel.Insert3DSketch();

            int nOfPatterns;
            var tolerance = Math.Pow(10, -4);
            MyVertex headCentroidPattern;
            MyVertex backCentroidPattern;
            foreach (var composedPattern in listOfOutputComposedPattern)
            {
                nOfPatterns = composedPattern.ListOfMyPatternOfComponents.Count;
                headCentroidPattern = composedPattern.ListOfMyPatternOfComponents[0].patternCentroid;
                backCentroidPattern = composedPattern.ListOfMyPatternOfComponents[nOfPatterns - 1].patternCentroid;
                if (composedPattern.pathOfMyComposedPatternOfComponents.GetType() == typeof(MyLine))
                {
                    SwModel.GetActiveSketch2();
                    SwModel.CreateLine2(headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                        backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z);

                }
                else //typeof(MyCircumForPath)
                {
                    var pathObject = (MyCircumForPath)composedPattern.pathOfMyComposedPatternOfComponents;
                    var circumCenter = pathObject.circumcenter;

                    if (
                        Math.Abs(headCentroidPattern.Distance(backCentroidPattern) -
                                 composedPattern.constStepOfMyComposedPatternOfComponents) < tolerance)
                    {
                        //SwModel.ClearSelection2(true);
                        //SwModel.Insert3DSketch();
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
                            composedPattern.ListOfMyPatternOfComponents[1].patternCentroid.x,
                            composedPattern.ListOfMyPatternOfComponents[1].patternCentroid.y,
                            composedPattern.ListOfMyPatternOfComponents[1].patternCentroid.z);
                        //SwModel.InsertSketch();
                    }
                }
            }

            foreach (var composedPattern in listOfOutputComposedPatternTwo)
            {
                headCentroidPattern = composedPattern.ListOfMyPatternOfComponents[0].patternCentroid;
                backCentroidPattern = composedPattern.ListOfMyPatternOfComponents[1].patternCentroid;
                SwModel.CreateLine2(headCentroidPattern.x, headCentroidPattern.y, headCentroidPattern.z,
                    backCentroidPattern.x, backCentroidPattern.y, backCentroidPattern.z);

            }
            SwModel.InsertSketch();
          
        }
    }
}
