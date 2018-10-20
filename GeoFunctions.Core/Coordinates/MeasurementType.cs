using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    internal class MeasurementType
    {
        public DistanceMeasurement Measurement { get; set; }
        public char Code { get; set; }

        public MeasurementType(DistanceMeasurement measurement, char code)
        {
            Measurement = measurement;
            Code = code;
        }
    }
}
