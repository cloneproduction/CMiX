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

namespace CMiX.Studio.Views.BaseControl
{
    /// <summary>
    /// Interaction logic for AxisSelector3D.xaml
    /// </summary>
    public partial class AxisSelector3D : UserControl
    {
        public AxisSelector3D()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(AxisSelector3D), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty XIsCheckedProperty =
        DependencyProperty.Register("XIsChecked", typeof(bool), typeof(AxisSelector3D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool XIsChecked
        {
            get { return (bool)GetValue(XIsCheckedProperty); }
            set { SetValue(XIsCheckedProperty, value); }
        }

        public static readonly DependencyProperty YIsCheckedProperty =
        DependencyProperty.Register("YIsChecked", typeof(bool), typeof(AxisSelector3D), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool YIsChecked
        {
            get { return (bool)GetValue(YIsCheckedProperty); }
            set { SetValue(YIsCheckedProperty, value); }
        }

        public static readonly DependencyProperty ZIsCheckedProperty =
        DependencyProperty.Register("ZIsChecked", typeof(bool), typeof(AxisSelector3D), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ZIsChecked
        {
            get { return (bool)GetValue(ZIsCheckedProperty); }
            set { SetValue(ZIsCheckedProperty, value); }
        }
    }
}
