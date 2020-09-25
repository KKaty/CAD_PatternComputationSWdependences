using System;
using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {

        //It verifies if a symmetry REFLECTIONAL relation between two MyRepeatedEntity exists
        public static bool IsReflectionTwoRE(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, SldWorks swapplication)
        {
            //const string nameFile = "GetReflectionalPattern.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            var candidateReflMyPlane = GetCandidateReflectionalMyPlane(firstMyRepeatedEntity.centroid,
                secondMyRepeatedEntity.centroid, swapplication);

            //Vertex analysis for Reflection:
            var firstListOfVertices = firstMyRepeatedEntity.listOfVertices;
            var firstNumOfVerteces = firstListOfVertices.Count;
            var secondListOfVertices = secondMyRepeatedEntity.listOfVertices;
            var secondNumOfVerteces = secondListOfVertices.Count;

            #region STAMPA DEI VERTICI DELLE DUE RE
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 1^ RE", nameFile);
            //foreach (var myVert in firstListOfVertices)
            //{
            //    var reflectionVertex = myVert.Reflect(candidateReflMyPlane);
            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", firstListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);

            //    whatToWrite = string.Format("{0}-esimo riflessione: ({1}, {2}, {3})", firstListOfVertices.IndexOf(myVert),
            //        reflectionVertex.x, reflectionVertex.y, reflectionVertex.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("STAMPA DEI VERTICI DELLA 2^ RE", nameFile);
            //foreach (var myVert in secondListOfVertices)
            //{
            //    var reflectionVertex = myVert.Reflect(candidateReflMyPlane);

            //    whatToWrite = string.Format("{0}-esimo vertice: ({1}, {2}, {3})", secondListOfVertices.IndexOf(myVert),
            //        myVert.x, myVert.y, myVert.z);
            //    KLdebug.Print(whatToWrite, nameFile);

            //    whatToWrite = string.Format("{0}-esimo riflessione: ({1}, {2}, {3})", firstListOfVertices.IndexOf(myVert),
            //        reflectionVertex.x, reflectionVertex.y, reflectionVertex.z);
            //    KLdebug.Print(whatToWrite, nameFile);
            //}
            //KLdebug.Print(" ", nameFile);
            #endregion

            if (firstNumOfVerteces == secondNumOfVerteces)
            {
                //KLdebug.Print(" ", nameFile);
                //KLdebug.Print("Numero di vertici prima RE: " + firstNumOfVerteces, nameFile);
                //KLdebug.Print("Numero di vertici seconda RE: " + secondNumOfVerteces, nameFile);
                //KLdebug.Print("Il numero di vertici corrisponde. Passo alla verifica della corrispondenza per riflessione:", nameFile);
                //KLdebug.Print(" ", nameFile);

                int i = 0;
                while (i < firstNumOfVerteces)
                {
                    var found =
                            secondListOfVertices.Find(
                                vert => vert.IsReflectionOf(firstListOfVertices[i], candidateReflMyPlane));
                       
                    if (found != null)
                    {
                        //whatToWrite = string.Format("Ho trovato che {0}-esimo vertice: ({1}, {2}, {3})", i, firstListOfVertices[i].x,
                        //   firstListOfVertices[i].y, firstListOfVertices[i].z);
                        //KLdebug.Print(whatToWrite, nameFile);
                        //whatToWrite = string.Format("ha come suo riflesso ({0}, {1}, {2})", found.x, found.y, found.z);
                        //var scarto =
                            new MyVertex(Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).x - found.x),
                                Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).y - found.y),
                                Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).z - found.z));
                        //whatToWrite = string.Format("scarto: ({0}, {1}, {2})", scarto.x, scarto.y, scarto.z);
                        //KLdebug.Print(whatToWrite, nameFile);
                        //KLdebug.Print(" ", nameFile);

                        i++;
                    }
                    else
                    {
                        //KLdebug.Print("TROVATO VERTICE NON CORRISPONDENTE ALLA CERCATA RIFLESSIONE!", nameFile);
                        //KLdebug.Print(" ", nameFile);

                        return false;
                    }
                }
            }
            else
            {
                //KLdebug.Print("NUMERO DI VERTICI NELLE DUE RE NON CORRISPONDENTE. IMPOSSIBILE EFFETTUARE IL CHECK DEI VERTICI!", nameFile);
                //KLdebug.Print(" ", nameFile);

                return false;
            }

            //KLdebug.Print("   ANDATO A BUON FINE IL CHECK DEI VERTICI: PASSO AL CHECK DELLE FACCE.", nameFile);
            //KLdebug.Print(" ", nameFile);

            //Check of correct position of normals of all Planar face:
            if (!CheckOfPlanesForReflection(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateReflMyPlane))
            {
                return false;
            }

            ////Check of correct position of cylinder faces:
            //if (!CheckOfCylindersForReflection(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateReflMyPlane))
            //{
            //    return false;
            //}

            ////CONTINUARE CON GLI ALTRI TIPI DI SUPERFICI............
            //KLdebug.Print("   ====>>> TRASLAZIONE TRA QUESTE DUE re VERIFICATA!", nameFile);
            return true;
        }

        public static MyPlane GetCandidateReflectionalMyPlane(MyVertex firstReferencePoint,
            MyVertex secondReferencePoint, SldWorks swapplication)
        {
            //const string nameFile = "GetReflectionalPattern.txt";
            //KLdebug.Print(" ", nameFile);
            //var whatToWrite = "";

            double[] candidateReflectionalNormal =
            {
                secondReferencePoint.x - firstReferencePoint.x,
                secondReferencePoint.y - firstReferencePoint.y,
                secondReferencePoint.z - firstReferencePoint.z,
            };
            //whatToWrite = string.Format("Candidate reflectional normal: ({0}, {1}, {2})", candidateReflectionalNormal[0],
            //    candidateReflectionalNormal[1], candidateReflectionalNormal[2]);
            //KLdebug.Print(whatToWrite, nameFile);

            double[] candidateReflectionalAppPoint =
            {
                (firstReferencePoint.x + secondReferencePoint.x)/2,
                (firstReferencePoint.y + secondReferencePoint.y)/2,
                (firstReferencePoint.z + secondReferencePoint.z)/2
            };
            //whatToWrite = string.Format("Candidate reflectional App Point: ({0}, {1}, {2})", candidateReflectionalAppPoint[0],
            //    candidateReflectionalAppPoint[1], candidateReflectionalAppPoint[2]);
            //KLdebug.Print(whatToWrite, nameFile);

            //if (swapplication != null)
            //{
            //    var SwModel = swapplication.ActiveDoc;
            //    SwModel.ClearSelection2(true);
            //    SwModel.Insert3DSketch();
            //    SwModel.CreatePoint2(candidateReflectionalAppPoint[0], candidateReflectionalAppPoint[1],
            //        candidateReflectionalAppPoint[2]);
            //    SwModel.InsertSketch();
            //}

            var candidateReflMyPlane = new MyPlane(candidateReflectionalNormal, candidateReflectionalAppPoint);
            //whatToWrite = string.Format("Candidate reflectional MyPlane: {0}x +{1}y +{2}z + {3} = 0", candidateReflMyPlane.a,
            //    candidateReflMyPlane.b, candidateReflMyPlane.c, candidateReflMyPlane.d);
            //KLdebug.Print(whatToWrite, nameFile);

            //KLdebug.Print(" ", nameFile);
            return candidateReflMyPlane;
        }

        public static double[] ReflectNormal(double[] normal, MyPlane candidateReflMyPlane)
        {
            double[] planeNormal = { candidateReflMyPlane.a, candidateReflMyPlane.b, candidateReflMyPlane.c };
            double[] origin = { 0, 0, 0 };
            var planeForReflectionOfNormals = new MyPlane(planeNormal, origin);
            var normalMyVertex = new MyVertex(normal[0], normal[1], normal[2]);
            var reflectedNormalMyVertex = normalMyVertex.Reflect(planeForReflectionOfNormals);
            double[] reflectedNormal = { reflectedNormalMyVertex.x, reflectedNormalMyVertex.y, reflectedNormalMyVertex.z };
            return reflectedNormal;
        }

        public static bool CheckOfPlanesForReflection(MyRepeatedEntity firstMyRepeatedEntity, MyRepeatedEntity secondMyRepeatedEntity, MyPlane candidateReflMyPlane)
        {
            //const string nameFile = "GetReflectionalPattern.txt";
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
                    var listOfFirstNormals = new List<double[]>();
                    var listOfSecondNormals = new List<double[]>();

                    //KLdebug.Print("Lista delle normali delle facce planari 1^ RE:", nameFile);
                    foreach (var planarSurface in firstMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        double[] normalToAdd = { planarSurface.a, planarSurface.b, planarSurface.c };
                        listOfFirstNormals.Add(normalToAdd);

                        //whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd[0], normalToAdd[1], normalToAdd[2]);
                        //KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Lista delle normali delle facce planari 2^ RE:", nameFile);
                    foreach (var planarSurface in secondMyRepeatedEntity.listOfPlanarSurfaces)
                    {
                        double[] normalToAdd = { planarSurface.a, planarSurface.b, planarSurface.c };
                        listOfSecondNormals.Add(normalToAdd);

                        //whatToWrite = string.Format("-aggiunta ({0},{1},{2})", normalToAdd[0], normalToAdd[1], normalToAdd[2]);
                        //KLdebug.Print(whatToWrite, nameFile);
                    }

                    //KLdebug.Print(" ", nameFile);
                    //KLdebug.Print("Inizio della verifica della corrispondenza delle normali", nameFile);
                    int i = 0;
                    while (listOfSecondNormals.Count != 0 && i < numOfPlanarSurfacesFirst)  // DOVREBBERO ESSERE FALSE O VERE CONTEMPORANEAMENTE!!!
                    {
                        //KLdebug.Print(i + "-esima normale della 1^RE:", nameFile);

                        var reflectedNormal = ReflectNormal(listOfFirstNormals[i], candidateReflMyPlane);
                        
                        var indexOfFound =
                            listOfSecondNormals.FindIndex(normal => FunctionsLC.MyEqualsArray(normal, reflectedNormal));
                        if (indexOfFound != -1)
                        {
                            //whatToWrite = string.Format("Analizzo la normale: ({0},{1},{2})", listOfFirstNormals[i][0], listOfFirstNormals[i][1], listOfFirstNormals[i][2]);
                            //KLdebug.Print(whatToWrite, nameFile);
                            //KLdebug.Print(" -> Trovata corrispondenza con una faccia planare della 2^ RE;", nameFile);

                            listOfSecondNormals.RemoveAt(indexOfFound);
                            i++;
                        }
                        else
                        {
                        //    KLdebug.Print(" -> Non è stata trovata corrispondenza con le normali della 2^ RE.", nameFile);
                        //    KLdebug.Print("FINE", nameFile);
                            return false;
                        }
                       // KLdebug.Print(" ", nameFile);
                    }
                    //KLdebug.Print(" ", nameFile);

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
                //KLdebug.Print("NUMERO DI FACCE PLANARI NON CORRISPONDENTE", nameFile);
                //KLdebug.Print("FINE", nameFile);
                return false;
            }


        }


        public static bool CheckOfCylindersForReflection(MyRepeatedEntity firstMyRepeatedEntity, MyRepeatedEntity secondMyRepeatedEntity, MyPlane candidateReflMyPlane)
        {

            var numOfCylinderSurfacesFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces.Count;
            var numOfCylinderSurfacesSecond = secondMyRepeatedEntity.listOfCylindricalSurfaces.Count;

            if (numOfCylinderSurfacesFirst == numOfCylinderSurfacesSecond)
            {
            
                if (numOfCylinderSurfacesFirst > 0)
                {
                    var listOfCylindersToLookIn = new List<MyCylinder>(secondMyRepeatedEntity.listOfCylindricalSurfaces);
                    int i = 0;
                    while (listOfCylindersToLookIn.Count != 0 && i < numOfCylinderSurfacesFirst)
                    {
                        var cylinderOfFirst = firstMyRepeatedEntity.listOfCylindricalSurfaces[i];

                        if (!CylinderOfFirstAtPosition_i_IsOkReflection(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateReflMyPlane, cylinderOfFirst, ref i)) return false;
                    }
                    return true;
                }
                return true;
            }
            return false;
        }

        public static bool CylinderOfFirstAtPosition_i_IsOkReflection(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, MyPlane candidateReflMyPlane, MyCylinder cylinderOfFirst, ref int i)
        {
            const string nameFile = "GetReflectionalPattern.txt";
            var tolerance = Math.Pow(10, -5);
            //I compute the distance vector v between the centroidOfFirst and the Origin of the cylinderOfFirst (which pass through the axis)
            //The axis of the second MyRepeatedEntity should have the same axis direction and
            //should pass through the point = centroidOfSecond + v
            double[] originOfFirst = cylinderOfFirst.originCylinder;
            var originOfFirstMyVertex = new MyVertex(originOfFirst[0], originOfFirst[1], originOfFirst[2]);

            var pointOnAxisMyVertex = originOfFirstMyVertex.Reflect(candidateReflMyPlane);
            double[] pointOnAxis = { pointOnAxisMyVertex.x, pointOnAxisMyVertex.y, pointOnAxisMyVertex.z };

            var reflectedDirectionOfAxis = ReflectNormal(cylinderOfFirst.axisDirectionCylinder, candidateReflMyPlane);

            var axisToFind = FunctionsLC.ConvertPointPlusDirectionInMyLine(pointOnAxis,
                reflectedDirectionOfAxis);
            var indOfFound =
                secondMyRepeatedEntity.listOfCylindricalSurfaces.FindIndex(
                    cyl => cyl.axisCylinder.Equals(axisToFind));
            if (indOfFound != -1)
            {
                var listOfPossibleCylinders =
                    secondMyRepeatedEntity.listOfCylindricalSurfaces.FindAll(
                        cyl => cyl.axisCylinder.Equals(axisToFind));
                var numOfPossibleCylinders = listOfPossibleCylinders.Count;

                //If I find cylinders with right axis line, I check the radius correspondence,
                //the center of bases correspondence.
                //For elliptical bases I make a further control on radius and direction axis.
                var notFound = true;
                int j = 0;
                while (notFound && j < numOfPossibleCylinders)
                {
                    var possibleCylinder = listOfPossibleCylinders[j];
                    if (Math.Abs(cylinderOfFirst.radiusCylinder - possibleCylinder.radiusCylinder) < tolerance)
                    {
                        if (CheckOfClosedEdgesCorrespondenceRefl(cylinderOfFirst, possibleCylinder,
                            candidateReflMyPlane))
                        {
                            notFound = false;
                        }
                    }
                    j++;
                }
                if (notFound)
                {
                    return false;
                }

                //if the previous cylinder is ok, i pass to the next cylinder:
                i++;
            }
            else
            {
                return false;
            }
            return true;
        }

        //Given 2 cylinder with corresponding radius and axis (MyLine) corresponding to the expected position for translation, 
        //given the translation vector, it returns TRUE if the two base edges corresponds to the ones of the second cylinder,
        //it return FALSE otherwise.
        //For circles: a comparison of the centers is sufficient
        //For ellipses: a comparison of the centers, of the major and minor radius and of the axis (MyLine) is necessary
        public static bool CheckOfClosedEdgesCorrespondenceRefl(MyCylinder cylinderOfFirst, MyCylinder possibleCylinder, MyPlane candidateReflMyPlane)
        {
            const string nameFile = "GetReflectionalPattern.txt";

            var listOfCircleFirst = cylinderOfFirst.listOfBaseCircle;
            var listOfCircleSecond = new List<MyCircle>(possibleCylinder.listOfBaseCircle);
            var listOfEllipseFirst = cylinderOfFirst.listOfBaseEllipse;
            var listOfEllipseSecond = new List<MyEllipse>(possibleCylinder.listOfBaseEllipse);

            int numOfCircleFirst = listOfCircleFirst.Count;
            int numOfEllipseFirst = listOfEllipseFirst.Count;
            int numOfCircleSecond = listOfCircleSecond.Count;
            int numOfEllipseSecond = listOfEllipseSecond.Count;

            if (numOfCircleFirst == numOfCircleSecond && numOfEllipseFirst == numOfEllipseSecond)
            {
                if (numOfCircleFirst + numOfEllipseFirst == 0)  //cylinders with baseFace not planar
                {
                    return true;
                }
                if (!CorrespondenceOfCirclesInCylinderRefl(candidateReflMyPlane, listOfCircleFirst, listOfCircleSecond)) return false;

                if (!CorrespondenceOfEllipsesInCylinderRefl(candidateReflMyPlane, listOfEllipseFirst, listOfEllipseSecond)) return false;

                return true;
            }
            return false;

        }

        public static bool CorrespondenceOfCirclesInCylinderRefl(MyPlane candidateReflMyPlane,
            List<MyCircle> listOfCircleFirst, List<MyCircle> listOfCircleSecond)
        {
            const string nameFile = "GetReflectionalPattern.txt";

            var numOfCircleFirst = listOfCircleFirst.Count;
            var numOfCircleSecond = listOfCircleSecond.Count;

            // OBSERVATION: int i, j are such that 0<=i,j<=2
            if (numOfCircleFirst > 0)
            {
                int i = 0;
                // for every circle of first cylinder
                while (i < numOfCircleFirst)
                {

                    var firstCenter = listOfCircleFirst[i].centerCircle;
                   
                    int j = 0;
                    var thisCircleIsOk = false;

                    while (thisCircleIsOk == false && j < numOfCircleSecond)
                    {
                        var secondCenter = listOfCircleSecond[j].centerCircle;

                        
                        thisCircleIsOk = secondCenter.IsReflectionOf(firstCenter, candidateReflMyPlane);
                        
                        if (thisCircleIsOk)
                        {
                            listOfCircleSecond.RemoveAt(j);
                        }
                        j++;
                    }
                    if (!thisCircleIsOk)
                    {
                        return false;
                    }
                    else
                    {
                        numOfCircleSecond = listOfCircleSecond.Count;
                        i++;
                    }
                }
            }
       
            return true;
        }

        public static bool CorrespondenceOfEllipsesInCylinderRefl(MyPlane candidateReflMyPlane,
            List<MyEllipse> listOfEllipseFirst, List<MyEllipse> listOfEllipseSecond)
        {
            const string nameFile = "GetReflectionalPattern.txt";

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
                    int j = 0;
                    var thisEllipseIsOk = false;

                    while (thisEllipseIsOk == false && j < numOfEllipseSecond)
                    {
                        var secondEllipse = listOfEllipseSecond[j];
                       
                        //comparison of the ellipse centers:
                        thisEllipseIsOk = secondEllipse.centerEllipse.IsReflectionOf(firstEllipse.centerEllipse,
                            candidateReflMyPlane);
                        if (thisEllipseIsOk)
                        {
                            //if the radius correspond:
                            if (Math.Abs(firstEllipse.majorRadEllipse - secondEllipse.majorRadEllipse) < tolerance &&
                                Math.Abs(firstEllipse.minorRadEllipse - secondEllipse.minorRadEllipse) < tolerance)
                            {
                                //check if the directions corresponding to the axis are ok:

                                double[] firstMajorAxisDirectionEllipseOpposite =
                                {
                                    -firstEllipse.majorAxisDirectionEllipse[0], 
                                    -firstEllipse.majorAxisDirectionEllipse[1],
                                    -firstEllipse.majorAxisDirectionEllipse[2]
                                };
                                double[] firstMinorAxisDirectionEllipseOpposite =
                                {
                                    -firstEllipse.minorAxisDirectionEllipse[0], 
                                    -firstEllipse.minorAxisDirectionEllipse[1],
                                    -firstEllipse.minorAxisDirectionEllipse[2]
                                };
                                if (FunctionsLC.MyEqualsArray(ReflectNormal(firstEllipse.majorAxisDirectionEllipse, candidateReflMyPlane), secondEllipse.majorAxisDirectionEllipse) ||
                                    FunctionsLC.MyEqualsArray(ReflectNormal(firstMajorAxisDirectionEllipseOpposite, candidateReflMyPlane), secondEllipse.majorAxisDirectionEllipse))
                                {
                                    if (
                                        FunctionsLC.MyEqualsArray(ReflectNormal(firstEllipse.minorAxisDirectionEllipse, candidateReflMyPlane), secondEllipse.minorAxisDirectionEllipse) ||
                                    FunctionsLC.MyEqualsArray(ReflectNormal(firstMinorAxisDirectionEllipseOpposite, candidateReflMyPlane), secondEllipse.minorAxisDirectionEllipse))
                                    {
                                        listOfEllipseSecond.RemoveAt(j);
                                    }
                                    else
                                    {
                                        thisEllipseIsOk = false;
                                    }

                                }
                                else
                                {
                                    thisEllipseIsOk = false;
                                }
                            }
                            else
                            {
                                thisEllipseIsOk = false;
                            }
                        }
                        j++;
                    }
                    if (!thisEllipseIsOk)
                    {
                        return false;
                    }
                    else
                    {
                        numOfEllipseSecond = listOfEllipseSecond.Count;
                        i++;
                    }
                }
            }
          
            return true;
        }

        //(THE FOLLOWING FUNCTION IS CREATED FOR COMPOSED PATTERN SEARCH)
        //This function is the analogous of IsReflectionTwoRE, but in this situation the candidate
        //reflectional plane is given as input.        
        public static bool IsReflectionTwoREGivenCandidateReflPlane(MyRepeatedEntity firstMyRepeatedEntity,
            MyRepeatedEntity secondMyRepeatedEntity, MyPlane candidateReflMyPlane)
        {

            //Vertex analysis for Reflection:
            var firstListOfVertices = firstMyRepeatedEntity.listOfVertices;
            var firstNumOfVerteces = firstListOfVertices.Count;
            var secondListOfVertices = secondMyRepeatedEntity.listOfVertices;
            var secondNumOfVerteces = secondListOfVertices.Count;

            if (firstNumOfVerteces == secondNumOfVerteces)
            {
                int i = 0;
                while (i < firstNumOfVerteces)
                {
                    if (secondListOfVertices.FindIndex(
                            vert => vert.IsReflectionOf(firstListOfVertices[i], candidateReflMyPlane)) != -1)
                    {
                        var found =
                            secondListOfVertices.Find(
                                vert => vert.IsReflectionOf(firstListOfVertices[i], candidateReflMyPlane));

                        var scarto =
                            new MyVertex(Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).x - found.x),
                                Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).y - found.y),
                                Math.Abs(firstListOfVertices[i].Reflect(candidateReflMyPlane).z - found.z));
                       
                        i++;
                    }
                    else
                    {
                       return false;
                    }
                }
            }
            else
            {
                return false;
            }

            //Check of correct position of normals of all Planar face:
            if (!CheckOfPlanesForReflection(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateReflMyPlane))
            {
                return false;
            }

            ////Check of correct position of cylinder faces:
            if (!CheckOfCylindersForReflection(firstMyRepeatedEntity, secondMyRepeatedEntity, candidateReflMyPlane))
            {
                return false;
            }

            ////CONTINUARE CON GLI ALTRI TIPI DI SUPERFICI............
            //KLdebug.Print("   ====>>> TRASLAZIONE TRA QUESTE DUE re VERIFICATA!", nameFile);
            return true;
        }

    }
}
