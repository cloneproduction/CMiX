// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class ColorSlider : Slider, IColorClient
    {
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        protected ColorSlider()
        {
            ValueChanged += ColorSlider_ValueChanged;
        }

        public Color LeftColor
        {
            get { return (Color)GetValue(LeftColorProperty); }
            set { SetValue(LeftColorProperty, value); }
        }
        public static readonly DependencyProperty LeftColorProperty =
        DependencyProperty.Register("LeftColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.Black));


        public Color RightColor
        {
            get { return (Color)GetValue(RightColorProperty); }
            set { SetValue(RightColorProperty, value); }
        }
        public static readonly DependencyProperty RightColorProperty =
        DependencyProperty.Register("RightColor", typeof(Color), typeof(ColorSlider), new UIPropertyMetadata(Colors.White));


        private bool _isPressed = false;


        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isPressed = false;
        }

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

                if (p >= this.Maximum)
                    p = this.Maximum;
                if (p <= this.Minimum)
                    p = this.Minimum;

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





        protected bool UpdateBackgroundWhenColorUpdated = true;
        protected IColorManager ColorManager { get; private set; }

        public virtual void ColorUpdated(Color color, IColorClient client)
        {
            ValueChanged -= ColorSlider_ValueChanged;

            UpdateColor(color);

            //if (UpdateBackgroundWhenColorUpdated)
            //    Background = CreateBackgroundBrush(color);

            ValueChanged += ColorSlider_ValueChanged;
        }

        public void Init(IColorManager colorManager)
        {
            ColorManager = colorManager;
            ColorManager.ColorChanged += ColorManager_ColorChanged;
        }

        protected virtual void ColorManager_ColorChanged(Color obj)
        {
            //System.Console.WriteLine("ColorManager_ColorChanged");
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnValueChanged();
        }

        protected virtual void OnValueChanged()
        {
        }

        protected virtual void UpdateColor(in Color color)
        {
        }
    }
}
