using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

namespace CMiX.Studio.Views
{
    public partial class PrefabSlotManager : UserControl
    {
        public PrefabSlotManager()
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
                    //this.Focus();
                }
            }
        }

        public FrameworkElement SelectionPanel
        {
            get { return (FrameworkElement)GetValue(SelectionPanelProperty); }
            set { SetValue(SelectionPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionPanelProperty =
        DependencyProperty.Register("SelectionPanel", typeof(FrameworkElement), typeof(PrefabSlotManager), new UIPropertyMetadata(null));


        public FrameworkElement ItemTemplate
        {
            get { return (FrameworkElement)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register("ItemTemplate", typeof(FrameworkElement), typeof(PrefabSlotManager), new UIPropertyMetadata(null));



        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PrefabSlotManager), new PropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        Point initPoint = new Point();
        double currentHeight;

        private void Button_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

        private void Button_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).CaptureMouse();
            initPoint = e.GetPosition(this);
            currentHeight = prefabListBox.Height;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as UIElement).ReleaseMouseCapture();
        }

        private void prefabListBox_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void ListBoxItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
