// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record HSCBModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> Hue { get; set; } = ModulatableValueModel<float>.Of("Hue", 0.0f);
        public ModulatableValueModel<float> Saturation { get; set; } = ModulatableValueModel<float>.Of("Saturation", 1.0f);
        public ModulatableValueModel<float> Contrast { get; set; } = ModulatableValueModel<float>.Of("Contrast", 0.0f);
        public ModulatableValueModel<float> Brightness { get; set; } = ModulatableValueModel<float>.Of("Brightness", 0.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
