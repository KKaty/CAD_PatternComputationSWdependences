using System;
using System.Diagnostics.CodeAnalysis;
using Accord.Math;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace AssemblyRetrieval.PatternLisa.Functions_modifiedFromKatia
{
    public class GeometryFunctions
    {
        /// <summary>
        /// The my line equation.
        /// </summary>
        /// <param name="edgeDirection">
        /// The edge direction.
        /// </param>
        /// <param name="startPoint">
        /// The start point.
        /// </param>
        /// <param name="secondaEquazione">
        /// The seconda equazione.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public static double[] MyLineEquation(Array edgeDirection, Array startPoint, out double[] secondaEquazione)
        {
            // Ricavo due vettori che servono per identificare l'equazione cartesiana della retta r' precedentemente descritta.
            // Per praticità riscrivo le componenti del vettore direzione della retta r' e il punto Sp per cui passa.
            var dirX = (double)edgeDirection.GetValue(0);
            var dirY = (double)edgeDirection.GetValue(1);
            var dirZ = (double)edgeDirection.GetValue(2);
            var puntoX = (double)startPoint.GetValue(0);
            var puntoY = (double)startPoint.GetValue(1);
            var puntoZ = (double)startPoint.GetValue(2);

            // Scrivo le due equazioni dei piani che identificano la retta r'
            var primaEquazione = new double[4] { dirY, -dirX, 0, dirX * puntoY - dirY * puntoX };
            secondaEquazione = new double[4] { dirZ, 0, -dirX, dirX * puntoZ - dirZ * puntoX };
            return primaEquazione;
        }

        /// <summary>
        /// The my point intersection two plane.
        /// </summary>
        /// <param name="primoPiano">
        /// The primo piano.
        /// </param>
        /// <param name="secondoPiano">
        /// The secondo piano.
        /// </param>
        /// <param name="primaEquazione">
        /// The prima equazione.
        /// </param>
        /// <param name="secondaEquazione">
        /// The seconda equazione.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] MyPointIntersectionTwoPlane(
            double[] primoPiano, double[] secondoPiano, double[] primaEquazione, double[] secondaEquazione)
        {
            double[,] matrice =
            {
                {
                    (double)primoPiano.GetValue(0), (double)primoPiano.GetValue(1),
                    (double)primoPiano.GetValue(2)
                },
                {
                    (double)secondoPiano.GetValue(0), (double)secondoPiano.GetValue(1),
                    (double)secondoPiano.GetValue(2)
                },
                {
                    (double)primaEquazione.GetValue(0), (double)primaEquazione.GetValue(1),
                    (double)primaEquazione.GetValue(2)
                },
                {
                    (double)secondaEquazione.GetValue(0), (double)secondaEquazione.GetValue(1),
                    (double)secondaEquazione.GetValue(2)
                }
            };

            double[,] terminiNoti =
            {
                { -(double)primoPiano.GetValue(3) }, { -(double)secondoPiano.GetValue(3) },
                { -(double)primaEquazione.GetValue(3) }, { -(double)secondaEquazione.GetValue(3) },
            };

            // Devo qudrare il sistema per poterlo risolverlo, così scrivo l'equazione normale A'Ax=A'b
            var matriceEqNormali = matrice.TransposeAndMultiply(matrice);
            var terminiNotiEqNormali = matrice.TransposeAndMultiply(terminiNoti);
            
            // Risolvo il sistema per determinare il punto di intersezione tra le due rette
            var matIntersezione = matriceEqNormali.Solve(terminiNotiEqNormali, true);
            var puntoIntersezioneQ = matIntersezione.GetColumn(0);
            return puntoIntersezioneQ;
        }

        /// <summary>
        /// The my point intersection line plane.
        /// </summary>
        /// <param name="primoPianoRetta">
        /// The primo piano retta.
        /// </param>
        /// <param name="secondoPianoRetta">
        /// The secondo piano retta.
        /// </param>
        /// <param name="equazionePiano">
        /// The equazione piano.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] MyPointIntersectionLinePlane(
            double[] primoPianoRetta, double[] secondoPianoRetta, double[] equazionePiano)
        {
            double[,] matrice =
            {
                {
                    (double)primoPianoRetta.GetValue(0), (double)primoPianoRetta.GetValue(1),
                    (double)primoPianoRetta.GetValue(2)
                },
                {
                    (double)secondoPianoRetta.GetValue(0), (double)secondoPianoRetta.GetValue(1),
                    (double)secondoPianoRetta.GetValue(2)
                },
                {
                    (double)equazionePiano.GetValue(0), (double)equazionePiano.GetValue(1),
                    (double)equazionePiano.GetValue(2)
                }
            };

            double[,] terminiNoti =
            {
                { -(double)primoPianoRetta.GetValue(3) }, { -(double)secondoPianoRetta.GetValue(3) },
                { -(double)equazionePiano.GetValue(3) }
            };

            // Risolvo il sistema per determinare il punto di intersezione tra le due rette
            var matIntersezione = matrice.Solve(terminiNoti, true);
            var puntoIntersezioneQ = matIntersezione.GetColumn(0);
            return puntoIntersezioneQ;
        }

        /// <summary>
        /// The my get plane equation.
        /// </summary>
        /// <param name="firstNormal">
        /// The first normal.
        /// </param>
        /// <param name="firstPoint">
        /// The first point.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Suppression is OK here."),SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        public static double[] MyGetPlaneEquation(double[] firstNormal, double[] firstPoint)
        {
            var primoPiano = new double[4];
            var k = (double)firstNormal.GetValue(0) * (double)firstPoint.GetValue(0)
                    + (double)firstNormal.GetValue(1) * (double)firstPoint.GetValue(1)
                    + (double)firstNormal.GetValue(2) * (double)firstPoint.GetValue(2);
            primoPiano.SetValue((double)firstNormal.GetValue(0), 0);
            primoPiano.SetValue((double)firstNormal.GetValue(1), 1);
            primoPiano.SetValue((double)firstNormal.GetValue(2), 2);
            primoPiano.SetValue(-k, 3);
            return primoPiano;
        }

        public static Array MyNormalInPoint(Face2 face, double x, double y, double z) 
        {
            var normal = new double[3];
            var mySurf = (Surface)face.GetSurface();

            double[] firstEvalutation = mySurf.EvaluateAtPoint(x, y, z); // --> EvaluateAtPoint restituisce la normale alla superficie in un punto! 

           
            if (face.FaceInSurfaceSense())
            {
                normal.SetValue((double)firstEvalutation.GetValue(0), 0);
                normal.SetValue((double)firstEvalutation.GetValue(1), 1);
                normal.SetValue((double)firstEvalutation.GetValue(2), 2);
            }
            else
            {
                normal.SetValue(-(double)firstEvalutation.GetValue(0), 0);
                normal.SetValue(-(double)firstEvalutation.GetValue(1), 1);
                normal.SetValue(-(double)firstEvalutation.GetValue(2), 2);
            }
           
            return normal;
        }

        public static void pointIntersection(double[] firstPoint, Face2 firstFace, double[] secondPoint, Face2 secondFace, IBody2[] body, ModelDoc2 myModel, SldWorks mySwApplication, ref int correctIntersection, ref int correctIntersectionForPair)
        {
            var firstSurface = (Surface)firstFace.GetSurface();
            var secondSurface = (Surface)secondFace.GetSurface();
            correctIntersection = 0;
            if (firstPoint != null && secondPoint != null)
            {

                var direction = (double[])MathFunctions.MyNormalization(MathFunctions.MyVectorDifferent(secondPoint, firstPoint));
                var intersection =
                    (int)
                        myModel.RayIntersections(
                            body,
                            (object)firstPoint,
                            (object)direction,
                            (int)(swRayPtsOpts_e.swRayPtsOptsENTRY_EXIT),
                            (double)0,
                            (double)0);

                // Se sono dei presenti dei punti di intersezione li salvo in un apposito vettore.

                if (intersection > 0)
                {
                    var points = (double[])myModel.GetRayIntersectionsPoints();
                    var totalEntity = (Array)myModel.GetRayIntersectionsTopology();
                    //mySwApplication.SendMsgToUser("punti totali " + points.Length.ToString() + "intersezioni " + intersection.ToString());
                    if (points != null)
                    {
                        for (int i = 0; i < points.Length; i += 9)
                        {
                            double[] pt = new double[] { points[i + 3], points[i + 4], points[i + 5] };
                            double[] directionIntersection =
                                (double[])MathFunctions.MyNormalization(MathFunctions.MyVectorDifferent(pt, firstPoint));

                            if (Math.Abs(MathFunctions.MyInnerProduct(direction, directionIntersection) - 1) < 0.01
                                /*&& MyDistanceTwoPoint(pt, secondPoint) > 0.001 && MyDistanceTwoPoint(pt, firstPoint) > 0.001*/)
                            {
                                if (firstSurface.Identity() == 4001 && secondSurface.Identity() == 4001)
                                {
                                    if (Distance.MyDistanceTwoPoint(firstPoint, secondPoint) > Distance.MyDistanceTwoPoint(firstPoint, pt))
                                    {
                                        correctIntersection++;
                                        correctIntersectionForPair++;
                                    }
                                }
                                else if (firstSurface.Identity() == 4002 && secondSurface.Identity() == 4002)
                                {
                                    double[] closestPointToIntersectionOnFirstSurface = firstSurface.GetClosestPointOn(pt[0], pt[1], pt[2]);
                                    double[] closestPointToIntersectionOnSecondSurface = secondSurface.GetClosestPointOn(pt[0], pt[1], pt[2]);
                                    //myModel.Insert3DSketch();
                                    //myModel.CreatePoint2(pt[0], pt[1], pt[2]);
                                    if (Distance.MyDistanceTwoPoint(firstPoint, secondPoint) > Distance.MyDistanceTwoPoint(firstPoint, pt) &&
                                        Math.Abs(Distance.MyDistanceTwoPoint(closestPointToIntersectionOnFirstSurface, pt)) > 0.0001 && Math.Abs(Distance.MyDistanceTwoPoint(closestPointToIntersectionOnSecondSurface, pt)) > 0.0001)
                                    {
                                        correctIntersection++;
                                        correctIntersectionForPair++;
                                    }
                                    else
                                    {
                                        var normalFirstFace = (double[])MyNormalInPoint(firstFace, closestPointToIntersectionOnFirstSurface[0], closestPointToIntersectionOnFirstSurface[1], closestPointToIntersectionOnFirstSurface[2]);
                                        double[] dir =
                                            (double[])MathFunctions.MyNormalization(MathFunctions.MyVectorDifferent(closestPointToIntersectionOnSecondSurface, closestPointToIntersectionOnFirstSurface));
    
                                    }

                                }
                            }
                                
                        }
                    }
                    else { mySwApplication.SendMsgToUser("Punti intersezione nulli"); }
                }
            }
            else { mySwApplication.SendMsgToUser("Punti nulli"); }

        }


        /// <summary>
        /// The my get normal for plane face.
        /// </summary>
        /// <param name="firstFace">
        /// The first face.
        /// </param>
        /// <param name="firstPoint">
        /// The first point.
        /// </param>
        /// <returns>
        /// The <see cref="double[]"/>.
        /// </returns>
        public static double[] MyGetNormalForPlaneFace(Face2 firstFace, out double[] firstPoint)
        {
            var firstSurface = (Surface)firstFace.GetSurface();
            var firstNormal = new double[3];
            firstPoint = new double[3];
            
            Array firstValuesPlane = null;

            if (firstSurface.IsPlane())
            {
                firstValuesPlane = firstSurface.PlaneParams;
                Array.Copy(firstValuesPlane, 0, firstNormal, 0, 3);
                Array.Copy(firstValuesPlane, 3, firstPoint, 0, 3);
            }

            
            // Pongo il verso della normale alla superficie concorde con quello della faccia.
            if (!firstFace.FaceInSurfaceSense())
            {
                firstNormal.SetValue((double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(2), 2);
            }
            else
            {
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(2), 2);
            }

          
            return firstNormal;
        }

        public static double[] KLMyGetNormalForPlaneFace(Face2 firstFace, out double[] firstPoint, double[,] transformMatrix)
        {
            var firstSurface = (Surface)firstFace.GetSurface();
            var firstNormal = new double[3];
            firstPoint = new double[3];

            Array firstValuesPlane = null;

            if (firstSurface.IsPlane())
            {
                firstValuesPlane = firstSurface.PlaneParams;
                Array.Copy(firstValuesPlane, 0, firstNormal, 0, 3);
                Array.Copy(firstValuesPlane, 3, firstPoint, 0, 3);
            }

            // Pongo il verso della normale alla superficie concorde con quello della faccia.
            if (!firstFace.FaceInSurfaceSense())
            {
                firstNormal.SetValue((double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue((double)firstValuesPlane.GetValue(2), 2);
            }
            else
            {
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(0), 0);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(1), 1);
                firstNormal.SetValue(-(double)firstValuesPlane.GetValue(2), 2);
            }

            if (transformMatrix != null)
            {
                double[] firstNormalAffine = { (double)firstNormal.GetValue(0), (double)firstNormal.GetValue(1), (double)firstNormal.GetValue(2), 1 };
                double[] firstPointAffine = { (double)firstPoint.GetValue(0), (double)firstPoint.GetValue(1), (double)firstPoint.GetValue(2), 1 };

                var newFirstNormal = Matrix.Multiply(transformMatrix, firstNormalAffine);
                var newFirstPoint = Matrix.Multiply(transformMatrix, firstPointAffine);

                firstNormal.SetValue((double)newFirstNormal.GetValue(0), 0);
                firstNormal.SetValue((double)newFirstNormal.GetValue(1), 1);
                firstNormal.SetValue((double)newFirstNormal.GetValue(2), 2);

                firstPoint.SetValue((double)newFirstPoint.GetValue(0), 0);
                firstPoint.SetValue((double)newFirstPoint.GetValue(1), 1);
                firstPoint.SetValue((double)newFirstPoint.GetValue(2), 2);
            }

            return firstNormal;
        }
    }
}