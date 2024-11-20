using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace CMiX.Studio.Views.BaseControl
{
    public partial class ColorSelector : UserControl
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(ControlPosition), typeof(ColorSelector),
                new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(ColorSelector),
            new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelector),
            new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }


        public ColorSelector()
        {
            InitializeComponent();
        }
    }
}
