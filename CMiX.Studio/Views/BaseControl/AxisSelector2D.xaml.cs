using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class AxisSelector2D : UserControl
    {
        public AxisSelector2D()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty XIsCheckedProperty =
        DependencyProperty.Register("XIsChecked", typeof(bool), typeof(AxisSelector2D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool XIsChecked
        {
            get { return (bool)GetValue(XIsCheckedProperty); }
            set { SetValue(XIsCheckedProperty, value); }
        }

        public static readonly DependencyProperty YIsCheckedProperty =
        DependencyProperty.Register("YIsChecked", typeof(bool), typeof(AxisSelector2D), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool YIsChecked
        {
            get { return (bool)GetValue(YIsCheckedProperty); }
            set { SetValue(YIsCheckedProperty, value); }
        }
    }
}
