using System;
using System.Globalization;
using GeoFunctions.Core.Common;

namespace GeoFunctions.Core.Coordinates.Structs
{
    public struct DmsCoordinate : IFormattable
    {
        private const string DefaultFormat = "DD° MM' SS\"H";

        private const double Tolerance = 1.0E-11;

        private const double MaxNorthSouthAngle = 90.0;
        private const double MaxEastWestAngle = 180.0;

        private int _degrees;
        private int _minutes;
        private double _seconds;
        private Hemisphere _hemisphere;
        
        public int Degrees
        {
            get => _degrees;
            set
            {
                CheckDMSInRange(value, Minutes, Seconds);

                switch (Hemisphere)
                {
                    case Hemisphere.North:
                    case Hemisphere.South:
                        if (value < 0 || value > MaxNorthSouthAngle)
                            throw new ArgumentOutOfRangeException(nameof(value));

                        break;
                   case Hemisphere.East:
                        if (value < 0 || value > MaxEastWestAngle)
                            throw new ArgumentOutOfRangeException(nameof(value));

                        break;
                    case Hemisphere.West:
                        if (value < 0 || value >= MaxEastWestAngle)
                            throw new ArgumentOutOfRangeException(nameof(value));

                        break;
                }

                _degrees = value;
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                CheckDMSInRange(Degrees, value, Seconds);
                
                if (value < 0.0 || value >= 60.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _minutes = value;
            }
        }

        public double Seconds
        {
            get => _seconds;
            set
            {
                CheckDMSInRange(Degrees, Minutes, value);

                if (value < 0.0 || value >= 60.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _seconds = value;
            }
        }

        public Hemisphere Hemisphere
        {
            get => _hemisphere;
            set
            {
                if ((value == Hemisphere.North || value == Hemisphere.South) &&
                    (_hemisphere == Hemisphere.East || _hemisphere == Hemisphere.West) &&
                    (Angle(Degrees, Minutes, Seconds) > MaxNorthSouthAngle))
                {
                    var exceptionMessage = "The sum of the degrees, minutes and seconds yields a value greater than ";
                    throw new ArgumentOutOfRangeException(string.Concat(exceptionMessage, MaxNorthSouthAngle));
                }
                
                _hemisphere = value;
            }
        }

        public DmsCoordinate()
        {
           Hemisphere = Hemisphere.North;
        }

        public DmsCoordinate(int degrees, Hemisphere hemisphere)
        {
            Hemisphere = hemisphere;
            Degrees = degrees;
        }

        public DmsCoordinate(int degrees, int minutes, Hemisphere hemisphere)
        {
            Hemisphere = hemisphere;

            CheckDMSInRange(degrees, minutes, 0.0);

            Degrees = degrees;
            Minutes = minutes;
        }

        public DmsCoordinate(int degrees, int minutes, double seconds, Hemisphere hemisphere)
        {
            Hemisphere = hemisphere;
            
            CheckDMSInRange(degrees, minutes, seconds);

            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
        }

        private void CheckDMSInRange(double degrees, double minutes, double seconds)
        {
            var angle = Angle(degrees, minutes, seconds);
            var exceptionMessage = "The sum of the degrees, minutes and seconds yields a value greater than ";
            switch (Hemisphere)
            {
                case Hemisphere.North:
                case Hemisphere.South:
                    if (angle < 0 || angle > MaxNorthSouthAngle)
                        throw new ArgumentOutOfRangeException(string.Concat(exceptionMessage, MaxNorthSouthAngle));

                    break;
                case Hemisphere.East:
                    if (angle < 0 || angle > MaxEastWestAngle)
                        throw new ArgumentOutOfRangeException(string.Concat(exceptionMessage, MaxEastWestAngle));

                    break;
                case Hemisphere.West:
                    if (angle < 0 || angle >= MaxEastWestAngle)
                    {
                        exceptionMessage = "The sum of the degrees, minutes and seconds yields a value greater than or equal to ";
                        throw new ArgumentOutOfRangeException(string.Concat(exceptionMessage, MaxEastWestAngle));
                    }

                    break;
            }
        }

        private double Angle(double degrees, double minutes, double seconds) => degrees + minutes / 60.0 + seconds / 3600.0;

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

        private static void CorrectIfSecondsGreaterThan60(DmsCoordinateFormatHelper dmsCoordinateFormatHelper, ref int minutes, ref double seconds)
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

        private static void CorrectIfMinutesGreaterThan60(DmsCoordinateFormatHelper dmsCoordinateFormatHelper, ref int degrees, ref int minutes)
        {
            if (dmsCoordinateFormatHelper.MinutesRequested)
            {
                var charToReplace = char.Parse(DmsElement.Minutes.ToString().Substring(0, 1));
                var degreesElementHelpers = dmsCoordinateFormatHelper.Format.FindConsecutiveChars(charToReplace);

                foreach (var degreesElementHelper in degreesElementHelpers)
                {
                    var strMins = minutes.ToString(degreesElementHelper.FormatSpecifier);
                    minutes = Convert.ToInt32(double.Parse(strMins));
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
