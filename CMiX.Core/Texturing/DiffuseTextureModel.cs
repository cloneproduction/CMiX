// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing
{
    public record DiffuseTextureModel : IControlModel
    {
        public Guid ID { get ; init; } = Guid.NewGuid();
        public PrefabManagerModel TextureManager { get; init; } = new();
        public PrefabManagerModel TextureFilterManager { get; init; } = new();
        public Transform2DModel Transform2D { get; init; } = new();
        public SamplerStateModel SamplerState { get; init; } = new();
    }
}
