﻿using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using GeoFunctions.Core.Coordinates.Structs;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class LongitudeTests
    {
        [Fact]
        public void Longitude_CanInstantiate()
        {
            var sut = new Longitude();
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(-179.999999999999)]
        [InlineData(0.0)]
        [InlineData(180.0)]
        public void Longitude_CanInstantiateWithValidDegree(double angle)
        {
            IAngle testAngle = new Angle(angle);
            var expected = testAngle;

            var sut = new Longitude(angle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-180.0)]
        [InlineData(180.000000000001)]
        [InlineData(double.MaxValue)]
        public void Longitude_CannotInstantiateWithInvalidDegree(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Longitude(angle));
        }

        [Theory]
        [InlineData(-Math.PI + 0.0000000000001)]
        [InlineData(0.0)]
        [InlineData(Math.PI / 2.0)]
        public void Longitude_CanInstantiateWithValidRadians(double angle)
        {
            IAngle testAngle = new Angle(angle, AngleMeasurement.Radians);
            var expected = testAngle;

            var sut = new Longitude(angle, AngleMeasurement.Radians);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-Math.PI)]
        [InlineData((Math.PI) + 0.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Longitude_CannotInstantiateWithInvalidRadians(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Longitude(angle, AngleMeasurement.Radians));
        }

        [Theory]
        [InlineData(-179.999999999999)]
        [InlineData(0.0)]
        [InlineData(180.0)]
        public void Longitude_CanInstantiateWithValidAngleDegrees(double angle)
        {
            IAngle testAngle = new Angle(angle);
            var expected = testAngle;

            var sut = new Longitude(testAngle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-180.0)]
        [InlineData(180.000000000001)]
        [InlineData(double.MaxValue)]
        public void Longitude_CannotInstantiateWithInvalidAngleDegree(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Longitude(new Angle(angle)));
        }

        [Theory]
        [InlineData(-Math.PI + 0.0000000000001)]
        [InlineData(0.0)]
        [InlineData(Math.PI)]
        public void Longitude_CanInstantiateWithValidAngleRadians(double angle)
        {
            IAngle testAngle = new Angle(angle, AngleMeasurement.Radians);
            var expected = testAngle;

            var sut = new Longitude(testAngle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-Math.PI)]
        [InlineData(Math.PI + 0.0000000000001)]
        [InlineData(double.MaxValue)]
        public void Longitude_CannotInstantiateWithInvalidAngleRadians(double angle)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Longitude(new Angle(angle, AngleMeasurement.Radians)));
        }

        [Fact]
        public void Longitude_CanGetAngle()
        {
            var sut = new Longitude();
            Assert.NotNull(sut.Angle);
        }

        [Theory]
        [InlineData(144.9)]
        public void Longitude_CanSetAngle(double angle)
        {
            var expected = new Angle(angle);

            var sut = new Longitude(angle);
            var result = sut.Angle;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(144.999637777534, 144, 59, 58.6959991223671)]
        [InlineData(-144.7234706, 144, 43, 24.4941600000470)]
        public void Longitude_CanConvertToDms(double angle, int degrees, int minutes, double seconds)
        {
            var expectedHemisphere = angle >= 0 ? Hemisphere.East : Hemisphere.West;
            var expected = new DmsCoordinate(degrees, minutes, seconds, expectedHemisphere);

            var sut = new Longitude(angle);
            var result = sut.DmsCoordinate;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(144.999637777534)]
        [InlineData(-144.7234706)]
        public void Longitude_ConfirmEquality(double angle)
        {
            var testObject = new Longitude(angle);

            var sut = new Longitude(angle);

            Assert.Equal(testObject, sut);
        }

        [Fact]
        public void Latitude_CorrectlyParsesDefaultFormatString()
        {
            const string expected = "144° 59' 59\"E";

            ISphericalCoordinate sut = new Longitude(144.999637777534);
            var result = sut.ToString();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Latitude_CorrectlyParsesDefaultFormatStringNoFormat()
        {
            const string expected = "144° 59' 59\"E";

            ISphericalCoordinate sut = new Longitude(144.999637777534);
            var result = sut.ToString(null, null);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Latitude_CorrectlyParsesDefaultFormatStringWithFormat()
        {
            const string expected = "144° 59' 59\"E";

            ISphericalCoordinate sut = new Longitude(144.999637777534);
            var result = sut.ToString("DDD° MM' SS\"H", CultureInfo.CurrentCulture);

            Assert.Equal(expected, result);
        }
    }
}
