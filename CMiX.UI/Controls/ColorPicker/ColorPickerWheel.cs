// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.ColorPicker
{
    [TemplatePart(PART_CursorEllipse, typeof(Ellipse))]
    public class ColorPickerWheel : ColorPickerBase
    {
        private const string PART_CursorEllipse = "PART_CursorEllipse";

        private Ellipse? _cursorEllipse;
        private bool _isDragging;
        private Cursor? _cursorBeforeDrag;

        // The pointer of the last event, and how far the point that sets the color is from the pointer. The point
        // follows the pointer exactly, until Shift slows it down: then it moves only a fraction of the mouse, and the
        // distance keeps what it lost, so the indicator does not jump when Shift is let go.
        private Point _lastPointer;
        private DragCursorWrap? _wrap;
        private global::Avalonia.Vector _offset;
        protected override Type StyleKeyOverride => typeof(ColorPickerWheel);

        public ColorPickerWheel()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerCaptureLost += (s, e) => EndDrag();
            ColorChanged += ColorPickerWheel_ColorChanged;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _cursorEllipse = e.NameScope.Find<Ellipse>(PART_CursorEllipse);

            SetCursor();
        }

        private void ColorPickerWheel_ColorChanged(object? sender, ColorRoutedEventArgs e) => SetCursor();

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            if (!_isDragging)
            {
                BeginDrag();

                _lastPointer = e.GetPosition(this);
                _wrap = new DragCursorWrap(this, _lastPointer);
                _offset = default;
                SetColor(_lastPointer);
                e.Pointer.Capture(this);
                e.Handled = true;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging && e.Pointer.Captured == this)
            {
                // The position goes on when the cursor is moved to the other side of the screen.
                var position = _wrap?.Position(e) ?? e.GetPosition(this);

                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                    _offset += (position - _lastPointer) * (DragEditHelper.FineSpeed - 1);

                _lastPointer = position;
                SetColor(position + _offset);
                SetCursor();
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                EndDrag();
                PlaceCursorOnTheIndicator();
                e.Pointer.Capture(null);
                TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
            }
        }

        // The cursor is hidden for the whole drag, also when the mouse is outside the wheel, and comes back at the end.
        // This is set here and not by a style on the state of the mouse, because that state ends when the pointer
        // leaves the wheel, while the drag goes on.
        private void BeginDrag()
        {
            _isDragging = true;
            PseudoClasses.Set(":dragging", true);
            _cursorBeforeDrag = Cursor;
            Cursor = new Cursor(StandardCursorType.None);
        }

        private void EndDrag()
        {
            if (!_isDragging)
                return;

            _isDragging = false;
            _wrap = null;
            PseudoClasses.Set(":dragging", false);
            Cursor = _cursorBeforeDrag;
        }

        // The cursor goes back on the indicator of the color, as the cursor of a slider goes back on its thumb.
        // The indicator is inside the circle, so the cursor is never on the edge of the control.
        private void PlaceCursorOnTheIndicator()
        {
            if (GetColorLocation() is not { } location)
                return;

            var onScreen = this.PointToScreen(location);
            DragEditHelper.PlaceCursorAt(new Point(onScreen.X, onScreen.Y));
        }

        private void SetCursor()
        {
            if (_cursorEllipse == null)
                return;

            Point? location = GetColorLocation();
            if (location == null)
                return;

            Canvas.SetLeft(_cursorEllipse, location.Value.X - _cursorEllipse.Width / 2);
            Canvas.SetTop(_cursorEllipse, location.Value.Y - _cursorEllipse.Height / 2);
        }


        private void SetColor(Point mousePosition)
        {
            if (_cursorEllipse == null)
                return;

            var centerPoint = new Point(Width / 2, Height / 2);

            var radius = (Height - _cursorEllipse.Height) / 2;

            var dx = Math.Abs(mousePosition.X - centerPoint.X);
            var dy = Math.Abs(mousePosition.Y - centerPoint.Y);

            // At the exact center there is no direction. The color has no saturation and keeps its hue.
            if (dx == 0 && dy == 0)
            {
                Color.HSV_S = 0;
                Color.UpdateEverything();
                return;
            }

            var angle = Math.Atan(dy / dx) / Math.PI * 180;
            var distance = Math.Pow(Math.Pow(dx, 2) + Math.Pow(dy, 2), 0.5);
            var saturation = distance / radius;

            if (mousePosition.X < centerPoint.X)
                angle = 180 - angle;

            if (mousePosition.Y > centerPoint.Y)
                angle = 360 - angle;

            Color.HSV_H = angle;
            Color.HSV_S = saturation;

            if (saturation > 1.0)
                Color.HSV_S = 1.0;

            Color.UpdateEverything();
        }

        private Point? GetColorLocation()
        {
            if (_cursorEllipse == null)
                return null;

            var angle = Color.HSV_H * Math.PI / 180;
            var radius = (Height - _cursorEllipse.Height) / 2 * Color.HSV_S;

            var centerPoint = new Point(Width / 2, Height / 2);

            var x = centerPoint.X + Math.Cos(angle) * radius;
            var y = centerPoint.Y - Math.Sin(angle) * radius;

            return new Point(x, y);
        }
    }
}
