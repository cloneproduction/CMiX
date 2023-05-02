using System.Windows;
using System.Windows.Controls;

namespace CMiX.MVVM.ViewModels.Assets
{
    public class AssetItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AssetItemTextureTemplate { get; set; }
        public DataTemplate AssetItemGeometryTemplate { get; set; }
        public DataTemplate AssetItemDirectoryTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is AssetTexture)
                return AssetItemTextureTemplate;
            else if (item != null && item is AssetGeometry)
                return AssetItemGeometryTemplate;
            else if (item != null && item is AssetDirectory)
                return AssetItemDirectoryTemplate;
            else
                return null;
        }
    }
}