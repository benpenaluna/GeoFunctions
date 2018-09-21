using System;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public interface IGeographicCoordinate : IFormattable 
    {
        ISphericalCoordinate Latitude { get; set; }
        ISphericalCoordinate Longitude { get; set; }
        IElevation Elevation { get; set; }
    }
}
