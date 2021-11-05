// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Drawing;

namespace CMiX.Core.Presentation.Controls
{
    public struct HsvColor
    {

        public static readonly HsvColor Empty;


        private int _alpha;

        private double _hue;

        private double _brightness;

        private double _saturation;


        public bool IsEmpty { get; private set; }

        public int A
        {
            get => _alpha;
            set => _alpha = Math.Min(0, Math.Max(255, value));
        }

        public double H
        {
            get => _hue;
            set
            {
                if (_hue > 359) _hue = 0;

                if (_hue < 0) _hue = 359;

                _hue = value;
            }
        }

        public double S
        {
            get => _saturation;
            set => _saturation = Math.Min(1, Math.Max(0, value));
        }

        public double V
        {
            get => _brightness;
            set => _brightness = Math.Min(1, Math.Max(0, value));
        }


        static HsvColor()
        {
            Empty = new HsvColor
            {
                IsEmpty = true
            };
        }


        public HsvColor(double hue, double saturation, double brightness)
            : this(255, hue, saturation, brightness)
        {
        }

        public HsvColor(byte alpha, double hue, double saturation, double brightness)
        {
            _hue = Math.Min(359, hue);
            _saturation = Math.Min(1, saturation);
            _brightness = Math.Min(1, brightness);
            _alpha = alpha;
            IsEmpty = false;
        }

        public HsvColor(Color color)
        {
            _alpha = color.A;

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            _hue = color.GetHue();
            _saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            _brightness = max / 255d;

            IsEmpty = false;
        }


        public Color ToRgbColor() => ToRgbColor(A);

        public Color ToRgbColor(int alpha)
        {
            int hi = Convert.ToInt32(Math.Floor(_hue / 60)) % 6;
            double f = _hue / 60 - Math.Floor(_hue / 60);

            double value = _brightness * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - _saturation));
            byte q = Convert.ToByte(value * (1 - f * _saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * _saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);

            //byte r = 0;
            //byte g = 0;
            //byte b = 0;

            //var hue = _hue;

            //hue /= 60;
            //var i = (int)Math.Floor(hue);

            //var f = hue - i;
            //var p = _brightness * (1 - _saturation);
            //var q = _brightness * (1 - _saturation * f);
            //var t = _brightness * (1 - _saturation * (1 - f));

            //switch (i)
            //{
            //    case 0:
            //        r = (byte)(255 * _brightness);
            //        g = (byte)(255 * t);
            //        b = (byte)(255 * p);
            //        break;
            //    case 1:
            //        r = (byte)(255 * q);
            //        g = (byte)(255 * _brightness);
            //        b = (byte)(255 * p);
            //        break;
            //    case 2:
            //        r = (byte)(255 * p);
            //        g = (byte)(255 * _brightness);
            //        b = (byte)(255 * t);
            //        break;
            //    case 3:
            //        r = (byte)(255 * p);
            //        g = (byte)(255 * q);
            //        b = (byte)(255 * _brightness);
            //        break;
            //    case 4:
            //        r = (byte)(255 * t);
            //        g = (byte)(255 * p);
            //        b = (byte)(255 * _brightness);
            //        break;
            //    default:
            //        r = (byte)(255 * _brightness);
            //        g = (byte)(255 * p);
            //        b = (byte)(255 * q);
            //        break;
            //}

            //return Color.FromArgb(alpha, r, g, b);
        }
    }
}
