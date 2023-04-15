// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public class ProceduralSelectorModel : IModel
    {
        public ProceduralSelectorModel()
        {
            ID = Guid.NewGuid();
            ProceduralName = new GenericValueModel<TextureSourceName>(TextureSourceName.Gradient);
            Gradient = new GradientModel();
            BubbleNoise = new BubbleNoiseModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<TextureSourceName> ProceduralName { get; internal set; }
        public GradientModel Gradient { get; internal set; }
        public BubbleNoiseModel BubbleNoise { get; internal set; }
    }
}
