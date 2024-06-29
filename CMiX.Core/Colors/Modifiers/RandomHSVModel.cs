// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Colors.Modifiers
{
    public class RandomHSVModel : IPrefabModel
    {
        public RandomHSVModel()
        {
            ID = Guid.NewGuid();
            Hue = new GenericValueModel<float>();
            Saturation = new GenericValueModel<float>();
            Value = new GenericValueModel<float>();
            Alpha = new GenericValueModel<float>();
            PrefabService = new PrefabServiceModel();
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Vector3Model HSV { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<float> Hue { get; set; }
        public GenericValueModel<float> Saturation { get; set; }
        public GenericValueModel<float> Value { get; set; }
        public GenericValueModel<float> Alpha { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
