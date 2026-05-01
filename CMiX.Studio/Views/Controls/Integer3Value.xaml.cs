using System;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.Controls
{
    public partial class Integer3Value : UserControl
    {
        public Integer3Value()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(Integer3Value), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
    }
}
