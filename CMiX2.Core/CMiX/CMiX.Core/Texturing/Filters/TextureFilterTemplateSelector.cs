// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace CMiX.Core.Texturing.Filters
{
    public class TextureFilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HSCBTemplate { get; set; }
        public DataTemplate InvertTemplate { get; set; }
        public DataTemplate BlurTemplate { get; set; }
        public DataTemplate EdgeTemplate { get; set; }
        public DataTemplate TransformTextureTemplate { get; set; }
        public DataTemplate PixelateTemplate { get; set; }
        public DataTemplate EchoTemplate { get; set; }
        public DataTemplate FeedbackTemplate { get; set; }
        public DataTemplate TriColorTemplate { get; set; }
        public DataTemplate RandomUVTemplate { get; set; }
        public DataTemplate LFOUVTemplate { get; set; }

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
                if (item is Pixelate)
                    dataTemplate = PixelateTemplate;
                if (item is Echo)
                    dataTemplate = EchoTemplate;
                if (item is Feedback)
                    dataTemplate = FeedbackTemplate;
                if (item is TriColor)
                    dataTemplate = TriColorTemplate;
                if (item is RandomUV)
                    dataTemplate = RandomUVTemplate;
                if (item is LFOUV)
                    dataTemplate = LFOUVTemplate;
            }

            return dataTemplate;
        }
    }
}
