using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using System;

namespace GeoFunctions.Core.Common
{
    public static class Vincenty
    {
        private const double semiMajorAxis = 6378137.0;
        private const double ellipsoidFlattening = 1.0 / 298.257223563;
        private static double semiMinorAxis = (1.0 - ellipsoidFlattening) * semiMajorAxis;

        internal static VincentyFormulaVariables V { get; set; }

        public static IDistance GreatCircleDistanceTo(this IGeographicCoordinate pointA, IGeographicCoordinate pointB, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            return VincentysInverseFormula(pointA, pointB, maxInterations, tolerance).Distance;
        }

        public static IAngle BearingTo(this IGeographicCoordinate pointA, IGeographicCoordinate pointB, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            return VincentysInverseFormula(pointA, pointB, maxInterations, tolerance).InitialBearing;
        }

        public static IAngle BearingFrom(this IGeographicCoordinate pointA, IGeographicCoordinate pointB, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            return VincentysInverseFormula(pointA, pointB, maxInterations, tolerance).FinalBearing;
        }

        private static IBearingDistance VincentysInverseFormula(IGeographicCoordinate pointA, IGeographicCoordinate pointB, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            PrepareLambdaConvergenceLoop(pointA, pointB);
            
            RecalculateLambaUntilConvergence(maxInterations, tolerance);

            CalculateGeodesicDistanceAzimuth();

            IBearingDistance bearingDistance = new BearingDistance()
            {
                Distance = new Distance(V.GeodesicLength, DistanceMeasurement.Meters),
                InitialBearing = new Angle(V.ForwardAzimuth, AngleMeasurement.Radians),
                FinalBearing = new Angle(V.BackwardAzimuth, AngleMeasurement.Radians)
            };

            return bearingDistance;
        }

        private static void PrepareLambdaConvergenceLoop(IGeographicCoordinate pointA, IGeographicCoordinate pointB)
        {
            V = new VincentyFormulaVariables
            {
                ReducedLatitudePointA = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointA.Latitude.Angle.ToRadians())),
                ReducedLatitudePointB = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointB.Latitude.Angle.ToRadians())),

                LongitudeDifference = Angle.ToRadians(pointB.Longitude.Angle.ToDegrees() - pointA.Longitude.Angle.ToDegrees())
            };

            V.Lambda = V.LongitudeDifference;

            V.SinReducedLatitudePointA = Math.Sin(V.ReducedLatitudePointA);
            V.CosReducedLatitudePointA = Math.Cos(V.ReducedLatitudePointA);
            V.SinReducedLatitudePointB = Math.Sin(V.ReducedLatitudePointB);
            V.CosReducedLatitudePointB = Math.Cos(V.ReducedLatitudePointB);
        }

        private static void RecalculateLambaUntilConvergence(int maxInterations, double tolerance)
        {
            for (var i = 0; i < maxInterations; i++)
            {
                double Lambda_prev = V.Lambda;
                V.Lambda = RecalculateLambda();

                var diff = Math.Abs(Lambda_prev - V.Lambda);
                if (diff <= tolerance)
                    break;
            }
        }

        private static double RecalculateLambda()
        {
            var cos_lambda = Math.Cos(V.Lambda);
            var sin_lambda = Math.Sin(V.Lambda);

            V.SinSigma = Math.Sqrt(Math.Pow(V.CosReducedLatitudePointB * Math.Sin(V.Lambda), 2.0) +
                                   Math.Pow(V.CosReducedLatitudePointA * V.SinReducedLatitudePointB -
                                   V.SinReducedLatitudePointA * V.CosReducedLatitudePointB * cos_lambda, 2.0));

            V.CosSigma = V.SinReducedLatitudePointA * V.SinReducedLatitudePointB +
                         V.CosReducedLatitudePointA * V.CosReducedLatitudePointB * cos_lambda;

            V.Sigma = Math.Atan2(V.SinSigma, V.CosSigma);

            var sin_alpha = (V.CosReducedLatitudePointA * V.CosReducedLatitudePointB * sin_lambda) / V.SinSigma;

            V.CosSquaredAlpha = 1 - Math.Pow(sin_alpha, 2.0);

            V.Cos2SigmaM = V.CosSigma - ((2.0 * V.SinReducedLatitudePointA * V.SinReducedLatitudePointB) / V.CosSquaredAlpha);

            var C = (ellipsoidFlattening / 16.0) * V.CosSquaredAlpha * (4 + ellipsoidFlattening * (4.0 - 3.0 * V.CosSquaredAlpha));

            return V.LongitudeDifference + (1.0 - C) * ellipsoidFlattening * sin_alpha *
                                                  (V.Sigma + C * V.SinSigma * (V.Cos2SigmaM + C * V.CosSigma *
                                                                               (-1.0 + 2.0 * Math.Pow(V.Cos2SigmaM, 2.0))));
        }

        private static void CalculateGeodesicDistanceAzimuth()
        {
            V.USquared = V.CosSquaredAlpha * ((Math.Pow(semiMajorAxis, 2.0) - Math.Pow(semiMinorAxis, 2.0)) / Math.Pow(semiMinorAxis, 2.0));
            V.A = 1 + (V.USquared / 16384.0) * (4096.0 + V.USquared * (-768.0 + V.USquared * (320.0 - 175.0 * V.USquared)));
            V.B = (V.USquared / 1024.0) * (256.0 + V.USquared * (-128.0 + V.USquared * (74.0 - 47.0 * V.USquared)));
            
            V.DeltaSigma = V.B * V.SinSigma * (V.Cos2SigmaM + 0.25 * V.B * (V.CosSigma * (-1.0 + 2.0 * Math.Pow(V.Cos2SigmaM, 2.0)) -
                                               (1.0 / 6.0) * V.B * V.Cos2SigmaM * (-3.0 + 4.0 * Math.Pow(V.SinSigma, 2.0)) * 
                                               (-3.0 + 4.0 * Math.Pow(V.Cos2SigmaM, 2.0))));

            V.GeodesicLength = semiMinorAxis * V.A * (V.Sigma - V.DeltaSigma);
            V.ForwardAzimuth = CalculateForwardAzimuth();
            V.BackwardAzimuth = CalcaulateBackwardAzimuth();
        }

        private static double CalculateForwardAzimuth()
        {
            var dividend = Math.Cos(V.ReducedLatitudePointB) * Math.Sin(V.Lambda);
            var divisor = Math.Cos(V.ReducedLatitudePointA) * Math.Sin(V.ReducedLatitudePointB) - 
                          Math.Sin(V.ReducedLatitudePointA) * Math.Cos(V.ReducedLatitudePointB) * Math.Cos(V.Lambda);
            
            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth;
        }

        private static double CalcaulateBackwardAzimuth()
        {
            var dividend = Math.Cos(V.ReducedLatitudePointA) * Math.Sin(V.Lambda);
            var divisor = Math.Cos(V.ReducedLatitudePointA) * Math.Sin(V.ReducedLatitudePointB) * Math.Cos(V.Lambda) - 
                          Math.Sin(V.ReducedLatitudePointA) * Math.Cos(V.ReducedLatitudePointB);

            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth >= 0 ? azimuth - Math.PI : azimuth + Math.PI;
        }
    }
}
