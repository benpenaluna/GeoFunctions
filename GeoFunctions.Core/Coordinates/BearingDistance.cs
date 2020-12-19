namespace GeoFunctions.Core.Coordinates
{
    public class BearingDistance : IBearingDistance
    {
        public IDistance Distance { get; set; }

        public IAngle InitialBearing { get; set; }

        public IAngle FinalBearing { get; set; }

        public BearingDistance()
        {
            Distance = new Distance();
            InitialBearing = new Angle();
            FinalBearing = new Angle();
        }
    }
}
