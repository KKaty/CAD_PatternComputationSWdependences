using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a group on MyRepeatedComponents with a symmetric arrangement
    [JsonObject("PatternOfComponents")]
    public class MyPatternOfComponents
    {
        [JsonProperty("Id")] public int idMyPattern = -1;
        [JsonProperty("ListPattern")]
        public List<MyRepeatedComponent> listOfMyRCOfMyPattern = new List<MyRepeatedComponent>();
        [JsonProperty("Path")]
        public MyPathGeometricObject pathOfMyPattern;
        [JsonProperty("Type")]
        public string typeOfMyPattern;   // translation, reflection, rotation
        [JsonProperty("Step")]
        public double constStepOfMyPattern;
        [JsonProperty("Angle")]
        public double angle = -1; //it is modified only in case of circular patterns
        [JsonProperty("Centroid")]
        public MyVertex patternCentroid = new MyVertex();

        public MyPatternOfComponents()
        {
        }

        public MyPatternOfComponents(List<MyRepeatedComponent> ListOfMyRCOfMyPattern, 
            MyPathGeometricObject PathOfMyPattern, string TypeOfMyPattern)
        {
            this.listOfMyRCOfMyPattern = ListOfMyRCOfMyPattern;
            this.pathOfMyPattern = PathOfMyPattern;
            this.typeOfMyPattern = TypeOfMyPattern;
            this.constStepOfMyPattern = ListOfMyRCOfMyPattern[0].Origin.Distance(ListOfMyRCOfMyPattern[1].Origin);
        }
    }
}
