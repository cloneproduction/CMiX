// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;

namespace CMiX.Core.Presentations.ViewModels
{
    public interface ITexture : IIDObject
    {
        BooleanValue IsEnabled { get; set; }
        ModifierManager TextureModifierManager { get; set; }
        IntegerValue SelectedAssetType { get; set; }
        ModifierManager TextureTransformModifierManager { get; set; }
        SamplerState SamplerState { get; set; }
        TypeWriter TypeWriter { get; set; }
        VideoIn VideoIn { get; set; }
        VideoPlayer VideoPlayer { get; set; }
        ProceduralSelector ProceduralSelector { get; set; }
    }
}
