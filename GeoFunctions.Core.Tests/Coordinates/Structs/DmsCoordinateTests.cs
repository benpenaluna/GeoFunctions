using System;
using GeoFunctions.Core.Coordinates.Structs;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates.Structs
{
    public class DmsCoordinateTests
    {
        [Theory]
        [InlineData(91)]
        [InlineData(-1)]
        public void DmsCoordinate_CannotSetDegreesOutOfRangeWithoutHemisphere(int degrees)
        {
            var sut = new DmsCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Degrees = degrees);
        }

        [Theory]
        [InlineData(90, Hemisphere.North)]
        [InlineData(45, Hemisphere.North)]
        [InlineData(0, Hemisphere.North)]
        [InlineData(90, Hemisphere.South)]
        [InlineData(45, Hemisphere.South)]
        [InlineData(0, Hemisphere.South)]
        [InlineData(180, Hemisphere.East)]
        [InlineData(90, Hemisphere.East)]
        [InlineData(0, Hemisphere.East)]
        [InlineData(179, Hemisphere.West)]
        [InlineData(90, Hemisphere.West)]
        [InlineData(0, Hemisphere.West)]
        public void DmsCoordinate_CorrectlyInstantatesWithDH(int degrees, Hemisphere hemisphere)
        {
            var expected = degrees;

            var sut = new DmsCoordinate(degrees, hemisphere);

            Assert.Equal(expected, sut.Degrees);
        }

        [Theory]
        [InlineData(89, 45, Hemisphere.North)]
        [InlineData(45, 45, Hemisphere.North)]
        [InlineData(0, 45, Hemisphere.North)]
        [InlineData(89, 45, Hemisphere.South)]
        [InlineData(45, 45, Hemisphere.South)]
        [InlineData(0, 45, Hemisphere.South)]
        [InlineData(179, 45, Hemisphere.East)]
        [InlineData(89, 45, Hemisphere.East)]
        [InlineData(0, 45, Hemisphere.East)]
        [InlineData(179, 45, Hemisphere.West)]
        [InlineData(89, 45, Hemisphere.West)]
        [InlineData(0, 45, Hemisphere.West)]
        public void DmsCoordinate_CorrectlyInstantatesWithDMH(int degrees, int minutes, Hemisphere hemisphere)
        {
            var expected = degrees;

            var sut = new DmsCoordinate(degrees, minutes, hemisphere);

            Assert.Equal(expected, sut.Degrees);
        }

        [Theory]
        [InlineData(89, 45, 45.0, Hemisphere.North)]
        [InlineData(45, 45, 45.0, Hemisphere.North)]
        [InlineData(0, 45, 45.0, Hemisphere.North)]
        [InlineData(89, 45, 45.0, Hemisphere.South)]
        [InlineData(45, 45, 45.0, Hemisphere.South)]
        [InlineData(0, 45, 45.0, Hemisphere.South)]
        [InlineData(179, 45, 45.0, Hemisphere.East)]
        [InlineData(89, 45, 45.0, Hemisphere.East)]
        [InlineData(0, 45, 45.0, Hemisphere.East)]
        [InlineData(179, 45, 45.0, Hemisphere.West)]
        [InlineData(89, 45, 45.0, Hemisphere.West)]
        [InlineData(0, 45, 45.0, Hemisphere.West)]
        public void DmsCoordinate_CorrectlyInstantatesWithDMSH(int degrees, int minutes, double seconds, Hemisphere hemisphere)
        {
            var expected = degrees;

            var sut = new DmsCoordinate(degrees, minutes, seconds, hemisphere);

            Assert.Equal(expected, sut.Degrees);
        }

        [Theory]
        [InlineData(90, 1, Hemisphere.North)]
        [InlineData(90, 1, Hemisphere.South)]
        [InlineData(180, 1, Hemisphere.East)]
        public void DmsCoordinate_CannotInstantateWithDMHValueOutofRange(int degrees, int minutes, Hemisphere hemisphere)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DmsCoordinate(degrees, minutes, hemisphere));
        }

        [Theory]
        [InlineData(90, 1, 0.1, Hemisphere.North)]
        [InlineData(90, 1, 0.1, Hemisphere.South)]
        [InlineData(180, 1, 0.1, Hemisphere.East)]
        public void DmsCoordinate_CannotInstantateWithDMSHValueOutofRange(int degrees, int minutes, double seconds, Hemisphere hemisphere)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DmsCoordinate(degrees, minutes, seconds, hemisphere));
        }

        [Theory]
        [InlineData(91, Hemisphere.North)]
        [InlineData(-1, Hemisphere.North)]
        [InlineData(91, Hemisphere.South)]
        [InlineData(-1, Hemisphere.South)]
        [InlineData(181, Hemisphere.East)]
        [InlineData(-1, Hemisphere.East)]
        [InlineData(180, Hemisphere.West)]
        [InlineData(-1, Hemisphere.West)]
        public void DmsCoordinate_CannotSetDegreesOutOfRange(int degrees, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate
            {
                Hemisphere = hemisphere
            };
            
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Degrees = degrees);
        }

        [Theory]
        [InlineData(90, Hemisphere.North)]
        [InlineData(90, Hemisphere.South)]
        [InlineData(180, Hemisphere.East)]
        public void DmsCoordinate_CannotSetDegreesWhenDMSSumOutOfRange(int degrees, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate
            {
                Hemisphere = hemisphere,
                Minutes = 1,
                Seconds = 0.1
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Degrees = degrees);
        }

        [Theory]
        [InlineData(61)]
        [InlineData(-1)]
        public void DmsCoordinate_CannotSetMinutesOutOfRange(int minutes)
        {
            var sut = new DmsCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Minutes = minutes);
        }

        [Theory]
        [InlineData(90, 1, Hemisphere.North)]
        [InlineData(90, 1, Hemisphere.South)]
        [InlineData(180, 1, Hemisphere.East)]
        public void DmsCoordinate_CannotSetMinutesWhenDMSSumOutOfRange(int degrees, int minutes, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate()
            {
                Hemisphere = hemisphere,
                Degrees = degrees
            };
            
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Minutes = minutes);
        }

        [Theory]
        [InlineData(60.000000001)]
        [InlineData(-0.000000001)]
        public void DmsCoordinate_CannotSetSecondsOutOfRange(double seconds)
        {
            var sut = new DmsCoordinate();
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Seconds = seconds);
        }

        [Theory]
        [InlineData(90, 0.1, Hemisphere.North)]
        [InlineData(90, 0.1, Hemisphere.South)]
        [InlineData(180, 0.1, Hemisphere.East)]
        public void DmsCoordinate_CannotSetSecondsWhenDMSSumOutOfRange(int degrees, double seconds, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate
            {
                Hemisphere = hemisphere,
                Degrees = degrees
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Seconds = seconds);
        }

        [Theory]
        [InlineData(89, 1, 59.999999999999, Hemisphere.North, Hemisphere.South)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.North, Hemisphere.East)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.North, Hemisphere.West)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.South, Hemisphere.North)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.South, Hemisphere.East)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.South, Hemisphere.West)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.East, Hemisphere.North)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.East, Hemisphere.South)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.East, Hemisphere.West)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.West, Hemisphere.North)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.West, Hemisphere.South)]
        [InlineData(89, 1, 59.999999999999, Hemisphere.West, Hemisphere.East)]
        public void DmsCoordinate_CorrectlyChangeHemisphere(int degrees, int minutes, double seconds, Hemisphere originalHemisphere, Hemisphere newHemisphere)
        {
            var expected = newHemisphere;
            
            var sut = new DmsCoordinate(degrees, minutes, seconds, originalHemisphere);

            sut.Hemisphere = newHemisphere;

            Assert.Equal(expected, sut.Hemisphere);
        }

        [Theory]
        [InlineData(179, 1, 59.999999999999, Hemisphere.East, Hemisphere.North)]
        [InlineData(179, 1, 59.999999999999, Hemisphere.East, Hemisphere.South)]
        [InlineData(179, 1, 59.999999999999, Hemisphere.West, Hemisphere.North)]
        [InlineData(179, 1, 59.999999999999, Hemisphere.West, Hemisphere.South)]
        public void DmsCoordinate_HemisphereChangeThrowsException(int degrees, int minutes, double seconds, Hemisphere originalHemisphere, Hemisphere newHemisphere)
        {
            var sut = new DmsCoordinate(degrees, minutes, seconds, originalHemisphere);

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Hemisphere = newHemisphere);
        }

        [Theory]
        [InlineData("D MM SS.ss H", "9 02 38.95 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM SS.ss H", "45 02 38.95 N", 45, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM SS.ss H", "145 02 38.95 E", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM SS.ss H", "145 02 38.95 W", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("DD MM SS.ss", "-09 02 38.95", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("DD MM SS.ss", "09 02 38.95", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("DD MM SS.ss", "-145 02 38.95", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("DD MM SS.ss", "145 02 38.95", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("HDMMSS.s", "S90238.9", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("HDMMSS.s", "N90238.9", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("HDMMSS.s", "W990238.9", 99, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("HDMMSS.s", "E990238.9", 99, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("HDDMMSS.s", "S090238.9", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("HDDMMSS.s", "N090238.9", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("HDDMMSS.s", "W1450238.9", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("HDDMMSS.s", "E1450238.9", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" N", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("DD° MM' SS.s\" H", "144° 02' 38.9\" W", 144, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("DD° MM' SS.s\" H", "144° 02' 38.9\" E", 144, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("DD° MM' SS\" H", "180° 00' 00\" W", 179, 59, 59.99999999999, Hemisphere.West)]
        [InlineData("D.ddd", "-9.044", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D.ddd", "9.044", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("D.ddd", "-9.044", 9, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("D.ddd", "9.044", 9, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("D.ddd H", "9.044 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D.ddd H", "9.044 N", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("D.ddd H", "9.044 W", 9, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("D.ddd H", "9.044 E", 9, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mmm", "-9 02.649", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm", "9 02.649", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM.mmm", "-145 02.649", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("D MM.mmm", "145 02.649", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mmm H", "9 02.649 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm H", "9 02.649 N", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("D MM.mmm H", "145 02.649 W", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("D MM.mmm H", "145 02.649 E", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("MM SS.ss", "02 38.95", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("MM SS.ss", "02 38.95", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("MM SS.ss", "02 38.95", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("MM SS.ss", "02 38.95", 145, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("DD SS.ss", "-09 38.95", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("DD SS.ss", "09 38.95", 9, 2, 38.94503637783, Hemisphere.North)]
        [InlineData("DD SS.ss", "-09 38.95", 9, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("DD SS.ss", "09 38.95", 9, 2, 38.94503637783, Hemisphere.East)]
        [InlineData("D MM.mm SS.ss H", "9 02.00 38.95 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("DDD D.ddd M.mm SS.ss H", "009 9.000 2.00 38.95 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm M.mm SS.ss H", "9 02.000 2.00 38.95 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.mmm S.ss SS.ssss H", "9 02.000 38.95 38.9450 S", 9, 2, 38.94503637783, Hemisphere.South)]
        [InlineData("D MM.sss", "-145 02.039", 145, 2, 38.94503637783, Hemisphere.West)]
        [InlineData("DD.MMSSH", "145.0239W", 145, 2, 38.94503637783, Hemisphere.West)]
        public void DmsCoordinate_CorrectlyParsesFormatString(string format, string expected, int degrees, int minutes, double seconds, Hemisphere hemisphere)
        {
            var sut = new DmsCoordinate(degrees, minutes, seconds, hemisphere);

            var result = sut.ToString(format);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void DmsCoordinate_CorrectlyParsesDefaultFormatString()
        {
            const string expected = "09° 02' 39\"S";

            var sut = new DmsCoordinate
            {
                Degrees = 9,
                Minutes = 2,
                Seconds = 38.94503637783,
                Hemisphere = Hemisphere.South
            };

            var result = sut.ToString();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(45, 45, 0.0,Hemisphere.North, 45.75, Hemisphere.North)]
        [InlineData(37, 41, 18.948112887474053, Hemisphere.North, 37.6885966980243, Hemisphere.North)]
        [InlineData(89, 59, 59.999999999641886, Hemisphere.North, 89.9999999999999, Hemisphere.North)]
        [InlineData(89, 59, 59.999999999948841, Hemisphere.North, 89.99999999999999, Hemisphere.North)]
        [InlineData(90, 0, 0.0, Hemisphere.North, 89.999999999999999, Hemisphere.North)]
        [InlineData(0, 0, 0.000000000360000, Hemisphere.North, 0.0000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.000000000036000, Hemisphere.North, 0.00000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.000000000003600, Hemisphere.North, 0.000000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.000000000000360, Hemisphere.North, 0.0000000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.000000000000036, Hemisphere.North, 0.00000000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.0000000000000036, Hemisphere.North, 0.000000000000000001, Hemisphere.North)]
        [InlineData(0, 0, 0.00000000000000036, Hemisphere.North, 0.0000000000000000001, Hemisphere.North)]
        [InlineData(45, 45, 0.0, Hemisphere.South, 45.75, Hemisphere.South)]
        [InlineData(37, 41, 18.948112887474053, Hemisphere.South, 37.6885966980243, Hemisphere.South)]
        [InlineData(89, 59, 59.999999999641886, Hemisphere.South, 89.9999999999999, Hemisphere.South)]
        [InlineData(89, 59, 59.999999999948841, Hemisphere.South, 89.99999999999999, Hemisphere.South)]
        [InlineData(90, 0, 0.0, Hemisphere.South, 89.999999999999999, Hemisphere.South)]
        [InlineData(0, 0, 0.00000000000000036, Hemisphere.South, 0.0000000000000000001, Hemisphere.South)]
        [InlineData(90, 0, 0.00000000000000036, Hemisphere.East, 90.0000000000000000001, Hemisphere.East)]
        [InlineData(37, 41, 18.948112887474053, Hemisphere.East, 37.6885966980243, Hemisphere.East)]
        [InlineData(179, 59, 59.999999999590727, Hemisphere.East, 179.9999999999999, Hemisphere.East)]
        [InlineData(180, 0, 0.0, Hemisphere.East, 179.99999999999999, Hemisphere.East)]
        [InlineData(0, 0, 0.00000000000000036, Hemisphere.East, 0.0000000000000000001, Hemisphere.East)]
        [InlineData(90, 0, 0.00000000000000036, Hemisphere.West, 90.0000000000000000001, Hemisphere.West)]
        [InlineData(37, 41, 18.948112887474053, Hemisphere.West, 37.6885966980243, Hemisphere.West)]
        [InlineData(179, 59, 59.999999999590727, Hemisphere.West, 179.9999999999999, Hemisphere.West)]
        [InlineData(0, 0, 0.00000000000000036, Hemisphere.West, 0.0000000000000000001, Hemisphere.West)]
        public void DmsCoordinate_AngleCorrectlyConvertsToDMS(int expectedDegrees, int expectedMinutes, double expectedSeconds, Hemisphere expectedHemisphere, 
                                                              double angle, Hemisphere hemisphere)
        {
            var expected = new DmsCoordinate(expectedDegrees, expectedMinutes, expectedSeconds, expectedHemisphere);
            
            var sut = DmsCoordinate.ConvertAngleToDms(angle, hemisphere);

            Assert.Equal(expected, sut);
        }
    }
}
