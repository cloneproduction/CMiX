using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class DiffuseTexture : UserControl
    {
        public DiffuseTexture()
        {
            InitializeComponent();
        }

        private void Expander_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
