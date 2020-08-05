using System.Text;
using AssemblyRetrieval.Debug;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //Restituisce le equazioni cartesiane della circonferenza passante per V1, V2, V3
        public static MyCircumForPath CircumPassingThrough(MyVertex V1, MyVertex V2, MyVertex V3, ref StringBuilder fileOutput, ModelDoc2 SwModel, SldWorks swApplication)
        {
    
            //The circumference does not exist if the 3 points lie on the same line or if 2 of them are coincident.
            if (V1.Lieonline(LinePassingThrough(V2, V3)) || V1.Equals(V2) || V1.Equals(V3) || V2.Equals(V3))
            {
                MyCircumForPath OutputCircum = new MyCircumForPath();
                return OutputCircum;
            }
            else
            {
                MyPlane CircumPlane = PlanePassingThrough(V1, V2, V3);

                //There is a unique sphere passing through 4 points: we must arbitrarily choose a fourth point not lying on the V1-V2-V3 plane
                //So we choose the point resulting from adding the normal direction to a point lying on the V1-V2-V3 plane, for example V1
                
                MyVertex V4 = new MyVertex(V1.x + CircumPlane.a * 10, V1.y + CircumPlane.b * 10, V1.z + CircumPlane.c * 10);
                //Vertex V4 = new Vertex(0, 0, 0);
                //if (SwModel != null)
                //{
                //    SwModel = swApplication.ActiveDoc;
                //    SwModel.ClearSelection2(true);
                //    SwModel.Insert3DSketch();
                //    SwModel.CreatePoint2(V1.x, V1.y, V1.z);
                //    SwModel.InsertSketch();
                //    SwModel.CreatePoint2(V2.x, V2.y, V2.z);
                //    SwModel.InsertSketch();
                //    SwModel.CreatePoint2(V3.x, V3.y, V3.z);
                //    SwModel.InsertSketch();
                //    SwModel.CreatePoint2(V4.x, V4.y, V4.z);                    
                //    SwModel.InsertSketch();
                //}

                MySphere CircumSphere = SpherePassingThrough(V1, V2, V3, V4, ref fileOutput, SwModel);

                if (CircumSphere.centerSphere == null)
                {
                    KLdebug.Print("CircumSphere.centerSphere nullo", "CircumPassingThrough.txt");
                }
                else if (CircumSphere.centerSphere != null)
                {
                    MyCircumForPath OutputCircum = new MyCircumForPath(CircumPlane, CircumSphere);
                    return OutputCircum;
                }

            }
            MyCircumForPath OutputCircumNull = new MyCircumForPath();
            return OutputCircumNull;
         }
    }
}
