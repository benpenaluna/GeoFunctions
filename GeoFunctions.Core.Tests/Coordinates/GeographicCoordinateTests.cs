using System;
using GeoFunctions.Core.Coordinates;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class GeographicCoordinateTests
    {
        [Fact]
        public void GeographicCoordinate_CanInstantiate()
        {
            var result = InstantiateNewGeographicCoordinate();
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(90.0)]
        public void GeographicCoordinate_CanUpdateValidLatitude(double latitude)
        {
            var expected = new Angle(latitude);

            var sut = InstantiateNewGeographicCoordinate();
            sut.Latitude = new Angle(latitude);
            var result = sut.Latitude;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-90.1)]
        [InlineData(90.1)]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        public void GeographicCoordinate_CannotUpdateInvalidLatitude(double latitude)
        {
            var sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Latitude = new Angle(latitude));
        }

        [Theory]
        [InlineData(-179.9)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(180.0)]
        public void GeographicCoordinate_CanUpdateValidLongitude(double longitude)
        {
            var expected = new Angle(longitude);

            var sut = InstantiateNewGeographicCoordinate();
            sut.Longitude = new Angle(longitude);
            var result = sut.Longitude;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-180.0)]
        [InlineData(180.1)]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        public void GeographicCoordinate_CannotUpdateInvalidLongitude(double longitude)
        {
            var sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Longitude = new Angle(longitude));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(180.0)]
        [InlineData(double.MaxValue)]
        public void GeographicCoordinate_CanUpdateValidLongitudeElevation(double elevation)
        {
            var expected = elevation;

            var sut = InstantiateNewGeographicCoordinate();
            sut.Elevation = elevation;
            var result = sut.Elevation;

            Assert.Equal(expected, result);
        }

        private static GeographicCoordinate InstantiateNewGeographicCoordinate()
        {
            return new GeographicCoordinate();
        }
    }
}
