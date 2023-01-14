using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl.Panels
{
    public partial class BorderContainer : UserControl
    {
        public BorderContainer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(BorderContainer), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
    }
}
