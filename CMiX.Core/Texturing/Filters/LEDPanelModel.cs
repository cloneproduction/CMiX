// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public record class LEDPanelModel : IPrefabModel, ITextureFilterModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<float> Control { get; set; } = new(1.0f);
        public GenericValueModel<float> PixelSize { get; set; } = new(10.0f);
        public GenericValueModel<float> MaskStagger { get; set; } = new(0.0f);
        public GenericValueModel<float> MaskBorder { get; set; } = new(0.0f);
        public GenericValueModel<float> MaskIntensity { get; set; } = new(0.0f);
        public BlendModel Blend { get; set; } = new();
        public List<ModulatableValueModel<float>> Bindables { get; set; } = new();
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
