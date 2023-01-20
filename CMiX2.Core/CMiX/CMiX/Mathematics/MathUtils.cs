// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CMiX.Core.Mathematics
{
    public static class MathUtils
    {
        private static Random random = new Random();
        //=-------------------------------------------------------------------
        // double between min and the max number
        public static double RandomDouble(double min, double max)
        {
            return (random.NextDouble() * (max - min)) + min;
        }
        //=----------------------------------
        // double between 0 and the max number
        public static double RandomDouble(int max)
        {
            return (random.NextDouble() * max);
        }
        //=-------------------------------------------------------------------
        // int between the min and the max number
        public static int RandomInt(int min, int max)
        {
            return random.Next(min, max + 1);
        }
        //=----------------------------------
        // int between 0 and the max number
        public static int RandomInt(int max)
        {
            return random.Next(max + 1);
        }
        //=-------------------------------------------------------------------



        public static double GetAngle(double value, double maximum, double minimum)
        {
            double current = (value / (maximum - minimum)) * 360;
            if (current == 360)
                current = 359.999;

            return current;
        }

        public static double GetAngleR(Point pos, double radius)
        {
            //Calculate out the distance(r) between the center and the position
            Point center = new Point(radius, radius);
            double xDiff = center.X - pos.X;
            double yDiff = center.Y - pos.Y;
            double r = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            //Calculate the angle
            double angle = Math.Acos((center.Y - pos.Y) / r);

            if (pos.X < radius)
                angle = 2 * Math.PI - angle;

            if (Double.IsNaN(angle))
                return 0.0;
            else
                return angle;
        }


        public static double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

        public static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static Vector3D Lerp(Vector3D firstFloat, Vector3D secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }
}
