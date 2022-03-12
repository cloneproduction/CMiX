// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TextureFilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HSCBTemplate { get; set; }
        public DataTemplate InvertTemplate { get; set; }
        public DataTemplate BlurTemplate { get; set; }
        public DataTemplate EdgeTemplate { get; set; }
        public DataTemplate TransformTextureTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if (item != null)
            {
                if (item is HSCB)
                    dataTemplate = HSCBTemplate;
                if (item is Invert)
                    dataTemplate = InvertTemplate;
                if (item is Blur)
                    dataTemplate = BlurTemplate;
                if (item is Edge)
                    dataTemplate = EdgeTemplate;
                if (item is TransformTexture)
                    dataTemplate = TransformTextureTemplate;
            }

            return dataTemplate;
        }
    }
}
