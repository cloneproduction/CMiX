// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class ListBox : UserControl
    {
        public ListBox()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)prefabListBox.Items).CollectionChanged += Handler;
        }

        private void Handler(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(prefabListBox) > 0)
            {
                if (VisualTreeHelper.GetChild(prefabListBox, 0) is UIElement contentVisual)
                {
                    var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(contentVisual, 0);
                    scrollViewer.ScrollToBottom();
                }
            }
        }

        Point initPoint = new();
        double currentHeight;

        private void Border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!(sender as UIElement).IsMouseCaptureWithin)
                return;

            var p = e.GetPosition(this) - initPoint;
            var pos = p.Y + currentHeight;

            if (pos > 0)
                prefabListBox.Height = pos;

            if (prefabListBox.Height <= prefabListBox.MinHeight)
                prefabListBox.Height = prefabListBox.MinHeight;
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).CaptureMouse();
            initPoint = e.GetPosition(this);
            currentHeight = prefabListBox.Height;
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).ReleaseMouseCapture();
        }
    }
}
