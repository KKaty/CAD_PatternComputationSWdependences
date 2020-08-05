using System.Collections.Generic;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a group on patterns of MyRepeatedComponent with a symmetric arrangement
    public class MyComposedPatternOfComponents
    {
        public List<MyPatternOfComponents> ListOfMyPatternOfComponents = new List<MyPatternOfComponents>();
        public MyPathGeometricObject pathOfMyComposedPatternOfComponents;
        public string typeOfMyComposedPatternOfComponents;
        public double constStepOfMyComposedPatternOfComponents;   //the distance between two pattern origins

        public MyComposedPatternOfComponents()
        {
        }

        public MyComposedPatternOfComponents(List<MyPatternOfComponents> ListOfMyPatternOfComponents,
            MyPathGeometricObject PathOfMyComposedPatternOfComponents, string TypeOfMyComposedPatternOfComponents)
        {
            this.ListOfMyPatternOfComponents = ListOfMyPatternOfComponents;
            this.pathOfMyComposedPatternOfComponents = PathOfMyComposedPatternOfComponents;
            this.typeOfMyComposedPatternOfComponents = TypeOfMyComposedPatternOfComponents;
            this.constStepOfMyComposedPatternOfComponents = 
                ListOfMyPatternOfComponents[0].patternCentroid.Distance(ListOfMyPatternOfComponents[1].patternCentroid);
        }
    }
}
