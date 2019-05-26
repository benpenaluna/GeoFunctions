using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeoFunctions.Core.Common;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public class Elevation : IElevation
    {
        private const double RatioMetersToFeet = 0.3048;
        private const double RatioNMtoMeters = 1852.0;
        private const double RatioNMtoFeet = 6076.115486;

        private const string DefaultFormat = "nu";

        private static readonly List<int> MetricConversionFactors = new List<int> { 1, 10, 1000, 1000000 };

        private static readonly List<MeasurementType> MetricConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Millimeters, 'm', "millimeter", "millimeters", "mm"),
            new MeasurementType(DistanceMeasurement.Centimeters, 'c', "centimeter", "centimeters", "cm"),
            new MeasurementType(DistanceMeasurement.Meters, 't', "meter", "meters", "m"),
            new MeasurementType(DistanceMeasurement.Kilometers, 'k', "kilometer", "kilometers", "km")
        };

        public static List<DistanceMeasurement> MetricDistanceMeasurements => MetricConversionReferences.Select(x => x.Measurement).ToList();

        private static readonly List<int> ImperialConversionFactors = new List<int> { 1, 12, 36, 63360 };

        private static readonly List<MeasurementType> ImperialConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Inches, 'i', "inch", "inches", "\""),
            new MeasurementType(DistanceMeasurement.Feet, 'f', "foot", "feet", "'"),
            new MeasurementType(DistanceMeasurement.Yards, 'y', "yard", "yards", "yd"),
            new MeasurementType(DistanceMeasurement.Miles, 'l', "mile", "miles", "mi")
        };

        public static List<DistanceMeasurement> ImperialDistanceMeasurements => ImperialConversionReferences.Select(x => x.Measurement).ToList();

        private static readonly List<MeasurementType> GeographicConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.NauticalMiles, 'n', "nautical mile", "nautical miles", "nm")
        };

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (Math.Abs(value) > 1.0E+10)
                {
                    var errorMessage = $"Value must be between -1.0E+10 and 1.0E+10. {value.ToString(CultureInfo.InvariantCulture)} is an invalid number";
                    throw new ArgumentException(errorMessage);
                }

                _value = value;
            }
        }

        public DistanceMeasurement DistanceMeasurement { get; protected set; }
        
        public Elevation(double elevation = 0.0, DistanceMeasurement measurement = DistanceMeasurement.Feet)
        {
            Value = elevation;
            DistanceMeasurement = measurement;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Elevation))
            {
                return false;
            }

            var testObject = (Elevation) obj;

            return Value.Equals(testObject.Value) && DistanceMeasurement == testObject.DistanceMeasurement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public override string ToString()
        {
            var measurementSymbol = DistanceMeasurement == DistanceMeasurement.Feet ? "'" : " m";
            return $"{Value.ToString(CultureInfo.CurrentCulture)}{measurementSymbol}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultFormat;

            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture;

            return FormatString(format, formatProvider);
        }

        private string FormatString(string format, IFormatProvider formatProvider)
        {
            var helper = new FormatHelper(format, formatProvider, format);

            foreach (var measurementType in MetricConversionReferences)
            {
                helper = HandleMetricMeasurements(measurementType.Measurement, measurementType.Code, helper);
            }

            foreach (var measurementType in ImperialConversionReferences)
            {
                helper = HandleImperialMeasurements(measurementType.Measurement, measurementType.Code, helper);
            }

            foreach (var measurementType in GeographicConversionReferences)
            {
                helper = HandleGeographicalMeasurements(measurementType.Measurement, measurementType.Code, helper);
            }

            helper = HandleUnits(helper);

            return helper.FormattedString;
        }

        private FormatHelper HandleMetricMeasurements(DistanceMeasurement measurementHandling, char measurementCode, FormatHelper helper)
        {
            var valueElementHelper = helper.Format.FindConsecutiveChars(measurementCode);

            foreach (var element in valueElementHelper)
            {
                var conversionRatio = ConvertDistanceMetricMeasurementTo(measurementHandling);
                helper.FormattedString = helper.FormattedString.Replace(element.StringReplacement, (Value * conversionRatio)
                                                               .ToString(element.FormatSpecifier, helper.FormatProvider));
            }

            return helper;
        }

        private double ConvertDistanceMetricMeasurementTo(DistanceMeasurement convertingTo)
        {
            return ConvertDistanceMetricMeasurement(DistanceMeasurement, convertingTo);
        }

        private static double ConvertDistanceMetricMeasurement(DistanceMeasurement convertingFrom, DistanceMeasurement convertingTo)
        {
            if (convertingFrom == convertingTo)
                return 1.0;

            var distanceMeasurement = MetricDistanceMeasurements.Contains(convertingFrom) ? convertingFrom : DistanceMeasurement.Meters;

            var convertingToReferencePosition = MetricDistanceMeasurements.FindIndex(x => x == convertingTo);
            var previousReferencePosition = MetricDistanceMeasurements.FindIndex(x => x == distanceMeasurement);

            var metricConversionFactor = MetricConversionFactors[previousReferencePosition] / (double)MetricConversionFactors[convertingToReferencePosition];

            if (MetricDistanceMeasurements.Contains(convertingFrom))
                return metricConversionFactor;

            return ImperialDistanceMeasurements.Contains(convertingFrom)
                ? metricConversionFactor * ConvertDistanceImperialMeasurement(convertingFrom, DistanceMeasurement.Feet) * RatioMetersToFeet
                : metricConversionFactor * ConvertDistanceGeographicalMeasurement(convertingFrom, DistanceMeasurement.NauticalMiles) * RatioNMtoMeters;
        }

        private FormatHelper HandleImperialMeasurements(DistanceMeasurement measurementHandling, char measurementCode, FormatHelper helper)
        {
            var valueElementHelper = helper.Format.FindConsecutiveChars(measurementCode);

            foreach (var element in valueElementHelper)
            {
                var conversionRatio = ConvertDistanceImperialMeasurementTo(measurementHandling);
                helper.FormattedString = helper.FormattedString.Replace(element.StringReplacement, (Value * conversionRatio)
                    .ToString(element.FormatSpecifier, helper.FormatProvider));
            }

            return helper;
        }

        private double ConvertDistanceImperialMeasurementTo(DistanceMeasurement convertingTo)
        {
            return ConvertDistanceImperialMeasurement(DistanceMeasurement, convertingTo);
        }

        private static double ConvertDistanceImperialMeasurement(DistanceMeasurement convertingFrom, DistanceMeasurement convertingTo)
        {
            if (convertingFrom == convertingTo)
                return 1.0;

            var distanceMeasurement = ImperialDistanceMeasurements.Contains(convertingFrom) ? convertingFrom : DistanceMeasurement.Feet;

            var convertingToReferencePosition = ImperialDistanceMeasurements.FindIndex(x => x == convertingTo);
            var previousReferencePosition = ImperialDistanceMeasurements.FindIndex(x => x == distanceMeasurement);

            var imperialConversionFactor = (double)ImperialConversionFactors[previousReferencePosition] / ImperialConversionFactors[convertingToReferencePosition];

            if (ImperialDistanceMeasurements.Contains(convertingFrom))
                return imperialConversionFactor;

            return MetricDistanceMeasurements.Contains(convertingFrom)
                ? imperialConversionFactor / ConvertDistanceMetricMeasurement(convertingFrom, DistanceMeasurement.Meters) / RatioMetersToFeet
                : imperialConversionFactor / ConvertDistanceGeographicalMeasurement(convertingFrom, DistanceMeasurement.NauticalMiles) * RatioNMtoFeet;
        }

        private FormatHelper HandleGeographicalMeasurements(DistanceMeasurement measurementHandling, char measurementCode, FormatHelper helper)
        {
            var valueElementHelper = helper.Format.FindConsecutiveChars(measurementCode);

            foreach (var element in valueElementHelper)
            {
                var conversionRatio = ConvertDistanceGeographicalMeasurementTo(measurementHandling);
                helper.FormattedString = helper.FormattedString.Replace(element.StringReplacement, (Value / conversionRatio)
                    .ToString(element.FormatSpecifier, helper.FormatProvider));
            }

            return helper;
        }

        private double ConvertDistanceGeographicalMeasurementTo(DistanceMeasurement convertingTo)
        {
            return ConvertDistanceGeographicalMeasurement(DistanceMeasurement, convertingTo);
        }

        private static double ConvertDistanceGeographicalMeasurement(DistanceMeasurement convertingFrom, DistanceMeasurement convertingTo)
        {
            if (convertingFrom == convertingTo)
                return 1.0;

            var geographicalConversionReferences = GeographicConversionReferences.Select(x => x.Measurement).ToList();
            var geographicalConversionFactor = 1.0;
            if (geographicalConversionReferences.Contains(convertingFrom))
                return geographicalConversionFactor;

            return MetricDistanceMeasurements.Contains(convertingFrom)
                ? geographicalConversionFactor / ConvertDistanceMetricMeasurement(convertingFrom, DistanceMeasurement.Meters) * RatioNMtoMeters
                : geographicalConversionFactor / ConvertDistanceImperialMeasurement(convertingFrom, DistanceMeasurement.Feet) * RatioNMtoFeet;
        }

        private static FormatHelper HandleUnits(FormatHelper helper)
        {
            var valueElementHelper = helper.Format.FindConsecutiveChars('u');

            foreach (var element in valueElementHelper)
            {
                if (element.PreviousLetter == 'm')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Millimeters, helper);
                else if (element.PreviousLetter == 'c')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Centimeters, helper);
                else if (element.PreviousLetter == 't')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Meters, helper);
                else if (element.PreviousLetter == 'k')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Kilometers, helper);
                else if (element.PreviousLetter == 'i')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Inches, helper);
                else if (element.PreviousLetter == 'f')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Feet, helper);
                else if (element.PreviousLetter == 'y')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Yards, helper);
                else if (element.PreviousLetter == 'l')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.Miles, helper);
                else if (element.PreviousLetter == 'n')
                    helper.FormattedString = FormatUnits(element, DistanceMeasurement.NauticalMiles, helper);
            }

            return helper;
        }

        private static string FormatUnits(FormatElementHelper element, DistanceMeasurement measurement, FormatHelper formatHelper)
        {
            var measurementTypes = new List<MeasurementType>();
            measurementTypes.AddRange(MetricConversionReferences);
            measurementTypes.AddRange(ImperialConversionReferences);
            measurementTypes.AddRange(GeographicConversionReferences);

            var measurementType = measurementTypes.FirstOrDefault(x => x.Measurement == measurement);
            if (measurementType is null)
                throw new NullReferenceException();

            if (element.StringReplacement.Length == 1)
                return formatHelper.FormattedString.Replace(element.StringReplacement, measurementType.Abbreviation);

            var measurementTypeToUse = DetermineMeasurementPlurality(element, formatHelper, measurementType);
            return formatHelper.FormattedString.Replace(element.StringReplacement, measurementTypeToUse);
        }

        private static string DetermineMeasurementPlurality(FormatElementHelper element, FormatHelper formatHelper, MeasurementType measurementType)
        {
            var test = formatHelper.FormattedString.Split(new[] { element.StringReplacement }, StringSplitOptions.RemoveEmptyEntries);
            var measurementToUse = measurementType.PluralName;

            double.TryParse(test[0].Trim(), out var numericValue);
            if (Math.Abs(numericValue - 1.0) < 1.0E-11)
                measurementToUse = measurementType.SingularName;

            return measurementToUse;
        }

        public double ToMillimeters()
        {
            return ToMeters() * 1000.0;
        }

        public static double ToMillimeters(double value, DistanceMeasurement measurement)
        {
            return ToMeters(value, measurement) * 1000.0;
        }

        public double ToCentimeters()
        {
            return ToMeters() * 100.0;
        }

        public static double ToCentimeters(double value, DistanceMeasurement measurement)
        {
            return ToMeters(value, measurement) * 100.0;
        }

        public double ToMeters()
        {
            return DistanceMeasurement == DistanceMeasurement.Meters ? Value : ToMeters(Value, DistanceMeasurement);
        }

        public static double ToMeters(double value, DistanceMeasurement measurement)
        {
            if (measurement == DistanceMeasurement.Meters)
                return value;

            if (MetricDistanceMeasurements.Contains(measurement))
                return value * ConvertDistanceMetricMeasurement(measurement, DistanceMeasurement.Meters);

            if (ImperialDistanceMeasurements.Contains(measurement))
            {
                var valueInFeet = value * ConvertDistanceImperialMeasurement( measurement, DistanceMeasurement.Feet);
                return valueInFeet * RatioMetersToFeet;
            }

            if (measurement == DistanceMeasurement.NauticalMiles)
                return value * RatioNMtoMeters;

            return double.NaN;
        }

        public double ToKilometers()
        {
            return ToMeters() / 1000.0;
        }

        public static double ToKilometers(double value, DistanceMeasurement measurement)
        {
            return ToMeters(value, measurement) / 1000.0;
        }

        public double ToInches()
        {
            if (DistanceMeasurement == DistanceMeasurement.Inches)
                return Value;

            if (DistanceMeasurement == DistanceMeasurement.Millimeters)
                return Value / 25.4;

            return ToFeet() * 12.0;
        }

        public static double ToInches(double value, DistanceMeasurement measurement)
        {
            if (measurement == DistanceMeasurement.Inches)
                return value;

            if (measurement == DistanceMeasurement.Millimeters)
                return value / 25.4;

            return ToFeet(value, measurement) * 12.0;
        }

        public double ToFeet()
        {
            if (DistanceMeasurement == DistanceMeasurement.Inches)
                return Value / 12.0;

            return DistanceMeasurement == DistanceMeasurement.Feet ? Value : ToFeet(Value, DistanceMeasurement);  // TODO: Refactor to account for each conversion ratio
        }

        public static double ToFeet(double value, DistanceMeasurement measurement)
        {
            if (measurement == DistanceMeasurement.Feet)
                return value;

            if (measurement == DistanceMeasurement.Kilometers)
                return value / RatioMetersToFeet * 1000.0;

            if (MetricDistanceMeasurements.Contains(measurement))
            {
                var valueInMeters = value * ConvertDistanceMetricMeasurement(measurement, DistanceMeasurement.Meters);
                return valueInMeters / RatioMetersToFeet;
            }

            if (measurement == DistanceMeasurement.Inches)
                return value / 12.0;

            if (ImperialDistanceMeasurements.Contains(measurement))
            {
                return value * ConvertDistanceImperialMeasurement(measurement, DistanceMeasurement.Feet);
            }

            if (measurement == DistanceMeasurement.NauticalMiles)
                return value * RatioNMtoFeet;

            return double.NaN;
        }

        public double ToNauticalMiles()
        {
            return DistanceMeasurement == DistanceMeasurement.NauticalMiles ? Value : ToNauticalMiles(Value, DistanceMeasurement);
        }

        public static double ToNauticalMiles(double value, DistanceMeasurement measurement)
        {
            if (measurement == DistanceMeasurement.NauticalMiles)
                return value;

            if (measurement == DistanceMeasurement.Kilometers)
            {
                return value / (RatioNMtoMeters / 1000.0);
            }

            if (MetricDistanceMeasurements.Contains(measurement))
            {
                var valueInMeters = value * ConvertDistanceMetricMeasurement(measurement, DistanceMeasurement.Meters);
                return valueInMeters / RatioNMtoMeters;
            }

            if (ImperialDistanceMeasurements.Contains(measurement))
            {
                var valueInFeet = value * ConvertDistanceImperialMeasurement( measurement, DistanceMeasurement.Feet);
                return valueInFeet / RatioNMtoFeet;
            }

            return double.NaN;
        }
    }
}
