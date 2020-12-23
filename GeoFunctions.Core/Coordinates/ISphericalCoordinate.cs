using GeoFunctions.Core.Coordinates.Structs;
using System;

namespace GeoFunctions.Core.Coordinates
{
    public interface ISphericalCoordinate : IFormattable
    {
        IAngle Angle { get; set; }

        DmsCoordinate DmsCoordinate { get; }
    }
}
