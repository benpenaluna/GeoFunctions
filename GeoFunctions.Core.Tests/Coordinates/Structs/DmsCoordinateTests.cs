﻿using System;
using System.Collections.Generic;
using System.Text;
using GeoFunctions.Core.Coordinates.Structs;
using Xunit;

namespace GeoFunctions.Core.Tests.Coordinates.Structs
{
    public class DmsCoordinateTests
    {
        [Theory]
        [InlineData("D MM SS.ss H", "9 02 38.95 S")]
        [InlineData("HDMMSS.s", "S90238.9")]
        [InlineData("DD° MM' SS.s\" H", "09° 02' 38.9\" S")]
        public void DmsCoordinate_CorrectlyParsesFormatString(string format, string expected)
        {
            var sut = new DmsCoordinate
            {
                Degrees = 9.0,
                Minutes = 2.0,
                Seconds = 38.94503637783,
                Hemisphere = Hemisphere.South
            };

            var result = sut.ToString(format, null);

            Assert.Equal(expected, result);
        }

    }
}