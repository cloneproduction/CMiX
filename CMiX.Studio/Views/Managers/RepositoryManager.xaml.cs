using System.Collections;
using System.Windows;
using System.Windows.Controls;
using CMiX.Studio.Views.Managers;

namespace CMiX.Studio.Views
{
    public partial class RepositoryManager : UserControl
    {
        public RepositoryManager()
        {
            InitializeComponent();
        }

        public FrameworkElement SelectionPanel
        {
            get { return (FrameworkElement)GetValue(SelectionPanelProperty); }
            set { SetValue(SelectionPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionPanelProperty =
        DependencyProperty.Register("SelectionPanel", typeof(FrameworkElement), typeof(RepositoryManager), new UIPropertyMetadata(null));



        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(RepositoryManager), new PropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(RepositoryManager), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
    }
}
