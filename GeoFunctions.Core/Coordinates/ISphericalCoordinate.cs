using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public interface ISphericalCoordinate
    {
        IAngle Angle { get; set; }

        DmsCoordinate DmsCoordinate { get; }
    }
}
