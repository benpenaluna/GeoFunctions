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

        private const string DefaultFormat = "nu";

        private static readonly List<int> MetricConversionFactors = new List<int> { 1, 10, 1000, 1000000 };

        private static readonly List<MeasurementType> MetricConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Millimeters, 'm'),
            new MeasurementType(DistanceMeasurement.Centimeters, 'c'),
            new MeasurementType(DistanceMeasurement.Meters, 't'),
            new MeasurementType(DistanceMeasurement.Kilometers, 'k')
        };

        private static readonly List<int> ImperialConversionFactors = new List<int> { 1, 12, 36, 63360 };

        private static readonly List<MeasurementType> ImperialConversionReferences = new List<MeasurementType>()
        {
            new MeasurementType(DistanceMeasurement.Inches, 'i'),
            new MeasurementType(DistanceMeasurement.Feet, 'f'),
            new MeasurementType(DistanceMeasurement.Yards, 'y'),
            new MeasurementType(DistanceMeasurement.Miles, 'l')
        };

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (Math.Abs(value) > 1.0E+10)
                {
                    var errorMessage = string.Format("Value must be between -1.0E+10 and 1.0E+10. {0} is an invalid number",
                                                     value.ToString(CultureInfo.InvariantCulture));
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
            return HashCode.Combine(Value, DistanceMeasurement);
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
                    helper.FormattedString = FormatUnits(element, "millimeters", "mm", helper);
                else if (element.PreviousLetter == 'c')
                    helper.FormattedString = FormatUnits(element, "centimeters", "cm", helper);
                else if (element.PreviousLetter == 't')
                    helper.FormattedString = FormatUnits(element, "meters", "m", helper);
                else if (element.PreviousLetter == 'k')
                    helper.FormattedString = FormatUnits(element, "kilometers", "km", helper);
                else if (element.PreviousLetter == 'i')
                    helper.FormattedString = FormatUnits(element, "inches", "\"", helper);
                else if (element.PreviousLetter == 'f')
                    helper.FormattedString = FormatUnits(element, "feet", "'", helper);
                else if (element.PreviousLetter == 'y')
                    helper.FormattedString = FormatUnits(element, "yards", "yd", helper);
                else if (element.PreviousLetter == 'l')
                    helper.FormattedString = FormatUnits(element, "miles", "mi", helper);
            }

            return helper;
        }

        private static string FormatUnits(FormatElementHelper element, string longFormat, string shortFormat, FormatHelper formatHelper)
        {
            return formatHelper.FormattedString.Replace(element.StringReplacement, element.StringReplacement.Length > 1 ? longFormat : shortFormat);
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
