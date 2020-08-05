using AssemblyRetrieval.EAMcreation;
using AssemblyRetrieval.Graph;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelationComputation
{
    class PatternComputationFunctions
    {
        
        public static void GetAssemblyPatternsOfRepeatedElements(List<MyListOfInstances> listOfMyListOfInstances,
       KLgraph.KLnodeAssembly nodeAssembly, out List<MyPatternOfComponents> listPattern,
       out List<MyPatternOfComponents> listPattern2)
        {

            listPattern = new List<MyPatternOfComponents>();
            listPattern2 = new List<MyPatternOfComponents>();
            AssemblyTraverse.LCComputeRepeatedPattern(listOfMyListOfInstances, ref listPattern, ref listPattern2,
                null, null);
            nodeAssembly.KLlistPattern.AddRange(listPattern);
            nodeAssembly.KLlistPatternTwo.AddRange(listPattern2);

            foreach (MyPatternOfComponents pattern in listPattern2)
            {
                if (pattern.typeOfMyPattern == "linear TRANSLATION" ||
                    pattern.typeOfMyPattern == "TRANSLATION of length 2")
                {
                    nodeAssembly.KLstatistic.LinearPatternNumber++;
                }
                else if (pattern.typeOfMyPattern == "linear ROTATION")
                {
                    nodeAssembly.KLstatistic.CircularPatternNumber++;
                }
                else if (pattern.typeOfMyPattern == "circular TRANSLATION")
                {
                    nodeAssembly.KLstatistic.CircularPatternNumber++;
                }
                else if (pattern.typeOfMyPattern == "REFLECTION")
                {
                    nodeAssembly.KLstatistic.ReflectivePatternNumber++;
                }
            }

            foreach (MyPatternOfComponents myPatternOfComponentse in listPattern)
            {
                if (myPatternOfComponentse.typeOfMyPattern == "ROTATION")
                {
                    if (Math.Abs(myPatternOfComponentse.angle + 1) < 0.01)
                    {
                        nodeAssembly.KLstatistic.ReflectivePatternNumber++;
                    }
                    else
                    {
                        nodeAssembly.KLstatistic.CircularPatternNumber++;
                    }
                }
                else if (myPatternOfComponentse.typeOfMyPattern == "REFLECTION")
                {
                    nodeAssembly.KLstatistic.ReflectivePatternNumber++;
                }
                else
                {
                    nodeAssembly.KLstatistic.LinearPatternNumber++;
                }
            }
        }
    }
}
