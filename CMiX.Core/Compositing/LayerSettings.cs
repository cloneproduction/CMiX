// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class LayerSettings : IControl
    {
        public LayerSettings(GenericValue<float> opacity,
                             GenericValue<string> backgroundColor, 
                             GenericValue<BlendModeEnum> blendMode, 
                             AmbientOcclusion ambientOcclusion,
                             LocalReflection localReflection) 
        {
            Opacity = opacity;
            BackgroundColor = backgroundColor;
            BlendMode = blendMode;
            AmbientOcclusion = ambientOcclusion;
            LocalReflection = LocalReflection;
        }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public LocalReflection LocalReflection { get; set; }
        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
