using System;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface IDistance : IFormattable

    {
    double Value { get; set; }
    DistanceMeasurement DistanceMeasurement { get; }

    double ToCentimeters();
    double ToFeet();
    double ToInches();
    double ToKilometers();
    double ToMeters();
    double ToMiles();
    double ToMillimeters();
    double ToNauticalMiles();
    double ToYards();
    }
}
