// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace CMiX.Core.Presentation.Controls
{
    [TemplatePart(Name = PART_CursorEllipse, Type = typeof(Ellipse))]
    [TemplatePart(Name = PART_SpectrumEllipse, Type = typeof(Ellipse))]
    public class ColorWheel : Control, IColorClient
    {
        static ColorWheel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorWheel), new FrameworkPropertyMetadata(typeof(ColorWheel)));
        }

        private const string PART_CursorEllipse = "PART_CursorEllipse";
        private const string PART_SpectrumEllipse = "PART_SpectrumEllipse";

        private Ellipse _cursorEllipse;
        private Ellipse _spectrumEllipse;
        private bool _isDragging;

        private IColorManager _colorManager;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _cursorEllipse = GetTemplateChild(PART_CursorEllipse) as Ellipse;
            _spectrumEllipse = GetTemplateChild(PART_SpectrumEllipse) as Ellipse;

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeftButtonUp += OnMouseLeftButtonUp;

            if (_colorManager != null)
                SetCursor(_colorManager.CurrentColor);
        }


        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                _isDragging = true;

                SetColor(e.GetPosition(this));
                Mouse.Capture(this);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                SetColor(e.GetPosition(this));
                SetCursor(_colorManager.CurrentColor);
            }

        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                Mouse.Capture(null);
            }
        }

        private void SetCursor(Color color)
        {
            var location = GetColorLocation(color);

            Canvas.SetLeft(_cursorEllipse, (location.X - _cursorEllipse.Width / 2));
            Canvas.SetTop(_cursorEllipse, (location.Y - _cursorEllipse.Height / 2));
        }


        private void SetColor(Point mousePosition)
        {
            var centerPoint = new Point(ActualWidth / 2, ActualHeight / 2);

            var radius = (ActualHeight - _cursorEllipse.Height) / 2;

            var dx = Math.Abs(mousePosition.X - centerPoint.X);
            var dy = Math.Abs(mousePosition.Y - centerPoint.Y);

            var angle = Math.Atan(dy / dx) / Math.PI * 180;
            var distance = Math.Pow(Math.Pow(dx, 2) + Math.Pow(dy, 2), 0.5);
            var saturation = distance / radius;

            if (mousePosition.X < centerPoint.X)
                angle = 180 - angle;

            if (mousePosition.Y > centerPoint.Y)
                angle = 360 - angle;

            _colorManager.Color.HSV_H = angle;
            _colorManager.Color.HSV_S = saturation;
            if (saturation > 1.0)
                _colorManager.Color.HSV_S = 1.0;
        }

        private Point GetColorLocation(in Color color)
        {
            var angle = this._colorManager.ColorState.HSV_H * Math.PI / 180;
            var radius = (Height - _cursorEllipse.Height) / 2 * this._colorManager.ColorState.HSV_S;

            var centerPoint = new Point(Width / 2, Height / 2);

            var x = centerPoint.X + Math.Cos(angle) * radius;
            var y = centerPoint.Y - Math.Sin(angle) * radius;

            return new Point(x, y);
        }

        public void Init(IColorManager colorManager)
        {
            _colorManager = colorManager;
            _colorManager.ColorChanged += _colorManager_ColorChanged; ;
        }

        private void _colorManager_ColorChanged(Color obj)
        {
            SetCursor(obj);
        }
    }
}
