using System.Windows.Controls;
using CMiX.Studio.Services;
using GongSolutions.Wpf.DragDrop;

namespace CMiX.Studio.Views
{
    public partial class AssetManager : UserControl
    {
        public AssetManager()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not Core.Assets.AssetManager assetManager)
                return;

            var dropHandler = new AssetManagerDropHandler(assetManager);
            DragDrop.SetDropHandler(dropBorder, dropHandler);
            DragDrop.SetDropHandler(imagesListBox, dropHandler);
            DragDrop.SetDropHandler(videosListBox, dropHandler);
            DragDrop.SetDropHandler(geometriesListBox, dropHandler);
        }
    }
}
