using System.Collections.Generic;
using System.Linq;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;
using SolidWorks.Interop.sldworks;

namespace AssemblyRetrieval.PatternLisa.Assembly.AssemblyUtilities
{
    public partial class GeometryAnalysis
    {
        //It detects all the symmetry relations in a set of MyRepeatedComponent 
        //on a given MyPathOfPoints.
        //(symmetry types considered: translation, reflection, rotation)       
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static bool GetPatternsFromPath_Assembly(MyPathOfPoints myPathOfPoints,
            List<MyRepeatedComponent> listOfComponents, ref List<MyPathOfPoints> listOfPathOfPoints,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyPatternOfComponents> listOfOutputPattern,
            ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {

            var listOfOriginsOnThePath = listOfComponents.Select(re => re.Origin).ToList();

            var listOfComponentsOnThePath = myPathOfPoints.path.Select(ind => listOfComponents[ind]).ToList();


            if (myPathOfPoints.pathGeometricObject.GetType() == typeof (MyLine))
            {
                SwApplication.SendMsgToUser("traslazione vecchia");
                return GetPatternsFromLinearPath_Assembly(listOfComponentsOnThePath, myPathOfPoints.pathGeometricObject,
                    ref listOfPathOfPoints, listOfOriginsOnThePath, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo);
            }
            else
            {
                SwApplication.SendMsgToUser("tras circ vecchia");

                return GetPatternsFromCircularPath_Assembly(listOfComponentsOnThePath, myPathOfPoints.pathGeometricObject,
                    ref listOfPathOfPoints, listOfOriginsOnThePath, ref listOfMatrAdj,
                    ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, SwApplication);
            }
        }

        // Pecedura duplicata in modo che non ricalcoli le origini delle componenti, ma usi quelle che ha chiesto in input prima!
        public static bool KLGetPatternsFromPath_Assembly(MyPathOfPoints myPathOfPoints,
            List<MyRepeatedComponent> listOfComponents, List<MyVertex> listOfOriginsOnThePath, ref List<MyPathOfPoints> listOfPathOfPoints,
            ref List<MyMatrAdj> listOfMatrAdj, ref List<MyPatternOfComponents> listOfOutputPattern,
            ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            if (myPathOfPoints != null)
            {
                if (myPathOfPoints.path.Any())
                {
                    //KLdebug.Print("Comp max " + listOfComponents.Count.ToString(), "indici.txt");
                    //foreach (var ind in myPathOfPoints.path)
                    //{
                    //    KLdebug.Print(ind.ToString(), "indici.txt");
                    //}

                    var listOfComponentsOnThePath = myPathOfPoints.path.Select(ind => listOfComponents[ind]).ToList();

                 
                    // Aggiunto per non fargli pescare indici che non ha, ma così facendo va in loop da qualche altra parte.
                    //var listOfComponentsOnThePathEnumerable = from ind in myPathOfPoints.path
                    //    where ind < listOfComponents.Count
                    //    select listOfComponents[ind];

                    //if (!listOfComponentsOnThePathEnumerable.Any())
                    //{
                    //    listOfPathOfPoints.Clear();
                    //    return false;
                    //}
                    //var listOfComponentsOnThePath =
                    //    (List<MyRepeatedComponent>) listOfComponentsOnThePathEnumerable.ToList();
                    //SwApplication.SendMsgToUser("Seleziono path point FINE");

                    if (myPathOfPoints.pathGeometricObject.GetType() == typeof(MyLine))
                    {
                        //SwApplication.SendMsgToUser("Cerco linear path");
                        //const string nameFile = "GetTranslationalPatterns.txt";
                        //KLdebug.Print(" ", nameFile);
                        //KLdebug.Print("POSSIBILE TRASLAZIONE retta. AVVIO", nameFile);
                        return KLGetPatternsFromLinearPath_Assembly(listOfComponentsOnThePath, myPathOfPoints.pathGeometricObject,
                            ref listOfPathOfPoints, listOfOriginsOnThePath, ref listOfMatrAdj,
                            ref listOfOutputPattern, ref listOfOutputPatternTwo, SwApplication);
                
                    }
                    else
                    {
                        //SwApplication.SendMsgToUser("Cerco circular path");
                        //const string nameFile = "GetCircularPatterns.txt";
                        //KLdebug.Print(" ", nameFile);
                        //KLdebug.Print("POSSIBILE TRASLAZIONE o ROTAZIONE su circonferenza. AVVIO", nameFile);
                        return KLGetPatternsFromCircularPath_Assembly(listOfComponentsOnThePath, myPathOfPoints.pathGeometricObject,
                            ref listOfPathOfPoints, listOfOriginsOnThePath, ref listOfMatrAdj,
                            ref listOfOutputPattern, ref listOfOutputPatternTwo, SwModel, SwApplication);
                    }
                }
            }
         
            return false;
        }
        //It detects all the TRANSLATIONAL relations in a set of MyRepeatedComponent 
        //whose origins lie on a given line.
        //it saves patterns of length = 2 in a list;
        //it saves patterns of length > 2 in a list.
        //It returns TRUE if only one pattern has been detected and it has maximum length, FALSE otherwise.

        public static bool GetPatternsFromLinearPath_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
            MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfPoints,
            List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj, 
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo)
        {
            var numOfCompOnThisPath = listOfComponentsOnThePath.Count;
            var noStop = true;

            var i = 0;
            while (i < (numOfCompOnThisPath - 1))
            {
                var newPattern = new MyPatternOfComponents();
                var foundNewPattern = GetMaximumTranslation_Assembly(listOfComponentsOnThePath, pathObject, ref i, ref numOfCompOnThisPath,
                    ref noStop, ref newPattern);

                if (foundNewPattern)
                {
                    if (newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath ||
                        newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath - 1)
                    {
                        noStop = true;
                    }

                    CheckAndUpdate_Assembly(newPattern, ref listOfPathOfPoints,
                        listOfOrigins, ref listOfMatrAdj,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }
            }
           
            if (noStop)
            {
                return true;
            }
            return false;
        }

        public static bool KLGetPatternsFromLinearPath_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
            MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfPoints,
            List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo, SldWorks swApplication)
        {
            var numOfCompOnThisPath = listOfComponentsOnThePath.Count;
            var noStop = true;

            //const string nameFile = "GetTranslationalPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("VERIFICA DELLE POSSIBILI TRASLAZIONI TRA " + numOfCompOnThisPath + " COMPONENTS:", nameFile);
            //KLdebug.Print(" ", nameFile);

            var i = 0;
            while (i < (numOfCompOnThisPath - 1))
            {
                var newPattern = new MyPatternOfComponents();
                var foundNewPattern = KLGetMaximumTranslation_Assembly(listOfComponentsOnThePath, pathObject, ref i, ref numOfCompOnThisPath,
                    ref noStop, ref newPattern, swApplication);

                if (foundNewPattern)
                {
                    if (newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath ||
                        newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath - 1)
                    {
                        noStop = true;
                    }

                    var listREofRcomponents = new List<MyRepeatedEntity>(listOfComponentsOnThePath.Select(comp => comp.RepeatedEntity).ToList());
                    var listGS = new List<Surface>();
                    var newPatternPoint = new MyPattern(listREofRcomponents, newPattern.pathOfMyPattern,
                        newPattern.typeOfMyPattern, listGS);
                    
                    KLCheckAndUpdate_Assembly(newPattern, ref listOfPathOfPoints,
                        listOfOrigins, ref listOfMatrAdj,
                        ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }
            }
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
                //KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            return false;
        }

        public static bool  GetPatternsFromCircularPath_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
            MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfPoints,
            List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj,
            ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            var numOfCompOnThisPath = listOfComponentsOnThePath.Count;
            var noStop = true;

            var i = 0;
            while (i < (numOfCompOnThisPath - 1))
            {
                var newPattern = new MyPatternOfComponents();
                var j = i;
                var foundNewPattern = GetMaximumTranslation_Assembly(listOfComponentsOnThePath, pathObject, ref j, ref numOfCompOnThisPath,
                    ref noStop, ref newPattern);

                if (foundNewPattern == false)
                {
                    SwApplication.SendMsgToUser("pathCircumference vecchio");
                    var pathCircumference = (MyCircumForPath)pathObject;
                    foundNewPattern = GetMaximumRotation_Assembly(listOfComponentsOnThePath, pathCircumference, ref i, ref numOfCompOnThisPath,
                        ref noStop, ref newPattern, SwModel, SwApplication);
                }
                else
                {
                    i = j;
                }

                if (foundNewPattern)
                {
                    if (newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath ||
                        newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath - 1)
                    {
                        noStop = true;
                    }

                       CheckAndUpdate_Assembly(newPattern, ref listOfPathOfPoints,
                           listOfOrigins, ref listOfMatrAdj,
                           ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }

            }
   
           if (noStop)
            {
                return true;
            }
            return false;

        }

        public static bool KLGetPatternsFromCircularPath_Assembly(List<MyRepeatedComponent> listOfComponentsOnThePath,
           MyPathGeometricObject pathObject, ref List<MyPathOfPoints> listOfPathOfPoints,
           List<MyVertex> listOfOrigins, ref List<MyMatrAdj> listOfMatrAdj,
           ref List<MyPatternOfComponents> listOfOutputPattern, ref List<MyPatternOfComponents> listOfOutputPatternTwo, ModelDoc2 SwModel, SldWorks SwApplication)
        {
            var numOfCompOnThisPath = listOfComponentsOnThePath.Count;
            var noStop = true;

            //const string nameFile = "GetCircularPatterns.txt";
            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print(
            //    "VERIFICA DELLE POSSIBILI TRASLAZIONI SU CIRCONFERENZA O ROTAZIONI TRA " + numOfCompOnThisPath + " COMPONENTS:",
            //    nameFile);
            //KLdebug.Print(" ", nameFile);

            var i = 0;
            while (i < (numOfCompOnThisPath - 1))
            {
                var newPattern = new MyPatternOfComponents();
                var j = i;
                var foundNewPattern = KLGetMaximumTranslation_Assembly(listOfComponentsOnThePath, pathObject, ref j, ref numOfCompOnThisPath,
                    ref noStop, ref newPattern, SwApplication);
                if (foundNewPattern == false)
                {
                    var pathCircumference = (MyCircumForPath) pathObject;

                    foundNewPattern = KLGetMaximumRotation_Assembly(listOfComponentsOnThePath, pathCircumference,
                        ref i, ref numOfCompOnThisPath,
                        ref noStop, ref newPattern, SwModel, SwApplication);

                }
                else
                {
                    i = j;
                }

                if (foundNewPattern)
                {
                    //KLdebug.Print("foundNewPattern vero", nameFile);
                    if (newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath ||
                        newPattern.listOfMyRCOfMyPattern.Count == numOfCompOnThisPath - 1)
                    {
                        //KLdebug.Print("foundNewPattern vero no stop", nameFile);
                        noStop = true;
                    }

                    KLCheckAndUpdate_Assembly(newPattern, ref listOfPathOfPoints,
                           listOfOrigins, ref listOfMatrAdj,
                           ref listOfOutputPattern, ref listOfOutputPatternTwo);
                }

            }

            //KLdebug.Print(" ", nameFile);
            //KLdebug.Print("FINE LISTA :) ", nameFile);

            if (noStop)
            {
            // KLdebug.Print("NESSUNA INTERRUZIONE: PATTERN DI LUNGHEZZA MASSIMA SU QUESTO PATH!", nameFile);
                return true;
            }
            return false;

        }
    }


}
