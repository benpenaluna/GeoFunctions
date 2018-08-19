using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Standard.Coordinates;
using Xunit;

namespace GeoFunctions.Tests.Coordinates
{
    public class GeographicCoordinateTests
    {
        [Fact]
        public void GeographicCoordinate_CanInstantiate()
        {
            var result = new GeographicCoordinate();

            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(-45.1)]
        [InlineData(24.0)]
        [InlineData(90.0)]
        public void GeographicCoordinate_CanSetValidLatitude(double latitude)
        {
            var expected = latitude;

            var sut = new GeographicCoordinate {Latitude = latitude};
            var result = sut.Latitude;

            Assert.Equal(expected, result);
        }
    }
}
