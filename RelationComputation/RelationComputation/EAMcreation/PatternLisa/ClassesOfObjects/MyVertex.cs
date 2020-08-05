using System;
using Accord.Math;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    public class MyVertex
    {
        

        public double x;
        public double y;
        public double z;

        public MyVertex()
        {
        }

        public MyVertex(double X, double Y, double Z)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }

        protected bool Equals(MyVertex other)
        {
            var toll = Math.Pow(10, -4);
            return (Math.Abs(x - other.x) < toll && Math.Abs(y - other.y) <= toll && Math.Abs(z - other.z) <= toll);
            //return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyVertex) obj);
        }
     
        //Applicato ad un vertice, restituisce la sua distanza euclidea da un altro vertice
        public double Distance(MyVertex other)
        {
            if (this.Equals(other))
            {
                return 0;
            }
            else
            {
                var distance =
                    Math.Sqrt(Math.Pow(this.x - other.x, 2) + Math.Pow(this.y - other.y, 2) +
                              Math.Pow(this.z - other.z, 2));
                if (!Double.IsNaN(distance))
                {
                    return distance;
                }
                return -1;
            }
        }

        //Applied to a firstvertex, given secondvertex and a double[3], it verifies if firstvertex is obtained summing secondvertex with double[3]
        public bool IsTranslationOf(MyVertex secondVertex, double[] translationArray)
        {
            var translatedVertex = new MyVertex(secondVertex.x + (double) translationArray.GetValue(0),
                secondVertex.y + (double) translationArray.GetValue(1),
                secondVertex.z + (double) translationArray.GetValue(2));

            return (this.Equals(translatedVertex));
        }

        //Applied to a MyVertex, this method reflects it by a given MyPlane
        //RECALL:
        // P: x*u = lambda , x in R^3, u in R^3 unit vector <-- Reflectional Plane
        // R_P(x) = x-2(x*u-lambda)u                        <-- Reflectional transformation
        public MyVertex Reflect(MyPlane reflectionalMyPlane)
        {
            double[] coeff = {
                                 2*(this.x*reflectionalMyPlane.a+reflectionalMyPlane.d),
                                 2*(this.y*reflectionalMyPlane.b+reflectionalMyPlane.d),
                                 2*(this.z*reflectionalMyPlane.c+reflectionalMyPlane.d)
                             };
            var reflectedVertex = new MyVertex(this.x - (double)coeff.GetValue(0) * reflectionalMyPlane.a,
                this.y - (double)coeff.GetValue(1)*reflectionalMyPlane.b,
                this.z - (double)coeff.GetValue(2)*reflectionalMyPlane.c);

            return reflectedVertex;
        }

        //Applied to a firstvertex, given secondvertex and a MyPlane, it verifies if secondvertex is the reflected of
        //firstvertex by the given MyPlane
        public bool IsReflectionOf(MyVertex secondVertex, MyPlane reflectionalMyPlane)
        {
            var reflectedVertex = this.Reflect(reflectionalMyPlane);
            return (secondVertex.Equals(reflectedVertex));
        }

        //Applied to a MyVertex, this method rotates it by a given angle TETA respect to a given 
        //NORMAL DIRECTION VECTOR referred to a rotation axis passing through the origin
        public MyVertex Rotate(double teta, double[] axisDirection)
        {
            double[] pointToRotate = {this.x, this.y, this.z};
            //double[] row0 =
            //{
            //    Math.Pow(axisDirection[0], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta),
            //    axisDirection[0]*axisDirection[1]*(1 - Math.Cos(teta)) - axisDirection[2]*Math.Sin(teta),
            //    axisDirection[0]*axisDirection[2]*(1 - Math.Cos(teta)) + axisDirection[1]*Math.Sin(teta)
            //};
            //double[] row1 =
            //{
            //    axisDirection[0]*axisDirection[1]*(1 - Math.Cos(teta)) + axisDirection[2]*Math.Sin(teta),
            //    Math.Pow(axisDirection[1], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta),
            //    axisDirection[1]*axisDirection[2]*(1 - Math.Cos(teta)) - axisDirection[0]*Math.Sin(teta)
            //};
            //double[] row2 =
            //{
            //    axisDirection[0]*axisDirection[2]*(1 - Math.Cos(teta)) - axisDirection[1]*Math.Sin(teta),
            //    axisDirection[1]*axisDirection[2]*(1 - Math.Cos(teta)) + axisDirection[0]*Math.Sin(teta),
            //    Math.Pow(axisDirection[2], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta)
            //};
            double[,] rotationMatrix = 
            {
                {
                    Math.Pow(axisDirection[0], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta),
                    axisDirection[0]*axisDirection[1]*(1 - Math.Cos(teta)) - axisDirection[2]*Math.Sin(teta),
                    axisDirection[0]*axisDirection[2]*(1 - Math.Cos(teta)) + axisDirection[1]*Math.Sin(teta)
                }, 
                {
                    axisDirection[0]*axisDirection[1]*(1 - Math.Cos(teta)) + axisDirection[2]*Math.Sin(teta),
                    Math.Pow(axisDirection[1], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta),
                    axisDirection[1]*axisDirection[2]*(1 - Math.Cos(teta)) - axisDirection[0]*Math.Sin(teta)
                }, 
                {
                    axisDirection[0]*axisDirection[2]*(1 - Math.Cos(teta)) - axisDirection[1]*Math.Sin(teta),
                    axisDirection[1]*axisDirection[2]*(1 - Math.Cos(teta)) + axisDirection[0]*Math.Sin(teta),
                    Math.Pow(axisDirection[2], 2)*(1 - Math.Cos(teta)) + Math.Cos(teta)
                }
            };
            var rotated = Matrix.Multiply(rotationMatrix, pointToRotate);
            var rotatedVertex = new MyVertex(rotated[0], rotated[1], rotated[2]);
            return rotatedVertex;
        }

        //Applied to a firstvertex, given secondvertex, an angle and a axis direction, it verifies if secondvertex 
        //is the rotated of firstvertex
        public bool IsRotationOf(MyVertex firstVertex, double teta, double[] axisDirection)
        {
            //const string nameFile = "GetRotationalPatterns.txt";
            var rotatedVertex = firstVertex.Rotate(teta, axisDirection);
            //KLdebug.Print("ROTATO DOVREBBE ESSERE: (" + rotatedVertex.x + ", "  + rotatedVertex.y + ", " +rotatedVertex.z + ")", nameFile);
            return (this.Equals(rotatedVertex));
        }

        //Applicato ad un vertice, dice se il vertice sta sul piano dato in input
        public bool Lieonplane(MyPlane GivenPlane)
        {
            double toleranceLieOnPlane = Math.Pow(10, -3);
            return (Math.Abs(GivenPlane.a * this.x + GivenPlane.b * this.y + GivenPlane.c * this.z + GivenPlane.d) < toleranceLieOnPlane);
        }

        //Applicato ad un vertice, dice se il vertice sta sulla retta data in input
        public bool Lieonline(MyLine GivenLine)
        {
            return (this.Lieonplane(GivenLine.plane1) && this.Lieonplane(GivenLine.plane2));
        }

        //Applicato ad un vertice, dice se il vertice sta sulla sfera data in input
        public bool Lieonsphere(MySphere givenMySphere)
        {
            double toleranceLieOnSphere = Math.Pow(10, -3);
            return (Math.Abs(Math.Pow(this.x, 2) + Math.Pow(this.y, 2) + Math.Pow(this.z, 2) + givenMySphere.a * this.x + givenMySphere.b * this.y + givenMySphere.c * this.z + givenMySphere.d) < toleranceLieOnSphere);
        }

        //Applicato ad un vertice, dice se il vertice sta sulla circonferenza data in input
        public bool Lieoncircum(MyCircumForPath GivenCircum)
        {
            if (GivenCircum.circumplane == null || GivenCircum.circumsphere.centerSphere == null)
            {
                return false;
            }
            return (this.Lieonplane(GivenCircum.circumplane) && this.Lieonsphere(GivenCircum.circumsphere));
        }
    }
}
