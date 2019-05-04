using System;
using System.Globalization;
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
            IElevation elevation = new Elevation(96, DistanceMeasurement.Meters);

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

        [Theory]
        [InlineData("[lat:DD° MM' SS\"H] [lon:DDD° MM' SS\"H]", "37° 41' 19\"S 144° 59' 59\"E")]
        [InlineData("[LAT:DD° MM' SS\"H] [lon:DDD° MM' SS\"H]", "37° 41' 19\"S 144° 59' 59\"E")]
        [InlineData("[lat:DD° MM' SS\"H] [LON:DDD° MM' SS\"H]", "37° 41' 19\"S 144° 59' 59\"E")]
        [InlineData("[lon:DDD° MM' SS\"H],[lat:DD° MM' SS\"H]", "144° 59' 59\"E,37° 41' 19\"S")]
        [InlineData("[lat:DD° MM' SS.ss\"H] [lon:DDD° MM' SS.ss\"H]", "37° 41' 18.95\"S 144° 59' 58.70\"E")]
        [InlineData("[lat:DD° MM.mm'H] [lon:DDD° MM.mm'H]", "37° 41.32'S 144° 59.98'E")]
        [InlineData("[lat:DD.dd°H] [lon:DDD.dd°H]", "37.69°S 145.00°E")]
        [InlineData("[lat:DD.dd°],[lon:DDD.dd°]", "-37.69°,145.00°")]
        [InlineData("[lid:DD.dd°],[lon:DDD.dd°]", "lid:DD.dd°,145.00°")]
        [InlineData("[lat:lon:DD° MM' SS.ss\"H] [lon:DDD° MM' SS.ss\"H]", "LON:37° 41' 18.95\"S 144° 59' 58.70\"E")]
        [InlineData("[lat:DD° MM' SS.ss\"H] [lon:lat:DDD° MM' SS.ss\"H]", "37° 41' 18.95\"S LAT:144° 59' 58.70\"E")]
        [InlineData("[Alat:DD° MM' SS.ss\"H] [lon:DDD° MM' SS.ss\"H]", "Alat:DD° MM' SS.ss\"H 144° 59' 58.70\"E")]
        [InlineData("[lat:DD° MM' SS\"H] [lon:DDD° MM' SS\"H] [ele:t u]", "37° 41' 19\"S 144° 59' 59\"E 119 m")]
        [InlineData("[lon:DDD.dddddddddddd],[lat:DD.ddddddddddddd],[ele:f.ffffffff]", "144.999637777534,-37.6885966980243,389.31430446")]
        [InlineData("[lon:DDD.dddddddddddd],[lat:DD.ddddddddddddd],[lat:ele:f.ffffffff]", "144.999637777534,-37.6885966980243,ELE:F.FFFFFFFF")]
        public void GeographicCoordinate_CorrectlyParsesDefaultFormatStringWithFormat(string format, string expected)
        {
            ISphericalCoordinate latitude = new Latitude(-37.6885966980243);
            ISphericalCoordinate longitude = new Longitude(144.999637777534);
            IElevation elevation = new Elevation(118.663,DistanceMeasurement.Meters);
            IGeographicCoordinate sut = new GeographicCoordinate(latitude, longitude, elevation);
            var result = sut.ToString(format, CultureInfo.InvariantCulture);

            Assert.Equal(expected, result);
        }

        private static GeographicCoordinate InstantiateNewGeographicCoordinate()
        {
            return new GeographicCoordinate();
        }
    }
}
