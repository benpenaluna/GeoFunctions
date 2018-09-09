using System;

namespace GeoFunctions.Core.Coordinates.Structs
{
    internal class FormatHelper
    {
        public string Format { get; }
        public IFormatProvider FormatProvider { get; }
        public string FormattedString { get; set; }

        public FormatHelper(string format, IFormatProvider formatProvider, string formattedString)
        {
            Format = format;
            FormatProvider = formatProvider;
            FormattedString = formattedString;
        }

        public bool DegreesRequested => FormattedString.Contains("D");
        public bool MinutesRequested => FormattedString.Contains("M");
        public bool SecondsRequested => FormattedString.Contains("S");
        public bool HemisphereRequested => FormattedString.Contains("H");
    }
}
