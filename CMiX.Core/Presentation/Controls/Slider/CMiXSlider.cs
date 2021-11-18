// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.Mathematics;

namespace CMiX.Core.Presentation.Controls
{
    public class CMiXSlider : Slider
    {
        public CMiXSlider()
        {
            OnApplyTemplate();
        }

        TextBox InputValue { get; set; }
        Border Border { get; set; }

        //private int ScreenHeight;
        private int ScreenWidth;

        public override void OnApplyTemplate()
        {
            InputValue = GetTemplateChild("textInput") as TextBox;
            Border = GetTemplateChild("sliderBorder") as Border;

            if (InputValue != null)
            {
                InputValue.MouseLeave += View_OnMouseLeave;
                InputValue.MouseEnter += View_OnMouseEnter;
            }
            //ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            base.OnApplyTemplate();
        }



        private void View_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
        }

        private void View_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
        }

        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);

            if (IsEditing == true)
            {
                if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
                {
                    UpdateValue();
                    OnSwitchToNormalMode();
                }
                else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
                {
                    CancelUpdateValue();
                    OnSwitchToNormalMode();
                }
            }
        }


        private void TextInput_GotFocus(object sender, RoutedEventArgs e)
        {
            InputValue.MouseLeave += View_OnMouseLeave;
            InputValue.MouseEnter += View_OnMouseEnter;
        }

        private void Text_OnLostFocus(object sender, RoutedEventArgs e)
        {
            InputValue.MouseLeave -= View_OnMouseLeave;
            InputValue.MouseEnter -= View_OnMouseEnter;
        }



        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnSwitchToNormalMode();
            CancelUpdateValue();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing == false)
            {
                _lastPoint = GetMousePosition();
                _mouseDownPos = e.GetPosition(this);
                Border.CaptureMouse();
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_mouseDownPos != null)
            {
                var currentPoint = GetMousePosition();

                if (currentPoint.X >= ScreenWidth - 1)
                    SetCursorPos(0, Convert.ToInt32(currentPoint.Y));
                else if (currentPoint.X <= 0)
                    SetCursorPos(ScreenWidth - 1, Convert.ToInt32(currentPoint.Y));

                var newValue = SmoothValue(currentPoint);
                this.Value = ClampMinMax(newValue);

                _lastPoint = GetMousePosition();
            }
        }

        private double SmoothValue(Point currentPoint)
        {
            Vector offset = currentPoint - _lastPoint.Value;
            double newValue;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                newValue = this.Value + offset.X * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualWidth) * 0.01;
            else
                newValue = this.Value + offset.X * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualWidth);

            return newValue;
        }

        private double ClampMinMax(double newValue)
        {
            if (newValue >= this.Maximum)
                newValue = this.Maximum;
            else if (newValue <= this.Minimum)
                newValue = this.Minimum;

            return newValue;
        }


        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            var mouseUpPos = e.GetPosition(this);
            Border.ReleaseMouseCapture();

            if (_mouseDownPos == mouseUpPos)
                OnSwitchToEditingMode();

            if (_mouseDownPos != null && IsEditing == false)
            {

                Point pointToScreen;

                double YPos = ActualHeight / 2;
                double XPos = MathUtils.Map(this.Value, this.Minimum, this.Maximum, 0, ActualWidth);

                if (XPos >= ActualWidth)
                    XPos -= 1;

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


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateValue();
                OnSwitchToNormalMode();
            }
            else if (e.Key == Key.Escape)
            {
                CancelUpdateValue();
                OnSwitchToNormalMode();
            }
            e.Handled = !IsTextAllowed(InputValue.Text);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            e.Handled = !IsTextAllowed(InputValue.Text);
        }


        private void OnSwitchToEditingMode()
        {
            IsEditing = true;
            InputValue.Focus();
            InputValue.SelectAll();
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            Keyboard.ClearFocus();
            _mouseDownPos = null;
        }

        private readonly Regex _regex = new Regex(@"[^0-9.-]+"); //regex that matches disallowed text
        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextInput_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = !IsTextAllowed(InputValue.Text);
        }

        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateValue();
            else if (e.Key == Key.Escape)
                CancelUpdateValue();

            e.Handled = !IsTextAllowed(InputValue.Text);
        }


        public void UpdateValue()
        {
            if (IsTextAllowed(InputValue.Text))
                this.Value = Double.Parse(InputValue.Text);
        }

        public void CancelUpdateValue()
        {
            double oldValue = this.Value;

            if (IsTextAllowed(InputValue.Text))
                this.Value = oldValue;

            InputValue.Text = oldValue.ToString();
        }


        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(CMiXSlider), new UIPropertyMetadata(false));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(CMiXSlider), new UIPropertyMetadata(ControlPosition.Default));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
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
