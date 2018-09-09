using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Core.Coordinates.Structs;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates.Structs
{
    public class DmsCoordinateTests
    {
        [Theory]
        [InlineData("D MM SS.ss H", "9 02 38.95 S")]
        [InlineData("DD MM SS.ss", "-09 02 38.95")]
        [InlineData("HDMMSS.s", "S90238.9")]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" S")]
        [InlineData("D.ddd", "-9.044")]
        [InlineData("D.ddd H", "9.044 S")]
        [InlineData("D MM.mmm", "-9 02.649")]
        [InlineData("D MM.mmm H", "9 02.649 S")]
        [InlineData("MM SS.ss", "02 38.95")]
        public void DmsCoordinate_CorrectlyParsesFormatString(string format, string expected)
        {
            var sut = new DmsCoordinate
            {
                Degrees = 9.0,
                Minutes = 2.0,
                Seconds = 38.94503637783,
                Hemisphere = Hemisphere.South
            };

            var result = sut.ToString(format);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void DmsCoordinate_CorrectlyParsesDefault()
        {
            const string expected = "09° 02' 39\" S";

            var sut = new DmsCoordinate
            {
                Degrees = 9.0,
                Minutes = 2.0,
                Seconds = 38.94503637783,
                Hemisphere = Hemisphere.South
            };

            var result = sut.ToString();

            Assert.Equal(expected, result);
        }

    }
}
