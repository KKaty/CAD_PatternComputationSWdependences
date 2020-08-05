using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {
        //It verifies if a symmetry REFLECTIONAL relation between two MyRepeatedEntity exists
        public static bool IsReflectionTwoComp_Assembly(MyRepeatedComponent firstComponent,
            MyRepeatedComponent secondComponent)
        {
            const string nameFile = "GetReflectionalPattern.txt";
            KLdebug.Print(" ", nameFile);
            var whatToWrite = "";
            int i = 0;

            for (var j = 0; j < 3; j++)
            {
                double[] firstVector =
                {
                    firstComponent.Transform.RotationMatrix[0, j],
                    firstComponent.Transform.RotationMatrix[1, j],
                    firstComponent.Transform.RotationMatrix[2, j]
                };
                double[] secondVector =
                {
                    secondComponent.Transform.RotationMatrix[0, j],
                    secondComponent.Transform.RotationMatrix[1, j],
                    secondComponent.Transform.RotationMatrix[2, j]
                };

                whatToWrite = string.Format("Versore 1^ componente: ({0},{1},{2})", firstVector[0], firstVector[1], firstVector[2]);
                KLdebug.Print(whatToWrite, nameFile);
                whatToWrite = string.Format("Versore 2^ componente: ({0},{1},{2})", secondVector[0], secondVector[1], secondVector[2]);
                KLdebug.Print(whatToWrite, nameFile);
                KLdebug.Print("", nameFile);

            }

            var candidateReflMyPlane = Part.PartUtilities.GeometryAnalysis.GetCandidateReflectionalMyPlane(firstComponent.RepeatedEntity.centroid,
                secondComponent.RepeatedEntity.centroid, null);

            while (i < 3)
            {
                KLdebug.Print("Controllo del " + i + "-esimo versore", nameFile);
                var firstVector = new double[] {
                    firstComponent.Transform.RotationMatrix[0, i],
                    firstComponent.Transform.RotationMatrix[1, i],
                    firstComponent.Transform.RotationMatrix[2, i]
                };
                var secondVector = new double[] {
                    secondComponent.Transform.RotationMatrix[0, i],
                    secondComponent.Transform.RotationMatrix[1, i],
                    secondComponent.Transform.RotationMatrix[2, i]
                };

                whatToWrite = string.Format("Versore 1^ componente: ({0},{1},{2})", firstVector[0], firstVector[1], firstVector[2]);
                KLdebug.Print(whatToWrite, nameFile);
                whatToWrite = string.Format("Versore 2^ componente: ({0},{1},{2})", secondVector[0], secondVector[1], secondVector[2]);
                KLdebug.Print(whatToWrite, nameFile);

                var reflectedNormal = Part.PartUtilities.GeometryAnalysis.ReflectNormal(firstVector, candidateReflMyPlane);
                whatToWrite = string.Format("Riflesso di versore 1^comp: ({0},{1},{2})", reflectedNormal[0], reflectedNormal[1], reflectedNormal[2]);
                KLdebug.Print(whatToWrite, nameFile);

                if (FunctionsLC.MyEqualsArray(secondVector, reflectedNormal))
                {
                    KLdebug.Print(" -> Trovata corrispondenza per il versore " + i, nameFile);
                    i++;
                }
                else
                {
                    KLdebug.Print(" -> NON è stata trovata corrispondenza per il versore " + i, nameFile);
                    KLdebug.Print("FINE", nameFile);
                    return false;
                }
                KLdebug.Print(" ", nameFile);
            }

            KLdebug.Print(" ", nameFile);
            KLdebug.Print("ANDATO A BUON FINE IL CONTROLLO DEI VERSORI PER QUESTE COMPONENTI.", nameFile);
            KLdebug.Print(" ", nameFile);
            KLdebug.Print(" ", nameFile);
            return true;
        }
    }
}
