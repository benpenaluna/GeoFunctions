using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GeoFunctions.Core.Coordinates.Structs;
using GeoFunctions.Core.Common;

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
