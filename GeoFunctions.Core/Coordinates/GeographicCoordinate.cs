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
                if (Math.Abs(value.Value) > 90.0 )
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _latitude = value;
            }
        }

        public IAngle Longitude
        {
            get => _longitude;
            set
            {
                if (value.Value <= -180.0 || value.Value > 180.0)
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _longitude = value;
            }
        }

        public double Elevation { get; set; }

        public GeographicCoordinate()
        {
            Latitude = new Angle();
            Longitude = new Angle();
        } 
    }
}
