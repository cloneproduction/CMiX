using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class Material : UserControl
    {
        public Material()
        {
            InitializeComponent();
        }

        private void Expander_RequestBringIntoView(object sender, System.Windows.RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
