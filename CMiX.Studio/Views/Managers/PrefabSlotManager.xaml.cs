using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class PrefabSlotManager : UserControl
    {
        public PrefabSlotManager()
        {
            InitializeComponent();
            prefabListBox.SelectionChanged += (s, e) =>
            {
                Debug.WriteLine($"SelectionChanged: SelectedItem={prefabListBox.SelectedItem?.GetType().Name}, IsFocused={prefabListBox.IsFocused}, IsKeyboardFocusWithin={prefabListBox.IsKeyboardFocusWithin}");
            };
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
    }
}
