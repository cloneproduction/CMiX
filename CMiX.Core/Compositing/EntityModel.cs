// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;

namespace CMiX.Core.Compositing
{
    public record EntityModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public MeshModel Mesh { get; set; } = new();
        public TransformSRTModel TransformSRT { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
        public PrefabSelectorModel MaterialSelector { get; set; } = new();
        public ColorModel Color { get; set; } = new();
        public TextureModel Texture { get; set; } = new();
        public MaterialModel Material { get; set; } = new();
    }
}
