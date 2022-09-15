using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class PrefabSlotManager : UserControl
    {
        public PrefabSlotManager()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
                typeof(PrefabSlotManager), new PropertyMetadata(null));
    }
}
