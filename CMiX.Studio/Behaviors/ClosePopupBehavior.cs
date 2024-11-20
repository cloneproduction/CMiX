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
            var colorPickerPopup = AssociatedObject;

            if (colorPickerPopup != null)
            {
                colorPickerPopup.MouseLeave += ColorPickerPopup_MouseLeave;
                colorPickerPopup.MouseEnter += ColorPickerPopup_MouseEnter;
                colorPickerPopup.Opened += ColorPickerPopup_Opened;
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


        private void ColorPickerPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            AddParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

//public static ContentControl GetPopupContainer(DependencyObject obj)
//{
//    return (ContentControl)obj.GetValue(PopupContainerProperty);
//}

//public static void SetPopupContainer(DependencyObject obj, ContentControl value)
//{
//    obj.SetValue(PopupContainerProperty, value);
//}

//public static readonly DependencyProperty PopupContainerProperty =
//    DependencyProperty.RegisterAttached("PopupContainer", typeof(ContentControl), typeof(ClosePopupBehavior), 
//        new PropertyMetadata(OnPopupContainerChanged));

//private static void OnPopupContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//{
//    var popup = (Popup)d;
//    var contentControl = e.NewValue as ContentControl;

//    popup.LostFocus += (sender, args) =>
//    {
//        var popup1 = (Popup)sender;
//        popup.IsOpen = false;
//        if (contentControl != null)
//            contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
//    };
//    popup.Opened += (sender, args) =>
//    {
//        var popup1 = (Popup)sender;
//        popup.Focus();
//        SetWindowPopup(contentControl, popup1);
//        //contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
//        //contentControl.PreviewMouseDown += ContainerOnPreviewMouseDown;
//    };
//    popup.PreviewMouseUp += (sender, args) =>
//    {
//        popup.IsOpen = false;
//        if (contentControl != null)
//            contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
//    };
//    popup.MouseLeave += (sender, args) =>
//    {
//        popup.IsOpen = false;
//        if (contentControl != null)
//            contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
//    };
//    popup.Unloaded += (sender, args) =>
//    {
//        popup.IsOpen = false;
//        if (contentControl != null)
//            contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
//    };
//}

////This is really to handle touch panel, Not sure if it is needed since handle LostFocus.
////Remove the contentControl stuff if it is not needed
//private static void ContainerOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
//{
//    var popup = GetWindowPopup((DependencyObject)sender);
//    popup.IsOpen = false;
//    ((FrameworkElement)sender).PreviewMouseUp -= ContainerOnPreviewMouseDown;
//}

//private static Popup GetWindowPopup(DependencyObject obj)
//{
//    return (Popup)obj.GetValue(WindowPopupProperty);
//}

//private static void SetWindowPopup(DependencyObject obj, Popup value)
//{
//    obj.SetValue(WindowPopupProperty, value);
//}

//private static readonly DependencyProperty WindowPopupProperty =
//    DependencyProperty.RegisterAttached("WindowPopup",
//        typeof(Popup), typeof(ClosePopupBehavior));
//    }
//}
