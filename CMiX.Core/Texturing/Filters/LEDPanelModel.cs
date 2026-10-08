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
        public BlendModel Blend { get; set; } = new();
        public ModulatableValueModel<float> PixelSize { get; set; } = ModulatableValueModel<float>.Of("Pixel Size", 10.0f);
        public ModulatableValueModel<float> MaskStagger { get; set; } = ModulatableValueModel<float>.Of("Mask Stagger", 0.0f);
        public ModulatableValueModel<float> MaskBorder { get; set; } = ModulatableValueModel<float>.Of("Mask Border", 0.0f);
        public ModulatableValueModel<float> MaskIntensity { get; set; } = ModulatableValueModel<float>.Of("Mask Intensity", 0.0f);
        public PrefabManagerModel ModulatorManager { get; set; } = new();
    }
}
