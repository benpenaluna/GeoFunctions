using GeoFunctions.Core.Coordinates.Measurement;

namespace GeoFunctions.Core.Coordinates
{
    internal class MeasurementType
    {
        public DistanceMeasurement Measurement { get; set; }
        public char Code { get; set; }
        public string SingularName { get; set; }
        public string PluralName { get; set; }
        public string Abbreviation { get; set; }
        public string Symbol { get; set; }

        public MeasurementType(DistanceMeasurement measurement, char code, string singular, string plural, string abbreviation, string symbol = null)
        {
            Measurement = measurement;
            Code = code;
            SingularName = singular;
            PluralName = plural;
            Abbreviation = abbreviation;
            Symbol = symbol ?? abbreviation;
        }
    }
}
