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
    /// Interaction logic for VectorXY.xaml
    /// </summary>
    public partial class VectorXY : UserControl
    {
        public VectorXY()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(VectorXYZ), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty XProperty =
        DependencyProperty.Register("X", typeof(float), typeof(VectorXYZ), new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public float X
        {
            get { return (float)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty YProperty =
        DependencyProperty.Register("Y", typeof(float), typeof(VectorXYZ), new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public float Y
        {
            get { return (float)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
    }
}
