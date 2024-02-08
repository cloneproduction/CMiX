// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomXYZModel : IModifierModel
    {
        public RandomXYZModel()
        {
            ID = Guid.NewGuid();
            Visible = new GenericValueModel<bool>(true);
            BeatModifier = new BeatModifierModel();
            Easing = new EasingModel();
            RandomizeLocation = new GenericValueModel<bool>(true);
            Location = new Vector3Model();
            RandomizeScale = new GenericValueModel<bool>(true);
            Uniform = new GenericValueModel<float>();
            Scale = new Vector3Model();
            RandomizeRotation = new GenericValueModel<bool>(true);
            Rotation = new Vector3Model();
            ModifierModeSelector = new ModifierModeSelectorModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<bool> RandomizeLocation { get; set; }
        public Vector3Model Location { get; set; }
        public GenericValueModel<bool> RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }
        public GenericValueModel<bool> RandomizeRotation { get; set; }
        public Vector3Model Rotation { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public ModifierModeSelectorModel ModifierModeSelector { get; set; }
    }
}
