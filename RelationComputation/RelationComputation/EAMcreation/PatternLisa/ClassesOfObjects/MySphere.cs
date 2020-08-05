using System;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Classe sfera: equazione cartesiana generica x^2 + y^2 + z^2 + ax + by + cz + d = 0, t.c. a^2 + b^2 + c^2 -4d > 0

    public class MySphere
    {
        public double a;
        public double b;
        public double c;
        public double d;
        public double[] centerSphere;
        public double radiusSphere;

        public MySphere()
        {
        }

        public MySphere(double A, double B, double C, double D)
        {
            this.a = A;
            this.b = B;
            this.c = C;
            this.d = D;
            double[] CenterSphere = {-A / 2, -B / 2, -C / 2};
            this.centerSphere = CenterSphere;
            this.radiusSphere = Math.Sqrt(Math.Pow(A, 2) / 4 + Math.Pow(B, 2) / 4 + Math.Pow(C, 2) / 4 - D);
        }

        public MySphere(double[] centerSphere, double radiusSphere)
        {
            this.a = -2 * centerSphere[0];
            this.b = -2 * centerSphere[1];
            this.c = -2 * centerSphere[2];
            this.d = Math.Pow(centerSphere[0], 2) + Math.Pow(centerSphere[1], 2) + Math.Pow(centerSphere[2], 2) - Math.Pow(radiusSphere, 2);
            this.centerSphere = centerSphere;
            this.radiusSphere = radiusSphere;
        }

        protected bool Equals(MySphere other)
        {
            return Equals(centerSphere, other.centerSphere) && radiusSphere.Equals(other.radiusSphere);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((centerSphere != null ? centerSphere.GetHashCode() : 0) * 397) ^ radiusSphere.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((MySphere)obj);
        }
  }


}
