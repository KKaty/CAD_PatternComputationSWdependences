using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Classe circonferenza: intersezione di un piano e una sfera
    public class MyCircumForPath : MyPathGeometricObject
    {
        public MyPlane circumplane;
        public MySphere circumsphere;
        public MyVertex circumcenter;

        public MyCircumForPath()
        {
        }

        public MyCircumForPath(MyPlane CircumPlane, MySphere circumMySphere)
        {
            this.circumplane = CircumPlane;
            this.circumsphere = circumMySphere;

            //computation of the circle center:
            double[] centerSphereVector =
            {
                this.circumsphere.centerSphere[0],
                this.circumsphere.centerSphere[1], this.circumsphere.centerSphere[2]
            };
            
            var centerSphereOfCircumMyVertex = new MyVertex(centerSphereVector[0], centerSphereVector[1], centerSphereVector[2]);
            double[] planeNormal =
            {
                this.circumplane.a, 
                this.circumplane.b,
                this.circumplane.c
            };

            var pointOnAxisMyVertex = new MyVertex(centerSphereOfCircumMyVertex.x + planeNormal[0],
                centerSphereOfCircumMyVertex.y + planeNormal[1], centerSphereOfCircumMyVertex.z + planeNormal[2]);

            var rotationAxis = FunctionsLC.LinePassingThrough(centerSphereOfCircumMyVertex, pointOnAxisMyVertex);

            this.circumcenter = FunctionsLC.PointIntersectionOfPlaneAndLine(this.circumplane,
                rotationAxis);

        }
    }
}
