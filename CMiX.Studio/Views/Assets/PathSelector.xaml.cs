using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Studio.Services;

namespace CMiX.Studio.Views
{
    public partial class PathSelector : UserControl
    {
        public PathSelector()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AssetSelector assetSelector)
                GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(pathComboBox, new AssetSelectorDropHandler(assetSelector));
        }


        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
                return;

            e.Handled = true;
        }

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PathSelector), new FrameworkPropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(PathSelector), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
    }
}
