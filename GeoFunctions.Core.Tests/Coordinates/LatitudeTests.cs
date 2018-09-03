using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using GeoFunctions.Core.Coordinates.Structs;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class LatitudeTests
    {
        [Fact]
        public void Latitude_CanInstantiate()
        {
            var sut = new Latitude();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(0.0)]
        [InlineData(90.0)]
        public void Latitude_CanInstantiateWithValidDegree(double angle)
        {
            IAngle testAngle = new Angle(angle);
            var expected = testAngle;

            var sut = new Latitude(angle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-90.0000000000001)]
        [InlineData(90.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Latitude_CannotInstantiateWithInvalidDegree(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Latitude(angle));
        }

        [Theory]
        [InlineData(-Math.PI / 2.0)]
        [InlineData(0.0)]
        [InlineData(Math.PI / 2.0)]
        public void Latitude_CanInstantiateWithValidRadians(double angle)
        {
            IAngle testAngle = new Angle(angle, AngleMeasurement.Radians);
            var expected = testAngle;

            var sut = new Latitude(angle, AngleMeasurement.Radians);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-(Math.PI / 2.0) - 0.0000000000001)]
        [InlineData((Math.PI / 2.0) + 0.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Latitude_CannotInstantiateWithInvalidRadians(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Latitude(angle, AngleMeasurement.Radians));
        }

        [Theory]
        [InlineData(-90.0)]
        [InlineData(0.0)]
        [InlineData(90.0)]
        public void Latitude_CanInstantiateWithValidAngleDegrees(double angle)
        {
            IAngle testAngle = new Angle(angle);
            var expected = testAngle;

            var sut = new Latitude(testAngle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-90.0000000000001)]
        [InlineData(90.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Latitude_CannotInstantiateWithInvalidAngleDegree(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Latitude(new Angle(angle)));
        }

        [Theory]
        [InlineData(-Math.PI / 2.0)]
        [InlineData(0.0)]
        [InlineData(Math.PI / 2.0)]
        public void Latitude_CanInstantiateWithValidAngleRadians(double angle)
        {
            IAngle testAngle = new Angle(angle, AngleMeasurement.Radians);
            var expected = testAngle;

            var sut = new Latitude(testAngle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-(Math.PI / 2.0) - 0.0000000000001)]
        [InlineData((Math.PI / 2.0) + 0.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Latitude_CannotInstantiateWithInvalidAngleRadians(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Latitude(new Angle(angle, AngleMeasurement.Radians)));
        }

        [Fact]
        public void Latitude_CanGetAngle()
        {
            var sut = new Latitude();
            Assert.NotNull(sut.Angle);
        }

        [Theory]
        [InlineData(-37.5)]
        public void Latitude_CanSetAngle(double angle)
        {
            var expected = new Angle(angle);

            var sut = new Latitude(angle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.6885966980243, 37.0, 41.0, 18.94811289)]
        [InlineData(37.8059488030474, 37.0, 48.0, 21.4156909706401)]
        public void Latitude_CanConvertToDms(double angle, double degrees, double minutes, double seconds)
        {
            var expectedHemisphere = angle >= 0 ? Hemisphere.North : Hemisphere.South;
            var expected = new DmsCoordinate()
            {
                Degrees = degrees,
                Minutes = minutes,
                Seconds = seconds,
                Hemisphere = expectedHemisphere
            };

            var sut = new Latitude(angle);
            var result = sut.DmsCoordinate;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.6885966980243)]
        [InlineData(37.8059488030474)]
        public void Latitude_ConfirmEquality(double angle)
        {
            var testObject = new Latitude(angle);
            
            var sut = new Latitude(angle);

            Assert.Equal(testObject, sut);
        }
    }
}
