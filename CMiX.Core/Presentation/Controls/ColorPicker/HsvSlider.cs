// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class HSVSlider : Slider
    {
        static HSVSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HSVSlider), new FrameworkPropertyMetadata(typeof(HSVSlider)));
        }

        #region Dependency Properties

        public Color LeftColor
        {
            get { return (Color)GetValue(LeftColorProperty); }
            set { SetValue(LeftColorProperty, value); }
        }
        public static readonly DependencyProperty LeftColorProperty =
        DependencyProperty.Register("LeftColor", typeof(Color), typeof(HSVSlider), new UIPropertyMetadata(Colors.Black));

        public Color RightColor
        {
            get { return (Color)GetValue(RightColorProperty); }
            set { SetValue(RightColorProperty, value); }
        }
        public static readonly DependencyProperty RightColorProperty =
        DependencyProperty.Register("RightColor", typeof(Color), typeof(HSVSlider), new UIPropertyMetadata(Colors.White));

        #endregion

        private bool _isPressed = false;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            AddHandler();

            _isPressed = true;
            if (_isPressed)
            {
                Point position = e.GetPosition(this);
                double d = 0.0;
                if (this.Orientation == Orientation.Horizontal)
                    d = 1.0d / this.ActualWidth * position.X;

                else if (this.Orientation == Orientation.Vertical)
                    d = -(1.0d / this.ActualHeight * position.Y) + 1.0;

                var p = this.Maximum * d;
                this.Value = p;
            }
            e.Handled = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isPressed)
            {
                Point position = e.GetPosition(this);
                double d = 0.0;
                if (this.Orientation == Orientation.Horizontal)
                    d = 1.0d / this.ActualWidth * position.X;

                else if (this.Orientation == Orientation.Vertical)
                    d = -(1.0d / this.ActualHeight * position.Y) + 1.0;

                var p = this.Maximum * d;
                this.Value = p;
            }
        }

        private void AddHandler()
        {
            AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            _isPressed = false;
            ReleaseMouseCapture();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _isPressed = false;
        }
    }
}
