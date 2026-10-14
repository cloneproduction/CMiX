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
using Avalonia.Layout;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Interaction;
using CMiX.UI.Mathematics;

namespace CMiX.UI.Controls.Number
{
    public class SliderCMiX : Slider
    {
        public static readonly StyledProperty<bool> IsEditableProperty =
            AvaloniaProperty.Register<SliderCMiX, bool>(nameof(IsEditable), true);
        public bool IsEditable
        {
            get => GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<SliderCMiX, bool>(nameof(IsEditing));
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<SliderCMiX, bool>(nameof(IsReadOnly), false, defaultBindingMode: BindingMode.OneWay);
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<SliderCMiX, string>(nameof(Caption), "");
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        // Reserves space on the right for a control like ButtonModulatorAssign, so a SliderCMiX
        // with a trailing button lines up with a plain one - see DragValue.TrailingContent for the
        // same mechanism on the other value editor.
        public static readonly StyledProperty<object> TrailingContentProperty =
            AvaloniaProperty.Register<SliderCMiX, object>(nameof(TrailingContent));
        public object TrailingContent
        {
            get => GetValue(TrailingContentProperty);
            set => SetValue(TrailingContentProperty, value);
        }

        public static readonly StyledProperty<ICommand> ResetCommandProperty =
            AvaloniaProperty.Register<SliderCMiX, ICommand>(nameof(ResetCommand));
        public ICommand ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(SliderCMiX);

        private Border? Border { get; set; }
        private readonly ContextMenuGuard _contextMenuGuard;

        public SliderCMiX()
        {
            AddHandler(PointerPressedEvent, OnTunnelPointerPressed, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnTunnelPointerMoved, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, OnTunnelPointerReleased, RoutingStrategies.Tunnel);
            _contextMenuGuard = ContextMenuGuard.Install(this, () => IsEditing);
            ResetMenu.Follow(this, ResetCommandProperty);
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
            EndDrag();
        }

        // The pointer can be taken away without a release, for example when the popup of the slider closes.
        // The slider must not keep following the mouse after that.
        private void EndDrag()
        {
            isDragging = false;
            _dragStarted = false;
            PseudoClasses.Set(":pressed", false);
            Cursor = Cursor.Default;
        }

        private bool isDragging = false;
        private bool _dragStarted = false;
        private double lastValue;
        private Point _lastPoint;
        private Point _mouseDownPoint;
        private IDisposable? _interaction;

        private static double Length(Point p) => Math.Sqrt(p.X * p.X + p.Y * p.Y);

        private void OnTunnelPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            _contextMenuGuard.PointerPressed(properties.IsRightButtonPressed);

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

            // A click on TrailingContent (the modulator assign button) is not a drag on the
            // slider itself - only claim it when the press lands on the slider's own track,
            // the same guard DragValue already uses for its own trailing content.
            if (!Border.IsPointerOver)
                return;

            isDragging = true;
            _dragStarted = false;
            lastValue = Value;
            _lastPoint = e.GetPosition(Border);
            _mouseDownPoint = _lastPoint;
            e.Pointer.Capture(this);
            _interaction = InteractionHost.BeginScope();
            Focus();
            Cursor = new Cursor(StandardCursorType.None);
            PseudoClasses.Set(":pressed", true);
            e.Handled = true;
        }

        private void OnTunnelPointerMoved(object? sender, PointerEventArgs e)
        {
            if (IsEditing || !isDragging || Border == null || e.Pointer.Captured != this)
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
                currentValue = MathHelper.Map(-offset.Y, 0, Border.Bounds.Height, Minimum, Maximum);
            }
            else if (Orientation == Orientation.Horizontal)
            {
                currentPoint = currentPoint.WithY(Border.Bounds.Height / 2);
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathHelper.Map(offset.X, 0, Border.Bounds.Width, Minimum, Maximum);
            }

            double smooth = 1.0;
            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                smooth = 0.01;

            Value = Math.Clamp(lastValue + currentValue * smooth, Minimum, Maximum);
        }

        // The cursor goes back on the thumb. At the end of the track the thumb is on the far edge, and a cursor on
        // the edge itself is outside the track, so the next press would miss the slider. The cursor stays a few
        // pixels inside the track, on the screen.
        private const int CursorInset = 3;

        private Point InsideTheTrack(PixelPoint point)
        {
            var topLeft = Border!.PointToScreen(new Point(0, 0));
            var bottomRight = Border.PointToScreen(new Point(Border.Bounds.Width, Border.Bounds.Height));

            return new Point(
                Clamp(point.X, topLeft.X + CursorInset, bottomRight.X - 1 - CursorInset),
                Clamp(point.Y, topLeft.Y + CursorInset, bottomRight.Y - 1 - CursorInset));
        }

        // A track shorter than twice the inset keeps its middle.
        private static int Clamp(int value, int low, int high) => low > high ? (low + high) / 2 : Math.Clamp(value, low, high);

        private void OnTunnelPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!isDragging || Border == null)
                return;

            var pointFromValue = new Point();

            if (Orientation == Orientation.Vertical)
                pointFromValue = new Point(Border.Bounds.Width / 2, MathHelper.Map(Value, Maximum, Minimum, 0, Border.Bounds.Height));
            else if (Orientation == Orientation.Horizontal)
                pointFromValue = new Point(MathHelper.Map(Value, Minimum, Maximum, 0, Border.Bounds.Width), Border.Bounds.Height / 2);

            var pointToScreen = this.PointToScreen(pointFromValue);

            var upPoint = e.GetPosition(Border);
            var moved = new Point(upPoint.X - _mouseDownPoint.X, upPoint.Y - _mouseDownPoint.Y);
            if (Length(moved) < DragEditHelper.ClickThreshold && IsEditable)
            {
                pointToScreen = this.PointToScreen(_mouseDownPoint);
                IsEditing = true;
            }

            DragEditHelper.PlaceCursorAt(InsideTheTrack(pointToScreen));
            e.Pointer.Capture(null);
            _interaction?.Dispose();

            isDragging = false;
            lastValue = Value;
            PseudoClasses.Set(":pressed", false);
            Cursor = Cursor.Default;
        }
    }
}
