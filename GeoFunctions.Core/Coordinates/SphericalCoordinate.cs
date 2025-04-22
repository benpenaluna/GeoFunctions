using System;
using System.Globalization;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public abstract class SphericalCoordinate : ISphericalCoordinate
    {
        public abstract IAngle Angle { get; set; }

        public abstract DmsCoordinate DmsCoordinate { get; }

        protected DmsCoordinate CalculateDmsCoordinate(Hemisphere hemisphere) 
        {
            var value = Math.Abs(Angle.Value);
            var degrees = Convert.ToInt32(Math.Floor(value));
            var minutes = Convert.ToInt32(Math.Floor((value - degrees) * 60.0));
            var seconds = ((value - degrees) * 60.0 - minutes) * 60.0;

            CorrectIfSecondsGreaterThan60(ref minutes, ref seconds);
            CorrectIfMinutesGreaterThan60(ref degrees, ref minutes);

            return new DmsCoordinate(degrees, minutes, seconds, hemisphere);
        }

        private static void CorrectIfSecondsGreaterThan60(ref int minutes, ref double seconds)
        {
            if (seconds < 60.0)
                return;

            seconds -= 60.0;
            if (seconds < 0)
            {
                seconds = 0;
            }

            minutes += 1;
        }

        private static void CorrectIfMinutesGreaterThan60(ref int degrees, ref int minutes)
        {
            if (minutes < 60.0)
                return;

            minutes -= 60;
            if (minutes < 0)
            {
                minutes = 0;
            }

            degrees += 1;
        }

        public override string ToString()
        {
            return DmsCoordinate.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return ToString();

            if (formatProvider == null)
                formatProvider = CultureInfo.CurrentCulture;

            return DmsCoordinate.ToString(format, formatProvider);
        }
    }
}
