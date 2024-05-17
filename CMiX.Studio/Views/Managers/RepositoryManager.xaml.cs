using System.Collections;
using System.Windows;
using System.Windows.Controls;

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
    }
}
