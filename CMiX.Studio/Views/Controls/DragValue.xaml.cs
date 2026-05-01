// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.Controls
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

            if (SubButton != null)
            {
                AddButton.Click += AddButton_Click;
                SubButton.Click += SubButton_Click;
            }
        }

        private Point? _mouseDownPos;
        private Point? _cursorDownScreenPos;
        private bool _dragging;

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing || !borderValueDisplay.IsMouseOver)
                return;

            _mouseDownPos = e.GetPosition(borderValueDisplay);
            _cursorDownScreenPos = DragEditHelper.GetMousePosition();
            _dragging = false;

            borderValueDisplay.CaptureMouse();
            e.Handled = true;
        }

        private Point _lastScreenPos;

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownPos == null)
                return;

            var current = e.GetPosition(borderValueDisplay);

            if (!_dragging)
            {
                if ((current - _mouseDownPos.Value).Length < DragEditHelper.ClickThreshold)
                    return;

                _dragging = true;
                Mouse.OverrideCursor = Cursors.None;
                _lastScreenPos = DragEditHelper.GetMousePosition();
                return;
            }

            var screenPos = DragEditHelper.GetMousePosition();
            var delta = screenPos - _lastScreenPos;

            bool wrapped = false;
            if (screenPos.X >= DragEditHelper.ScreenWidth - 1)
            {
                DragEditHelper.PlaceCursorAt(new Point(1, screenPos.Y));
                wrapped = true;
            }
            else if (screenPos.X <= 0)
            {
                DragEditHelper.PlaceCursorAt(new Point(DragEditHelper.ScreenWidth - 2, screenPos.Y));
                wrapped = true;
            }

            if (wrapped)
            {
                _lastScreenPos = DragEditHelper.GetMousePosition();
                return;
            }

            double step = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? SmallChange : LargeChange;
            AdjustValue(delta.X * step);
            _lastScreenPos = screenPos;
        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            borderValueDisplay.ReleaseMouseCapture();

            if (_mouseDownPos == null)
                return;

            var up = e.GetPosition(borderValueDisplay);

            if (!_dragging && (up - _mouseDownPos.Value).Length < DragEditHelper.ClickThreshold)
                IsEditing = true;

            Mouse.OverrideCursor = null;

            if (_dragging && _cursorDownScreenPos != null)
                DragEditHelper.PlaceCursorAt(_cursorDownScreenPos.Value);

            _mouseDownPos = null;
            _cursorDownScreenPos = null;
            _dragging = false;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            IsEditing = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustValue(SmallChange);
            e.Handled = true;
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            AdjustValue(-SmallChange);
            e.Handled = true;
        }

        private void AdjustValue(double delta)
        {
            Value = Math.Clamp(Value + delta, Minimum, Maximum);
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
