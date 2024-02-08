// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Texturing;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class LayerSettings
    {
        public LayerSettings(GenericValue<float> opacity,
                             GenericValue<string> backgroundColor, 
                             GenericValue<BlendModeEnum> blendMode, 
                             AmbientOcclusion ambientOcclusion) 
        {
            Opacity = opacity;
            BackgroundColor = backgroundColor;
            BlendMode = blendMode;
            AmbientOcclusion = ambientOcclusion;
        }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }
    }
}
