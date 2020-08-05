using System.Collections.Generic;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {

        public static void CheckAndUpdate_Assembly(MyPatternOfComponents newPattern,
            ref List<MyPathOfPoints> listOfPathOfPoints,
            List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            KLdebug.Print(" ", nameFile);
            KLdebug.Print(" ", nameFile);
            KLdebug.Print("             UPDATE STEP", nameFile);
            KLdebug.Print("CURRENT SITUATION:", nameFile);
            KLdebug.Print("listOfOutputPattern.Count = " + listOfOutputPattern.Count, nameFile);
            KLdebug.Print("listOfOutputPatternTwo.Count = " + listOfOutputPatternTwo.Count, nameFile);
            KLdebug.Print(" ", nameFile);

            var lengthOfPattern = newPattern.listOfMyRCOfMyPattern.Count;

            // if lengthOfPattern = 2, I add the newPattern only if there is not another pattern in listOfOutputPatternTwo 
            // containing one of the two RE in the newPattern.
            if (lengthOfPattern == 2)
            {
                KLdebug.Print(
                    "Entrata per aggiornare con l'inserimento di un pattern di lunghezza lengthOfPattern = " +
                    lengthOfPattern, nameFile);

                int i = 0;
                var addOrNot = true;
                while (addOrNot == true && i < 2)
                {
                    var currentRC = newPattern.listOfMyRCOfMyPattern[i];
                    var indOfFound =
                        listOfOutputPatternTwo.FindIndex(
                            pattern =>
                                pattern.listOfMyRCOfMyPattern.FindIndex(
                                    comp => Equals(comp.Transform, currentRC.Transform)) != -1);
                    if (indOfFound != -1)
                    {
                        addOrNot = false;
                    }
                    i++;
                }

                if (addOrNot == true)
                {
                    listOfOutputPatternTwo.Add(newPattern);
                    KLdebug.Print(
                        "AGGIUNTO! Non ho trovato altri Pattern da 2 con intersezione non nulla con il corrente.",
                        nameFile);
                }
                else
                {
                    KLdebug.Print("NON AGGIUNTO! Trovato altro Pattern da 2 che interseca questo.", nameFile);
                }

            }
            // if lengthOfPattern > 2, I add the newPattern and I update the other data
            // (aiming not to find pattern containing RE already set in this newPattern)
            else
            {
                KLdebug.Print("Entrata nel lengthOfPattern = " + lengthOfPattern, nameFile);

                listOfOutputPattern.Add(newPattern);
                KLdebug.Print("AGGIUNTO (senza verifiche..)", nameFile);

                UpdateOtherData_Assembly(newPattern, ref listOfPathOfPoints, listOfOrigins, ref listOfMatrAdj,
                   ref listOfOutputPatternTwo);
            }
        }


        public static void KLCheckAndUpdate_Assembly(MyPatternOfComponents newPattern,
            ref List<MyPathOfPoints> listOfPathOfPoints,
            List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo)
        {

            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("             UPDATE STEP", nameFile);
            //KLdebug.Print("CURRENT SITUATION:", nameFile);
            //KLdebug.Print("listOfOutputPattern.Count = " + listOfOutputPattern.Count, nameFile);
            //KLdebug.Print("listOfOutputPatternTwo.Count = " + listOfOutputPatternTwo.Count, nameFile);
            //KLdebug.Print(" ", nameFile);

            var lengthOfPattern = newPattern.listOfMyRCOfMyPattern.Count;

            // if lengthOfPattern = 2, I add the newPatternPoint only if there is not another pattern in listOfOutputPatternTwo 
            // containing one of the two RE in the newPatternPoint.
            if (lengthOfPattern == 2)
            {
                //KLdebug.Print("lengh 2: Entrata nel lengthOfPattern = " + lengthOfPattern, nameFile);

                int i = 0;
                var addOrNot = true;
                while (addOrNot == true && i < 2)
                {
                    if (i < newPattern.listOfMyRCOfMyPattern.Count)
                    {
                        var currentRE = newPattern.listOfMyRCOfMyPattern[i];
                        var indOfFound =
                            listOfOutputPatternTwo.FindIndex(
                                pattern =>
                                    pattern.listOfMyRCOfMyPattern.FindIndex(
                                        re => re.RepeatedEntity.idRE == currentRE.RepeatedEntity.idRE) != -1);
                        if (indOfFound != -1)
                        {
                            addOrNot = false;
                        }
                    }
                    i++;
                    
                }

                if (addOrNot == true)
                {
                    listOfOutputPatternTwo.Add(newPattern);
                    //KLdebug.Print("AGGIUNTO! Non ho trovato altri Pattern da 2 con intersezione non nulla con il corrente.", nameFile);
                }
                else
                {
                    //KLdebug.Print("NON AGGIUNTO! Trovato altro Pattern da 2 che interseca questo.", nameFile);
                }

            }
            // if lengthOfPattern > 2, I add the newPatternPoint and I update the other data
            // (aiming not to find pattern containing RE already set in this newPatternPoint)
            else
            {
                //KLdebug.Print("Entrata nel lengthOfPattern = " + lengthOfPattern, nameFile);

                listOfOutputPattern.Add(newPattern);
                //KLdebug.Print("AGGIUNTO (senza verifiche..) faccio update", nameFile);

                UpdateOtherData_Assembly(newPattern, ref listOfPathOfPoints, listOfOrigins, ref listOfMatrAdj,
                    ref listOfOutputPatternTwo);
            }

            
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("             UPDATE STEP", nameFile);
            //KLdebug.Print("CURRENT SITUATION:", nameFile);
            //KLdebug.Print("listOfOutputPattern.Count = " + listOfOutputPattern.Count, nameFile);
            //KLdebug.Print("listOfOutputPatternTwo.Count = " + listOfOutputPatternTwo.Count, nameFile);
            //KLdebug.Print(" ", nameFile);

            //var lengthOfPattern = newPattern.listOfMyRCOfMyPattern.Count;

            //// if lengthOfPattern = 2, I add the newPattern only if there is not another pattern in listOfOutputPatternTwo 
            //// containing one of the two RE in the newPattern.
            //if (lengthOfPattern == 2)
            //{
            //    KLdebug.Print(
            //        "Entrata per aggiornare con l'inserimento di un pattern di lunghezza lengthOfPattern = " +
            //        lengthOfPattern, nameFile);

            //    int i = 0;
            //    var addOrNot = true;
            //    while (addOrNot == true && i < 2)
            //    {
            //        var currentRC = newPattern.listOfMyRCOfMyPattern[i];
            //        var listOfMyREOfMyPattern = new List<MyRepeatedEntity>(
            //            newPattern.listOfMyRCOfMyPattern.Select(ent => ent.RepeatedEntity)).ToList();
                    
            //        var currentRE = currentRC.RepeatedEntity;                   
            //        var indOfFound = listOfMyREOfMyPattern.FindIndex(re => re.idRE == currentRE.idRE);
            //        if (indOfFound != -1)
            //        {
            //            addOrNot = false;
            //        }
            //        i++;
            //    }

            //    if (addOrNot == true)
            //    {
            //        listOfOutputPatternTwo.Add(newPattern);
            //        KLdebug.Print(
            //            "AGGIUNTO! Non ho trovato altri Pattern da 2 con intersezione non nulla con il corrente.",
            //            nameFile);
            //    }
            //    else
            //    {
            //        KLdebug.Print("NON AGGIUNTO! Trovato altro Pattern da 2 che interseca questo.", nameFile);
            //    }

            //}
            //// if lengthOfPattern > 2, I add the newPattern and I update the other data
            //// (aiming not to find pattern containing RE already set in this newPattern)
            //else
            //{
            //    KLdebug.Print("Entrata nel lengthOfPattern = " + lengthOfPattern, nameFile);

            //    listOfOutputPattern.Add(newPattern);
            //    KLdebug.Print("AGGIUNTO (senza verifiche..)", nameFile);

            //    UpdateOtherData_Assembly(newPattern, ref listOfPathOfPoints, listOfOrigins, ref listOfMatrAdj,
            //        ref listOfOutputPatternTwo);
            //}
        }

        //Update data in case on newPattern of length > 2
        public static void UpdateOtherData_Assembly(MyPatternOfComponents newPattern,
            ref List<MyPathOfPoints> listOfPathOfPoints, List<MyVertex> listOfOrigins,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyPatternOfComponents> listOfOutputPatternTwo)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("--> AGGIORNAMENTO DI TUTTI GLI ALTRI DATI:", nameFile);

            foreach (var comp in newPattern.listOfMyRCOfMyPattern)
            {
                
                //var listOfREOnThisSurface =
                //new List<MyRepeatedEntity>(newPattern.listOfMyRCOfMyPattern.Select(re => comp.RepeatedEntity));
                //var indOfThisOrigin = listOfREOnThisSurface.IndexOf(comp.RepeatedEntity);
                var indOfThisOrigin = listOfOrigins.IndexOf(comp.Origin);
                //KLdebug.Print("UPDATE per RE n° " + indOfThisOrigin, nameFile);

                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyPathOfCentroids(ref listOfPathOfPoints, indOfThisOrigin, nameFile);
                Part.PartUtilities.GeometryAnalysis.UpdateListOfMyMatrAdj(ref listOfMatrAdj, indOfThisOrigin, nameFile);
                KLUpdateListOfPatternTwo_Assembly(comp, ref listOfOutputPatternTwo, indOfThisOrigin);

                //KLdebug.Print(" ", nameFile);

            }
        }

        //Update list of Pattern geometrically verified of length = 2: delete the ones containing
        //a RE of the newPattern
        public static void UpdateListOfPatternTwo_Assembly(MyRepeatedComponent comp,
            ref List<MyPatternOfComponents> listOfOutputPatternTwo, int indOfThisOrigin)
        {
            const string nameFile = "GetTranslationalPatterns.txt";
            KLdebug.Print("     ---> UpdateListOfPatternTwo", nameFile);

           var found = listOfOutputPatternTwo.Find(
                    pattern =>
                        pattern.listOfMyRCOfMyPattern.FindIndex(
                            compInPattern => Equals(compInPattern.Transform, comp.Transform)) != -1);
            if (found != null)
            {
                
                KLdebug.Print(
                    " Trovato pattern da 2 contenente la COMP corrente (posiz :" + indOfThisOrigin + "):", nameFile);
                KLdebug.Print(" Lunghezza (deve essere 2): " + found.listOfMyRCOfMyPattern.Count, nameFile);
                KLdebug.Print(" Posizione nella lista: " + listOfOutputPatternTwo.IndexOf(found), nameFile);
                KLdebug.Print(
                    " Origin 1^ RE nel pattern: (" + found.listOfMyRCOfMyPattern[0].Origin.x + "," +
                    found.listOfMyRCOfMyPattern[0].Origin.y + "," + found.listOfMyRCOfMyPattern[0].Origin.z + ")",
                    nameFile);
                KLdebug.Print(
                    " Origin 2^ RE nel pattern: (" + found.listOfMyRCOfMyPattern[1].Origin.x + "," +
                    found.listOfMyRCOfMyPattern[1].Origin.y + "," + found.listOfMyRCOfMyPattern[1].Origin.z + ")",
                    nameFile);
                KLdebug.Print("(Uno di questi due origin deve essere quello della current COMP)", nameFile);


                listOfOutputPatternTwo.Remove(found);
                KLdebug.Print(" RIMOSSO dalla lista listOfOutputPatternTwo!", nameFile);

            }
            KLdebug.Print(" ", nameFile);

        }

        public static void KLUpdateListOfPatternTwo_Assembly(MyRepeatedComponent comp,
           ref List<MyPatternOfComponents> listOfOutputPatternTwo, int indOfThisOrigin)
        {
            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print("     ---> UpdateListOfPatternTwo", nameFile);

            var found = listOfOutputPatternTwo.Find(
                pattern => pattern.listOfMyRCOfMyPattern.FindIndex(reInPattern => reInPattern.RepeatedEntity.idRE == comp.RepeatedEntity.idRE) != -1);

            if (found != null)
            {
                listOfOutputPatternTwo.Remove(found);
                //KLdebug.Print(" RIMOSSO dalla lista listOfOutputPatternTwo!", nameFile);

            }
            //KLdebug.Print(" ", nameFile);

        }

       


        // NOT USED: THE "UpdateListOfMyPathOfCentroids" BUILT FOR PART IS EQUAL TO IT!

        ////Update the list of paths of centroids found in the current MyMatrAdj
        //public static void UpdateListOfMyPathOfCentroids_Assembly(ref List<MyPathOfPoints> listOfPathOfCentroids, 
        //    int indOfThisCentroid)
        //{
        //    const string nameFile = "GetTranslationalPatterns.txt";
        //    KLdebug.Print("     ---> UpdateListOfMyPathOfCentroids", nameFile);


        //    // I check if there are candidate MyPathOfPoints not geometrically verified yet:
        //    // for each of them I split them in 2 MyPathOfPoints sons
        //    // if the two sons are long enough I save them for a future Geometrical Verification.
        //    var indOfFound =
        //        listOfPathOfCentroids.FindIndex(
        //            myPathOfCentroids => myPathOfCentroids.path.FindIndex(ind => ind == indOfThisCentroid) != -1);
        //    if (indOfFound != -1)
        //    {
        //        KLdebug.Print(" Trovati Path contenenti il centroid della RE corrente: ", nameFile);

        //        var listOfPathOfCentroidsToSplit = listOfPathOfCentroids.FindAll(
        //            myPathOfCentroids => myPathOfCentroids.path.FindIndex(ind => ind == indOfThisCentroid) != -1);
        //        KLdebug.Print(" Numero di path trovati: " + listOfPathOfCentroidsToSplit.Count, nameFile);
        //        KLdebug.Print(" ", nameFile);

        //        foreach (var myPathOfCentroids in listOfPathOfCentroidsToSplit)
        //        {
        //            KLdebug.Print(" -considero il " + listOfPathOfCentroidsToSplit.IndexOf(myPathOfCentroids) + "path:", nameFile);
        //            for (int i = 0; i < myPathOfCentroids.path.Count; i++)
        //            {
        //                KLdebug.Print("      " + myPathOfCentroids.path[i], nameFile);
        //            }

        //            var indInPathOfThisCentroid = myPathOfCentroids.path.IndexOf(indOfThisCentroid);
        //            KLdebug.Print("  in questo path il centroid di Current RE è al posto " + indInPathOfThisCentroid, nameFile);

        //            var firstPartOfThePath = myPathOfCentroids.path.GetRange(0, indInPathOfThisCentroid);
        //            var firstPartOfThePathLength = firstPartOfThePath.Count;
        //            KLdebug.Print("Spezzo il path in un 1° path di lunghezza " + firstPartOfThePathLength, nameFile);

        //            if (firstPartOfThePathLength > 2)
        //            {
        //                var newSonPath = new MyPathOfPoints(firstPartOfThePath,
        //                    myPathOfCentroids.pathGeometricObject);
        //                listOfPathOfCentroids.Add(newSonPath);
        //                KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
        //                for (int i = 0; i < newSonPath.path.Count; i++)
        //                {
        //                    KLdebug.Print("      " + newSonPath.path[i], nameFile);
        //                }
        //            }
        //            else
        //            {
        //                KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);

        //            }

        //            var secondPartOfThePath = myPathOfCentroids.path.GetRange(indInPathOfThisCentroid + 1,
        //                myPathOfCentroids.path.Count - (indInPathOfThisCentroid + 1));
        //            var secondPartOfThePathLength = secondPartOfThePath.Count;
        //            KLdebug.Print("Spezzo il path in un 2° path di lunghezza " + secondPartOfThePathLength, nameFile);

        //            if (secondPartOfThePathLength > 2)
        //            {
        //                var newSonPath = new MyPathOfPoints(secondPartOfThePath,
        //                    myPathOfCentroids.pathGeometricObject);
        //                listOfPathOfCentroids.Add(newSonPath);
        //                KLdebug.Print("è sufficientemente lungo: lo aggiungo di nuovo alla lista path da esaminare. Eccolo:", nameFile);
        //                for (int i = 0; i < newSonPath.path.Count; i++)
        //                {
        //                    KLdebug.Print("      " + newSonPath.path[i], nameFile);
        //                }
        //            }
        //            else
        //            {
        //                KLdebug.Print("Non è sufficientemente lungo: non lo aggiungo alla lista path da esaminare", nameFile);
        //            }

        //            listOfPathOfCentroids.Remove(myPathOfCentroids);
        //            KLdebug.Print("RIMOSSO il Path spezzato in due.", nameFile);
        //            KLdebug.Print(" ", nameFile);

        //        }
        //        listOfPathOfCentroids = listOfPathOfCentroids.OrderByDescending(x => x.path.Count).ThenBy(y => y.pathGeometricObject.GetType() == typeof(MyLine) ? 0 : 1).ToList();
        //        KLdebug.Print("Riordino la lista listOfPathOfCentroids ancora da geometricamente verificare.", nameFile);
        //    }
        //    else
        //    {
        //        KLdebug.Print("Non ho trovato nessun path che intersechi la RE " + indOfThisCentroid, nameFile);
        //    }

        //    KLdebug.Print(" ", nameFile);

        //}

    }
}

