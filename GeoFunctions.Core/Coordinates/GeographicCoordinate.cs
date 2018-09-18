using System.Globalization;

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

        public string ToString(string latitudeFormat, string longitudeFormat)
        {
            return string.Format("{0} {1}", 
                Latitude.ToString(latitudeFormat, CultureInfo.CurrentCulture), 
                Longitude.ToString(longitudeFormat, CultureInfo.CurrentCulture));
        }
    }
}
