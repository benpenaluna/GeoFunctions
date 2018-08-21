namespace GeoFunctions.Core.Coordinates
{
    public interface IGeographicCoordinate
    {
        Angle Latitude { get; set; }
        Angle Longitude { get; set; }
        double Elevation { get; set; }
    }
}
