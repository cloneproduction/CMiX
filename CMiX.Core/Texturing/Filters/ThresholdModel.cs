// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record ThresholdModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public GenericValueModel<string> Foreground { get; set; } = new("#FFFFFFFF");
        public GenericValueModel<string> Background { get; set; } = new("#FF000000");
        public GenericValueModel<bool> Antialiasing { get; set; } = new(true);
        public GenericValueModel<bool> Invert { get; set; } = new(false);
        public BlendModel Blend { get; set; } = new();
        public List<ModulatableValueModel<float>> Bindables { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
