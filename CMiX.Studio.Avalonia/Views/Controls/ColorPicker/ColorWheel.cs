// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    [TemplatePart(PART_CursorEllipse, typeof(Ellipse))]
    public class ColorWheel : PickerControlBase
    {
        private const string PART_CursorEllipse = "PART_CursorEllipse";

        private Ellipse? _cursorEllipse;
        private bool _isDragging;
        protected override Type StyleKeyOverride => typeof(ColorWheel);

        public ColorWheel()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            ColorChanged += ColorWheel_ColorChanged;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _cursorEllipse = e.NameScope.Find<Ellipse>(PART_CursorEllipse);

            SetCursor();
        }

        private void ColorWheel_ColorChanged(object? sender, ColorRoutedEventArgs e) => SetCursor();

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            if (!_isDragging)
            {
                _isDragging = true;

                SetColor(e.GetPosition(this));
                e.Pointer.Capture(this);
                e.Handled = true;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
            {
                SetColor(e.GetPosition(this));
                SetCursor();
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                e.Pointer.Capture(null);
                TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
            }
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
