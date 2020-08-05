namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects
{
    public class MyCircle
    {
        public MyVertex centerCircle;
        public double radiusCircle;

        public MyCircle()
        {
        }

        public MyCircle(double[] CircleParameters)//Curve myCircle
        {
            //double[] circleParam = myCircle.CircleParams();
            //double[] centerCircleArray = { circleParam[0], circleParam[1], circleParam[2] };
            this.centerCircle = new MyVertex(CircleParameters[0], CircleParameters[1], CircleParameters[2]);
            this.radiusCircle = CircleParameters[6];
            
        }

    }
}
