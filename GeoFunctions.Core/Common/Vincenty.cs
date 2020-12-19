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
            var reducedLatitude_pointA = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointA.Latitude.Angle.ToRadians()));
            var reducedLatitude_pointB = Math.Atan((1.0 - ellipsoidFlattening) * Math.Tan(pointB.Latitude.Angle.ToRadians()));

            var longitudeDifference = Angle.ToRadians(pointB.Longitude.Angle.ToDegrees() - pointA.Longitude.Angle.ToDegrees());

            var Lambda = longitudeDifference;

            var sin_u1 = Math.Sin(reducedLatitude_pointA);
            var cos_u1 = Math.Cos(reducedLatitude_pointA);
            var sin_u2 = Math.Sin(reducedLatitude_pointB);
            var cos_u2 = Math.Cos(reducedLatitude_pointB);

            double sin_sigma = 0.0;
            double cos_sigma = 0.0;
            double sigma = 0.0;
            double cos_sq_alpha = 0.0;
            double cos2_sigma_m = 0.0;

            for (var i = 0; i < maxInterations; i++)
            {
                double Lambda_prev = Lambda;
                Lambda = RecalculateLambda(longitudeDifference, Lambda, sin_u1, cos_u1, sin_u2, cos_u2, out sin_sigma, out cos_sigma, out sigma, out cos_sq_alpha, out cos2_sigma_m);

                var diff = Math.Abs(Lambda_prev - Lambda);
                if (diff <= tolerance)
                    break;
            }

            var u_sq = cos_sq_alpha * ((Math.Pow(semiMajorAxis, 2.0) - Math.Pow(semiMinorAxis, 2.0)) / Math.Pow(semiMinorAxis, 2.0));
            var A = 1 + (u_sq / 16384.0) * (4096.0 + u_sq * (-768.0 + u_sq * (320.0 - 175.0 * u_sq)));
            var B = (u_sq / 1024.0) * (256.0 + u_sq * (-128.0 + u_sq * (74.0 - 47.0 * u_sq)));
            var delta_sig = B * sin_sigma * (cos2_sigma_m + 0.25 * B * (cos_sigma * (-1.0 + 2.0 * Math.Pow(cos2_sigma_m, 2.0))
                            - (1.0 / 6.0) * B * cos2_sigma_m * (-3.0 + 4.0 * Math.Pow(sin_sigma, 2.0)) * (-3.0 + 4.0 * Math.Pow(cos2_sigma_m, 2.0))));

            var s = semiMinorAxis * A * (sigma - delta_sig);

            var alpha1 = CalculateForwardAzimuth(reducedLatitude_pointA, reducedLatitude_pointB, Lambda);

            var alpha2 = CalcaulateBackwardAzimuth(reducedLatitude_pointA, reducedLatitude_pointB, Lambda);

            IBearingDistance bearingDistance = new BearingDistance()
            {
                Distance = new Distance(s, DistanceMeasurement.Meters),
                InitialBearing = new Angle(alpha1, AngleMeasurement.Radians),
                FinalBearing = new Angle(alpha2, AngleMeasurement.Radians)
            };

            return bearingDistance;
        }

        private static double RecalculateLambda(double longitudeDifference, double Lambda, double sin_u1, double cos_u1, double sin_u2, double cos_u2, out double sin_sigma, out double cos_sigma, out double sigma, out double cos_sq_alpha, out double cos2_sigma_m)
        {
            var cos_lambda = Math.Cos(Lambda);
            var sin_lambda = Math.Sin(Lambda);
            sin_sigma = Math.Sqrt(Math.Pow(cos_u2 * Math.Sin(Lambda), 2.0) + Math.Pow(cos_u1 * sin_u2 - sin_u1 * cos_u2 * cos_lambda, 2.0));
            cos_sigma = sin_u1 * sin_u2 + cos_u1 * cos_u2 * cos_lambda;
            sigma = Math.Atan2(sin_sigma, cos_sigma);
            var sin_alpha = (cos_u1 * cos_u2 * sin_lambda) / sin_sigma;
            cos_sq_alpha = 1 - Math.Pow(sin_alpha, 2.0);
            cos2_sigma_m = cos_sigma - ((2.0 * sin_u1 * sin_u2) / cos_sq_alpha);
            var C = (ellipsoidFlattening / 16.0) * cos_sq_alpha * (4 + ellipsoidFlattening * (4.0 - 3.0 * cos_sq_alpha));

            return longitudeDifference + (1.0 - C) * ellipsoidFlattening * sin_alpha * (sigma + C * sin_sigma * (cos2_sigma_m + C * cos_sigma * (-1.0 + 2.0 * Math.Pow(cos2_sigma_m, 2.0))));
        }

        private static double CalculateForwardAzimuth(double reducedLatitude_pointA, double reducedLatitude_pointB, double Lambda)
        {
            var dividend = Math.Cos(reducedLatitude_pointB) * Math.Sin(Lambda);
            var divisor = Math.Cos(reducedLatitude_pointA) * Math.Sin(reducedLatitude_pointB) - Math.Sin(reducedLatitude_pointA) * Math.Cos(reducedLatitude_pointB) * Math.Cos(Lambda);
            var azimuth = Math.Atan2(dividend, divisor);

            return azimuth;
        }

        private static double CalcaulateBackwardAzimuth(double reducedLatitude_pointA, double reducedLatitude_pointB, double Lambda)
        {
            var dividend = Math.Cos(reducedLatitude_pointA) * Math.Sin(Lambda);
            var divisor = Math.Cos(reducedLatitude_pointA) * Math.Sin(reducedLatitude_pointB) * Math.Cos(Lambda) - Math.Sin(reducedLatitude_pointA) * Math.Cos(reducedLatitude_pointB);
            var azimuth = Math.Atan2(dividend, divisor);

            if (azimuth >= 0)
                return azimuth - Math.PI;

            return azimuth + Math.PI;
        }
    }
}
