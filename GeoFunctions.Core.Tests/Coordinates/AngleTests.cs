using System;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class AngleTests
    {
        private const double Tolerance = 0.00001;

        [Fact]
        public void Angle_CanInstantiate()
        {
            var result = InstantiateNewCoordinate();
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        [InlineData(100.0)]
        public void Angle_CanInstantiateWithValue(double value)
        {
            var expected = value;

            var sut = new Angle(value);
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1.0E+10 - 0.00001)]
        [InlineData(1.0E+10 + 0.00001)]
        [InlineData(double.MaxValue)]
        public void Angle_CanNotInstantiateWithValue(double value)
        {
            Assert.Throws<ArgumentException>(() => new Angle(value));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        [InlineData(Math.PI)]
        public void Angle_CanInstantiateWithValueAndMeasurement(double value)
        {
            var expectedValue = value;
            const AngleMeasurement angleMeasurement = AngleMeasurement.Radians;

            var sut = new Angle(value, AngleMeasurement.Radians);
            var resultValue = sut.Value;
            var resultMeasurement = sut.AngleMeasurement;

            Assert.Equal(expectedValue, resultValue);
            Assert.Equal(angleMeasurement, resultMeasurement);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        [InlineData(0.0)]
        public void Angle_CanSetValue(double value)
        {
            var expected = value;

            var sut = InstantiateNewCoordinate();
            sut.Value = value;
            var result = sut.Value;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1.0E+10 - 0.00001)]
        [InlineData(1.0E+10 + 0.00001)]
        [InlineData(double.MaxValue)]
        public void Angle_CanNotSetValue(double value)
        {
            var sut = InstantiateNewCoordinate();
            Assert.Throws<ArgumentException>(() => sut.Value = value);
        }

        [Fact]
        public void Angle_CanNotSetNaNValue()
        {
            var sut = InstantiateNewCoordinate();
            Assert.Throws<ArgumentException>(() => sut.Value = double.NaN);
        }

        [Theory]
        [InlineData(-9999999720.0, 0.0)]
        [InlineData(-270.0, 90.0)]
        [InlineData(-45.0, 315.0)]
        [InlineData(0.0, 0.0)]
        [InlineData(360.0, 0.0)]
        [InlineData(765.0, 45.0)]
        [InlineData(9999999720.0, 0.0)]
        public void Angle_CoTerminalValueCorrectlyCalculatesDegrees(double angle, double expectedAngle)
        {
            var expected = expectedAngle;

            var coTerminalAngle = angle >= 0.0 ? angle - 360.0 : angle + 360.0;
            var sut = new Angle(coTerminalAngle);
            var result = sut.CoTerminalValue;

            Assert.True(Math.Abs(expected - result) <= Tolerance);
        }

        [Theory]
        [InlineData(-3183098858.0 * Math.PI, 0.0)]
        [InlineData(-20.0 * Math.PI / 7.0, 8.0 * Math.PI / 7.0)]
        [InlineData(-Math.PI / 2.0, 3.0 * Math.PI / 2.0)]
        [InlineData(0.0, 0.0)]
        [InlineData(2.0*Math.PI/3.0, 2.0 * Math.PI / 3.0)]
        [InlineData(4.0 * Math.PI, 0.0)]
        [InlineData(3183098858.0 * Math.PI, 0.0)]
        public void Angle_CoTerminalValueCorrectlyCalculatesRadians(double angle, double expectedAngle)
        {
            var expected = expectedAngle;

            var coTerminalAngle = angle + 2.0 * Math.PI;
            var sut = new Angle(coTerminalAngle, AngleMeasurement.Radians);
            var result = sut.CoTerminalValue;

            Assert.True(Math.Abs(expected - result) <= Tolerance);
        }


        [Fact]
        public void Angle_AngleMeasurement_DefaultsToDegrees()
        {
            const AngleMeasurement expected = AngleMeasurement.Degrees;

            var result = InstantiateNewCoordinate().AngleMeasurement;

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

            var sut = new Angle(inputValue);
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
            const double valueDegrees = 180.0;
            const double expected = Math.PI;

            var result = Angle.ToRadians(valueDegrees);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        [InlineData(1771882.8891)]
        public void Angle_CorrectlyChecksEqualityOfValue(double value)
        {
            var sut = new Angle(value);
            var testObject = new Angle(value);

            Assert.True(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(1.0E+10)]
        [InlineData(1771882.8891)]
        public void Angle_CorrectlyChecksFindsInequalityOfValue(double value)
        {
            var valueOffset = value * 0.00001;
            var testValue = value - valueOffset;

            var sut = new Angle(value);
            var testObject = new Angle(testValue);

            Assert.False(sut.Equals(testObject));
        }

        [Fact]
        public void Angle_EqualityFailsWhenObjectNotAngle()
        {
            var sut = new Angle();
            var testObject = new object();

            Assert.False(sut.Equals(testObject));
        }

        [Fact]
        public void Angle_CorrectlyChecksEqualityOfMeasurement()
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
