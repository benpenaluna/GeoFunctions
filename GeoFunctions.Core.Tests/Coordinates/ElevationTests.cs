using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Core.Coordinates;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class ElevationTests
    {
        [Fact]
        public void Elevation_CanInstantiate()
        {
            Elevation sut = InstantiateNewElevation();

            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CanSetValidValue(double elevation)
        {
            var expected = elevation;

            var sut = InstantiateNewElevation();
            sut.Value = elevation;
            var result = sut.Value;

            Assert.Equal(expected, result);

        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1.0E+10 - 0.00001)]
        [InlineData(1.0E+10 + 0.00001)]
        [InlineData(double.MaxValue)]
        public void Elevation_CanNotSetInvalidValue(double elevation)
        {
            var sut = InstantiateNewElevation();
            Assert.Throws<ArgumentException>(() => sut.Value = elevation);

        }

        private static Elevation InstantiateNewElevation()
        {
            return new Elevation();
        }
    }
}
