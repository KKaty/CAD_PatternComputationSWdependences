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

                

            }

            var candidateReflMyPlane = Part.PartUtilities.GeometryAnalysis.GetCandidateReflectionalMyPlane(firstComponent.RepeatedEntity.centroid,
                secondComponent.RepeatedEntity.centroid, null);

            while (i < 3)
            {
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

                
                var reflectedNormal = Part.PartUtilities.GeometryAnalysis.ReflectNormal(firstVector, candidateReflMyPlane);
                if (FunctionsLC.MyEqualsArray(secondVector, reflectedNormal))
                {
                    i++;
                }
                else
                {
                 return false;
                }
            }

            return true;
        }
    }
}
