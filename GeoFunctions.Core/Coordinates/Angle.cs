using GeoFunctions.Core.Measurement;
using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class Angle : IAngle
    {
        private const double MaxValue = 1.0E+10;

        private double Modulas => AngleMeasurement == AngleMeasurement.Degrees ? 360.0 : 2.0 * Math.PI;

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (double.IsNaN(value) || Math.Abs(value) > MaxValue)
                {
                    throw new ArgumentException(value.ToString(CultureInfo.InvariantCulture));
                }
                
                _value = value;
            }
        }

        public double CoTerminalValue => AbsoluteValueLessThanModulas() ? Value : CalculateCoTerminalValue();

        private bool AbsoluteValueLessThanModulas()
        {
            return Value >= 0.0 && Value < Modulas;
        }

        private double CalculateCoTerminalValue()
        {
            var normiisationFactor = Value / Modulas;
            var baseOfNormilisationFactor = Math.Floor(normiisationFactor);
            var coTangent = Value - Modulas * baseOfNormilisationFactor;

            return coTangent >= 0 ? coTangent : coTangent + Modulas;
        }

        public AngleMeasurement AngleMeasurement { get; private set; }

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
            if (!(obj is Angle) && obj != null)
            {
                return false;
            }

            var testCoordinate = (Angle) obj;

            return CoTerminalValue.Equals(testCoordinate.CoTerminalValue) && AngleMeasurement == testCoordinate.AngleMeasurement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, AngleMeasurement);
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
