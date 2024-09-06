// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class LayerSettings : IControl
    {
        public LayerSettings(GenericValue<float> opacity,
                             GenericValue<string> backgroundColor, 
                             GenericValue<BlendModeEnum> blendMode) 
        {
            Opacity = opacity;
            BackgroundColor = backgroundColor;
            BlendMode = blendMode;
        }

        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }


        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
