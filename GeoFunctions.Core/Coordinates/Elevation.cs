using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public class Elevation : IElevation
    {
        private const double ConversionRatio = 0.3048;

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (Math.Abs(value) > 1.0E+10)
                {
                    var errorMessage = string.Format("Value must be between -1.0E+10 and 1.0E+10. {0} is an invalid number",
                                                     value.ToString(CultureInfo.InvariantCulture));
                    throw new ArgumentException(errorMessage);
                }

                _value = value;
            }
        }

        public ElevationMeasurement ElevationMeasurement { get; protected set; }
        
        public Elevation(double elevation = 0.0, ElevationMeasurement measurement = ElevationMeasurement.Feet)
        {
            Value = elevation;
            ElevationMeasurement = measurement;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Elevation))
            {
                return false;
            }

            var testObject = (Elevation) obj;

            return Value.Equals(testObject.Value) && ElevationMeasurement == testObject.ElevationMeasurement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, ElevationMeasurement);
        }

        public override string ToString()
        {
            var measurementSymbol = ElevationMeasurement == ElevationMeasurement.Feet ? "'" : " m";
            return $"{Value.ToString(CultureInfo.CurrentCulture)}{measurementSymbol}";
        }

        public string ToString(string format, IFormatProvider formatProvider) // TODO: Code this method
        {
            throw new NotImplementedException();
        }

        public static double ToFeet(double valueInMeters)
        {
            return valueInMeters / ConversionRatio;
        }

        public double ToFeet()
        {
            return ElevationMeasurement == ElevationMeasurement.Feet ? Value : ToFeet(Value);
        }

        public static double ToMeters(double valueInFeet)
        {
            return valueInFeet * ConversionRatio;
        }

        public double ToMeters()
        {
            return ElevationMeasurement == ElevationMeasurement.Meters ? Value : ToMeters(Value);
        }
    }
}
