using System.Collections.Generic;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using SolidWorks.Interop.sldworks;
//using Functions.DataStructure;
//using Functions.Functions;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class ExtractInfoFromBRep
    {
        // The function takes as input - a Face2
        //.                            - a list of Face2 to remove from the the adjacent ones 
        //It returns the list of the surfaces corresponding to the faces adjacent to the input face but not in the 
        //list of faces to remove.        
        // (this list of faces to remove is provided in preparation for the next function, as for every
        // face of a RE we want to consider the surfaces corresponding only to faces of the BRep different
        // from the faces of the RE itself).

        public static List<Surface> GetSurfacesOfFacesAdjacentToFace(Face2 inputFace, List<Face2> listOfFacesToRemoveFromTheAdjacents, SldWorks swApplWorks)
        {
            List<Surface> outputListOfSurfaces = new List<Surface>();

            List<Face2> listOfAdjacentFaces = new List<Face2>();
            listOfAdjacentFaces = BRepFunctions.MyAdjacenceFace(inputFace, swApplWorks);

            if (listOfAdjacentFaces == null)
            {
                return outputListOfSurfaces;
            }

            const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            KLdebug.Print("Numero di Facce adiacenti di questa faccia: " + listOfAdjacentFaces.Count.ToString(), fileNameBuildRepeatedEntity);
            KLdebug.Print(" ", "buildRepeatedEntity.txt");

            foreach (var face in listOfFacesToRemoveFromTheAdjacents)
            {
                KLdebug.Print("face id listOfFacesToRemoveFromTheAdjacents: " + face.GetFaceId(), fileNameBuildRepeatedEntity);
            }
            foreach (var face in listOfAdjacentFaces)
            {
                KLdebug.Print("face id listOfAdjacentFaces: " + face.GetFaceId(), fileNameBuildRepeatedEntity);
            }
            


            listOfAdjacentFaces.RemoveAll(face => listOfFacesToRemoveFromTheAdjacents.FindIndex(facebis => face.GetFaceId() == facebis.GetFaceId()) != -1);

            KLdebug.Print("Rimosso le altre facce della Repeated Entity da quelle da esaminare." +
                              " Sono rimaste " + listOfAdjacentFaces.Count.ToString() + " facce.", fileNameBuildRepeatedEntity);

            #region stampa dati della superficie esaminata
            //var thisSurface = (Surface)inputFace.GetSurface();

            //var firstPlaneParameters1 = thisSurface.PlaneParams;
            //double[] firstPlaneNormalUnchecked1 = { firstPlaneParameters1[0], firstPlaneParameters1[1], firstPlaneParameters1[2] };
            //double[] firstPlanePoint1 = { firstPlaneParameters1[3], firstPlaneParameters1[4], firstPlaneParameters1[5] };
            //double[] firstPlaneNormal1 = GeometryFunctions.MyGetNormalForPlaneFace(inputFace, out firstPlaneNormalUnchecked1);

            //MyPlane firstPlane1 = new MyPlane(firstPlaneNormal1, firstPlanePoint1);

            //KLdebug.Print("Superficie della faccia in esame:", fileNameBuildRepeatedEntity);
            //string whatToWrite1 = string.Format("a {0}, b {1}, c {2}, d {3}", firstPlane1.a, firstPlane1.b, firstPlane1.c, firstPlane1.d);
            //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", "buildRepeatedEntity.txt");
            #endregion

            int index = 0;

            foreach (Face2 face in listOfAdjacentFaces)
            {

                var newSurface = (Surface)face.GetSurface();

                #region stampa dati della superficie trovata adiacente a quella esaminata
                //var firstPlaneParameters = newSurface.PlaneParams;
                //double[] firstPlaneNormalUnchecked = { firstPlaneParameters[0], firstPlaneParameters[1], firstPlaneParameters[2] };
                //double[] firstPlanePoint = { firstPlaneParameters[3], firstPlaneParameters[4], firstPlaneParameters[5] };
                //double[] firstPlaneNormal = GeometryFunctions.MyGetNormalForPlaneFace(face, out firstPlaneNormalUnchecked);

                //MyPlane firstPlane = new MyPlane(firstPlaneNormal, firstPlanePoint);

                ////string whatToWriteBis = string.Format("Primo normale: a {0}, b {1}, c {2}", firstPlaneNormal.GetValue(0), firstPlaneNormal.GetValue(1), firstPlaneNormal.GetValue(2));
                //string whatToWrite = string.Format("a {0}, b {1}, c {2}, d {3}", firstPlane.a, firstPlane.b, firstPlane.c, firstPlane.d);
                ////KLdebug.Print(whatToWriteBis, fileNameBuildRepeatedEntity);
                //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                #endregion

                string whatToWrite1 = "";

                if (outputListOfSurfaces.FindIndex(surface => MyEqualsSurface(surface, newSurface, swApplWorks)) == -1)
                {
                    outputListOfSurfaces.Add(newSurface);
                    whatToWrite1 = string.Format("Trovata Nuova superficie: tipo superficie: {0}", newSurface.Identity());
                    KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                    KLdebug.Print(" ", fileNameBuildRepeatedEntity);

                    index++;
                }
                else
                {
                    //var indexOfExistingSurface = outputListOfSurfaces.FindIndex(surface => myEqualsSurface(surface, newSurface));
                    whatToWrite1 = string.Format("Trovata superficie per questa faccia di tipo {0} ma già trovata per questa faccia.", newSurface.Identity());
                    KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                    KLdebug.Print(" ", fileNameBuildRepeatedEntity);

                }
            }
            return outputListOfSurfaces;
        }

        // The function takes as input - a MyRepeatedEntity
        //.                            - the list of MyGroupingSurface to update (ref)
        //It updates the list of MyGroupingSurface, adding the new MyGroupingSurface-s resulting
        //from the current MyRepeatedEntity
        public static void GetSurfacesOfFacesAdjacentToSetOfFaces(MyRepeatedEntity newRepeatedEntity, ref List<MyGroupingSurface> listOfGroupingSurfaces, SldWorks SwApplication)
        {
            const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            var whatToWrite = "";

            var inputSetOfFaces = newRepeatedEntity.listOfFaces;
            List<Surface> listOfSurfacesAdjacentToAFace = new List<Surface>();
            List<Face2> listOfOtherFaces = new List<Face2>(inputSetOfFaces);

            foreach (Face2 face in inputSetOfFaces)
            {
                whatToWrite = string.Format(" ");
                KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                whatToWrite = string.Format("NUOVA FACCIA di cui esaminiamo le superfici adiacenti");
                KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                listOfSurfacesAdjacentToAFace.Clear();

                listOfOtherFaces.Remove(face);
                KLdebug.Print("listOfOtherFaces.Count = " + listOfOtherFaces.Count, fileNameBuildRepeatedEntity);
                listOfSurfacesAdjacentToAFace = GetSurfacesOfFacesAdjacentToFace(face, listOfOtherFaces,
                    SwApplication);

                var index = 0;
                var indexbis = 0;
                foreach (Surface surface in listOfSurfacesAdjacentToAFace)
                {
                    //SwApplication.SendMsgToUser("Ci sono superfici: " + listOfSurfacesAdjacentToAFace.Count);

                    //Find if there already exists this MyGroupingSurface:
                    var indexOfFound =
                        listOfGroupingSurfaces.FindIndex(
                            myGroupingSurface =>
                                MyEqualsSurface(myGroupingSurface.groupingSurface, surface, SwApplication));
                    if (indexOfFound == -1)
                    {
                        //If it does not, create a new MyGroupingSurface:
                        var newListOfRepeatedEntityOfGroupingSurface = new List<MyRepeatedEntity>();
                        newListOfRepeatedEntityOfGroupingSurface.Add(newRepeatedEntity);
                        //var newListOfIdOfRepeatedEntityOfGroupingSurface = new List<int>();
                        //newListOfIdOfRepeatedEntityOfGroupingSurface.Add(idOfNewRepeatedEntity);
                        var newMyGroupingSurface = new MyGroupingSurface(surface,
                            newListOfRepeatedEntityOfGroupingSurface);
                        listOfGroupingSurfaces.Add(newMyGroupingSurface);

                        KLdebug.Print(" ", fileNameBuildRepeatedEntity);
                        whatToWrite = string.Format("Ho aggiunto una nuova MyGroupingSurface di tipo: {0}",
                            surface.Identity());
                        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                        index++;
                    }
                    else
                    {
                        //If it does, update the corresponding MyGroupingSurface if the current RE is not in the set yet:
                        var myGroupingSurfaceOfFound = listOfGroupingSurfaces[indexOfFound];
                        if (
                            myGroupingSurfaceOfFound.listOfREOfGS.FindIndex(re => re.idRE == newRepeatedEntity.idRE) ==
                            -1)
                        {
                            myGroupingSurfaceOfFound.listOfREOfGS.Add(newRepeatedEntity);
                            //myGroupingSurfaceOfFound.listOfIdOfRE.Add(idOfNewRepeatedEntity);

                            indexbis++;


                            KLdebug.Print(" ", fileNameBuildRepeatedEntity);
                            whatToWrite = string.Format("Ho aggiornato una vecchia MyGroupingSurface di tipo: {0}",
                                myGroupingSurfaceOfFound.groupingSurface.Identity());
                            KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                        }
                    }
                }

                KLdebug.Print(" ", fileNameBuildRepeatedEntity);
                whatToWrite = string.Format("Ho aggiunto {0} superfici nuove per questa RepeatedEntity.", index);
                KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                KLdebug.Print(" ", fileNameBuildRepeatedEntity);
                whatToWrite = string.Format("Ho aggiornato {0} superfici vecchie per questa RepeatedEntity.",
                    indexbis);
                KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                listOfOtherFaces.Add(face);
            }
            KLdebug.Print(" ", fileNameBuildRepeatedEntity);

        }



        #region Ex versione: getSurfacesOfFacesAdjacentToSetOfFaces con output una List<Surface>; void UpdateListOfGroupingSurfaces
        //public static List<Surface> getSurfacesOfFacesAdjacentToSetOfFaces(List<Face2> inputSetOfFaces, SldWorks SwApplication)
        //{
        //    const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
        //    var whatToWrite = "";

        //    List<Surface> outputListOfSurfaces = new List<Surface>();
        //    List<Surface> listOfSurfacesAdjacentToAFace = new List<Surface>();

        //    List<Face2> listOfOtherFaces = new List<Face2>(inputSetOfFaces);

        //    foreach (Face2 face in inputSetOfFaces)
        //    {
        //        whatToWrite = string.Format(" ");
        //        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
        //        whatToWrite = string.Format("NUOVA FACCIA di cui esaminiamo le superfici adiacenti");
        //        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

        //        listOfSurfacesAdjacentToAFace.Clear();

        //        listOfOtherFaces.Remove(face);
        //        listOfSurfacesAdjacentToAFace = getSurfacesOfFacesAdjacentToFace(face, listOfOtherFaces, SwApplication);

        //        var index = 0;
        //        foreach (Surface surface in listOfSurfacesAdjacentToAFace)
        //        {
        //            //SwApplication.SendMsgToUser("Ci sono superfici: " + listOfSurfacesAdjacentToAFace.Count);

        //            if (outputListOfSurfaces.FindIndex(surfaceInList => myEqualsSurface(surfaceInList, surface)) == -1)
        //            {
        //                outputListOfSurfaces.Add(surface);

        //                KLdebug.Print(" ", fileNameBuildRepeatedEntity);
        //                whatToWrite = string.Format("Ho aggiunto una superficie adiacente nuova di tipo: {0}", surface.Identity());
        //                KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

        //                index++;
        //            }
        //        }

        //        KLdebug.Print(" ", fileNameBuildRepeatedEntity);
        //        whatToWrite = string.Format("Ho aggiunto {0} superfici per questa faccia.", index);
        //        KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

        //        listOfOtherFaces.Add(face);
        //    }
        //    KLdebug.Print(" ", fileNameBuildRepeatedEntity);
        //    whatToWrite = string.Format("Numero di superfici adiacenti alla MyRepeatedEntity sono: {0}", outputListOfSurfaces.Count);
        //    KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
        //    KLdebug.Print(" ", fileNameBuildRepeatedEntity);

        //    return outputListOfSurfaces;
        //}

        //public static void UpdateListOfGroupingSurfaces(ref List<MyGroupingSurface> listOfGroupingSurfaces, MyRepeatedEntity newRepeatedEntity, List<Surface> newAdjacentSurfacesList)
        //{
        //    foreach (Surface newSurf in newAdjacentSurfacesList)
        //    {
        //        var indexOfFound = listOfGroupingSurfaces.FindIndex(myGroupingSurf => myEqualsSurface(myGroupingSurf.groupingSurface, newSurf));
        //        if (indexOfFound == -1)
        //        {
        //            var listOfRepeatedEntitiesOfNewGroupingSurface = new List<MyRepeatedEntity>();
        //            listOfRepeatedEntitiesOfNewGroupingSurface.Add(newRepeatedEntity);
        //            MyGroupingSurface newGroupingSurface = new MyGroupingSurface(newSurf, listOfRepeatedEntitiesOfNewGroupingSurface);
        //            listOfGroupingSurfaces.Add(newGroupingSurface);
        //        }
        //        else
        //        {
        //            listOfGroupingSurfaces[indexOfFound].listOfREOfGS.Add(newRepeatedEntity);
        //        }
        //    }
        //}
        #endregion
        //La nuova versione evita di scorrere di nuovo la lista di superfici adiacenti ad una RepeatedEntity, anche perché
        //per ogni RepeatedEntity in realtà non mi serve la lista delle superfici adiacenti.
        //Memorizzo per ogni superficie la lista di RepeatedEntity che la toccano.
        //Quindi mi basta aggiornare ogni volta la lista delle MyGroupingSurface: 
        //se spunta una nuova superficie creo una nuova MyGroupingSurface, altrimenti aggiorno
        //la MyGroupingSurface esistente.
    }
}
