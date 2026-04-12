// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.Mathematics;

namespace CMiX.Studio.Views.BaseControl
{
    public class CMiXSlider : System.Windows.Controls.Slider
    {
        public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register("IsEditable", typeof(bool), typeof(CMiXSlider), new UIPropertyMetadata(true));
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
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
        DependencyProperty.Register("Caption", typeof(string), typeof(CMiXSlider), new PropertyMetadata(""));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        private TextBox InputValue { get; set; }
        private Border Border { get; set; }

        public override void OnApplyTemplate()
        {
            InputValue = GetTemplateChild("textInput") as TextBox;
            Border = GetTemplateChild("sliderBorder") as Border;
            if (InputValue != null)
            {
                InputValue.MouseLeave += InputValue_OnMouseLeave;
                InputValue.MouseEnter += InputValue_OnMouseEnter;
                InputValue.KeyDown += InputValue_KeyDown;
                InputValue.GotFocus += InputValue_GotFocus;
                InputValue.LostFocus += InputValue_LostFocus;
            }
        }

        private void AddParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.AddPreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.AddPreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }

        private void RemoveParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.RemovePreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.RemovePreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }

        private double oldValue;

        private void OnSwitchToEditingMode()
        {
            IsEditing = true;
            if (InputValue != null)
            {
                oldValue = this.Value;
                InputValue.Focus();
                InputValue.SelectAll();
                InputValue.CaptureMouse();
            }
        }

        private void OnSwitchToNormalMode()
        {
            IsEditing = false;
            if (InputValue != null)
            {
                oldValue = this.Value;
                InputValue.ReleaseMouseCapture();
                InputValue.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                Keyboard.ClearFocus();
            }
        }

        public void CancelUpdateValue()
        {
            InputValue.Text = oldValue.ToString();
        }

        private void InputValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSwitchToNormalMode();
                RemoveParentWindowHandlers();
            }
            else if (e.Key == Key.Escape)
            {
                CancelUpdateValue();
                OnSwitchToNormalMode();
                RemoveParentWindowHandlers();
            }
        }

        private void InputValue_OnMouseEnter(object sender, MouseEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void InputValue_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsEditing)
                return;
            AddParentWindowHandlers();
        }

        private void InputValue_GotFocus(object sender, RoutedEventArgs e)
        {
            oldValue = this.Value;
        }

        private void InputValue_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            if (!IsValidInput(textBox.Text))
                textBox.Text = oldValue.ToString();
        }

        private void ParentWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (IsEditing == false)
                return;

            if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
                CancelUpdateValue();

            OnSwitchToNormalMode();
            mouseButtonEventArgs.Handled = true;
        }

        private void ParentWindow_OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            RemoveParentWindowHandlers();
            mouseButtonEventArgs.Handled = true;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing == false)
                return;

            CancelUpdateValue();
            OnSwitchToNormalMode();
            e.Handled = true;
        }

        private bool isDragging = false;
        private double lastValue;
        private Point _lastPoint;
        private Point _mouseDownPoint;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing)
                return;

            isDragging = true;
            lastValue = this.Value;
            _lastPoint = e.GetPosition(Border);
            _mouseDownPoint = _lastPoint;
            this.CaptureMouse();
            this.Focus();
            Cursor = Cursors.None;
            e.Handled = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsEditing)
                return;

            if (!isDragging)
                return;

            var currentPoint = e.GetPosition(Border);
            var currentValue = 0.0;

            if (Orientation == Orientation.Vertical)
            {
                currentPoint.X = Border.ActualWidth / 2;
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(-offset.Y, 0, Border.ActualHeight, this.Minimum, this.Maximum);
            }
            else if (Orientation == Orientation.Horizontal)
            {
                currentPoint.Y = Border.ActualHeight / 2;
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(offset.X, 0, Border.ActualWidth, this.Minimum, this.Maximum);
            }

            double smooth = 1.0;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                smooth = 0.01;

            this.Value = Math.Clamp(lastValue + currentValue * smooth, this.Minimum, this.Maximum);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!isDragging)
                return;

            var pointFromValue = new Point();

            if (Orientation == Orientation.Vertical)
                pointFromValue = new Point(Border.ActualWidth / 2, MathUtils.Map(this.Value, this.Maximum, this.Minimum, 0, Border.ActualHeight));
            else if (Orientation == Orientation.Horizontal)
                pointFromValue = new Point(MathUtils.Map(this.Value, this.Minimum, this.Maximum, 0, Border.ActualWidth), Border.ActualHeight / 2);

            Point pointToScreen = this.PointToScreen(pointFromValue);

            if ((e.GetPosition(Border) - _mouseDownPoint).Length < DragEditHelper.ClickThreshold && IsEditable)
            {
                pointToScreen = this.PointToScreen(_mouseDownPoint);
                OnSwitchToEditingMode();
            }

            DragEditHelper.PlaceCursorAt(pointToScreen);
            this.ReleaseMouseCapture();

            isDragging = false;
            lastValue = this.Value;
            Cursor = Cursors.Arrow;
        }

        private const NumberStyles validNumberStyles =
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowThousands |
            NumberStyles.AllowLeadingSign;

        public TextBoxInputMode InputMode { get; set; }

        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;
                case TextBoxInputMode.DigitInput:
                    return CheckIsDigit(input);
                case TextBoxInputMode.DecimalInput:
                    if (input.ToCharArray().Where(x => x == ',').Count() > 1)
                        return false;
                    if (input.Contains("-"))
                    {
                        if (input.IndexOf("-") == 0 && input.Length == 1)
                            return true;
                        else
                            return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out _);
                    }
                    else
                        return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out _);

                default: throw new ArgumentException("Unknown TextBoxInputMode");
            }
        }

        private bool CheckIsDigit(string text)
        {
            return text.ToCharArray().All(Char.IsDigit);
        }
    }

    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput
    }
}
