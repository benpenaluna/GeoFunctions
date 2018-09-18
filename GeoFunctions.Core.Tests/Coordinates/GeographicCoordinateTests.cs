using System;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class GeographicCoordinateTests
    {
        [Fact]
        public void GeographicCoordinate_CanInstantiate()
        {
            IGeographicCoordinate result = InstantiateNewGeographicCoordinate();
            Assert.NotNull(result);
        }

        [Fact]
        public void GeographicCoordinate_CanInstantiateWithLatLong()
        {
            ISphericalCoordinate latitude = new Latitude(-37.1);
            ISphericalCoordinate longitude = new Longitude(144.9);

            IGeographicCoordinate result = new GeographicCoordinate(latitude, longitude);
            Assert.NotNull(result);
        }

        [Fact]
        public void GeographicCoordinate_CanInstantiateWithLatLongDoubles()
        {
            IGeographicCoordinate result = new GeographicCoordinate(-37.1, 144.9);
            Assert.NotNull(result);
        }

        [Fact]
        public void GeographicCoordinate_CanInstantiateWithLatLongElevation()
        {
            ISphericalCoordinate latitude = new Latitude(-37.1);
            ISphericalCoordinate longitude = new Longitude(144.9);
            IElevation elevation = new Elevation(96, ElevationMeasurement.Meters);

            IGeographicCoordinate result = new GeographicCoordinate(latitude, longitude, elevation);
            Assert.NotNull(result);
        }

        [Fact]
        public void GeographicCoordinate_CanInstantiateWithLatLongElevationDoubles()
        {
            IGeographicCoordinate result = new GeographicCoordinate(-37.1, 144.9, 96);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(90.0)]
        public void GeographicCoordinate_CanUpdateValidLatitude(double angle)
        {
            ISphericalCoordinate expected = new Latitude(angle);

            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            sut.Latitude = new Latitude(angle);
            var result = sut.Latitude;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-90.1)]
        [InlineData(90.1)]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        public void GeographicCoordinate_CannotUpdateInvalidLatitude(double angle)
        {
            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Latitude = new Latitude(angle));
        }

        [Theory]
        [InlineData(-179.9)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(180.0)]
        public void GeographicCoordinate_CanUpdateValidLongitude(double longitude)
        {
            ISphericalCoordinate expected = new Longitude(longitude);

            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            sut.Longitude = new Longitude(longitude);
            var result = sut.Longitude;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-180.0)]
        [InlineData(180.1)]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        public void GeographicCoordinate_CannotUpdateInvalidLongitude(double angle)
        {
            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Longitude = new Longitude(angle));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(180.0)]
        [InlineData(1.0E+10)]
        public void GeographicCoordinate_CanUpdateValidElevation(double elevation)
        {
            IElevation expected = new Elevation(elevation);

            var sut = InstantiateNewGeographicCoordinate();
            sut.Elevation.Value = elevation;
            var result = sut.Elevation;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GeographicCoordinate_CorrectlyParsesDefaultFormatString()
        {
            const string expected = "37° 41' 19\"S 144° 59' 59\"E";

            ISphericalCoordinate latitude = new Latitude(-37.6885966980243);
            ISphericalCoordinate longitude = new Longitude(144.999637777534);
            IGeographicCoordinate sut = new GeographicCoordinate(latitude, longitude);
            var result = sut.ToString();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GeographicCoordinate_CorrectlyParsesDefaultFormatStringNoFormat()
        {
            const string expected = "37° 41' 19\"S 144° 59' 59\"E";

            ISphericalCoordinate latitude = new Latitude(-37.6885966980243);
            ISphericalCoordinate longitude = new Longitude(144.999637777534);
            IGeographicCoordinate sut = new GeographicCoordinate(latitude, longitude);
            var result = sut.ToString(null, null);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GeographicCoordinate_CorrectlyParsesDefaultFormatStringWithFormat()
        {
            const string expected = "37° 41' 19\"S 144° 59' 59\"E";
            const string latFormat = "DD° MM' SS\"H";
            const string lonFormat = "DDD° MM' SS\"H";

            ISphericalCoordinate latitude = new Latitude(-37.6885966980243);
            ISphericalCoordinate longitude = new Longitude(144.999637777534);
            IGeographicCoordinate sut = new GeographicCoordinate(latitude, longitude);
            var result = sut.ToString(latFormat, lonFormat);

            Assert.Equal(expected, result);
        }

        private static GeographicCoordinate InstantiateNewGeographicCoordinate()
        {
            return new GeographicCoordinate();
        }
    }
}
