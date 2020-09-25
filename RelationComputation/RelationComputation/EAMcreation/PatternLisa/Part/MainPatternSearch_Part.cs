using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using AssemblyRetrieval.PatternLisa.Part.PartUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part
{
    public partial class GeometryAnalysis
    {
        public static void MainPatternSearch_Part(bool EntirePart, ref List<MyGroupingSurface> listOfMyGroupingSurface,                 
            List<MyGroupingSurface> listOfInitialGroupingSurface, ref StringBuilder fileOutput, SldWorks mySwApplication)
        {
            var listOfOutputPattern = new List<MyPattern>();     //list of output patterns found
            var listOfOutputPatternTwo = new List<MyPattern>();  //list of output patterns of length 2 found
            var toleranceOK = true;                          //it is TRUE if the tolerance level is OK during the paths searching, FALSE otherwise

                while (listOfMyGroupingSurface.Count > 0 && toleranceOK == true)
                {
                    MyGroupingSurface currentGroupingSurface;
                    if (!EntirePart)
                    {
                    currentGroupingSurface =
                        new MyGroupingSurface(listOfMyGroupingSurface[0].groupingSurface,
                            listOfMyGroupingSurface[0].listOfREOfGS);
                    }
                    else
                    {
                        currentGroupingSurface = new MyGroupingSurface(listOfMyGroupingSurface[0].KLplanareSurface,
                            listOfMyGroupingSurface[0].listOfREOfGS);
                    }

                    listOfMyGroupingSurface.RemoveAt(0);
                    fileOutput.AppendLine(" ");
                    fileOutput.AppendLine("(Al momento sono rimaste " + listOfMyGroupingSurface.Count +
                                          " superfici, compresa questa.)");

                    PartUtilities.GeometryAnalysis.FindPatternsInGS(currentGroupingSurface, ref fileOutput,
                        ref listOfOutputPattern, ref listOfMyGroupingSurface,
                        ref listOfOutputPatternTwo, ref toleranceOK, listOfInitialGroupingSurface);

                }
            
            //Assigning of id numbers to the found MyPattern
            //(the id will be used in composed pattern search phase)
            //At the same time, we draw the pattern centroids
            ModelDoc2 SwModel = mySwApplication.ActiveDoc;
            SwModel.ClearSelection2(true);
            SwModel.Insert3DSketch();
            var i = 0;

            foreach (var pattern in listOfOutputPattern)
            {
                pattern.idMyPattern = i;
                i++;
                var listOfCentroidsOfPattern = pattern.listOfMyREOfMyPattern.Select(re => re.centroid).ToList();
                var patternCentroid = ExtractInfoFromBRep.computeCentroidsOfVertices(listOfCentroidsOfPattern);
                pattern.patternCentroid = patternCentroid;
                //SwModel.CreatePoint2(pattern.patternCentroid.x, pattern.patternCentroid.y, pattern.patternCentroid.z);
            }
            foreach (var pattern in listOfOutputPatternTwo)
            {
                pattern.idMyPattern = i;
                i++;
                var listOfCentroidsOfPattern = pattern.listOfMyREOfMyPattern.Select(re => re.centroid).ToList();
                var patternCentroid = ExtractInfoFromBRep.computeCentroidsOfVertices(listOfCentroidsOfPattern);
                pattern.patternCentroid = patternCentroid;
                //SwModel.CreatePoint2(pattern.patternCentroid.x, pattern.patternCentroid.y, pattern.patternCentroid.z);
            }
            //SwModel.InsertSketch();

            //Patterns are subdivided in Line patterns and in Circle patterns:
            var listOfOutputPatternLine = new List<MyPattern>();
            var listOfOutputPatternCircum = new List<MyPattern>();
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

            //The results are shown giving color to the model:
            ColorFace.MyVisualOutput(listOfOutputPatternLine, mySwApplication);
            ColorFace.MyVisualOutput(listOfOutputPatternCircum, mySwApplication);

            //The same for patterns of length 2:
       
            ColorFace.MyVisualOutput(listOfOutputPatternTwo, mySwApplication);

            //Build the list of MyGroupingSurfaceForPatterns:
            var listOfGroupingSurfaceForPatterns = new List<MyGroupingSurfaceForPatterns>();
            if (!EntirePart)
            {
                foreach (var gs in listOfInitialGroupingSurface)
                {
                    var listOfPatternsLineForThisGS = listOfOutputPatternLine.FindAll(
                        pattern => pattern.listOfGroupingSurfaces.FindIndex(
                            surface => ExtractInfoFromBRep.MyEqualsSurface(surface, gs.groupingSurface, mySwApplication)) !=
                                   -1);
                    var listOfPatternsTwoForThisGS = listOfOutputPatternTwo.FindAll(
                        pattern => pattern.listOfGroupingSurfaces.FindIndex(
                            surface => ExtractInfoFromBRep.MyEqualsSurface(surface, gs.groupingSurface, mySwApplication)) !=
                                   -1);
                    listOfPatternsLineForThisGS.AddRange(listOfPatternsTwoForThisGS);

                    var listOfPatternsCircumForThisGS = listOfOutputPatternCircum.FindAll(
                        pattern => pattern.listOfGroupingSurfaces.FindIndex(
                            surface => ExtractInfoFromBRep.MyEqualsSurface(surface, gs.groupingSurface, mySwApplication)) !=
                                   -1);
                    if (listOfPatternsLineForThisGS.Count > 1 || listOfPatternsCircumForThisGS.Count > 1)
                    {
                        var newGSForPatterns = new MyGroupingSurfaceForPatterns(gs.groupingSurface,
                            listOfPatternsLineForThisGS, listOfPatternsCircumForThisGS);
                        listOfGroupingSurfaceForPatterns.Add(newGSForPatterns);
                    }
                }
            }
            
            
            //>>>>>>>COMPOSED PATTERN SEARCH:
            List<MyComposedPattern> listOfOutputComposedPattern;
            List<MyComposedPattern> listOfOutputComposedPatternTwo;

            PartUtilities_ComposedPatterns.GeometryAnalysis.FindComposedPatterns(listOfGroupingSurfaceForPatterns, out listOfOutputComposedPattern,
                out listOfOutputComposedPatternTwo, SwModel, mySwApplication, ref fileOutput);


            ColorFace.MyVisualOutput_ComposedPatterns(listOfOutputComposedPattern,
            listOfOutputComposedPatternTwo, mySwApplication, SwModel);
        }

        /****************************************************************************************************/
        /*    Parte aggiunta per permettere il riconoscimento di ripetizioni con la stessa origine (Katia)  */
        /****************************************************************************************************/

        public static void GetRepeatedEntities(List<List<object>> RepeatedEntities, bool EntirePart,
            List<MyGroupingSurface> listOfInitialGroupingSurface, SldWorks swApp)
        {

            const string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
            string whatToWrite = "";

            var listOfMyRepeatedEntity = new List<MyRepeatedEntity>();
            var listOfGroupingSurfaces = new List<MyGroupingSurface>();

            if (!EntirePart)
            {
                foreach (List<object> selectedEntities in RepeatedEntities)
                {
                    //whatToWrite = string.Format("NEW SELECTED ENTITY: ");
                    //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                    var idNewRepeatedEntity = RepeatedEntities.IndexOf(selectedEntities);
                    MyRepeatedEntity newRepeatedEntity = ExtractInfoFromBRep.BuildRepeatedEntity(
                        selectedEntities, idNewRepeatedEntity, swApp);


                    ExtractInfoFromBRep.GetSurfacesOfFacesAdjacentToSetOfFaces(
                        newRepeatedEntity, ref listOfGroupingSurfaces, swApp);

                    //GeometryAnalysis.AddFacesInformationToMyRE(newRepeatedEntity, SwApplication);
                    listOfMyRepeatedEntity.Add(newRepeatedEntity);
                }
            }
            else
            {
                
                /*
                 // Parte aggiunta da Katia per il conto delle parti ripetute.
                if (RepeatedEntities.Count >= 3)
                {
                    foreach (var entity in RepeatedEntities)
                    {
                        var faceEntity = new List<Face2>();
                        foreach (var obj in entity)
                        {
                            var face = (Face2)obj;
                            faceEntity.Add(face);
                        }

                        //var colorParam = 1 / (RepeatedEntities.IndexOf(entity) + 0.01);
                        //ColorFace.KLColorFace(faceEntity, swApp, colorParam);

                        MyRepeatedEntity repeatedEntity = ExtractInfoFromBRep.BuildRepeatedEntity(
                            entity, RepeatedEntities.IndexOf(entity));
                        listOfMyRepeatedEntity.Add(repeatedEntity);
            
                        //var print = string.Format("Baricentro calcolato per entità {0}: {1}, {2}, {3}", RepeatedEntities.IndexOf(entity), repeatedEntity1.centroid.x,
                          //  repeatedEntity1.centroid.y, repeatedEntity1.centroid.z);
                        //swApp.SendMsgToUser(print);
                    }
                        var firstCentroid = listOfMyRepeatedEntity[0].centroid;
                        var secondCentroid = listOfMyRepeatedEntity[1].centroid;
                        var thirdCentroid = listOfMyRepeatedEntity[2].centroid;

                        var groupingPlane = FunctionsLC.PlanePassingThrough(firstCentroid, secondCentroid, thirdCentroid);
                        var groupingSurface = new MyGroupingSurface(groupingPlane, listOfMyRepeatedEntity);
                        listOfGroupingSurfaces.Add(groupingSurface);
                    
                    string whatToWritekl =
                        string.Format(
                            "Numero delle grouping surface create da katia: {0} \nnumero entità ripetute: {1}",
                            listOfGroupingSurfaces.Count, RepeatedEntities.Count);
                    KLdebug.Print(whatToWritekl, fileNameBuildRepeatedEntity);
                    KLdebug.Print(" ", fileNameBuildRepeatedEntity);
                   
                }
                 * */
            }


            listOfInitialGroupingSurface = listOfGroupingSurfaces;

            //SwModel = (ModelDoc2)SwApplication.ActiveDoc;
            //SwModel.ClearSelection2(true);
            //SwModel.Insert3DSketch();


            //foreach (var re in listOfMyRepeatedEntity)
            //{
            //    SwModel.CreatePoint2(re.centroid.x, re.centroid.y, re.centroid.z);
            //}
            //SwModel.InsertSketch();



            #region stampa delle MyGroupingSurface trovate nelle varie MyRepeatedEntity

            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            //KLdebug.Print("LISTA DELLE GROUPING SURFACE TROVATE:", fileNameBuildRepeatedEntity);
            //string whatToWrite1 = string.Format("Numero delle grouping surface trovate: " + listOfGroupingSurfaces.Count);
            //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            foreach (var myGroupingSurface in listOfGroupingSurfaces)
            {
                MyPlane firstPlane1;

                if (!myGroupingSurface.EntirePartAsRE)
                {
                    var thisSurface = myGroupingSurface.groupingSurface;

                    var firstPlaneParameters1 = thisSurface.PlaneParams;
                    double[] firstPlanePoint1 =
                    {
                        firstPlaneParameters1[3], firstPlaneParameters1[4],
                        firstPlaneParameters1[5]
                    };
                    double[] firstPlaneNormal1 =
                    {
                        firstPlaneParameters1[0], firstPlaneParameters1[1],
                        firstPlaneParameters1[2]
                    };

                    //  QUAL ERA PIù IL PROBLEMA DELLE NORMALI DELLE FACCE??????
                    //  Forse vanno aggiustate le normali come ho fatto per le superfici prese da altre facce!
                    //  (vedi esempio quaderno, da' normale sbagliata di una dell superfici trovate)
                    firstPlane1 = new MyPlane(firstPlaneNormal1, firstPlanePoint1);

                }
                else
                {
                    firstPlane1 = new MyPlane(myGroupingSurface.KLplanareSurface.a, myGroupingSurface.KLplanareSurface.b,
                        myGroupingSurface.KLplanareSurface.c, myGroupingSurface.KLplanareSurface.d);
                }

                //KLdebug.Print("Superficie:", fileNameBuildRepeatedEntity);
                //whatToWrite1 = string.Format("a {0}, b {1}, c {2}, d {3}", firstPlane1.a, firstPlane1.b, firstPlane1.c,
                //    firstPlane1.d);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //whatToWrite1 =
                //    string.Format("Numero di MyRepeatedEntity adiacenti: " + myGroupingSurface.listOfREOfGS.Count);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            }

            #endregion

        

        //I remove all the MyGroupingSurface with only one occurrence
            listOfGroupingSurfaces.RemoveAll(groupingSurface => groupingSurface.listOfREOfGS.Count == 1);
            #region stampa delle MyGroupingSurface dopo la rimozione di quelle con solo una occorrenza

            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            //KLdebug.Print("LISTA DELLE GROUPING SURFACE DOPO LA RIMOZIONE DI QUELLE CON SOLO UNA 1 OCCORRENZA:", fileNameBuildRepeatedEntity);
            //whatToWrite1 = string.Format("Numero delle grouping surface trovate: " + listOfGroupingSurfaces.Count);
            //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            foreach (var myGroupingSurface in listOfGroupingSurfaces)
            {
                MyPlane firstPlane1;
                if (!EntirePart)
                {
                    var thisSurface = myGroupingSurface.groupingSurface;

                    var firstPlaneParameters1 = thisSurface.PlaneParams;
                    double[] firstPlanePoint1 =
                    {
                        firstPlaneParameters1[3], firstPlaneParameters1[4],
                        firstPlaneParameters1[5]
                    };
                    double[] firstPlaneNormal1 =
                    {
                        firstPlaneParameters1[0], firstPlaneParameters1[1],
                        firstPlaneParameters1[2]
                    };

                    firstPlane1 = new MyPlane(firstPlaneNormal1, firstPlanePoint1);

                }
                else
                {
                    firstPlane1 = myGroupingSurface.KLplanareSurface;
                }


                //KLdebug.Print("Superficie:", fileNameBuildRepeatedEntity);
                //whatToWrite1 = string.Format("a {0}, b {1}, c {2}, d {3}", firstPlane1.a, firstPlane1.b, firstPlane1.c, firstPlane1.d);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //whatToWrite1 = string.Format("Numero di MyRepeatedEntity adiacenti: " + myGroupingSurface.listOfREOfGS.Count);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            }

            #endregion

            //I order the list by decreasing ordering respect to number of elements of listOfREOfGS
            listOfGroupingSurfaces = listOfGroupingSurfaces.OrderByDescending(
                groupingSurface => groupingSurface.listOfREOfGS.Count).ToList();
            #region stampa delle MyGroupingSurface dopo il riordino per numero di occorrenze

            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            //KLdebug.Print("LISTA DELLE GROUPING SURFACE DOPO IL RIORDINO:", fileNameBuildRepeatedEntity);
            //whatToWrite1 = string.Format("Numero delle grouping surface trovate: " + listOfGroupingSurfaces.Count);
            //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
            //KLdebug.Print(" ", fileNameBuildRepeatedEntity);

            foreach (var myGroupingSurface in listOfGroupingSurfaces)
            {
                MyPlane firstPlane1;
                if (!EntirePart)
                {
                    var thisSurface = myGroupingSurface.groupingSurface;

                    var firstPlaneParameters1 = thisSurface.PlaneParams;
                    double[] firstPlanePoint1 =
                    {
                        firstPlaneParameters1[3], firstPlaneParameters1[4],
                        firstPlaneParameters1[5]
                    };
                    double[] firstPlaneNormal1 =
                    {
                        firstPlaneParameters1[0], firstPlaneParameters1[1],
                        firstPlaneParameters1[2]
                    };

                    firstPlane1 = new MyPlane(firstPlaneNormal1, firstPlanePoint1);

                }
                else
                {
                    firstPlane1 = myGroupingSurface.KLplanareSurface;
                }

                //KLdebug.Print("Superficie:", fileNameBuildRepeatedEntity);
                //whatToWrite1 = string.Format("a {0}, b {1}, c {2}, d {3}", firstPlane1.a, firstPlane1.b, firstPlane1.c, firstPlane1.d);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //whatToWrite1 = string.Format("Numero di MyRepeatedEntity adiacenti: " + myGroupingSurface.listOfREOfGS.Count);
                //KLdebug.Print(whatToWrite1, fileNameBuildRepeatedEntity);
                //KLdebug.Print(" ", "buildRepeatedEntity.txt");

            }

            #endregion

            StringBuilder fileOutput = new StringBuilder();
            fileOutput.AppendLine("RICERCA PATH");


            GeometryAnalysis.MainPatternSearch_Part(true, ref listOfGroupingSurfaces,
                listOfInitialGroupingSurface, ref fileOutput, swApp);
            #region per test
            //var listOfOutputPattern = new List<MyPattern>();     //list of output patterns found
            //var listOfOutputPatternTwo = new List<MyPattern>();  //list of output patterns of length 2 found
            //var toleranceOK = true;                          //it is TRUE if the tolerance level is OK during the paths searching, FALSE otherwise

            //var currentGroupingSurface = new MyGroupingSurface(listOfGroupingSurfaces[0].groupingSurface, listOfGroupingSurfaces[0].listOfREOfGS);
            //listOfGroupingSurfaces.RemoveAt(0);
            //fileOutput.AppendLine(" ");
            //fileOutput.AppendLine("CURRENT SURFACE OF TYPE: " + currentGroupingSurface.groupingSurface.Identity());
            //fileOutput.AppendLine("(Al momento sono rimaste " + listOfGroupingSurfaces.Count + " superfici, compresa questa.)");

            //var ListOfREOnThisSurface =
            //    currentGroupingSurface.listOfREOfGS.Select(myRepeatedEntity => myRepeatedEntity).ToList();
            //var listOfCentroidsThisGS =
            //    currentGroupingSurface.listOfREOfGS.Select(myRepeatedEntity => myRepeatedEntity.centroid).ToList();
            //var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.

            //List<MyMatrAdj> listOfMyMatrAdj = Functions.CreateMatrAdj(listOfCentroidsThisGS, ref fileOutput);
            //fileOutput.AppendLine(" ----> listOfMyMatrAdj.Count = " + listOfMyMatrAdj.Count + ". Ora rimuovo la posizione 0:");

            //var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj[0].d, listOfMyMatrAdj[0].matr, listOfMyMatrAdj[0].nOccur);
            //listOfMyMatrAdj.Remove(listOfMyMatrAdj[0]);
            //fileOutput.AppendLine(" ");
            //fileOutput.AppendLine("Esamino NUOVA MatrAdj. La elimino dalla lista.");
            //fileOutput.AppendLine(" ----> aggiornato listOfMyMatrAdj.Count = " + listOfMyMatrAdj.Count);
            //fileOutput.AppendLine("AVVIO RICERCA PATH in questa MatrAdj!");
            //fileOutput.AppendLine(" ");

            //bool onlyShortPath;

            //var listOfPathOfCentroids = new List<MyPathOfPoints>();
            //maxPath = Functions.FindPaths(currentMatrAdj, ListOfREOnThisSurface, ref fileOutput,
            //    out listOfPathOfCentroids, out onlyShortPath, ref toleranceOK,
            //    ref listOfMyMatrAdj, ref listOfGroupingSurfaces,
            //    ref listOfOutputPattern,
            //    ref listOfOutputPatternTwo);

            //fileOutput.AppendLine(" ");
            //fileOutput.AppendLine("RISULTA DA QUESTA MATRADJ: ");
            //fileOutput.AppendLine("maxPath = " + maxPath);
            //fileOutput.AppendLine("listOfMyPattern.Count = " + listOfOutputPattern.Count);
            //fileOutput.AppendLine("listOfMyPatternTwo.Count = " + listOfOutputPatternTwo.Count);
            //fileOutput.AppendLine("onlyShortPath = " + onlyShortPath);
            //fileOutput.AppendLine("toleranceOK = " + toleranceOK);
            //fileOutput.AppendLine("listOfPathOfCentroids.Count = " + listOfPathOfCentroids.Count);

            ////PATTERN

            //var myPathOfCentroids = listOfPathOfCentroids[0];
            //listOfPathOfCentroids.RemoveAt(0);
            //var listOfREOnThePath = myPathOfCentroids.path.Select(ind => ListOfREOnThisSurface[ind]).ToList();
            //GeometryAnalysis.GetTranslationalPatterns(listOfREOnThePath, myPathOfCentroids.pathGeometricObject,
            //    ref listOfPathOfCentroids, listOfCentroidsThisGS,
            //    ref listOfMyMatrAdj, ref listOfGroupingSurfaces, ref listOfOutputPattern, ref listOfOutputPatternTwo);

            #endregion


            // Create a file to write to. 
            string mydocpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(mydocpath + @"\PathCreation.txt", true))
            {
                outfile.Write(fileOutput.ToString());
                outfile.Close();
            }
        }






    }


}
