// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record HalftoneModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<HalftoneMode> Mode { get; set; } = new(HalftoneMode.Pixels);
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> NumberOfTiles { get; set; } = ModulatableValueModel<float>.Of("Tile Count", 48.0f);
        public ModulatableValueModel<float> DotSize { get; set; } = ModulatableValueModel<float>.Of("Dot Size", 0.01f);
        public ModulatableValueModel<float> Softness { get; set; } = ModulatableValueModel<float>.Of("Softness", 1.35f);
        public ModulatableValueModel<float> Brightness { get; set; } = ModulatableValueModel<float>.Of("Brightness", 1.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();

    }
}
