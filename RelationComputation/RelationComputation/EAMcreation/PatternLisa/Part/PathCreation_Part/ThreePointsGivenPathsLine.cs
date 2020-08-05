using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
   
    public partial class Functions
    {
        //Crea il massimo path LINEA a partire dai 3 punti dati
        public static List<int> ThreePointsGivenPathsLine(MyMatrAdj MatrAdjToSee, List<MyVertex> ListCentroid, 
            List<int> ListOfExtremePoints, int Point1, int Point2, int Point3, ref StringBuilder fileOutput,
            ref bool ToleranceOk, out MyPathGeometricObject pathCurve)
        {
            #region VERSION 1: the original, it is ok
            //var nameFile = "ContaElementiPath.txt";

            //List<int> Path = new List<int>();

            //Path.Add(Point1);
            //Path.Add(Point2);
            //Path.Add(Point3);
            //KLdebug.Print("point1 = " + Point1, nameFile);
            //KLdebug.Print("point2 = " + Point2, nameFile);
            //KLdebug.Print("point1 = " + Point3, nameFile);


            //MyLine linePath = FunctionsLC.LinePassingThrough(ListCentroid[Point2], ListCentroid[Point3]);

            ////procedo nella direzione Point2-Point3

            //List<int> BranchesThird = MatrAdjToSee.matr.GetRow(Point3).Find(entry => entry == 1).ToList(); //cerco tra questi

            //BranchesThird.Remove(Point2);
            //KLdebug.Print("Num of elementi in BranchesThird (1^ volta) = " + BranchesThird.Count, nameFile);

            //foreach (int ind in BranchesThird)
            //{
            //    KLdebug.Print("- " + ind, nameFile);
            //}

            //int Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));                        
            //while (Next != -1)
            //{
            //    // Check if the tolerance is too rough
            //    int NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
            //    if (NumOfFound == 1)
            //    {
            //        int NextInd = BranchesThird[Next];
            //        BranchesThird.Clear();
            //        BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
            //        KLdebug.Print("Num of elementi in BranchesThird = " + BranchesThird.Count, nameFile);

            //        foreach (int ind in BranchesThird)
            //        {
            //            KLdebug.Print("- " + ind, nameFile);
            //        }

            //        BranchesThird.Remove(Path[Path.Count - 1]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in coda), perché se no tornerei indietro
            //        KLdebug.Print("Path.count = " + Path.Count, nameFile);
            //        Path.Add(NextInd); //aggiungo in coda al path
            //        KLdebug.Print("Aggiunto al path: - " + NextInd, nameFile);
            //        KLdebug.Print("Path.count dopo aggiunta = " + Path.Count, nameFile);
            //        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
            //        KLdebug.Print(" ", nameFile);

            //    }
            //    else
            //    {
            //        //End the execution in the higher levels
            //        ToleranceOk = false;
            //        pathCurve = null;
            //        return Path;
            //    }
                
            //}

            //#region Check if I can invert the direction of expansion on not
            ////se Point1 non era un Extreme point, inverto la direzione procedendo verso Point2-Point1
            //if (!(ListOfExtremePoints.Contains(Point1)))
            //{
            //    BranchesThird.Clear();
            //    BranchesThird = MatrAdjToSee.matr.GetRow(Point1).Find(entry => entry == 1).ToList(); //cerco tra questi                
            //    BranchesThird.Remove(Point2);
            //    Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
            //    //uso FindIndex perché mi restituisce -1 se non trova
            //    while (Next != -1)
            //    {
            //        // Check if the tolerance is too rough
            //        int NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
            //        if (NumOfFound == 1)
            //        {
            //            int NextInd = BranchesThird[Next];
            //            BranchesThird.Clear();
            //            BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
            //            BranchesThird.Remove(Path[0]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in testa), perché se no tornerei indietro
            //            Path.Insert(0, NextInd);  //aggiungo in testa al path
            //            Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
            //        }
            //        else
            //        {
            //            //End the execution in the higher levels
            //            ToleranceOk = false;
            //            pathCurve = null;
            //            return Path;
            //        }

            //    }
            //}
            //#endregion

            //fileOutput.AppendLine("\n Nuovo path retta:");
            //foreach (int ind in Path)
            //{
            //    fileOutput.AppendLine(" - " + ind);
            //}
            //pathCurve = linePath;
            //return Path;    
            #endregion

            #region VERSION 2: ok but not very compact
            /*   var nameFile = "ContaElementiPath.txt";

            List<int> Path = new List<int>();

            Path.Add(Point1);
            Path.Add(Point2);
            Path.Add(Point3);
            KLdebug.Print("point1 = " + Point1, nameFile);
            KLdebug.Print("point2 = " + Point2, nameFile);
            KLdebug.Print("point1 = " + Point3, nameFile);

            KLdebug.Print("Matr distance = " + MatrAdjToSee.d, nameFile);
            KLdebug.Print("Matrice dimension = " + MatrAdjToSee.matr.GetLength(0), nameFile);
            var line = string.Format(
                              "Matrice :\n" +
                              "{0} {1} {2} {3}\n" +
                              "{4} {5} {6} {7}\n" +
                              "{8} {9} {10} {11}\n"+
                              "{12} {13} {14} {15}\n\n",

                              MatrAdjToSee.matr[0, 0], MatrAdjToSee.matr[0, 1], MatrAdjToSee.matr[0, 2], MatrAdjToSee.matr[0, 3],
                              MatrAdjToSee.matr[1, 0], MatrAdjToSee.matr[1, 1], MatrAdjToSee.matr[1, 2], MatrAdjToSee.matr[1, 3],
                              MatrAdjToSee.matr[2, 0], MatrAdjToSee.matr[2, 1], MatrAdjToSee.matr[2, 2], MatrAdjToSee.matr[2, 3],
                              MatrAdjToSee.matr[3, 0], MatrAdjToSee.matr[3, 1], MatrAdjToSee.matr[3, 2], MatrAdjToSee.matr[3, 3]
                             );
            KLdebug.Print(line, nameFile);

            MyLine linePath = FunctionsLC.LinePassingThrough(ListCentroid[Point2], ListCentroid[Point3]);
            int NextInd;

            //procedo nella direzione Point2-Point3
            List<int> BranchesThird = MatrAdjToSee.matr.GetRow(Point3).Find(entry => entry == 1).ToList(); //cerco tra questi
            BranchesThird.Remove(Point2);

            KLdebug.Print("Branches di " + Point3  + " :", nameFile);
            foreach (int ind in BranchesThird)
            {
                KLdebug.Print("- " + ind, nameFile);
            }

            int Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
            int NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;

            if (NumOfFound == 1)
            {
                NextInd = BranchesThird[Next];
                Path.Add(NextInd); //aggiungo in coda al path
                KLdebug.Print("Aggiunto al path (1^ volta) : - " + NextInd, nameFile);
                KLdebug.Print("Path dopo aggiunta: ", nameFile);
                foreach (int ind in Path)
                {
                    KLdebug.Print("- " + ind, nameFile);
                }

                BranchesThird.Clear();
                BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
                BranchesThird.Remove(Path[Path.Count - 2]);
                KLdebug.Print("Rimosso dai branch (1^ volta) : - " + Path[Path.Count - 2], nameFile);

                KLdebug.Print("Branches (1^ volta)" + NextInd + " :", nameFile);
                foreach (int ind in BranchesThird)
                {
                    KLdebug.Print("- " + ind, nameFile);
                }
                Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                KLdebug.Print(" ", nameFile);


                while (Next != -1)
                {
                    KLdebug.Print("(Entro nel while)", nameFile);
                    // Check if the tolerance is too rough
                    NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
                    if (NumOfFound == 1)
                    {
                        NextInd = BranchesThird[Next];
                        BranchesThird.Clear();
                        BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
                        BranchesThird.Remove(Path[Path.Count - 1]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in coda), perché se no tornerei indietro
                        KLdebug.Print("Rimosso dai branch: - " + Path[Path.Count - 2], nameFile);

                        KLdebug.Print("Branches di " + NextInd + " :", nameFile);
                        foreach (int ind in BranchesThird)
                        {
                            KLdebug.Print("- " + ind, nameFile);
                        }

                        Path.Add(NextInd); //aggiungo in coda al path
                        KLdebug.Print("Aggiunto al path: - " + NextInd, nameFile);
                        KLdebug.Print("Path dopo aggiunta: " + Path.Count, nameFile);
                        foreach (int ind in Path)
                        {
                            KLdebug.Print("- " + ind, nameFile);
                        }
                        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                        KLdebug.Print(" ", nameFile);

                    }
                    else
                    {
                        //End the execution in the higher levels
                        ToleranceOk = false;
                        pathCurve = null;
                        return Path;
                    }
         

                }
            }

            #region Check if I can invert the direction of expansion on not
            //se Point1 non era un Extreme point, inverto la direzione procedendo verso Point2-Point1
            if (!(ListOfExtremePoints.Contains(Point1)))
            {
                BranchesThird.Clear();
                BranchesThird = MatrAdjToSee.matr.GetRow(Point1).Find(entry => entry == 1).ToList(); //cerco tra questi                
                BranchesThird.Remove(Point2);
                Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                //uso FindIndex perché mi restituisce -1 se non trova
                while (Next != -1)
                {
                    // Check if the tolerance is too rough
                    NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
                    if (NumOfFound == 1)
                    {
                        NextInd = BranchesThird[Next];
                        BranchesThird.Clear();
                        BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
                        BranchesThird.Remove(Path[0]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in testa), perché se no tornerei indietro
                        Path.Insert(0, NextInd);  //aggiungo in testa al path
                        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                    }
                    else
                    {
                        //End the execution in the higher levels
                        ToleranceOk = false;
                        pathCurve = null;
                        return Path;
                    }

                }
            }
            #endregion

            fileOutput.AppendLine("\n Nuovo path retta:");
            foreach (int ind in Path)
            {
                fileOutput.AppendLine(" - " + ind);
            }
            pathCurve = linePath;
            return Path;     
         */
            #endregion

            //VERSION 3: Ok and more compact

            List<int> Path = new List<int>();

            Path.Add(Point1);
            Path.Add(Point2);
            Path.Add(Point3);

            MyLine linePath = FunctionsLC.LinePassingThrough(ListCentroid[Point2], ListCentroid[Point3]);

            //procedo nella direzione Point2-Point3
            List<int> BranchesThird = MatrAdjToSee.matr.GetRow(Point3).Find(entry => entry == 1).ToList(); //cerco tra questi
            BranchesThird.Remove(Point2);

            int Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
            while (Next != -1)
            {
                // Check if the tolerance is too rough
                int NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
                if (NumOfFound == 1)
                {
                    int NextInd = BranchesThird[Next];
                    Path.Add(NextInd); //aggiungo in coda al path
                    
                    BranchesThird.Clear();
                    BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
                    
                    BranchesThird.Remove(Path[Path.Count - 2]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in coda), perché se no tornerei indietro
                    Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                }
                else
                {
                    //End the execution in the higher levels
                    ToleranceOk = false;
                    pathCurve = null;
                    return Path;
                }

            }

            #region Check if I can invert the direction of expansion on not
            //se Point1 non era un Extreme point, inverto la direzione procedendo verso Point2-Point1
            if (!(ListOfExtremePoints.Contains(Point1)))
            {
                BranchesThird.Clear();
                BranchesThird = MatrAdjToSee.matr.GetRow(Point1).Find(entry => entry == 1).ToList(); //cerco tra questi                
                BranchesThird.Remove(Point2);
                Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                //uso FindIndex perché mi restituisce -1 se non trova
                while (Next != -1)
                {
                    // Check if the tolerance is too rough
                    int NumOfFound = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieonline(linePath)).Count;
                    if (NumOfFound == 1)
                    {
                        int NextInd = BranchesThird[Next];
                        BranchesThird.Clear();
                        BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList(); //cerco tra questi
                        BranchesThird.Remove(Path[0]); //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in testa), perché se no tornerei indietro
                        Path.Insert(0, NextInd);  //aggiungo in testa al path
                        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieonline(linePath));
                    }
                    else
                    {
                        //End the execution in the higher levels
                        ToleranceOk = false;
                        pathCurve = null;
                        return Path;
                    }

                }
            }
            #endregion

            fileOutput.AppendLine("\n Nuovo path retta:");
            foreach (int ind in Path)
            {
                fileOutput.AppendLine(" - " + ind);
            }
            pathCurve = linePath;
            return Path;    
        }//fine ThreePointsGivenPathsLine
  
    } 
} //fine Namespace Inizio

