// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels.Assets
{
    public class AssetTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AssetTextureTemplate { get; set; }
        public DataTemplate AssetGeometryTemplate { get; set; }
        public DataTemplate AssetDirectoryTemplate { get; set; }
        public DataTemplate AssetVideoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item == null)
                return null;

            if (item is AssetImage)
                return AssetTextureTemplate;
            else if (item is AssetGeometry)
                return AssetGeometryTemplate;
            else if (item is AssetVideo)
                return AssetVideoTemplate;
            else if (item is AssetDirectory)
                return AssetDirectoryTemplate;
            else
                return null;
        }
    }
}
