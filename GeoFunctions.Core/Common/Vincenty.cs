using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using System;

namespace GeoFunctions.Core.Common
{
    public static class Vincenty
    {
        private const double a = 6378137.0;
        private const double f = 1.0 / 298.257223563;
        private static double b = (1.0 - f) * a;

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
            // Source: https://nathanrooy.github.io/posts/2016-12-18/vincenty-formula-with-python/

            VincentyFormulaVariables v = PrepareλConvergenceLoop(pointA, pointB);

            v = RecalculateλUntilConvergence(v, maxInterations, tolerance);

            v = CalculateGeodesicDistanceAzimuth(v);

            IBearingDistance bearingDistance = new BearingDistance()
            {
                Distance = new Distance(v.GeodesicLength, DistanceMeasurement.Meters),
                InitialBearing = new Angle(v.ForwardAzimuth, AngleMeasurement.Radians),
                FinalBearing = new Angle(v.BackwardAzimuth, AngleMeasurement.Radians)
            };

            return bearingDistance;
        }

        private static VincentyFormulaVariables PrepareλConvergenceLoop(IGeographicCoordinate pointA, IGeographicCoordinate pointB)
        {
            var v = new VincentyFormulaVariables
            {
                U1 = Math.Atan((1.0 - f) * Math.Tan(pointA.Latitude.Angle.ToRadians())),
                U2 = Math.Atan((1.0 - f) * Math.Tan(pointB.Latitude.Angle.ToRadians())),

                L = Angle.ToRadians(pointB.Longitude.Angle.ToDegrees() - pointA.Longitude.Angle.ToDegrees())
            };

            v.λ = v.L;

            v.SinU1 = Math.Sin(v.U1);
            v.CosU1 = Math.Cos(v.U1);
            v.SinU2 = Math.Sin(v.U2);
            v.CosU2 = Math.Cos(v.U2);

            return v;
        }

        private static VincentyFormulaVariables RecalculateλUntilConvergence(VincentyFormulaVariables v, int maxInterations, double tolerance)
        {
            for (var i = 0; i < maxInterations; i++)
            {
                double Lambda_prev = v.λ;
                v = Recalculateλ(v);

                var diff = Math.Abs(Lambda_prev - v.λ);
                if (diff <= tolerance)
                    break;
            }

            return v;
        }

        private static VincentyFormulaVariables Recalculateλ(VincentyFormulaVariables v)
        {
            var cosλ = Math.Cos(v.λ);
            var sinλ = Math.Sin(v.λ);

            v.Sinσ = Math.Sqrt(Math.Pow(v.CosU2 * Math.Sin(v.λ), 2.0) +
                                   Math.Pow(v.CosU1 * v.SinU2 -
                                   v.SinU1 * v.CosU2 * cosλ, 2.0));

            v.Cosσ = v.SinU1 * v.SinU2 +
                         v.CosU1 * v.CosU2 * cosλ;

            v.σ = Math.Atan2(v.Sinσ, v.Cosσ);

            v.sinα = (v.CosU1 * v.CosU2 * sinλ) / v.Sinσ;

            v.CosSqα = 1 - Math.Pow(v.sinα, 2.0);

            v.Cos2σM = v.Cosσ - ((2.0 * v.SinU1 * v.SinU2) / v.CosSqα);

            v.C = (f / 16.0) * v.CosSqα * (4 + f * (4.0 - 3.0 * v.CosSqα));

            v.λ = v.L + (1.0 - v.C) * f * v.sinα * (v.σ + v.C * v.Sinσ * (v.Cos2σM + v.C * v.Cosσ * (-1.0 + 2.0 * Math.Pow(v.Cos2σM, 2.0))));

            return v;
        }

        private static VincentyFormulaVariables CalculateGeodesicDistanceAzimuth(VincentyFormulaVariables v)
        {
            v.USquared = v.CosSqα * ((Math.Pow(a, 2.0) - Math.Pow(b, 2.0)) / Math.Pow(b, 2.0));
            v.A = 1 + (v.USquared / 16384.0) * (4096.0 + v.USquared * (-768.0 + v.USquared * (320.0 - 175.0 * v.USquared)));
            v.B = (v.USquared / 1024.0) * (256.0 + v.USquared * (-128.0 + v.USquared * (74.0 - 47.0 * v.USquared)));

            v.Δσ = v.B * v.Sinσ * (v.Cos2σM + 0.25 * v.B * (v.Cosσ * (-1.0 + 2.0 * Math.Pow(v.Cos2σM, 2.0)) -
                                               (1.0 / 6.0) * v.B * v.Cos2σM * (-3.0 + 4.0 * Math.Pow(v.Sinσ, 2.0)) *
                                               (-3.0 + 4.0 * Math.Pow(v.Cos2σM, 2.0))));

            v.GeodesicLength = b * v.A * (v.σ - v.Δσ);
            v.ForwardAzimuth = CalculateForwardAzimuth(v);
            v.BackwardAzimuth = CalcaulateBackwardAzimuthInverseMethod(v);

            return v;
        }

        private static double CalculateForwardAzimuth(VincentyFormulaVariables v)
        {
            var dividend = Math.Cos(v.U2) * Math.Sin(v.λ);
            var divisor = Math.Cos(v.U1) * Math.Sin(v.U2) -
                          Math.Sin(v.U1) * Math.Cos(v.U2) * Math.Cos(v.λ);

            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth;
        }

        private static double CalcaulateBackwardAzimuthInverseMethod(VincentyFormulaVariables v)
        {
            var dividend = Math.Cos(v.U1) * Math.Sin(v.λ);
            var divisor = Math.Cos(v.U1) * Math.Sin(v.U2) * Math.Cos(v.λ) -
                          Math.Sin(v.U1) * Math.Cos(v.U2);

            var azimuth = Math.Atan2(dividend, divisor);
            return azimuth >= 0 ? azimuth - Math.PI : azimuth + Math.PI;
        }

        public static IGeographicCoordinate DestinationCoordinates(this IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance,
                                                                   int maxInterations = 200, double tolerance = 1.0E-12)
        {
            return VincentysDirectFormula(pointA, initialBearing, distance, maxInterations, tolerance).DestinationCoordinates;
        }

        public static IAngle FinalBearing(this IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            var finalBearing = VincentysDirectFormula(pointA, initialBearing, distance, maxInterations, tolerance).FinalBearing;

            return new Angle(finalBearing, AngleMeasurement.Radians);
        }

        public static IAngle BearingFrom(this IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance, int maxInterations = 200, double tolerance = 1.0E-12)
        {
            var backwardAzimuth = VincentysDirectFormula(pointA, initialBearing, distance, maxInterations, tolerance).BackwardAzimuth;

            return new Angle(backwardAzimuth, AngleMeasurement.Radians);
        }

        private static VincentyFormulaVariables VincentysDirectFormula(IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance,
                                                                    int maxInterations = 200, double tolerance = 1.0E-12)
        {
            // Source: https://www.movable-type.co.uk/scripts/latlong-vincenty.html

            VincentyFormulaVariables v = PrepareσConvergenceLoop(pointA, initialBearing, distance);

            v = RecalculateσUntilConvergence(distance, maxInterations, tolerance, v);

            v.DestinationCoordinates = CalculateDestinationCoordinates(pointA, v);
            v.FinalBearing = Math.Atan2(v.sinα, -1.0 * (v.SinU1 * v.Sinσ - v.CosU1 * v.Cosσ * v.Cosα1));

            return v;
        }

        private static VincentyFormulaVariables PrepareσConvergenceLoop(IGeographicCoordinate pointA, IAngle initialBearing, IDistance distance)
        {
            VincentyFormulaVariables v = new VincentyFormulaVariables();

            v.Sinα1 = Math.Sin(initialBearing.ToRadians());
            v.Cosα1 = Math.Cos(initialBearing.ToRadians());

            v.TanU1 = (1.0 - f) * Math.Tan(pointA.Latitude.Angle.ToRadians());
            v.CosU1 = 1.0 / Math.Sqrt((1 + Math.Pow(v.TanU1, 2.0)));
            v.SinU1 = v.TanU1 * v.CosU1;

            v.σ1 = Math.Atan2(v.TanU1, v.Cosα1);
            v.sinα = v.CosU1 * v.Sinα1;
            v.CosSqα = 1.0 - Math.Pow(v.sinα, 2.0);
            v.USquared = v.CosSqα * (Math.Pow(a, 2.0) - Math.Pow(b, 2.0)) / Math.Pow(b, 2.0);
            v.A = 1 + v.USquared / 16384.0 * (4096.0 + v.USquared * (-768.0 + v.USquared * (320.0 - 175.0 * v.USquared)));
            v.B = v.USquared / 1024.0 * (256.0 + v.USquared * (-128.0 + v.USquared * (74.0 - 47.0 * v.USquared)));

            v.σ = distance.ToMeters() / (b * v.A);

            return v;
        }

        private static VincentyFormulaVariables RecalculateσUntilConvergence(IDistance distance, int maxInterations, double tolerance, VincentyFormulaVariables v)
        {
            for (var i = 0; i < maxInterations; i++)
            {
                v.Cos2σM = Math.Cos(2 * v.σ1 + v.σ);
                v.Sinσ = Math.Sin(v.σ);
                v.Cosσ = Math.Cos(v.σ);
                v.Δσ = v.B * v.Sinσ * (v.Cos2σM + v.B / 4.0 * (v.Cosσ * (-1.0 + 2.0 * Math.Pow(v.Cos2σM, 2.0)) -
                       v.B / 6.0 * v.Cos2σM * (-3.0 + 4.0 * Math.Pow(v.Sinσ, 2.0)) * (-3.0 + 4.0 * Math.Pow(v.Cos2σM, 2.0))));

                v.σPrevious = v.σ;
                v.σ = distance.ToMeters() / (b * v.A) + v.Δσ;

                var diff = Math.Abs(v.σ - v.σPrevious);
                if (diff <= tolerance)
                    break;
            }

            return v;
        }

        private static IGeographicCoordinate CalculateDestinationCoordinates(IGeographicCoordinate pointA, VincentyFormulaVariables v)
        {
            var tmp = v.SinU1 * v.Sinσ - v.CosU1 * v.Cosσ * v.Cosα1;
            v.φ2 = Math.Atan2(v.SinU1 * v.Cosσ + v.CosU1 * v.Sinσ * v.Cosα1, (1 - f) * Math.Sqrt(Math.Pow(v.sinα, 2.0) + Math.Pow(tmp, 2.0)));
            v.λ = Math.Atan2(v.Sinσ * v.Sinα1, v.CosU1 * v.Cosσ - v.SinU1 * v.Sinσ * v.Cosα1);
            v.C = f / 16.0 * v.CosSqα * (4.0 + f * (4.0 - 3.0 * v.CosSqα));
            v.L = v.λ - (1.0 - v.C) * f * v.sinα *
                    (v.σ + v.C * v.Sinσ * (v.Cos2σM + v.C * v.Cosσ * (-1.0 + 2.0 * Math.Pow(v.Cos2σM, 2.0))));
            v.λ2 = (pointA.Longitude.Angle.ToRadians() + v.L + 3.0 * Math.PI) % (2.0 * Math.PI) - Math.PI;  // normalise to -180...+180

            var latitude = Angle.ToDegrees(v.φ2);
            var longitude = Angle.ToDegrees(v.λ2);
            return new GeographicCoordinate(latitude, longitude);
        }
    }
}
