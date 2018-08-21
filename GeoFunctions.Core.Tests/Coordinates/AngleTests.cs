using System;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class AngleTests
    {
        [Fact]
        public void Angle_CanInstantiate()
        {
            var result = InstantiateNewCoordinate();
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(100.0)]
        public void Angle_CanInstantateWithValue(double value)
        {
            var expected = value;

            var sut = new Angle(value);
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(Math.PI)]
        public void Angle_CanInstantateWithValueAndMeasuement(double value)
        {
            var expectedValue = value;
            const AngleMeasurement expectedMeasuement = AngleMeasurement.Radians;

            var sut = new Angle(value, AngleMeasurement.Radians);
            var resultValue = sut.Value;
            var resultMeasurement = sut.AngleMeasurement;

            Assert.Equal(expectedValue, resultValue);
            Assert.Equal(expectedMeasuement, resultMeasurement);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.0)]
        public void Angle_CanSetValue(double value)
        {
            var expected = value;

            var sut = InstantiateNewCoordinate();
            sut.Value = value;
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_CanSetNaNValue()
        {
            var sut = InstantiateNewCoordinate();
            Assert.Throws<ArgumentException>(() => sut.Value = double.NaN);
        }

        [Fact]
        public void Angle_AngleMeasurement_DefulatsToDegrees()
        {
            var expected = AngleMeasurement.Degrees;

            var result = InstantiateNewCoordinate().AngleMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_CanSetAngleMeasurement()
        {
            const AngleMeasurement expected = AngleMeasurement.Radians;

            var sut = InstantiateNewCoordinate();
            sut.AngleMeasurement = expected;
            var result = sut.AngleMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_ToDegrees_DoesNotConvertDegrees()
        {
            const double expected = 180.0;

            var sut = new Angle(expected);
            var result = sut.ToDegrees();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_ToDegrees_CorrectlyConvertsRadians()
        {
            const double expected = 180.0;
            const double inputValue = Math.PI;
            
            var sut = new Angle(inputValue, AngleMeasurement.Radians);
            var result = sut.ToDegrees();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_ToRadians_DoesNotConvertRadians()
        {
            const double expected = Math.PI;

            var sut = new Angle(expected, AngleMeasurement.Radians);
            var result = sut.ToRadians();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_ToRadians_CorrectlyConvertsDegrees()
        {
            const double expected = Math.PI;
            const double inputValue = 180.0;

            var sut = new Angle(inputValue, AngleMeasurement.Degrees);
            var result = sut.ToRadians();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_CanConvertToDegreesStatically()
        {
            const double valueRadians = Math.PI;
            const double expected = 180.0;

            var result = Angle.ToDegrees(valueRadians);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Angle_CanConvertToRadiansStatically()
        {
            const double valuedegrees = 180.0;
            const double expected = Math.PI;

            var result = Angle.ToRadians(valuedegrees);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(1771882.8891)]
        public void Angle_CorrectlyChecksEqualityOfValue(double value)
        {
            var sut = new Angle(value);
            var testObject = new Angle(value);

            Assert.True(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(1771882.8891)]
        public void Angle_CorrectlyChecksFindsInequalityOfValue(double value)
        {
            var valueOffset = value * 0.00001;
            var testValue = value >= 0 ? value - valueOffset : value + valueOffset;

            var sut = new Angle(value);
            var testObject = new Angle(testValue);

            Assert.False(sut.Equals(testObject));
        }

        [Fact]
        public void Angle_CorrectlyChecksEqualityOfMeasuement()
        {
            var sut = new Angle(Math.PI, AngleMeasurement.Radians);
            var testObject = new Angle(Math.PI, AngleMeasurement.Radians);
            
            Assert.True(sut.Equals(testObject));
        }

        private static Angle InstantiateNewCoordinate()
        {
            return new Angle();
        }
    }
}
