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
            return DmsCoordinate.ConvertAngleToDms(Angle, hemisphere);
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
