using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Accord.Math;
using AssemblyRetrieval.EAMcreation;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
//using AssemblyRetrieval.Filter;
//using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using Newtonsoft.Json;
using QuickGraph;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using Matrix = Accord.Math.Matrix;

//using Accord.Math;

namespace AssemblyRetrieval.Graph
{
    public static partial class KLgraph
    {
        public const string RootLabel = "I am your father!";

        [JsonObject("Graph")]
        public class Graph
        {
            [JsonProperty("Nodes")]
            public List<KLnode> Nodes { get; set; }

            [JsonProperty("Edge")]
            public List<KLedge> Edges { get; set; }

            public Graph()
            {
            }

            public Graph(List<KLnode> nodes, List<KLedge> edges)
            {
                Nodes = nodes;
                Edges = edges;
            }

        
    /*
            public static string GetJointMeasureParameters(KLgraph.KLedgeJoint joint)
            {
                var dof = "";
                var linear = 0;
                var puntual = 0;

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                if (joint.ContacType == KLgraph.KLcontactType.Face)
                {
                    dof += "[";
                    foreach (KLgraph.KLRotation rotation in joint.Dof.Rotation)
                    {
                        var rot = String.Format("({0},{1},{2})", rotation.RotationAxis[0].ToString(nfi),
                            rotation.RotationAxis[1].ToString(nfi),
                            rotation.RotationAxis[2].ToString(nfi));
                        dof += rot;
                    }
                    dof += ";";
                    foreach (KLgraph.KLTranslation translation in joint.Dof.Translation)
                    {
                        var tra = String.Format("({0},{1},{2})", translation.Translation[0].ToString(nfi),
                            translation.Translation[1].ToString(nfi),
                            translation.Translation[2].ToString(nfi));
                        dof += tra;
                    }
                    dof += "]";
                }
                else if (joint.ContacType == KLgraph.KLcontactType.Edge)
                {
                    linear = 1;
                }
                else if (joint.ContacType == KLgraph.KLcontactType.Vertex)
                {
                    puntual = 1;
                }

                var measureParameters = String.Format("{0},{1},{2}", dof, linear, puntual);
                return measureParameters;
            }

            public static double[] GetDirectionUnlikedNodes(AdjacencyGraph<KLnode, KLedge> graph, string sourceName,
                string targetName)
            {
                var sourceCentroid = GetNodeCentroid(graph, sourceName);
                var targetCentroid = GetNodeCentroid(graph, targetName);

                var directionCurrent = new double[]
                {
                    sourceCentroid.x - targetCentroid.x,
                    sourceCentroid.y - targetCentroid.y,
                    sourceCentroid.z - targetCentroid.z
                };
                return directionCurrent;
            }

            public static double[] GetDirectionUnlikedNodes(KLnodeAssembly root, string sourceName, string targetName)
            {
                var sourceCentroid = GetNodeCentroid(root, sourceName);
                var targetCentroid = GetNodeCentroid(root, targetName);

                if (sourceCentroid != null && targetCentroid != null)
                {

                    var directionCurrent = new double[]
                    {
                        sourceCentroid.x - targetCentroid.x,
                        sourceCentroid.y - targetCentroid.y,
                        sourceCentroid.z - targetCentroid.z
                    };

                    return directionCurrent;
                }
                else
                {
                   //SwTaskPaneHost.SwApplication.SendMsgToUser("qualcosa è nullo!");
                }
                return null;
            }
        
        
        */
        
        }

        #region Vertex definition

        [JsonObject("KLnode")]
        public class KLnode
        {

            protected bool Equals(KLnode other)
            {
                return Name == other.Name; // && Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((KLnode) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }

            [JsonProperty("HashCode")]
            //[JsonIgnore]
            public int HashCode { get; set; }

            [JsonProperty("Id")]
            public int Id { get; set; }

            [JsonProperty("ComponentType")]
            public int ComponentType { get; set; }

            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("NameLastPart")]
            //[JsonIgnore]
            public string NameLastPart { get; set; }

            [JsonProperty("ComponentPath")]
            //[JsonIgnore]
            public string ComponentPath { get; set; }

            [JsonProperty("FatherName")]
            //[JsonIgnore]
            public string FatherName { get; set; }

            [JsonProperty("FatherId")]
            //[JsonIgnore]
            public int FatherId { get; set; }

            [JsonIgnore] public Component2 Comp;
            [JsonIgnore] public Array Transformation;


            public KLnode()
            {
            }

            public KLnode(int hashCode, Array transformation, string fatherName, int fatherId, Component2 comp,
                string name, string nameLastPart, string componentPath, int id, int componentType = (int)KLComponentType.NotAssigned)
            {
                HashCode = hashCode;
                Transformation = transformation;
                FatherName = fatherName;
                FatherId = fatherId;
                Comp = comp;
                Name = name;
                NameLastPart = nameLastPart;
                ComponentPath = componentPath;
                Id = id;
                ComponentType = componentType;
            }

            public bool SameTypeNode(KLnode other)
            {
                if (this.GetType() == typeof (KLgraph.KLnodePart) &&
                    other.GetType() == typeof(KLgraph.KLnodePart))
                {
                    return true;
                }
                if (this.GetType() == typeof(KLgraph.KLnodeAssembly) &&
                    other.GetType() == typeof(KLgraph.KLnodeAssembly))
                {
                    return true;
                }
                return false;
            }
        }

        [JsonObject("KLnodePart")]
        public class KLnodePart : KLnode
        {
            [JsonProperty("KLstatistic")] public KLstatisticPart KLstatistic;
            [JsonProperty("KLshape")] public KLshapePart KLshape;

            public KLnodePart()
            {
            }

            public KLnodePart(int hashCode, Array transformation, string fatherName, int fatherId, Component2 comp,
                string name, string nameLastPart, string componentPath, int id,
                KLstatisticPart kLstatistic, KLshapePart kLshape)
                : base(hashCode, transformation, fatherName, fatherId, comp, name, nameLastPart, componentPath, id)
            {
                KLstatistic = kLstatistic;
                KLshape = kLshape;
            }

            
            public bool NodeShapeIsSphere()
            {
                /*
                // Creo il nodo simile "sfera" per farne l'uguaglianza con le componenti che ho
                var listFirstModel = AssemblyTraverse.ReadSHdescriptor(swApplication, SwTaskPaneHost.pathSphere,
                    "ArmonicaSfera");
                var shape = new KLshapePart(0, 0, listFirstModel);
                var statistic = new KLstatisticPart();
                //swApplication.SendMsgToUser(componentName + "  " + statistic.Genus);
                var sphereNode = new KLnodePart(0, null, null, 0,
                    null, "sfera", "sfera", "sfera",
                    0,
                    statistic, shape);

                double firstDistance;
                if (Math.Abs(this.KLstatistic.FreeformPercent - 1) < 0.01)
                {
                    if (KLcriteriaCheck.Shape.SHshapedescriptor(this, sphereNode, 0.003, out firstDistance, swApplication))
                    {
                        return true;
                    }
                }
                else if (Math.Abs(this.KLstatistic.ShericalNumber - 1) < 0.01)
                {
                    return true;
                }
                return false;
                 * 
                 */
                return false;
            }
        }

        [JsonObject("KLnodeAssembly")]
        public class KLnodeAssembly : KLnode
        {
            [JsonProperty("ChildrenNumber")] public int ChildrenNumber;
            [JsonProperty("KLstatistic")] public KLstatisticAssembly KLstatistic;
            [JsonProperty("KLshape")] public KLshapeAssembly KLshape;

            [JsonProperty("Instances")] public List<MyListOfInstances> Instances = new List<MyListOfInstances>();

            [JsonProperty("KLlistPattern")] public List<MyPatternOfComponents> KLlistPattern = new List<MyPatternOfComponents>();

            [JsonProperty("KLlistPatternTwo")] public List<MyPatternOfComponents> KLlistPatternTwo = new List<MyPatternOfComponents>();

            [JsonProperty("EquivalentNode")] public List<EquivalentNode> EquivalentNode = new List<EquivalentNode>();


            public KLnodeAssembly()
            {
            }

            public KLnodeAssembly(int hashCode, Array transformation, string fatherName, int fatherId, Component2 comp,
                string name, string nameLastPart, string componentPath, int id,
                int childrenNumber, KLstatisticAssembly statistic, KLshapeAssembly shape)
                : base(hashCode, transformation, fatherName, fatherId, comp, name, nameLastPart, componentPath, id)
            {
                ChildrenNumber = childrenNumber;
                KLstatistic = statistic;
                KLshape = shape;
            }
        }

        #endregion

        #region Edge definition

        [JsonObject("KLedge")]
        public class KLedge : UndirectedEdge<KLnode>
        {
            public KLedge(KLnode source, KLnode target)
                : base(source, target)
            {
            }

            [JsonProperty("Id")]
            public int Id { get; set; }


            public KLedge(KLnode source, KLnode target, int id) : base(source, target)
            {
                Id = id;
            }
        }

        [JsonObject("KLedgeStructure")]
        public class KLedgeStructure : KLedge
        {
            public KLedgeStructure(KLnode source, KLnode target, int id)
                : base(source, target, id)
            {
            }
        }

        [JsonObject("KLedgePattern")]
        public class KLedgePattern : KLedge
        {
            [JsonProperty("Type")]
            public string Type { get; set; }


            public KLedgePattern(KLnode source, KLnode target, string type, int id)
                : base(source, target, id)
            {
                Type = type;
            }
        }

        public enum KLcontactType
        {
            Unsolved = -1,
            Face = 0,
            Edge = 1,
            Vertex = 2,
        }

        public enum KLfaceContactType
        {
            NoFace = -1,
            Plane = 0,
            Cylinder = 1,
            Cone = 2,
            Sphere = 3,
            Torus = 4,
            FreeForm = 5,
        }

        public enum KLfaceOrientation
        {
            NoMeaning = -1,
            Concave = 0,
            Convex = 1,
        }

        [JsonObject("KLedgeInterface")]
        public class KLedgeInterface : KLedge
        {
            public KLedgeInterface(KLnode source, KLnode target, int id)
                : base(source, target, id)
            {
            }

            [JsonProperty("DOF")] public KLdof Dof;
            [JsonProperty("ContactType")] public KLcontactType ContacType;


            //[JsonIgnore]
            //public List<Face2> Faces;
            //[JsonProperty("KinematicPair")]
            //public string KinematicPair;

            public KLedgeInterface(KLnode source, KLnode target, KLdof dof, KLcontactType contactType, int id)
                : base(source, target, id)
            {
                Dof = dof;
                ContacType = contactType;
            }
        }

        [JsonObject("KLedgeContact")]
        public class KLedgeContact : KLedgeInterface
        {
            //[JsonIgnore] public Face2 Face;
            //[JsonIgnore] public Edge Edge;
            //[JsonIgnore] public Vertex Vertex;

            // Non posso mettere queste entotà nel costruttore perché poi non riesco a deserializzare!
            // Il tipo faccia edge etc lo dovrò salvare in altro modo!
            [JsonProperty("FaceContactType")] public KLfaceContactType FaceContacType;
            [JsonProperty("FaceOrientation")] public KLfaceOrientation FaceOrientation;
            [JsonProperty("Radius")] public double Radius;
            [JsonProperty("PointOnContact")] public double[] PointOnContact;

            public KLedgeContact(KLnode source, KLnode target, KLdof dof, KLcontactType contacType,
                KLfaceContactType faceContacType, KLfaceOrientation faceOrientation, double radius, double[] pointOnContact, int id)
                : base(source, target, dof, contacType, id)
            {
                FaceContacType = faceContacType;
                FaceOrientation = faceOrientation;
                PointOnContact = pointOnContact;
                Radius = radius;
            }

            // Sarà meglio fare un metodo per dire il tipo in modo "facile"
        }

        [JsonObject("KLedgeJoint")]
        public class KLedgeJoint : KLedgeInterface
        {
            public KLcontactStatistic ContactStatistic;

            [JsonProperty("PointOnJoint")] public double[] PointOnJoint;

            public KLedgeJoint(KLnode source, KLnode target, KLdof dof, KLcontactType contacType, int id,
                KLcontactStatistic contactStatistic, double[] pointOnJoint)
                : base(source, target, dof, contacType, id)
            {
                ContactStatistic = contactStatistic;
                PointOnJoint = pointOnJoint;
            }
        }

        [JsonObject("KLedgeSyntesis")]
        public class KLedgeSyntesis : KLedgeInterface
        {
            public KLedgeSyntesis(KLnode source, KLnode target, List<Face2> faces, KLdof dof, KLcontactType contacType,
                int id)
                : base(source, target, dof, contacType, id)
            {
            }
        }

        [JsonObject("KLedgeMainPart")]
        public class KLedgeMainPart : KLedgeInterface
        {
            public KLedgeMainPart(KLnode source, KLnode target, List<Face2> faces, KLdof dof, KLcontactType contacType,
                int id)
                : base(source, target, dof, contacType, id)
            {
            }
        }

        [JsonObject("KLedgeMechanism")]
        public class KLedgeMechanism : KLedgeInterface
        {
            public KLedgeMechanism(KLnode source, KLnode target, List<Face2> faces, KLdof dof, KLcontactType contacType,
                int id)
                : base(source, target, dof, contacType, id)
            {
            }
        }

        #endregion

        #region Component type

        public enum KLComponentType
        {
            NotAssigned = -1,
            BearingSimplifiedAsPart = 0,
            Bearing = 1,
            CClip = 2,
            Cylinder = 3,
            Cube = 4,
            Gear = 5,
            Key = 6,
            LinkageArm = 7,
            Nut = 8,
            PartOfBearing = 9,
            Screw = 10,
            Shaft = 11,
            Spacer = 12,
            Sphere = 13,
            Torus = 14,
            Miscellaneous = 15,
        }

        #endregion

        #region Statistic definition

        [JsonObject("KLstatisticPart")]
        public class KLstatisticPart
        {
            public int Genus;

            public double PlanarPercent;
            public double CylindricalPercent;
            public double ConicalPercent;
            public double ShericalPercent;
            public double ToroidalPercent;
            public double FreeformPercent;

            public double PlanarNumber;
            public double CylindricalNumber;
            public double ConicalNumber;
            public double ShericalNumber;
            public double ToroidalNumber;
            public double FreeformNumber;

            public KLstatisticPart()
            {
            }

            public KLstatisticPart(int genus, double planarPercent, double cylindricalPercent, double conicalPercent,
                double shericalPercent, double toroidalPercent, double freeformPercent, double planarNumber,
                double cylindricalNumber, double conicalNumber, double shericalNumber, double toroidalNumber,
                double freeformNumber)
            {
                Genus = genus;
                PlanarPercent = planarPercent;
                CylindricalPercent = cylindricalPercent;
                ConicalPercent = conicalPercent;
                ShericalPercent = shericalPercent;
                ToroidalPercent = toroidalPercent;
                FreeformPercent = freeformPercent;
                PlanarNumber = planarNumber;
                CylindricalNumber = cylindricalNumber;
                ConicalNumber = conicalNumber;
                ShericalNumber = shericalNumber;
                ToroidalNumber = toroidalNumber;
                FreeformNumber = freeformNumber;
            }
        }

        [JsonObject("KLstatisticAssembly")]
        public class KLstatisticAssembly
        {
            public double SubassemblyNumber;
            public double PrincipalPartNumber;
            public double FastenerNumber;
            public double NormalPartNumber;
            public double ThinPartNumber;
            public double LinearPatternNumber;
            public double CircularPatternNumber;
            public double ReflectivePatternNumber;

            public KLstatisticAssembly()
            {
            }

            public KLstatisticAssembly(double subassemblyNumber, double principalPartNumber, double fastenerNumber,
                double normalPartNumber, double thinPartNumber, double linearPatternNumber, double circularPatternNumber,
                double reflectivePatternNumber)
            {
                SubassemblyNumber = subassemblyNumber;
                PrincipalPartNumber = principalPartNumber;
                FastenerNumber = fastenerNumber;
                NormalPartNumber = normalPartNumber;
                ThinPartNumber = thinPartNumber;
                LinearPatternNumber = linearPatternNumber;
                CircularPatternNumber = circularPatternNumber;
                ReflectivePatternNumber = reflectivePatternNumber;
            }
        }

        #endregion

        #region Shape definition

        [JsonObject("KLshapePart")]
        public class KLshapePart
        {
            [JsonProperty("Volume")] public double Volume;
            [JsonProperty("Surface")] public double Surface;
            [JsonProperty("SphericalHarmonic")] public List<double> SphericalHarmonic;


            public KLshapePart()
            {
            }

            public KLshapePart(double volume, double surface, List<double> sphericalHarmonic)
            {
                Volume = volume;
                Surface = surface;
                SphericalHarmonic = sphericalHarmonic;
            }
        }

        public class KLshapeAssembly
        {
            public KLshapeAssembly()
            {
            }
        }

        #endregion

        #region Joint definition

        public enum KLKinematicPairValues
        {
            Revolute = 0,
            Prismatic = 1,
            Screw = 2,
            Cylindrical = 3,
            Spherical = 4,
            Planar = 5
        }

        public class KLcontactStatistic
        {
            public int FaceNumber = 0;
            public int TraslationNumber = 0;
            public int TranslationNotAllowedNumber = 0;
            public int RotationNumber = 0;

            public KLcontactStatistic()
            {
            }

            public KLcontactStatistic(int rotationNumber, int translationNotAllowedNumber, int traslationNumber,
                int faceNumber)
            {
                RotationNumber = rotationNumber;
                TranslationNotAllowedNumber = translationNotAllowedNumber;
                TraslationNumber = traslationNumber;
                FaceNumber = faceNumber;
            }
        }

        public class KLdof
        {
            public List<KLTranslation> Translation = new List<KLTranslation>();
            public List<KLTranslation> TranslationNotAllowed = new List<KLTranslation>();
            public List<KLRotation> Rotation = new List<KLRotation>();
            public bool IsAddedFirstPlanarTranslation = false;
            public double[] FirstNormal = new double[] {};

            public KLdof()
            {
            }

            public bool ContainsTranslation(double[] newTranslation)
            {
                foreach (var trasl in Translation)
                {
                    var innerProduct = Math.Abs(Math.Abs(Matrix.InnerProduct(trasl.Translation, newTranslation)) - 1);
                    if (innerProduct < 0.01)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool ContainsRotation(double[] newRotation)
            {
                foreach (var rot in Rotation)
                {
                    var innerProduct = Math.Abs(Math.Abs(Matrix.InnerProduct(rot.RotationAxis, newRotation)) - 1);
                    if (innerProduct < 0.01)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public class KLTranslation
        {
            public double[] Translation = new double[] {};
            public int Repetition;

            public KLTranslation()
            {
            }

            public KLTranslation(double[] translation, int repetition)
            {
                Translation = translation;
                Repetition = repetition;
            }

            protected bool Equals(KLTranslation other)
            {
                var innerProduct = Math.Abs(Math.Abs(Matrix.InnerProduct(Translation, other.Translation)) - 1);
                if (innerProduct < 0.01)
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((KLTranslation) obj);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        public class KLRotation
        {
            public double[] RotationAxis = new double[] {};
            public double[] RotationPoint = new double[] {};
            public int Repetition;

            public KLRotation()
            {
            }

            public KLRotation(double[] rotationAxis, double[] rotationPoint, int repetition)
            {
                RotationAxis = rotationAxis;
                RotationPoint = rotationPoint;
                Repetition = repetition;
            }

            protected bool Equals(KLRotation other)
            {
                var innerProduct = Math.Abs(Math.Abs(Matrix.InnerProduct(RotationAxis, other.RotationAxis)) - 1);
                if (innerProduct < 0.01)
                {
                    if (RotationPoint != null && other.RotationPoint != null)
                    {
                        var pointDifferent = new double[]
                        {
                            RotationPoint[0] - other.RotationPoint[0], RotationPoint[1] - other.RotationPoint[1],
                            RotationPoint[2] - other.RotationPoint[2]
                        };
                        var norm = Norm.Euclidean(pointDifferent);
                        if (norm != 0.0)
                        {
                            pointDifferent = pointDifferent.Divide(norm);
                        }
                        else
                        {
                            return true;
                        }
                        var innerProduct2 = Math.Abs(Math.Abs(Matrix.InnerProduct(RotationAxis, pointDifferent)) - 1);

                        //if (FolderUtilities.FolderUtilities.KLIsVectorNull(pointDifferent, 0.01))
                        //{
                        //    return true;
                        //}
                        //else 
                        if (innerProduct2 < 0.01)
                        {
                            return true;
                        }
                    }
                    else if (RotationPoint == null || other.RotationPoint == null)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((KLRotation) obj);
            }

            public override int GetHashCode()
            {
                return (RotationAxis != null ? RotationAxis.GetHashCode() : 0);
            }
        }

        #endregion

        #region Functions

        /*
         // Si possono trovare con funzioni di In Out degli edge
        public static List<KLnode> getAdjacents(KLnode DelegateNode, AdjacencyGraph<KLnode, Edge<KLnode>> graph)
        {
            var adjacents = new List<KLnode>();
            foreach (KLEdge edge in graph.Edges)
            {
                adjacents.Add(edge.GetOtherVertex(DelegateNode));
                /*
                if (edge.Source.Equals(DelegateNode))
                {
                    adjacents.Add(edge.Target);
                }
                else if (edge.Target.Equals(DelegateNode))
                {
                    adjacents.Add(edge.Source);
                }
                */
        /*        }
    
            return adjacents;
        }
 
        public static double[,] buildAdjMatrix(AdjacencyGraph<KLnode, KLEdge> graph)
        {
            var row = graph.VertexCount;
            double[,] adjMatrix = new double[row, row];

            foreach(var vertex in graph.Vertices)
            {
                
                var adj = getAdjacents(vertex, graph);
                foreach (var adjVertex in adj)
                {
                    adjMatrix[vertex.Id, adjVertex.Id] = 1;
                }
            }

            return adjMatrix;
        }

        public static bool isEquivalent(KLnode v1, AdjacencyGraph<KLnode, KLEdge> graph1, KLnode v2, AdjacencyGraph<KLnode, KLEdge> graph2)
        {
            int v1Adj = getAdjacents(v1, graph1).Count;
            int v2Adj = getAdjacents(v2, graph2).Count;
            if (v1Adj >= v2Adj)
            {
                return true;
            }
            return false;
        }
    */

        #endregion


        public static int GetComponentTypeEquivalentNameInDb(string categoryNameInDB)
        {
            int componentType = -1;

            switch (categoryNameInDB)
            {
                case "Bearing":
                    componentType = (int)KLComponentType.Bearing;
                    break;
                case "C-Clip":
                    componentType = (int)KLComponentType.CClip;
                    break;
                case "Cylinder_Like":
                    componentType = (int)KLComponentType.Cylinder;
                    break;
                case "Cube_Like":
                    componentType = (int)KLComponentType.Cube;
                    break;
                case  "Gears":
                    componentType = (int)KLComponentType.Gear;
                    break;
                case "Key":
                    componentType = (int)KLComponentType.Key;
                    break;
                case "LinkageArm":
                    componentType = (int)KLComponentType.LinkageArm;
                    break;
                case "Nuts":
                    componentType = (int)KLComponentType.Nut;
                    break;
                case "PoB":
                    componentType = (int)KLComponentType.PartOfBearing;
                    break;
                case "ScrewsNBolts":
                    componentType = (int)KLComponentType.Screw;
                    break;
                case "Axis":
                    componentType = (int)KLComponentType.Shaft;
                    break;
                case "Spacer":
                    componentType = (int)KLComponentType.Spacer;
                    break;
                case "Sphere_Like":
                    componentType = (int)KLComponentType.Sphere;
                    break;
                case "Torus":
                    componentType = (int)KLComponentType.Torus;
                    break;

                default:
                    componentType = (int)KLComponentType.Miscellaneous;
                    break;
            }
            return componentType;
        }


        public static List<KLedgeContact> GetEquivalentClassesOfCylindricalContact(
            AdjacencyGraph<KLnode, KLedge> startingGraph, KLnode partOfSubAss, SldWorks SwApplication)
        {
            var outJointPartList = new List<KLedgeContact>();
            var allOutJointPartList = new List<KLedge>();
            var equivalentContactList = new List<KLedgeContact>();

            allOutJointPartList = (KLgraph.GetIncidentArcsForType(startingGraph, partOfSubAss,
                typeof(KLgraph.KLedgeContact)));

            if (allOutJointPartList.Any())
            {
                outJointPartList.AddRange(allOutJointPartList.Cast<KLgraph.KLedgeContact>().ToList());
            }

            //if (partOfSubAss.Name.Contains("Part.Chapeau"))
            //{
            //var toPrint = "";
            //foreach (var kLedge in outJointPartList)
            //{
            //    toPrint += kLedge.Source.ComponentPath + " - " + kLedge.Target.ComponentPath;
            //}
            //SwApplication.SendMsgToUser(KLgraph.GetIncidentArcsForType(startingGraph, partOfSubAss,
            //typeof(KLgraph.KLedgeContact)).Count.ToString());
            //}

            if (outJointPartList.Any())
            {
                outJointPartList = outJointPartList.FindAll(
                    edge => edge.FaceContacType == KLgraph.KLfaceContactType.Cylinder);
            }

            foreach (KLedgeContact contact in outJointPartList)
            {
                var isEquivalent =
                    equivalentContactList.Find(
                        edge =>
                            (Math.Abs(edge.Radius - contact.Radius) < 0.001 &&
                             ((edge.Dof.Rotation.First())).Equals(contact.Dof.Rotation.First())));

                //if (isEquivalent == null)
                {
                    var otherContact = contact.GetOtherVertex(partOfSubAss);
                    if (!otherContact.Name.Contains("ArretSortie") && !otherContact.Name.Contains("ArretEntr"))
                    {
                        equivalentContactList.Add(contact);
                    }
                }
            }
            //if (partOfSubAss.Name.Contains("PRT_RoulementEntree"))
            //{
            //    SwApplication.SendMsgToUser(outJointPartList.Count + "  " + equivalentContactList.Count);
            //}
            return equivalentContactList;
        }

        public static List<KLedge> GetIncidentArcsForType(AdjacencyGraph<KLnode, KLedge> startingGraph,
            KLnode partOfSubAss, Type type)
        {
            var outJointPartList = new List<KLedge>();
            IEnumerable<KLedge> outInterferencesSupport;
            IEnumerable<KLedge> inInterferencesSupport;

            foreach (var node in startingGraph.Vertices)
            {
                startingGraph.TryGetEdges(node, partOfSubAss, out outInterferencesSupport);

                foreach (KLgraph.KLedge outedge in outInterferencesSupport)
                {
                    outJointPartList.Add(outedge);
                }

                startingGraph.TryGetEdges(partOfSubAss, node, out inInterferencesSupport);

                foreach (KLgraph.KLedge inedge in inInterferencesSupport)
                {
                    outJointPartList.Add(inedge);
                }
            }

            outJointPartList.RemoveAll(arc => arc.GetType() != type);
            return outJointPartList;
        }

        public static List<KLedge> GetIncidentArcs(AdjacencyGraph<KLnode, KLedge> startingGraph, KLnode partOfSubAss)
        {
            var outJointPartList = new List<KLedge>();
            IEnumerable<KLedge> outInterferencesSupport;
            IEnumerable<KLedge> inInterferencesSupport;


            foreach (var node in startingGraph.Vertices)
            {
                startingGraph.TryGetEdges(node, partOfSubAss, out outInterferencesSupport);
                outJointPartList.AddRange(outInterferencesSupport);

                startingGraph.TryGetEdges(partOfSubAss, node, out inInterferencesSupport);
                outJointPartList.AddRange(inInterferencesSupport);
            }
            return outJointPartList;
        }


        public static List<KLedge> GetArcsBetweenNodes(AdjacencyGraph<KLnode, KLedge> originalGraph, KLnode firstOriginalNode,
            KLnode firstComparisonNode)
        {
            // Don't remove the oppositeEdge. Even if klEdge is undirect, the graph is an adjacent graph,
            // thus it is not able to retrieve the edges between nodes without considering the directcion!
            IEnumerable<KLgraph.KLedge> oedgeFirst;
            IEnumerable<KLgraph.KLedge> oedgeFirstOpposite;

            originalGraph.TryGetEdges(firstOriginalNode, firstComparisonNode,
                out oedgeFirst);
            originalGraph.TryGetEdges(firstComparisonNode, firstOriginalNode,
                out oedgeFirstOpposite);

            var arcListFirstGraph = new List<KLgraph.KLedge>();
            arcListFirstGraph.AddRange(oedgeFirstOpposite);
            arcListFirstGraph.AddRange(oedgeFirst);
            return arcListFirstGraph;
        }

        public static List<KLnode> GetAdjacentNodesForEdgeType(AdjacencyGraph<KLnode, KLedge> startingGraph,
            KLnode partOfSubAss, Type edgeType)
        {
            var outJointPartList = new List<KLnode>();
            IEnumerable<KLedge> outInterferencesSupport;
            IEnumerable<KLedge> inInterferencesSupport;

            foreach (var node in startingGraph.Vertices)
            {
                startingGraph.TryGetEdges(node, partOfSubAss, out outInterferencesSupport);

                foreach (KLgraph.KLedge outedge in outInterferencesSupport)
                {
                    if (outedge.GetType() == edgeType)
                    {
                        var adjacentNode = outedge.GetOtherVertex(partOfSubAss);
                        outJointPartList.Add(adjacentNode);
                    }
                }


                startingGraph.TryGetEdges(partOfSubAss, node, out inInterferencesSupport);

                foreach (KLgraph.KLedge inedge in inInterferencesSupport)
                {
                    if (inedge.GetType() == edgeType)
                    {
                        var adjacentNode = inedge.GetOtherVertex(partOfSubAss);
                        outJointPartList.Add(adjacentNode);
                    }
                }
            }
            return outJointPartList;
        }

        public static List<KLnode> GetAdjacentNodes(AdjacencyGraph<KLnode, KLedge> startingGraph, KLnode partOfSubAss)
        {
            var outJointPartList = new List<KLnode>();
            IEnumerable<KLedge> outInterferencesSupport;
            IEnumerable<KLedge> inInterferencesSupport;


            foreach (var node in startingGraph.Vertices)
            {
                startingGraph.TryGetEdges(node, partOfSubAss, out outInterferencesSupport);

                foreach (KLgraph.KLedge outedge in outInterferencesSupport)
                {
                    var adjacentNode = outedge.GetOtherVertex(partOfSubAss);
                    outJointPartList.Add(adjacentNode);
                }


                startingGraph.TryGetEdges(partOfSubAss, node, out inInterferencesSupport);

                foreach (KLgraph.KLedge inedge in inInterferencesSupport)
                {
                    var adjacentNode = inedge.GetOtherVertex(partOfSubAss);
                    outJointPartList.Add(adjacentNode);
                }
            }
            return outJointPartList;
        }

        public static void GetSubGraphFromVertices(AdjacencyGraph<KLnode, KLedge> graph,
            ref AdjacencyGraph<KLnode, KLedge> subGraph)
        {
            foreach (var node in subGraph.Vertices)
            {
                foreach (var node2 in subGraph.Vertices)
                {
                    IEnumerable<KLedge> outEdges;
                    graph.TryGetEdges(node, node2, out outEdges);

                    foreach (KLedge edge in outEdges)
                    {
                        if (!subGraph.ContainsEdge(edge))
                        {
                            subGraph.AddEdge(edge);
                        }
                    }

                    IEnumerable<KLedge> outEdgesOpposite;
                    graph.TryGetEdges(node2, node, out outEdgesOpposite);

                    foreach (KLedge edge in outEdgesOpposite)
                    {
                        if (!subGraph.ContainsEdge(edge))
                        {
                            subGraph.AddEdge(edge);
                        }
                    }
                }
            }
        }
        
        /*
        public static MyVertex GetNodeCentroid(AdjacencyGraph<KLnode, KLedge> graph, string nodeName)
        {
            var queryRoot = (KLgraph.KLnodeAssembly)graph.Vertices.First();
            foreach (var instance in queryRoot.Instances)
            {
                var repComponent = instance.ListOfMyComponent.Find(repComp => repComp.Name == nodeName);
                if (repComponent != null)
                {
                    var firstCentroid = repComponent.Origin;
                    return firstCentroid;
                }
            }

            return null;
        }

        public static MyVertex GetNodeCentroid(KLnodeAssembly root, string nodeName)
        {
            foreach (var instance in root.Instances)
            {
                var repComponent = instance.ListOfMyComponent.Find(repComp => repComp.Name == nodeName);
                if (repComponent != null)
                {
                    var firstCentroid = repComponent.Origin;
                    return firstCentroid;
                }
            }

            return null;
        }

      */
        #region Equivalent DelegateNode commputation

        public class EquivalentNode
        {
            protected bool Equals(EquivalentNode other)
            {
                return Equals(DelegateNode, other.DelegateNode) || NodesOfThisClass.Contains(other.DelegateNode);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((EquivalentNode) obj);
            }

            public override int GetHashCode()
            {
                return (DelegateNode != null ? DelegateNode.GetHashCode() : 0);
            }

            public KLnode DelegateNode;
            public List<KLnode> NodesOfThisClass = new List<KLnode>();
            public List<KLnode> Joint = new List<KLnode>();


            public EquivalentNode()
            {
            }

            public EquivalentNode(KLnode delegateNode, List<KLnode> joint)
            {
                DelegateNode = delegateNode;
                Joint = joint;
                NodesOfThisClass.Add(delegateNode);
            }

            public bool IsEquivalentNode(EquivalentNode otherNode)
            {
                var jointCopy = new List<KLnode>();
                jointCopy.AddRange(Joint);
                var otherJointCopy = new List<KLnode>();
                otherJointCopy.AddRange(otherNode.Joint);

                foreach (var thisNode in Joint)
                {
                    var correspondingJoint = (KLnode) otherJointCopy.Find(n => n.Equals(thisNode));
                    if (correspondingJoint != null)
                    {
                        jointCopy.Remove(thisNode);
                        otherJointCopy.Remove(correspondingJoint);
                    }
                }

                if (!jointCopy.Any() && !otherJointCopy.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public class JointsRepeatedParts
        {
            protected bool Equals(JointsRepeatedParts other)
            {
                return Equals(Node, other.Node);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((JointsRepeatedParts) obj);
            }

            public override int GetHashCode()
            {
                return (Node != null ? Node.GetHashCode() : 0);
            }

            public KLnode Node;
            public int Repetition;

            public JointsRepeatedParts()
            {
            }

            public JointsRepeatedParts(KLnode node, int repetition)
            {
                Node = node;
                Repetition = repetition;
            }
        }

        #endregion

        /*
        public static List<KLnodePart> GetBearingAsPart(
            AdjacencyGraph<KLnode, KLedge> storedGraph, List<KLnode> nodeList,
            out int totalRoulement)
        {
            totalRoulement = 0;
            // Rimuovo i nodi che non sono parti
            nodeList.RemoveAll(node => node.GetType() != typeof(KLgraph.KLnodePart));
            var partList = new List<KLnodePart>(nodeList.Cast<KLgraph.KLnodePart>());

            partList.RemoveAll(node => node.Name.Contains("Circlips"));

            // Rimuovo i nodi con genere diverso da 1
            partList.RemoveAll(part => part.KLstatistic.Genus != 1);

            // Rimuovo i nodi che non hanno esattamente due contatti cilindrici con la stessa superficie
            // questo implicata che i contatti possono essere anche tre, ma le sup solo 2.
            //partList.RemoveAll(
            //    part =>
            //        ((((KLgraph.GetIncidentArcsForType(storedGraph, part,
            //            typeof (KLgraph.KLedgeContact)))
            //            .Cast<KLgraph.KLedgeContact>().ToList()).Where(
            //                edge => edge.FaceContacType == KLgraph.KLfaceContactType.Cylinder))
            //            .Count()) !=
            //        2);

            //SwTaskPaneHost.SwApplication.SendMsgToUser("Calcolo equivalenze " + partList.Count);
            partList.RemoveAll(
                part =>
                    KLgraph.GetEquivalentClassesOfCylindricalContact(storedGraph, part, SwTaskPaneHost.SwApplication)
                        .Count != 2);
            //SwTaskPaneHost.SwApplication.SendMsgToUser("Equivalenze " + partList.Count);

            // I contatti cilindrici devono appartenere a due parti diverse (che non siano c-clips, dovrebbero essere eliminate prima)
            // e avere raggio diverso
            var partListCopy = new List<KLnodePart>(partList);
            foreach (var part in partListCopy)
            {
                var arcs = new List<KLedgeContact>();
                //arcs.AddRange(
                //    (((KLgraph.GetIncidentArcsForType(storedGraph, part,
                //        typeof (KLgraph.KLedgeContact)))
                //        .Cast<KLgraph.KLedgeContact>().ToList()).Where(
                //            edge => edge.FaceContacType == KLgraph.KLfaceContactType.Cylinder)));
                arcs.AddRange(
                    KLgraph.GetEquivalentClassesOfCylindricalContact(storedGraph, part, SwTaskPaneHost.SwApplication)
                        .Cast<KLgraph.KLedgeContact>());

                var firstDestination = arcs[0].GetOtherVertex((KLgraph.KLnode) part);
                var secondDestination = arcs[1].GetOtherVertex((KLgraph.KLnode) part);
                IEnumerable<KLedge> outedge;
                IEnumerable<KLedge> inedge;

                storedGraph.TryGetEdges(firstDestination, secondDestination, out outedge);
                storedGraph.TryGetEdges(secondDestination, firstDestination, out inedge);

                var edgeBetweenExternalParts = new List<KLedge>();
                edgeBetweenExternalParts.AddRange(outedge);
                edgeBetweenExternalParts.AddRange(inedge);

                if (firstDestination.Equals(secondDestination) ||
                    firstDestination.Name.Contains("Circlip") ||
                    firstDestination.Name.Contains("ArretSortie") ||
                    firstDestination.Name.Contains("ArretEntr") ||
                    secondDestination.Name.Contains("Circlip") ||
                    secondDestination.Name.Contains("ArretSortie") ||
                    secondDestination.Name.Contains("ArretEntr"))
                {
                    partList.Remove(part);
                }
                else if (edgeBetweenExternalParts.Any())
                {
                    partList.Remove(part);
                }
                else
                {
                    if (Math.Abs(arcs[0].Radius - arcs[1].Radius) > 0.001)
                    {
                        // Prima lo usavo per colorare la parte, ma ora l'identificazione non sfrutta questa colorazione

                        //var partComponent = componentList.Find(comp => comp.Name2 == part.Name);
                        //if (partComponent != null)
                        //{
                        //    Utility.ColorModel.KLColorPart(modelDoc, partComponent, 1);
                        //}
                        //else
                        //{
                        //    SwTaskPaneHost.SwApplication.SendMsgToUser("Parte non colorata");
                        //}
                    }
                    else
                    {
                        partList.Remove(part);
                    }
                }
            }
            return partList;
        }

        public static List<KLnode> GetListOfChildrenOfAssemblyNode(KLgraph.KLnodeAssembly testAssembly, AdjacencyGraph<KLgraph.KLnode, KLgraph.KLedge> graph, string modelId = "")
        {
                    var conn = new MySqlConnection("SERVER=localhost;database=imati;uid=IMATI;password=@partclassifier2017");
                    conn.Open();

            var nodeList = new List<KLnode>();
            IEnumerable<KLgraph.KLedge> outSubEdges;
            graph.TryGetOutEdges(testAssembly, out outSubEdges);
            foreach (var outEdge in outSubEdges)
            {
                if (outEdge.GetType() == typeof(KLgraph.KLedgeStructure))
                {
                    var targetNode = outEdge.Target;

                    Regex rgx = new Regex("[^a-zA-Z0-9]");
                    var nodeCategory = "";
                    var comparisonNodeNameDB =
                        targetNode.Name.Substring(0, targetNode.Name.LastIndexOf('-')).Split('/').Last();
                    comparisonNodeNameDB = rgx.Replace(comparisonNodeNameDB, "");
                    comparisonNodeNameDB.Replace("'", "''");
                    comparisonNodeNameDB.Replace(" ", String.Empty);

                    var query = String.Format("SELECT * FROM am_{0} where NewName LIKE \"{1}\"", modelId,
                        comparisonNodeNameDB);
                    var queryCmd = new MySqlCommand(query, conn);

                    var readerQuery = queryCmd.ExecuteReader();
                    while (readerQuery != null && readerQuery.Read())
                    {
                        nodeCategory = readerQuery["Category"].ToString();
                    }
                    readerQuery.Close();
                    var componentType = KLgraph.GetComponentTypeEquivalentNameInDb(nodeCategory);
                    targetNode.ComponentType = componentType;


                    nodeList.Add(targetNode);
                }
            }
            return nodeList;
        }
        */
    }
}