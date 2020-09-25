using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {


        public static void CheckAndUpdate(MyPattern newPattern, ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyGroupingSurface> listOfMyGroupingSurface,
            ref List<MyPattern> listOfOutputPattern, ref List<MyPattern> listOfOutputPatternTwo)
        {
            
            var lengthOfPattern = newPattern.listOfMyREOfMyPattern.Count;

            // if lengthOfPattern = 2, I add the newPatternPoint only if there is not another pattern in listOfOutputPatternTwo 
            // containing one of the two RE in the newPatternPoint.
            if (lengthOfPattern == 2)
            {
                int i = 0;
                var addOrNot = true;
                while (addOrNot == true && i < 2)
                {
                    var currentRE = newPattern.listOfMyREOfMyPattern[i];
                    var indOfFound =
                        listOfOutputPatternTwo.FindIndex(
                            pattern => pattern.listOfMyREOfMyPattern.FindIndex(re => re.idRE == currentRE.idRE) != -1);
                    if (indOfFound != -1)
                    {
                        addOrNot = false;
                    }
                    i++;
                }

                if (addOrNot == true)
                {
                    listOfOutputPatternTwo.Add(newPattern);
                }
             

            }
            // if lengthOfPattern > 2, I add the newPatternPoint and I update the other data
            // (aiming not to find pattern containing RE already set in this newPatternPoint)
            else
            {

                listOfOutputPattern.Add(newPattern);

                UpdateOtherData(newPattern, ref listOfPathOfCentroids, listOfREOnThisSurface, ref listOfMatrAdj,
                    ref listOfMyGroupingSurface, ref listOfOutputPatternTwo);
            }

        }
        
        //Update data in case on newPatternPoint of length > 2
        public static void UpdateOtherData(MyPattern newPattern, ref List<MyPathOfPoints> listOfPathOfCentroids,
            List<MyRepeatedEntity> listOfREOnThisSurface, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyGroupingSurface> listOfMyGroupingSurface, ref List<MyPattern> listOfOutputPatternTwo)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
         
            foreach (var re in newPattern.listOfMyREOfMyPattern)
            {
                var indOfThisCentroid = listOfREOnThisSurface.IndexOf(re);

                UpdateListOfMyPathOfCentroids(ref listOfPathOfCentroids, indOfThisCentroid, nameFile);
                UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisCentroid, nameFile);
                UpdateListOfMyGroupingSurface(re, ref listOfMyGroupingSurface, indOfThisCentroid);
                UpdateListOfPatternTwo(re, ref listOfOutputPatternTwo, indOfThisCentroid);

            }
        }

        //Update list of Pattern geometrically verified of length = 2: delete the ones containing
        //a RE of the newPatternPoint
        public static void UpdateListOfPatternTwo(MyRepeatedEntity re, 
            ref List<MyPattern> listOfOutputPatternTwo, int indOfThisCentroid)
        {
            var indOfFound =
                listOfOutputPatternTwo.FindIndex(
                    pattern => pattern.listOfMyREOfMyPattern.FindIndex(reInPattern => reInPattern.idRE == re.idRE) != -1);
            if (indOfFound != -1)
            {
                var found = listOfOutputPatternTwo.Find(
                    pattern => pattern.listOfMyREOfMyPattern.FindIndex(reInPattern => reInPattern.idRE == re.idRE) != -1);

                listOfOutputPatternTwo.Remove(found);
            }
        }

        //Update the list of MyGroupingSurface, deleting the MyRepeatedEntity already set in the newPatternPoint
        public static void UpdateListOfMyGroupingSurface(MyRepeatedEntity re, 
            ref List<MyGroupingSurface> listOfMyGroupingSurface, int indOfThisCentroid)
        {
            const string nameFile = "GetTranslationalPatterns.txt";

            var indOfFound =
                listOfMyGroupingSurface.FindIndex(
                    gs => gs.listOfREOfGS.FindIndex(reInGS => reInGS.idRE == re.idRE) != -1);
            if (indOfFound != -1)
            {
                var listOfGroupingSurfaceToUpdate = listOfMyGroupingSurface.FindAll(
                    gs => gs.listOfREOfGS.FindIndex(reInGS => reInGS.idRE == re.idRE) != -1);

                foreach (var gs in listOfGroupingSurfaceToUpdate)
                {

                    gs.listOfREOfGS.Remove(re);
                    if (gs.listOfREOfGS.Count < 2)
                    {
                        listOfMyGroupingSurface.Remove(gs);
                    }
                }              
            }

        }

        //Update the list of MyMatrAdj: it delete all the relations stored in the various matrices
        //related to the centroids involved in the current newPatternPoint
        //(this function is used also in CheckAndUpdate_ComposedPatterns)
        public static void UpdateListOfMyMatrAdj(ref List<MyMatrAdj> listOfMatrAdj, 
            int indOfThisCentroid, string nameFile)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("     ---> UpdateListOfMyMatrAdj", nameFile);

            
            //KLdebug.Print("indOfThisCentroid nella lista di tutti i centroids in GS: " + indOfThisCentroid, nameFile);
            //KLdebug.Print(" AGGIORNO TUTTE LE MATRADJ mandando a 0 le entrate corrispondenti al centroid della current RE:", nameFile);
            //KLdebug.Print(" Numero di MatrAdj nella lista: " + listOfMatrAdj.Count, nameFile);

            if (listOfMatrAdj.Count > 0)
            {
                var matrAdjDim = listOfMatrAdj[0].matr.GetLength(1);
                foreach (var matrAdj in listOfMatrAdj)
                {
                    //KLdebug.Print(" MatrAdj position: " + listOfMatrAdj.IndexOf(matrAdj), nameFile);
                    //KLdebug.Print(" d della MatrAdj: " + matrAdj.d, nameFile);
                    //KLdebug.Print(" nOccur prima dell'aggiornamento: " + matrAdj.nOccur, nameFile);


                    for (int i = 0; i < matrAdjDim; i++)
                    {
                        if (matrAdj.matr[indOfThisCentroid, i] != 0)
                        {
                            matrAdj.matr[indOfThisCentroid, i] = 0;
                            //KLdebug.Print(" porto a 0 l'entrata (" + indOfThisCentroid + "," + i + ")" , nameFile);
                            matrAdj.matr[i, indOfThisCentroid] = 0;
                           // KLdebug.Print(" porto a 0 l'entrata (" + i + "," + indOfThisCentroid + ")", nameFile);
                            matrAdj.nOccur -= 1;
                        }
                    }

                    //KLdebug.Print(" nOccur dopo dell'aggiornamento: " + matrAdj.nOccur, nameFile);
                    //KLdebug.Print(" ", nameFile);

                }

               // KLdebug.Print("Elimino le matrAdj con nOccur <2 ", nameFile);
                var listOfFound = listOfMatrAdj.FindAll(matrAdj => matrAdj.nOccur < 2);
                if (listOfFound.Any())
                {
                    
                    //KLdebug.Print(" Ci sono " + listOfFound.Count + " matrAdj da cancellare", nameFile);
                    foreach (var matrAdj in listOfFound)
                    {
                  //      KLdebug.Print(" calcello la MatrAdj con d=" + matrAdj.d, nameFile);
                        listOfMatrAdj.Remove(matrAdj);
                    }
                }
                //KLdebug.Print("Sono rimaste " + listOfMatrAdj.Count + " matrAdj.", nameFile);

                //   Successivamente si levano le stampe e basta fare questo:
                //listOfMatrAdj.RemoveAll(matrAdj => matrAdj.nOccur < 2);
            }
            //else
            //{
            //    KLdebug.Print("La Lista delle MatrAdj di questa GS è vuota!", nameFile);

            //}
            
            
        }

        //Update the list of paths of centroids found in the current MyMatrAdj
        public static void UpdateListOfMyPathOfCentroids(ref List<MyPathOfPoints> listOfPathOfCentroids,
            int indOfThisCentroid, string nameFile)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("     ---> UpdateListOfMyPathOfCentroids", nameFile);

          
            // I check if there are candidate MyPathOfPoints not geometrically verified yet:
            // for each of them I split them in 2 MyPathOfPoints sons
            // if the two sons are long enough I save them for a future Geometrical Verification.
            
            var listOfPathOfCentroidsToSplit = listOfPathOfCentroids.FindAll(
                    myPathOfCentroids => myPathOfCentroids.path.FindIndex(ind => ind == indOfThisCentroid) != -1);
            if (listOfPathOfCentroidsToSplit.Any())
            {
                //KLdebug.Print(" Trovati Path contenenti il centroid della RE corrente: ", nameFile);

                //KLdebug.Print(" Numero di path trovati: " + listOfPathOfCentroidsToSplit.Count, nameFile);
                //KLdebug.Print(" ", nameFile);

                foreach (var myPathOfCentroids in listOfPathOfCentroidsToSplit)
                {
                    //KLdebug.Print(" -considero il " + listOfPathOfCentroidsToSplit.IndexOf(myPathOfCentroids) + "path:", nameFile);
                    //for (int i = 0; i < myPathOfCentroids.path.Count; i++)
                    //{
                    //    KLdebug.Print("      " + myPathOfCentroids.path[i], nameFile);
                    //}

                    var indInPathOfThisCentroid = myPathOfCentroids.path.IndexOf(indOfThisCentroid);
                    //KLdebug.Print("  in questo path il centroid di Current RE è al posto " + indInPathOfThisCentroid, nameFile);

                    var firstPartOfThePath = myPathOfCentroids.path.GetRange(0, indInPathOfThisCentroid);
                    var firstPartOfThePathLength = firstPartOfThePath.Count;
                    //KLdebug.Print("Spezzo il path in un 1° path di lunghezza " + firstPartOfThePathLength, nameFile);

                    if (firstPartOfThePathLength > 2)
                    {
                        var newSonPath = new MyPathOfPoints(firstPartOfThePath,
                            myPathOfCentroids.pathGeometricObject);
                        listOfPathOfCentroids.Add(newSonPath);
                        //KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
                        //for (int i = 0; i < newSonPath.path.Count; i++)
                        //{
                        //    KLdebug.Print("      " + newSonPath.path[i], nameFile);
                        //}
                    }
                    //else
                    //{
                    //    KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);

                    //}

                    var secondPartOfThePath = myPathOfCentroids.path.GetRange(indInPathOfThisCentroid + 1,
                        myPathOfCentroids.path.Count - (indInPathOfThisCentroid + 1));
                    var secondPartOfThePathLength = secondPartOfThePath.Count;
                    //KLdebug.Print("Spezzo il path in un 2° path di lunghezza " + secondPartOfThePathLength, nameFile);

                    if (secondPartOfThePathLength > 2)
                    {
                        var newSonPath = new MyPathOfPoints(secondPartOfThePath,
                            myPathOfCentroids.pathGeometricObject);
                        listOfPathOfCentroids.Add(newSonPath);
                        //KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
                        //for (int i = 0; i < newSonPath.path.Count; i++)
                        //{
                        //    KLdebug.Print("      " + newSonPath.path[i], nameFile);
                        //}
                    }
                    //else
                    //{
                    //    KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);
                    //}

                    listOfPathOfCentroids.Remove(myPathOfCentroids);
                    //KLdebug.Print("RIMOSSO il Path spezzato in due.", nameFile);
                    //KLdebug.Print(" ", nameFile);

                }
                listOfPathOfCentroids = listOfPathOfCentroids.OrderByDescending(x => x.path.Count).ThenBy(y => y.pathGeometricObject.GetType() == typeof(MyLine) ? 0 : 1).ToList();
                //KLdebug.Print("Riordino la lista listOfPathOfCentroids ancora da geometricamente verificare.", nameFile);
            }
            else
            {
                //KLdebug.Print("Non ho trovato nessun path che intersechi la RE " + indOfThisCentroid, nameFile);
            }

           // KLdebug.Print(" ", nameFile);

        }
    }
}

