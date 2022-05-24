// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProjectTemplate { get; set; }
        public DataTemplate LayerTemplate { get; set; }
        public DataTemplate SceneTemplate { get; set; }
        public DataTemplate PointLightTemplate { get; set; }
        public DataTemplate SpotLightTemplate { get; set; }

        public DataTemplate CompositionTemplate { get; set; }
        public DataTemplate MeshTemplate { get; set; }
        public DataTemplate MeshEntityTemplate { get; set; }
        public DataTemplate LightEntityTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if(item != null)
            {
                if (item is Project)
                    dataTemplate = ProjectTemplate;
                else if (item is Composition)
                    dataTemplate = CompositionTemplate;
                else if (item is Layer)
                    dataTemplate = LayerTemplate;


                else if (item is Mesh)
                    dataTemplate = MeshEntityTemplate;
                else if (item is LightEntity)
                    dataTemplate = LightEntityTemplate;
            }

            return dataTemplate;         
        }
    }
}
