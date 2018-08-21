using System.Dynamic;
using GeoFunctions.Core.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IAngle
    {
        double Value { get; set; }
        double UnitCircleValue { get; }
        AngleMeasurement AngleMeasurement { get; set; }
    }
}
