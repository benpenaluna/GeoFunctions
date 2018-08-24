using System;
using System.Globalization;
using GeoFunctions.Core.Measurement;

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
                    throw new ArgumentException(value.ToString(CultureInfo.InvariantCulture));
                }

                _value = value;
            }
        }

        public ElevationMeasurement ElevationMeasurement { get; internal set; }
        
        public Elevation()
        {
            Initialise(0.0, ElevationMeasurement.Feet);
        }

        public Elevation(double elevation)
        {
            Initialise(elevation, ElevationMeasurement.Feet);
        }

        public Elevation(double elevation, ElevationMeasurement measurment)
        {
            Initialise(elevation, measurment);
        }

        private void Initialise(double elevation, ElevationMeasurement measurment)
        {
            Value = elevation;
            ElevationMeasurement = measurment;
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
            unchecked
            {
                return (_value.GetHashCode() * 397) ^ (int) ElevationMeasurement;
            }
        }

        public double ToFeet()
        {
            return Value / ConversionRatio;
        }

        public double ToMeters()
        {
            return Value * ConversionRatio;
        }
    }
}
