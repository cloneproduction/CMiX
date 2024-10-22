// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Colors.Modifiers
{
    public record RandomHSVModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public Vector3Model HSV { get; set; } = new(0.0f, 0.0f, 0.0f);
        public GenericValueModel<ColorMode> ColorMode { get; set; } = new(Colors.ColorMode.HSV);
        public GenericValueModel<float> Hue { get; set; } = new(0.0f);
        public GenericValueModel<float> Saturation { get; set; } = new(0.0f);
        public GenericValueModel<float> Value { get; set; } = new(0.0f);
        public GenericValueModel<float> Alpha { get; set; } = new(0.0f);
        public ModifierModeSelectorModel ModifierModeSelector { get; set; } = new();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
    }
}
