using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a surface with a list of Patterns (of Repeated entities) adjacent to it
    public class MyGroupingSurfaceForPatterns
    {
        public Surface groupingSurface;
        public List<MyPattern> listOfPatternsLine;
        public List<MyPattern> listOfPatternsCircum;

        public MyGroupingSurfaceForPatterns()
        {
        }

        public MyGroupingSurfaceForPatterns(Surface GroupingSurface, List<MyPattern> ListOfPatternsLine, List<MyPattern> ListOfPatternsCircum)
        {
            this.groupingSurface = GroupingSurface;
            this.listOfPatternsLine = ListOfPatternsLine;
            this.listOfPatternsCircum = ListOfPatternsCircum;
        }
    }
}

