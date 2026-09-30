// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record KuwaharaModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> Radius { get; set; } = new(1.0f);
        public GenericValueModel<KuwaharaType> Type { get; set; } = new(KuwaharaType.Standard);
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public List<ModulatableValueModel<float>> Bindables { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
