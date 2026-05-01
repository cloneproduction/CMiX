using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.Controls.Panels
{
    public partial class IndentedExpander : UserControl
    {
        public IndentedExpander()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IndentationProperty =
            DependencyProperty.Register(
                "Indentation", 
                typeof(int), 
                typeof(IndentedExpander), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(IndentedExpander), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }
    }
}
