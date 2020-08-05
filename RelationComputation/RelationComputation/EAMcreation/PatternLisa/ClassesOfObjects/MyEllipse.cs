using System;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    public class MyEllipse
    {
        

        public MyVertex centerEllipse;
        public double majorRadEllipse;
        public double minorRadEllipse;
        public double[] majorAxisDirectionEllipse;
        public double[] minorAxisDirectionEllipse;

        public MyEllipse()
        {
        }

        public MyEllipse(double[] EllipseParameters) //Curve myEllipse
        {
            //double[] ellipseParam = myEllipse.GetEllipseParams();
            this.centerEllipse = new MyVertex(EllipseParameters[0], EllipseParameters[1], EllipseParameters[2]);
            this.majorRadEllipse = EllipseParameters[3];
            double[] vectorMajorAxisDirectionEllipse = { EllipseParameters[4], EllipseParameters[5], EllipseParameters[6] };
            this.majorAxisDirectionEllipse = vectorMajorAxisDirectionEllipse;
            //this.majorAxisRadEllipse = FunctionsLC.ConvertPointPlusDirectionInMyLine(centerEllipseArray,
                //majorRadVectorEllipse);
            this.minorRadEllipse = EllipseParameters[7];
            double[] vectorMinorAxisDirectionEllipse = { EllipseParameters[8], EllipseParameters[9], EllipseParameters[10] };
            this.minorAxisDirectionEllipse = vectorMinorAxisDirectionEllipse;
            //this.minorAxisRadEllipse = FunctionsLC.ConvertPointPlusDirectionInMyLine(centerEllipseArray,
            //minorAxisRadVectorEllipse);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyEllipse) obj);
        }

        protected bool Equals(MyEllipse other)
        {
            var tolerance = Math.Pow(10, -5);
            return Equals(centerEllipse, other.centerEllipse) &&
                Math.Abs(majorRadEllipse - other.majorRadEllipse) < tolerance &&
                Math.Abs(minorRadEllipse - other.minorRadEllipse) < tolerance &&
                FunctionsLC.MyEqualsArray(majorAxisDirectionEllipse, other.majorAxisDirectionEllipse) &&
                FunctionsLC.MyEqualsArray(minorAxisDirectionEllipse, other.minorAxisDirectionEllipse);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (centerEllipse != null ? centerEllipse.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ majorRadEllipse.GetHashCode();
                hashCode = (hashCode * 397) ^ minorRadEllipse.GetHashCode();
                hashCode = (hashCode * 397) ^ (majorAxisDirectionEllipse != null ? majorAxisDirectionEllipse.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (minorAxisDirectionEllipse != null ? minorAxisDirectionEllipse.GetHashCode() : 0);
                return hashCode;
            }
        }

  
    }
}
