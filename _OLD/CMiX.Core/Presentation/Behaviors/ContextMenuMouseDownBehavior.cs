// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace CMiX.Core.Presentation.Behaviors
{
    public class ContextMenuMouseDownBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += this.OnWindowLoaded;
            this.AssociatedObject.Unloaded += this.OnWindowUnloaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            this.AssociatedObject.MouseRightButtonUp += AssociatedObject_MouseRightButtonUp;
        }

        private void AssociatedObject_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Control fe && fe.ContextMenu != null)
            {
                if (fe.ContextMenu != null)
                {
                    // If we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click. It
                    // seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control? So we have to set
                    // up ContextMenu.DataContext manually here.
                    if (fe.ContextMenu?.DataContext == null)
                    {
                        fe.ContextMenu?.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
                    }

                    fe.ContextMenu.IsOpen = false;
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObject_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Control fe && fe.ContextMenu != null)
            {
                if (fe.ContextMenu != null)
                {
                    // If we use binding in our context menu, then it's DataContext won't be set when we show the menu on left click. It
                    // seems setting DataContext for ContextMenu is hardcoded in WPF when user right clicks on a control? So we have to set
                    // up ContextMenu.DataContext manually here.
                    if (fe.ContextMenu?.DataContext == null)
                    {
                        fe.ContextMenu?.SetBinding(FrameworkElement.DataContextProperty, new Binding { Source = fe.DataContext });
                    }

                    fe.ContextMenu.IsOpen = true;
                }
            }
        }

        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            this.AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseRightButtonUp;// Cannot override OnDetached(), as this is not called on Dispose. Known issue in WPF.
        }
    }
}
