using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using AssemblyRetrieval.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using QuickGraph;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.Utility
{
    public class Serialization
    {
        public class binder : SerializationBinder
        {
            Type[] types;
            public binder(Type[] Types)
            {
                types = Types;
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                if (assemblyName == "RaspElements")
                {

                    var type = types.Where(t => t.Name == typeName).FirstOrDefault();

                    if (type != null)
                        return type;

                }
                return Type.GetType(typeName + ", " + assemblyName);
            }
        }

        //public class JsonNodeConverter : CustomCreationConverter<KLgraph.KLnode>
        //{
        //    public override KLgraph.KLnode Create(Type objectType)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public KLgraph.KLnode Create(Type objectType, JObject jObject)
        //    {
        //        var type = (string) jObject.Property("valueType");
        //        switch (type)
        //        {
        //            case "int":
        //                return new KLgraph.KLnodePart();
        //            case "string":
        //                return new KLgraph.KLnodeAssembly();
        //        }

        //        throw new ApplicationException(String.Format("The given vehicle type {0} is not supported!", type));
        //    }

        //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        //        JsonSerializer serializer)
        //    {
        //        // Load JObject from stream
        //        JObject jObject = JObject.Load(reader);

        //        // Create target object based on JObject
        //        var target = Create(objectType, jObject);

        //        // Populate the object properties
        //        serializer.Populate(jObject.CreateReader(), target);

        //        return target;
        //    }
        //}

        public static bool ReadJsonModel(string jsonFile, ref AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge> graph, SldWorks swApplication)
        {
            Type[] runtimeTypes = new Type[] { typeof(KLgraph.KLedgeJoint), typeof(KLgraph.KLedgeContact), typeof(KLgraph.KLedgeStructure) };
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                Binder = new Serialization.binder(runtimeTypes)
            };
            var deserializedGraph = JsonConvert.DeserializeObject<KLgraph.Graph>(jsonFile, settings);

            
            graph.Clear();
            graph.AddVertexRange(deserializedGraph.Nodes);
            graph.AddEdgeRange(deserializedGraph.Edges);
            
           if (deserializedGraph.Edges == null)
            {
                swApplication.SendMsgToUser("DelegateNode graph nulli");
                return false;
            }
            if (graph == null)
            {
                swApplication.SendMsgToUser("Comparison nullo");
                return false;
            }
            
            return true;
        }

     /*
        public static bool ReadCliqueModel(string jsonFile, ref AdjacencyGraph<KLassociativeGraph.KLassociativeNode, KLassociativeGraph.KLassociativeEdge> clique, SldWorks swApplication)
        {
            try
            {
                Type[] runtimeTypes = new Type[] { typeof(KLassociativeGraph.KLassociativeEdge), typeof(UndirectedEdge<KLassociativeGraph.KLassociativeNode>)};
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
                    Binder = new Serialization.binder(runtimeTypes)
                };
                
                var deserializedGraph = JsonConvert.DeserializeObject<KLassociativeGraph.AssociativeGraph>(jsonFile, settings);

                if (deserializedGraph == null)
                {
                    swApplication.SendMsgToUser("Grafo des nullo");
                    return false;

                }

                var edgeList = new List<KLassociativeGraph.KLassociativeEdge>();
                foreach (var undirectedEdge in deserializedGraph.Edges)
                {
                    var edge = new KLassociativeGraph.KLassociativeEdge(undirectedEdge.Source, undirectedEdge.Target, 0, 0);
                    edgeList.Add(edge);
                }
                clique.AddVertexRange(deserializedGraph.Nodes.ToList());
                clique.AddEdgeRange(edgeList);

                if (deserializedGraph.Edges == null)
                {
                    swApplication.SendMsgToUser("DelegateNode graph nulli");
                    return false;
                }
                if (clique == null)
                {
                    swApplication.SendMsgToUser("Clique nulla");
                    return false;
                }

                swApplication.SendMsgToUser("Nodi clique deserializzati " + clique.VertexCount);
                swApplication.SendMsgToUser("Edge clique deserializzati " + clique.EdgeCount);
            }
            catch (Exception exception)
            {

                swApplication.SendMsgToUser(exception.Message);
                swApplication.SendMsgToUser(exception.Source);
            }

            return true;
        }

        public static void SaveCliqueResult(AdjacencyGraph<KLassociativeGraph.KLassociativeNode, KLassociativeGraph.KLassociativeEdge> clique, Measures measure, string fileName)
        {
            var undirectEdgeList = new List<UndirectedEdge<KLassociativeGraph.KLassociativeNode>>();
            foreach (var edge in clique.Edges)
            {
                var undirectedEdge = new UndirectedEdge<KLassociativeGraph.KLassociativeNode>(edge.Source, edge.Target);
                undirectEdgeList.Add(undirectedEdge);
            }
            var assGraph = new KLassociativeGraph.AssociativeGraph(clique.Vertices.ToList(), undirectEdgeList);
            var resultToBeSaved = new CliqueResults(assGraph, measure);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            };
            Debug.KLdebug.SaveModel(JsonConvert.SerializeObject(resultToBeSaved, settings), SwTaskPaneHost.PathFolderClique,
                fileName + ".json");
        }
    
    */
    }
}
