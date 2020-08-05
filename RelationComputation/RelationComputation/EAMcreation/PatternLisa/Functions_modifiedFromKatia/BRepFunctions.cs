using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia
{
    public class BRepFunctions
    {
        /// <summary>
        /// The my adjacence face.
        /// </summary>
        /// <param name="face">
        /// The face.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static List<Face2> MyAdjacenceFace(Face2 face, SldWorks swApplWorks)
        {
            var listAdjacentFace = new List<Face2>();

            // Aggiungo la faccia stessa e tutte le sue adiacenti,
            // Se non aggiungo anche se stessa poi controlla la presenza di edge virtuali tra sé e sé.
           // listAdjacentFace.Add(face);
            var myEdgeOfFace = (Array) face.GetEdges();
            foreach (Edge edge in myEdgeOfFace)
            {
                Array myAdiacenceFaces = edge.GetTwoAdjacentFaces2();
                Face2 myAdiacenceFace = null;
                var firstFace = (Face2) myAdiacenceFaces.GetValue(0);
                var secondFace = (Face2) myAdiacenceFaces.GetValue(1);

                // Se non c'è un'altra faccia adiacente allora ho un valore NULL e c'è un errore nel modello
                //(se ho un modello manifold devo avere per ogni edge due facce).
                if (firstFace == null || secondFace == null)
                {
                    swApplWorks.SendMsgToUser("ERROR: the model is defective and the adjacency relation cannot be stored correctly (NOT ALL THE EDGES ARE SHARED BY 2 FACES).");
                    return listAdjacentFace;
                }
            
                // Se la faccia non è quella che ho già la salvo come faccia adiacente, altrimenti salvo l'altra.               
                if (firstFace.GetFaceId() == face.GetFaceId())
                {
                    myAdiacenceFace = secondFace;
                }
                else if (secondFace.GetFaceId() == face.GetFaceId())
                {
                    myAdiacenceFace = firstFace;
                }

                listAdjacentFace.Add(myAdiacenceFace);
            }

            return listAdjacentFace;
        }


        // VERSIONE ORIGINARIA DI KATIA di MyGetPointFromFaceEntity
        /*
    public static List<double> MyGetPointFromFaceEntity(Face2 face)
    {
        //var face = (Face2)entity.GetSafeEntity();
        var listPointFace = new List<double>();

        var listEdgeFace = (Array) face.GetEdges();

        foreach (Edge edge in listEdgeFace)
        {
            if (edge != null)
            {
                var edgeCurve = (Curve) edge.GetCurve();
                var vertexS = (Vertex) edge.GetStartVertex();
                double[] startPoint = new double[3];
                if (vertexS != null)
                {
                    startPoint = vertexS.GetPoint();
                }
                else
                {
                    var curveParaData = (CurveParamData) edge.GetCurveParams3();
                    double[] vStartPoint = (double[]) curveParaData.StartPoint;
                    for (int i = 0; i <= vStartPoint.GetUpperBound(0); i++)
                    {
                        startPoint[i] = vStartPoint[i];
                    }
                }

                var vertexE = (Vertex) edge.GetEndVertex();
                double[] endPoint = new double[3];
                if (vertexE != null)
                {
                    endPoint = vertexE.GetPoint();
                }
                else
                {
                    var curveParaData = (CurveParamData) edge.GetCurveParams3();
                    double[] vEndPoint = (double[]) curveParaData.EndPoint;
                    for (int i = 0; i <= vEndPoint.GetUpperBound(0); i++)
                    {
                        endPoint[i] = vEndPoint[i];
                    }

                }

                for (int i = 0; i < startPoint.Length; i++)
                {
                    listPointFace.Add((double) startPoint.GetValue(i));
                }
                for (int i = 0; i < startPoint.Length; i++)
                {
                    listPointFace.Add((double) endPoint.GetValue(i));
                }

            }
        }

        return listPointFace;
    }
    */

        // VERSIONE MODIFICATA LISA di MyGetPointFromFaceEntity
        /*
        public static List<double> MyGetPointFromFaceEntity(Face2 face)
        {
            const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            var whatToWrite = " ";
            //var face = (Face2)entity.GetSafeEntity();
            var listPointFace = new List<double>();

            var listEdgeFace = (Array) face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                if (edge != null)
                {
                    var edgeCurve = (Curve) edge.GetCurve();
                    var curveParaData = (CurveParamData)edge.GetCurveParams3();
                    var vertexS = (Vertex) edge.GetStartVertex();
                    double[] startPoint = new double[3];
                    if (vertexS != null)
                    {
                        startPoint = vertexS.GetPoint();
                        whatToWrite = string.Format("Vertice startpoint");
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    }
                    else
                    {                       
                        double[] vStartPoint = (double[]) curveParaData.StartPoint;
                        for (int i = 0; i <= vStartPoint.GetUpperBound(0); i++)
                        {
                            startPoint[i] = vStartPoint[i];
                        }
                        whatToWrite = string.Format("Vertice startpoint da null");
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    }

                    var vertexE = (Vertex) edge.GetEndVertex();
                    double[] endPoint = new double[3];
                    if (vertexE != null)
                    {
                        endPoint = vertexE.GetPoint();
                        whatToWrite = string.Format("Vertice endpoint");
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    }
                    else
                    {
                        double[] vEndPoint = (double[]) curveParaData.EndPoint;
                        for (int i = 0; i <= vEndPoint.GetUpperBound(0); i++)
                        {
                            endPoint[i] = vEndPoint[i];
                        }
                        whatToWrite = string.Format("Vertice endpoint da null");
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                    }

                    for (int i = 0; i < startPoint.Length; i++)
                    {
                        listPointFace.Add((double) startPoint.GetValue(i));
                    }
                    for (int i = 0; i < endPoint.Length; i++)
                    {
                        listPointFace.Add((double) endPoint.GetValue(i));
                    }

                    if (!(edgeCurve.IsLine()))
                    {
                        var maxParameter = curveParaData.UMaxValue;
                        var minParameter = curveParaData.UMinValue;
                        var meanParameter = (maxParameter + minParameter) / 2;
                        double[] midPoint = edgeCurve.Evaluate(meanParameter);
                        string str = "Vertice nuovo: ";
                        for (int i = 0; i < 3; i++)
                        {
                            listPointFace.Add((double)midPoint.GetValue(i));
                            str += midPoint.GetValue(i).ToString() + " ";
                        }
                        whatToWrite = string.Format("Vertice midpoint");
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                        KLdebug.Print(str, fileNameBuildRepeatedEntity);

                    }
                    

                }
            }

            return listPointFace;
        }
         * */

        // VERSIONE 2 DI LISA
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void MyGetPointFromFaceEntity(Entity entity, ref List<MyVertex> listMyVertexFace,
            ref List<MyVertex> listAddedMyVertex, SldWorks swApplication)
        {
            // In both cases, if the underlying curve is not a line, I add a new MyVertex:
            // - in case of open edge, it is in the middle of the edge
            // - in case of closed edge, it is opposite to the firstVertex

            //const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            //var whatToWrite = " ";

            var listEdgeFace = (Array)((Face2)entity).GetEdges();
            var listEntityEdges = new List<Entity>();

            foreach (var oldEdge in listEdgeFace)
            {
                var ent = (Entity) oldEdge;
                Entity safeEdge;

                safeEdge = ent.GetSafeEntity();

                listEntityEdges.Add(safeEdge);
            }
            //swApplication.SendMsgToUser(listEdgeFace.Length + " spigoli nella faccia");

            foreach (var ent in listEntityEdges)
            {
                if (ent != null)
                {
                    //var edge = ent as Edge;

                    var vertexS = (Vertex)((Edge)ent).GetStartVertex();
                    var vertexE = (Vertex)((Edge)ent).GetEndVertex();

                    if (vertexS != null && vertexE != null) // i.e. open edge
                    {
                        var edgeCurve = (Curve)((Edge)ent).GetCurve();
                        if (edgeCurve != null)
                        {
                            var arrayVertexStart = (Array) vertexS.GetPoint();
                            var startPoint = new MyVertex((double) arrayVertexStart.GetValue(0),
                                (double) arrayVertexStart.GetValue(1), (double) arrayVertexStart.GetValue(2));

                            var arrayVertexEnd = (Array) vertexE.GetPoint();
                            var endPoint = new MyVertex((double) arrayVertexEnd.GetValue(0),
                                (double) arrayVertexEnd.GetValue(1), (double) arrayVertexEnd.GetValue(2));

                            //there are situation of startPoint and endPoint not null but coinciding
                            //(these cases must be excluded and considered as closed edges)
                            if (!startPoint.Equals(endPoint))
                            {
                                listMyVertexFace.Add(startPoint);
                                //whatToWrite = string.Format("Vertice startpoint ({0},{1},{2})", startPoint.x,
                                //    startPoint.y,
                                //    startPoint.z);
                                //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                                listMyVertexFace.Add(endPoint);
                                //whatToWrite = string.Format("Vertice endpoint ({0},{1},{2})", endPoint.x, endPoint.y,
                                //    endPoint.z);
                                //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                                edgeCurve = (Curve)((Edge)ent).GetCurve();
                                if (!(edgeCurve.IsLine()))
                                {
                                    var midPoint = CreateMidMyVertex(ent);

                                    listAddedMyVertex.Add(midPoint);
                                    //whatToWrite = string.Format(
                                    //    "Vertice creato midpoint su edge curvo aperto ({0},{1},{2})", midPoint.x,
                                    //    midPoint.y,
                                    //    midPoint.z);
                                    //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                                }
                            }
                            //else // i.e. closed edge
                            //{
                            //    double[] arrayVertex = (double[]) curveParaData.StartPoint;
                            //        // == curveParaData.EndPoint ??
                            //    var newVertex = new MyVertex((double) arrayVertex.GetValue(0),
                            //        (double) arrayVertex.GetValue(1), (double) arrayVertex.GetValue(2));

                            //    listAddedMyVertex.Add(startPoint);
                            //    whatToWrite = string.Format("Null: vertice creato ({0},{1},{2})", newVertex.x,
                            //        newVertex.y,
                            //        newVertex.z);
                            //    KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);


                            //    if (!(edgeCurve.IsLine()))
                            //    {
                            //        var midPoint = CreateMidMyVertex(curveParaData, edgeCurve);

                            //        listAddedMyVertex.Add(midPoint);
                            //        whatToWrite = string.Format(
                            //            "Vertice creato midpoint su edge curvo chiuso ({0},{1},{2})", midPoint.x,
                            //            midPoint.y,
                            //            midPoint.z);
                            //        //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                            //    }
                            //}
                        }
                    }
                    else
                    // i.e. closed edge?
                    {
                        var curveParaData = (CurveParamData)((Edge)ent).GetCurveParams3();
                        double[] arrayVertex = (double[])curveParaData.StartPoint; // == curveParaData.EndPoint ??
                        var newVertex = new MyVertex((double)arrayVertex.GetValue(0), (double)arrayVertex.GetValue(1),
                            (double)arrayVertex.GetValue(2));
                      
                        listAddedMyVertex.Add(newVertex);
                        //whatToWrite = string.Format("Null: vertice creato ({0},{1},{2})", newVertex.x, newVertex.y,
                        //    newVertex.z);
                        // KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                        var edgeCurve = (Curve)((Edge)ent).GetCurve();
                        if (!(edgeCurve.IsLine()))
                        {
                            var midPoint = CreateMidMyVertex(ent);

                            listAddedMyVertex.Add(midPoint);
                            //whatToWrite = string.Format(
                            //    "Vertice creato midpoint su edge curvo chiuso ({0},{1},{2})",
                            //    midPoint.x, midPoint.y,
                            //    midPoint.z);
                            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                        }
                    }
                }
            }

        }

        public static MyVertex CreateMidMyVertex(Entity ent)
        {
            var edgeCurve = (Curve) ((Edge) ent).GetCurve();
            var curveParaData = (CurveParamData) ((Edge) ent).GetCurveParams3();
            var maxParameter = curveParaData.UMaxValue;
            var minParameter = curveParaData.UMinValue;
            var meanParameter = (maxParameter - minParameter)/2;
            double[] arrayVertexMid = edgeCurve.Evaluate(meanParameter);
            var midPoint = new MyVertex((double) arrayVertexMid.GetValue(0),
                (double) arrayVertexMid.GetValue(1), (double) arrayVertexMid.GetValue(2));
            return midPoint;
        }

        /// <summary>
        /// The my get vertex from face.
        /// Funzione che estrae i vertici di una faccia solo se l'edge formato da quei vertici è una linea!
        /// </summary>
        /// <param name="face">
        /// The face.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static List<Vertex> MyGetVertexFromFace(object obj)
        {
            var entity = (Entity)obj;
            var face = (Face2) entity.GetSafeEntity();
            var listVertexFace = new List<Vertex>();
            var listEdgeFaceCount = face.GetEdgeCount();
                // Anche se non è utilizzato è necessario per non far disconnettere l'oggetto, evitando il refresh.
            var listEdgeFace = (Array) face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                if (edge != null)
                {
                    var edgeCurve = (Curve) edge.GetCurve();
                    if (edgeCurve.IsLine())
                        // --> Non è da fare solo per edge piani, ma per tutti, con piccolo accorgimento
                        //Se lo start vertex o l'end vertex sono nulli, allora si chiama la funzione MyPointOfCircleEdge e si salva quel punto.
                    {
                        var vertexS = (Vertex) edge.GetStartVertex();
                            // Se è nullo si chiama MyPointOfCircleEdge con t=0
                        if (!listVertexFace.Contains(vertexS))
                        {
                            listVertexFace.Add(vertexS);
                        }

                        var vertexE = (Vertex) edge.GetEndVertex(); // Se è nullo si chiama MyPointOfCircleEdge con t=1
                        if (!listVertexFace.Contains(vertexE))
                        {
                            listVertexFace.Add(vertexE);
                        }
                    }
                }
            }

            return listVertexFace;
        }

        public static List<object> GetSelectedFaces(SldWorks swApplication, ModelDoc2 swModel)
        {
            swModel = swApplication.ActiveDoc;
            var swSafeEntityList = new List<object>();

            var modelType = swModel.GetTitle();
            var documentType = (swDocumentTypes_e)swModel.GetType();

            if (documentType == swDocumentTypes_e.swDocPART)
            {

                var myPartDoc = (PartDoc)swModel;

                Array myBodyVar = myPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
                int myBodyCount = 0;
                for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
                {
                    var myBodyComparison = (Body2)myBodyVar.GetValue(myBodyCount);

                    var swFace = (Face2)myBodyComparison.GetFirstFace();
                    while (swFace != null)
                    {
                        var idNode = swFace.GetHashCode();
                        swFace.SetFaceId(idNode);
                        swFace = swFace.GetNextFace();
                    }

                }

                var mySelManager = (SelectionMgr) swModel.SelectionManager;
                var selectionCount = mySelManager.GetSelectedObjectCount2(-1);
                //swApplication.SendMsgToUser("Numero oggetti selezionati " + selectionCount.ToString());
                for (int i = 0; i < selectionCount; i++)
                {
                    var myFace = (Face2) mySelManager.GetSelectedObject6(i + 1, -1);
                  //  var idNode = myFace.GetHashCode();
                  //  myFace.SetFaceId(idNode);
                    var swEntity = ((Entity) myFace).GetSafeEntity();
                    var swSafeEntity = (Entity) swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                }
            }

            //var listFace = new List<Face2>();
            //swApplication.SendMsgToUser("Lista facce " + listFace.Count.ToString());
            //foreach (Face2 face2 in listFace)
            //{
            //    swApplication.SendMsgToUser("Id: " + face2.GetFaceId().ToString());
            //}
            //ColorFace.KLColorFace(listFace, swApplication);
            return swSafeEntityList;
        }

        //To load an assembly model
        public static Component2 GetRootComponent(SldWorks swApplication, ModelDoc2 swModel)
        {
            Component2 swRootComponent = null;
            swModel = swApplication.ActiveDoc;
            var swSafeEntityList = new List<object>();

            var modelType = swModel.GetTitle();
            var documentType = (swDocumentTypes_e) swModel.GetType();
            if (documentType == swDocumentTypes_e.swDocASSEMBLY)
            {

                var assemblyModel = (AssemblyDoc)swModel;

                var configuration = (Configuration)swModel.GetActiveConfiguration();
                swRootComponent = configuration.GetRootComponent();

                #region FACE ID SET (not used)

                //QUESTA PARTE SERVIVA PER SETTARE L'ID DELLE FACCE
                // (NON FUNZIONA SE UNA DELLE COMPONENTI è UN SOTTOASSEMBLATO, PRENDE TUTTE
                // LE COMPONENTI DEL 1° LIVELLO)
                //var listComponents = (Array)assemblyModel.GetComponents(true);
                //swApplication.SendMsgToUser("Ho scomposto il model in componenti. NUmero compo: " + listComponents.Length);
                //foreach (Component2 component in listComponents)
                //{

                //var bodies = (Array)component.GetBodies2((int)swBodyType_e.swAllBodies);
                //swApplication.SendMsgToUser("numero body " + bodies.Length.ToString());

                //        foreach (Body2 body in bodies)
                //        {
                //          //  var procBody = body.GetProcessedBodyWithSelFace();
                //          //  var swFace = (Face2)procBody.GetFirstSelectedFace();
                //            var swFace = (Face2) body.GetFirstFace();
                //            while (swFace != null)
                //            {
                //              //  swApplication.SendMsgToUser("C'è un faccia ");
                //                var idNode = swFace.GetHashCode();
                //                swFace.SetFaceId(idNode);
                //                swFace = swFace.GetNextFace();
                //            }
                //        }

                //}

                //QUESTA PARTE SERVIVA PER SALVARE LE FACCE SELEZIONATE CON CLICK
                //    var mySelManager = (SelectionMgr)swModel.SelectionManager;
                //    var selectionCount = mySelManager.GetSelectedObjectCount2(-1);
                //    //swApplication.SendMsgToUser("Numero oggetti selezionati " + selectionCount.ToString());
                //    for (int i = 0; i < selectionCount; i++)
                //    {
                //        var myFace = (Face2)mySelManager.GetSelectedObject6(i + 1, -1);
                //        //  var idNode = myFace.GetHashCode();
                //        //  myFace.SetFaceId(idNode);
                //        listFace.Add(myFace);
                //        var swEntity = ((Entity)myFace).GetSafeEntity();
                //        var swSafeEntity = (Entity)swEntity.GetSafeEntity();
                //        swSafeEntityList.Add(swSafeEntity);
                //    }

                #endregion
            }

            
            return swRootComponent;

        }

        public static List<object> GetFaces(SldWorks swApplication, ModelDoc2 swModel)
        {
            //swModel = swApplication.ActiveDoc;
            var myPartDoc = (PartDoc)swModel;

            Array myBodyVar = myPartDoc.GetBodies2((int) swBodyType_e.swAllBodies, true);
            var swSafeEntityList = new List<object>();
            int myBodyCount = 0;
            for (myBodyCount = myBodyVar.GetLowerBound(0); myBodyCount <= myBodyVar.GetUpperBound(0); myBodyCount++)
            {
                var myBodyComparison = (Body2) myBodyVar.GetValue(myBodyCount);
               
                var swFace = (Face2) myBodyComparison.GetFirstFace();
                while (swFace != null)
                {
                    var swEntity = ((Entity) swFace).GetSafeEntity();
                    var swSafeEntity = (Entity) swEntity.GetSafeEntity();
                    swSafeEntityList.Add(swSafeEntity);
                    swFace = swFace.GetNextFace();
                }

            }


            return swSafeEntityList;
        }
    
    }
}
