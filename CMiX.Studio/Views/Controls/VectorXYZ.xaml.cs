using System;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.Controls
{
    public partial class VectorXYZ : UserControl
    {
        public VectorXYZ()
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

        public static readonly DependencyProperty ZProperty =
        DependencyProperty.Register("Z", typeof(float), typeof(VectorXYZ), new FrameworkPropertyMetadata(0.0f, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public float Z
        {
            get { return (float)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
    }
}
