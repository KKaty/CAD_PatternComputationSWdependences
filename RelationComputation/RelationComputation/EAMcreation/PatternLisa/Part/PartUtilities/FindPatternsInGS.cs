using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using Functions = AssemblyRetrieval.PatternLisa.Part.PathCreation_Part.Functions;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class GeometryAnalysis
    {

        //PRIMA DI USARE QUESTO METODO RICORDARSI DI ELIMINARE LA CURRENTgROUPINGSURFACE DALLA LISTA!!!!

        public static void FindPatternsInGS(MyGroupingSurface currentGroupingSurface, ref StringBuilder fileOutput,
            ref List<MyPattern> listOfMyPattern, ref List<MyGroupingSurface> listOfMyGroupingSurface,
            ref List<MyPattern> listOfMyPatternTwo, ref bool toleranceOK, List<MyGroupingSurface> listOfInitialGroupingSurface)
        {
            var ListOfREOnThisSurface =
                currentGroupingSurface.listOfREOfGS.Select(myRepeatedEntity => myRepeatedEntity).ToList();
            var listOfCentroidsThisGS =
                currentGroupingSurface.listOfREOfGS.Select(myRepeatedEntity => myRepeatedEntity.centroid).ToList();
            var numOfCentroidsOnThisGS = listOfCentroidsThisGS.Count;
            var maxPath = false; //it is TRUE if the maximum Pattern is found, FALSE otherwise.


            if (numOfCentroidsOnThisGS > 2)
            {
                List<MyMatrAdj> listOfMyMatrAdj = Functions.CreateMatrAdj(listOfCentroidsThisGS, ref fileOutput);


                while (listOfMyMatrAdj.Count > 0 && maxPath == false && toleranceOK == true)
                {
                    bool onlyShortPath;

                    var currentMatrAdj = new MyMatrAdj(listOfMyMatrAdj[0].d, listOfMyMatrAdj[0].matr, listOfMyMatrAdj[0].nOccur);
                    listOfMyMatrAdj.Remove(listOfMyMatrAdj[0]);
                    //NOTA: forse la mia MatrAdj non deve essere rimossa ma conservata,
                    //soprattutto nel caso in cui si presenta onlyShortPath = true
                    //(non avrebbe senso cancellarla, ma conservarla per la ricerca di path
                    //di 2 RE).
                    fileOutput.AppendLine("----> Considero NUOVA MatrAdj. Sono rimaste ancora " + listOfMyMatrAdj.Count +
                                          " MatrAdj da controllare.");
                    fileOutput.AppendLine(" ");

                    List<MyPathOfPoints> listOfPathOfCentroids;
                    maxPath = Functions.FindPaths(currentMatrAdj, ListOfREOnThisSurface, ref fileOutput,
                        out listOfPathOfCentroids, out onlyShortPath, ref toleranceOK,
                        ref listOfMyMatrAdj, ref listOfMyGroupingSurface,
                        listOfInitialGroupingSurface, ref listOfMyPattern, ref listOfMyPatternTwo);
                    fileOutput.AppendLine(" ");
                    fileOutput.AppendLine("PER QUESTA MATRADJ prima della ricerca 'ufficiale' di pattern': ");
                    fileOutput.AppendLine("maxPath = " + maxPath);
                    fileOutput.AppendLine("listOfMyPattern.Count = " + listOfMyPattern.Count);
                    fileOutput.AppendLine("listOfMyPatternTwo.Count = " + listOfMyPatternTwo.Count);
                    fileOutput.AppendLine("onlyShortPath = " + onlyShortPath);
                    fileOutput.AppendLine("toleranceOK = " + toleranceOK);
                    //fileOutput.AppendLine("listOfPathOfCentroids.Count = " + listOfPathOfCentroids.Count);

                    if (toleranceOK == true)
                    {
                        if (listOfPathOfCentroids != null)
                        {
                            if (maxPath == false)
                            {
                                if (onlyShortPath == false)
                                {
                                    GetPatternsFromListOfPaths(listOfPathOfCentroids, ListOfREOnThisSurface,
                                        ref listOfMyMatrAdj, ref listOfMyGroupingSurface, listOfInitialGroupingSurface,
                                        ref listOfMyPattern, ref listOfMyPatternTwo);
                                }
                                else
                                {
                                    //non faccio niente e li rimetto in gioco per 
                                }
                            }
                        }
                        else
                        {
                            fileOutput.AppendLine(" ---> NO PATH FOUND in this adjacency matrix!");
                        }
                    }
                    else
                    {
                        fileOutput.AppendLine("===>>    TOLLERANZA NON SUFFICIENTEMENTE PICCOLA. TERMINATO.");
                        return;
                    }


                }
            }
            else
            {
                //numOfCentroidsOnThisGS = 2
                if (numOfCentroidsOnThisGS == 2)
                {
                    fileOutput.AppendLine("Su QUESTA GS ci sono solo 2 RE: ");

                    if (IsTranslationTwoRE(ListOfREOnThisSurface[0], ListOfREOnThisSurface[1]))
                    {
                        fileOutput.AppendLine("Le due RE sono legate da TRASLAZIONE!");
                        var listOfPathOfCentroids = new List<MyPathOfPoints>();
                        var listOfMyMatrAdj = new List<MyMatrAdj>();
                        var newPatternRE = new List<MyRepeatedEntity>();
                        newPatternRE.Add(ListOfREOnThisSurface[0]);
                        newPatternRE.Add(ListOfREOnThisSurface[1]);
                        var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfCentroidsThisGS[0], listOfCentroidsThisGS[1]);
                        var newPatternType = "TRANSLATION of length 2";
                        var listOfGroupingSurfaceForThisPattern = findGroupingSurfacesForThisPattern(listOfInitialGroupingSurface,
                            newPatternRE, numOfCentroidsOnThisGS);
                        var newPattern = new MyPattern(newPatternRE, newPatternGeomObject, newPatternType, listOfGroupingSurfaceForThisPattern);
                        CheckAndUpdate(newPattern, ref listOfPathOfCentroids,
                            ListOfREOnThisSurface, ref listOfMyMatrAdj, ref listOfMyGroupingSurface,
                            ref listOfMyPattern, ref listOfMyPatternTwo);

                    }
                    else
                    {
                        if (IsReflectionTwoRE(ListOfREOnThisSurface[0], ListOfREOnThisSurface[1], null))
                        {
                            fileOutput.AppendLine("Le due RE sono legate da RIFLESSIONE!");
                            var listOfPathOfCentroids = new List<MyPathOfPoints>();
                            var listOfMyMatrAdj = new List<MyMatrAdj>();
                            var newPatternRE = new List<MyRepeatedEntity>();
                            newPatternRE.Add(ListOfREOnThisSurface[0]);
                            newPatternRE.Add(ListOfREOnThisSurface[1]);
                            var newPatternGeomObject = FunctionsLC.LinePassingThrough(listOfCentroidsThisGS[0], listOfCentroidsThisGS[1]);
                            var newPatternType = "REFLECTION";
                            var listOfGroupingSurfaceForThisPattern = findGroupingSurfacesForThisPattern(listOfInitialGroupingSurface,
                             newPatternRE, numOfCentroidsOnThisGS);
                            var newPattern = new MyPattern(newPatternRE, newPatternGeomObject, newPatternType, listOfGroupingSurfaceForThisPattern);
                            CheckAndUpdate(newPattern, ref listOfPathOfCentroids,
                                ListOfREOnThisSurface, ref listOfMyMatrAdj, ref listOfMyGroupingSurface,
                                ref listOfMyPattern, ref listOfMyPatternTwo);
                        }
                    }
                } 
            }            
        }
    }
}
