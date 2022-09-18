// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraTransformModifierTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CameraLFOTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if (item != null)
            {
                if (item is CameraLFO)
                    dataTemplate = CameraLFOTemplate;
                //else if (item is LinearXYZ)
                //    dataTemplate = LinearXYZTemplate;
            }

            return dataTemplate;
        }
    }
}
