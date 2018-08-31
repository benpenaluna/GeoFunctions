using System;
using GeoFunctions.Core.Coordinates.Structs;

namespace GeoFunctions.Core.Coordinates
{
    public abstract class SphericalCoordinate : ISphericalCoordinate
    {
        public abstract IAngle Angle { get; set; }

        public abstract DmsCoordinate DmsCoordinate { get; }

        protected DmsCoordinate CalculateDmsCoordinate(Hemisphere hemisphere) 
        {
            var value = Math.Abs(Angle.CoTerminalValue);
            var degrees = Math.Floor(value);
            var minutes = Math.Floor((value - degrees) * 60.0);
            var seconds = (minutes - Math.Floor(minutes)) * 60.0;

            CorrectIfSecondsGreaterThan60(ref minutes, ref seconds);
            CorrectIfMinutesGreaterThan60(ref degrees, ref minutes);

            return new DmsCoordinate()
            {
                Degrees = degrees,
                Minutes = minutes,
                Seconds = seconds,
                Hemisphere = hemisphere
            };
        }

        private static void CorrectIfSecondsGreaterThan60(ref double minutes, ref double seconds)
        {
            if (seconds < 60.0)
                return;

            seconds -= 60.0;
            minutes += 1.0;
        }

        private static void CorrectIfMinutesGreaterThan60(ref double degrees, ref double minutes)
        {
            if (minutes < 60.0)
                return;

            minutes -= 60.0;
            degrees += 1.0;
        }
    }
}
