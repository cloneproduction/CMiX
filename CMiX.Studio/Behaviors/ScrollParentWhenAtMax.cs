using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace CMiX.Studio.Behaviors
{
    public class ScrollParentWhenAtMax : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseWheel -= PreviewMouseWheel;
            base.OnDetaching();
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = GetVisualChild<ScrollViewer>(this.AssociatedObject);
            var scrollPos = scrollViewer.ContentVerticalOffset;
            if ((scrollPos == scrollViewer.ScrollableHeight && e.Delta < 0)
                || (scrollPos == 0 && e.Delta > 0))
            {
                e.Handled = true;
                var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                e2.RoutedEvent = UIElement.MouseWheelEvent;
                AssociatedObject.RaiseEvent(e2);
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
    }
}
