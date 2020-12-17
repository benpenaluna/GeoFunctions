using System;
using System.Globalization;
using GeoFunctions.Core.Common;

namespace GeoFunctions.Core.Coordinates
{
    public class GeographicCoordinate : IGeographicCoordinate
    {
        public ISphericalCoordinate Latitude { get; set; }

        public ISphericalCoordinate Longitude { get; set; }

        public IDistance Elevation { get; set; }

        public GeographicCoordinate()
        {
            Initialise(new Latitude(), new Longitude(), new Distance());
        }

        public GeographicCoordinate(ISphericalCoordinate latitude, ISphericalCoordinate longitude)
        {
            Initialise(latitude, longitude, new Distance());
        }
        
        public GeographicCoordinate(double latitude, double longitude, double elevation = 0)
        {
            Initialise(new Latitude(latitude), new Longitude(longitude), new Distance(elevation));
        }

        public GeographicCoordinate(ISphericalCoordinate latitude, ISphericalCoordinate longitude, IDistance elevation)
        {
            Initialise(latitude, longitude, elevation);
        }

        private void Initialise(ISphericalCoordinate latitude, ISphericalCoordinate longitude, IDistance elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public override string ToString()
        {
            return $"{Latitude.ToString()} {Longitude.ToString()}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is null)
                return ToString();

            if (formatProvider is null)
                formatProvider = CultureInfo.InvariantCulture;

            return CustomFormat(format, formatProvider);
        }

        private string CustomFormat(string format, IFormatProvider formatProvider)
        {
            var splits = format.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var split in splits)
            {
                if (split.ToLower().StartsWith("lat:"))
                {
                    format = split.Format(Latitude, format, formatProvider);
                }
                else if (split.ToLower().StartsWith("lon:"))
                {
                    format = split.Format(Longitude, format, formatProvider);
                }
                else if (split.ToLower().StartsWith("ele:"))
                {
                    format = split.Format(Elevation, format, formatProvider);
                }
            }

            format = format.Replace("[", "");
            format = format.Replace("]", "");

            return format;
        }
    }
}
