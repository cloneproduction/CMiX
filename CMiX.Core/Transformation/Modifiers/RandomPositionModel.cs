// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomPositionModel : IModifierModel
    {
        public RandomPositionModel()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            Location = new Vector3Model();
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public EasingModel Easing { get; set; }
        public Vector3Model Location { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
