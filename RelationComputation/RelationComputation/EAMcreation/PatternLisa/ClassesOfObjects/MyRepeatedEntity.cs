using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class for a repeated entity (set of selected faces from the model)
    public class MyRepeatedEntity
    {
        public int idRE = -1;
        public List<Face2> listOfFaces;                 //list of faces of the repeated entity
        public MyVertex centroid;                       //centroid of the repeated entity
        public List<MyVertex> listOfVertices;           //list of vertices of the repeated entity (from BRep)
        public List<MyVertex> listOfAddedVertices;      //list of vertices added to curved edge (both closed and open)

        public List<MyPlane> listOfPlanarSurfaces;
        public List<MySphere> listOfSphericalSurfaces;
        public List<MyCone> listOfConicalSurfaces; 
        public List<MyCylinder> listOfCylindricalSurfaces;
        public List<MyTorus> listOfToroidalSurfaces;

        public double[,] transformationMatrix;
        public MyRepeatedEntity()
        {
        }

        //public MyRepeatedEntity(int IdRE, List<Face2> ListOfFaces, MyVertex Centroid, List<MyVertex> ListOfVerteces, List<MyVertex> ListOfAddedVertices,
        //                        List<MyPlane> ListOfPlanarSurfaces = null, List<MySphere> ListOfSphericalSurfaces = null, List<MyCone> ListOfConicalSurfaces = null,
        //                        List<MyCylinder> ListOfCylindricalSurfaces = null, List<MyTorus> ListOfToroidalSurfaces = null)
        public MyRepeatedEntity(int IdRE, List<Face2> ListOfFaces, MyVertex Centroid, List<MyVertex> ListOfVertices, List<MyVertex> ListOfAddedVertices,
                                List<MyPlane> ListOfPlanarSurfaces, List<MySphere> ListOfSphericalSurfaces, List<MyCone> ListOfConicalSurfaces,
                                List<MyCylinder> ListOfCylindricalSurfaces, List<MyTorus> ListOfToroidalSurfaces)
        {
            this.idRE = IdRE;
            this.listOfFaces = ListOfFaces;
            this.centroid = Centroid;
            this.listOfVertices = ListOfVertices;
            this.listOfAddedVertices = ListOfAddedVertices;
            this.listOfPlanarSurfaces = ListOfPlanarSurfaces;
            this.listOfSphericalSurfaces = ListOfSphericalSurfaces;
            this.listOfConicalSurfaces = ListOfConicalSurfaces;
            this.listOfCylindricalSurfaces = ListOfCylindricalSurfaces;
            this.listOfToroidalSurfaces = ListOfToroidalSurfaces;
            this.transformationMatrix = null;
        }

        public MyRepeatedEntity(int IdRE, List<Face2> ListOfFaces, MyVertex Centroid, List<MyVertex> ListOfVertices, List<MyVertex> ListOfAddedVertices,
                             List<MyPlane> ListOfPlanarSurfaces, List<MySphere> ListOfSphericalSurfaces, List<MyCone> ListOfConicalSurfaces,
                             List<MyCylinder> ListOfCylindricalSurfaces, List<MyTorus> ListOfToroidalSurfaces, double[,] TransformationMatrix)
        {
            this.idRE = IdRE;
            this.listOfFaces = ListOfFaces;
            this.centroid = Centroid;
            this.listOfVertices = ListOfVertices;
            this.listOfAddedVertices = ListOfAddedVertices;
            this.listOfPlanarSurfaces = ListOfPlanarSurfaces;
            this.listOfSphericalSurfaces = ListOfSphericalSurfaces;
            this.listOfConicalSurfaces = ListOfConicalSurfaces;
            this.listOfCylindricalSurfaces = ListOfCylindricalSurfaces;
            this.listOfToroidalSurfaces = ListOfToroidalSurfaces;
            this.transformationMatrix = TransformationMatrix;
        }
    }
}
