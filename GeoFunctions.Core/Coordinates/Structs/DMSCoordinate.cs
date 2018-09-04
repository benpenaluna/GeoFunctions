using System;
using System.Globalization;
using System.Linq;

namespace GeoFunctions.Core.Coordinates.Structs
{
    public struct DmsCoordinate : IFormattable
    {
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

        public string ToString(string format, IFormatProvider formatProvider) // TODO: Refactor and test for formats that should NOT work
        {
            if (string.IsNullOrEmpty(format))
                format = "D MM SS.ss H";
            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            var reformatted = format.ToUpper();
            
            // handle degrees
            var formatSpecifier = "";
            var stringReplacement = "";
            var formatCharacters = format.ToCharArray();
            foreach (var c in formatCharacters)
            {
                if (char.ToUpper(c) != 'D')
                    continue;

                formatSpecifier += "0";
                stringReplacement += "D";
            }

            reformatted = reformatted.Replace(stringReplacement, Degrees.ToString(formatSpecifier, formatProvider));

            // handle minutes
            formatSpecifier = "";
            stringReplacement = "";
            foreach (var c in formatCharacters)
            {
                if (char.ToUpper(c) != 'M')
                    continue;

                formatSpecifier += "0";
                stringReplacement += "M";
            }

            reformatted = reformatted.Replace(stringReplacement, Minutes.ToString(formatSpecifier, formatProvider));

            // handle seconds
            formatSpecifier = "";
            stringReplacement = "";
            foreach (var c in formatCharacters)
            {
                if (char.ToUpper(c) == 'S')
                {
                    formatSpecifier += "0";
                    stringReplacement += "S";
                    continue;
                }

                if (char.ToUpper(c) != '.')
                    continue;

                formatSpecifier += ".";
                stringReplacement += ".";
            }

            reformatted = reformatted.Replace(stringReplacement, Seconds.ToString(formatSpecifier, formatProvider));

            // handle hemisphere
            var replacement = Hemisphere.ToString().Substring(0, 1);
            reformatted = reformatted.Replace("H", replacement);

            return reformatted;
        }
    }
}
