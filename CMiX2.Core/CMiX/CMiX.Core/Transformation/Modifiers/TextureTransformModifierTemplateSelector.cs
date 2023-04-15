// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Transformation.Modifiers
{
    public class TextureTransformModifierTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RandomXYTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if (item != null)
            {
                if (item is RandomUV)
                    dataTemplate = RandomXYTemplate;

            }

            return dataTemplate;
        }
    }
}
