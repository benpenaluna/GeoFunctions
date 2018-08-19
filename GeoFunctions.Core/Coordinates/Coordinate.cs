using GeoFunctions.Core.Measurement;
using System;

namespace GeoFunctions.Core.Coordinates
{
    public class Coordinate : ICoordinate
    {
        public double Value { get; set; }

        public AngleMeasurement AngleMeasurement { get; set; }

        public Coordinate() { }

        public Coordinate(double value)
        {
            Value = value;
        }

        public Coordinate(double value, AngleMeasurement measurment)
        {
            Value = value;
            AngleMeasurement = measurment;
        }

        public double ToDegrees()
        {
            return AngleMeasurement == AngleMeasurement.Degrees ? Value : ToDegrees(Value);
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        public double ToRadians()
        {
            return AngleMeasurement == AngleMeasurement.Radians ? Value : ToRadians(Value);
        }

        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
