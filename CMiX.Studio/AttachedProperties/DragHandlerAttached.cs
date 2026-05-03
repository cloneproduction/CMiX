// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Input;

namespace CMiX.Studio.AttachedProperties
{
    public static class DragHandler
    {
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.RegisterAttached(
                "IsPressed",
                typeof(bool),
                typeof(DragHandler),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static bool GetIsPressed(DependencyObject obj) => (bool)obj.GetValue(IsPressedProperty);
        public static void SetIsPressed(DependencyObject obj, bool value) => obj.SetValue(IsPressedProperty, value);

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(DragHandler),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element) return;

            if ((bool)e.NewValue)
            {
                element.PreviewMouseLeftButtonDown += OnMouseDown;
                element.PreviewMouseLeftButtonUp += OnMouseUp;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= OnMouseDown;
                element.PreviewMouseLeftButtonUp -= OnMouseUp;
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
            => ((UIElement)sender).SetValue(IsPressedProperty, true);

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
            => ((UIElement)sender).SetValue(IsPressedProperty, false);
    }
}
