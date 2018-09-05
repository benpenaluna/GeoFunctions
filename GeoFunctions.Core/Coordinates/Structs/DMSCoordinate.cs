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

        private string FormatString(string format, IFormatProvider formatProvider)
        {
            var formatHelper = new FormatHelper(format, formatProvider, format.ToUpper());

            FormatElement(DmsElement.Degrees, Degrees, ref formatHelper);
            FormatElement(DmsElement.Minutes, Minutes, ref formatHelper);
            FormatElement(DmsElement.Seconds, Seconds, ref formatHelper);
            FormatHemisphere(ref formatHelper);

            return formatHelper.FormattedString;
        }

        private static void FormatElement(DmsElement element, double elementSource, ref FormatHelper helper)
        {
            var charToReplace = char.Parse(element.ToString().Substring(0, 1));
            var periodToCheck = element == DmsElement.Seconds;

            var degreesElementHelper = ReplaceChars(charToReplace, helper.Format, periodToCheck);
            helper.FormattedString = helper.FormattedString.Replace(degreesElementHelper.StringReplacement, elementSource.ToString(degreesElementHelper.FormatSpecifier, helper.FormatProvider));
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

        private void FormatHemisphere(ref FormatHelper helper)
        {
            var replacement = Hemisphere.ToString().Substring(0, 1);
            helper.FormattedString = helper.FormattedString.Replace("H", replacement);
        }
    }
}
