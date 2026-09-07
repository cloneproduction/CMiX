// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing
{
    public interface ITexture
    {
        PrefabManager TextureManager { get; set; }
        Transform2D Transform2D { get; set; }
        SamplerState SamplerState { get; set; }
    }
}
