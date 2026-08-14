// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Popup with a draggable title area named dragBar in its content.
    public class CMiXPopup : Popup
    {
        Point _initialMousePosition;
        bool _isDragging;
        double actualWidth;
        double actualHeight;
        Border _dragBar;

        public CMiXPopup()
        {
            Opened += CMiXPopup_Opened;
            Closed += CMiXPopup_Closed;
        }

        private void CMiXPopup_Opened(object sender, EventArgs e)
        {
            if (_dragBar != null)
                return;

            _dragBar = (Child as Visual)?.GetVisualDescendants()
                                         .OfType<Border>()
                                         .FirstOrDefault(b => b.Name == "dragBar");

            if (_dragBar != null)
            {
                _dragBar.PointerPressed += DragBar_PointerPressed;
                _dragBar.PointerReleased += DragBar_PointerReleased;
                _dragBar.PointerMoved += DragBar_PointerMoved;
            }
        }

        private void DragBar_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(_dragBar).Properties.IsLeftButtonPressed)
                return;

            e.Handled = true;
            actualWidth = _dragBar.Bounds.Width;
            actualHeight = _dragBar.Bounds.Height;
            _initialMousePosition = e.GetPosition(null);
            _isDragging = true;
            e.Pointer.Capture(_dragBar);
        }

        private void DragBar_PointerMoved(object sender, PointerEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = e.GetPosition(null);
                HorizontalOffset = HorizontalOffset + (currentPoint.X - _initialMousePosition.X);
                VerticalOffset = VerticalOffset + (currentPoint.Y - _initialMousePosition.Y);
            }
        }

        private void DragBar_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                e.Pointer.Capture(null);
                e.Handled = true;
            }
        }

        private void CMiXPopup_Closed(object sender, EventArgs e)
        {
            HorizontalOffset = -actualWidth * 0.5;
            VerticalOffset = -actualHeight * 0.5;
        }
    }
}
