// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace CMiX.Studio.Views.BaseControl
{
    public partial class Slider : UserControl
    {
        public Slider()
        {
            InitializeComponent();
            CMiXSlider.ApplyTemplate();

            CMiXSlider.PreviewMouseUp += CMiXSlider_PreviewMouseUp;
            CMiXSlider.PreviewMouseMove += CMiXSlider_MouseMove;
            CMiXSlider.PreviewMouseDown += CMiXSlider_MouseDown;
            EditableValue = editableValue;
        }

        EditableValue EditableValue { get; set; }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
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

        private void CMiXSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastPoint = GetMousePosition();
            _mouseDownPos = e.GetPosition(this);
            CMiXSlider.CaptureMouse();
        }

        private void CMiXSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (_lastPoint != null)
            {
                var currentPoint = GetMousePosition();
                var offset = currentPoint - _lastPoint.Value;

                newValue = CMiXSlider.Value + offset.X * ((Math.Abs(CMiXSlider.Minimum) + Math.Abs(CMiXSlider.Maximum)) / ActualWidth);

                if (newValue >= CMiXSlider.Maximum)
                    newValue = CMiXSlider.Maximum;
                else if (newValue <= CMiXSlider.Minimum)
                    newValue = CMiXSlider.Minimum;

                CMiXSlider.Value = newValue;
                _lastPoint = GetMousePosition();
            }
        }

        private void CMiXSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mouseUpPos = e.GetPosition(this);

            if (_mouseDownPos != null)
            {
                Point pointToScreen;
                if (mouseUpPos.X > this.ActualWidth)
                {
                    pointToScreen = this.PointToScreen(new Point() { X = ActualWidth, Y = ActualHeight / 2 });
                }
                else if (mouseUpPos.X < 0)
                {
                    pointToScreen = this.PointToScreen(new Point() { X = 0, Y = ActualHeight / 2 });
                }
                else
                {
                    pointToScreen = this.PointToScreen(new Point() { X = mouseUpPos.X, Y = ActualHeight / 2 });
                }
                SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            }

            if (_mouseDownPos == mouseUpPos)
            {
                EditableValue.IsEditing = true;
                Console.WriteLine("EditableValue.IsEditing " + EditableValue.IsEditing);
            }


            CMiXSlider.ReleaseMouseCapture();
            _lastPoint = null;
        }



        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(Slider), new PropertyMetadata(""));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
    }
}
