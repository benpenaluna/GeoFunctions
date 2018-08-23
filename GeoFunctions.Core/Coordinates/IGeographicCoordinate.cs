namespace GeoFunctions.Core.Coordinates
{
    public interface IGeographicCoordinate
    {
        IAngle Latitude { get; set; }
        IAngle Longitude { get; set; }
        double Elevation { get; set; }
    }
}
