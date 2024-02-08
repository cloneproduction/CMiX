// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Texturing
{
    public interface ITexture : IControl
    {
        PrefabManager TextureManager { get; set; }
        TransformTexture TransformTexture { get; set; }
        SamplerState SamplerState { get; set; }
    }
}
