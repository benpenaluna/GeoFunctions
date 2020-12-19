namespace GeoFunctions.Core.Coordinates
{
    public interface IBearingDistance
    {
        IDistance Distance { get; set; }

        IAngle InitialBearing { get; set; }

        IAngle FinalBearing { get; set; }
    }
}
