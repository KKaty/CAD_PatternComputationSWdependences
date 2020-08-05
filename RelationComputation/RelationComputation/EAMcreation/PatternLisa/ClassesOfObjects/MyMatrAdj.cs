namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{    
    //Classe per la ricerca di path in un insieme di baricentri (distanza tra baricentri ripetuta)
    public class MyMatrAdj 
    {
        public double d;                        //distanza ricorrente
        public int[,] matr;                     //matrice di adiacenza
        public int nOccur;                      //numero delle occorrenze della distanza d

        public MyMatrAdj(double D, int[,] Matr, int NOccur)
        {
            this.d = D;
            this.matr = Matr;
            this.nOccur = NOccur;
        }
        
    }
}
