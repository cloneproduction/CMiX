// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.Mathematics;

namespace CMiX.Studio.Views.BaseControl
{
    public class CMiXSlider : Slider
    {
        public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register("IsEditable", typeof(bool), typeof(CMiXSlider), new UIPropertyMetadata(true));
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(CMiXSlider), new UIPropertyMetadata(false));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(CMiXSlider), new UIPropertyMetadata(ControlPosition.Default));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(CMiXSlider), new PropertyMetadata(""));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        private Border Border { get; set; }

        public override void OnApplyTemplate()
        {
            Border = GetTemplateChild("sliderBorder") as Border;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing == false)
                return;

            IsEditing = false;
            e.Handled = true;
        }

        private bool isDragging = false;
        private bool _dragStarted = false;
        private double lastValue;
        private Point _lastPoint;
        private Point _mouseDownPoint;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsEditing)
                return;

            isDragging = true;
            _dragStarted = false;
            lastValue = this.Value;
            _lastPoint = e.GetPosition(Border);
            _mouseDownPoint = _lastPoint;
            this.CaptureMouse();
            this.Focus();
            Cursor = Cursors.None;
            e.Handled = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (IsEditing)
                return;

            if (!isDragging)
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
                currentPoint.X = Border.ActualWidth / 2;
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(-offset.Y, 0, Border.ActualHeight, this.Minimum, this.Maximum);
            }
            else if (Orientation == Orientation.Horizontal)
            {
                currentPoint.Y = Border.ActualHeight / 2;
                Point offset = new Point(currentPoint.X - _lastPoint.X, currentPoint.Y - _lastPoint.Y);
                currentValue = MathUtils.Map(offset.X, 0, Border.ActualWidth, this.Minimum, this.Maximum);
            }

            double smooth = 1.0;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                smooth = 0.01;

            this.Value = Math.Clamp(lastValue + currentValue * smooth, this.Minimum, this.Maximum);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!isDragging)
                return;

            var pointFromValue = new Point();

            if (Orientation == Orientation.Vertical)
                pointFromValue = new Point(Border.ActualWidth / 2, MathUtils.Map(this.Value, this.Maximum, this.Minimum, 0, Border.ActualHeight));
            else if (Orientation == Orientation.Horizontal)
                pointFromValue = new Point(MathUtils.Map(this.Value, this.Minimum, this.Maximum, 0, Border.ActualWidth), Border.ActualHeight / 2);

            Point pointToScreen = this.PointToScreen(pointFromValue);

            if ((e.GetPosition(Border) - _mouseDownPoint).Length < DragEditHelper.ClickThreshold && IsEditable)
            {
                pointToScreen = this.PointToScreen(_mouseDownPoint);
                IsEditing = true;
            }

            DragEditHelper.PlaceCursorAt(pointToScreen);
            this.ReleaseMouseCapture();

            isDragging = false;
            lastValue = this.Value;
            Cursor = Cursors.Arrow;
        }
    }
}
