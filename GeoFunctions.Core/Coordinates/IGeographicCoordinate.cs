namespace GeoFunctions.Core.Coordinates
{
    public interface IGeographicCoordinate
    {
        ISphericalCoordinate Latitude { get; set; }
        ISphericalCoordinate Longitude { get; set; }
        IElevation Elevation { get; set; }

        string ToString(string latitudeFormat, string longitudeFormat);
    }
}
