using System;
using System.Globalization;

namespace GeoFunctions.Core.Coordinates.Structs
{
    public class DmsCoordinate : IFormattable
    {
        private const string DefaultFormat = "DD° MM' SS\" H";

        private const double Tolerance = 1.0E+11;

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
            unchecked
            {
                var hashCode = Degrees.GetHashCode();
                hashCode = (hashCode * 397) ^ Minutes.GetHashCode();
                hashCode = (hashCode * 397) ^ Seconds.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Hemisphere;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return ToString(DefaultFormat, CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider) // TODO: Test for formats that should NOT work
        {
            if (string.IsNullOrEmpty(format))
                format = DefaultFormat;

            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            return FormatString(format, formatProvider);
        }

        private string FormatString(string format, IFormatProvider formatProvider) // TODO: Refactor - code currently passes unit tests, but is not clean, and repeats
        {
            var formatHelper = new FormatHelper(format, formatProvider, format.ToUpper());

            if (formatHelper.DegreesRequested && !formatHelper.MinutesRequested && !formatHelper.SecondsRequested)
            {
                FormatDegreesOnly(ref formatHelper);
            }
            else if (formatHelper.DegreesRequested && formatHelper.MinutesRequested && !formatHelper.SecondsRequested)
            {
                FormatDegreesMinutes(ref formatHelper);
            }
            else if (formatHelper.DegreesRequested && formatHelper.MinutesRequested && formatHelper.SecondsRequested)
            {
                FormatDegreesMinutesSeconds(ref formatHelper);
            }
            else
            {
                if (formatHelper.DegreesRequested)
                    FormatElement(DmsElement.Degrees, Degrees, ref formatHelper);

                if (formatHelper.MinutesRequested)
                    FormatElement(DmsElement.Minutes, Minutes, ref formatHelper);

                if (formatHelper.SecondsRequested)
                    FormatElement(DmsElement.Seconds, Seconds, ref formatHelper);

                if (formatHelper.HemisphereRequested)
                    FormatHemisphere(ref formatHelper);
            }

            return formatHelper.FormattedString;
        }

        private void FormatDegreesOnly(ref FormatHelper helper)
        {
            const char charToReplace = 'D';
            var factor = NegationOfDegreesRequired(helper) ? -1 : 1;
            var degreesValue = (Degrees + Minutes / 60.0 + Seconds / 3600.0) * factor;

            UpdateFormatString(helper, charToReplace, degreesValue);

            if (helper.HemisphereRequested)
                FormatHemisphere(ref helper);
        }

        private static void UpdateFormatString(FormatHelper helper, char charToReplace, double value, bool periodToCheck = true)
        {
            var degreesElementHelper = ReplaceChars(charToReplace, helper.Format, periodToCheck);
            helper.FormattedString = helper.FormattedString
                .Replace(degreesElementHelper.StringReplacement,
                    value.ToString(degreesElementHelper.FormatSpecifier, helper.FormatProvider));
        }

        private bool NegationOfDegreesRequired(FormatHelper helper)
        {
            return helper != null && (!helper.HemisphereRequested && (Hemisphere == Hemisphere.South || Hemisphere == Hemisphere.West));
        }

        private void FormatDegreesMinutes(ref FormatHelper helper)
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

        private void FormatDegreesMinutesSeconds(ref FormatHelper helper)
        {
            FormatElement(DmsElement.Degrees, Degrees, ref helper);
            if (NegationOfDegreesRequired(helper))
                helper.FormattedString = "-" + helper.FormattedString;

            FormatElement(DmsElement.Minutes, Minutes, ref helper);

            FormatElement(DmsElement.Seconds, Seconds, ref helper);

            if (helper.HemisphereRequested)
                FormatHemisphere(ref helper);
        }

        private static void FormatElement(DmsElement element, double elementSource, ref FormatHelper helper)
        {
            var charToReplace = char.Parse(element.ToString().Substring(0, 1));
            var periodToCheck = element == DmsElement.Seconds;

            UpdateFormatString(helper, charToReplace, elementSource, periodToCheck);
        }

        private void FormatHemisphere(ref FormatHelper helper)
        {
            var replacement = Hemisphere.ToString().Substring(0, 1);
            helper.FormattedString = helper.FormattedString.Replace("H", replacement);
        }

        private static FormatElementHelper ReplaceChars(char charToReplace, string testObject, bool checkForPeriod = true)
        {
            var helper = new FormatElementHelper();

            var formatCharacters = testObject.ToCharArray();
            foreach (var c in formatCharacters)
            {
                if (char.ToUpper(c) == charToReplace)
                {
                    helper.FormatSpecifier += "0";
                    helper.StringReplacement += charToReplace;
                    continue;
                }

                if (checkForPeriod && char.ToUpper(c) == '.')
                {
                    helper.FormatSpecifier += ".";
                    helper.StringReplacement += ".";
                }
            }

            return helper;
        }
    }
}
