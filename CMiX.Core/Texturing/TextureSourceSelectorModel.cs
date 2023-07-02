// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class TextureSourceSelectorModel : IControlModel
    {
        public TextureSourceSelectorModel()
        {
            ID = Guid.NewGuid();
            ProceduralName = new GenericValueModel<TextureSourceName>(TextureSourceName.BubbleNoise);
            Gradient = new GradientModel();
            BubbleNoise = new BubbleNoiseModel();
            Image = new ImageModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<TextureSourceName> ProceduralName { get; set; }
        public GradientModel Gradient { get; set; }
        public BubbleNoiseModel BubbleNoise { get; set; }
        public ImageModel Image { get; set; }
    }
}
