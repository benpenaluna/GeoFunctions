using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class ElevationTests
    {
        [Fact]
        public void Elevation_CanInstantiate()
        {
            var sut = InstantiateNewElevation();

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_CanInstantiateWithValue()
        {
            const double value = 999.0;
            var sut = new Elevation(value);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_CanInstantiateWithValueAndMeasurement()
        {
            const double value = 999.0;
            const ElevationMeasurement measurement = ElevationMeasurement.Meters;
            var sut = new Elevation(value, measurement);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_MeasurementDefaultsToFeet()
        {
            var expected = ElevationMeasurement.Feet;

            var sut = InstantiateNewElevation();
            var result = sut.ElevationMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Elevation_MeasurementDefaultsToFeetValueSpecified()
        {
            const ElevationMeasurement expected = ElevationMeasurement.Feet;

            const double value = 99.09;
            var sut = new Elevation(value);
            var result = sut.ElevationMeasurement;

            Assert.Equal(expected, result);
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

        [Theory]
        [InlineData(ElevationMeasurement.Feet)]
        [InlineData(ElevationMeasurement.Meters)]
        public void Elevation_CanGetMeasurement(ElevationMeasurement measurement)
        {
            var expected = measurement;

            var sut = new Elevation(0.0, measurement);
            var result = sut.ElevationMeasurement;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyChecksEqualityOfValue(double value)
        {
            var sut = new Elevation(value);
            var testObject = new Elevation(value);

            Assert.True(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyFindsInequalityOfValue(double value)
        {
            var valueOffset = value > 0 ? -1.0 : 1.0;

            var sut = new Elevation(value);
            var testObject = new Elevation(value + valueOffset);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyFindsInequalityOfMeasurement(double value)
        {
            var sut = new Elevation(value);
            var testObject = new Elevation(value, ElevationMeasurement.Meters);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToFeetFromFeet(double value)
        {
            var expected = value;

            var sut = new Elevation(value);
            var result = sut.ToFeet();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToFeetFromMeters(double value)
        {
            var expected = value / 0.3048;

            var sut = new Elevation(value, ElevationMeasurement.Meters);
            var result = sut.ToFeet();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToFeetStatically(double value)
        {
            var expected = value / 0.3048;

            var result = Elevation.ToFeet(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToMetersFromMeters(double value)
        {
            var expected = value;

            var sut = new Elevation(value, ElevationMeasurement.Meters);
            var result = sut.ToMeters();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToMetersFromFeet(double value)
        {
            var expected = value * 0.3048;

            var sut = new Elevation(value);
            var result = sut.ToMeters();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToMetersStatically(double value)
        {
            var expected = value * 0.3048;

            var result = Elevation.ToMeters(value);

            Assert.Equal(expected, result);
        }

        private static Elevation InstantiateNewElevation()
        {
            return new Elevation();
        }
    }
}
