// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Mapping
{
    public class TexturingPairProfile : ControlPairProfile
    {
        public TexturingPairProfile()
        {
            CreatePair<DiffuseTexture, DiffuseTextureModel>();
            CreatePair<SamplerState, SamplerStateModel>();
            CreatePair<TextureSourceSelector, TextureSourceSelectorModel>();
            CreatePair<Texture, TextureModel>();
            CreatePair<DiffuseTexture, DiffuseTextureModel>();
            CreatePair<MaskTexture, MaskTextureModel>();
        }
    }
}
