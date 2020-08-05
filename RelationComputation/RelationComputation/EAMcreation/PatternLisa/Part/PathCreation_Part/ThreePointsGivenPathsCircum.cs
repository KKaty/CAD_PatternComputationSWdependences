using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{

    public partial class Functions
    {
        //Crea il massimo path CIRCONFERENZA a partire dai 3 punti dati
        public static List<int> ThreePointsGivenPathsCircum(MyMatrAdj MatrAdjToSee,
            List<MyVertex> ListCentroid, List<int> ListOfExtremePoints, int Point1,
            int Point2, int Point3, ref StringBuilder fileOutput, ref bool ToleranceOk,
            out MyPathGeometricObject pathCurve, ModelDoc2 swModel = null, SldWorks swApplication = null)
        {
            List<int> Path = new List<int>();

            Path.Add(Point1);
            Path.Add(Point2);
            Path.Add(Point3);

            MyCircumForPath CircumPath = FunctionsLC.CircumPassingThrough(
                ListCentroid[Point1], ListCentroid[Point2], ListCentroid[Point3], ref fileOutput, swModel, swApplication);

             //procedo nella direzione Point2-Point3
                fileOutput.AppendLine("Point3: " + Point3);
                List<int> BranchesThird = MatrAdjToSee.matr.GetRow(Point3).Find(entry => entry == 1).ToList();
                //cerco tra questi
                BranchesThird.Remove(Point2);
                //foreach (int ind in BranchesThird)
                //{
                //    fileOutput.AppendLine("\n branch del 3° punto " + Point3 + ": " + ind);
                //}

                int Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                while (Next != -1 && BranchesThird[Next] != Point1)
                    // mi devo fermare anche quando completo la circonferenza, se non va all'infinito
                {
                    // Check if the tolerance is too rough
                    int NumOfFound =
                        BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath)).Count;
                    if (NumOfFound == 1)
                    {
                        //List<int> listafind = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                        //foreach (int ind in listafind)
                        //{
                        //    fileOutput.AppendLine("Branch candidato a proseguire il path (dovrebbe essere solo uno!): " + ind);
                        //}
                        //fileOutput.AppendLine("Next: " + Next);

                        int NextInd = BranchesThird[Next];
                        //fileOutput.AppendLine("Nextind: " + NextInd);
                        BranchesThird.Clear();
                        BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList();
                        //cerco tra questi
                        BranchesThird.Remove(Path.Last());
                        //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in coda), perché se no tornerei indietro
                        Path.Add(NextInd); //aggiungo in coda al path
                        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                    }
                    else
                    {
                        fileOutput.AppendLine("NumOfFound > 1  !!!!");
                        //End the execution in the higher levels
                        ToleranceOk = false;
                        pathCurve = null;
                        return Path;
                    }
                }

                #region Check if I can invert the direction of expansion on not

                //se non ho ancora completato il giro (Next=-1) e Point1 non era un Extreme point, inverto la direzione procedendo verso Point2-Point1
                if (Next == -1)
                {
                    if (!(ListOfExtremePoints.Contains(Point1)))
                    {
                        fileOutput.AppendLine("non ho ancora completato il giro");

                        BranchesThird.Clear();
                        BranchesThird = MatrAdjToSee.matr.GetRow(Point1).Find(entry => entry == 1).ToList();
                        //cerco tra questi
                        BranchesThird.Remove(Point2);
                        Next = BranchesThird.FindIndex(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                        while (Next != -1)
                            // qui sono sicura che che non completerò mai la crf, se no l'avrebbe già completata nell'altro while
                        {
                            // Check if the tolerance is too rough
                            int NumOfFound =
                                BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath))
                                    .Count;
                            if (NumOfFound == 1)
                            {
                                //List<int> listafind = BranchesThird.FindAll(ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                                //foreach (int ind in listafind)
                                //{
                                //    fileOutput.AppendLine("Branch candidato a proseguire il path (dovrebbe essere solo uno!): " + ind);
                                //}
                                //fileOutput.AppendLine("Next: " + Next);
                                int NextInd = BranchesThird[Next];
                                //fileOutput.AppendLine("Nextind: " + NextInd);
                                Console.ReadLine();

                                BranchesThird.Clear();
                                BranchesThird = MatrAdjToSee.matr.GetRow(NextInd).Find(entry => entry == 1).ToList();
                                //cerco tra questi
                                BranchesThird.Remove(Path[0]);
                                //rimuovo dai branch trovati l'ultimo elemento che avevo aggiunto al path (in testa), perché se no tornerei indietro
                                Path.Insert(0, NextInd); //aggiungo in testa al path
                                Next =
                                    BranchesThird.FindIndex(
                                        ind_branch => ListCentroid[ind_branch].Lieoncircum(CircumPath));
                            }
                            else
                            {
                                fileOutput.AppendLine("NumOfFound > 1  !!!!  ERRORE. ");
                                //End the execution in the higher levels
                                ToleranceOk = false;
                                pathCurve = null;
                                return Path;
                            }
                        }
                    }
                }
                else //si è usciti dal 1° while perché BranchesThird[Next] == Point1, cioè ho completato il giro
                {
                    Path.Add(BranchesThird[Next]); //aggiungo Point1 al Path (faccio un ciclo chiuso)
                }

                #endregion


                fileOutput.AppendLine("\n Nuovo path circonferenza:");
                foreach (int ind in Path)
                {
                    fileOutput.AppendLine(" - " + ind);
                }
                pathCurve = CircumPath;


            if (CircumPath != null)
            {
                return Path;
            }
            Path.Clear();
            return Path;
        } //fine ThreePointsGivenPathsCircum
    }
}