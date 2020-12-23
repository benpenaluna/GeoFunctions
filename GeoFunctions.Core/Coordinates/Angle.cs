using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public class Angle : IAngle
    {
        private const double MaxValueToEnsurePrecision = 1.0E+10;

        private double Modulus => AngleMeasurement == AngleMeasurement.Degrees ? 360.0 : 2.0 * Math.PI;

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value) || Math.Abs(value) > MaxValueToEnsurePrecision)
                {
                    var errorMessage = $"Value must be between -1.0E+10 and 1.0E+10. {value.ToString(CultureInfo.InvariantCulture)} is an invalid number";
                    throw new ArgumentOutOfRangeException(errorMessage);
                }
                
                _value = value;
            }
        }

        public double CoTerminalValue => AbsoluteValueLessThanModulus() ? Value : CalculateCoTerminalValue();

        private bool AbsoluteValueLessThanModulus()
        {
            return Value >= 0.0 && Value < Modulus;
        }

        private double CalculateCoTerminalValue()
        {
            var normalizationFactor = Value / Modulus;
            var baseOfNormalizationFactor = Math.Floor(normalizationFactor);
            var coTangent = Value - Modulus * baseOfNormalizationFactor;

            return coTangent >= 0 ? coTangent : coTangent + Modulus;
        }

        public AngleMeasurement AngleMeasurement { get; protected set; }

        public Angle(double value = 0.0, AngleMeasurement measurement = AngleMeasurement.Degrees)
        {
            Value = value;
            AngleMeasurement = measurement;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Angle) && obj != null)
            {
                return false;
            }

            var testCoordinate = (Angle) obj;

            return testCoordinate != null && (CoTerminalValue.Equals(testCoordinate.CoTerminalValue) && AngleMeasurement == testCoordinate.AngleMeasurement);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator == (Angle a, Angle b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Angle a, Angle b)
        {
            return !(a == b);
        }

        public object Clone()
        {
            return new Angle(Value, AngleMeasurement);
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
