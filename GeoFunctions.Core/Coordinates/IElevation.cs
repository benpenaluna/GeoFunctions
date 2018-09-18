using System;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IElevation : IFormattable

    {
    double Value { get; set; }
    ElevationMeasurement ElevationMeasurement { get; }

    double ToFeet();
    double ToMeters();
    }
}
