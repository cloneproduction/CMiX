// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomScaleModel : IModifierModel
    {
        public RandomScaleModel()
        {
            ID = Guid.NewGuid();
            Visible = new GenericValueModel<bool>(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Scale = new Vector3Model();
            UniformXYZ = new GenericValueModel<float>();
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<bool> RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<float> UniformXYZ { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
