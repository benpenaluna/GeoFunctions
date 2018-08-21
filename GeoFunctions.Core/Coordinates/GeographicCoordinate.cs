using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class GeographicCoordinate : IGeographicCoordinate
    {
        private Angle _latitude;

        private Angle _longitude;

        private double _elevation;

        public Angle Latitude
        {
            get => _latitude;
            set
            {
                if (Math.Abs(value.Value) > 90.0 )
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _latitude = value;
            }
        }

        public Angle Longitude
        {
            get => _longitude;
            set
            {
                if (value.Value <= -180.0 || value.Value > 180.0)
                    throw new ArgumentOutOfRangeException(value.Value.ToString(CultureInfo.InvariantCulture));

                _longitude = value;
            }
        }

        public double Elevation
        {
            get => _elevation;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));

                _elevation = value;
            }
        }

        public GeographicCoordinate()
        {
            Latitude = new Angle();
        }
        
    }
}
