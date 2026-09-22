// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;

namespace CMiX.Core.Compositing
{
    public record FullscreenQuadModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public ColorModel Color { get; set; } = new();
        public TextureModel Texture { get; set; } = new();
        public Transform2DModel TransformTexture { get; set; } = new();
    }
}
