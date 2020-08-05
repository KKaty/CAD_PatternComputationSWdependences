using System;
using System.Collections.Generic;
using Accord.Math;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
//using Functions.DataStructure;
//using Functions.Functions;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public static partial class ExtractInfoFromBRep
    {
        // This function takes - the List<object> corresponding to the set of faces of a new RE 
        // .                   - the progression number of the new RE in the list of selected entities
        // It returns a new MyRepeatedEntity

        public static MyRepeatedEntity BuildRepeatedEntity(List<object> setOfFaces, int idNewRepeatedEntity, SldWorks swapp)
        {
            List<Face2> listOfFacesForRepeatedEntity = new List<Face2>();

            List<MyVertex> listOfMyVertexForRepeatedEntity = new List<MyVertex>();
            List<MyVertex> listOfMyVertexAddedForRepeatedEntity = new List<MyVertex>();

            List<MyVertex> listOfMyVertexOfCurrentFace = new List<MyVertex>();
            List<MyVertex> listOfAddedMyVertexOfCurrentFace = new List<MyVertex>();

            var listOfMyPlane = new List<MyPlane>();
            var listOfMySphere = new List<MySphere>();
            var listOfMyCone = new List<MyCone>();
            var listOfMyCylinder = new List<MyCylinder>();
            var listOfMyTorus = new List<MyTorus>();

            //const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            //string whatToWrite = "";
            //whatToWrite = string.Format("Numero di facce : {0}", setOfFaces.Count);
            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            foreach (object obj in setOfFaces)
            {
                // face storing and classification:
                var face = (Face2)obj;
                listOfFacesForRepeatedEntity.Add(face);

                ClassifyFace(face, ref listOfMyPlane, ref listOfMySphere,
                    ref listOfMyCone, ref listOfMyCylinder, ref listOfMyTorus);

                // vertex storing and classification (from a current face):
                listOfMyVertexOfCurrentFace.Clear();
                listOfAddedMyVertexOfCurrentFace.Clear();
               // FunctionsKL.BRepFunctions.MyGetPointFromFaceEntity(face, ref listOfMyVertexOfCurrentFace, ref listOfAddedMyVertexOfCurrentFace);
                // If in the face itself there are two MyVertex repeated (in listOfMyVertexOfCurrentFace) with 
                // the following foreach instruction they are stored only once.
                foreach (MyVertex myVert in listOfMyVertexOfCurrentFace)
                {
                    if (!(listOfMyVertexForRepeatedEntity.Contains(myVert)))
                    {
                        listOfMyVertexForRepeatedEntity.Add(myVert);
                        //whatToWrite = string.Format("Aggiunto Vertice normale: ({0},{1},{2})", myVert.x, myVert.y, myVert.z);
                        //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    }
                }

                // If in the face itself there are two MyVertex repeated (in listOfAddedMyVertexOfCurrentFace) with 
                // the following foreach instruction they are stored only once.
                foreach (MyVertex myVert in listOfAddedMyVertexOfCurrentFace)
                {
                    if (!(listOfMyVertexAddedForRepeatedEntity.Contains(myVert)))
                    {
                        listOfMyVertexAddedForRepeatedEntity.Add(myVert);
                        //whatToWrite = string.Format("Aggiunto Vertice su edge curvo chiuso: ({0},{1},{2})", myVert.x, myVert.y, myVert.z);
                        //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    }
                }
            }           

            // Centroid computation:
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);
            var listOfAllVertices = new List<MyVertex>();
            listOfAllVertices.AddRange(listOfMyVertexForRepeatedEntity);
            listOfAllVertices.AddRange(listOfMyVertexAddedForRepeatedEntity);
            MyVertex outputCentroid = computeCentroidsOfVertices(listOfAllVertices);   // centroid of the vertices
            //whatToWrite = string.Format("Baricentro ({0},{1},{2})", outputCentroid.x, outputCentroid.y, outputCentroid.z);
            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            // Creation of new MyRepeatedEntity:
            MyRepeatedEntity outputRepeatedEntity = new MyRepeatedEntity(idNewRepeatedEntity, listOfFacesForRepeatedEntity, outputCentroid,
                listOfMyVertexForRepeatedEntity, listOfMyVertexAddedForRepeatedEntity, listOfMyPlane, listOfMySphere, listOfMyCone,
                                listOfMyCylinder, listOfMyTorus);
            
            return outputRepeatedEntity;
        }

        public static MyRepeatedEntity KLBuildRepeatedEntity(List<Entity> entityList, int idNewRepeatedEntity, double[,] compositionMatrixOfComponentPart, SldWorks swapp)
        {
            List<Face2> listOfFacesForRepeatedEntity = new List<Face2>(); 
          
            List<MyVertex> listOfMyVertexForRepeatedEntity = new List<MyVertex>();
            List<MyVertex> listOfMyVertexAddedForRepeatedEntity = new List<MyVertex>();

            List<MyVertex> listOfMyVertexOfCurrentFace = new List<MyVertex>();
            List<MyVertex> listOfAddedMyVertexOfCurrentFace = new List<MyVertex>();

            var listOfMyPlane = new List<MyPlane>();
            var listOfMySphere = new List<MySphere>();
            var listOfMyCone = new List<MyCone>();
            var listOfMyCylinder = new List<MyCylinder>();
            var listOfMyTorus = new List<MyTorus>();

            //const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            //string whatToWrite = "";
            //whatToWrite = string.Format("Numero di facce : {0}", entityList.Count);
           // KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            foreach (Entity entity in entityList)
            {
                // face storing and classification:

                var face = (Face2)entity;
                if (((((Surface)face.GetSurface()).Identity() == (int)swSurfaceTypes_e.PLANE_TYPE)) ||
                    ((((Surface)face.GetSurface()).Identity() == (int)swSurfaceTypes_e.CYLINDER_TYPE)) ||
                    ((((Surface)face.GetSurface()).Identity() == (int)swSurfaceTypes_e.SPHERE_TYPE)))
                {
                    listOfFacesForRepeatedEntity.Add(face);

                    KLClassifyFace(entity, ref listOfMyPlane, ref listOfMySphere,
                        ref listOfMyCone, ref listOfMyCylinder, ref listOfMyTorus, compositionMatrixOfComponentPart);
                    
                    // vertex storing and classification (from a current face):
                    listOfMyVertexOfCurrentFace.Clear();
                    listOfAddedMyVertexOfCurrentFace.Clear();
                    BRepFunctions.MyGetPointFromFaceEntity(entity, ref listOfMyVertexOfCurrentFace,
                        ref listOfAddedMyVertexOfCurrentFace, swapp);
                    // If in the face itself there are two MyVertex repeated (in listOfMyVertexOfCurrentFace) with 
                    // the following foreach instruction they are stored only once.
                    foreach (MyVertex myVert in listOfMyVertexOfCurrentFace)
                    {
                        double[] vertexAffine =
                        {
                            myVert.x, myVert.y, myVert.z, 1
                        };
                        var newVertex = Matrix.Multiply(compositionMatrixOfComponentPart, vertexAffine);
                        var vertexToAdd = new MyVertex(newVertex[0], newVertex[1], newVertex[2]);
                        if (!(listOfMyVertexForRepeatedEntity.Contains(vertexToAdd)))
                        {
                            listOfMyVertexForRepeatedEntity.Add(vertexToAdd);
                            //whatToWrite = string.Format("Aggiunto Vertice normale: ({0},{1},{2})", myVert.x, myVert.y,
                            //    myVert.z);
                            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                        }
                    }

                    // If in the face itself there are two MyVertex repeated (in listOfAddedMyVertexOfCurrentFace) with 
                    //// the following foreach instruction they are stored only once.
                    foreach (MyVertex myVert in listOfAddedMyVertexOfCurrentFace)
                    {
                        double[] vertexAffine =
                        {
                            myVert.x, myVert.y, myVert.z, 1
                        };
                        var newVertex = Matrix.Multiply(compositionMatrixOfComponentPart, vertexAffine);
                        var vertexToAdd = new MyVertex(newVertex[0], newVertex[1], newVertex[2]);
                        if (!(listOfMyVertexAddedForRepeatedEntity.Contains(vertexToAdd)))
                        {
                            listOfMyVertexAddedForRepeatedEntity.Add(vertexToAdd);
                            //whatToWrite = string.Format("Aggiunto Vertice su edge curvo chiuso: ({0},{1},{2})", myVert.x,
                            //    myVert.y, myVert.z);
                            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                        }
                    }
                }
            }

           // Centroid computation:
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);
            var listOfAllVertices = new List<MyVertex>();
            listOfAllVertices.AddRange(listOfMyVertexForRepeatedEntity);
            listOfAllVertices.AddRange(listOfMyVertexAddedForRepeatedEntity);
            MyVertex outputCentroid = computeCentroidsOfVertices(listOfAllVertices);   // centroid of the vertices
            //whatToWrite = string.Format("Baricentro ({0},{1},{2})", outputCentroid.x, outputCentroid.y, outputCentroid.z);
            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            // Creation of new MyRepeatedEntity:
            MyRepeatedEntity outputRepeatedEntity = new MyRepeatedEntity(idNewRepeatedEntity, listOfFacesForRepeatedEntity, outputCentroid,
                listOfMyVertexForRepeatedEntity, listOfMyVertexAddedForRepeatedEntity, listOfMyPlane, listOfMySphere, listOfMyCone,
                                listOfMyCylinder, listOfMyTorus, compositionMatrixOfComponentPart);

            return outputRepeatedEntity;
        }

        public static void ClassifyFace(Face2 face, ref List<MyPlane> listOfMyPlane, ref List<MySphere> listOfMySphere,
           ref List<MyCone> listOfMyCone, ref List<MyCylinder> listOfMyCylinder, ref List<MyTorus> listOfMyTorus)
        {
            var faceSurface = (Surface)face.GetSurface();
            const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            var whatToWrite = string.Format("faccia di tipo: {0}", faceSurface.Identity());
            KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            if (faceSurface.IsPlane())
            {
                var newMyPlane = CreateMyPlaneFromFace(face);
                listOfMyPlane.Add(newMyPlane);
            }

            if (faceSurface.IsSphere())
            {
                var newMySphere = CreateMySphereFromFace(faceSurface);
                listOfMySphere.Add(newMySphere);
            }

            if (faceSurface.IsCone())
            {
                var newMyCone = CreateMyConeFromFace(faceSurface);
                listOfMyCone.Add(newMyCone);
            }

            if (faceSurface.IsCylinder())
            {
                var newMyCylinder = CreateMyCylinderFromFace(faceSurface, face);
                listOfMyCylinder.Add(newMyCylinder);
            }

            if (faceSurface.IsTorus())
            {
                var newMyTorus = CreateMyTorusFromFace(faceSurface);
                listOfMyTorus.Add(newMyTorus);
            }
        }

        public static void KLClassifyFace(Entity entity, ref List<MyPlane> listOfMyPlane, ref List<MySphere> listOfMySphere, 
            ref List<MyCone> listOfMyCone, ref List<MyCylinder> listOfMyCylinder, ref List<MyTorus> listOfMyTorus, double[,] transformMatrix)
        {
            var face = (Face2)entity;
            var faceSurface = (Surface)face.GetSurface();
            //const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            //var whatToWrite = string.Format("faccia di tipo: {0}", faceSurface.Identity());
            //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

            if (faceSurface.IsPlane())
            {
                var newMyPlane = KLCreateMyPlaneFromFace(face, transformMatrix);
                listOfMyPlane.Add(newMyPlane);
            }

            if (faceSurface.IsSphere())
            {
                var newMySphere = CreateMySphereFromFace(faceSurface);
                listOfMySphere.Add(newMySphere);
            }

            if (faceSurface.IsCone())
            {
                var newMyCone = CreateMyConeFromFace(faceSurface);
                listOfMyCone.Add(newMyCone);
            }

            if (faceSurface.IsCylinder())
            {
                var newMyCylinder = KLCreateMyCylinderFromFace(faceSurface, face, transformMatrix);
                listOfMyCylinder.Add(newMyCylinder);
            }

            if (faceSurface.IsTorus())
            {
                var newMyTorus = CreateMyTorusFromFace(faceSurface);
                listOfMyTorus.Add(newMyTorus);
            }
        }

        public static MyPlane CreateMyPlaneFromFace(Face2 face)
        {
            //KLdebug.Print("FACCIA PIANA", "buildRepeatedEntity.txt");
            //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            double[] planePoint;
            var normal = GeometryFunctions.MyGetNormalForPlaneFace(face, out planePoint);
            MyPlane newMyPlane = new MyPlane(normal, planePoint);
            return newMyPlane;
        }

        public static MyPlane KLCreateMyPlaneFromFace(Face2 face, double[,] transformMatrix)
        {
            //KLdebug.Print("FACCIA PIANA", "buildRepeatedEntity.txt");
            //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            double[] planePoint;
            var normal = GeometryFunctions.KLMyGetNormalForPlaneFace(face, out planePoint, transformMatrix);
            var toPrint = string.Format("Normale calcolata {0}, {1}, {2}", normal[0], normal[1], normal[2]);
            //KLdebug.Print(toPrint, "buildRepeatedEntity.txt");
            MyPlane newMyPlane = new MyPlane(normal, planePoint);
            return newMyPlane;
        }

        public static MySphere CreateMySphereFromFace(Surface faceSurface)
        {
            KLdebug.Print("FACCIA SFERICA", "buildRepeatedEntity.txt");
            KLdebug.Print(" ", "buildRepeatedEntity.txt");

            var mySphereParameters = faceSurface.SphereParams;
            double[] mySphereCenter =
            {
                (double) mySphereParameters.GetValue(0), (double) mySphereParameters.GetValue(1),
                (double) mySphereParameters.GetValue(2)
            };
            double mySphereRay = (double)mySphereParameters.GetValue(3);

            var newMySphere = new MySphere(mySphereCenter, mySphereRay);
            return newMySphere;
        }

        public static MyCone CreateMyConeFromFace(Surface faceSurface)
        {
            KLdebug.Print("FACCIA CONICA", "buildRepeatedEntity.txt");
            KLdebug.Print(" ", "buildRepeatedEntity.txt");

            var myConeParameters = faceSurface.ConeParams;
            double[] myConeOrigin =
            {
                (double) myConeParameters.GetValue(0), (double) myConeParameters.GetValue(1),
                (double) myConeParameters.GetValue(2)
            };
            double[] myConeAxis =
            {
                (double) myConeParameters.GetValue(3), (double) myConeParameters.GetValue(4),
                (double) myConeParameters.GetValue(5)
            };
            double myConeHalfAngle = (double)myConeParameters.GetValue(7);

            var newMyCone = new MyCone(myConeOrigin, myConeAxis, myConeHalfAngle);
            return newMyCone;
        }

        public static MyCylinder CreateMyCylinderFromFace(Surface faceSurface, Face2 face)
        {
            //OSSERVAZIONE: CI POTREBBERO ESSERE EDGE CHIUSI CHE NON AVERE CENTRO SULL'ASSE
            //AD ESEMPIO EDGE CHIUSI CHE SI CREANO INTERSECANDO UN CILINDRO CON UNA SFERA
            //"INFILATA" SULLA SUA SUPERFICIE LATERALE.
            //DI QUESTI EDGE CHIUSI NON ME NE PREOCCUPO PERCHE' SONO A CONTATTO CON ALTRE FORME
            //DI CUI SPERO DI AVER CONTROLLATO (NEL CONTROLLO DELLA GEOMETRIA) IL GIUSTO 
            //POSIZIONAMENTO.

            KLdebug.Print("FACCIA CILINDRICA", "buildRepeatedEntity.txt");
            KLdebug.Print(" ", "buildRepeatedEntity.txt");

            double[] myCylinderParameters = faceSurface.CylinderParams;
            double[] myCylinderOrigin =
            {
                (double) myCylinderParameters.GetValue(0), (double) myCylinderParameters.GetValue(1),
                (double) myCylinderParameters.GetValue(2)
            };
            double[] myCylinderAxis =
            {
                (double) myCylinderParameters.GetValue(3), (double) myCylinderParameters.GetValue(4),
                (double) myCylinderParameters.GetValue(5)
            };
            double myCylinderRadius = (double)myCylinderParameters.GetValue(6);

            var newMyCylinder = new MyCylinder(myCylinderOrigin, myCylinderAxis, myCylinderRadius);

            var faceEdges = (Array)face.GetEdges();
            var numOfEdges = faceEdges.Length;

            int i = 0;
            var currentListOfBaseCircle = new List<MyCircle>();
            var currentListOfBaseEllipse = new List<MyEllipse>();

            while (i < numOfEdges)
            {
                Edge edge = (Edge)faceEdges.GetValue(i);
                Curve curveEdge = edge.GetCurve();
                if (IsClosedEdge(edge))
                {
                    //is the closed edge a circle?
                    if (curveEdge.IsCircle())
                    {
                        double[] circleParam = curveEdge.CircleParams;
                        var newBaseCircle = new MyCircle(circleParam);
                        currentListOfBaseCircle.Add(newBaseCircle);
                    }
                    //is the closed edge an ellipse?
                    if (curveEdge.IsEllipse())
                    {
                        double[] ellipseParam = curveEdge.GetEllipseParams();
                        var newBaseEllipse = new MyEllipse(ellipseParam);
                        currentListOfBaseEllipse.Add(newBaseEllipse);
                    }
                }
                i++;
            }
            newMyCylinder.listOfBaseCircle = currentListOfBaseCircle;
            newMyCylinder.listOfBaseEllipse = currentListOfBaseEllipse;
            return newMyCylinder;
        }

        public static MyCylinder KLCreateMyCylinderFromFace(Surface faceSurface, Face2 face, double[,] transformationMatrix)
        {
            //OSSERVAZIONE: CI POTREBBERO ESSERE EDGE CHIUSI CHE NON AVERE CENTRO SULL'ASSE
            //AD ESEMPIO EDGE CHIUSI CHE SI CREANO INTERSECANDO UN CILINDRO CON UNA SFERA
            //"INFILATA" SULLA SUA SUPERFICIE LATERALE.
            //DI QUESTI EDGE CHIUSI NON ME NE PREOCCUPO PERCHE' SONO A CONTATTO CON ALTRE FORME
            //DI CUI SPERO DI AVER CONTROLLATO (NEL CONTROLLO DELLA GEOMETRIA) IL GIUSTO 
            //POSIZIONAMENTO.

            //KLdebug.Print("FACCIA CILINDRICA", "buildRepeatedEntity.txt");
            //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            double[] myCylinderParameters = faceSurface.CylinderParams;
            double[] myCylinderOrigin =
            {
                (double) myCylinderParameters.GetValue(0), (double) myCylinderParameters.GetValue(1),
                (double) myCylinderParameters.GetValue(2)
            };
            double[] myCylinderAxis =
            {
                (double) myCylinderParameters.GetValue(3), (double) myCylinderParameters.GetValue(4),
                (double) myCylinderParameters.GetValue(5)
            };
            double myCylinderRadius = (double)myCylinderParameters.GetValue(6);

            if (transformationMatrix != null)
            {
                double[] myCylinderOriginAffine = { (double)myCylinderOrigin.GetValue(0), (double)myCylinderOrigin.GetValue(1), (double)myCylinderOrigin.GetValue(2), 1 };
                double[] myCylinderAxisAffine = { (double)myCylinderAxis.GetValue(0), (double)myCylinderAxis.GetValue(1), (double)myCylinderAxis.GetValue(2), 1 };

                var newMyCylinderOrigin = Matrix.Multiply(transformationMatrix, myCylinderOriginAffine);
                var newMyCylinderAxis = Matrix.Multiply(transformationMatrix, myCylinderAxisAffine);

                myCylinderOrigin.SetValue((double)newMyCylinderOrigin.GetValue(0), 0);
                myCylinderOrigin.SetValue((double)newMyCylinderOrigin.GetValue(1), 1);
                myCylinderOrigin.SetValue((double)newMyCylinderOrigin.GetValue(2), 2);

                //myCylinderAxis.SetValue((double)newMyCylinderAxis.GetValue(0), 0);
                //myCylinderAxis.SetValue((double)newMyCylinderAxis.GetValue(1), 1);
                //myCylinderAxis.SetValue((double)newMyCylinderAxis.GetValue(2), 2);
            }

            var newMyCylinder = new MyCylinder(myCylinderOrigin, myCylinderAxis, myCylinderRadius);

            var faceEdges = (Array)face.GetEdges();
            var numOfEdges = faceEdges.Length;

            int i = 0;
            var currentListOfBaseCircle = new List<MyCircle>();
            var currentListOfBaseEllipse = new List<MyEllipse>();

            while (i < numOfEdges)
            {
                Edge edge = (Edge)faceEdges.GetValue(i);
                Curve curveEdge = edge.GetCurve();
                if (IsClosedEdge(edge))
                {
                    //KLdebug.Print("trovato edge chiuso:", "buildRepeatedEntity.txt");


                    //is the closed edge a circle?
                    if (curveEdge.IsCircle())
                    {
                        double[] circleParam = curveEdge.CircleParams;
                        if (transformationMatrix != null)
                        {
                            double[] circleCenterAffine = { (double)circleParam.GetValue(0), (double)circleParam.GetValue(1), (double)circleParam.GetValue(2), 1 };

                            var newMyCircleOrigin = Matrix.Multiply(transformationMatrix, circleCenterAffine);

                            circleParam.SetValue((double)newMyCircleOrigin.GetValue(0), 0);
                            circleParam.SetValue((double)newMyCircleOrigin.GetValue(1), 1);
                            circleParam.SetValue((double)newMyCircleOrigin.GetValue(2), 2);
                        }
                        var newBaseCircle = new MyCircle(circleParam);
                        currentListOfBaseCircle.Add(newBaseCircle);
                    }
                    //is the closed edge an ellipse?
                    if (curveEdge.IsEllipse())
                    {
                        double[] ellipseParam = curveEdge.GetEllipseParams();

                        if (transformationMatrix != null)
                        {
                            double[] circleCenterAffine = { (double)ellipseParam.GetValue(0), (double)ellipseParam.GetValue(1), (double)ellipseParam.GetValue(2), 1 };

                            var newMyCircleOrigin = Matrix.Multiply(transformationMatrix, circleCenterAffine);

                            ellipseParam.SetValue((double)newMyCircleOrigin.GetValue(0), 0);
                            ellipseParam.SetValue((double)newMyCircleOrigin.GetValue(1), 1);
                            ellipseParam.SetValue((double)newMyCircleOrigin.GetValue(2), 2);
                        }

                        var newBaseEllipse = new MyEllipse(ellipseParam);
                        currentListOfBaseEllipse.Add(newBaseEllipse);
                    }

                    KLdebug.Print(" ", "buildRepeatedEntity.txt");
                }
                i++;
            }
            newMyCylinder.listOfBaseCircle = currentListOfBaseCircle;
            newMyCylinder.listOfBaseEllipse = currentListOfBaseEllipse;
            return newMyCylinder;
        }


        public static MyTorus CreateMyTorusFromFace(Surface faceSurface)
        {
            KLdebug.Print("FACCIA TORO", "buildRepeatedEntity.txt");
            KLdebug.Print(" ", "buildRepeatedEntity.txt");

            var myTorusParameters = faceSurface.TorusParams;
            double[] myTorusCenter =
            {
                (double) myTorusParameters.GetValue(0), (double) myTorusParameters.GetValue(1),
                (double) myTorusParameters.GetValue(2)
            };
            double[] myTorusAxis =
            {
                (double) myTorusParameters.GetValue(3), (double) myTorusParameters.GetValue(4),
                (double) myTorusParameters.GetValue(5)
            };
            double myTorusMajorRadius = (double)myTorusParameters.GetValue(6);
            double myTorusMinorRadius = (double)myTorusParameters.GetValue(7);

            var newMyTorus = new MyTorus(myTorusCenter, myTorusAxis, myTorusMajorRadius, myTorusMinorRadius);
            return newMyTorus;
        }

        //To verify if an edge is closed:
        public static bool IsClosedEdge(Edge inputEdge)
        {
            if (inputEdge != null)
            {
                //var edgeCurve = (Curve)inputEdge.GetCurve();
                var vertexS = (Vertex)inputEdge.GetStartVertex();
                var vertexE = (Vertex)inputEdge.GetEndVertex();

                if (vertexS == null && vertexE == null) // i.e. closed edge
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }       


        //This function returns the centroid of a set of given MyVertex
        public static MyVertex computeCentroidsOfVertices(List<MyVertex> listPointVertex)
        {
            double sumOfx = 0;
            double sumOfy = 0;
            double sumOfz = 0;
            int numOfVerteces = listPointVertex.Count;

            foreach (MyVertex vertex in listPointVertex)
            {
                sumOfx += vertex.x;
                sumOfy += vertex.y;
                sumOfz += vertex.z;
            }
            MyVertex centroid = new MyVertex(sumOfx / numOfVerteces, sumOfy / numOfVerteces, sumOfz / numOfVerteces);
            return centroid;
            
        }
    }

    
}
