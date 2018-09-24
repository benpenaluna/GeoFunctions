using System;

namespace GeoFunctions.Core.Coordinates
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
    }
}
