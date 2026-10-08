// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record DisplaceModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel TextureSelector { get; set; } = new();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableVector2Model Offset { get; set; } = ModulatableVector2Model.Of(0.5f, 0.5f);
        public ModulatableVector2Model OffsetScale { get; set; } = ModulatableVector2Model.Of(0.1f, 0.1f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
