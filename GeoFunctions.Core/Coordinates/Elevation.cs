using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using GeoFunctions.Core.Common;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public class Elevation : IElevation
    {
        private const double RatioMetersToFeet = 0.3048;

        private const string DefaultFormat = "nu";

        private static readonly List<int> MetricConversionFactors = new List<int> { 1, 10, 1000, 1000000 };

        private static readonly List<MeasurementType> MetricConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Millimeters, 'm', "millimeter", "millimeters", "mm"),
            new MeasurementType(DistanceMeasurement.Centimeters, 'c', "centimeter", "centimeters", "cm"),
            new MeasurementType(DistanceMeasurement.Meters, 't', "meter", "meters", "m"),
            new MeasurementType(DistanceMeasurement.Kilometers, 'k', "kilometer", "kilometers", "km")
        };

        private static readonly List<int> ImperialConversionFactors = new List<int> { 1, 12, 36, 63360 };

        private static readonly List<MeasurementType> ImperialConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Inches, 'i', "inch", "inches", "\""),
            new MeasurementType(DistanceMeasurement.Feet, 'f', "foot", "feet", "'"),
            new MeasurementType(DistanceMeasurement.Yards, 'y', "yard", "yards", "yd"),
            new MeasurementType(DistanceMeasurement.Miles, 'l', "mile", "miles", "mi")
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
            if (DistanceMeasurement == convertingTo)
                return 1.0;

            var conversionReferences = MetricConversionReferences.Select(x => x.Measurement).ToList();
            var distanceMeasurement = conversionReferences.Contains(DistanceMeasurement) ? DistanceMeasurement : DistanceMeasurement.Meters;

            var convertingToReferencePosition = conversionReferences.FindIndex(x => x == convertingTo);
            var previousReferencePosition = conversionReferences.FindIndex(x => x == distanceMeasurement);

            var metricConversionFactor = MetricConversionFactors[previousReferencePosition] / (double)MetricConversionFactors[convertingToReferencePosition];
            return conversionReferences.Contains(DistanceMeasurement)
                ? metricConversionFactor
                : metricConversionFactor * ConvertDistanceImperialMeasurementTo(DistanceMeasurement.Feet) * RatioMetersToFeet;
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
            if (DistanceMeasurement == convertingTo)
                return 1.0;

            var conversionReferences = ImperialConversionReferences.Select(x => x.Measurement).ToList();
            var distanceMeasurement = conversionReferences.Contains(DistanceMeasurement) ? DistanceMeasurement : DistanceMeasurement.Feet;

            var convertingToReferencePosition = conversionReferences.FindIndex(x => x == convertingTo);
            var previousReferencePosition = conversionReferences.FindIndex(x => x == distanceMeasurement);

            var imperialConversionFactor = (double)ImperialConversionFactors[previousReferencePosition] / ImperialConversionFactors[convertingToReferencePosition];
            return conversionReferences.Contains(DistanceMeasurement)
                ? imperialConversionFactor
                : imperialConversionFactor / ConvertDistanceMetricMeasurementTo(DistanceMeasurement.Meters) / RatioMetersToFeet;
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
            }

            return helper;
        }

        private static string FormatUnits(FormatElementHelper element, DistanceMeasurement measurement, FormatHelper formatHelper)
        {
            var measurementTypes = new List<MeasurementType>();
            measurementTypes.AddRange(MetricConversionReferences);
            measurementTypes.AddRange(ImperialConversionReferences);

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
            if (test.Length > 0 && test[0].Trim() == "1")
                measurementToUse = measurementType.SingularName;

            return measurementToUse;
        }

        public static double ToFeet(double valueInMeters)
        {
            return valueInMeters / RatioMetersToFeet;
        }

        public double ToFeet()
        {
            return DistanceMeasurement == DistanceMeasurement.Feet ? Value : ToFeet(Value);
        }

        public static double ToMeters(double valueInFeet)
        {
            return valueInFeet * RatioMetersToFeet;
        }

        public double ToMeters()
        {
            return DistanceMeasurement == DistanceMeasurement.Meters ? Value : ToMeters(Value);
        }
    }
}
