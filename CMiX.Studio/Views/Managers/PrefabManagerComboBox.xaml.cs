using CMiX.Studio.Views.BaseControl;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.Managers
{
    public partial class PrefabManagerComboBox : UserControl
    {
        public PrefabManagerComboBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata(null));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty AddItemCommandProperty =
        DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionChangedCommandProperty =
        DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ICommand SelectionChangedCommand
        {
            get { return (ICommand)GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        public static readonly DependencyProperty SelectionChangedCommandParameterProperty =
        DependencyProperty.Register("SelectionChangedCommandParameter", typeof(object), typeof(PrefabManagerComboBox), new FrameworkPropertyMetadata());
        public object SelectionChangedCommandParameter
        {
            get { return (object)GetValue(SelectionChangedCommandParameterProperty); }
            set { SetValue(SelectionChangedCommandParameterProperty, value); }
        }
    }
}
