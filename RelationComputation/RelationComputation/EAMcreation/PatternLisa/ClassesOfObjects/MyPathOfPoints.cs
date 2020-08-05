using System.Collections.Generic;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class of path of centroids
    public class MyPathOfPoints
    {
        public List<int> path;
        public MyPathGeometricObject pathGeometricObject;

        public MyPathOfPoints()
        {

        }

        public MyPathOfPoints(List<int> Path, MyPathGeometricObject PathGeometricObject)
        {
            this.path = Path;
            this.pathGeometricObject = PathGeometricObject;
        }

        public void prova()
        {
            if (pathGeometricObject.GetType() == typeof (MyLine))
            {

            }
        }
    }
}

