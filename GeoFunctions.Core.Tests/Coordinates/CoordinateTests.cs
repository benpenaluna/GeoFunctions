using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class CoordinateTests
    {
        [Fact]
        public void Coordinate_CanInstantiate()
        {
            var result = InstantiateNewCoordinate();
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(100.0)]
        public void Coordinate_CanInstantateWithValue(double value)
        {
            var expected = value;

            var sut = new Coordinate(value);
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(Math.PI)]
        public void Coordinate_CanInstantateWithValueAndMeasuement(double value)
        {
            var expectedValue = value;
            const AngleMeasurement expectedMeasuement = AngleMeasurement.Radians;

            var sut = new Coordinate(value, AngleMeasurement.Radians);
            var resultValue = sut.Value;
            var resultMeasurement = sut.AngleMeasurement;

            Assert.Equal(expectedValue, resultValue);
            Assert.Equal(expectedMeasuement, resultMeasurement);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.0)]
        public void Coordinate_CanSetValue(double value)
        {
            var expected = value;

            var sut = InstantiateNewCoordinate();
            sut.Value = value;
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_AngleMeasurement_DefulatsToDegrees()
        {
            var expected = AngleMeasurement.Degrees;

            var result = InstantiateNewCoordinate().AngleMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_CanSetAngleMeasurement()
        {
            const AngleMeasurement expected = AngleMeasurement.Radians;

            var sut = InstantiateNewCoordinate();
            sut.AngleMeasurement = expected;
            var result = sut.AngleMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_ToDegrees_DoesNotConvertDegrees()
        {
            const double expected = 180.0;

            var sut = new Coordinate(expected);
            var result = sut.ToDegrees();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_ToDegrees_CorrectlyConvertsRadians()
        {
            const double expected = 180.0;
            const double inputValue = Math.PI;
            
            var sut = new Coordinate(inputValue, AngleMeasurement.Radians);
            var result = sut.ToDegrees();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_ToRadians_DoesNotConvertRadians()
        {
            const double expected = Math.PI;

            var sut = new Coordinate(expected, AngleMeasurement.Radians);
            var result = sut.ToRadians();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_ToRadians_CorrectlyConvertsDegrees()
        {
            const double expected = Math.PI;
            const double inputValue = 180.0;

            var sut = new Coordinate(inputValue, AngleMeasurement.Degrees);
            var result = sut.ToRadians();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_CanConvertToDegreesStatically()
        {
            const double valueRadians = Math.PI;
            const double expected = 180.0;

            var result = Coordinate.ToDegrees(valueRadians);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Coordinate_CanConvertToRadiansStatically()
        {
            const double valuedegrees = 180.0;
            const double expected = Math.PI;

            var result = Coordinate.ToRadians(valuedegrees);

            Assert.Equal(expected, result);
        }


        private static Coordinate InstantiateNewCoordinate()
        {
            return new Coordinate();
        }
    }
}
