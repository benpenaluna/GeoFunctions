using System;

namespace GeoFunctions.Core.Coordinates
{
    public interface IGeographicCoordinate : IFormattable
    {
        ISphericalCoordinate Latitude { get; set; }
        ISphericalCoordinate Longitude { get; set; }
        IDistance Elevation { get; set; }
    }
}
