using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class ModifierManager : UserControl
    {
        public ModifierManager()
        {
            InitializeComponent();
        }


        // Using a DependencyProperty as the backing store for InnerContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionPanelProperty =
        DependencyProperty.Register("SelectionPanel", typeof(FrameworkElement), typeof(ModifierManager), new UIPropertyMetadata(null));
        public FrameworkElement SelectionPanel
        {
            get { return (FrameworkElement)GetValue(SelectionPanelProperty); }
            set { SetValue(SelectionPanelProperty, value); }
        }
    }
}
