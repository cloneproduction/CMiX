// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Texturing
{
    public interface ITexture : IModifiable
    {
        BooleanValue IsEnabled { get; set; }
        IntegerValue SelectedAssetType { get; set; }
        SamplerState SamplerState { get; set; }
        TypeWriter TypeWriter { get; set; }
        VideoIn VideoIn { get; set; }
        VideoPlayer VideoPlayer { get; set; }
        TextureSourceSelector TextureSourceSelector { get; set; }
        TransformTexture TransformTexture { get; set; }
    }
}
