using GeoFunctions.Core.Measurement;
using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class Angle : IAngle
    {
        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value))
                {
                    throw new ArgumentException(value.ToString(CultureInfo.InvariantCulture));
                }

                _value = value;
            }
        }

        public double UnitCircleValue //TODO: Unit test this
        {
            get
            {
                var modulas = AngleMeasurement == AngleMeasurement.Degrees ? 360.0 : Math.PI;
                return Value % modulas;
            }
        }

        public AngleMeasurement AngleMeasurement { get; set; }

        public Angle() { }

        public Angle(double value)
        {
            Value = value;
        }

        public Angle(double value, AngleMeasurement measurment)
        {
            Value = value;
            AngleMeasurement = measurment;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Angle))
            {
                return false;
            }

            var testCoordinate = (Angle) obj;

            return UnitCircleValue.Equals(testCoordinate.UnitCircleValue) && AngleMeasurement == testCoordinate.AngleMeasurement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, AngleMeasurement);
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
