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
            IAngle latitude = new Angle(-37.1);
            IAngle longitude = new Angle(144.9);

            IGeographicCoordinate result = new GeographicCoordinate(latitude, longitude);
            Assert.NotNull(result);
        }

        [Fact]
        public void GeographicCoordinate_CanInstantiateWithLatLongElevation()
        {
            IAngle latitude = new Angle(-37.1);
            IAngle longitude = new Angle(144.9);
            IElevation elevation = new Elevation(96, ElevationMeasurement.Meters);

            IGeographicCoordinate result = new GeographicCoordinate(latitude, longitude, elevation);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(90.0)]
        public void GeographicCoordinate_CanUpdateValidLatitude(double latitude)
        {
            IAngle expected = new Angle(latitude);

            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
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
            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Latitude = new Angle(latitude));
        }

        [Theory]
        [InlineData(-179.9)]
        [InlineData(-42.7)]
        [InlineData(0.0)]
        [InlineData(180.0)]
        public void GeographicCoordinate_CanUpdateValidLongitude(double longitude)
        {
            IAngle expected = new Angle(longitude);

            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
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
            IGeographicCoordinate sut = InstantiateNewGeographicCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Longitude = new Angle(longitude));
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

        private static GeographicCoordinate InstantiateNewGeographicCoordinate()
        {
            return new GeographicCoordinate();
        }
    }
}
