// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
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
            AssociatedObject.MouseLeave += OnMouseLeave;
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.Opened += OnOpened;
            AssociatedObject.Closed += OnClosed;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeave -= OnMouseLeave;
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.Opened -= OnOpened;
            AssociatedObject.Closed -= OnClosed;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            RemoveParentWindowHandlers();
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Button)
                ClosePopup();
        }

        private Window ParentWindow => Window.GetWindow(AssociatedObject);

        private void AddParentWindowHandlers()
        {
            var window = ParentWindow;
            if (window == null) return;
            Mouse.AddPreviewMouseDownHandler(window, OnParentMouseDown);
            Mouse.AddPreviewMouseMoveHandler(window, OnParentMouseMove);
        }

        private void RemoveParentWindowHandlers()
        {
            var window = ParentWindow;
            if (window == null) return;
            Mouse.RemovePreviewMouseDownHandler(window, OnParentMouseDown);
            Mouse.RemovePreviewMouseMoveHandler(window, OnParentMouseMove);
        }

        private void ClosePopup()
        {
            if (AssociatedObject.PlacementTarget is ToggleButton tb)
                tb.IsChecked = false;
            AssociatedObject.IsOpen = false;
        }

        private void OnOpened(object sender, EventArgs e) => AddParentWindowHandlers();

        private void OnClosed(object sender, EventArgs e)
        {
            RemoveParentWindowHandlers();
            Application.Current.MainWindow?.Focus();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e) => RemoveParentWindowHandlers();
        private void OnMouseLeave(object sender, MouseEventArgs e) => AddParentWindowHandlers();

        private void OnParentMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
            e.Handled = true;
        }

        private void OnParentMouseMove(object sender, MouseEventArgs e)
        {
            if (AssociatedObject.Child == null) return;

            var size = AssociatedObject.Child.RenderSize;
            var pos = e.GetPosition(AssociatedObject.Child);

            if (pos.X < -64 || pos.Y < -64 ||
                pos.X > size.Width + 64 || pos.Y > size.Height + 64)
            {
                ClosePopup();
                RemoveParentWindowHandlers();
            }
        }
    }
}
