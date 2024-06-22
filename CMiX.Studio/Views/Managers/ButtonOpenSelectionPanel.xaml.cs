using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class ButtonOpenSelectionPanel : UserControl
    {
        public ButtonOpenSelectionPanel()
        {
            InitializeComponent();
        }

        public FrameworkElement SelectionPanel
        {
            get { return (FrameworkElement)GetValue(SelectionPanelProperty); }
            set { SetValue(SelectionPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionPanelProperty =
        DependencyProperty.Register("SelectionPanel", typeof(FrameworkElement), typeof(ButtonOpenSelectionPanel), new UIPropertyMetadata(null));
    }
}
