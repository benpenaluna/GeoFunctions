using System;

namespace GeoFunctions.Core.Coordinates.Structs
{
    internal class DmsCoordinateFormatHelper : FormatHelper
    {
        public bool DegreesRequested => FormattedString.Contains("D");
        public bool MinutesRequested => FormattedString.Contains("M");
        public bool SecondsRequested => FormattedString.Contains("S");
        public bool HemisphereRequested => FormattedString.Contains("H");

        public DmsCoordinateFormatHelper(string format, IFormatProvider formatProvider, string formattedString) : base(format, formatProvider, formattedString) { }
    }
}
