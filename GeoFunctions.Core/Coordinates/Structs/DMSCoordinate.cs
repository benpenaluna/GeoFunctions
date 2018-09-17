using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GeoFunctions.Core.Coordinates.Structs
{
    public struct DmsCoordinate : IFormattable
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
            if (string.IsNullOrEmpty(format))
                format = DefaultFormat;

            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
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

            if (DegreesOnlyRequested(formatHelper))
                FormatDegreesOnly(ref formatHelper);
            else if (DegreesAndMinutesOnlyRequested(formatHelper))
                FormatDegreesMinutes(ref formatHelper);
            else
                formatHelper = FormatDefault(formatHelper);

            return formatHelper.FormattedString;
        }

        private static bool DegreesOnlyRequested(FormatHelper formatHelper)
        {
            return formatHelper.DegreesRequested && !formatHelper.MinutesRequested && !formatHelper.SecondsRequested;
        }

        private void FormatDegreesOnly(ref FormatHelper formatHelper)
        {
            const char charToReplaceInFormatString = 'D';
            var factor = NegationOfDegreesRequired(formatHelper) ? -1 : 1;
            var valueInDecimalDegreesToFormat = (Degrees + Minutes / 60.0 + Seconds / 3600.0) * factor;

            UpdateFormatString(formatHelper, charToReplaceInFormatString, valueInDecimalDegreesToFormat);

            if (formatHelper.HemisphereRequested)
                FormatHemisphere(ref formatHelper);
        }

        private static void UpdateFormatString(FormatHelper helper, char charToReplace, double value)
        {
            var degreesElementHelpers = ReplaceChars(charToReplace, helper.Format);

            foreach (var degreesElementHelper in degreesElementHelpers)
            {
                helper.FormattedString = helper.FormattedString
                    .Replace(degreesElementHelper.StringReplacement,
                        value.ToString(degreesElementHelper.FormatSpecifier, helper.FormatProvider));
            }
        }

        private bool NegationOfDegreesRequired(FormatHelper helper)
        {
            return helper != null && (!helper.HemisphereRequested && (Hemisphere == Hemisphere.South || Hemisphere == Hemisphere.West));
        }

        private static bool DegreesAndMinutesOnlyRequested(FormatHelper formatHelper)
        {
            return formatHelper.DegreesRequested && formatHelper.MinutesRequested && !formatHelper.SecondsRequested;
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

        private static void FormatElement(DmsElement element, double elementSource, ref FormatHelper helper)
        {
            var charToReplace = char.Parse(element.ToString().Substring(0, 1));
            UpdateFormatString(helper, charToReplace, elementSource);
        }

        private FormatHelper FormatDefault(FormatHelper formatHelper)
        {
            if (formatHelper.DegreesRequested)
            {
                FormatElement(DmsElement.Degrees, Degrees, ref formatHelper);
                if (NegationOfDegreesRequired(formatHelper))
                    formatHelper.FormattedString = "-" + formatHelper.FormattedString;
            }
                
            if (formatHelper.MinutesRequested)
                FormatElement(DmsElement.Minutes, Minutes, ref formatHelper);

            if (formatHelper.SecondsRequested)
                FormatElement(DmsElement.Seconds, Seconds, ref formatHelper);

            if (formatHelper.HemisphereRequested)
                FormatHemisphere(ref formatHelper);

            return formatHelper;
        }

        private void FormatHemisphere(ref FormatHelper helper)
        {
            var replacement = Hemisphere.ToString().Substring(0, 1);
            helper.FormattedString = helper.FormattedString.Replace("H", replacement);
        }

        private static IEnumerable<FormatElementHelper> ReplaceChars(char charToReplace, string testObject)
        {
            var helpers = new List<FormatElementHelper>();

            var helper = new FormatElementHelper();

            var formatCharacters = testObject.ToCharArray();
            var charPreviouslyFound = false;
            for (var i = 0; i < formatCharacters.Length; i++)
            {
                if (ExaminingCharToReplace(charToReplace, formatCharacters, i))
                {
                    UpdateHelper('0', charToReplace, helper);
                    charPreviouslyFound = true;
                }
                else if (charPreviouslyFound && ExaminingFullStop(formatCharacters, i) && NextCharacterIsCharToReplace(charToReplace, formatCharacters, i))
                    UpdateHelper('.', '.', helper);
                else
                    charPreviouslyFound = false;

                if (HelpersNotReadyToBeUpdated(helper, formatCharacters, charPreviouslyFound, i))
                    continue;

                helpers.Add(helper);
                helper = new FormatElementHelper();
            }

            return helpers.OrderByDescending(x => x.StringReplacement.Length)
                          .ThenBy(x => x.StringReplacement.Contains('.'));
        }

        private static bool ExaminingCharToReplace(char charToReplace, IReadOnlyList<char> formatCharacters, int i)
        {
            return char.ToUpper(formatCharacters[i]) == charToReplace;
        }

        private static void UpdateHelper(char formatAddition, char replacementAddition, FormatElementHelper helper)
        {
            helper.FormatSpecifier += formatAddition;
            helper.StringReplacement += replacementAddition;
        }

        private static bool ExaminingFullStop(IReadOnlyList<char> formatCharacters, int i)
        {
            return char.ToUpper(formatCharacters[i]) == '.';
        }

        private static bool NextCharacterIsCharToReplace(char charToReplace, IReadOnlyList<char> formatCharacters, int i)
        {
            return i != formatCharacters.Count - 1 && char.ToUpper(formatCharacters[i + 1]) == charToReplace;
        }

        private static bool HelpersNotReadyToBeUpdated(FormatElementHelper helper, IReadOnlyCollection<char> formatCharacters, bool charPreviouslyFound, int i)
        {
            return helper.StringReplacement == null || (charPreviouslyFound && i != formatCharacters.Count - 1);
        }
    }
}
