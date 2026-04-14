using System;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class ToggleButton : UserControl
    {
        public ToggleButton()
        {
            InitializeComponent();
            InitializeComponent();
            labelBorder.MouseLeftButtonDown += (s, e) =>
            {
                toggleButton.IsChecked = !toggleButton.IsChecked;
                e.Handled = true;
            };
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(ToggleButton), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
    }
}
