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


    }
}
