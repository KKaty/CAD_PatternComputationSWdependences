using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        //Se NumOfCent>=3:
        //Data una lista di centroid, crea matrici di adiacenza per ogni distanza tra centroid con occorrenza >=2
        //Inoltre le ordine per numero di occorrenze decrescente
        public static List<MyMatrAdj> CreateMatrAdj(List<MyVertex> ListCentroid,  ref StringBuilder fileOutput)
        {
            List<MyMatrAdj> MatrAdjList = new List<MyMatrAdj>();  
                       
            int NumOfCent = ListCentroid.Count;

            if (NumOfCent < 3)           //(forse questo controllo non serve perché utilizzo questa function se so già di avere n>=4)
            {
                fileOutput.AppendLine("There are not enough centroids in the list.");
            }
            else
            {
                for (int i = 0; i < NumOfCent - 1; i++)
                {
                    for (int j = i + 1; j < NumOfCent; j++)
                    {
                        double dist = ListCentroid[i].Distance(ListCentroid[j]);
                        int FoundIndex = MatrAdjList.FindIndex(matradj => Math.Abs(matradj.d - dist)< Math.Pow(10, -4));
                        if (FoundIndex == -1)  //non è ancora stata creata la matrice di adiacenza per d
                        {
                            int[,] Matrix = new int[NumOfCent, NumOfCent];  //initialized to zero
                            Matrix[i, j] = 1;
                            Matrix[j, i] = 1;
                            int NOccur = 1;
                            MyMatrAdj NewMatrAdj = new MyMatrAdj(dist, Matrix, NOccur);
                            MatrAdjList.Add(NewMatrAdj);
                        }
                        else                //esiste già la matrice di adiacenza per d, è Found, la recupero
                        {
                            MatrAdjList[FoundIndex].matr[i, j] = 1;
                            MatrAdjList[FoundIndex].matr[j, i] = 1;
                            MatrAdjList[FoundIndex].nOccur += 1;

                        }
                    }
                }

                //I remove all the MatrAdjs with only one occurrence
                MatrAdjList.RemoveAll(m => m.nOccur == 1);

                //I order the list by creasing ordering respect to d, then by decreasing ordering respect to nOccur
                MatrAdjList = MatrAdjList.OrderBy(x => x.d).ThenByDescending(x => x.nOccur).ToList();

                //I order the list by decreasing ordering respect to nOccur, then by creasing ordering respect to d
                //MatrAdjList = MatrAdjList.OrderByDescending(x => x.nOccur).ThenBy(x => x.d).ToList();

            }

            return MatrAdjList;
        }    
 
    }
}
