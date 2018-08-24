using GeoFunctions.Core.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IAngle
    {
        double Value { get; set; }
        double CoTerminalValue { get; }
        AngleMeasurement AngleMeasurement { get; }

        double ToDegrees();
        double ToRadians();
    }
}
