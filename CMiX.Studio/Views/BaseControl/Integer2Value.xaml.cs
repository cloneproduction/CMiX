using System;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class Integer2Value : UserControl
    {
        public Integer2Value()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(Integer2Value), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
    }
}
