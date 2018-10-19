using System;
using System.Collections.Generic;
using System.Globalization;
using GeoFunctions.Core.Common;
using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public class Elevation : IElevation
    {
        private const double RatioMetersToFeet = 0.3048;

        private const string DefaultFormat = "nu";

        private static readonly List<int> MetricConversionFactors = new List<int> { 1, 10, 1000, 1000000 };

        private static readonly List<DistanceMeasurement> MetricConversionReference = new List<DistanceMeasurement>()
        {
            DistanceMeasurement.Millimeters,
            DistanceMeasurement.Centimeters,
            DistanceMeasurement.Meters,
            DistanceMeasurement.Kilometers
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

            helper = HandleMeasurements(DistanceMeasurement.Millimeters, 'm', helper);
            helper = HandleMeasurements(DistanceMeasurement.Centimeters, 'c', helper);
            helper = HandleMeasurements(DistanceMeasurement.Meters, 't', helper);
            helper = HandleMeasurements(DistanceMeasurement.Kilometers, 'k', helper);

            helper = HandleUnits(helper);

            return helper.FormattedString;
        }

        private FormatHelper HandleMeasurements(DistanceMeasurement measurementHandling, char measurementCode, FormatHelper helper)
        {
            var valueElementHelper = helper.Format.FindConsecutiveChars(measurementCode);

            foreach (var element in valueElementHelper)
            {
                var conversionRatio = ConvertDistanceMeasurementTo(measurementHandling);
                helper.FormattedString = helper.FormattedString.Replace(element.StringReplacement, (Value * conversionRatio).ToString(element.FormatSpecifier, helper.FormatProvider));
            }

            return helper;
        }

        private double ConvertDistanceMeasurementTo(DistanceMeasurement convertingTo)
        {
            if (DistanceMeasurement == convertingTo)
                return 1.0;

            var centimetersReferencePosition = MetricConversionReference.FindIndex(x => x == convertingTo);
            var previousReferencePosition = MetricConversionReference.FindIndex(x => x == DistanceMeasurement);
            return (double) MetricConversionFactors[previousReferencePosition] / MetricConversionFactors[centimetersReferencePosition];
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
