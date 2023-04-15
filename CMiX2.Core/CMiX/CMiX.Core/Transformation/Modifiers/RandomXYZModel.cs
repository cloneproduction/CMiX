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
            Name = TransformModifierNames.RandomXY;
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            Counter = new IntegerValueModel(1);
            Easing = new EasingModel();
            RandomizeLocation = new BooleanValueModel(true);
            Location = new Vector3Model();
            RandomizeScale = new BooleanValueModel(true);
            Uniform = new FloatValueModel();
            Scale = new Vector3Model();
            RandomizeRotation = new BooleanValueModel(true);
            Rotation = new Vector3Model();
        }

        public Guid ID { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public BooleanValueModel Visible { get; set; }
        public EasingModel Easing { get; set; }
        public IntegerValueModel Counter { get; set; }
        public BooleanValueModel RandomizeLocation { get; set; }
        public Vector3Model Location { get; set; }
        public BooleanValueModel RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }
        public BooleanValueModel RandomizeRotation { get; set; }
        public Vector3Model Rotation { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public TransformModifierNames Name { get; set; }
        public FloatValueModel Uniform { get; internal set; }
    }
}
