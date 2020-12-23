using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Measurement;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public class Longitude : SphericalCoordinate 
    {
        private IAngle _angle;

        public sealed override IAngle Angle
        {
            get => _angle;
            set
            {
                var maxValue = value.AngleMeasurement == AngleMeasurement.Degrees ? 180.0 : Math.PI;
                if (value.Value <= -1.0 * maxValue || value.Value > maxValue)
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _angle = value;
            }
        }

        public override DmsCoordinate DmsCoordinate => CalculateDmsCoordinate(Angle.Value >= 0.0 ? Hemisphere.East : Hemisphere.West); // TODO: Unit Test this method

        public Longitude()
        {
            Angle = new Angle();
        }

        public Longitude(double angle, AngleMeasurement measurement = AngleMeasurement.Degrees)
        {
            Angle = new Angle(angle, measurement);
        }

        public Longitude(IAngle angle)
        {
            Angle = new Angle(angle.Value, angle.AngleMeasurement);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Longitude) && Equals((Longitude) obj);
        }

        protected bool Equals(Longitude other)
        {
            return Equals(Angle, other.Angle);
        }

        public override int GetHashCode()
        {
            return (Angle != null ? Angle.GetHashCode() : 0);
        }
    }
}
