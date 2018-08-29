using GeoFunctions.Core.Measurement;
using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class GeographicCoordinate : IGeographicCoordinate
    {
        private IAngle _latitude;

        private IAngle _longitude;

        public IAngle Latitude
        {
            get => _latitude;
            set
            {
                var maxValue = value.AngleMeasurement == AngleMeasurement.Degrees ? 90.0 : Math.PI / 2.0;
                if (Math.Abs(value.Value) > maxValue )
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _latitude = value;
            }
        }

        public IAngle Longitude
        {
            get => _longitude;
            set
            {
                var maxValue = value.AngleMeasurement == AngleMeasurement.Degrees ? 180.0 : Math.PI;
                if (value.Value <= -1.0 * maxValue  || value.Value > maxValue)
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _longitude = value;
            }
        }

        public IElevation Elevation { get; set; }

        public GeographicCoordinate()
        {
            Initialise(new Angle(), new Angle(), new Elevation());
        }

        public GeographicCoordinate(IAngle latitude, IAngle longitude)
        {
            Initialise(latitude, longitude, new Elevation());
        }

        public GeographicCoordinate(IAngle latitude, IAngle longitude, IElevation elevation)
        {
            Initialise(latitude, longitude, elevation);
        }

        private void Initialise(IAngle latitude, IAngle longitude, IElevation elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }
    }
}
