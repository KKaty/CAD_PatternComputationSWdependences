using System;
using System.Collections.Generic;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {

        //It detect the maximum symmetry relation in a set of MyRepeatedEntity, starting from index i of the list of MyRE
        public static bool GetMaximumRotation(List<MyRepeatedEntity> listOfREOnThePath, MyCircumForPath pathObject,
            ref int i, ref int numOfRE, ref bool noStop, ref MyPattern outputPattern,
            List<MyGroupingSurface> listOfInitialGroupingSurface)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE", nameFile);
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            var listOfREOfNewMyPattern = new List<MyRepeatedEntity> { listOfREOnThePath[i] };
            var lengthOfCurrentPath = 1;   // = it represents the number of couples related by translation
            var exit = false;

            //Computation of the rotation angle:
            double[] planeNormal =
            {
                pathObject.circumplane.a, 
                pathObject.circumplane.b,
                pathObject.circumplane.c
            };
            var teta = FunctionsLC.FindAngle(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid, pathObject.circumcenter);

            var axisDirection = establishAxisDirection(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid,
                pathObject.circumcenter, planeNormal);

            //var SwModel = (ModelDoc2)SwApplication.ActiveDoc;
            //SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();
            //SwModel.CreatePoint2(pathObject.circumcenter.x, pathObject.circumcenter.y, pathObject.circumcenter.z);
            //SwModel.InsertSketch();

            while (i < (numOfRE - 1) && exit == false) //fino alla penultima RE
            {
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print(" ", nameFile);
                //whatToWrite = string.Format("         Confronto {0}^ RE e la {1}^ RE: ", i, i + 1);
                //KLdebug.Print(whatToWrite, nameFile);

                if (IsRotationTwoRE(listOfREOnThePath[i], listOfREOnThePath[i + 1], teta, axisDirection, pathObject.circumcenter))
                {
                    listOfREOfNewMyPattern.Add(listOfREOnThePath[i + 1]);
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
                outputPattern.listOfMyREOfMyPattern = listOfREOfNewMyPattern;
                outputPattern.pathOfMyPattern = pathObject;
                var listOfGroupingSurfaceForThisPattern = findGroupingSurfacesForThisPattern(listOfInitialGroupingSurface,
                                    listOfREOfNewMyPattern, lengthOfCurrentPath);
                outputPattern.listOfGroupingSurfaces = listOfGroupingSurfaceForThisPattern;
                //KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);

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

        }
        /*
         * //It detect the maximum symmetry relation in a set of MyRepeatedEntity, starting from index i of the list of MyRE
        public static MyPattern GetMaximumRotation(List<MyRepeatedEntity> listOfREOnThePath, MyCircumForPath pathObject,
            ref int i, ref int numOfRE, ref MyVertex circumCenter, ref MyVertex centerSphereOfCircumMyVertex, ref bool noStop)
        {
            const string nameFile = "GetRotationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("NUOVO AVVIO DI RICERCA ROTAZIONE", nameFile);
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";

            var newMyPattern = new MyPattern();

            var listOfREOfNewMyPattern = new List<MyRepeatedEntity> { listOfREOnThePath[i] };
            var lengthOfCurrentPath = listOfREOfNewMyPattern.Count;   // = number of couple possibly related by translation (= number of repetition of distance d) 
            var exit = false;

            //Computation of the rotation angle:
            //var pathCircumference = (MyCircumForPath)pathObject;
            //var 
            double[] centersphere =
            {
                pathObject.circumsphere.centerSphere[0],
                pathObject.circumsphere.centerSphere[1], pathObject.circumsphere.centerSphere[2]
            };
            whatToWrite = string.Format("centersphere ({0},{1},{2}) ", centersphere[0], centersphere[1], centersphere[2]);
            KLdebug.Print(whatToWrite, nameFile);

            centerSphereOfCircumMyVertex = new MyVertex(centersphere[0], centersphere[1], centersphere[2]);
            double[] planeNormal =
            {
                pathObject.circumplane.a, 
                pathObject.circumplane.b,
                pathObject.circumplane.c
            };
            var pointOnAxisMyVertex = new MyVertex(centerSphereOfCircumMyVertex.x + planeNormal[0],
                centerSphereOfCircumMyVertex.y + planeNormal[1], centerSphereOfCircumMyVertex.z + planeNormal[2]);

            var rotationAxis = FunctionsLC.LinePassingThrough(centerSphereOfCircumMyVertex, pointOnAxisMyVertex);

            //var 
            circumCenter = FunctionsLC.PointIntersectionOfPlaneAndLine(pathObject.circumplane,
                rotationAxis);

            var teta = FunctionsLC.FindAngle(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid, circumCenter);

            var axisDirection = establishAxisDirection(listOfREOnThePath[0].centroid, listOfREOnThePath[1].centroid,
                circumCenter, planeNormal);
            //var axisDirection = planeNormal;

            while (i < (numOfRE - 1) && exit == false) //fino alla penultima RE
            {
                KLdebug.Print(" ", nameFile);
                KLdebug.Print(" ", nameFile);
                whatToWrite = string.Format("         Confronto {0}^ RE e la {1}^ RE: ", i, i + 1);
                KLdebug.Print(whatToWrite, nameFile);

                if (IsRotationTwoRE(listOfREOnThePath[i], listOfREOnThePath[i + 1], teta, axisDirection, circumCenter))
                {
                    listOfREOfNewMyPattern.Add(listOfREOnThePath[i + 1]);
                    lengthOfCurrentPath += 1;
                    KLdebug.Print("Aggiunta " + (i + 1) + "-esima RE al MYPattern. lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
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

            if (lengthOfCurrentPath > 1)
            {
                newMyPattern.listOfMyREOfMyPattern = listOfREOfNewMyPattern;
                newMyPattern.pathOfMyPattern = pathObject;
                KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);

                newMyPattern.typeOfMyPattern = "ROTATION";
                KLdebug.Print("CREATO PATTERN Rotazionale circolare DI LUNGHEZZA = " + lengthOfCurrentPath, nameFile);
            
            }
            else
            {
                KLdebug.Print("lengthOfCurrentPath = " + lengthOfCurrentPath, nameFile);
                KLdebug.Print("IL PATTERN TRANS ha LUNGHEZZA NULLA, NON HO CREATO NIENTE.", nameFile);
            }
            KLdebug.Print(" ", nameFile);

            return newMyPattern;

        }
        */
        //The rotation matrix is referred to a counterclockwise rotation, so we must establish
        //which of the two possible direction of the outer pointing normals is the correct one
        public static double[] establishAxisDirection(MyVertex first, MyVertex second, MyVertex center, double[] planeNormal)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = string.Format("planenormal: ({0},{1},{2}) ", planeNormal[0], planeNormal[1], planeNormal[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            var tolerance = Math.Pow(10, -6);

            double[] vectorV1 = { first.x - center.x, first.y - center.y, first.z - center.z };
            double[] vectorV2 = { second.x - center.x, second.y - center.y, second.z - center.z };
            double[] crossProduct = FunctionsLC.CrossProduct(vectorV1, vectorV2);
            double[] crossProductNormalized = FunctionsLC.Normalize(crossProduct);
            //whatToWrite = string.Format("crossProduct normalized: ({0},{1},{2}) ", crossProductNormalized[0], crossProductNormalized[1], crossProductNormalized[2]);
            //KLdebug.Print(whatToWrite, nameFile);
            if (FunctionsLC.MyEqualsArray(crossProductNormalized, planeNormal))
            {
                //KLdebug.Print("OK: tengo il versore così e lo arrotondo", nameFile);
                for (var i = 0; i < 3; i++)
                {
                    if (Math.Abs(planeNormal[i]) < tolerance)
                    {
                        planeNormal.SetValue(0, i);
                    }
                }
                //whatToWrite = string.Format("Diventa: ({0},{1},{2}) ", planeNormal[0], planeNormal[1], planeNormal[2]);
                //KLdebug.Print(whatToWrite, nameFile);
                return planeNormal;
            }
            else
            {
                //KLdebug.Print("Devo invertire il versore.", nameFile);
                double[] oppositePlaneNormal = {-planeNormal[0], -planeNormal[1], -planeNormal[2]};
                return oppositePlaneNormal;
            }

        }

        public static List<MyVertex> translateListOfVertices(List<MyVertex> listOfVerticesToTranslate,
            double[] translationalVector)
        {
            var outputList = new List<MyVertex>();
            foreach (MyVertex vert in listOfVerticesToTranslate)
            {
                var translatedVert = new MyVertex(vert.x+translationalVector[0], vert.y+translationalVector[1], vert.z+translationalVector[2]);
                outputList.Add(translatedVert);
            }
            return outputList;
        }

        //It verifies if a symmetry ROTATIONAL relation between two MyRepeatedEntity exists
        //public static bool IsRotationTwoRE(MyRepeatedEntity firstMyRepeatedEntity,
        //    MyRepeatedEntity secondMyRepeatedEntity, MyVertex circumCenter, double teta, double[] axisDirection)
        public static bool IsRotationTwoRE(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double teta, double[] axisDirection, MyVertex circumCenter)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            var firstListOfVertices = firstMyRepeatedEntity.listOfVertices;
            var firstNumOfVerteces = firstListOfVertices.Count;
            var secondListOfVertices = secondMyRepeatedEntity.listOfVertices;
            var secondNumOfVerteces = secondListOfVertices.Count;

            #region STAMPA DEI VERTICI DELLE DUE RE

            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 1^ RE", nameFile);
            //foreach (var myVert in firstListOfVertices)
            //{
            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", firstListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 2^ RE", nameFile);
            //foreach (var myVert in secondListOfVertices)
            //{
            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", secondListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);           
            #endregion

            if (firstNumOfVerteces == secondNumOfVerteces)
            {
                // I use the rotation matrix relative to a rotation around an axis passing through the origin.
                // For this reason I need to translate the vertices coherently to the translated axis
                //I consider an arbitrary point ON the candidate rotationAxis and I translate it to the origin.
                //I choose the center of the circumference path.
                //Then, I translate all the vertices to compare by the same translational vector.

                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print("Numero di vertici prima RE: " + firstNumOfVerteces, nameFile);
                //KLdebug.Print("Numero di vertici seconda RE: " + secondNumOfVerteces, nameFile);
                //KLdebug.Print(
                //    "Il numero di vertici corrisponde. Li traslo tutti:",
                //    nameFile);
                //KLdebug.Print(" ", nameFile);

                double[] translationalVector = {-circumCenter.x, -circumCenter.y, -circumCenter.z};
                var firstListOfVerticesTransl = translateListOfVertices(firstListOfVertices, translationalVector);
                var secondListOfVerticesTransl = translateListOfVertices(secondListOfVertices, translationalVector);

                #region STAMPA DEI VERTICI DELLE DUE RE TRASLATI
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print("STAMPA DEI VERTICI DELLA 1^ RE TRASLATI ", nameFile);
                //foreach (var myVert in firstListOfVerticesTransl)
                //{
                //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", firstListOfVerticesTransl.IndexOf(myVert),
                //        myVert.x, myVert.y, myVert.z);
                //    KLdebug.Print(whatToWrite, nameFile);
                //}
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print("STAMPA DEI VERTICI DELLA 2^ RE TRASLATI", nameFile);
                //foreach (var myVert in secondListOfVerticesTransl)
                //{
                //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", secondListOfVerticesTransl.IndexOf(myVert),
                //        myVert.x, myVert.y, myVert.z);
                //    KLdebug.Print(whatToWrite, nameFile);
                //}
                //KLdebug.Print(" ", nameFile);
                #endregion

                int i = 0;
                while (i < firstNumOfVerteces)
                {
                    if (secondListOfVerticesTransl.FindIndex(
                        vert => vert.IsRotationOf(firstListOfVerticesTransl[i], teta, axisDirection)) != -1)
                    {
                        var found =
                            secondListOfVerticesTransl.Find(
                                vert => vert.IsRotationOf(firstListOfVerticesTransl[i], teta, axisDirection));
                        //whatToWrite = string.Format("Ho trovato che {0}-esimo vertice: ({1}, {2}, {3})", i,
                        //    firstListOfVerticesTransl[i].x,
                        //    firstListOfVerticesTransl[i].y, firstListOfVerticesTransl[i].z);
                        //KLdebug.Print(whatToWrite, nameFile);
                        //whatToWrite = string.Format("ha come suo ruotato ({0}, {1}, {2})", found.x, found.y, found.z);
                        //KLdebug.Print(whatToWrite, nameFile);
                        var scarto =
                            new MyVertex(Math.Abs(firstListOfVerticesTransl[i].Rotate(teta, axisDirection).x- found.x),
                                Math.Abs(firstListOfVerticesTransl[i].Rotate(teta, axisDirection).y - found.y),
                                Math.Abs(firstListOfVerticesTransl[i].Rotate(teta, axisDirection).z - found.z));
                        //whatToWrite = string.Format("scarto: ({0}, {1}, {2})", scarto.x, scarto.y, scarto.z);
                        //KLdebug.Print(whatToWrite, nameFile);
                        //KLdebug.Print(" ", nameFile);

                        i++;
                    }
                    else
                    {
                    //    KLdebug.Print("TROVATO VERTICE NON CORRISPONDENTE ALLA CERCATA ROTAZIONE!", nameFile);
                    //    KLdebug.Print(" ", nameFile);

                        return false;
                    }
                }

                //KLdebug.Print("   ANDATO A BUON FINE IL CHECK DEI VERTICI: PASSO AL CHECK DELLE FACCE.", nameFile);
                //KLdebug.Print(" ", nameFile);

                //Check of correct position of normals of all Planar face:
                if (!CheckOfPlanesForRotation(firstMyRepeatedEntity, secondMyRepeatedEntity, teta, axisDirection))
                {
                    return false;
                }
                //KLdebug.Print("   ANDATO A BUON FINE IL CHECK DELLE NORMALI: PASSO AL CHECK DELLE FACCE CILINDRICHE.", nameFile);

                //Check of correct position of Cylindrical faces:
                if (
                    !CheckOfCylindersForRotation(firstMyRepeatedEntity, secondMyRepeatedEntity, teta,
                    axisDirection, translationalVector))
                {
                    return false;
                }
                //KLdebug.Print("   ANDATO A BUON FINE IL CHECK DELLE FACCE CILINDRICHE.", nameFile);

                ////CONTINUARE CON GLI ALTRI TIPI DI SUPERFICI............
                //  KLdebug.Print("   ====>>> ROTAZIONE TRA QUESTE DUE re VERIFICATA!", nameFile);
                return true;
            }

            //KLdebug.Print(
            //    "NUMERO DI VERTICI NELLE DUE RE NON CORRISPONDENTE. IMPOSSIBILE EFFETTUARE IL CHECK DEI VERTICI!",
            //    nameFile);
            //KLdebug.Print(" ", nameFile);

            return false;
        }

        public static bool CheckOfPlanesForRotation(MyRepeatedEntity firstMyRepeatedEntity, 
            MyRepeatedEntity secondMyRepeatedEntity, double teta, double[] axisDirection)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("------>> CHECK DELLE NORMALI DELLE FACCE PLANARI:", nameFile);
            //var whatToWrite = "";

            var numOfPlanarSurfacesFirst = firstMyRepeatedEntity.listOfPlanarSurfaces.Count;
            var numOfPlanarSurfacesSecond = secondMyRepeatedEntity.listOfPlanarSurfaces.Count;

            if (numOfPlanarSurfacesFirst == numOfPlanarSurfacesSecond)
            {
                //KLdebug.Print("Numero di facce planari prima RE: " + numOfPlanarSurfacesFirst, nameFile);
                //KLdebug.Print("Numero di facce planari seconda RE: " + numOfPlanarSurfacesSecond, nameFile);
                //KLdebug.Print("Il numero di facce planari corrisponde, passo alla verifica delle normali:", nameFile);
                //KLdebug.Print(" ", nameFile);

                //If there are planar faces I check the correct position of the normals
                if (numOfPlanarSurfacesFirst > 0)
                {
                    var listOfFirstNormals = new List<MyVertex>();
                    var listOfSecondNormals = new List<MyVertex>();

                    //KLdebug.Print("Lista delle normali delle facce planari 1^ RE:", nameFile);
                    foreach (var planarSurface in firstMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        var normalToAdd = new MyVertex(planarSurface.a, planarSurface.b, planarSurface.c);
                        listOfFirstNormals.Add(normalToAdd);

                        //whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd.x, normalToAdd.y, normalToAdd.z);
                        //KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Lista delle normali delle facce planari 2^ RE:", nameFile);
                    foreach (var planarSurface in secondMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        var normalToAdd = new MyVertex(planarSurface.a, planarSurface.b, planarSurface.c);
                        listOfSecondNormals.Add(normalToAdd);

                        //whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd.x, normalToAdd.y, normalToAdd.z);
                        //KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Inizio della verifica della corrispondenza delle normali", nameFile);
                    int i = 0;
                    while (listOfSecondNormals.Count != 0 && i < numOfPlanarSurfacesFirst)  // DOVREBBERO ESSERE FALSE O VERE CONTEMPORANEAMENTE!!!
                    {
                        //KLdebug.Print(i + "-esima normale della 1^RE:", nameFile);

                        var rotatedNormal = listOfFirstNormals[i].Rotate(teta, axisDirection);

                        var indexOfFound =
                            listOfSecondNormals.FindIndex(normal => normal.Equals(rotatedNormal));
                        if (indexOfFound != -1)
                        {
                            //whatToWrite = string.Format("Analizzo la normale: ({0},{1},{2})", listOfFirstNormals[i].x, listOfFirstNormals[i].y, listOfFirstNormals[i].z);
                            //KLdebug.Print(whatToWrite, nameFile);
                            //KLdebug.Print(" -> Trovata corrispondenza con una faccia planare della 2^ RE: in posizione " + indexOfFound, nameFile);

                            listOfSecondNormals.RemoveAt(indexOfFound);
                            i++;
                        }
                        else
                        {
                            //KLdebug.Print(" -> Non è stata trovata corrispondenza con le normali della 2^ RE.", nameFile);
                            //KLdebug.Print("FINE", nameFile);
                            return false;
                        }
                       // KLdebug.Print(" ", nameFile);
                    }
                   // KLdebug.Print(" ", nameFile);

                    return true;
                }
                else
                {
                    //KLdebug.Print("NESSUNA FACCIA PLANARE.", nameFile);
                    //KLdebug.Print("FINE", nameFile);
                    return true;
                }
            }
            else
            {
               // KLdebug.Print("NUMERO DI FACCE PLANARI NON CORRISPONDENTE", nameFile);
               // KLdebug.Print("FINE", nameFile);
                return false;
            }


        }

        public static bool CheckOfCylindersForRotation(MyRepeatedEntity firstMyRepeatedEntity, MyRepeatedEntity secondMyRepeatedEntity,
            double teta, double[] axisDirection, double[] translationalVector)
        {

            const string nameFile = "GetRotationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("------>> CHECK DELLE FACCE CILINDRICHE:", nameFile);

            var numOfCylinderSurfacesFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces.Count;
            var numOfCylinderSurfacesSecond = secondMyRepeatedEntity.listOfCylindricalSurfaces.Count;

            if (numOfCylinderSurfacesFirst == numOfCylinderSurfacesSecond)
            {
                KLdebug.Print("Numero di facce CILINDRICHE prima RE: " + numOfCylinderSurfacesFirst, nameFile);
                KLdebug.Print("Numero di facce CILINDRICHE seconda RE: " + numOfCylinderSurfacesSecond, nameFile);
                KLdebug.Print("Il numero di facce cilidriche corrisponde, passo alla verifica geometrica:", nameFile);
                KLdebug.Print(" ", nameFile);

                if (numOfCylinderSurfacesFirst > 0)
                {
                    var listOfCylindersToLookIn = new List<MyCylinder>(secondMyRepeatedEntity.listOfCylindricalSurfaces);
                    int i = 0;
                    while (listOfCylindersToLookIn.Count != 0 && i < numOfCylinderSurfacesFirst)
                    {
                        var cylinderOfFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces[i];
                        KLdebug.Print("Sto analizzando lo " + i + "-esimo cilindro: ", nameFile);

                        if (!CylinderOfFirstAtPosition_i_IsOkRotation(firstMyRepeatedEntity, secondMyRepeatedEntity, 
                            teta, axisDirection, translationalVector, cylinderOfFirst, ref i)) 
                            return false;
                    }
                    return true;
                }
                return true;
            }
            return false;
        }

        public static bool CylinderOfFirstAtPosition_i_IsOkRotation(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, double teta, double[] axisDirection, 
            double[] translationalVector, MyCylinder cylinderOfFirst, ref int i)
        {
            const string nameFile = "GetRotationalPatterns.txt";
            var tolerance = Math.Pow(10, -5);
            
            //The axis of the second MyRepeatedEntity should have the same axis direction and
            //should pass through the rotated Origin of the cylinderOfFirst
            double[] originOfFirst = cylinderOfFirst.originCylinder;
            //var whatToWrite = string.Format("Centroid of first: ({0},{1},{2})", firstMyRepeatedEntity.centroid.x, firstMyRepeatedEntity.centroid.y, firstMyRepeatedEntity.centroid.z);
            //KLdebug.Print(whatToWrite, nameFile);
            //whatToWrite = string.Format("Centroid of first: ({0},{1},{2})", secondMyRepeatedEntity.centroid.x, secondMyRepeatedEntity.centroid.y, secondMyRepeatedEntity.centroid.z);
            //KLdebug.Print(whatToWrite, nameFile);

            double[] originOfFirstTranslated = { originOfFirst[0] + translationalVector[0], 
                                                   originOfFirst[1] + translationalVector[1], 
                                                   originOfFirst[2] + translationalVector[2] };
            var originOfFirstTranslatedMyVertex = new MyVertex(originOfFirstTranslated[0], originOfFirstTranslated[1], originOfFirstTranslated[2]);

            var pointOnAxisTranslatedMyVertex = originOfFirstTranslatedMyVertex.Rotate(teta, axisDirection);
            double[] pointOnAxis = { pointOnAxisTranslatedMyVertex.x - translationalVector[0],
                                       pointOnAxisTranslatedMyVertex.y - translationalVector[1], 
                                       pointOnAxisTranslatedMyVertex.z - translationalVector[2] };
            //whatToWrite = string.Format("Point on axis: ({0},{1},{2})", pointOnAxis[0], pointOnAxis[1], pointOnAxis[2]);
            //KLdebug.Print(whatToWrite, nameFile);

            var firstAxisDirectionMyVertex = new MyVertex(cylinderOfFirst.axisDirectionCylinder[0],
                cylinderOfFirst.axisDirectionCylinder[1], cylinderOfFirst.axisDirectionCylinder[2]);
            var rotatedFirstAxisDirectionMyVertex = firstAxisDirectionMyVertex.Rotate(teta, axisDirection);
            double[] rotatedFirstAxisDirection =
            {
                rotatedFirstAxisDirectionMyVertex.x,
                rotatedFirstAxisDirectionMyVertex.y, rotatedFirstAxisDirectionMyVertex.z
            };

            var axisToFind = FunctionsLC.ConvertPointPlusDirectionInMyLine(pointOnAxis,
                rotatedFirstAxisDirection);
            var indOfFound =
                secondMyRepeatedEntity.listOfCylindricalSurfaces.FindIndex(
                    cyl => cyl.axisCylinder.Equals(axisToFind));
            if (indOfFound != -1)
            {
                KLdebug.Print("----> Insieme dei cilindri con asse cercato della 2^ RE NON VUOTO: ", nameFile);

                var listOfPossibleCylinders =
                    secondMyRepeatedEntity.listOfCylindricalSurfaces.FindAll(
                        cyl => cyl.axisCylinder.Equals(axisToFind));
                var numOfPossibleCylinders = listOfPossibleCylinders.Count;
                KLdebug.Print("Ci sono " + numOfPossibleCylinders + "cilindri candidati. Verifico le altre caratteristiche:", nameFile);
                KLdebug.Print("", nameFile);

                //If I find cylinders with right axis line, I check the radius correspondence,
                //the center of bases correspondence.
                //For elliptical bases I make a further control on radius and direction axis.
                var notFound = true;
                int j = 0;
                while (notFound && j < numOfPossibleCylinders)
                {
                    var possibleCylinder = listOfPossibleCylinders[j];
                    KLdebug.Print(j + "-esimo cilindro candidato:", nameFile);
                    if (Math.Abs(cylinderOfFirst.radiusCylinder - possibleCylinder.radiusCylinder) < tolerance)
                    {
                        KLdebug.Print("----raggio del cilindro OK", nameFile);

                        if (CheckOfClosedEdgesCorrespondenceRot(cylinderOfFirst, possibleCylinder,
                            teta, axisDirection, translationalVector))
                        {
                            KLdebug.Print("TROVATO CILINDRO GIUSTO. FINE", nameFile);
                            notFound = false;
                        }
                    }
                    j++;
                }
                if (notFound)
                {
                    KLdebug.Print("", nameFile);
                    KLdebug.Print("NESSUN CILINDRO CORRISPONDENTE. FINE", nameFile);
                    return false;
                }

                //if the previous cylinder is ok, i pass to the next cylinder:
                i++;
            }
            else
            {
                KLdebug.Print("----> Insieme dei cilindri con asse corrispondente della 2^ RE VUOTO. FINE", nameFile);
                return false;
            }
            return true;
        }

        //Given 2 cylinder with corresponding radius and axis (MyLine) corresponding to the expected position for rotation, 
        //given the rotation angle and the axis direction of rotation, it returns TRUE if the two base edges corresponds 
        //to the ones of the second cylinder, it return FALSE otherwise.
        //For circles: a comparison of the centers is sufficient
        //For ellipses: a comparison of the centers, of the major and minor radius and of the axis (MyLine) is necessary
        public static bool CheckOfClosedEdgesCorrespondenceRot(MyCylinder cylinderOfFirst, MyCylinder possibleCylinder, 
            double teta, double[] axisDirection, double[] translationalVector)
        {
            const string nameFile = "GetRotationalPatterns.txt";

            var listOfCircleFirst = cylinderOfFirst.listOfBaseCircle;
            var listOfCircleSecond = new List<MyCircle>(possibleCylinder.listOfBaseCircle);
            var listOfEllipseFirst = cylinderOfFirst.listOfBaseEllipse;
            var listOfEllipseSecond = new List<MyEllipse>(possibleCylinder.listOfBaseEllipse);

            int numOfCircleFirst = listOfCircleFirst.Count;
            int numOfEllipseFirst = listOfEllipseFirst.Count;
            int numOfCircleSecond = listOfCircleSecond.Count;
            int numOfEllipseSecond = listOfEllipseSecond.Count;

            KLdebug.Print("Numero circle chiuse della 1^ RE: " + numOfCircleFirst, nameFile);
            KLdebug.Print("Numero circle chiuse della 2^ RE: " + numOfCircleSecond, nameFile);
            KLdebug.Print("Numero ellipse chiuse della 1^ RE: " + numOfEllipseFirst, nameFile);
            KLdebug.Print("Numero ellipse chiuse della 2^ RE: " + numOfEllipseSecond, nameFile);

            if (numOfCircleFirst == numOfCircleSecond && numOfEllipseFirst == numOfEllipseSecond)
            {
                KLdebug.Print("Il numero di CIRCLE e ELLIPSE corrisponde.", nameFile);
                KLdebug.Print(" ", nameFile);

                if (numOfCircleFirst + numOfEllipseFirst == 0)  //cylinders with baseFace not planar
                {
                    KLdebug.Print("Non ci sono circle o ellipse di base. FINE DELLA VERIFICA DELLA CORRISPONDENZA DEGLI EDGE DI BASE DEL CILINDRO.", nameFile);
                    return true;
                }
                if (!CorrespondenceOfCirclesInCylinderRot(teta, axisDirection, translationalVector, listOfCircleFirst, listOfCircleSecond)) return false;

                if (!CorrespondenceOfEllipsesInCylinderRot(teta, axisDirection, translationalVector, listOfEllipseFirst, listOfEllipseSecond)) return false;

                return true;
            }
            return false;           
        }

        public static bool CorrespondenceOfCirclesInCylinderRot(double teta, double[] axisDirection, double[] translationalVector,
            List<MyCircle> listOfCircleFirst, List<MyCircle> listOfCircleSecond)
        {
            const string nameFile = "GetRotationalPatterns.txt";

            var numOfCircleFirst = listOfCircleFirst.Count;
            var numOfCircleSecond = listOfCircleSecond.Count;

            // OBSERVATION: int i, j are such that 0<=i,j<=2
            if (numOfCircleFirst > 0)
            {
                int i = 0;
                // for every circle of first cylinder
                while (i < numOfCircleFirst)
                {
                    KLdebug.Print(i + "-esimo Circle del 1° cilindro:", nameFile);

                    var firstCenter = listOfCircleFirst[i].centerCircle;
                    var firstCenterTranslated = new MyVertex(firstCenter.x+translationalVector[0], 
                        firstCenter.y+translationalVector[1], firstCenter.z+translationalVector[2]);
                    var whatToWrite = string.Format("Centro della prima crf ({0},{1},{2})", firstCenter.x, firstCenter.y, firstCenter.z);
                    KLdebug.Print(whatToWrite, nameFile);

                    int j = 0;
                    var thisCircleIsOk = false;

                    while (thisCircleIsOk == false && j < numOfCircleSecond)
                    {
                        KLdebug.Print(j + "-esimo Circle del 2° cilindro:", nameFile);

                        var secondCenter = listOfCircleSecond[j].centerCircle;
                        var secondCenterTranslated = new MyVertex(secondCenter.x + translationalVector[0],
                        secondCenter.y + translationalVector[1], secondCenter.z + translationalVector[2]);

                        whatToWrite = string.Format("Centro della seconda crf ({0},{1},{2})", secondCenter.x, secondCenter.y, secondCenter.z);
                        KLdebug.Print(whatToWrite, nameFile);

                        thisCircleIsOk = secondCenterTranslated.IsRotationOf(firstCenterTranslated, teta, axisDirection);
                        KLdebug.Print("Corrispondono i centri? " + thisCircleIsOk, nameFile);
                        KLdebug.Print(" ", nameFile);

                        if (thisCircleIsOk)
                        {
                            listOfCircleSecond.RemoveAt(j);
                        }
                        j++;
                    }
                    if (!thisCircleIsOk)
                    {
                        KLdebug.Print("Non ho trovato nessun Circle che vada bene nel 2° cilindro. FINE.", nameFile);
                        return false;
                    }
                    numOfCircleSecond = listOfCircleSecond.Count;
                    i++;
                }
            }
            else
            {
                KLdebug.Print("Non ci sono Circle in questo cilindro della 1^ RE. Passo agli ellissi.", nameFile);
            }
            KLdebug.Print("VERIFICA DEI CIRCLE é OK.", nameFile);
            KLdebug.Print(" ", nameFile);

            return true;
        }

        public static bool CorrespondenceOfEllipsesInCylinderRot(double teta, double[] axisDirection, double[] translationalVector,
            List<MyEllipse> listOfEllipseFirst, List<MyEllipse> listOfEllipseSecond)
        {
            const string nameFile = "GetRotationalPatterns.txt";

            var numOfEllipseFirst = listOfEllipseFirst.Count;
            var numOfEllipseSecond = listOfEllipseSecond.Count;
            var tolerance = Math.Pow(10, -5);

            // OBSERVATION: int i, j are such that 0<=i,j<=2
            if (numOfEllipseFirst > 0)
            {
                int i = 0;
                // for every ellipse of first cylinder
                while (i < numOfEllipseFirst)
                {
                    var firstEllipse = listOfEllipseFirst[i];
                    var firstCenter = firstEllipse.centerEllipse;
                    var firstCenterTranslated = new MyVertex(firstCenter.x + translationalVector[0],
                        firstCenter.y + translationalVector[1], firstCenter.z + translationalVector[2]);
                    KLdebug.Print(i + "-esimo Ellipse del 1° cilindro:", nameFile);
                    //var whatToWrite = string.Format("Centro della prima ellisse ({0},{1},{2})", firstEllipse.centerEllipse.x, firstEllipse.centerEllipse.y, firstEllipse.centerEllipse.z);
                    //KLdebug.Print(whatToWrite, nameFile);

                    int j = 0;
                    var thisEllipseIsOk = false;

                    while (thisEllipseIsOk == false && j < numOfEllipseSecond)
                    {
                        var secondEllipse = listOfEllipseSecond[j];
                        KLdebug.Print(j + "-esimo Ellipse del 2° cilindro:", nameFile);
                        //whatToWrite = string.Format("Centro della seconda ellisse ({0},{1},{2})", secondEllipse.centerEllipse.x, secondEllipse.centerEllipse.y, secondEllipse.centerEllipse.z);
                        //KLdebug.Print(whatToWrite, nameFile);

                        //comparison of the ellipse centers:
                        var secondCenter = secondEllipse.centerEllipse;
                        var secondCenterTranslated = new MyVertex(secondCenter.x + translationalVector[0],
                            secondCenter.y + translationalVector[1], secondCenter.z + translationalVector[2]);
                        thisEllipseIsOk = secondCenterTranslated.IsRotationOf(firstCenterTranslated,
                            teta, axisDirection);
                        if (thisEllipseIsOk)
                        {
                            KLdebug.Print("Corrispondono i centri? " + thisEllipseIsOk, nameFile);
                            //if the radius correspond:
                            if (Math.Abs(firstEllipse.majorRadEllipse - secondEllipse.majorRadEllipse) < tolerance &&
                                Math.Abs(firstEllipse.minorRadEllipse - secondEllipse.minorRadEllipse) < tolerance)
                            {
                                KLdebug.Print("Corrispondono i major e minor radius? True", nameFile);

                                //check if the directions corresponding to the axis are ok:

                                var firstMajorAxisDirectionEllipseMyVertex = new MyVertex(firstEllipse.majorAxisDirectionEllipse[0],
                                    firstEllipse.majorAxisDirectionEllipse[1], firstEllipse.majorAxisDirectionEllipse[2]);

                                var firstMinorAxisDirectionEllipseMyVertex = new MyVertex(firstEllipse.minorAxisDirectionEllipse[0],
                                    firstEllipse.minorAxisDirectionEllipse[1], firstEllipse.minorAxisDirectionEllipse[2]);

                                var firstMajorAxisDirectionEllipseOppositeMyVertex = new MyVertex(-firstEllipse.majorAxisDirectionEllipse[0],
                                    -firstEllipse.majorAxisDirectionEllipse[1], -firstEllipse.majorAxisDirectionEllipse[2]);
                             
                                var firstMinorAxisDirectionEllipseOppositeMyVertex = new MyVertex(-firstEllipse.minorAxisDirectionEllipse[0], 
                                    -firstEllipse.minorAxisDirectionEllipse[1], -firstEllipse.minorAxisDirectionEllipse[2]);

                                var secondMajorAxisDirectionEllipseMyVertex = new MyVertex(secondEllipse.majorAxisDirectionEllipse[0],
                                    secondEllipse.majorAxisDirectionEllipse[1], secondEllipse.majorAxisDirectionEllipse[2]);

                                var secondMinorAxisDirectionEllipseMyVertex = new MyVertex(secondEllipse.minorAxisDirectionEllipse[0],
                                    secondEllipse.minorAxisDirectionEllipse[1], secondEllipse.minorAxisDirectionEllipse[2]);
                             
                                if (
                                    secondMajorAxisDirectionEllipseMyVertex.IsRotationOf(
                                        firstMajorAxisDirectionEllipseMyVertex, teta, axisDirection) ||
                                    secondMajorAxisDirectionEllipseMyVertex.IsRotationOf(
                                        firstMajorAxisDirectionEllipseOppositeMyVertex, teta, axisDirection))                           
                                {
                                    if (
                                        secondMinorAxisDirectionEllipseMyVertex.IsRotationOf(
                                        firstMinorAxisDirectionEllipseMyVertex, teta, axisDirection) ||
                                    secondMinorAxisDirectionEllipseMyVertex.IsRotationOf(
                                        firstMinorAxisDirectionEllipseOppositeMyVertex, teta, axisDirection))
                                    {
                                        KLdebug.Print("Corrispondono i major minor axis direction? True", nameFile);
                                        listOfEllipseSecond.RemoveAt(j);
                                    }
                                    else
                                    {
                                        KLdebug.Print("Corrispondono i major minor axis direction? False", nameFile);
                                        thisEllipseIsOk = false;
                                    }

                                }
                                else
                                {
                                    KLdebug.Print("Corrispondono i major minor axis direction? False", nameFile);
                                    thisEllipseIsOk = false;
                                }
                            }
                            else
                            {
                                KLdebug.Print("Corrispondono i major minor radius? False", nameFile);
                                thisEllipseIsOk = false;
                            }
                        }
                        j++;
                    }
                    if (!thisEllipseIsOk)
                    {
                        KLdebug.Print("Non ho trovato nessun Ellipse che vada bene nel 2° cilindro. Fine.", nameFile);
                        return false;
                    }
                    else
                    {
                        numOfEllipseSecond = listOfEllipseSecond.Count;
                        i++;
                    }
                }
            }
            else
            {
                KLdebug.Print("Non ci sono Ellipse in questo cilindro della 1^ RE.", nameFile);
            }
            KLdebug.Print("VERIFICA DEGLI ELLIPSE é OK.", nameFile);
            KLdebug.Print(" ", nameFile);
            return true;
        }      

    }
}