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
        [InlineData("D MM SS.ss H", "9 02 38.95 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM SS.ss H", "45 02 38.95 N", 45.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM SS.ss H", "145 02 38.95 E", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM SS.ss H", "145 02 38.95 W", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("DD MM SS.ss", "-09 02 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("DD MM SS.ss", "09 02 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("DD MM SS.ss", "-145 02 38.95", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("DD MM SS.ss", "145 02 38.95", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("HDMMSS.s", "S90238.9", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("HDMMSS.s", "N90238.9", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("HDMMSS.s", "W990238.9", 99.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("HDMMSS.s", "E990238.9", 99.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("HDDMMSS.s", "S090238.9", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("HDDMMSS.s", "N090238.9", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("HDDMMSS.s", "W1450238.9", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("HDDMMSS.s", "E1450238.9", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" N", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("DD° MM' SS.s\" H", "144° 02' 38.9\" W", 144.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("DD° MM' SS.s\" H", "144° 02' 38.9\" E", 144.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D.ddd", "-9.044", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D.ddd", "9.044", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("D.ddd", "-9.044", 9.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("D.ddd", "9.044", 9.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D.ddd H", "9.044 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D.ddd H", "9.044 N", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("D.ddd H", "9.044 W", 9.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("D.ddd H", "9.044 E", 9.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mmm", "-9 02.649", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm", "9 02.649", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM.mmm", "-145 02.649", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("D MM.mmm", "145 02.649", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mmm H", "9 02.649 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm H", "9 02.649 N", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM.mmm H", "145 02.649 W", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("D MM.mmm H", "145 02.649 E", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("MM SS.ss", "02 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("MM SS.ss", "02 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("MM SS.ss", "02 38.95", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("MM SS.ss", "02 38.95", 145.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("DD SS.ss", "-09 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("DD SS.ss", "09 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.North)]
        [InlineData("DD SS.ss", "-09 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("DD SS.ss", "09 38.95", 9.0, 2.0, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mm SS.ss H", "9 02.00 38.95 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("DDD D.ddd M.mm SS.ss H", "009 9.000 2.00 38.95 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm M.mm SS.ss H", "9 02.000 2.00 38.95 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm S.ss SS.ssss H", "9 02.000 38.95 38.9450 S", 9.0, 2.0, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.sss", "-145 02.039", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        [InlineData("DD.MMSSH", "145.0239W", 145.0, 2.0, 38.94503637783, Hemisphere.West)]
        public void DmsCoordinate_CorrectlyParsesFormatString(string format, string expected, double degrees, double minutes, double seconds, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate
            {
                Degrees = degrees,
                Minutes = minutes,
                Seconds = seconds,
                Hemisphere = hemisphere
            };

            var result = sut.ToString(format);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void DmsCoordinate_CorrectlyParsesDefaultFormatString()
        {
            const string expected = "09° 02' 39\"S";

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
