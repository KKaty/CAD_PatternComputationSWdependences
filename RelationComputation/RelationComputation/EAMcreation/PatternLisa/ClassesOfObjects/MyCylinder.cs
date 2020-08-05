using System;
using System.Collections.Generic;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    public class MyCylinder
    {
        

        public double[] originCylinder;
        public double[] axisDirectionCylinder;
        public MyLine axisCylinder;
        public double radiusCylinder;
        public List<MyCircle> listOfBaseCircle;     //da aggiornare solo se il cilindro ha delle basi con edge chiuso circle (max count = 2)
        public List<MyEllipse> listOfBaseEllipse;   //da aggiornare solo se il cilindro ha delle basi con edge chiuso ellipse (max count = 2)

        public MyCylinder()
        {
        }

        public MyCylinder(double[] OriginCylinder, double[] AxisDirectionCylinder, double RadiusCylinder,
            List<MyCircle> ListOfBaseCircle = null, List<MyEllipse> ListOfBaseEllipse = null)
        {
            this.originCylinder = OriginCylinder;
            this.axisDirectionCylinder = AxisDirectionCylinder;
            this.axisCylinder = FunctionsLC.ConvertPointPlusDirectionInMyLine(OriginCylinder, AxisDirectionCylinder);
            this.radiusCylinder = RadiusCylinder;
        }

        
        protected bool Equals(MyCylinder other)
        {
            double toleranceEqualsCylinder = Math.Pow(10, -5);
            return (Equals(axisCylinder, other.axisCylinder) && Math.Abs(radiusCylinder - other.radiusCylinder) < toleranceEqualsCylinder);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((axisCylinder != null ? axisCylinder.GetHashCode() : 0) * 397) ^ radiusCylinder.GetHashCode();
            }
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyCylinder) obj);
        }

    }
}
