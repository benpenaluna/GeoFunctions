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
            VincentyFormulaVariables v = PrepareLambdaConvergenceLoop(pointA, pointB);
            
            v = RecalculateLambaUntilConvergence(v, maxInterations, tolerance);

            v = CalculateGeodesicDistanceAzimuth(v);

            IBearingDistance bearingDistance = new BearingDistance()
            {
                Distance = new Distance(v.GeodesicLength, DistanceMeasurement.Meters),
                InitialBearing = new Angle(v.ForwardAzimuth, AngleMeasurement.Radians),
                FinalBearing = new Angle(v.BackwardAzimuth, AngleMeasurement.Radians)
            };

            return bearingDistance;
        }

        private static VincentyFormulaVariables PrepareLambdaConvergenceLoop(IGeographicCoordinate pointA, IGeographicCoordinate pointB)
        {
            var v = new VincentyFormulaVariables
            {
                ReducedLatitudePointA = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointA.Latitude.Angle.ToRadians())),
                ReducedLatitudePointB = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointB.Latitude.Angle.ToRadians())),

                LongitudeDifference = Angle.ToRadians(pointB.Longitude.Angle.ToDegrees() - pointA.Longitude.Angle.ToDegrees())
            };

            v.Lambda = v.LongitudeDifference;

            v.SinReducedLatitudePointA = Math.Sin(v.ReducedLatitudePointA);
            v.CosReducedLatitudePointA = Math.Cos(v.ReducedLatitudePointA);
            v.SinReducedLatitudePointB = Math.Sin(v.ReducedLatitudePointB);
            v.CosReducedLatitudePointB = Math.Cos(v.ReducedLatitudePointB);

            return v;
        }

        private static VincentyFormulaVariables RecalculateLambaUntilConvergence(VincentyFormulaVariables v, int maxInterations, double tolerance)
        {
            for (var i = 0; i < maxInterations; i++)
            {
                double Lambda_prev = v.Lambda;
                v = RecalculateLambda(v);

                var diff = Math.Abs(Lambda_prev - v.Lambda);
                if (diff <= tolerance)
                    break;
            }

            return v;
        }

        private static VincentyFormulaVariables RecalculateLambda(VincentyFormulaVariables v)
        {
            var cos_lambda = Math.Cos(v.Lambda);
            var sin_lambda = Math.Sin(v.Lambda);

            v.SinSigma = Math.Sqrt(Math.Pow(v.CosReducedLatitudePointB * Math.Sin(v.Lambda), 2.0) +
                                   Math.Pow(v.CosReducedLatitudePointA * v.SinReducedLatitudePointB -
                                   v.SinReducedLatitudePointA * v.CosReducedLatitudePointB * cos_lambda, 2.0));

            v.CosSigma = v.SinReducedLatitudePointA * v.SinReducedLatitudePointB +
                         v.CosReducedLatitudePointA * v.CosReducedLatitudePointB * cos_lambda;

            v.Sigma = Math.Atan2(v.SinSigma, v.CosSigma);

            var sin_alpha = (v.CosReducedLatitudePointA * v.CosReducedLatitudePointB * sin_lambda) / v.SinSigma;

            v.CosSquaredAlpha = 1 - Math.Pow(sin_alpha, 2.0);

            v.Cos2SigmaM = v.CosSigma - ((2.0 * v.SinReducedLatitudePointA * v.SinReducedLatitudePointB) / v.CosSquaredAlpha);

            var C = (ellipsoidFlattening / 16.0) * v.CosSquaredAlpha * (4 + ellipsoidFlattening * (4.0 - 3.0 * v.CosSquaredAlpha));

            v.Lambda = v.LongitudeDifference + (1.0 - C) * ellipsoidFlattening * sin_alpha *
                                                  (v.Sigma + C * v.SinSigma * (v.Cos2SigmaM + C * v.CosSigma *
                                                                               (-1.0 + 2.0 * Math.Pow(v.Cos2SigmaM, 2.0))));

            return v;
        }

        private static VincentyFormulaVariables CalculateGeodesicDistanceAzimuth(VincentyFormulaVariables v)
        {
            v.USquared = v.CosSquaredAlpha * ((Math.Pow(semiMajorAxis, 2.0) - Math.Pow(semiMinorAxis, 2.0)) / Math.Pow(semiMinorAxis, 2.0));
            v.A = 1 + (v.USquared / 16384.0) * (4096.0 + v.USquared * (-768.0 + v.USquared * (320.0 - 175.0 * v.USquared)));
            v.B = (v.USquared / 1024.0) * (256.0 + v.USquared * (-128.0 + v.USquared * (74.0 - 47.0 * v.USquared)));
            
            v.DeltaSigma = v.B * v.SinSigma * (v.Cos2SigmaM + 0.25 * v.B * (v.CosSigma * (-1.0 + 2.0 * Math.Pow(v.Cos2SigmaM, 2.0)) -
                                               (1.0 / 6.0) * v.B * v.Cos2SigmaM * (-3.0 + 4.0 * Math.Pow(v.SinSigma, 2.0)) * 
                                               (-3.0 + 4.0 * Math.Pow(v.Cos2SigmaM, 2.0))));

            v.GeodesicLength = semiMinorAxis * v.A * (v.Sigma - v.DeltaSigma);
            v.ForwardAzimuth = CalculateForwardAzimuth(v);
            v.BackwardAzimuth = CalcaulateBackwardAzimuth(v);

            return v;
        }

        private static double CalculateForwardAzimuth(VincentyFormulaVariables v)
        {
            var dividend = Math.Cos(v.ReducedLatitudePointB) * Math.Sin(v.Lambda);
            var divisor = Math.Cos(v.ReducedLatitudePointA) * Math.Sin(v.ReducedLatitudePointB) - 
                          Math.Sin(v.ReducedLatitudePointA) * Math.Cos(v.ReducedLatitudePointB) * Math.Cos(v.Lambda);
            
            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth;
        }

        private static double CalcaulateBackwardAzimuth(VincentyFormulaVariables v)
        {
            var dividend = Math.Cos(v.ReducedLatitudePointA) * Math.Sin(v.Lambda);
            var divisor = Math.Cos(v.ReducedLatitudePointA) * Math.Sin(v.ReducedLatitudePointB) * Math.Cos(v.Lambda) - 
                          Math.Sin(v.ReducedLatitudePointA) * Math.Cos(v.ReducedLatitudePointB);

            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth >= 0 ? azimuth - Math.PI : azimuth + Math.PI;
        }

        public static IGeographicCoordinate DestinationCoordinates(this IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance, 
                                                                   int maxInterations = 200, double tolerance = 1.0E-12)
        {
            return VincentysDirectFormula(pointA, initialBearing, distance, maxInterations, tolerance);
        }

        private static IGeographicCoordinate VincentysDirectFormula(IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance, 
                                                                    int maxInterations = 200, double tolerance = 1.0E-12)
        {
            var sinα1 = Math.Sin(initialBearing.ToRadians());
            var cosα1 = Math.Cos(initialBearing.ToRadians());

            var tanU1 = (1.0 - ellipsoidFlattening) * Math.Tan(pointA.Latitude.Angle.ToRadians());
            var cosU1 = 1.0 / Math.Sqrt((1 + Math.Pow(tanU1, 2.0)));
            var sinU1 = tanU1 * cosU1;

            var σ1 = Math.Atan2(tanU1, cosα1);
            var sinα = cosU1 * sinα1;
            var cosSqα = 1.0 - Math.Pow(sinα, 2.0);
            var uSq = cosSqα * (Math.Pow(semiMajorAxis, 2.0) - Math.Pow(semiMinorAxis, 2.0)) / Math.Pow(semiMinorAxis, 2.0);
            var A = 1 + uSq / 16384.0 * (4096.0 + uSq * (-768.0 + uSq * (320.0 - 175.0 * uSq)));
            var B = uSq / 1024.0 * (256.0 + uSq * (-128.0 + uSq * (74.0 - 47.0 * uSq)));

            
            
            var σ = distance.ToMeters() / (semiMinorAxis * A);
            double cos2σM = 0.0;
            double sinσ = 0.0;
            double cosσ = 0.0;
            for (var i = 0; i < maxInterations; i++)
            {
                cos2σM = Math.Cos(2 * σ1 + σ);
                sinσ = Math.Sin(σ);
                cosσ = Math.Cos(σ);
                var Δσ = B * sinσ * (cos2σM + B / 4.0 * (cosσ * (-1.0 + 2.0 * Math.Pow(cos2σM, 2.0)) -
                         B / 6.0 * cos2σM * (-3.0 + 4.0 * Math.Pow(sinσ, 2.0)) * (-3.0 + 4.0 * Math.Pow(cos2σM, 2.0))));

                double σ_previous = σ;
                σ = distance.ToMeters() / (semiMinorAxis * A) + Δσ;

                var diff = Math.Abs(σ - σ_previous);
                if (diff <= tolerance)
                    break;
            }

            var tmp = sinU1 * sinσ - cosU1 * cosσ * cosα1;
            var φ2 = Math.Atan2(sinU1 * cosσ + cosU1 * sinσ * cosα1, (1 - ellipsoidFlattening) * Math.Sqrt(Math.Pow(sinα, 2.0) + Math.Pow(tmp, 2.0)));
            var λ = Math.Atan2(sinσ * sinα1, cosU1 * cosσ - sinU1 * sinσ * cosα1);
            var C = ellipsoidFlattening / 16.0 * cosSqα * (4.0 + ellipsoidFlattening * (4.0 - 3.0 * cosSqα));
            var L = λ - (1.0 - C) * ellipsoidFlattening * sinα *
                    (σ + C * sinσ * (cos2σM + C * cosσ * (-1.0 + 2.0 * Math.Pow(cos2σM, 2.0))));
            var λ2 = (pointA.Longitude.Angle.ToRadians() + L + 3.0 * Math.PI) % (2.0 * Math.PI) - Math.PI;  // normalise to -180...+180
            
            var revAz = Math.Atan2(sinα, -tmp);

            var latitude = Angle.ToDegrees(φ2);
            var longitude = Angle.ToDegrees(λ2);
            return new GeographicCoordinate(latitude, longitude); 
        }
    }
}
