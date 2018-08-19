using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates
{
    public class GeographicCoordinate : IGeographicCoordinate
    {
        private double _latitude;

        private double _longitude;

        private double _elevation;

        public double Latitude
        {
            get => _latitude;
            set
            {
                if (Math.Abs(value) > 90.0 )
                    throw new ArgumentOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));

                _latitude = value;
            }
        }

        public double Longitude
        {
            get => _longitude;
            set
            {
                if (value <= -180.0 || value > 180.0)
                    throw new ArgumentOutOfRangeException(value.ToString(CultureInfo.InvariantCulture));

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


        
    }
}
