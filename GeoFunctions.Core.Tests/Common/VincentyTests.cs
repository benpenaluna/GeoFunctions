using GeoFunctions.Core.Common;
using GeoFunctions.Core.Coordinates;
using GeoFunctions.Core.Coordinates.Measurement;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GeoFunctions.Core.Tests.Common
{
    public class VincentyTests
    {
        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 714102.60631513281, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 5853100.327933725, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 18913621.974381331, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesGreatCircleDistance(double latA, double lonA, double latB, double lonB, double distanceinMeters, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IDistance expected = new Distance(distanceinMeters, DistanceMeasurement.Meters);

            IDistance result = pointA.GreatCircleDistanceTo(pointB, maxIterations, tolerance);

            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 714102.60631513281, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 5853100.327933725, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 18913621.974381331, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesGreatCircleDistanceConcurrently(double latA, double lonA, double latB, double lonB, double distanceinMeters, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IDistance expected = new Distance(distanceinMeters, DistanceMeasurement.Meters);

            var distances = new ConcurrentQueue<IDistance>() { };
            void action() => distances.Enqueue(pointA.GreatCircleDistanceTo(pointB, maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            Parallel.Invoke(actions);

            while (distances.Count > 0)
            {
                distances.TryPeek(out IDistance result);

                Assert.Equal(expected, result);

                distances.TryDequeue(out result);
            }
        }

        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 0.94019000526431273, 200, 1.0E-12)]
        [InlineData(-33.85678333, 151.2152972, -37.81996667, 144.98345, 0.87642759894760014 + System.Math.PI, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 0.93747586784865922, 200, 1.0E-12)]
        [InlineData(48.85836944, 2.294480556, 40.68925, -74.0445, 5.0926982336331088, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 0.91781936198368552, 200, 1.0E-12)]
        [InlineData(-45.03016389, 168.6616139, 51.50329722, -0.119552778, 5.5080130507549114, 200, 1.0E-12)]
        [InlineData(-10.68711389, 142.5314139, -25.34442778, 131.0368833, 3.756817578462325, 200, 1.0E-12)]
        [InlineData(-25.34442778, 131.0368833, -10.68711389, 142.5314139, 0.6779759461321444, 200, 1.0E-12)]
        [InlineData(60.17019722, 24.93927222, 52.26813333, -113.8112389, 5.812472888517858, 200, 1.0E-12)]
        [InlineData(52.26813333, -113.8112389, 60.17019722, 24.93927222, 0.37771058163667104, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingTo(double latA, double lonA, double latB, double lonB, double bearingInRadians, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IAngle expected = new Angle(bearingInRadians, AngleMeasurement.Radians);

            IAngle result = pointA.BearingTo(pointB, maxIterations, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 0.94019000526431273, 200, 1.0E-12)]
        [InlineData(-33.85678333, 151.2152972, -37.81996667, 144.98345, 0.87642759894760014 + System.Math.PI, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 0.93747586784865922, 200, 1.0E-12)]
        [InlineData(48.85836944, 2.294480556, 40.68925, -74.0445, 5.0926982336331088, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 0.91781936198368552, 200, 1.0E-12)]
        [InlineData(-45.03016389, 168.6616139, 51.50329722, -0.119552778, 5.5080130507549114, 200, 1.0E-12)]
        [InlineData(-10.68711389, 142.5314139, -25.34442778, 131.0368833, 3.756817578462325, 200, 1.0E-12)]
        [InlineData(-25.34442778, 131.0368833, -10.68711389, 142.5314139, 0.6779759461321444, 200, 1.0E-12)]
        [InlineData(60.17019722, 24.93927222, 52.26813333, -113.8112389, 5.812472888517858, 200, 1.0E-12)]
        [InlineData(52.26813333, -113.8112389, 60.17019722, 24.93927222, 0.37771058163667104, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingToConcurrently(double latA, double lonA, double latB, double lonB, double bearingInRadians, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IAngle expected = new Angle(bearingInRadians, AngleMeasurement.Radians);

            var angles = new ConcurrentQueue<IAngle>() { };
            void action() => angles.Enqueue(pointA.BearingTo(pointB, maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            Parallel.Invoke(actions);

            while (angles.Count > 0)
            {
                angles.TryPeek(out IAngle result);

                Assert.Equal(expected, result);

                angles.TryDequeue(out result);
            }
        }

        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 0.87642759894760014 + System.Math.PI, 200, 1.0E-12)]
        [InlineData(-33.85678333, 151.2152972, -37.81996667, 144.98345, 0.9401900052643124, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 5.0926982336331088, 200, 1.0E-12)]
        [InlineData(48.85836944, 2.294480556, 40.68925, -74.0445, 0.93747586784865922, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 5.5080130507549114, 200, 1.0E-12)]
        [InlineData(-45.03016389, 168.6616139, 51.50329722, -0.119552778, 0.91781936198368541, 200, 1.0E-12)]
        [InlineData(-10.68711389, 142.5314139, -25.34442778, 131.0368833, 0.67797594613214418, 200, 1.0E-12)]
        [InlineData(-25.34442778, 131.0368833, -10.68711389, 142.5314139, 3.756817578462325, 200, 1.0E-12)]
        [InlineData(60.17019722, 24.93927222, 52.26813333, -113.8112389, 0.37771058163667082, 200, 1.0E-12)]
        [InlineData(52.26813333, -113.8112389, 60.17019722, 24.93927222, 5.812472888517858, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingFrom(double latA, double lonA, double latB, double lonB, double bearingInRadians, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IAngle expected = new Angle(bearingInRadians, AngleMeasurement.Radians);

            IAngle result = pointA.BearingFrom(pointB, maxIterations, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.81996667, 144.98345, -33.85678333, 151.2152972, 0.87642759894760014 + System.Math.PI, 200, 1.0E-12)]
        [InlineData(-33.85678333, 151.2152972, -37.81996667, 144.98345, 0.9401900052643124, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 48.85836944, 2.294480556, 5.0926982336331088, 200, 1.0E-12)]
        [InlineData(48.85836944, 2.294480556, 40.68925, -74.0445, 0.93747586784865922, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, -45.03016389, 168.6616139, 5.5080130507549114, 200, 1.0E-12)]
        [InlineData(-45.03016389, 168.6616139, 51.50329722, -0.119552778, 0.91781936198368541, 200, 1.0E-12)]
        [InlineData(-10.68711389, 142.5314139, -25.34442778, 131.0368833, 0.67797594613214418, 200, 1.0E-12)]
        [InlineData(-25.34442778, 131.0368833, -10.68711389, 142.5314139, 3.756817578462325, 200, 1.0E-12)]
        [InlineData(60.17019722, 24.93927222, 52.26813333, -113.8112389, 0.37771058163667082, 200, 1.0E-12)]
        [InlineData(52.26813333, -113.8112389, 60.17019722, 24.93927222, 5.812472888517858, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingFromConcurrently(double latA, double lonA, double latB, double lonB, double bearingInRadians, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IGeographicCoordinate pointB = new GeographicCoordinate(latB, lonB);

            IAngle expected = new Angle(bearingInRadians, AngleMeasurement.Radians);

            var angles = new ConcurrentQueue<IAngle>() { };
            void action() => angles.Enqueue(pointA.BearingFrom(pointB, maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            Parallel.Invoke(actions);

            while (angles.Count > 0)
            {
                angles.TryPeek(out IAngle result);

                Assert.Equal(expected, result);

                angles.TryDequeue(out result);
            }
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -37.652821149141957, 143.92649553423738, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, -33.856783329998891, 151.2152972000016, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 48.858369440010456, 2.2944805559602606, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, -45.03016389001283, 168.66161390001733, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesDestinationCoordinates(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                       double expectedLat, double expectedLon, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);

            IGeographicCoordinate expected = new GeographicCoordinate(expectedLat, expectedLon);

            IGeographicCoordinate result = pointA.DestinationCoordinates(new Angle(bearingInRadians, AngleMeasurement.Radians), new Distance(distanceInMeters, DistanceMeasurement.Meters),
                                                                         maxIterations, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -37.652821149141957, 143.92649553423738, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, -33.856783329998891, 151.2152972000016, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 48.858369440010456, 2.2944805559602606, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, -45.03016389001283, 168.66161390001733, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesDestinationCoordinatesConcurrently(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                                   double expectedLat, double expectedLon, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);

            IGeographicCoordinate expected = new GeographicCoordinate(expectedLat, expectedLon);

            var coordinates = new ConcurrentQueue<IGeographicCoordinate>() { };
            void action() => coordinates.Enqueue(pointA.DestinationCoordinates(new Angle(bearingInRadians, AngleMeasurement.Radians), new Distance(distanceInMeters, DistanceMeasurement.Meters),
                                                                         maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            while (coordinates.Count > 0)
            {
                coordinates.TryPeek(out IGeographicCoordinate result);

                Assert.Equal(expected, result);

                coordinates.TryDequeue(out result);
            }
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -0.92199409297205726 + 2.0 * Math.PI, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, 0.87642759894758449, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 1.9511055800427943, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, 2.3664203971648994, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesFinalBearing(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                                double expectedBearing, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IAngle initialBearing = new Angle(bearingInRadians, AngleMeasurement.Radians);
            IDistance distance = new Distance(distanceInMeters, DistanceMeasurement.Meters);

            IAngle expected = new Angle(expectedBearing, AngleMeasurement.Radians);

            IAngle result = pointA.FinalBearing(initialBearing, distance, maxIterations, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -0.92199409297205726 + 2.0 * Math.PI, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, 0.87642759894758449, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 1.9511055800427943, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, 2.3664203971648994, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesFinalBearingConcurrently(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                                double expectedBearing, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IAngle initialBearing = new Angle(bearingInRadians, AngleMeasurement.Radians);
            IDistance distance = new Distance(distanceInMeters, DistanceMeasurement.Meters);

            IAngle expected = new Angle(expectedBearing, AngleMeasurement.Radians);

            var angles = new ConcurrentQueue<IAngle>() { };
            void action() => angles.Enqueue(pointA.FinalBearing(initialBearing, distance, maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            while (angles.Count > 0)
            {
                angles.TryPeek(out IAngle result);

                Assert.Equal(expected, result);

                angles.TryDequeue(out result);
            }
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -0.92199409297205726 + Math.PI, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, 0.87642759894758449 + Math.PI, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 1.9511055800427943 + Math.PI, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, 2.3664203971648994 + Math.PI, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingFromPointBearingDistance(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                                double expectedBearing, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IAngle initialBearing = new Angle(bearingInRadians, AngleMeasurement.Radians);
            IDistance distance = new Distance(distanceInMeters, DistanceMeasurement.Meters);

            IAngle expected = new Angle(expectedBearing, AngleMeasurement.Radians);

            IAngle result = pointA.BearingFrom(initialBearing, distance, maxIterations, tolerance);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-37.95103342, 144.4248679, 5.355859732, 54972.271, -0.92199409297205726 + Math.PI, 200, 1.0E-12)]
        [InlineData(-37.81996667, 144.98345, 0.94019000526431273, 714102.60631513281, 0.87642759894758449 + Math.PI, 200, 1.0E-12)]
        [InlineData(40.68925, -74.0445, 0.93747586784865922, 5853100.327933725, 1.9511055800427943 + Math.PI, 200, 1.0E-12)]
        [InlineData(51.50329722, -0.119552778, 0.91781936198368552, 18913621.974381331, 2.3664203971648994 + Math.PI, 200, 1.0E-12)]
        public void Vincenty_CorrectlyCalculatesBearingFromPointBearingDistanceConcurrently(double latA, double lonA, double bearingInRadians, double distanceInMeters,
                                                                                            double expectedBearing, int maxIterations, double tolerance)
        {
            IGeographicCoordinate pointA = new GeographicCoordinate(latA, lonA);
            IAngle initialBearing = new Angle(bearingInRadians, AngleMeasurement.Radians);
            IDistance distance = new Distance(distanceInMeters, DistanceMeasurement.Meters);

            IAngle expected = new Angle(expectedBearing, AngleMeasurement.Radians);

            var angles = new ConcurrentQueue<IAngle>() { };
            void action() => angles.Enqueue(pointA.BearingFrom(initialBearing, distance, maxIterations, tolerance));
            Action[] actions = Enumerable.Repeat((Action)action, 10).ToArray();

            while (angles.Count > 0)
            {
                angles.TryPeek(out IAngle result);

                Assert.Equal(expected, result);

                angles.TryDequeue(out result);
            }
        }
    }
}
