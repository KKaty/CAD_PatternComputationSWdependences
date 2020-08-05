using System;
using AssemblyRetrieval.PatternLisa.ClassesOfObjects;

namespace AssemblyRetrieval.PatternLisa.GeometricUtilities
{
    public static partial class FunctionsLC
    {
        //Restituisce le equazioni cartesiane della retta passante per V1, V2
        public static MyLine LinePassingThrough(MyVertex V1, MyVertex V2)
        {
            //var whatToWrite = string.Format("LinePassingThrough:");
            //KLdebug.Print(whatToWrite, "LinePassingThrough.txt");

            //manca il controllo se V1==V2           
            var tolerance = Math.Pow(10, -5);
            if (Math.Abs(V1.x -V2.x)<tolerance)
            {
                MyPlane FirstPlane = new MyPlane(1, 0, 0, -V1.x);
                if (V1.y == V2.y)
                {
                    MyPlane SecondPlane = new MyPlane(0, 1, 0, -V1.y);
                    MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                    return OutputLine;
                }
                else
                {
                    if (Math.Abs(V1.z - V2.z)<tolerance)
                    {

                        MyPlane SecondPlane = new MyPlane(0, 0, 1, -V1.z);
                        MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                        return OutputLine;
                    }
                    else
                    {
                        MyPlane SecondPlane = new MyPlane(0, -(V2.z - V1.z) / (V2.y - V1.y), 1, -V1.z + V1.y * (V2.z - V1.z) / (V2.y - V1.y));
                        MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                        return OutputLine;

                        
                    }
                }
            }
            else
            {
                if (Math.Abs(V1.y - V2.y)<tolerance)
                {
                    MyPlane FirstPlane = new MyPlane(0, 1, 0, -V1.y);
                    if (Math.Abs(V1.z - V2.z)<tolerance)
                    {

                        MyPlane SecondPlane = new MyPlane(0, 0, 1, -V1.z);
                        MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                        return OutputLine;
                    }
                    else
                    {

                        MyPlane SecondPlane = new MyPlane(-(V2.z - V1.z) / (V2.x - V1.x), 0, 1, -V1.z + V1.x * (V2.z - V1.z) / (V2.x - V1.x));
                        MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                        return OutputLine;
                    }
                }
                else
                {

                    MyPlane FirstPlane = new MyPlane(-(V2.y - V1.y) / (V2.x - V1.x), 1, 0, -V1.y + V1.x * (V2.y - V1.y) / (V2.x - V1.x));
                    MyPlane SecondPlane = new MyPlane(-(V2.z - V1.z) / (V2.x - V1.x), 0, 1, -V1.z + V1.x * (V2.z - V1.z) / (V2.x - V1.x));
                    MyLine OutputLine = new MyLine(FirstPlane, SecondPlane);
                    return OutputLine;
                }
            }
            
            
        }
    }
}
