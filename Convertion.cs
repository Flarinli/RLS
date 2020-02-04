using System;

namespace RLC
{
    public static class Convertion
    {
        public static double RadDegrees(double x) => x * 180 / Math.PI;
        public static double DegreesRad(double x) => x * Math.PI / 180;
    }
}
