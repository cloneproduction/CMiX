// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentPanelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LayerPanelTemplate { get; set; }
        public DataTemplate CompositionPanelTemplate { get; set; }
        public DataTemplate ScenePanelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if(item != null)
            {
                if (item is Composition)
                    dataTemplate = CompositionPanelTemplate;
                else if (item is Layer)
                    dataTemplate = LayerPanelTemplate;
            }

            return dataTemplate;
        }
    }
}
