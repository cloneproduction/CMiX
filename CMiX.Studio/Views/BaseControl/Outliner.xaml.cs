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

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Outliner));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty TypeParameterProperty =
        DependencyProperty.Register("TypeParameter", typeof(Type), typeof(Outliner));
        public Type TypeParameter
        {
            get { return (Type)GetValue(TypeParameterProperty); }
            set { SetValue(TypeParameterProperty, value); }
        }
    }
}
