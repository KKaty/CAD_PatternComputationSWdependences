using System;
using SolidWorks.Interop.sldworks;
using Matrix = Accord.Math.Matrix;

namespace AssemblyRetrieval.Utility
{
    public static class GeometryUtilities
    {
        public static bool samePlane(Face2 firstFace, Face2 secondFace, SldWorks swApp)
        {

            var firstSurf = (Surface) firstFace.GetSurface();
            var secondSurf = (Surface) secondFace.GetSurface();

            var firstParameters = (Array) firstSurf.PlaneParams;
            var secondParameters = (Array) secondSurf.PlaneParams;

            var firstNormal = new double[3];
            var firstPoint = new double[3];

            var secondNormal = new double[3];
            var secondPoint = new double[3];

            Array.Copy(firstParameters, 0, firstNormal, 0, 3);
            Array.Copy(firstParameters, 3, firstPoint, 0, 3);

            Array.Copy(secondParameters, 0, secondNormal, 0, 3);
            Array.Copy(secondParameters, 3, secondPoint, 0, 3);

            if (!firstFace.FaceInSurfaceSense())
            {
                firstNormal.SetValue(-(double) firstNormal.GetValue(0), 0);
                firstNormal.SetValue(-(double) firstNormal.GetValue(1), 1);
                firstNormal.SetValue(-(double) firstNormal.GetValue(2), 2);
            }

            if (!secondFace.FaceInSurfaceSense())
            {
                secondNormal.SetValue(-(double) secondNormal.GetValue(0), 0);
                secondNormal.SetValue(-(double) secondNormal.GetValue(1), 1);
                secondNormal.SetValue(-(double) secondNormal.GetValue(2), 2);
            }

            var results = Math.Abs(Matrix.InnerProduct(firstNormal, secondNormal) - 1);
            var normalPrint = String.Format("{0} {1} {2} --- {3} {4} {5} = {6}",
                firstNormal[0], firstNormal[1], firstNormal[2],
                secondNormal[0], secondNormal[1], secondNormal[2], results);
                     
            var firstEquation = new double[4]
            {
                (double) firstNormal.GetValue(0), (double) firstNormal.GetValue(1), (double) firstNormal.GetValue(2),
                -(double) firstNormal.GetValue(0)*(double) firstPoint.GetValue(0) -
                (double) firstNormal.GetValue(1)*(double) firstPoint.GetValue(1) -
                (double) firstNormal.GetValue(2)*(double) firstPoint.GetValue(2),
            };
            var secondEquation = new double[4]
            {
                (double) secondNormal.GetValue(0), (double) secondNormal.GetValue(1), (double) secondNormal.GetValue(2),
                -(double) secondNormal.GetValue(0)*(double) secondPoint.GetValue(0) -
                (double) secondNormal.GetValue(1)*(double) secondPoint.GetValue(1) -
                (double) secondNormal.GetValue(2)*(double) secondPoint.GetValue(2),
            };


             var equationPrint = String.Format("Eq: {0}x {1}y {2}z = {3}",
                firstEquation[0], firstEquation[1], firstEquation[2], firstEquation[3]);
            /*
            if (Math.Abs(Accord.Math.Matrix.InnerProduct(firstNormal, secondNormal) - 1) < 0.001)
            {
                swApp.SendMsgToUser("Normale uguale");
                return false;
            }
            */
            //return firstEquation.Equals(secondFace);
            if (Math.Abs(firstEquation[0] - secondEquation[0]) < 0.01 &&
                Math.Abs(firstEquation[1] - secondEquation[1]) < 0.01 &&
                Math.Abs(firstEquation[2] - secondEquation[2]) < 0.01 &&
                Math.Abs(firstEquation[3] - secondEquation[3]) < 0.01)
            {
                swApp.SendMsgToUser("Equazione uguale");
                return true;
            }
            return false;

        }

        public static bool sameCylinder(Surface firstSurf, Surface secondSurf)
        {
            double[] firstParameters = firstSurf.CylinderParams;
            double[] firstOrigin = new double[3];
            double[] firstAxes = new double[3];
            Array.Copy(firstParameters, 0, firstOrigin, 0, 3);
            Array.Copy(firstParameters, 3, firstAxes, 0, 3);
            var firstRay = (double)firstParameters.GetValue(6);

            double[] secondParameters = secondSurf.CylinderParams;
            double[] secondOrigin = new double[3];
            double[] secondAxes = new double[3];
            Array.Copy(secondParameters, 0, secondOrigin, 0, 3);
            Array.Copy(secondParameters, 3, secondAxes, 0, 3);
            var secondRay = (double)secondParameters.GetValue(6);


            double[,] matrix =
            {
                {(double)firstAxes.GetValue(0) + (double)firstOrigin.GetValue(0), (double)firstAxes.GetValue(1) + (double)firstOrigin.GetValue(1), (double)firstAxes.GetValue(2) + (double)firstOrigin.GetValue(2)},
                {(double)firstOrigin.GetValue(0), (double)firstOrigin.GetValue(1), (double)firstOrigin.GetValue(2)},
                {(double)secondAxes.GetValue(0) + (double)secondOrigin.GetValue(0), (double)secondAxes.GetValue(1) + (double)secondOrigin.GetValue(1), (double)secondAxes.GetValue(2) + (double)secondOrigin.GetValue(2)},
                {(double)secondOrigin.GetValue(0), (double)secondOrigin.GetValue(1), (double)secondOrigin.GetValue(2)},
            };

            return Matrix.Rank(matrix) == 1 && Math.Abs(firstRay - secondRay) < 0.001;
        }

        public static bool KLIsVectorNull(double[] vector, double toll)
        {
            var status = Math.Abs(vector[0]) < toll && Math.Abs(vector[1]) < toll && Math.Abs(vector[2]) < toll;
            if (status)
            {
                return true;
            }
            return false;
        }
    }
}
