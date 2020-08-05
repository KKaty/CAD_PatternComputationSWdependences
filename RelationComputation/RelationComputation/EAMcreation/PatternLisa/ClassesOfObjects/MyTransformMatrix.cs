using System;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.GeometricUtilities;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    //Class representing a transform referred to a component2 
    // (it is obtained from a IMathTransform of SW).
    //It contains:
    // - a rotation matrix 3x3
    // - a translation vector of length 3
    // - a scale factor
    public class MyTransformMatrix
    {
        
        public double[,] RotationMatrix;
        public double[] TranslationVector;
        public double ScaleFactor;   

        public MyTransformMatrix()
        {
            double[,] identityMatrix = new double[3, 3] {{1, 0, 0}, {0, 1, 0}, {0, 0, 1}};
            double[] nullTransVector = {0, 0, 0};
            this.RotationMatrix = identityMatrix;
            this.TranslationVector = nullTransVector;
            this.ScaleFactor = 1;
        }

        public MyTransformMatrix(double[,] rotationMatrix, double[] translationVector, double scaleFactor)
        {
            this.RotationMatrix = rotationMatrix;
            this.TranslationVector = translationVector;
            this.ScaleFactor = scaleFactor;
        }

        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyTransformMatrix) obj);
        }
        protected bool Equals(MyTransformMatrix other)
        {
            var tolerance = Math.Pow(10, -6);
            var okMatrix = true;
            for (var i = 0; i < 3; i++)
            {
                double[] firstVector =
                {
                    this.RotationMatrix[i, 0],
                    this.RotationMatrix[i, 1], 
                    this.RotationMatrix[i, 2]
                };
                double[] secondVector =
                {
                    other.RotationMatrix[i, 0],
                    other.RotationMatrix[i, 1], 
                    other.RotationMatrix[i, 2]
                };
                if (okMatrix == true)
                {
                    okMatrix = FunctionsLC.MyEqualsArray(firstVector, secondVector);
                }
                //KLdebug.Print("okMatrix" + okMatrix, "prova.txt");
            }
            
            return okMatrix && FunctionsLC.MyEqualsArray(TranslationVector, other.TranslationVector) && (Math.Abs(ScaleFactor-other.ScaleFactor)<tolerance);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (RotationMatrix != null ? RotationMatrix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TranslationVector != null ? TranslationVector.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ScaleFactor.GetHashCode();
                return hashCode;
            }
        }

        //This function, once applied to a MyTransformMatrix, composes it with another given one.
        public MyTransformMatrix ComposeTwoTransformMatrix(MyTransformMatrix other, SldWorks swApplication)
        {
            var outputMyTransformMatrix = new MyTransformMatrix();
            if (other == null)
            {
                swApplication.SendMsgToUser("ERROR. Found null MyTransformMatrix.");
                return outputMyTransformMatrix;
            }
            var outputRotMatrix = this.RotationMatrix.Multiply(other.RotationMatrix);
            var outputTransVector = this.TranslationVector.Add(other.TranslationVector);
            var outputScaleFactor = this.ScaleFactor * other.ScaleFactor;

            outputMyTransformMatrix = new MyTransformMatrix(outputRotMatrix, outputTransVector, outputScaleFactor);
            return outputMyTransformMatrix;
        }


    }

}
