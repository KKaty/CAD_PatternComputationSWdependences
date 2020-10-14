using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AssemblyRetrieval.Graph;
using AssemblyRetrieval.PatternLisa.Assembly;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using AssemblyRetrieval.PatternLisa.Part;
using AssemblyRetrieval.Utility;
using QuickGraph;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace AssemblyRetrieval.EAMcreation
{

    public static class AssemblyTraverse
    {
        public static bool KL_IsAssemblyModel(ModelDoc2 swModel, SldWorks swApplication)
        {
            var swDocumentType = (swDocumentTypes_e)swModel.GetType();

            switch (swDocumentType)
            {
                case swDocumentTypes_e.swDocPART:
                    swApplication.SendMsgToUser("Le model est une part!");
                    return false;
                case swDocumentTypes_e.swDocDRAWING:
                    swApplication.SendMsgToUser("Le model est un dessin!");
                    return false;
                case swDocumentTypes_e.swDocASSEMBLY:
                    return true;
            }
            return false;
        }

        public static void KL_TraversAssemblyComponents(Component2 swComponent2, int nLevel, SldWorks swApplication)
        {
            for (var i = 0; i < nLevel; i++)
            {
                if (swComponent2 == null) continue;
                var swChildrenComponent = (Array)swComponent2.GetChildren();
                foreach (Component2 component2 in swChildrenComponent)
                {
                    KL_TraversAssemblyComponents(component2, nLevel + 1, swApplication);
                    KL_GetTransformsOfAssemblyComponents(component2, swApplication);
                }
            }
        }


        public static AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge> KL_GetGraphOfAssemblyComponents
            (Component2 fatherComponent, KLgraph.KLnode father, ref List<KLgraph.KLnode> listVertices, List<KLgraph.KLedge> listEdges,
            ref List<MyListOfInstances> listOfMyListOfInstances, SldWorks swApplication)
        {
            var graph = new AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge>(true);

            var fatherId = father.HashCode;
            //var fatherComponent = father.Comp;
            graph.AddVertexRange(listVertices);
            graph.AddEdgeRange(listEdges);
            {
                if (fatherComponent != null)
                {
                    var swChildrenComponent = (Array) fatherComponent.GetChildren();
                    var rootNode = listVertices.FirstOrDefault(v => v.Name.Equals(father.Name));

                    int i = 0;
                    try
                    {
                        var index = 0;
                        //Console.WriteLine("Number of elements to analyze: " + swChildrenComponent.Length);
                        foreach (Component2 component in swChildrenComponent)
                        {
                            index++;
                            Console.WriteLine(String.Format("Processing component {0} over {1}" , index, swChildrenComponent.Length));

                            //if (!component.Name2.Contains("Arbre satellite BV"))
                            {

                                var transforation =
                                    (Array) KL_GetTransformsOfAssemblyComponents(component, swApplication);
                                var fatherName = fatherComponent.Name2;
                                var componentName = component.Name2;
                                object vBodyInfo;
                                var bodyList =
                                    (Array) (component.GetBodies3((int) swBodyType_e.swAllBodies, out vBodyInfo));
                                //var componentName = pathComponent.Split('\\').Last();
                                var nameLastPart = componentName.TrimEnd('-');
                                nameLastPart = nameLastPart.Remove(nameLastPart.LastIndexOf('-') + 1);
                                var componentPath = component.GetPathName();
                                var hashCode = component.GetHashCode();
                                //var nodeId = component.GetID();
                                var nodeId = 0;
                                var childrenNumber = component.IGetChildrenCount();

                                if (childrenNumber == 0)
                                {
                                    //Console.WriteLine("Single");
                                    //var shape = KL_GetShapePart(component, swApplication);
                                    //var statistic = KL_GetStatisticPart(component, shape.Surface, swApplication);
                                    //swApplication.SendMsgToUser(componentName + "  " + statistic.Genus);
                                    var nodePart = new KLgraph.KLnodePart(hashCode, transforation, fatherName, fatherId,
                                        component, componentName, nameLastPart, componentPath,
                                        nodeId,
                                        null, null);
                                    var edgeStructure = new KLgraph.KLedgeStructure(rootNode, nodePart, 0);

                                    listVertices.Add(nodePart);
                                    listEdges.Add(edgeStructure);
                                    graph.AddVertex(nodePart);
                                    graph.AddEdge(edgeStructure);

                                    // Per ogni parte vengono aggiornate le entità ripetute.  

                                    bool newIsLeaf = component.IGetChildrenCount() == 0;
                                    var newRelativeTransformMatrix = GetTransformMatrix(component, swApplication);

                                    var newMyComponent = LC_AssemblyTraverse.ComputeNewRepeatedComponent(swApplication,
                                        component, nodeId, newRelativeTransformMatrix, i, newIsLeaf);

                                    //if (!newMyComponent.RepeatedEntity.listOfFaces.Any())
                                    //{
                                    //    if (nodePart.NodeShapeIsSphere(swApplication))
                                    //    {
                                    //        newMyComponent.IsSphere = true;
                                    //    }
                                    //    else
                                    //    {
                                    //        newMyComponent.IsSphere = false;
                                    //    }
                                    //}

                                    string namePath = component.Name2;
                                    //Console.WriteLine("Nome completo " + namePath);

                                    string nameFileComponent = namePath.Split('/').Last();
                                    //Console.WriteLine("Nome splittato " + nameFileComponent);

                                    //to get the last name after the last "\"
                                    namePath = namePath.TrimEnd('-');
                                    nameFileComponent = nameFileComponent.Remove((nameFileComponent.LastIndexOf('-') + 1));
                                    //Console.WriteLine("Analizzo componente " + nameFileComponent);
                                    var indexOfFind =
                                        listOfMyListOfInstances.FindIndex(list => list.Name == nameFileComponent);
                                    if (indexOfFind != -1)
                                    {
                                        //Console.WriteLine("Trovata componente con lo stesso nome " + nameFileComponent);
                                        //The list referred to this component already exists. I add it to the corresponding list
                                        //var newMyComponent = new ComputeNewRepeatedComponent();

                                        //var newIndex =
                                        //    ListOfMyListOfInstances[indexOfFind].ListOfMyComponent.Count + 1;
                                        //newMyComponent.RepeatedEntity.idRE = newIndex;
                                        listOfMyListOfInstances[indexOfFind].ListOfMyComponent.Add(newMyComponent);
                                        //KLdebug.Print("       AGGIORNATA LISTA ESISTENTE: " + newMyComponent.Name + " con id" +newMyComponent.RepeatedEntity.idRE, "Istances.txt");
                                    }

                                    else
                                        {
                                        //    //Check of the component satisfy other shape criteria (volume and percentage of type of surfaces)
                                        //    var addCompForShape = false;

                                        //    foreach (MyListOfInstances instance in listOfMyListOfInstances)
                                        //    {
                                        //        var comparisonComponent = instance.ListOfMyComponent.First().Component;
                                        //        string comparisonComponentPath = comparisonComponent.Name2;
                                        //        //string nameFileComparisonComponent = comparisonComponentPath.Split('\\').Last();
                                        //        string nameFileComparisonComponent = comparisonComponentPath.TrimEnd('-');
                                        //        nameFileComparisonComponent =
                                        //            nameFileComparisonComponent.Remove(
                                        //                (nameFileComparisonComponent.LastIndexOf('-') + 1));

                                        //        var comparisonNode =
                                        //            (KLgraph.KLnodePart)
                                        //            listVertices.Find(
                                        //                part => part.NameLastPart == nameFileComparisonComponent);
                                        //        if (comparisonNode != null)
                                        //        {
                                        //            var comparisonShape = comparisonNode.KLshape;
                                        //            var comparisonStatistic = comparisonNode.KLstatistic;

                                        //            if (KLcriteriaCheck.Size.Volume(comparisonShape, shape, swApplication))
                                        //            {
                                        //                if (KLcriteriaCheck.Size.PercentageSurfacesType
                                        //                (comparisonStatistic,
                                        //                    statistic,
                                        //                    swApplication))
                                        //                {
                                        //                    namePath = comparisonComponent.Name2;
                                        //                    nameFileComponent = namePath.TrimEnd('-');
                                        //                    nameFileComponent =
                                        //                        nameFileComponent.Remove(
                                        //                            (nameFileComponent.LastIndexOf('-') + 1));

                                        //                    //nameFileComponent = namePath.Split('\\').Last();
                                        //                    //nameFileComponent = namePath;
                                        //                    indexOfFind =
                                        //                        listOfMyListOfInstances.FindIndex(
                                        //                            list => list.Name == nameFileComponent);
                                        //                    listOfMyListOfInstances[indexOfFind].ListOfMyComponent.Add(
                                        //                        newMyComponent);
                                        //                    //KLdebug.Print("       AGGIORNATA LISTA ESISTENTE: " + newMyComponent.Name + " con id" + newMyComponent.RepeatedEntity.idRE, "Istances.txt");
                                        //                    addCompForShape = true;
                                        //                    break;
                                        //                }
                                        //            }
                                        //        }
                                        //        else
                                        //        {
                                        //            swApplication.SendMsgToUser("Nodo ripetuto non recuperato");
                                        //        }
                                        //    }

                                        //    //The list referred to this component does not exist yet. I create it
                                        //    if (!addCompForShape)
                                        //    {

                                        List<MyRepeatedComponent> newListOfComponentsOfListOfInstances = new List
                                            <MyRepeatedComponent>
                                                {
                                                    newMyComponent
                                                };
                                    var newListOfInstances = new MyListOfInstances(nameFileComponent,
                                                newListOfComponentsOfListOfInstances);
                                    listOfMyListOfInstances.Add(newListOfInstances);
                                    //        //KLdebug.Print("       CREATA NUOVA LISTA nome:" + newListOfInstances.Name, "Istances.txt");
                                    //    }
                                    }

                                    i++;
                                }
                                else
                                {
                                    var shape = KL_GetShapeAssembly(component, swApplication);
                                    var statistic = KL_GetStatisticAssembly(component, swApplication);
                                    var nodeAssembly = new KLgraph.KLnodeAssembly(hashCode, transforation, fatherName,
                                        fatherId,
                                        component, componentName, nameLastPart, componentPath, nodeId,
                                        childrenNumber, statistic, shape);
                                    var edgeStructure = new KLgraph.KLedgeStructure(rootNode, nodeAssembly, 0);

                                    listVertices.Add(nodeAssembly);
                                    listEdges.Add(edgeStructure);
                                    graph.AddVertex(nodeAssembly);
                                    graph.AddEdge(edgeStructure);

                                    graph = KL_GetGraphOfAssemblyComponents(component, nodeAssembly, ref listVertices,
                                        listEdges,
                                        ref listOfMyListOfInstances, swApplication);
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.Source);
                        throw;
                    }

                }
                else
                {
                    Console.WriteLine("Padre nullo");
                }
            }
            //Console.WriteLine("Ritorno il grafo");
            return graph;
        }

        public static Array KL_GetTransformsOfAssemblyComponents(Component2 comp, SldWorks swApplication)
        {
            var componentXForm = comp.Transform2;
            if (componentXForm != null)
            {
                var xForm = (Array)componentXForm.ArrayData;
                return xForm;
            }
            return null;
        }

        public static void PrintTransformsOfAssemblyComponents(string compName, Array xForm, SldWorks swApplication)
        {
                var line = String.Format("Nome componente: {0}\n\n" +
                              "Matrice di rotazione:\n" +
                              "{1} {2} {3}\n" +
                              "{4} {5} {6}\n" +
                              "{7} {8} {9}\n\n" +
                              "Traslazione: ({10} {11} {12})\n\n" +
                              "Fattore scala: {13}\n\n\n", compName,
                              xForm.GetValue(0), xForm.GetValue(1), xForm.GetValue(2),
                              xForm.GetValue(3), xForm.GetValue(4), xForm.GetValue(5),
                              xForm.GetValue(6), xForm.GetValue(7), xForm.GetValue(8),
                              xForm.GetValue(9), xForm.GetValue(10), xForm.GetValue(11),
                              xForm.GetValue(12));
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static List<Entity> KL_GetPartFaces(ModelDoc2 model, string name, SldWorks swApplication)
        {
            var entityList = new List<Entity>();

            if (model != null && (swDocumentTypes_e) model.GetType() == swDocumentTypes_e.swDocPART)
            {
                var part = (PartDoc) model;
                
                var bodiesList = (Array) part.GetBodies2((int) swBodyType_e.swSolidBody, false); //Attenzione, il body è già massimo qui!
   
                foreach (Body2 body in bodiesList)
                {
                    if (body != null)
                    {
                        //var bodyInterference = body.GetProcessedBody2(1, 1); // Da aggiungere per non duplicare i cilindri, ma si deveno usare le entità altrimenti si disconnette.

                        var face = (Face2)body.GetFirstFace();
                        int numero = 0;
                        while (face != null)
                        {
                            var ent = (Entity)face;
                            var saveEnt = ent.GetSafeEntity();
                            entityList.Add(saveEnt);
                            //var faceNameComplete = part.GetEntityName(face);
                            //var faceName = faceNameComplete.Split('/').First();
                            //Debug.KLdebug.Print(newFaceName + " -- " + faceName + " -- " + resoult, "FaceName.txt");
                            face = face.GetNextFace();
                            numero++;
                        }
                    }
                    else
                    {
                        swApplication.SendMsgToUser("Body nullo");
                    }
                }
            }
            return entityList;
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static int KL_ComputeGenus(ModelDoc2 model, string name, SldWorks swApplication)
        {
            var genus = -1;
            var entityList = new List<Entity>();
            var edgeList = new List<Edge>();
            var verticesList = new List<Vertex>();
            var loopList = new List<Loop2>();

            if (model != null && (swDocumentTypes_e) model.GetType() == swDocumentTypes_e.swDocPART)
            {
                var part = (PartDoc) model;

                var bodiesList = (Array) part.GetBodies2((int) swBodyType_e.swSolidBody, false);
                //Attenzione, il body è già massimo qui!

                foreach (Body2 body in bodiesList)
                {

                    var swBody = (Body2) body.GetProcessedBody();

                    if (body != null)
                    {

                        var face = (Face2)swBody.GetFirstFace();
                        while (face != null)
                        {
                            var loop = (Loop2) face.GetFirstLoop();
                            while (loop != null)
                            {
                                if (!loop.IsSingular())
                                {
                                    loopList.Add(loop);
                                }
                                loop = loop.GetNext();
                            }

                            foreach (Edge edge in face.GetEdges())
                            {
                                if (!edgeList.Contains(edge))
                                {
                                    edgeList.Add(edge);
                                }

                                var startVertex = (Vertex) edge.GetStartVertex();
                                if (startVertex != null && !verticesList.Contains(startVertex))
                                {
                                    verticesList.Add(startVertex);
                                }

                                //if (startVertex != null)
                                //{
                                //    var arrayVertexStart = (Array) startVertex.GetPoint();
                                //    var startPoint = new MyVertex((double) arrayVertexStart.GetValue(0),
                                //        (double) arrayVertexStart.GetValue(1), (double) arrayVertexStart.GetValue(2));
                                //    if (!verticesList.Contains(startPoint))
                                //    {
                                //        verticesList.Add(startPoint);
                                //    }
                                //}
                                var endVertex = (Vertex) edge.GetEndVertex();
                                if (endVertex != null && !verticesList.Contains(endVertex))
                                {
                                    verticesList.Add(endVertex);
                                }

                                //if (endVertex != null)
                                //{
                                //    var arrayVertexEnd = (Array) endVertex.GetPoint();
                                //    var endPoint = new MyVertex((double) arrayVertexEnd.GetValue(0),
                                //        (double) arrayVertexEnd.GetValue(1), (double) arrayVertexEnd.GetValue(2));

                                //    if (!verticesList.Contains(endPoint))
                                //    {
                                //        verticesList.Add(endPoint);
                                //    }
                                //}
                            }

                            var ent = (Entity) face;
                            var saveEnt = ent.GetSafeEntity();
                            //if (saveEnt == null)
                            //{
                            //    swApplication.SendMsgToUser("entità nulla");
                            //}
                            //else
                            //{
                            //    swApplication.SendMsgToUser("entità NON nulla");
                            //}
                            entityList.Add(saveEnt);
                            //var faceNameComplete = part.GetEntityName(face);
                            //var faceName = faceNameComplete.Split('/').First();
                            //Debug.KLdebug.Print(newFaceName + " -- " + faceName + " -- " + resoult, "FaceName.txt");
                            face = face.GetNextFace();
                        }
                    }
                    else
                    {
                        swApplication.SendMsgToUser("Body nullo");
                    }

                }

                genus = ((int) verticesList.Count - edgeList.Count + 2*entityList.Count - loopList.Count - 2*bodiesList.Length)/2;

                //var toPrint = string.Format("V: {0} E: {1} F: \n{2} L: {3} S: {4} G: {5}", verticesList.Count,
                //    edgeList.Count, entityList.Count, loopList.Count, bodiesList.Length, genus);
                //swApplication.SendMsgToUser(name + "\n" + toPrint);

            }
            else
            {
                swApplication.SendMsgToUser("Componente nulla");
            }
            return -genus;
        }

        public static KLgraph.KLstatisticPart KL_GetStatisticPart(Component2 comp, double compSurface, SldWorks swApplication)
        {
            var faceNumber = 0;
            var edgeNumber = 0;
            var vertexNumber = 0;
            
            var verticesList = new List<Vertex>();

            var planarPercent = 0.0;
            var cylindricalPercent = 0.0;
            var conicalPercent = 0.0;
            var sphericalPercent = 0.0;
            var toroidalPercent = 0.0;
            var freeformPercent = 0.0;

            var planarNumber = 0;
            var cylindricalNumber = 0;
            var conicalNumber = 0;
            var sphericalNumber = 0;
            var toroidalNumber = 0;
            var freeformNumber = 0;

            if (comp != null)
            {
                var model = (ModelDoc2) comp.GetModelDoc2();
                var faceList = KL_GetPartFaces(model, comp.Name2, swApplication);
                foreach (Face2 face2 in faceList)
                {
                    faceNumber ++;
                    edgeNumber += face2.GetEdgeCount();
                    foreach (Edge edge in face2.GetEdges())
                    {
                        var startVertex = edge.GetStartVertex();
                        var endVertex = edge.GetEndVertex();

                        verticesList.Add(startVertex);
                        verticesList.Add(endVertex);
                    }
                    var surface = (Surface) face2.GetSurface();

                    if (surface.IsPlane())
                    {
                        planarPercent += face2.GetArea();
                        planarNumber++;
                    }
                    else if (surface.IsCylinder())
                    {
                        cylindricalPercent += face2.GetArea();
                        cylindricalNumber++;
                    }
                    else if (surface.IsCone())
                    {
                        conicalPercent += face2.GetArea();
                        conicalNumber++;
                    }
                    else if (surface.IsSphere())
                    {
                        sphericalPercent += face2.GetArea();
                        sphericalNumber++;
                    }
                    else if (surface.IsTorus())
                    {
                        toroidalPercent += face2.GetArea();
                        toroidalNumber++;
                    }
                    else
                    {
                        freeformPercent += face2.GetArea();
                        freeformNumber++;
                    }
                }

                planarPercent = (double) planarPercent/(double) compSurface;
                cylindricalPercent = (double) cylindricalPercent/(double) compSurface;
                conicalPercent = (double) conicalPercent/(double) compSurface;
                sphericalPercent = (double) sphericalPercent/(double) compSurface;
                toroidalPercent = (double) toroidalPercent/(double) compSurface;
                freeformPercent = (double) freeformPercent/(double) compSurface;


                var genus = KL_ComputeGenus(model, comp.Name, swApplication);

                if (comp.Name.Contains("PRT_RoulementSortieRouleaux"))
                {
                    swApplication.SendMsgToUser("Genere " + genus);
                }
                var statistic = new KLgraph.KLstatisticPart(genus, planarPercent, cylindricalPercent, conicalPercent,
                    sphericalPercent, toroidalPercent,
                    freeformPercent, planarNumber, cylindricalNumber, conicalNumber, sphericalNumber, toroidalNumber,
                    freeformNumber);
                return statistic;
            }
            var statisticEmpty = new KLgraph.KLstatisticPart();
            return statisticEmpty;
        }

        public static KLgraph.KLstatisticAssembly KL_GetStatisticAssembly(Component2 comp, SldWorks swApplication)
        {
            var subassemblyNumber = 0;
            var principalPartNumber = 0;
            var fastenerNumber = 0;
            var normalNumber = 0;
            var thinPartNumber = 0;
            var linearPatternNumber = 0;
            var circularPatternNumber = 0;
            var reflectivePatternNumber = 0;

            subassemblyNumber = comp.IGetChildrenCount();
            var statistic = new KLgraph.KLstatisticAssembly(subassemblyNumber, principalPartNumber, fastenerNumber, normalNumber, thinPartNumber,
                linearPatternNumber, circularPatternNumber, reflectivePatternNumber);

            return statistic;
        }

        public static KLgraph.KLshapePart KL_GetShapePart(Component2 comp, SldWorks swApplication)
        {
            var body = (Body2) comp.GetBody();
            var model = (ModelDoc2)comp.GetModelDoc2();
            var path = model.GetPathName();
            
            DirectoryInfo parentDir = Directory.GetParent(path.EndsWith("\\") ? path : String.Concat(path, "\\"));

            var pathParentDir = parentDir.Parent.FullName;

            //var pathComponent = Path.GetFileName(comp.GetPathName());
            var componentName = comp.Name2;

            componentName = (componentName.Split('/').Last());
            componentName = componentName.Replace("_", string.Empty);
            componentName = componentName.Substring(0, componentName.LastIndexOf('-'));
            componentName = componentName.Replace(" ", string.Empty);
            //componentName = FolderUtilities.FolderUtilities.RemoveDiacritics(componentName);

            //var componentName = ((pathComponent.Split(new[] { " - " }, StringSplitOptions.None).Last()).Replace(" ", string.Empty)).Replace("/", string.Empty);

            /*
            // Per essere coerente con le varie cazzate di Matteo, quando cerco le armoniche sferiche di una parte, non posso cercare il file con il suo nome, 
            // devo prendere come nome dall'ultimo spazio in poi,
            // perché fare il trim degli spazi era troppo complicato per un "genio" come lui!

            var componentName = comp.Name;
            componentName = componentName.Split(Convert.ToChar(" ")).Last();
            componentName = componentName.Split(Convert.ToChar("/")).Last();
            componentName = (componentName.Split(Convert.ToChar("."))).First();
            //componentName = FolderUtilities.FolderUtilities.RemoveDiacritics(componentName);
            */

            var prop = model.Extension.CreateMassProperty();
            prop.AddBodies(body);
            var part = (PartDoc) model;
            
            var volume = prop.Volume;
            var surface = prop.SurfaceArea;
            var listFirstModel = ReadSHdescriptor(swApplication, pathParentDir, componentName);

            var shapePart = new KLgraph.KLshapePart(volume, surface, listFirstModel);
            return shapePart;
        }

        public static List<double> ReadSHdescriptor(SldWorks swApplication, string pathParentDir, string componentName)
        {
            var listFirstModel = new List<double>();
            string line;
            string fileName = pathParentDir + "\\SPH\\" + componentName + ".txt";
            if (File.Exists(fileName))
            {
                StreamReader file = new StreamReader(fileName);
                while ((line = file.ReadLine()) != null)
                {
                    string[] bits = line.Split(' ');
                    foreach (string bit in bits)
                    {
                        double j;
                        if (Double.TryParse(bit, NumberStyles.Any, CultureInfo.InvariantCulture, out j))
                        {
                            listFirstModel.Add(j);
                        }
                    }
                }
                file.Close();
                listFirstModel.RemoveAt(0);
            }
            
            return listFirstModel;
        }

        public static KLgraph.KLshapeAssembly KL_GetShapeAssembly(Component2 comp, SldWorks swApplication)
        {
            var shape = new KLgraph.KLshapeAssembly();
            return shape;
        }

        public static void KL_ReadMates(ModelDoc2 swModel, SldWorks swApplication)
        {
            var swFeature = (Feature)swModel.FirstFeature();
            while (swFeature != null)
            {
                if ("MateGroup" == swFeature.GetTypeName())
                {
                    swApplication.SendMsgToUser(swFeature.Name);
                    var swSubFeature = (Feature)swFeature.GetFirstSubFeature();

                    while (swSubFeature != null)
                    {
                        var swMate = (Mate2)swSubFeature.GetSpecificFeature2();
                        if (swMate != null)
                        {
                            swApplication.SendMsgToUser("C'è la constraint: " + swMate.Type.ToString());
                        }
                        swSubFeature = swSubFeature.GetNextSubFeature();
                    }
                }

                swFeature = swFeature.GetNextFeature();
            }
        }

        public static void LCComputeRepeatedPattern(List<MyListOfInstances> ListOfMyListOfInstances, ref List<MyPatternOfComponents> listOfPattern, ref List<MyPatternOfComponents> listPattern2, ModelDoc2 SwModel, SldWorks swApplication)
        {
                StringBuilder fileOutput = new StringBuilder();
                fileOutput.AppendLine("RICERCA PATH");

                LC_AssemblyTraverse.MainPatternSearch_Assembly(ListOfMyListOfInstances, SwModel, swApplication, ref fileOutput, out listOfPattern, out listPattern2);
        }

        public static void LCComputeRepeatedPatternSameOrigin(List<MyGroupingSurface> listOfInitialGroupingSurface, ref List<MyPatternOfComponents> listOfPattern,
            ref List<MyPatternOfComponents> listPattern2, ModelDoc2 SwModel, SldWorks swApplication)
        {

            StringBuilder fileOutput = new StringBuilder();
            fileOutput.AppendLine("RICERCA PATH");

            var listOfMyGroupingSurface = new List<MyGroupingSurface>();
            
            GeometryAnalysis.MainPatternSearch_Part(false, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                                                                  ref fileOutput, swApplication);

            // Create a file to write to. 
            string mydocpath = @"C:\Users\Katia Lupinetti\Desktop\Debug";
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outfile = new StreamWriter(mydocpath + @"\PathCreation.txt", true))
            {
                outfile.Write(fileOutput.ToString());
                outfile.Close();
            }

        }

        //This function gets the transform matrix of a given component respect to its father.
        public static MyTransformMatrix GetTransformMatrix(Component2 swComponent, SldWorks swApplication)
        {
            var componentXForm = swComponent.GetTotalTransform(true);
            
            var newMyTransformMatrix = new MyTransformMatrix();
            if (componentXForm != null)
            {
                var xForm = (Array) componentXForm.ArrayData;
                double[,] newRotationMatrix = new double[3, 3]
                {
                    {(double) xForm.GetValue(0), (double) xForm.GetValue(3), (double) xForm.GetValue(6)},
                    {(double) xForm.GetValue(1), (double) xForm.GetValue(4), (double) xForm.GetValue(7)},
                    {(double) xForm.GetValue(2), (double) xForm.GetValue(5), (double) xForm.GetValue(8)}
                };
                double[] newTranslationalVector =
                    {(double) xForm.GetValue(9), (double) xForm.GetValue(10), (double) xForm.GetValue(11)};
                newMyTransformMatrix = new MyTransformMatrix(newRotationMatrix, newTranslationalVector,
                    (double) xForm.GetValue(12));
                
            }
            else
            {
                swApplication.SendMsgToUser("La componente " + swComponent.Name +
                                            " ha matrice di trasformazione NULLA ---->> ERRORE.");
            }


            return newMyTransformMatrix;
        }
    }
}
