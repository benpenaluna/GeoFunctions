using GeoFunctions.Core.Coordinates;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoFunctions.Core.Common
{
    internal class VincentyFormulaVariables
    {
        public double λ { get; set; }
        public double λPrevious { get; set; }

        public double L { get; set; }
        public double U1 { get; set; }
        public double U2 { get; set; }

        public double TanU1 { get; set; }
        public double CosU1 { get; set; }
        public double SinU1 { get; set; }
        public double CosU2 { get; set; }
        public double SinU2 { get; set; }
        
        public double σ { get; set; }
        public double σPrevious { get; set; }
        public double Δσ { get; set; }
        public double σ1 { get; set; }
        public double Cosσ { get; set; }
        public double Sinσ { get; set; }

        public double sinα { get; set; }
        public double CosSqα { get; set; }
        public double Cos2σM { get; set; }

        public double USquared { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public double GeodesicLength { get; set; }
        public double ForwardAzimuth { get; set; }
        public double BackwardAzimuth { get; set; }
        public IGeographicCoordinate DestinationCoordinates { get; set; }

        public double Sinα1 { get; set; }
        public double Cosα1 { get; set; }

        public double φ2 { get; set; }
        public double λ2 { get; set; }
        public double C { get; set; }
    }
}
