using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;
//using Functions.DataStructure;
//using Functions.Functions;

namespace AssemblyRetrieval.PatternLisa.Part.PartUtilities
{
    public partial class ExtractInfoFromBRep
    {
        public static bool MyEqualsSurface(Surface firstSurf, Surface secondSurf, SldWorks swApplWorks)
        {
            var answer = false;
            
            //var FirstSurface = (Surface)FirstFace.GetSurface();
            var typeOfFirstSurf = firstSurf.Identity();
            //var SecondSurface = (Surface)SecondFace.GetSurface();
            var typeOfSecondSurf = secondSurf.Identity();

            if (typeOfFirstSurf == typeOfSecondSurf)
            {
                if (firstSurf.IsPlane())
                {
                    var firstPlaneParameters = firstSurf.PlaneParams;
                    double[] firstPlaneNormal = { (double)firstPlaneParameters.GetValue(0), (double)firstPlaneParameters.GetValue(1), (double)firstPlaneParameters.GetValue(2) };
                    double[] firstPlanePoint = { (double)firstPlaneParameters.GetValue(3), (double)firstPlaneParameters.GetValue(4), (double)firstPlaneParameters.GetValue(5) };

                    MyPlane firstPlane = new MyPlane(firstPlaneNormal, firstPlanePoint);
                    
                    var secondPlaneParameters = secondSurf.PlaneParams;
                    double[] secondPlaneNormal = { (double)secondPlaneParameters.GetValue(0), (double)secondPlaneParameters.GetValue(1), (double)secondPlaneParameters.GetValue(2) };
                    double[] secondPlanePoint = { (double)secondPlaneParameters.GetValue(3), (double)secondPlaneParameters.GetValue(4), (double)secondPlaneParameters.GetValue(5) };
                    
                    MyPlane secondPlane = new MyPlane(secondPlaneNormal, secondPlanePoint);

                    //string fileNameBuildRepeatedEntity = "buildRepeatedEntity.txt";
                    //string whatToWrite = string.Format("Primo piano: a {0}, b {1}, c {2}, d {3}", firstPlane.a, firstPlane.b, firstPlane.c, firstPlane.d);
                    //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);
                    //whatToWrite = string.Format("Secondo piano: a {0}, b {1}, c {2}, d {3}", secondPlane.a, secondPlane.b, secondPlane.c, secondPlane.d);
                    //KLdebug.Print(whatToWrite, fileNameBuildRepeatedEntity);

                    if (firstPlane.Equals(secondPlane))
                    {
                        answer = true;
                    }
                }

                if (firstSurf.IsSphere())
                {
                    var firstSphereParameters = firstSurf.SphereParams;
                    double[] firstSphereCenter = { (double)firstSphereParameters.GetValue(0), (double)firstSphereParameters.GetValue(1), (double)firstSphereParameters.GetValue(2) };
                    double firstSphereRay = (double)firstSphereParameters.GetValue(3);

                    var firstSphere = new MySphere(firstSphereCenter, firstSphereRay);

                    var secondSphereParameters = secondSurf.SphereParams;
                    double[] secondSphereCenter = { (double)secondSphereParameters.GetValue(0), (double)secondSphereParameters.GetValue(1), (double)secondSphereParameters.GetValue(2) };
                    double secondSphereRay = (double)secondSphereParameters.GetValue(3);

                    var secondSphere = new MySphere(secondSphereCenter, secondSphereRay);

                    if (firstSphere.Equals(secondSphere))
                    {
                        answer = true;
                    }
                }

                if (firstSurf.IsCone())
                {
                    var firstConeParameters = firstSurf.ConeParams;
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(0));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(1));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(2));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(3));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(4));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(5));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(6));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(7));
                    swApplWorks.SendMsgToUser("lunghezza vettore paramentri: " + firstConeParameters.GetValue(8));


                    double[] firstConeOrigin = { (double)firstConeParameters.GetValue(0), (double)firstConeParameters.GetValue(1), (double)firstConeParameters.GetValue(2) };
                    double[] firstConeAxis = { (double)firstConeParameters.GetValue(3), (double)firstConeParameters.GetValue(4), (double)firstConeParameters.GetValue(5) };
                    double firstConeHalfAngle = (double)firstConeParameters.GetValue(8);

                    var firstCone = new MyCone(firstConeOrigin, firstConeAxis, firstConeHalfAngle);

                    var secondConeParameters = secondSurf.ConeParams;
                    double[] secondConeOrigin = { (double)secondConeParameters.GetValue(0), (double)secondConeParameters.GetValue(1), (double)secondConeParameters.GetValue(2) };
                    double[] secondConeAxis = { (double)secondConeParameters.GetValue(3), (double)secondConeParameters.GetValue(4), (double)secondConeParameters.GetValue(5) };
                    double secondConeHalfAngle = (double)secondConeParameters.GetValue(8);

                    var secondCone = new MyCone(secondConeOrigin, secondConeAxis, secondConeHalfAngle);
                    
                    if (firstCone.Equals(secondCone))
                    {
                        // L'EQUALS DEL CONO NON E' PROPRIO GIUSTO.
                        answer = true;
                    }

                }

                if (firstSurf.IsCylinder())
                {
                    double[] firstCylinderParameters = firstSurf.CylinderParams;
                    double[] firstCylinderOrigin = { (double)firstCylinderParameters.GetValue(0), (double)firstCylinderParameters.GetValue(1), (double)firstCylinderParameters.GetValue(2) };
                    double[] firstCylinderAxis = { (double)firstCylinderParameters.GetValue(3), (double)firstCylinderParameters.GetValue(4), (double)firstCylinderParameters.GetValue(5) };
                    double firstCylinderRadius = (double)firstCylinderParameters.GetValue(6);

                    var firstCylinder = new MyCylinder(firstCylinderOrigin, firstCylinderAxis, firstCylinderRadius);
                    
                    double[] secondCylinderParameters = secondSurf.CylinderParams;
                    double[] secondCylinderOrigin = { (double)secondCylinderParameters.GetValue(0), (double)secondCylinderParameters.GetValue(1), (double)secondCylinderParameters.GetValue(2) };
                    double[] secondCylinderAxis = { (double)secondCylinderParameters.GetValue(3), (double)secondCylinderParameters.GetValue(4), (double)secondCylinderParameters.GetValue(5) };
                    double secondCylinderRadius = (double)secondCylinderParameters.GetValue(6);

                    var secondCylinder = new MyCylinder(secondCylinderOrigin, secondCylinderAxis, secondCylinderRadius);

                    if (firstCylinder.Equals(secondCylinder))
                    {
                        // CONTROLLARE L'EQUALS DEL CILINDRO
                        answer = true;
                    }
                }

                if (firstSurf.IsTorus())
                {
                    var firstTorusParameters = firstSurf.TorusParams;
                    double[] firstTorusCenter = { (double)firstTorusParameters.GetValue(0), (double)firstTorusParameters.GetValue(1), (double)firstTorusParameters.GetValue(2) };
                    double[] firstTorusAxis = { (double)firstTorusParameters.GetValue(3), (double)firstTorusParameters.GetValue(4), (double)firstTorusParameters.GetValue(5) };
                    double firstTorusMajorRadius = (double)firstTorusParameters.GetValue(6);
                    double firstTorusMinorRadius = (double)firstTorusParameters.GetValue(7);

                    var firstTorus = new MyTorus(firstTorusCenter, firstTorusAxis, firstTorusMajorRadius, firstTorusMinorRadius);

                    var secondTorusParameters = secondSurf.TorusParams;
                    double[] secondTorusCenter = { (double)secondTorusParameters.GetValue(0), (double)secondTorusParameters.GetValue(1), (double)secondTorusParameters.GetValue(2) };
                    double[] secondTorusAxis = { (double)secondTorusParameters.GetValue(3), (double)secondTorusParameters.GetValue(4), (double)secondTorusParameters.GetValue(5) };
                    double secondTorusMajorRadius = (double)secondTorusParameters.GetValue(6);
                    double secondTorusMinorRadius = (double)secondTorusParameters.GetValue(7);

                    var secondTorus = new MyTorus(secondTorusCenter, secondTorusAxis, secondTorusMajorRadius, secondTorusMinorRadius);

                    if (firstTorus.Equals(secondTorus))
                    {
                        // CONTROLLARE L'EQUALS DEL TORO
                        answer = true;
                    }

                }
            }
            else
            {
                answer = false;
            }

            return answer;
        }     
    }
}
