using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using AssemblyRetrieval.EAMcreation;
using AssemblyRetrieval.Graph;
using Newtonsoft.Json;
using QuickGraph;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace RelationComputation
{
    class Program
    {
        public static AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge> graph = new AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge>(true);
        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");

            var swApp = new SldWorks();

            var swAPIWarnings = 0;
            var swAPIErrors = 0;

            var importData = (ImportStepData)swApp.GetImportFileData(args[0]);
            var swActiveDoc = (ModelDoc2)swApp.LoadFile4(args[0], "r", importData, swAPIErrors);

            if (swActiveDoc == null)
            {
                Console.WriteLine("swActiveDoc nullo " + swAPIWarnings + " " + swAPIErrors);
                return;
            }

            var swAssemblyDoc = (AssemblyDoc)swActiveDoc;

            var SwConfiguration = swActiveDoc.GetActiveConfiguration();
            var rootComponent = (Component2)SwConfiguration.GetRootComponent();
            var fileName = rootComponent.Name2;

            //Console.WriteLine("Model " + fileName + " loaded correctly");

            var transforation =
                (Array)AssemblyTraverse.KL_GetTransformsOfAssemblyComponents(rootComponent, swApp);
            const string fatherName = KLgraph.RootLabel;
            var pathComponent = rootComponent.Name2;
            var componentName = pathComponent.Split('/').Last();
            var componentPath = rootComponent.GetPathName();
            var id = rootComponent.GetHashCode();

            var childrenNumber = rootComponent.IGetChildrenCount();
            var shape = AssemblyTraverse.KL_GetShapeAssembly(rootComponent, swApp);
            var statistic = AssemblyTraverse.KL_GetStatisticAssembly(rootComponent, swApp);
            var nodeAssembly = new KLgraph.KLnodeAssembly(id, transforation, fatherName, -1, rootComponent, pathComponent, componentName, componentPath, -1,
                childrenNumber, statistic, shape);

            //Console.WriteLine("Creato il nodo assemblato");

            var vertexList = new List<KLgraph.KLnode>();
            var edgeList = new List<KLgraph.KLedge>();
            vertexList.Add(nodeAssembly);
            var listOfMyListOfInstances = new List<MyListOfInstances>();

            AssemblyTraverse.KL_GetGraphOfAssemblyComponents(rootComponent, nodeAssembly,
               ref vertexList, edgeList, ref listOfMyListOfInstances, swApp);

            
            var partList = new List<KLgraph.KLnode>(vertexList);
            partList.RemoveAll(v => v.GetType() != typeof(KLgraph.KLnodePart));

            nodeAssembly.KLstatistic.PrincipalPartNumber = partList.Count();

            nodeAssembly.Instances.AddRange(listOfMyListOfInstances);


            List<MyPatternOfComponents> listPattern;
            List<MyPatternOfComponents> listPattern2;


            PatternComputationFunctions.GetAssemblyPatternsOfRepeatedElements(listOfMyListOfInstances, nodeAssembly,
                out listPattern, out listPattern2);
          

            

            //Console.WriteLine("Istanze trovate " + listOfMyListOfInstances.Count);
            //foreach (var inst in listOfMyListOfInstances)
            //{
            //    Console.WriteLine("Istanza di " + inst.Name + " trovata " + inst.ListOfMyComponent.Count + " volte");
            //}

            //Console.WriteLine("Pattern trovati " + listPattern.Count);
            //foreach (var patt in listPattern)
            //{
            //    Console.WriteLine("Pattern di " + patt.listOfMyRCOfMyPattern.First().Name);
            //}


            //Console.WriteLine("Ending.. nodi " + graph.VertexCount + " e archi " + graph.EdgeCount);


            //var fileName = Path.GetFileNameWithoutExtension(args[0]);
            //var storageGraph = new KLgraph.Graph(graph.Vertices.ToList(), graph.Edges.ToList());

            var storageGraph = new List<List<MyPatternOfComponents>>();
            storageGraph.Add(listPattern2);
            storageGraph.Add(listPattern);

            JsonSerializerSettings setting = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
            };

            var directoryOutput = Path.GetDirectoryName(args[1]);
            var fileNameOutput = Path.GetFileNameWithoutExtension(args[1]);
            //Console.WriteLine(directoryOutput);
            //Console.WriteLine(fileNameOutput);

            SaveModel(JsonConvert.SerializeObject(storageGraph, setting), directoryOutput, fileNameOutput + ".json");
            Console.WriteLine("JSON saved!");
            
            swApp.ExitApp();
        }

        public static void SaveModel(string line, string pathfolder, string fileName)
        {

            using (StreamWriter w = File.CreateText(pathfolder + "\\" + fileName))
            {
                w.WriteLine(line);
            }
        }

    }
}
