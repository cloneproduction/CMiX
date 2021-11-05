// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Drawing;
using System.Windows;

namespace CMiX.Core.Presentation.Controls
{
    public class RgbaColorSlider : ColorSlider
    {
        public static readonly DependencyProperty ChannelProperty = DependencyProperty.Register(
           nameof(Channel), typeof(RgbaChannel), typeof(RgbaColorSlider),
           new PropertyMetadata(default(RgbaChannel), ChannelChanged));

        private static void ChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RgbaColorSlider colorSlider)
                colorSlider.ChannelChanged();
        }


        public RgbaChannel Channel
        {
            get => (RgbaChannel)GetValue(ChannelProperty);
            set => SetValue(ChannelProperty, value);
        }


        public RgbaColorSlider()
        {
            Minimum = 0;
            Maximum = 255;
        }

        protected override void UpdateColor(in Color color)
        {
            base.UpdateColor(in color);

            switch (Channel)
            {
                case RgbaChannel.Red:
                    Value = color.R;
                    break;
                case RgbaChannel.Green:
                    Value = color.G;
                    break;
                case RgbaChannel.Blue:
                    Value = color.B;
                    break;
                case RgbaChannel.Alpha:
                    Value = color.A;
                    break;
                default:
                    Value = 0;
                    break;
            }
        }

        protected override void OnValueChanged()
        {
            base.OnValueChanged();

            var color = ColorManager.CurrentColor;

            var r = color.R;
            var g = color.G;
            var b = color.B;
            var a = color.A;

            switch (Channel)
            {
                case RgbaChannel.Red:
                    r = (byte)Value;
                    break;
                case RgbaChannel.Green:
                    g = (byte)Value;
                    break;
                case RgbaChannel.Blue:
                    b = (byte)Value;
                    break;
                case RgbaChannel.Alpha:
                    a = (byte)Value;
                    break;
            }

            ColorManager.CurrentColor = Color.FromArgb(a, r, g, b);
        }


        private void ChannelChanged()
        {
            if (ColorManager != null)
                UpdateColor(ColorManager.CurrentColor);
        }




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
