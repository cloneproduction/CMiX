// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Drawing;

namespace CMiX.Core.Presentation.Controls
{
    public class SaturationColorSlider : ColorSlider
    {
        public SaturationColorSlider()
        {
            Minimum = 0;
            Maximum = 100;
        }


        protected override void UpdateColor(in Color color)
        {
            base.UpdateColor(in color);

            var hsv = new HsvColor(color);

            Value = hsv.S * 100;
        }

        protected override void OnValueChanged()
        {
            base.OnValueChanged();

            var hsv = new HsvColor(ColorManager.CurrentColor)
            {
                S = Value / 100.0
            };

            ColorManager.CurrentColor = hsv.ToRgbColor();
        }

        //protected override List<Color> CreateBackgroundColors(in Color color)
        //{
        //    var colors = new List<Color>();

        //    var hsv = new HsvColor(color)
        //    {
        //        S = 0
        //    };
        //    colors.Add(hsv.ToRgbColor());

        //    hsv.S = 1;

        //    colors.Add(hsv.ToRgbColor());

        //    return colors;
        //}



        //private bool _isPressed = false;

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    Mouse.Capture(this);
        //    AddHandler();

        //    _isPressed = true;
        //    if (_isPressed)
        //    {
        //        System.Windows.Point position = e.GetPosition(this);
        //        double d = 0.0;
        //        if (this.Orientation == Orientation.Horizontal)
        //            d = 1.0d / this.ActualWidth * position.X;

        //        else if (this.Orientation == Orientation.Vertical)
        //            d = -(1.0d / this.ActualHeight * position.Y) + 1.0;

        //        var p = this.Maximum * d;
        //        this.Value = p;
        //    }
        //    e.Handled = true;
        //    base.OnPreviewMouseLeftButtonDown(e);
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (_isPressed)
        //    {
        //        System.Windows.Point position = e.GetPosition(this);
        //        double d = 0.0;
        //        if (this.Orientation == Orientation.Horizontal)
        //            d = 1.0d / this.ActualWidth * position.X;

        //        else if (this.Orientation == Orientation.Vertical)
        //            d = -(1.0d / this.ActualHeight * position.Y) + 1.0;

        //        var p = this.Maximum * d;
        //        this.Value = p;
        //    }
        //}

        //private void AddHandler()
        //{
        //    AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        //}

        //private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        //{
        //    _isPressed = false;
        //    ReleaseMouseCapture();
        //}

        //protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonUp(e);
        //    _isPressed = false;
        //}
    }
}
