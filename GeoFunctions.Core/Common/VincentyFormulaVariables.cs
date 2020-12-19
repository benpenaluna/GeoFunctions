using System;
using System.Collections.Generic;
using System.Text;

namespace GeoFunctions.Core.Common
{
    internal class VincentyFormulaVariables
    {
        public double Lambda { get; set; }
        public double LambdaPrevious { get; set; }
        
        
        public double LongitudeDifference { get; set; }
        public double ReducedLatitudePointA { get; set; }
        public double ReducedLatitudePointB { get; set; }
        
        public double CosReducedLatitudePointA { get; set; }
        public double SinReducedLatitudePointA { get; set; }
        public double CosReducedLatitudePointB { get; set; }
        public double SinReducedLatitudePointB { get; set; }
        
        public double Sigma { get; set; }
        public double DeltaSigma { get; set; }
        public double CosSigma { get; set; }
        public double SinSigma { get; set; }

        public double CosSquaredAlpha { get; set; }
        public double Cos2SigmaM { get; set; }

        public double USquared { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public double GeodesicLength { get; set; }
        public double ForwardAzimuth { get; set; }
        public double BackwardAzimuth { get; set; }

    }
}
