using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a component2, equipped with the corresponding transform matrix
    //(which is the transform matrix respect to the main root)
    [JsonObject ("RepeatedComponent")]
    public class MyRepeatedComponent
    {
        [JsonIgnore] public Component2 Component;
        [JsonProperty("Name")] public String Name;
        [JsonProperty("IdCorrespondingNode")] public int IdCorrespondingNode;
        [JsonIgnore] public MyTransformMatrix Transform;
        [JsonIgnore] public bool IsLeaf;   // it is TRUE if the component is a partDoc, it is FALSE otherwise
        [JsonProperty("Origin")] public MyVertex Origin;
        [JsonIgnore] public MyRepeatedEntity RepeatedEntity;
        [JsonProperty("IsSphere")] public bool IsSphere;

        public MyRepeatedComponent()
        {
        }

        public MyRepeatedComponent(Component2 component, MyTransformMatrix transform, bool isLeaf, MyRepeatedEntity repeatedEntity)
        {
            this.Component = component;
            var pathComponent = component.Name2;
            var componentName = pathComponent.Split('/').Last();
            Name = componentName;
            this.Transform = transform;
            this.IsLeaf = isLeaf;

            var originVertex = new MyVertex(transform.TranslationVector[0], transform.TranslationVector[1],
                transform.TranslationVector[2]);


            this.Origin = originVertex;
            RepeatedEntity = repeatedEntity;

            IsSphere = false;
        }

        public MyRepeatedComponent(Component2 component, int idCorrespondingNode, MyTransformMatrix transform, bool isLeaf, MyRepeatedEntity repeatedEntity)
        {
            this.Component = component;
            Name = component.Name2;
            IdCorrespondingNode = idCorrespondingNode;
            this.Transform = transform;
            this.IsLeaf = isLeaf;
      
            var originVertex = new MyVertex(transform.TranslationVector[0], transform.TranslationVector[1],
                transform.TranslationVector[2]);
          
                
            this.Origin = originVertex;
            RepeatedEntity = repeatedEntity;

            IsSphere = false;
        }
    }



    //Class representing a list of MyRepeatedComponent corresponding to the same
    //file object (part or assembly), so the set of all the instances of it
    public class MyListOfInstances
    {
        public string Name;  //name of the Part file
        public List<MyRepeatedComponent> ListOfMyComponent;

        public MyListOfInstances()
        {
        }

        public MyListOfInstances(string name, List<MyRepeatedComponent> listOfMyComponent)
        {
            this.Name = name;
            this.ListOfMyComponent = listOfMyComponent;
        }
    }
}