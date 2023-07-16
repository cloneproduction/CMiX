using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public partial class ModelEntity : UserControl
    {
        public ModelEntity()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            CMiX.Core.Materials.Material? game = e.Item as CMiX.Core.Materials.Material;
            
            if (game != null)
            {
                e.Accepted = true;
            }

            e.Accepted = false;
        }
    }
}
