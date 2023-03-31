// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace CMiX.Core.Presentations.Controls
{
    public class CMiXPopup : Popup
    {
        public CMiXPopup()
        {

            OnApplyTemplate();
        }


        Point _initialMousePosition;
        bool _isDragging;
        double actualWidth;
        double actualHeight;

        protected override void OnInitialized(EventArgs e)
        {
            var contents = Child as FrameworkElement;
            var border = contents.FindName("dragBar") as Border;
            //Debug.Assert(contents != null, "DraggablePopup either has no content if content that " +
            // "does not derive from FrameworkElement. Must be fixed for dragging to work.");
            if (border != null)
            {
                border.MouseLeftButtonDown += Child_MouseLeftButtonDown;
                border.MouseLeftButtonUp += Child_MouseLeftButtonUp;
                border.MouseMove += Child_MouseMove;
                border.PreviewMouseLeftButtonDown += Child_MouseLeftButtonDown;
                Mouse.Capture(border, CaptureMode.SubTree);
            }
        }


        private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var element = sender as FrameworkElement;
            actualWidth = element.ActualWidth;
            actualHeight = element.ActualHeight;
            _initialMousePosition = e.GetPosition(null);
            _isDragging = true;
        }

        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(null);
                HorizontalOffset = HorizontalOffset + (currentPoint.X - _initialMousePosition.X);
                VerticalOffset = VerticalOffset + (currentPoint.Y - _initialMousePosition.Y);
            }
        }

        private void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                var element = sender as FrameworkElement;
                //element.ReleaseMouseCapture();
                _isDragging = false;

                e.Handled = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            HorizontalOffset = -actualWidth * 0.5;
            VerticalOffset = -actualHeight * 0.5;
        }
    }
}
