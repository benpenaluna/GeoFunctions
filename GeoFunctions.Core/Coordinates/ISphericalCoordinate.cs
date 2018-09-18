using System;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public interface ISphericalCoordinate : IFormattable
    {
        IAngle Angle { get; set; }

        DmsCoordinate DmsCoordinate { get; }
    }
}
