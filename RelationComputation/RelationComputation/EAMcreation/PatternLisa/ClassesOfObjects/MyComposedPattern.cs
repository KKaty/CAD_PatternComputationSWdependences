using System.Collections.Generic;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a group of patterns of MyRepeatedEntity with a symmetric arrangement
    public class MyComposedPattern
    {
        public List<MyPattern> listOfMyPattern = new List<MyPattern>();
        public MyPathGeometricObject pathOfMyComposedPattern;
        public string typeOfMyComposedPattern;
        public double constStepOfMyComposedPattern;   //the distance between two pattern centroids
        //public List<Surface> listOfGroupingSurfaces;
        //public MyVertex composedPatternCentroid = new MyVertex();

        public MyComposedPattern()
        {
        }

        public MyComposedPattern(List<MyPattern> ListOfMyPattern, MyPathGeometricObject PathOfMyComposedPattern, string TypeOfMyComposedPattern)
        {
            this.listOfMyPattern = ListOfMyPattern;
            this.pathOfMyComposedPattern = PathOfMyComposedPattern;
            this.typeOfMyComposedPattern = TypeOfMyComposedPattern;
            this.constStepOfMyComposedPattern = ListOfMyPattern[0].patternCentroid.Distance(ListOfMyPattern[1].patternCentroid);
        }
    }
}
