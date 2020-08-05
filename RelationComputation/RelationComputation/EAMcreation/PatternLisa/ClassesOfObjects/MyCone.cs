namespace AssemblyRetrieval.PatternLisa.ClassesOfObjects

    // DA SISTEMARE!!!!
{
    public class MyCone
    {
        public double[] originCone;
        public double[] axisCone;
      //public double radiusCone;    potrebbe essere raggio del cerchio di base, quindi forse diverso per ogni faccia conica riferita allo stesso cono
        public double halfAngleCone;

        public MyCone()
        {
        }

        public MyCone(double[] OriginCone, double[] AxisCone, double HalfAngleCone)
        {
            this.originCone = OriginCone;
            this.axisCone = AxisCone;
            this.halfAngleCone = HalfAngleCone;
        }

        protected bool Equals(MyCone other)
        {
            return Equals(originCone, other.originCone) && Equals(axisCone, other.axisCone) && halfAngleCone.Equals(other.halfAngleCone);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (originCone != null ? originCone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (axisCone != null ? axisCone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ halfAngleCone.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MyCone) obj);
        }
    }
}


// L'EQUALS DEL CONO NON E' PROPRIO GIUSTO:
// ORIGIN CONE SEMBRA ESSERE IL CENTRO DELLA BASE DEL CONO, E QUINDI DIPENDE SEMPRE DA DOVE
// IL CONO GEOMETRICO è STATO "TAGLIATO". L'UNICO PUNTO INDIPENDENTE DA DOVE VIENE TAGLIATO
// IL CONO SAREBBE IL VERTICE DEL CONO (LA PUNTA) MA è DIFFICILE DA CALCOLARE.
// IN QUESTO CASO MI LIMITO AD UN CONTROLLO APPROSSIMATIVO, AFFERMANDO CHE 
// DUE CONI SONO UGUALI SE HANNO ORIGIN UGUALE (ANCHE SE NON è VERO),
// MA AL MOMENTO DOVREBBE BASTARE.