using System;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    public class MyTorus
    {
        public double[] centerTorus;
        public MyLine axisTorus;
        public double majorRadiusTorus;
        public double minorRadiusTorus;

        public MyTorus()
        {
        }

        public MyTorus(double[] CenterTorus, double[] AxisVectorTorus, double MajorRadiusTorus, double MinorRadiusTorus)
        {
            this.centerTorus = CenterTorus;
            this.axisTorus = FunctionsLC.ConvertPointPlusDirectionInMyLine(CenterTorus, AxisVectorTorus);
            this.majorRadiusTorus = MajorRadiusTorus;
            this.minorRadiusTorus = MinorRadiusTorus;
        }

        

        protected bool Equals(MyTorus other)
        {
            double tolerance = Math.Pow(10, -5);
            var centerTorusEquality = (Math.Abs(centerTorus[0] - other.centerTorus[0]) < tolerance &&
                                       Math.Abs(centerTorus[1] - other.centerTorus[1]) < tolerance &&
                                       Math.Abs(centerTorus[2] - other.centerTorus[2]) < tolerance);
            return centerTorusEquality && axisTorus.Equals(other.axisTorus) &&
                Math.Abs(majorRadiusTorus - other.majorRadiusTorus) < tolerance && Math.Abs(minorRadiusTorus - other.minorRadiusTorus) < tolerance;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (centerTorus != null ? centerTorus.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (axisTorus != null ? axisTorus.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ majorRadiusTorus.GetHashCode();
                hashCode = (hashCode * 397) ^ minorRadiusTorus.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyTorus) obj);
        }
    }
}
