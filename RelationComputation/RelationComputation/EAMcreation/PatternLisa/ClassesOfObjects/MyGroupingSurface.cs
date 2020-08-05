using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a surface with a list of Repeated entities adjacent to it
    public class MyGroupingSurface
    {
        public Surface groupingSurface;
        public MyPlane KLplanareSurface;
        public bool EntirePartAsRE = false;
        public List<MyRepeatedEntity> listOfREOfGS;
        //public List<int> listOfIdOfRE;
        //public int cardinalityOfListOfRepeatedEntity;

        public MyGroupingSurface()
        {
        }

        public MyGroupingSurface(Surface GroupingSurface, List<MyRepeatedEntity> listOfReofGs)
        {
            this.groupingSurface = GroupingSurface;
            this.listOfREOfGS = listOfReofGs;
            //this.listOfIdOfRE = listOfIdOfRe;
            //this.cardinalityOfListOfRepeatedEntity = this.listOfREOfGS.Count;
        }

        public MyGroupingSurface(MyPlane planareSurface, List<MyRepeatedEntity> listOfReofGs)
        {
            KLplanareSurface = planareSurface;
            listOfREOfGS = listOfReofGs;
            EntirePartAsRE = true;

            //this.listOfIdOfRE = listOfIdOfRe;
            //this.cardinalityOfListOfRepeatedEntity = this.listOfREOfGS.Count;
        }
    }
}
