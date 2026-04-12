// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class DragValue : UserControl
    {
        public DragValue()
        {
            InitializeComponent();

            if (borderValueDisplay != null)
            {
                borderValueDisplay.PreviewMouseLeftButtonDown += Border_PreviewMouseLeftButtonDown;
                borderValueDisplay.PreviewMouseUp += Border_PreviewMouseUp;
                borderValueDisplay.PreviewMouseMove += Border_PreviewMouseMove;
            }

            if (ValueInput != null)
            {
                ValueInput.MouseLeave += View_OnMouseLeave;
                ValueInput.MouseEnter += View_OnMouseEnter;
            }

            if (SubButton != null)
            {
                AddButton.Click += AddButton_Click;
                SubButton.Click += SubButton_Click;
            }
        }

        private Point? _lastPoint;
        private Point? _mouseDownPos;
        private double _valueBeforeEdit;
        private double newValue;

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing)
                return;

            _lastPoint = DragEditHelper.GetMousePosition();
            _mouseDownPos = e.GetPosition(this);
            borderValueDisplay.CaptureMouse();
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownPos == null)
                return;

            var currentPoint = DragEditHelper.GetMousePosition();
            var offset = currentPoint - _lastPoint.Value;

            DragEditHelper.WrapCursorX(currentPoint, DragEditHelper.ScreenWidth, DragEditHelper.ScreenHeight);

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                newValue = this.Value + offset.X * SmallChange;
            else
                newValue = this.Value + offset.X * LargeChange;

            this.Value = Math.Clamp(newValue, Minimum, Maximum);
            _lastPoint = DragEditHelper.GetMousePosition();
        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mouseUpPos = e.GetPosition(this);
            borderValueDisplay.ReleaseMouseCapture();

            if (_mouseDownPos == null)
                return;

            if ((_mouseDownPos.Value - mouseUpPos).Length < DragEditHelper.ClickThreshold)
                OnSwitchToEditingMode();

            if (IsEditing == false)
            {
                double yPos = ActualHeight / 2;
                double xPos = ActualWidth / 4 - AddButton.ActualWidth;
                if (xPos >= ActualWidth) xPos -= 1;

                Point pointToScreen = borderValueDisplay.PointToScreen(new Point(xPos, yPos));
                DragEditHelper.PlaceCursorAt(pointToScreen);
            }

            _mouseDownPos = null;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnSwitchToNormalMode();
            CancelUpdateValue();
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

            if (IsEditing)
            {
                if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
                    UpdateValue();
                else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
                    CancelUpdateValue();

                OnSwitchToNormalMode();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = Math.Clamp(this.Value + SmallChange, Minimum, Maximum);
            e.Handled = true;
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = Math.Clamp(this.Value - SmallChange, Minimum, Maximum);
            e.Handled = true;
        }

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
        }

        private void OnSwitchToEditingMode()
        {
            _valueBeforeEdit = this.Value;
            IsEditing = true;
            ValueInput.Focus();
            ValueInput.SelectAll();
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            Keyboard.ClearFocus();
            this.Focus();
            _mouseDownPos = null;
        }

        public void UpdateValue()
        {
            if (double.TryParse(ValueInput.Text, out double result))
                this.Value = Math.Clamp(result, Minimum, Maximum);
            else
                ValueInput.Text = this.Value.ToString();
        }

        public void CancelUpdateValue()
        {
            this.Value = _valueBeforeEdit;
            ValueInput.Text = _valueBeforeEdit.ToString();
        }

        public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(10000.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(-10000.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register("SmallChange", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(0.001, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        public static readonly DependencyProperty LargeChangeProperty =
        DependencyProperty.Register("LargeChange", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(0.01, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(DragValue), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(DragValue), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(DragValue), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty IsIntegerProperty =
        DependencyProperty.Register("IsInteger", typeof(bool), typeof(DragValue), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsInteger
        {
            get { return (bool)GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }
    }
}
