// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.Mathematics;

namespace CMiX.Studio.Views.BaseControl
{
    public class CMiXSlider : System.Windows.Controls.Slider
    {
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
        DependencyProperty.Register("Caption", typeof(string), typeof(CMiXSlider), new PropertyMetadata(""));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }


        TextBox InputValue { get; set; }
        Border Border { get; set; }


        public override void OnApplyTemplate()
        {
            InputValue = GetTemplateChild("textInput") as TextBox;
            Border = GetTemplateChild("sliderBorder") as Border;

            if (InputValue != null)
            {
                InputValue.MouseLeave += View_OnMouseLeave;
                InputValue.MouseEnter += View_OnMouseEnter;
            }
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

            if (IsEditing == false)
                return;

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


        bool isDragging = false;
        double lastValue = 0;

        private Point _mouseDownPos;
        private Point _lastPoint;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing)
                return;

            _mouseDownPos = new Point(e.GetPosition(Border).X, e.GetPosition(Border).Y);
            Border.CaptureMouse();
            isDragging = true;

            var p = ValueToPoint(this.Value);
            _lastPoint = p;
            lastValue = this.Value;

            Point pointToScreen = this.PointToScreen(p);
            SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (!isDragging)
                return;

            var currentPoint = e.GetPosition(Border);

            if (currentPoint.X >= Border.ActualWidth)
                currentPoint.X = Border.ActualWidth;

            if (currentPoint.X <= 0)
                currentPoint.X = 0;

            currentPoint.Y = Border.ActualHeight / 2;

            Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
            var currentValue = MathUtils.Map(offset.X, 0, Border.ActualWidth, this.Minimum, this.Maximum);
            this.Value = lastValue + currentValue;

            Point pointToScreen = this.PointToScreen(currentPoint);
            SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));

            e.Handled = true;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!isDragging)
                return;

            isDragging = false;

            if(Math.Round(lastValue, 3) == Math.Round(this.Value, 3))
            {
                OnSwitchToEditingMode();
                Point pointToScreen = this.PointToScreen(_mouseDownPos);
                SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            }

            Border.ReleaseMouseCapture();
        }


        private Point ValueToPoint(double value)
        {
            var p = new Point(MathUtils.Map(value, this.Minimum, this.Maximum, 0, Border.ActualWidth), Border.ActualHeight / 2);
            return p;
        }

        private double SmoothValue(Point currentPoint)
        {
            Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
            double p = MathUtils.Map(offset.X, 0, ActualWidth, this.Minimum, this.Maximum);
            double smooth = 1.0;

            Debug.WriteLine("offset " + offset);
            Debug.WriteLine("mapped offset " + p);

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                smooth = 0.01;

            return this.Value + p;// * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualWidth); 
        }


        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);


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
            //_mouseDownPos = null;
        }
        
        private readonly Regex _regex = new Regex(@"[^0-9.-]+"); //regex that matches disallowed text
       //private readonly Regex _regex = new Regex(@"^-?[0-9]\d*(\.\d+)?$");
        private bool IsTextAllowed(string text)
        {
            bool result = !_regex.IsMatch(text);
            return result;
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
    }
}
