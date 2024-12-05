// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CMiX.Studio.Behaviors
{
    public class ClosePopupBehavior : Behavior<Popup>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var popup = AssociatedObject;

            if (popup != null)
            {
                popup.MouseLeave += ColorPickerPopup_MouseLeave;
                popup.MouseEnter += ColorPickerPopup_MouseEnter;
                popup.Opened += ColorPickerPopup_Opened;
                popup.Closed += Popup_Closed;
            }
        }

        private void AddParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(AssociatedObject);
            if (parentWindow != null)
            {
                Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.AddPreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.AddPreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }

        private void RemoveParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(AssociatedObject);
            if (parentWindow != null)
            {
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.RemovePreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.RemovePreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }

        private void ColorPickerPopup_Opened(object? sender, EventArgs e)
        {
            AddParentWindowHandlers();
        }

        private void Popup_Closed(object? sender, EventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            AddParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var childRenderSize = AssociatedObject.Child.RenderSize;
            var mousePosition = e.GetPosition(AssociatedObject.Child);

            if (mousePosition.X < 0 ||
                mousePosition.Y > childRenderSize.Height ||
                mousePosition.Y < 0 ||
                mousePosition.X > childRenderSize.Width)
            {
                AssociatedObject.IsOpen = false;
            }

            e.Handled = true;
        }

        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.IsOpen = false;
            e.Handled = true;
        }

        private void ParentWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            var childRenderSize = AssociatedObject.Child.RenderSize;
            var mousePosition = e.GetPosition(AssociatedObject.Child);

            if (mousePosition.X < -64 ||
                mousePosition.Y > childRenderSize.Height + 64 ||
                mousePosition.Y < -64 ||
                mousePosition.X > childRenderSize.Width + 64)
            {
                AssociatedObject.IsOpen = false;
                RemoveParentWindowHandlers();
            }

            e.Handled = true;
        }

        private void ParentWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            RemoveParentWindowHandlers();
            e.Handled = true;
        }
    }
}
