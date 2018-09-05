using System;

namespace GeoFunctions.Core.Coordinates.Structs
{
    internal class FormatHelper
    {
        public string Format { get; private set; }
        public IFormatProvider FormatProvider { get; private set; }
        public string FormattedString { get; set; }

        public FormatHelper(string format, IFormatProvider formatProvider, string formattedString)
        {
            Format = format;
            FormatProvider = formatProvider;
            FormattedString = formattedString;
        }
    }
}
