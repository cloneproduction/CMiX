// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class CMiXListBox : ListView
    {
        Border Border { get; set; }
        Border Bd { get; set; }

        public override void OnApplyTemplate()
        {
            Border = GetTemplateChild("border") as Border;
            Bd = GetTemplateChild("Bd") as Border;

            if (Border != null)
            {
                Border.MouseMove += Border_MouseMove;
                Border.PreviewMouseLeftButtonDown += Border_PreviewMouseLeftButtonDown;
                Border.PreviewMouseLeftButtonUp += Border_PreviewMouseLeftButtonUp;

            }

            if(Bd != null)
            {
                Bd.Height = Bd.MinHeight;
                Bd.PreviewMouseWheel += Bd_PreviewMouseWheel;
            }
        }

        ///ScrollParentWhenAtMax
        private void Bd_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = GetVisualChild<ScrollViewer>(Bd);

            if (scrollViewer == null)
                return;

            var scrollPos = scrollViewer.ContentVerticalOffset;
            if ((scrollPos == scrollViewer.ScrollableHeight 
                    && e.Delta < 0)
                    || (scrollPos == 0 && e.Delta > 0))
            {
                e.Handled = true;
                var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                e2.RoutedEvent = UIElement.MouseWheelEvent;
                this.RaiseEvent(e2);
            }
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }


        Point initPoint = new();
        double currentHeight;

        private void Border_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).CaptureMouse();
            initPoint = e.GetPosition(this);
            currentHeight = Bd.ActualHeight;
        }

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!(sender as UIElement).IsMouseCaptureWithin)
                return;

            var p = e.GetPosition(this) - initPoint;
            var pos = p.Y + currentHeight;

            if (pos > 0)
                Bd.Height = pos;

            if (pos <= Bd.MinHeight)
                Bd.Height = Bd.MinHeight;
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).ReleaseMouseCapture();
        }
    }
}
