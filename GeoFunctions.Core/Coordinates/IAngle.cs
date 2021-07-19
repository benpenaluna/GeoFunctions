using System;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IAngle : ICloneable
    {
        double Value { get; set; }
        double CoTerminalValue { get; }
        AngleMeasurement AngleMeasurement { get; }


        event EventHandler<ValueChangedEventArgs> Value_Changed;

        double ToDegrees();
        double ToRadians();
    }
}
