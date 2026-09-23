// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using CMiX.Core.BaseControls;
using CMiX.Studio.Avalonia.Mathematics;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public class CMiXSlider : Slider
    {
        public static readonly StyledProperty<bool> IsEditableProperty =
            AvaloniaProperty.Register<CMiXSlider, bool>(nameof(IsEditable), true);
        public bool IsEditable
        {
            get => GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<CMiXSlider, bool>(nameof(IsEditing));
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<CMiXSlider, string>(nameof(Caption), "");
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        // Reserves space on the right for a control like ModulatorAssignButton, so a CMiXSlider
        // with a trailing button lines up with a plain one - see DragValue.TrailingContent for the
        // same mechanism on the other value editor.
        public static readonly StyledProperty<object> TrailingContentProperty =
            AvaloniaProperty.Register<CMiXSlider, object>(nameof(TrailingContent));
        public object TrailingContent
        {
            get => GetValue(TrailingContentProperty);
            set => SetValue(TrailingContentProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(CMiXSlider);

        private Border? Border { get; set; }

        public CMiXSlider()
        {
            AddHandler(PointerPressedEvent, OnTunnelPointerPressed, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnTunnelPointerMoved, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, OnTunnelPointerReleased, RoutingStrategies.Tunnel);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Border = e.NameScope.Find<Border>("sliderBorder");
        }

        // A gesture that loses the pointer without a release must not leave the value interaction
        // scope open, which would keep throttling every later write in the application. Ending
        // through the handle keeps a capture loss that follows a gesture this slider never
        // started, or one it already ended, from flushing whatever scope is open now.
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            _interaction?.Dispose();
        }

        private bool isDragging = false;
        private bool _dragStarted = false;
        private double lastValue;
        private Point _lastPoint;
        private Point _mouseDownPoint;
        private ValueInteractionScope? _interaction;

        private static double Length(Point p) => Math.Sqrt(p.X * p.X + p.Y * p.Y);

        private void OnTunnelPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;

            if (properties.IsRightButtonPressed)
            {
                if (IsEditing == false)
                    return;

                IsEditing = false;
                e.Handled = true;
                return;
            }

            if (!properties.IsLeftButtonPressed || IsEditing || Border == null)
                return;

            isDragging = true;
            _dragStarted = false;
            lastValue = Value;
            _lastPoint = e.GetPosition(Border);
            _mouseDownPoint = _lastPoint;
            e.Pointer.Capture(this);
            _interaction = ValueInteraction.BeginScope();
            Focus();
            Cursor = new Cursor(StandardCursorType.None);
            PseudoClasses.Set(":pressed", true);
            e.Handled = true;
        }

        private void OnTunnelPointerMoved(object? sender, PointerEventArgs e)
        {
            if (IsEditing || !isDragging || Border == null)
                return;

            if (!_dragStarted)
            {
                _dragStarted = true;
                return;
            }

            var currentPoint = e.GetPosition(Border);
            var currentValue = 0.0;

            if (Orientation == Orientation.Vertical)
            {
                currentPoint = currentPoint.WithX(Border.Bounds.Width / 2);
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(-offset.Y, 0, Border.Bounds.Height, Minimum, Maximum);
            }
            else if (Orientation == Orientation.Horizontal)
            {
                currentPoint = currentPoint.WithY(Border.Bounds.Height / 2);
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(offset.X, 0, Border.Bounds.Width, Minimum, Maximum);
            }

            double smooth = 1.0;
            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                smooth = 0.01;

            Value = Math.Clamp(lastValue + currentValue * smooth, Minimum, Maximum);
        }

        private void OnTunnelPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!isDragging || Border == null)
                return;

            var pointFromValue = new Point();

            if (Orientation == Orientation.Vertical)
                pointFromValue = new Point(Border.Bounds.Width / 2, MathUtils.Map(Value, Maximum, Minimum, 0, Border.Bounds.Height));
            else if (Orientation == Orientation.Horizontal)
                pointFromValue = new Point(MathUtils.Map(Value, Minimum, Maximum, 0, Border.Bounds.Width), Border.Bounds.Height / 2);

            var pointToScreen = this.PointToScreen(pointFromValue);

            var upPoint = e.GetPosition(Border);
            var moved = new Point(upPoint.X - _mouseDownPoint.X, upPoint.Y - _mouseDownPoint.Y);
            if (Length(moved) < DragEditHelper.ClickThreshold && IsEditable)
            {
                pointToScreen = this.PointToScreen(_mouseDownPoint);
                IsEditing = true;
            }

            DragEditHelper.PlaceCursorAt(new Point(pointToScreen.X, pointToScreen.Y));
            e.Pointer.Capture(null);
            _interaction?.Dispose();

            isDragging = false;
            lastValue = Value;
            PseudoClasses.Set(":pressed", false);
            Cursor = Cursor.Default;
        }
    }
}
