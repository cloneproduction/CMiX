// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Interaction;

namespace CMiX.UI.Controls.Number
{
    public class DragValue : TemplatedControl, IValueText, ICaptioned, IEditableValue
    {
        private Border? _borderValueDisplay;
        private Point? _mouseDownPos;
        private Point? _cursorDownScreenPos;
        private bool _dragging;
        private Point _lastPosition;
        private DragCursorWrap? _wrap;
        private IDisposable? _interaction;
        private readonly ContextMenuGuard _contextMenuGuard;

        public DragValue()
        {
            AddHandler(PointerPressedEvent, Control_PointerPressed, RoutingStrategies.Tunnel);
            _contextMenuGuard = ContextMenuGuard.Install(this, () => IsEditing, () => new[] { LeadingContent, TrailingContent });
            ValueMenu.Follow(this, ResetCommandProperty, ResetAllCommandProperty);
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
                _borderValueDisplay.PointerCaptureLost += (s, ev) =>
                {
                    _interaction?.Dispose();
                    PseudoClasses.Set(":pressed", false);
                };
            }

            if (addButton != null)
                addButton.Click += AddButton_Click;

            if (subButton != null)
                subButton.Click += SubButton_Click;
        }


        bool IValueText.CanCopyValueText() => true;

        string IValueText.CopyValueText() => ValueText.Format(Value);

        bool IValueText.CanPasteValueText(string? text) => !IsReadOnly && ValueText.TryParse(text, out _);

        void IValueText.PasteValueText(string? text)
        {
            if (IsReadOnly || !ValueText.TryParse(text, out var number))
                return;

            if (IsInteger)
                number = Math.Round(number);
            Value = Math.Clamp(number, Minimum, Maximum);
        }

        bool IEditableValue.CanEdit => !IsReadOnly;

        // The number box of the template holds the typed text.
        void IEditableValue.EndEdit() => this.GetVisualDescendants().OfType<NumericInput>().FirstOrDefault(input => input.IsEditing)?.Commit();

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
            _wrap = new DragCursorWrap(_borderValueDisplay, _mouseDownPos.Value);
            _dragging = false;

            e.Pointer.Capture(_borderValueDisplay);
            PseudoClasses.Set(":pressed", true);
            _interaction = InteractionHost.BeginScope();
            e.Handled = true;
        }

        private void Border_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (_mouseDownPos == null || _borderValueDisplay == null)
                return;

            // The position goes on when the cursor is moved to the other side of the screen.
            var current = _wrap?.Position(e) ?? e.GetPosition(_borderValueDisplay);

            if (!_dragging)
            {
                if (Distance(current, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                    return;

                _dragging = true;
                _borderValueDisplay.Cursor = new Cursor(StandardCursorType.None);
                _lastPosition = current;
                return;
            }

            // The value changes by pixels of the screen, so the units of the visual are scaled back.
            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
            double step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? SmallChange : LargeChange;
            AdjustValue((current.X - _lastPosition.X) * scaling * step);
            _lastPosition = current;
        }

        private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            PseudoClasses.Set(":pressed", false);
            e.Pointer.Capture(null);
            _interaction?.Dispose();

            if (_mouseDownPos == null || _borderValueDisplay == null)
                return;

            var up = e.GetPosition(_borderValueDisplay);

            if (!_dragging && Distance(up, _mouseDownPos.Value) < DragEditHelper.ClickThreshold)
                IsEditing = true;

            _borderValueDisplay.ClearValue(CursorProperty);

            if (_dragging && _cursorDownScreenPos != null)
                DragEditHelper.PlaceCursorAt(_cursorDownScreenPos.Value);

            _mouseDownPos = null;
            _wrap = null;
            _cursorDownScreenPos = null;
            _dragging = false;
        }

        private void Control_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var rightPressed = e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
            _contextMenuGuard.PointerPressed(rightPressed);

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

        // The value change per pixel of mouse move. Shift gives the small step.
        public const double DefaultLargeChange = 0.001;
        public const double DefaultSmallChange = 0.0001;

        public static readonly StyledProperty<double> SmallChangeProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(SmallChange), DefaultSmallChange, defaultBindingMode: BindingMode.TwoWay);
        public double SmallChange
        {
            get => GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly StyledProperty<double> LargeChangeProperty =
            AvaloniaProperty.Register<DragValue, double>(nameof(LargeChange), DefaultLargeChange, defaultBindingMode: BindingMode.TwoWay);
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

        public static readonly StyledProperty<object> LeadingContentProperty =
            AvaloniaProperty.Register<DragValue, object>(nameof(LeadingContent));
        public object LeadingContent
        {
            get => GetValue(LeadingContentProperty);
            set => SetValue(LeadingContentProperty, value);
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

        // Set when this editor is one part of a group (a vector). The Reset menu then also has Reset All.
        public static readonly StyledProperty<ICommand> ResetAllCommandProperty =
            AvaloniaProperty.Register<DragValue, ICommand>(nameof(ResetAllCommand));

        public ICommand ResetAllCommand
        {
            get => GetValue(ResetAllCommandProperty);
            set => SetValue(ResetAllCommandProperty, value);
        }
    }
}
