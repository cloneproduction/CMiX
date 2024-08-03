using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class Outliner : UserControl
    {
        public Outliner()
        {
            InitializeComponent();
        }

        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionPanelProperty =
        DependencyProperty.Register("SelectionPanel", typeof(FrameworkElement), typeof(Outliner), new UIPropertyMetadata(null));
        public FrameworkElement SelectionPanel
        {
            get { return (FrameworkElement)GetValue(SelectionPanelProperty); }
            set { SetValue(SelectionPanelProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Outliner));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        //public static readonly DependencyProperty TypeParameterProperty =
        //DependencyProperty.Register("TypeParameter", typeof(Type), typeof(Outliner));
        //public Type TypeParameter
        //{
        //    get { return (Type)GetValue(TypeParameterProperty); }
        //    set { SetValue(TypeParameterProperty, value); }
        //}
    }
}
