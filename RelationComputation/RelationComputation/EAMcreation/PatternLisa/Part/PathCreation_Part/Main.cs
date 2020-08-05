using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    //Main
    class Pluto
    {
        public static void Paperino(SldWorks SwApplication)
        {

            //lista di baricentri
            List<MyVertex> baricentri = new List<MyVertex>();

            #region ESEMPIO 1
            //ESEMPIO 1        

            //Vertex v0 = new Vertex(0, 0, 0);
            //Vertex v1 = new Vertex(0, 3, 0);
            //Vertex v2 = new Vertex(3, 0, 0);
            //Vertex v3 = new Vertex(-3, 0, 0);
            //Vertex v4 = new Vertex(0, -3, 0);
            //baricentri.Add(v0);
            //baricentri.Add(v1);
            //baricentri.Add(v2);
            //baricentri.Add(v3);
            //baricentri.Add(v4);

            //Vertex v5 = new Vertex(3, 3, 0);
            //Vertex v6 = new Vertex(3, 6, 0);
            //Vertex v7 = new Vertex(6, 3, 0);
            //Vertex v8 = new Vertex(9, 3, 0);
            //Vertex v9 = new Vertex(12, 3, 0);
            //Vertex v10 = new Vertex(-3, 3, 0);
            //Vertex v11 = new Vertex(-6, 3, 0);
            //Vertex v12 = new Vertex(12, 6, 0);
            //Vertex v13 = new Vertex(15, 6, 0);
            //Vertex v14 = new Vertex(18, 6, 0);
            //Vertex v15 = new Vertex(18, 3, 0);
            //Vertex v16 = new Vertex(6, -3, 0);
            //Vertex v17 = new Vertex(9, -3, 0);
            //Vertex v18 = new Vertex(12, -3, 0);
            //Vertex v19 = new Vertex(12, -6, 0);
            //baricentri.Add(v5);
            //baricentri.Add(v6);
            //baricentri.Add(v7);
            //baricentri.Add(v8);
            //baricentri.Add(v9);
            //baricentri.Add(v10);
            //baricentri.Add(v11);
            //baricentri.Add(v12);
            //baricentri.Add(v13);
            //baricentri.Add(v14);
            //baricentri.Add(v15);
            //baricentri.Add(v16);
            //baricentri.Add(v17);
            //baricentri.Add(v18);
            //baricentri.Add(v19);
            #endregion

            #region ESEMPIO 3: griglia
            //Esempio griglia:
            //Vertex v0 = new Vertex(0, 0, 0);
            //Vertex v1 = new Vertex(0, 3, 0);
            //Vertex v2 = new Vertex(3, 0, 0);
            //Vertex v3 = new Vertex(3, 3, 0);
            //Vertex v4 = new Vertex(0, -3, 0);
            //Vertex v5 = new Vertex(3, -3, 0);
            //baricentri.Add(v0);
            //baricentri.Add(v1);
            //baricentri.Add(v2);
            //baricentri.Add(v3);
            //baricentri.Add(v4);
            //baricentri.Add(v5);
            //Vertex v6 = new Vertex(6, 3, 0);
            //Vertex v7 = new Vertex(6, 0, 0);
            //Vertex v8 = new Vertex(6, -3, 0);
            //baricentri.Add(v6);
            //baricentri.Add(v7);
            //baricentri.Add(v8);           
            #endregion

            #region ESEMPIO 4: ottagono con penzolante
            //Esempio ottagono:
            //Vertex v0 = new Vertex(1, 0, 1);
            //Vertex v1 = new Vertex(Math.Sqrt(2) / 2, Math.Sqrt(2) / 2, 1);
            //Vertex v2 = new Vertex(0, 1, 1);
            //Vertex v3 = new Vertex(-Math.Sqrt(2) / 2, Math.Sqrt(2) / 2, 1);
            //Vertex v4 = new Vertex(-1, 0, 1);
            //Vertex v5 = new Vertex(-Math.Sqrt(2) / 2, -Math.Sqrt(2) / 2, 1);
            //Vertex v6 = new Vertex(0, -1, 1);
            //Vertex v7 = new Vertex(Math.Sqrt(2) / 2, -Math.Sqrt(2) / 2, 1);
            //Vertex v8 = new Vertex(Math.Sqrt(2) / 2 + v0.distance(v1), -Math.Sqrt(2) / 2, 1);
            //baricentri.Add(v0);
            //baricentri.Add(v1);
            //baricentri.Add(v2);
            //baricentri.Add(v3);
            //baricentri.Add(v4);
            //baricentri.Add(v5);
            //baricentri.Add(v6);
            //baricentri.Add(v7);
            //baricentri.Add(v8);
            #endregion

            #region ESEMPIO 5: triangoli equilateri
            // Esempio triangoli equilateri:
            //Vertex v0 = new Vertex(-3, 0, 0);
            //Vertex v1 = new Vertex(0, 0, 0);
            //Vertex v2 = new Vertex(3, 0, 0);
            //Vertex v3 = new Vertex(-1.5, Math.Sqrt(3) * 3 / 2, 0);
            //Vertex v4 = new Vertex(1.5, Math.Sqrt(3) * 3 / 2, 0);
            //Vertex v5 = new Vertex(0, Math.Sqrt(3) * 3, 0);
            //baricentri.Add(v0);
            //baricentri.Add(v1);
            //baricentri.Add(v2);
            //baricentri.Add(v3);
            //baricentri.Add(v4);
            //baricentri.Add(v5);
            #endregion

            #region ESEMPIO 6: griglia bis
            //Esempio griglia: modificare n (num di righe) e aggiungendo v1,v2... si cambia il numero delle colonne
            //for (int i = 0; i < 9; i++)
            //{
            //    Vertex v1 = new Vertex(i, -i, 0);
            //    baricentri.Add(v1);
            //    Vertex v2 = new Vertex(i + 1, -i + 1, 0);
            //    baricentri.Add(v2);
            //    Vertex v3 = new Vertex(i + 2, -i + 2, 0);
            //    baricentri.Add(v3);
            //    Vertex v4 = new Vertex(i + 3, -i + 3, 0);
            //    baricentri.Add(v4);
            //    Vertex v5 = new Vertex(i + 4, -i + 4, 0);
            //    baricentri.Add(v5);
            //    Vertex v6 = new Vertex(i + 5, -i + 5, 0);
            //    baricentri.Add(v6);
            //    Vertex v7 = new Vertex(i + 6, -i + 6, 0);
            //    baricentri.Add(v7);
            //    Vertex v8 = new Vertex(i + 7, -i + 7, 0);
            //    baricentri.Add(v8);
            //    Vertex v9 = new Vertex(i + 8, -i + 8, 0);
            //    baricentri.Add(v9);
            //}
            //Console.WriteLine("Numero di baricentri: " + baricentri.Count);
            #endregion

            #region ESEMPIO 7: da Katia, punti su piano non parallelo ai piani cartesiani
            //Esempio su piano non parallelo (generato da Katia):
            //Vertex v0 = new Vertex(0.0238380280507662, 0.0181023083718007, 0.020260485891661);
            //Vertex v1 = new Vertex(0.0335075932860495, 0.0206517216691303, 0.020260485891661);
            //Vertex v2 = new Vertex(0.0238380280507662, 0.0181023083718007, 0.010260485891661);
            //Vertex v3 = new Vertex(0.0335075932860495, 0.0206517216691303, 0.010260485891661);
            //Vertex v4 = new Vertex(0.00449889758019963, 0.0130034817771415, 0.020260485891661);
            //Vertex v5 = new Vertex(0.0141684628154829, 0.0155528950744711, 0.020260485891661);
            //Vertex v6 = new Vertex(0.00449889758019962, 0.0130034817771415, 0.010260485891661);
            //Vertex v7 = new Vertex(0.0141684628154829, 0.0155528950744711, 0.010260485891661);
            //Vertex v8 = new Vertex(-0.0148402328903669, 0.00790465518248231, 0.020260485891661);
            //Vertex v9 = new Vertex(-0.00517066765508366, 0.0104540684798119, 0.020260485891661);
            //Vertex v10 = new Vertex(-0.0148402328903669, 0.00790465518248231, 0.010260485891661);
            //Vertex v11 = new Vertex(-0.00517066765508366, 0.0104540684798119, 0.010260485891661);
            //Vertex v12 = new Vertex(-0.0412579764002626, 0.000939528524686155, 0.0102604858916609);
            //Vertex v13 = new Vertex(-0.0412579764002626, 0.000939528524686156, 0.0202604858916609);
            //Vertex v14 = new Vertex(-0.0328838872629564, 0.00314738520491943, 0.00526048589166094);
            //Vertex v15 = new Vertex(-0.0245097981256502, 0.00535524188515271, 0.0102604858916609);
            //Vertex v16 = new Vertex(-0.0245097981256502, 0.00535524188515271, 0.020260485891661);
            //Vertex v17 = new Vertex(-0.0328838872629564, 0.00314738520491944, 0.025260485891661);
            //Vertex v18 = new Vertex(-0.05, -0.00136533531028923, 0.06);
            //Vertex v19 = new Vertex(0.05, 0.025, 0.06);
            //Vertex v20 = new Vertex(0.05, 0.025, 0);
            //Vertex v21 = new Vertex(-0.05, -0.00136533531028923, 0);

            //baricentri.Add(v0);
            //baricentri.Add(v1);
            //baricentri.Add(v2);
            //baricentri.Add(v3);
            //baricentri.Add(v4);
            //baricentri.Add(v5);
            //baricentri.Add(v6);
            //baricentri.Add(v7);
            //baricentri.Add(v8);
            //baricentri.Add(v9);
            //baricentri.Add(v10);
            //baricentri.Add(v11);
            //baricentri.Add(v12);
            //baricentri.Add(v13);
            //baricentri.Add(v14);
            //baricentri.Add(v15);
            //baricentri.Add(v16);
            //baricentri.Add(v17);
            #endregion


            List<MyVertex> listPointVertex = new List<MyVertex>();

            foreach(MyVertex vertex in listPointVertex)
            {
                baricentri.Add(vertex);
            }

            baricentri.Remove(baricentri.Last());
            baricentri.Remove(baricentri.Last());
            baricentri.Remove(baricentri.Last());
            baricentri.Remove(baricentri.Last());
         
            StringBuilder fileOutput = new StringBuilder();
            fileOutput.AppendLine("Prima riga del file");

            List<MyMatrAdj> listamatr = Functions.CreateMatrAdj(baricentri, ref fileOutput);
            #region Mostra informazioni su matrici di adiacenza
            //Ordina le matrici in modo decrescente rispetto al n° di occorrenze, poi a parità di n°occorrenze in modo crescente rispetto alla d
            //foreach (MyMatrAdj matradj in listamatr)
            //{
            //    fileOutput.AppendLine("\n Matrice n° " + listamatr.IndexOf(matradj) + ": ");
            //    fileOutput.AppendLine("\n la distanza è " + matradj.d);
            //    fileOutput.AppendLine("\n le occorrenze sono " + matradj.nOccur);
            //    fileOutput.AppendLine("\n");
            //}
            #endregion

            //List<List<int>> lista = PathCreation.Functions.FindPaths(listamatr[0], baricentri, ref fileOutput);

            // Create a file to write to. 
            string myPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outfile = new StreamWriter(myPath + @"\OutputFile.txt", true))
            {
                outfile.Write(fileOutput.ToString());
                outfile.Close();
            }

        }

        public static List<MyVertex> extractVertecesFromSelectedFaces(SldWorks SwApplication, ModelDoc2 swModel)
        {
            // Create a file to write to. 
            StringBuilder sb = new StringBuilder();
            List<MyVertex> listPointVertex = new List<MyVertex>();
            var selectedEntities = BRepFunctions.GetSelectedFaces(SwApplication, swModel);
            SwApplication.SendMsgToUser("Numero entità selezionate " + selectedEntities.Count.ToString());

            foreach (Entity selection in selectedEntities)
            {
                var VertecesList =  BRepFunctions.MyGetVertexFromFace(selection);
                
                foreach (SolidWorks.Interop.sldworks.Vertex vertex in VertecesList)
                {
                    var point = (Array)vertex.GetPoint();
                    var currentVertex = new ClassesOfObjects.MyVertex((double)point.GetValue(0), (double)point.GetValue(1), (double)point.GetValue(2));
                    listPointVertex.Add(currentVertex);

                    
                }
            }
            return listPointVertex;
        }

        /*
        public static List<ClassesOfObjects.MyVertex> MyGetVertexFromFace(Object entity)
        {
            var face = (Face2)entity;
            var listVertexFace = new List<ClassesOfObjects.MyVertex>();
            var listEdgeFaceCount = face.GetEdgeCount(); // Anche se non è utilizzato è necessario per non far disconnettere l'oggetto, evitando il refresh.
            var listEdgeFace = (Array)face.GetEdges();

            foreach (Edge edge in listEdgeFace)
            {
                if (edge != null)
                {
                    var edgeCurve = (Curve)edge.GetCurve();
                    if (edgeCurve.IsLine())  // --> Non è da fare solo per edge piani, ma per tutti, con piccolo accorgimento
                    //Se lo start vertex o l'end vertex sono nulli, allora si chiama la funzione MyPointOfCircleEdge e si salva quel punto.
                    {
                        var vertexS = (ClassesOfObjects.MyVertex)edge.GetStartVertex(); // Se è nullo si chiama MyPointOfCircleEdge con t=0
                        if (!listVertexFace.Contains(vertexS))
                        {
                            listVertexFace.Add(vertexS);
                        }

                        var vertexE = (ClassesOfObjects.MyVertex)edge.GetEndVertex(); // Se è nullo si chiama MyPointOfCircleEdge con t=1
                        if (!listVertexFace.Contains(vertexE))
                        {
                            listVertexFace.Add(vertexE);
                        }
                    }
                }
            }

            return listVertexFace;
        }
         * */
    } //fine Main
    //fine Program
}
