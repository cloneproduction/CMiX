// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Core.BaseControls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class DragValue : UserControl
    {
        private Point? _mouseDownPos;
        private Point? _cursorDownScreenPos;
        private bool _dragging;
        private Point _lastScreenPos;
        private ValueInteractionScope _interaction;

        public DragValue()
        {
            InitializeComponent();

            // WPF preview mouse events become tunneling pointer handlers.
            borderValueDisplay.AddHandler(PointerPressedEvent, Border_PointerPressed, RoutingStrategies.Tunnel);
            borderValueDisplay.AddHandler(PointerReleasedEvent, Border_PointerReleased, RoutingStrategies.Tunnel);
            borderValueDisplay.AddHandler(PointerMovedEvent, Border_PointerMoved, RoutingStrategies.Tunnel);

            // WPF overrode OnPreviewMouseRightButtonDown on the whole control.
            AddHandler(PointerPressedEvent, Control_PointerPressed, RoutingStrategies.Tunnel);

            // A gesture that loses the pointer without a release must not leave the value
            // interaction scope open, which would keep throttling every later write. Ending
            // through the handle keeps a capture loss that follows a gesture this control never
            // started, or one it already ended, from flushing whatever scope is open now.
            borderValueDisplay.PointerCaptureLost += (s, e) => _interaction.Dispose();

            AddButton.Click += AddButton_Click;
            SubButton.Click += SubButton_Click;
        }

        private static double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(borderValueDisplay).Properties.IsLeftButtonPressed)
                return;

            if (IsEditing || !borderValueDisplay.IsPointerOver)
                return;

            _mouseDownPos = e.GetPosition(borderValueDisplay);
            _cursorDownScreenPos = DragEditHelper.GetMousePosition();
            _dragging = false;

            e.Pointer.Capture(borderValueDisplay);
            _interaction = ValueInteraction.BeginScope();
            e.Handled = true;
        }

        private void Border_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_mouseDownPos == null)
                return;

            var current = e.GetPosition(borderValueDisplay);

            if (!_dragging)
            {
                if (Distance(current, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                    return;

                _dragging = true;
                // Mouse.OverrideCursor becomes a local cursor on the captured element.
                borderValueDisplay.Cursor = new Cursor(StandardCursorType.None);
                _lastScreenPos = DragEditHelper.GetMousePosition();
                return;
            }

            var screenPos = DragEditHelper.GetMousePosition();
            var delta = screenPos - _lastScreenPos;

            // Physical pixels from the current screen replace the WPF SystemParameters constants.
            double screenWidth = DragEditHelper.GetScreenBounds(this).Width;

            bool wrapped = false;
            if (screenPos.X >= screenWidth - 1)
            {
                DragEditHelper.PlaceCursorAt(new Point(1, screenPos.Y));
                wrapped = true;
            }
            else if (screenPos.X <= 0)
            {
                DragEditHelper.PlaceCursorAt(new Point(screenWidth - 2, screenPos.Y));
                wrapped = true;
            }

            if (wrapped)
            {
                _lastScreenPos = DragEditHelper.GetMousePosition();
                return;
            }

            double step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? SmallChange : LargeChange;
            AdjustValue(delta.X * step);
            _lastScreenPos = screenPos;
        }

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            e.Pointer.Capture(null);
            _interaction.Dispose();

            if (_mouseDownPos == null)
                return;

            var up = e.GetPosition(borderValueDisplay);

            if (!_dragging && Distance(up, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                IsEditing = true;

            // Clearing the local value restores the cursor set by the pointerover style,
            // which a local Cursor.Default would permanently override.
            borderValueDisplay.ClearValue(CursorProperty);

            if (_dragging && _cursorDownScreenPos != null)
                DragEditHelper.PlaceCursorAt(_cursorDownScreenPos.Value);

            _mouseDownPos = null;
            _cursorDownScreenPos = null;
            _dragging = false;
        }

        private void Control_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
                IsEditing = false;
        }

        private void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            AdjustValue(SmallChange);
            e.Handled = true;
        }

        private void SubButton_Click(object? sender, RoutedEventArgs e)
        {
            AdjustValue(-SmallChange);
            e.Handled = true;
        }

        private void AdjustValue(double delta)
        {
            Value = Math.Clamp(Value + delta, Minimum, Maximum);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(Maximum), 10000.0, defaultBindingMode: BindingMode.TwoWay);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(Minimum), -10000.0, defaultBindingMode: BindingMode.TwoWay);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(Value), 0.0, defaultBindingMode: BindingMode.TwoWay);
        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly StyledProperty<double> SmallChangeProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(SmallChange), 0.001, defaultBindingMode: BindingMode.TwoWay);
        public double SmallChange
        {
            get => GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly StyledProperty<double> LargeChangeProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(LargeChange), 0.01, defaultBindingMode: BindingMode.TwoWay);
        public double LargeChange
        {
            get => GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<DragValue, bool>(nameof(IsEditing), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<DragValue, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly StyledProperty<bool> IsIntegerProperty =
            AvaloniaProperty.Register<DragValue, bool>(nameof(IsInteger), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsInteger
        {
            get => GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }
    }
}
