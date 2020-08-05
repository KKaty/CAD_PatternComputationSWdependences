using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Math;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.Part.PathCreation_Part
{
    public partial class Functions
    {
        public static void AddPathsFromNewCheckOfMB(MyMatrAdj matrAdjToSee, List<MyVertex> listCentroid, 
            ref List<MyPathOfPoints> listOfPaths, List<int> listOfExtremePoints, List<int> listOfMBPoints, 
            ref StringBuilder fileOutput, ref bool toleranceOk)
        {
            //Ultima ricerca:
            //individuo i path di length >3 e t.c. se num di MB nel path = s allora 0<s<length e
            //per ogni MB di tali path verifico che esista un path che contenga b1-MB-b2, con b1,b2 branch di MB.
            //[QUESTA PARTE SERVE IN PARTICOLARE PER I CASI "GRIGLIA"]
            if (listOfPaths.FindIndex(pathObject =>
                (pathObject.path.Count > 3 && pathObject.path.Count(listOfMBPoints.Contains) < pathObject.path.Count &&
                pathObject.path.Count(listOfMBPoints.Contains) > 0)) != -1)
            {
                var listOfPathsContainingMBToCheckAgain = new List<MyPathOfPoints>(
                    listOfPaths.FindAll(pathObject => (pathObject.path.Count > 3 && pathObject.path.Count(listOfMBPoints.Contains) < pathObject.path.Count &&
                pathObject.path.Count(listOfMBPoints.Contains) > 0)));
                var listOfMBToCheckAgain = new List<int>(listOfMBPoints.FindAll(
                    mb => listOfPathsContainingMBToCheckAgain.FindIndex(pathObject => pathObject.path.Contains(mb)) != -1));

                fileOutput.AppendLine("\n");
                fileOutput.AppendLine("\n Path derivanti dal MB Check Again:");

                foreach (var mb in listOfMBToCheckAgain)
                {
                    List<int> branchesOfMB = matrAdjToSee.matr.GetRow(mb).Find(entry => entry == 1).ToList(); //indici dei branch di mb
                    int lengthOfBranchesList = branchesOfMB.Count;
                    for (var i = 0; i < lengthOfBranchesList - 1; i++)
                    {
                        for (var j = i + 1; j < lengthOfBranchesList; j++)
                        {
                            var branch1 = branchesOfMB[i];
                            var branch2 = branchesOfMB[j];
                            if (!(listOfExtremePoints.Contains(branch1)) && !(listOfExtremePoints.Contains(branch2)))
                            //perché se uno dei due branch è un estremo sono sicura che un path contenente branch1-mb-branch2 esiste
                            {
                                if (listOfPaths.FindIndex(pathObject => (pathObject.path.Contains(branch1) && pathObject.path.Contains(mb) && pathObject.path.Contains(branch2))) == -1)
                                //se non esiste path contenente branch1-mb-branch2
                                {

                                    List<int> currentPath;
                                    MyPathGeometricObject pathCurve;
                                    if (listCentroid[branch2].Lieonline(GeometricUtilities.FunctionsLC.LinePassingThrough(listCentroid[mb], listCentroid[branch1])))
                                    {
                                        currentPath = ThreePointsGivenPathsLine(matrAdjToSee, listCentroid, 
                                            listOfExtremePoints, branch1, mb, branch2, ref fileOutput,
                                            ref toleranceOk, out pathCurve);
                                    }
                                    else
                                    {
                                        currentPath = ThreePointsGivenPathsCircum(matrAdjToSee, listCentroid, 
                                            listOfExtremePoints, branch1, mb, branch2, ref fileOutput,
                                            ref toleranceOk, out pathCurve);
                                    }

                                    //Verifica LongestPattern?

                                    //se nel path che ho creato ci fossero dei MB li avrei già presi,
                                    //se ci fossero dei simple non ancora toccati li avrei già presi,
                                    //sono quasi certa che il path sia composto da punti già percorsi almeno una volta 
                                    //(sicuramente almeno una volta i branch del MB sono stati percorsi)...
                                    //----> Non faccio il salvataggio dei penultimi punti.

                                    var newPathObject = new MyPathOfPoints(currentPath, pathCurve);
                                    listOfPaths.Add(newPathObject);
                                }
                            }

                        }
                    }

                }
            }
        }// fine AddPathsFromNewCheckOfMB
    }
}
