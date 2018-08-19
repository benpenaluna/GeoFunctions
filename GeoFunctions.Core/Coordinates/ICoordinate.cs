using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Core.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    public interface ICoordinate
    {
        double Value { get; set; }
        AngleMeasurement AngleMeasurement { get; set; }
    }
}
