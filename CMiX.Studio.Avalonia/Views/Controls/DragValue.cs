// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Core.BaseControls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public class DragValue : TemplatedControl
    {
        private Border? _borderValueDisplay;
        private Point? _mouseDownPos;
        private Point? _cursorDownScreenPos;
        private bool _dragging;
        private Point _lastScreenPos;
        private ValueInteractionScope _interaction;
        private ContextMenu? _resetMenu;
        private bool _templateApplied;
        private bool _suppressContextMenu;

        public DragValue()
        {
            AddHandler(PointerPressedEvent, Control_PointerPressed, RoutingStrategies.Tunnel);
            AddHandler(ContextRequestedEvent, OnTunnelContextRequested, RoutingStrategies.Tunnel);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _borderValueDisplay = e.NameScope.Find<Border>("borderValueDisplay");
            var addButton = e.NameScope.Find<Button>("AddButton");
            var subButton = e.NameScope.Find<Button>("SubButton");

            if (_borderValueDisplay != null)
            {
                _borderValueDisplay.AddHandler(PointerPressedEvent, Border_PointerPressed, RoutingStrategies.Tunnel);
                _borderValueDisplay.AddHandler(PointerReleasedEvent, Border_PointerReleased, RoutingStrategies.Tunnel);
                _borderValueDisplay.AddHandler(PointerMovedEvent, Border_PointerMoved, RoutingStrategies.Tunnel);
                _borderValueDisplay.PointerCaptureLost += (s, ev) => _interaction.Dispose();
            }

            if (addButton != null)
                addButton.Click += AddButton_Click;

            if (subButton != null)
                subButton.Click += SubButton_Click;

            _templateApplied = true;
            UpdateResetMenu();
        }

        // The default menu exists only while the editor has a reset command. A menu set in XAML stays.
        private void UpdateResetMenu()
        {
            if (ResetCommand == null)
            {
                if (_resetMenu != null && ContextMenu == _resetMenu)
                    ContextMenu = null;
                _resetMenu = null;
                return;
            }

            if (ContextMenu == null)
                ContextMenu = _resetMenu = DefaultResetMenu.Create(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (_templateApplied && change.Property == ResetCommandProperty)
                UpdateResetMenu();
        }

        private static double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_borderValueDisplay == null || !e.GetCurrentPoint(_borderValueDisplay).Properties.IsLeftButtonPressed)
                return;

            if (IsEditing || !_borderValueDisplay.IsPointerOver)
                return;

            _mouseDownPos = e.GetPosition(_borderValueDisplay);
            _cursorDownScreenPos = DragEditHelper.GetMousePosition();
            _dragging = false;

            e.Pointer.Capture(_borderValueDisplay);
            _interaction = ValueInteraction.BeginScope();
            e.Handled = true;
        }

        private void Border_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_mouseDownPos == null || _borderValueDisplay == null)
                return;

            var current = e.GetPosition(_borderValueDisplay);

            if (!_dragging)
            {
                if (Distance(current, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                    return;

                _dragging = true;
                _borderValueDisplay.Cursor = new Cursor(StandardCursorType.None);
                _lastScreenPos = DragEditHelper.GetMousePosition();
                return;
            }

            var screenPos = DragEditHelper.GetMousePosition();
            var delta = screenPos - _lastScreenPos;

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

            if (_mouseDownPos == null || _borderValueDisplay == null)
                return;

            var up = e.GetPosition(_borderValueDisplay);

            if (!_dragging && Distance(up, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                IsEditing = true;

            _borderValueDisplay.ClearValue(CursorProperty);

            if (_dragging && _cursorDownScreenPos != null)
                DragEditHelper.PlaceCursorAt(_cursorDownScreenPos.Value);

            _mouseDownPos = null;
            _cursorDownScreenPos = null;
            _dragging = false;
        }

        // A right-click that ends editing must not open the menu when the button is released.
        private void OnTunnelContextRequested(object? sender, ContextRequestedEventArgs e)
        {
            if (!_suppressContextMenu && !IsEditing && !ContextMenuGuard.FromPopup(this, e))
                return;

            _suppressContextMenu = false;
            e.Handled = true;
        }

        private void Control_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var rightPressed = e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
            _suppressContextMenu = rightPressed && IsEditing;

            if (rightPressed)
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

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<DragValue, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
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

        public static readonly StyledProperty<bool> IsIntegerProperty =
            AvaloniaProperty.Register<DragValue, bool>(nameof(IsInteger), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsInteger
        {
            get => GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }

        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<DragValue, bool>(nameof(IsReadOnly), false, defaultBindingMode: BindingMode.OneWay);
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly StyledProperty<object> TrailingContentProperty =
            AvaloniaProperty.Register<DragValue, object>(nameof(TrailingContent));
        public object TrailingContent
        {
            get => GetValue(TrailingContentProperty);
            set => SetValue(TrailingContentProperty, value);
        }

        public static readonly StyledProperty<ICommand> ResetCommandProperty =
            AvaloniaProperty.Register<DragValue, ICommand>(nameof(ResetCommand));
        public ICommand ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }
    }
}
