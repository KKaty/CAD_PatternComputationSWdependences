using System;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Classe piano: equazione cartesiana generica ax + by + cz + d = 0
    public class MyPlane
    {   
        public double a;
        public double b;
        public double c;
        public double d;

        public MyPlane()
        {
        }

        public MyPlane(double A, double B, double C, double D)
        {
            double norm = Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2) + Math.Pow(C, 2));
            this.a = A/norm;
            this.b = B/norm;
            this.c = C/norm;
            this.d = D/norm;
        }

        public MyPlane(double[] Normal, double[] AppPoint)
        {
            //Normalization (to be sure)
            var norm = Math.Sqrt(Math.Pow(Normal[0], 2) + Math.Pow(Normal[1], 2) + Math.Pow(Normal[2], 2));
            this.a = Normal[0]/norm;
            this.b = Normal[1]/norm;
            this.c = Normal[2]/norm;
            this.d = - (Normal[0] * AppPoint[0] + Normal[1] * AppPoint[1] + Normal[2] * AppPoint[2])/norm;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyPlane) obj);
        }

        protected bool Equals(MyPlane other)
        {
            double toleranceEqualsPlane = Math.Pow(10, -5);
            double normalProduct = this.a * other.a + this.b * other.b + this.c * other.c;

            if (Math.Abs(normalProduct - 1) < toleranceEqualsPlane && Math.Abs(this.d - other.d) < toleranceEqualsPlane)
            {
                return true;
            }
            if (Math.Abs(normalProduct + 1) < toleranceEqualsPlane && Math.Abs(this.d + other.d) < toleranceEqualsPlane)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = a.GetHashCode();
                hashCode = (hashCode * 397) ^ b.GetHashCode();
                hashCode = (hashCode * 397) ^ c.GetHashCode();
                hashCode = (hashCode * 397) ^ d.GetHashCode();
                return hashCode;
            }
        }


    }
}
