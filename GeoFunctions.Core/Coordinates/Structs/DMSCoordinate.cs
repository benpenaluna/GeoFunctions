using System;
using System.Globalization;
using GeoFunctions.Core.Common;

namespace GeoFunctions.Core.Coordinates.Structs
{
    public struct DmsCoordinate : IFormattable
    {
        private const string DefaultFormat = "DD° MM' SS\"H";

        private const double Tolerance = 1.0E-11;

        public double Degrees;
        public double Minutes;
        public double Seconds;
        public Hemisphere Hemisphere;

        public override bool Equals(object obj)
        {
            return obj is DmsCoordinate coordinate && Equals(coordinate);
        }

        public bool Equals(DmsCoordinate other)
        {
            return Math.Abs(Degrees - other.Degrees) < Tolerance &&
                   Math.Abs(Minutes - other.Minutes) < Tolerance && 
                   Math.Abs(Seconds - other.Seconds) < Tolerance && 
                   Hemisphere == other.Hemisphere;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DefaultFormat, Tolerance);
        }

        public override string ToString()
        {
            return ToString(DefaultFormat, CultureInfo.InvariantCulture);
        }

        public string ToString(string format)
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultFormat;

            return ToString(format, CultureInfo.InvariantCulture);
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
            var formatHelper = new DmsCoordinateFormatHelper(format, formatProvider, format.ToUpper());

            if (DegreesOnlyRequested(formatHelper))
                FormatDegreesOnly(ref formatHelper);
            else if (DegreesAndMinutesOnlyRequested(formatHelper))
                FormatDegreesMinutes(ref formatHelper);
            else
                formatHelper = FormatDefault(formatHelper);

            return formatHelper.FormattedString;
        }

        private static bool DegreesOnlyRequested(DmsCoordinateFormatHelper dmsCoordinateFormatHelper)
        {
            return dmsCoordinateFormatHelper.DegreesRequested && !dmsCoordinateFormatHelper.MinutesRequested && !dmsCoordinateFormatHelper.SecondsRequested;
        }

        private void FormatDegreesOnly(ref DmsCoordinateFormatHelper dmsCoordinateFormatHelper)
        {
            const char charToReplaceInFormatString = 'D';
            var factor = NegationOfDegreesRequired(dmsCoordinateFormatHelper) ? -1 : 1;
            var valueInDecimalDegreesToFormat = (Degrees + Minutes / 60.0 + Seconds / 3600.0) * factor;

            UpdateFormatString(dmsCoordinateFormatHelper, charToReplaceInFormatString, valueInDecimalDegreesToFormat);

            if (dmsCoordinateFormatHelper.HemisphereRequested)
                FormatHemisphere(ref dmsCoordinateFormatHelper);
        }

        private static void UpdateFormatString(DmsCoordinateFormatHelper helper, char charToReplace, double value)
        {
            var degreesElementHelpers = helper.Format.FindConsecutiveChars(charToReplace);

            foreach (var degreesElementHelper in degreesElementHelpers)
            {
                helper.FormattedString = helper.FormattedString
                                               .Replace(degreesElementHelper.StringReplacement,
                                                        value.ToString(degreesElementHelper.FormatSpecifier, helper.FormatProvider));
            }
        }

        private bool NegationOfDegreesRequired(DmsCoordinateFormatHelper helper)
        {
            return helper != null && (!helper.HemisphereRequested && (Hemisphere == Hemisphere.South || Hemisphere == Hemisphere.West));
        }

        private static bool DegreesAndMinutesOnlyRequested(DmsCoordinateFormatHelper dmsCoordinateFormatHelper)
        {
            return dmsCoordinateFormatHelper.DegreesRequested && dmsCoordinateFormatHelper.MinutesRequested && !dmsCoordinateFormatHelper.SecondsRequested;
        }

        private void FormatDegreesMinutes(ref DmsCoordinateFormatHelper helper)
        {
            FormatElement(DmsElement.Degrees, Degrees, ref helper);
            if (NegationOfDegreesRequired(helper))
                helper.FormattedString = "-" + helper.FormattedString;

            const char charToReplace = 'M';
            var minutesValue = Minutes + Seconds / 60.0;

            UpdateFormatString(helper, charToReplace, minutesValue);

            if (helper.HemisphereRequested)
                FormatHemisphere(ref helper);
        }

        private static void FormatElement(DmsElement element, double elementSource, ref DmsCoordinateFormatHelper helper)
        {
            var charToReplace = char.Parse(element.ToString().Substring(0, 1));
            UpdateFormatString(helper, charToReplace, elementSource);
        }

        private DmsCoordinateFormatHelper FormatDefault(DmsCoordinateFormatHelper dmsCoordinateFormatHelper)
        {
            var degrees = Degrees;
            var minutes = Minutes;
            var seconds = Seconds;

            CorrectIfSecondsGreaterThan60(dmsCoordinateFormatHelper, ref minutes, ref seconds);
            CorrectIfMinutesGreaterThan60(dmsCoordinateFormatHelper, ref degrees, ref minutes);

            if (dmsCoordinateFormatHelper.DegreesRequested)
            {
                FormatElement(DmsElement.Degrees, degrees, ref dmsCoordinateFormatHelper);
                if (NegationOfDegreesRequired(dmsCoordinateFormatHelper))
                    dmsCoordinateFormatHelper.FormattedString = "-" + dmsCoordinateFormatHelper.FormattedString;
            }

            if (dmsCoordinateFormatHelper.MinutesRequested)
                FormatElement(DmsElement.Minutes, minutes, ref dmsCoordinateFormatHelper);

            if (dmsCoordinateFormatHelper.SecondsRequested)
                FormatElement(DmsElement.Seconds, seconds, ref dmsCoordinateFormatHelper);

            if (dmsCoordinateFormatHelper.HemisphereRequested)
                FormatHemisphere(ref dmsCoordinateFormatHelper);

            return dmsCoordinateFormatHelper;
        }

        private static void CorrectIfSecondsGreaterThan60(DmsCoordinateFormatHelper dmsCoordinateFormatHelper, ref double minutes, ref double seconds)
        {
            if (dmsCoordinateFormatHelper.SecondsRequested)
            {
                var charToReplace = char.Parse(DmsElement.Seconds.ToString().Substring(0, 1));
                var degreesElementHelpers = dmsCoordinateFormatHelper.Format.FindConsecutiveChars(charToReplace);

                foreach (var degreesElementHelper in degreesElementHelpers)
                {
                    var strSecs = seconds.ToString(degreesElementHelper.FormatSpecifier);
                    var secondsValue = Double.Parse(strSecs);
                    if (secondsValue >= 60)
                    {
                        seconds -= 60;
                        if (seconds < 0)
                        {
                            seconds = 0;
                        }

                        minutes += 1;
                    }
                }
            }
        }

        private static void CorrectIfMinutesGreaterThan60(DmsCoordinateFormatHelper dmsCoordinateFormatHelper, ref double degrees, ref double minutes)
        {
            if (dmsCoordinateFormatHelper.MinutesRequested)
            {
                var charToReplace = char.Parse(DmsElement.Minutes.ToString().Substring(0, 1));
                var degreesElementHelpers = dmsCoordinateFormatHelper.Format.FindConsecutiveChars(charToReplace);

                foreach (var degreesElementHelper in degreesElementHelpers)
                {
                    var strMins = minutes.ToString(degreesElementHelper.FormatSpecifier);
                    minutes = Double.Parse(strMins);
                    if (minutes >= 60)
                    {
                        minutes -= 60;
                        if (minutes < 0)
                        {
                            minutes = 0;
                        }

                        degrees += 1;
                    }
                }
            }
        }

        private void FormatHemisphere(ref DmsCoordinateFormatHelper helper)
        {
            var replacement = Hemisphere.ToString().Substring(0, 1);
            helper.FormattedString = helper.FormattedString.Replace("H", replacement);
        }
    }
}
