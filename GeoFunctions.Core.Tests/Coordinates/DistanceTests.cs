using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using System;
using System.Globalization;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates
{
    public class DistanceTests
    {
        public enum ArithmeticOperator
        {
            Multiply,
            Divide
        }

        private const double DoubleFloatingPointTolerance = 1.0E-11;

        [Fact]
        public void Distance_CanInstantiate()
        {
            IDistance sut = InstantiateNewDistance();

            Assert.NotNull(sut);
        }

        [Fact]
        public void Distance_CanInstantiateWithValue()
        {
            const double value = 999.0;
            IDistance sut = new Distance(value);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Distance_CanInstantiateWithValueAndMeasurement()
        {
            const double value = 999.0;
            const DistanceMeasurement measurement = DistanceMeasurement.Meters;
            IDistance sut = new Distance(value, measurement);

            Assert.NotNull(sut);
        }

        [Fact]
        public void Distance_MeasurementDefaultsToFeet()
        {
            const DistanceMeasurement expected = DistanceMeasurement.Feet;

            IDistance sut = InstantiateNewDistance();
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Distance_MeasurementDefaultsToFeetValueSpecified()
        {
            const DistanceMeasurement expected = DistanceMeasurement.Feet;

            const double value = 99.09;
            IDistance sut = new Distance(value);
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Distance_CanSetValidValue(double distance)
        {
            var expected = distance;

            IDistance sut = InstantiateNewDistance();
            sut.Value = distance;
            var result = sut.Value;

            Assert.Equal(expected, result);

        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1.0E+10 - 0.00001)]
        [InlineData(1.0E+10 + 0.00001)]
        [InlineData(double.MaxValue)]
        public void Distance_CanNotSetInvalidValue(double distance)
        {
            IDistance sut = InstantiateNewDistance();
            Assert.Throws<ArgumentException>(() => sut.Value = distance);

        }

        [Theory]
        [InlineData(DistanceMeasurement.Feet)]
        [InlineData(DistanceMeasurement.Meters)]
        public void Distance_CanGetMeasurement(DistanceMeasurement measurement)
        {
            var expected = measurement;

            IDistance sut = new Distance(0.0, measurement);
            var result = sut.DistanceMeasurement;

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Distance_CorrectlyChecksEqualityOfValue(double value)
        {
            IDistance sut = new Distance(value);
            IDistance testObject = new Distance(value);

            Assert.True(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Distance_CorrectlyFindsInequalityOfValue(double value)
        {
            var valueOffset = value > 0 ? -1.0 : 1.0;

            IDistance sut = new Distance(value);
            IDistance testObject = new Distance(value + valueOffset);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(-1.0E+10)]
        [InlineData(0.0)]
        [InlineData(1.0E+10)]
        public void Distance_CorrectlyFindsInequalityOfMeasurement(double value)
        {
            IDistance sut = new Distance(value);
            IDistance testObject = new Distance(value, DistanceMeasurement.Meters);

            Assert.False(sut.Equals(testObject));
        }

        [Theory]
        [InlineData(42.0, DistanceMeasurement.Feet, "42'")]
        [InlineData(42.0, DistanceMeasurement.Meters, "42 m")]
        public void Distance_CorrectlyReturnToString(double value, DistanceMeasurement distanceMeasurement, string expected)
        {
            IDistance sut = new Distance(value, distanceMeasurement);
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
        public void Distance_CorrectlyParsesFormatString(string format, string expected, double value, DistanceMeasurement unitOfMeasurement)
        {
            IDistance sut = new Distance(value, unitOfMeasurement);
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
        public void Distance_CorrectlyConvertsToMillimeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToMillimetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Distance.ToMillimeters(value, measurement);

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
        public void Distance_CorrectlyConvertsToCentimeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToCentimetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Distance.ToCentimeters(value, measurement);

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
        public void Distance_CorrectlyConvertsToMeters(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToMetersStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value * factor;

            var result = Distance.ToMeters(value, measurement);

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
        public void Distance_CorrectlyConvertsToKilometers(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? Math.Round(value * factor) : value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToKilometersStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? Math.Round(value * factor) : value / factor;

            var result = Distance.ToKilometers(value, measurement);

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
        public void Distance_CorrectlyConvertsToInches(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToInchesStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Distance.ToInches(value, measurement);

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
        public void Distance_CorrectlyConvertsToFeet(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToFeetStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Distance.ToFeet(value, measurement);

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
        public void Distance_CorrectlyConvertsToYards(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToYardsStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Distance.ToYards(value, measurement);

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
        public void Distance_CorrectlyConvertsToMiles(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToMilesStatically(double value, DistanceMeasurement measurement, double factor, ArithmeticOperator arithmeticOperator)
        {
            var expected = arithmeticOperator == ArithmeticOperator.Multiply ? value * factor : value / factor;

            var result = Distance.ToMiles(value, measurement);

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
        public void Distance_CorrectlyConvertsToNM(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value / factor;

            IDistance sut = new Distance(value, measurement);
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
        public void Distance_CorrectlyConvertsToNMFromNMStatically(double value, DistanceMeasurement measurement, double factor)
        {
            var expected = value / factor;

            var result = Distance.ToNauticalMiles(value, measurement);

            var testAssertion = Math.Abs(expected - result) < DoubleFloatingPointTolerance;
            Assert.True(testAssertion);
        }

        private static Distance InstantiateNewDistance()
        {
            return new Distance();
        }
    }
}
