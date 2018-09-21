using System;
using System.Collections.Generic;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public class GeographicCoordinate : IGeographicCoordinate
    {
        public ISphericalCoordinate Latitude { get; set; }

        public ISphericalCoordinate Longitude { get; set; }

        public IElevation Elevation { get; set; }

        public GeographicCoordinate()
        {
            Initialise(new Latitude(), new Longitude(), new Elevation());
        }

        public GeographicCoordinate(ISphericalCoordinate latitude, ISphericalCoordinate longitude)
        {
            Initialise(latitude, longitude, new Elevation());
        }
        
        public GeographicCoordinate(double latitude, double longitude, double elevation = 0)
        {
            Initialise(new Latitude(latitude), new Longitude(longitude), new Elevation(elevation));
        }

        public GeographicCoordinate(ISphericalCoordinate latitude, ISphericalCoordinate longitude, IElevation elevation)
        {
            Initialise(latitude, longitude, elevation);
        }

        private void Initialise(ISphericalCoordinate latitude, ISphericalCoordinate longitude, IElevation elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Latitude.ToString(), Longitude.ToString());
        }

        public string ToString(string format, IFormatProvider formatProvider) // TODO: Refactor!
        {
            if (format is null)
                return ToString();

            if (formatProvider is null)
                formatProvider = CultureInfo.InvariantCulture;

            var splits = format.Split(new[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var split in splits)
            {
                if (split.Contains("lat:"))
                {
                    var latFormat = split.Substring(4, split.Length - 4);
                    var latFormatted = Latitude.ToString(latFormat, formatProvider);
                    format = format.Replace(split, latFormatted);
                }
                else if (split.Contains("lon:"))
                {
                    var lonFormat = split.Substring(4, split.Length - 4);
                    var lonFormatted = Longitude.ToString(lonFormat, formatProvider);
                    format = format.Replace(split, lonFormatted);
                }
            }

            format = format.Replace("[", "");
            format = format.Replace("]", "");
            return format;
        }
    }
}
