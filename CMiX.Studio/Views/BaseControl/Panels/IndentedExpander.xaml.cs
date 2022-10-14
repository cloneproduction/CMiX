using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CMiX.Studio.Views.BaseControl.Panels
{
    /// <summary>
    /// Interaction logic for IndentedExpander.xaml
    /// </summary>
    public partial class IndentedExpander : UserControl
    {
        public IndentedExpander()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IndentationProperty =
        DependencyProperty.Register("Indentation", typeof(int), typeof(IndentedExpander), new PropertyMetadata(0));
        public int Indentation
        {
            get { return (int)GetValue(IndentationProperty); }
            set { SetValue(IndentationProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(object), typeof(IndentedExpander), new PropertyMetadata());
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(IndentedExpander), new PropertyMetadata(false));
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
    }
}
