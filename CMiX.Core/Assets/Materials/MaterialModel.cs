// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Materials
{
    public record MaterialModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; } = new();
        public MaskTextureModel MaskTexture { get; set; } = new();
        public MaterialSettingsModel MaterialSettings { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public PrefabManagerModel ExplodeTriangleTextureManager { get; set; } = new();
        public GenericValueModel<float> ExplodeStrength { get; set; } = new(0.6f);
        public PrefabManagerModel ModifierManager { get; set; } = new();
    }
}
