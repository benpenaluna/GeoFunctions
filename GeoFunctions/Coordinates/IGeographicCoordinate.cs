namespace GeoFunctions.Standard.Coordinates
{
    public interface IGeographicCoordinate
    {
        double Latitude { get; set; }
        double Longitude { get; set; }
        double Elevation { get; set; }
    }
}
