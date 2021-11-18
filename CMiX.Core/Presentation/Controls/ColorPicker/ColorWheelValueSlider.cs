// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CMiX.Core.Mathematics;

namespace CMiX.Core.Presentation.Controls
{
    public class ColorWheelValueSlider : Slider, IColorClient
    {
        public ColorWheelValueSlider()
        {
            OnApplyTemplate();
            Minimum = 0;
            Maximum = 100;
            ValueChanged += ColorSlider_ValueChanged;
        }

        Border Border { get; set; }

        private int ScreenHeight;
        //private int ScreenWidth;

        public override void OnApplyTemplate()
        {
            Border = GetTemplateChild("sliderBorder") as Border;
            ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            base.OnApplyTemplate();
        }


        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _lastPoint = GetMousePosition();
            _mouseDownPos = e.GetPosition(this);
            Border.CaptureMouse();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_mouseDownPos != null)
            {
                var currentPoint = GetMousePosition();

                if (currentPoint.Y >= ScreenHeight - 1)
                    SetCursorPos(0, Convert.ToInt32(currentPoint.Y));
                else if (currentPoint.Y <= 0)
                    SetCursorPos(ScreenHeight - 1, Convert.ToInt32(currentPoint.Y));

                var offset = currentPoint - _lastPoint.Value;

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    newValue = this.Value + (-offset.Y) * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualHeight) * 0.01;
                else
                    newValue = this.Value + (-offset.Y) * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualHeight);

                if (newValue >= this.Maximum)
                    newValue = this.Maximum;
                else if (newValue <= this.Minimum)
                    newValue = this.Minimum;

                this.Value = newValue;
                _lastPoint = GetMousePosition();
            }
        }





        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            Border.ReleaseMouseCapture();

            if (_mouseDownPos != null)
            {
                Point pointToScreen;

                double YPos = MathUtils.Map(this.Value, this.Minimum, this.Maximum, ActualHeight, 0);
                double XPos = ActualWidth / 2;

                if (YPos >= ActualHeight)
                    YPos -= 1;

                pointToScreen = this.PointToScreen(new Point(XPos, YPos));
                SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            }
            _mouseDownPos = null;
        }



        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };


        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }


        private Point? _mouseDownPos;
        private Point? _lastPoint;
        private double newValue;


        protected bool UpdateBackgroundWhenColorUpdated = true;
        protected IColorManager ColorManager { get; private set; }


        public void Init(IColorManager colorManager)
        {
            ColorManager = colorManager;
            ColorManager.ColorChanged += ColorManager_ColorChanged;
        }

        public void ColorManager_ColorChanged(Color obj)
        {
            Value = ColorManager.Color.HSV_V;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnValueChanged();
        }

        public void OnValueChanged()
        {
            ColorManager.Color.HSV_V = Value;
        }
    }
}
