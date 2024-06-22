// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CMiX.Studio.Behaviors
{
    public static class ClosePopupBehavior
    {
        public static ContentControl GetPopupContainer(DependencyObject obj)
        {
            return (ContentControl)obj.GetValue(PopupContainerProperty);
        }

        public static void SetPopupContainer(DependencyObject obj, ContentControl value)
        {
            obj.SetValue(PopupContainerProperty, value);
        }

        public static readonly DependencyProperty PopupContainerProperty =
            DependencyProperty.RegisterAttached("PopupContainer", typeof(ContentControl), typeof(ClosePopupBehavior), 
                new PropertyMetadata(OnPopupContainerChanged));

        private static void OnPopupContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)d;
            var contentControl = e.NewValue as ContentControl;

            popup.LostFocus += (sender, args) =>
            {
                var popup1 = (Popup)sender;
                popup.IsOpen = false;
                if (contentControl != null)
                    contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            };
            popup.Opened += (sender, args) =>
            {
                var popup1 = (Popup)sender;
                popup.Focus();
                SetWindowPopup(contentControl, popup1);
                //contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
                //contentControl.PreviewMouseDown += ContainerOnPreviewMouseDown;
            };
            popup.PreviewMouseUp += (sender, args) =>
            {
                popup.IsOpen = false;
                if (contentControl != null)
                    contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            };
            popup.MouseLeave += (sender, args) =>
            {
                popup.IsOpen = false;
                if (contentControl != null)
                    contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            };
            popup.Unloaded += (sender, args) =>
            {
                popup.IsOpen = false;
                if (contentControl != null)
                    contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            };
        }

        //This is really to handle touch panel, Not sure if it is needed since handle LostFocus.
        //Remove the contentControl stuff if it is not needed
        private static void ContainerOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var popup = GetWindowPopup((DependencyObject)sender);
            popup.IsOpen = false;
            ((FrameworkElement)sender).PreviewMouseUp -= ContainerOnPreviewMouseDown;
        }

        private static Popup GetWindowPopup(DependencyObject obj)
        {
            return (Popup)obj.GetValue(WindowPopupProperty);
        }

        private static void SetWindowPopup(DependencyObject obj, Popup value)
        {
            obj.SetValue(WindowPopupProperty, value);
        }

        private static readonly DependencyProperty WindowPopupProperty =
            DependencyProperty.RegisterAttached("WindowPopup",
                typeof(Popup), typeof(ClosePopupBehavior));
    }
}
