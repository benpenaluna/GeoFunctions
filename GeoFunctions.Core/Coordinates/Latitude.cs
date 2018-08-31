using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Measurement;
using GeoFunctions.Core.Coordinates.Structs;

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

        public override DmsCoordinate DmsCoordinate => CalculateDmsCoordinate(Angle.Value >= 0.0 ? Hemisphere.North : Hemisphere.South); // TODO: Unit Test this method

        public Latitude()
        {
            Angle = new Angle();
        }

        public Latitude(double angle)
        {
            Angle = new Angle(angle);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Latitude) && Equals((Latitude) obj);
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
