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
    public class SliderCMiX : Slider, IValueText, ICaptioned, IEditableValue
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


        bool IValueText.CanCopyValueText() => true;

        string IValueText.CopyValueText() => ValueText.Format(Value);

        bool IValueText.CanPasteValueText(string? text) => !IsReadOnly && ValueText.TryParse(text, out _);

        void IValueText.PasteValueText(string? text)
        {
            if (IsReadOnly || !ValueText.TryParse(text, out var number))
                return;

            Value = Math.Clamp(number, Minimum, Maximum);
        }

        protected override Type StyleKeyOverride => typeof(SliderCMiX);

        private Border? Border { get; set; }
        private readonly ContextMenuGuard _contextMenuGuard;

        public SliderCMiX()
        {
            AddHandler(PointerPressedEvent, OnTunnelPointerPressed, RoutingStrategies.Tunnel);
            AddHandler(PointerMovedEvent, OnTunnelPointerMoved, RoutingStrategies.Tunnel);
            AddHandler(PointerReleasedEvent, OnTunnelPointerReleased, RoutingStrategies.Tunnel);
            _contextMenuGuard = ContextMenuGuard.Install(this, () => IsEditing, () => new[] { TrailingContent });
            ValueMenu.Follow(this, ResetCommandProperty);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Border = e.NameScope.Find<Border>("sliderBorder");
        }

        bool IEditableValue.CanEdit => IsEditable && !IsReadOnly;

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
            _wrap = null;
            PseudoClasses.Set(":pressed", false);
            Cursor = Cursor.Default;
        }

        // The value change per pixel of mouse move, as a part of the range. A slider from 0 to 1 moves as a DragValue.
        // Shift gives the small step.
        public static readonly StyledProperty<double> SmallChangeProperty =
            AvaloniaProperty.Register<SliderCMiX, double>(nameof(SmallChange), DragValue.DefaultSmallChange);
        public double SmallChange
        {
            get => GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }

        public static readonly StyledProperty<double> LargeChangeProperty =
            AvaloniaProperty.Register<SliderCMiX, double>(nameof(LargeChange), DragValue.DefaultLargeChange);
        public double LargeChange
        {
            get => GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        private DragCursorWrap? _wrap;
        private bool isDragging = false;
        private bool _dragStarted = false;
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
            _lastPoint = e.GetPosition(Border);
            _mouseDownPoint = _lastPoint;
            _wrap = new DragCursorWrap(Border, _lastPoint);
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

            // The position goes on when the cursor is moved to the other side of the screen.
            var currentPoint = _wrap?.Position(e) ?? e.GetPosition(Border);

            if (!_dragStarted)
            {
                _dragStarted = true;
                return;
            }

            // The value changes by a fixed part of the range for each pixel, whatever the size of the track.
            var delta = Orientation == Orientation.Vertical ? _lastPoint.Y - currentPoint.Y : currentPoint.X - _lastPoint.X;
            _lastPoint = currentPoint;

            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
            var step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? SmallChange : LargeChange;
            Value = Math.Clamp(Value + delta * scaling * step * (Maximum - Minimum), Minimum, Maximum);
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
            PseudoClasses.Set(":pressed", false);
            Cursor = Cursor.Default;
        }
    }
}
