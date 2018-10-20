using System;
using System.Globalization;
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
            IElevation sut = InstantiateNewElevation();

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_CanInstantiateWithValue()
        {
            const double value = 999.0;
            IElevation sut = new Elevation(value);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_CanInstantiateWithValueAndMeasurement()
        {
            const double value = 999.0;
            const DistanceMeasurement measurement = DistanceMeasurement.Meters;
            IElevation sut = new Elevation(value, measurement);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Elevation_MeasurementDefaultsToFeet()
        {
            const DistanceMeasurement expected = DistanceMeasurement.Feet;

            IElevation sut = InstantiateNewElevation();
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Elevation_MeasurementDefaultsToFeetValueSpecified()
        {
            const DistanceMeasurement expected = DistanceMeasurement.Feet;

            const double value = 99.09;
            IElevation sut = new Elevation(value);
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CanSetValidValue(double elevation)
        {
            var expected = elevation;

            IElevation sut = InstantiateNewElevation();
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
            IElevation sut = InstantiateNewElevation();
            Assert.Throws<ArgumentException>(() => sut.Value = elevation);

        }

        [Theory]
        [InlineData(DistanceMeasurement.Feet)]
        [InlineData(DistanceMeasurement.Meters)]
        public void Elevation_CanGetMeasurement(DistanceMeasurement measurement)
        {
            var expected = measurement;

            IElevation sut = new Elevation(0.0, measurement);
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyChecksEqualityOfValue(double value)
        {
            IElevation sut = new Elevation(value);
            IElevation testObject = new Elevation(value);

            Assert.True(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyFindsInequalityOfValue(double value)
        {
            var valueOffset = value > 0 ? -1.0 : 1.0;

            IElevation sut = new Elevation(value);
            IElevation testObject = new Elevation(value + valueOffset);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyFindsInequalityOfMeasurement(double value)
        {
            IElevation sut = new Elevation(value);
            IElevation testObject = new Elevation(value, DistanceMeasurement.Meters);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(42.0, DistanceMeasurement.Feet, "42'")]
        [InlineData(42.0, DistanceMeasurement.Meters, "42 m")]
        public void Elevation_CorrectlyReturnToString(double value, DistanceMeasurement distanceMeasurement, string expected)
        {
            IElevation sut = new Elevation(value, distanceMeasurement);
            var result = sut.ToString();

            Assert.Equal(expected, result);
        }

        [Theory]
        //[InlineData("m uu", "1 millimeter", 1.0, DistanceMeasurement.Centimeters)]
        [InlineData("c u", "42 cm", 42.0, DistanceMeasurement.Centimeters)]
        [InlineData("c uu", "42 centimeters", 42.0, DistanceMeasurement.Centimeters)]
        //[InlineData("c uu", "1 centimeter", 1.0, DistanceMeasurement.Centimeters)]
        [InlineData("t u", "42 m", 42.0, DistanceMeasurement.Meters)]
        [InlineData("t uu", "42 meters", 42.0, DistanceMeasurement.Meters)]
        //[InlineData("t uu", "1 meter", 1.0, DistanceMeasurement.Meters)]
        [InlineData("c u", "4200 cm", 42.0, DistanceMeasurement.Meters)]
        [InlineData("m u", "42000 mm", 42.0, DistanceMeasurement.Meters)]
        [InlineData("m uu", "42000 millimeters", 42.0, DistanceMeasurement.Meters)]
        //[InlineData("k uu", "1 kilometer", 1.0, DistanceMeasurement.Meters)]
        [InlineData("k.kkk u", "0.042 km", 42.0, DistanceMeasurement.Meters)]
        [InlineData("k.kkk uu", "0.042 kilometers", 42.0, DistanceMeasurement.Meters)]

        [InlineData("iu", "42\"", 42.0, DistanceMeasurement.Inches)]
        [InlineData("i uu", "42 inches", 42.0, DistanceMeasurement.Inches)]
        [InlineData("i.iiiu", "2671992.576\"", 42.1716, DistanceMeasurement.Miles)]
        [InlineData("fu", "42'", 42.0, DistanceMeasurement.Feet)]
        [InlineData("f uu", "42 feet", 42.0, DistanceMeasurement.Feet)]
        [InlineData("fu", "126'", 42.0, DistanceMeasurement.Yards)]
        [InlineData("yu", "42yd", 42.0, DistanceMeasurement.Yards)]
        [InlineData("y uu", "42 yards", 42.0, DistanceMeasurement.Yards)]
        [InlineData("fu", "5280'", 1.0, DistanceMeasurement.Miles)]
        [InlineData("lu", "10mi", 10.0, DistanceMeasurement.Miles)]
        [InlineData("l uu", "10 miles", 10.0, DistanceMeasurement.Miles)]
        [InlineData("l.llllu", "0.0162mi", 1027.0, DistanceMeasurement.Inches)]

        [InlineData("t.ttttu", "0.3048m", 1.0, DistanceMeasurement.Feet)]
        [InlineData("c.ccu", "30.48cm", 1.0, DistanceMeasurement.Feet)]
        [InlineData("c.ccu", "91.44cm", 1.0, DistanceMeasurement.Yards)]
        [InlineData("f.ffffu", "3.2808'", 1.0, DistanceMeasurement.Meters)]
        [InlineData("i.iiiiu", "39.3701\"", 1.0, DistanceMeasurement.Meters)]
        [InlineData("i.iiiiu", "0.0394\"", 1.0, DistanceMeasurement.Kilometers)]
        public void Elevation_CorrectlyParsesFormatString(string format, string expected, double value, DistanceMeasurement unitOfMeasurement)
        {
            IElevation sut = new Elevation(value, unitOfMeasurement);
            var result = sut.ToString(format, CultureInfo.InvariantCulture);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Elevation_CorrectlyConvertsToFeetFromFeet(double value)
        {
            var expected = value;

            IElevation sut = new Elevation(value);
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

            IElevation sut = new Elevation(value, DistanceMeasurement.Meters);
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

            IElevation sut = new Elevation(value, DistanceMeasurement.Meters);
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

            IElevation sut = new Elevation(value);
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
