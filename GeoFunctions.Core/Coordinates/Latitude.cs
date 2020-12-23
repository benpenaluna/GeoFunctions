using GeoFunctions.Core.Coordinates.Measurement;
using GeoFunctions.Core.Coordinates.Structs;
using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class Latitude : SphericalCoordinate
    {
        private IAngle _angle;

        public sealed override IAngle Angle
        {
            get => _angle;
            set
            {
                var maxValue = value.AngleMeasurement == AngleMeasurement.Degrees ? 90.0 : Math.PI / 2.0;
                if (Math.Abs(value.Value) > maxValue)
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _angle = value;
            }
        }

        public override DmsCoordinate DmsCoordinate => CalculateDmsCoordinate(Angle.Value >= 0.0 ? Hemisphere.North : Hemisphere.South);

        public Latitude()
        {
            Angle = new Angle();
        }

        public Latitude(double angle, AngleMeasurement measurement = AngleMeasurement.Degrees)
        {
            Angle = new Angle(angle, measurement);
        }

        public Latitude(IAngle angle)
        {
            Angle = new Angle(angle.Value, angle.AngleMeasurement);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Latitude) && Equals((Latitude)obj);
        }

        protected bool Equals(Latitude other)
        {
            return Equals(Angle, other.Angle);
        }

        public override int GetHashCode()
        {
            return (Angle != null ? Angle.GetHashCode() : 0);
        }
    }
}
