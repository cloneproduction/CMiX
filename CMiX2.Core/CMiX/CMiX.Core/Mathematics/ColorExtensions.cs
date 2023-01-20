// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Media;

namespace CMiX.Core.Mathematics
{
    public static class ColorExtensions
    {
        public static Color HexStringToColor(this string hex)
        {
            var color = Color.FromArgb(255, 0, 0, 0);

            if (hex != null)
            {
                if (hex.StartsWith("#"))
                {
                    //remove the # at the front
                    hex = hex.Replace("#", "");

                    byte a = 255;
                    byte r = 255;
                    byte g = 255;
                    byte b = 255;

                    int start = 0;

                    //handle ARGB strings (8 characters long)
                    if (hex.Length == 8)
                    {
                        a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        start = 2;
                    }

                    //convert RGB characters to bytes
                    r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

                    color = Color.FromArgb(a, r, g, b);
                }
            }

            return color;
        }


        static char[] hexDigits = {
         '0', '1', '2', '3', '4', '5', '6', '7',
         '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        public static string ColorToHexString(this Color color)
        {
            byte[] bytes = new byte[4];
            bytes[0] = color.A;
            bytes[1] = color.R;
            bytes[2] = color.G;
            bytes[3] = color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return "#" + new string(chars);
        }


        public static void ConvertRgbToHsv(Color color, out double hue, out double saturation, out double value)
        {
            double red = color.R / 255.0;
            double green = color.G / 255.0;
            double blue = color.B / 255.0;
            double min = Math.Min(red, Math.Min(green, blue));
            double max = Math.Max(red, Math.Max(green, blue));

            value = max;
            double delta = max - min;

            if (value == 0)
                saturation = 0;
            else
                saturation = delta / max;

            if (saturation == 0)
                hue = 0;
            else
            {
                if (red == max)
                    hue = (green - blue) / delta;
                else if (green == max)
                    hue = 2 + (blue - red) / delta;
                else // blue == max
                    hue = 4 + (red - green) / delta;
            }
            hue *= 60;
            if (hue < 0)
                hue += 360;
        }

        public static Color ConvertHsvToRgb(double hue, double saturation, double value)
        {
            double chroma = value * saturation;

            if (hue == 360)
                hue = 0;

            double hueTag = hue / 60;
            double x = chroma * (1 - Math.Abs(hueTag % 2 - 1));
            double m = value - chroma;
            switch ((int)hueTag)
            {
                case 0:
                    return BuildColor(chroma, x, 0, m);
                case 1:
                    return BuildColor(x, chroma, 0, m);
                case 2:
                    return BuildColor(0, chroma, x, m);
                case 3:
                    return BuildColor(0, x, chroma, m);
                case 4:
                    return BuildColor(x, 0, chroma, m);
                default:
                    return BuildColor(chroma, 0, x, m);
            }
        }

        private static Color BuildColor(double red, double green, double blue, double m)
        {
            return Color.FromArgb(
                255,
                (byte)((red + m) * 255 + 0.5),
                (byte)((green + m) * 255 + 0.5),
                (byte)((blue + m) * 255 + 0.5));
        }



        public static void ColorToHSV(Color rgb, out double H, out double S, out double V)
        {
            double max, min, delta;

            //g_return_if_fail(rgb != NULL);
            //g_return_if_fail(hsv != NULL);

            max = Max(rgb);
            min = Min(rgb);

            V = max / 255;
            delta = max - min;

            if (delta > 0.0001)
            {

                S = delta / max;

                if (rgb.R == max)
                {
                    H = (rgb.G - rgb.B) / delta;
                    if (H < 0.0)
                        H += 6.0;
                }
                else if (rgb.G == max)
                {
                    H = 2.0 + (rgb.B - rgb.R) / delta;
                }
                else
                {
                    H = 4.0 + (rgb.R - rgb.G) / delta;
                }

                H /= 6.0 * 360;
            }
            else
            {
                S = 0.0;
                H = 0.0;
            }

        }

        public static Color ColorFromHSV(double H, double S, double V)
        {
            int i;
            double f, w, q, t;

            double hue;
            byte value = (byte)(V * 255);
            Color color = new Color();
            //g_return_if_fail(rgb != NULL);
            //g_return_if_fail(hsv != NULL);

            if (S == 0.0)
            {
                color.R = value;
                color.G = value;
                color.B = value;
            }
            else
            {
                hue = H;

                if (hue == 360.0)
                    hue = 0.0;

                hue *= 6.0;

                i = (int)hue;
                f = hue - i;
                w = V * (1.0 - S);
                q = V * (1.0 - (S * f));
                t = V * (1.0 - (S * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        color.R = value;
                        color.G = (byte)(t * 255);
                        color.B = (byte)(w * 255);
                        break;
                    case 1:
                        color.R = (byte)(q * 255);
                        color.G = value;
                        color.B = (byte)(w * 255);
                        break;
                    case 2:
                        color.R = (byte)(w * 255);
                        color.G = value;
                        color.B = (byte)(t * 255);
                        break;
                    case 3:
                        color.R = (byte)(w * 255);
                        color.G = (byte)(q * 255);
                        color.B = value;
                        break;
                    case 4:
                        color.R = (byte)(t * 255);
                        color.G = (byte)(w * 255);
                        color.B = value;
                        break;
                    case 5:
                        color.R = value;
                        color.G = (byte)(w * 255);
                        color.B = (byte)(q * 255);
                        break;
                }
            }

            return color;
        }

        public static byte Max(Color rgb)
        {
            if (rgb.R > rgb.G)
                return (rgb.R > rgb.B) ? rgb.R : rgb.B;
            else
                return (rgb.G > rgb.B) ? rgb.G : rgb.B;
        }

        public static byte Min(Color rgb)
        {
            if (rgb.R > rgb.G)
                return (rgb.R < rgb.B) ? rgb.R : rgb.B;
            else
                return (rgb.G < rgb.B) ? rgb.G : rgb.B;
        }
    }
}
