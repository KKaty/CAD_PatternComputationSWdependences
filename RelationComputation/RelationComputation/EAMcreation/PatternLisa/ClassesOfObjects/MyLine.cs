using System;
using AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Classe retta: equazioni cartesiane generiche {a x + b y + c z + d = 0 
    //                                             {a'x + b'y + c'z + d' = 0
    public class MyLine : MyPathGeometricObject 
    {      
        public MyPlane plane1;
        public MyPlane plane2;
        public double[] direction;

        public MyLine()
        {
        }

        public MyLine(MyPlane Plane1, MyPlane Plane2)
        {
            this.plane1 = Plane1;
            this.plane2 = Plane2;
            Array firstArray = new double[3] { Plane1.a, Plane1.b, Plane1.c };
            Array secondArray = new double[3] { Plane2.a, Plane2.b, Plane2.c };
            var directionLine = MathFunctions.MyVectorProduct(firstArray, secondArray);
            var directionLineNormalized = MathFunctions.MyNormalization(directionLine);
            var directionLineNormalizedVector = new double[] { 
                (double)directionLineNormalized.GetValue(0), 
                (double)directionLineNormalized.GetValue(1), 
                (double)directionLineNormalized.GetValue(2) 
            };
            this.direction = directionLineNormalizedVector;
        }

        protected bool Equals(MyLine other)
        {
            //SBAGLIATO!!!
            return Equals(plane1, other.plane1) && Equals(plane2, other.plane2);
            #region PROVA non completata
            //double x;
            //double y;
            //double z;
            //if (FunctionsLC.MyEqualsToZero(plane1.a))
            //{
            //    if (FunctionsLC.MyEqualsToZero(plane1.b))
            //    {
            //        z = -plane1.d / plane1.c;
            //        if (FunctionsLC.MyEqualsToZero(plane2.a))
            //        {
            //            y = (-plane2.d - plane2.c * z)/plane2.b;
            //        }
            //        else
            //        {
            //            if (FunctionsLC.MyEqualsToZero(plane2.b))
            //            {
            //                x = (-plane2.d - plane2.c * z) / plane2.a;
            //            }
            //            else
            //            { 
            //                //I give an arbitrary value to x:
            //                x = 0;
            //                y = (-plane2.d - plane2.c * z) / plane2.b;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (FunctionsLC.MyEqualsToZero(plane1.c))
            //        {
            //            y = -plane1.d / plane1.b;
            //        }
            //    }
            //}
            //else
            //{
            //    if (FunctionsLC.MyEqualsToZero(plane1.b))
            //    {
            //        if (FunctionsLC.MyEqualsToZero(plane1.c))
            //        {
            //            x = -plane1.d / plane1.a;
            //        }
            //    }
            //}
            #endregion
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((plane1 != null ? plane1.GetHashCode() : 0) * 397) ^ (plane2 != null ? plane2.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyLine) obj);
        }
    }
}
