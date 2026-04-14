using System;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class IntegerValue : UserControl
    {
        public IntegerValue()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(IntegerValue), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(IntegerValue), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(IntegerValue),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
