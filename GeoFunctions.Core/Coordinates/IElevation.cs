using System;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IElevation : IFormattable

    {
    double Value { get; set; }
    DistanceMeasurement DistanceMeasurement { get; }

    double ToCentimeters();
    double ToFeet();
    double ToKilometers();
    double ToMeters();
    double ToMillimeters();
    double ToNauticalMiles();
    }
}
