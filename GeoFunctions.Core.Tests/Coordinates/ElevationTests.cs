using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class ElevationTests
    {
        public enum ArithmeticOperator
        {
            Multiply,
            Divide
        }

        private const double DoubleFloatingPointTolerance = 1.0E-11;

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
        [InlineData("m uu", "1 millimeter", 1.0, DistanceMeasurement.Millimeters)]
        [InlineData("m.m uu", "1.0 millimeter", 1.0, DistanceMeasurement.Millimeters)]
        [InlineData("c u", "42 cm", 42.0, DistanceMeasurement.Centimeters)]
        [InlineData("c uu", "42 centimeters", 42.0, DistanceMeasurement.Centimeters)]
        [InlineData("c uu", "1 centimeter", 1.0, DistanceMeasurement.Centimeters)]
        [InlineData("t u", "42 m", 42.0, DistanceMeasurement.Meters)]
        [InlineData("t uu", "42 meters", 42.0, DistanceMeasurement.Meters)]
        [InlineData("t uu", "1 meter", 1.0, DistanceMeasurement.Meters)]
        [InlineData("c u", "4200 cm", 42.0, DistanceMeasurement.Meters)]
        [InlineData("m u", "42000 mm", 42.0, DistanceMeasurement.Meters)]
        [InlineData("m uu", "42000 millimeters", 42.0, DistanceMeasurement.Meters)]
        [InlineData("k uu", "1 kilometer", 1.0, DistanceMeasurement.Kilometers)]
        [InlineData("k.kkk u", "0.042 km", 42.0, DistanceMeasurement.Meters)]
        [InlineData("k.kkk uu", "0.042 kilometers", 42.0, DistanceMeasurement.Meters)]

        [InlineData("iu", "42\"", 42.0, DistanceMeasurement.Inches)]
        [InlineData("i uu", "1 inch", 1.0, DistanceMeasurement.Inches)]
        [InlineData("i uu", "42 inches", 42.0, DistanceMeasurement.Inches)]
        [InlineData("i.iiiu", "2671992.576\"", 42.1716, DistanceMeasurement.Miles)]
        [InlineData("fu", "42'", 42.0, DistanceMeasurement.Feet)]
        [InlineData("f uu", "1 foot", 1.0, DistanceMeasurement.Feet)]
        [InlineData("f uu", "42 feet", 42.0, DistanceMeasurement.Feet)]
        [InlineData("fu", "126'", 42.0, DistanceMeasurement.Yards)]
        [InlineData("yu", "42yd", 42.0, DistanceMeasurement.Yards)]
        [InlineData("y uu", "1 yard", 1.0, DistanceMeasurement.Yards)]
        [InlineData("y uu", "42 yards", 42.0, DistanceMeasurement.Yards)]
        [InlineData("fu", "5280'", 1.0, DistanceMeasurement.Miles)]
        [InlineData("lu", "10mi", 10.0, DistanceMeasurement.Miles)]
        [InlineData("l uu", "1 mile", 1.0, DistanceMeasurement.Miles)]
        [InlineData("l uu", "10 miles", 10.0, DistanceMeasurement.Miles)]
        [InlineData("l.llllu", "0.0162mi", 1027.0, DistanceMeasurement.Inches)]

        [InlineData("t.ttttu", "0.3048m", 1.0, DistanceMeasurement.Feet)]
        [InlineData("c.ccu", "30.48cm", 1.0, DistanceMeasurement.Feet)]
        [InlineData("c.ccu", "91.44cm", 1.0, DistanceMeasurement.Yards)]
        [InlineData("f.ffffu", "3.2808'", 1.0, DistanceMeasurement.Meters)]
        [InlineData("i.iiiiu", "39.3701\"", 1.0, DistanceMeasurement.Meters)]
        [InlineData("i.iiiiu", "0.0394\"", 1.0, DistanceMeasurement.Kilometers)]

        [InlineData("", "1nm", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("m u", "1852000 mm", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("c u", "185200 cm", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("t u", "1852 m", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("k.kkk u", "1.852 km", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("i.iiiii uu", "72913.38583 inches", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("f.ffffffffff uu", "6076.1154855643 feet", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("y.yyyyyy uu", "2025.371829 yards", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("l.lllllllll uu", "1.150779448 miles", 1.0, DistanceMeasurement.NauticalMiles)]
        [InlineData("n.n u", "1.0 nm", 1852000, DistanceMeasurement.Millimeters)]
        [InlineData("n.n u", "1.0 nm", 185200, DistanceMeasurement.Centimeters)]
        [InlineData("n.n u", "1.0 nm", 1852, DistanceMeasurement.Meters)]
        [InlineData("n.n uu", "1.0 nautical mile", 1852, DistanceMeasurement.Meters)]
        [InlineData("n.n uu", "2.0 nautical miles", 3704, DistanceMeasurement.Meters)]
        [InlineData("n.n u", "1.0 nm", 1.852, DistanceMeasurement.Kilometers)]
        [InlineData("n.n u", "1.0 nm", 72913.38583, DistanceMeasurement.Inches)]
        [InlineData("n.n u", "1.0 nm", 6076.1154855643, DistanceMeasurement.Feet)]
        [InlineData("n.n u", "1.0 nm", 2025.371829, DistanceMeasurement.Yards)]
        [InlineData("n.n u", "1.0 nm", 1.150779448, DistanceMeasurement.Miles)]
        public void Elevation_CorrectlyParsesFormatString(string format, string expected, double value, DistanceMeasurement unitOfMeasurement)
        {
            IElevation sut = new Elevation(value, unitOfMeasurement);
            var result = sut.ToString(format, CultureInfo.InvariantCulture);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 25.4)]
        [InlineData(0.0, DistanceMeasurement.Inches, 25.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 25.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 304.8)]
        [InlineData(0.0, DistanceMeasurement.Feet, 304.8)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 304.8)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 914.4)]
        [InlineData(0.0, DistanceMeasurement.Yards, 914.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 914.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1852000.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1852000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1852000.0)]
        public void Elevation_CorrectlyConvertsToMillimeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToMillimeters();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }


        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 10.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1000000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 25.4)]
        [InlineData(0.0, DistanceMeasurement.Inches, 25.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 25.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 304.8)]
        [InlineData(0.0, DistanceMeasurement.Feet, 304.8)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 304.8)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 914.4)]
        [InlineData(0.0, DistanceMeasurement.Yards, 914.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 914.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1609344.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1852000.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1852000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1852000.0)]
        public void Elevation_CorrectlyConvertsToMillimetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Elevation.ToMillimeters(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 100.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 100.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 100.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 2.54)]
        [InlineData(0.0, DistanceMeasurement.Inches, 2.54)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 2.54)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 30.48)]
        [InlineData(0.0, DistanceMeasurement.Feet, 30.48)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 30.48)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 91.44)]
        [InlineData(0.0, DistanceMeasurement.Yards, 91.44)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 91.44)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(0.0, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 185200.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 185200.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 185200.0)]
        public void Elevation_CorrectlyConvertsToCentimeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToCentimeters();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }


        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.1)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 100.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 100.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 100.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 100000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 2.54)]
        [InlineData(0.0, DistanceMeasurement.Inches, 2.54)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 2.54)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 30.48)]
        [InlineData(0.0, DistanceMeasurement.Feet, 30.48)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 30.48)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 91.44)]
        [InlineData(0.0, DistanceMeasurement.Yards, 91.44)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 91.44)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(0.0, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 160934.4)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 185200.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 185200.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 185200.0)]
        public void Elevation_CorrectlyConvertsToCentimetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Elevation.ToCentimeters(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(0.0, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(0.0, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(0.0, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1852.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1852.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1852.0)]
        public void Elevation_CorrectlyConvertsToMeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToMeters();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.001)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 0.01)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(0.0, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 0.0254)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(0.0, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 0.3048)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(0.0, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 0.9144)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1609.344)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1852.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1852.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1852.0)]
        public void Elevation_CorrectlyConvertsToMetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Elevation.ToMeters(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToKilometers(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? Math.Round(value * factor) : value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToKilometers();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 0.000001, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 100000.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.001, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 0.0000254, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 0.0003048, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 0.0009144, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.609344, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.852, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToKilometersStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? Math.Round(value * factor) : value / factor;

            var result = Elevation.ToKilometers(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToInches(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToInches();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 25.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 2.54, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.0254, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0000254, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 12.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 36.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 63360.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 72913.3858267717, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToInchesStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Elevation.ToInches(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToFeet(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToFeet();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 304.8, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 30.48, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.3048, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0003048, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 12.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 3.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 5280.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 6076.1154855643, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToFeetStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Elevation.ToFeet(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToYards(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToYards();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 914.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 91.44, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 0.9144, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 0.0009144, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 36.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 3.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1760.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 2025.37182852143, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToYardsStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Elevation.ToYards(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToMiles(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToMiles();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1609344.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 160934.4, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1609.344, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.609344, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 63360.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 5280.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(0.0, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 1760.0, ArithmeticOperator.Divide)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.0, ArithmeticOperator.Multiply)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.15077944802354, ArithmeticOperator.Multiply)]
        public void Elevation_CorrectlyConvertsToMilesStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Elevation.ToMiles(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(0.0, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(0.0, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(0.0, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.0)]
        public void Elevation_CorrectlyConvertsToNM(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value / factor;

            IElevation sut = new Elevation(value, measurement);
            var result = sut.ToNauticalMiles();

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        [Theory]
        [InlineData(-1.0E+10, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(0.0, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Millimeters, 1852000.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(0.0, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Centimeters, 185200.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(0.0, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(1.0E+10, DistanceMeasurement.Meters, 1852.0)]
        [InlineData(-1.0E+10, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(0.0, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(1.0E+10, DistanceMeasurement.Kilometers, 1.852)]
        [InlineData(-1.0E+10, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(0.0, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(1.0E+10, DistanceMeasurement.Inches, 72913.3858267717)]
        [InlineData(-1.0E+10, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(0.0, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(1.0E+10, DistanceMeasurement.Feet, 6076.1154855643)]
        [InlineData(-1.0E+10, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(0.0, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(1.0E+10, DistanceMeasurement.Yards, 2025.37182852143)]
        [InlineData(-1.0E+10, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(0.0, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(1.0E+10, DistanceMeasurement.Miles, 1.15077944802354)]
        [InlineData(-1.0E+10, DistanceMeasurement.NauticalMiles, 1.0)]
        [InlineData(0.0, DistanceMeasurement.NauticalMiles, 1.0)]
        [InlineData(1.0E+10, DistanceMeasurement.NauticalMiles, 1.0)]
        public void Elevation_CorrectlyConvertsToNMFromNMStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value / factor;

            var result = Elevation.ToNauticalMiles(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        private static Elevation InstantiateNewElevation()
        {
            return new Elevation();
        }
    }
}
