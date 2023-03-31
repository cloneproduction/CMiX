// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Animation;
using CMiX.Core.Presentations.Beat;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class RandomXYZModel : IModifierModel
    {
        public RandomXYZModel()
        {
            ID = Guid.NewGuid();
            Name = TransformModifierNames.RandomXY;
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);

            Visible = new BooleanValueModel(true);

            BeatModifierModel = new BeatModifierModel();
            CounterModel = new IntegerValueModel(1);
            EasingModel = new EasingModel();

            RandomizeLocation = new BooleanValueModel();
            Location = new Vector3Model();


            RandomizeScale = new BooleanValueModel();
            Uniform = new FloatValueModel();
            Scale = new Vector3Model();

            RandomizeRotation = new BooleanValueModel();
            Rotation = new Vector3Model();

            Spread = new BooleanValueModel();
        }

        public Guid ID { get; set; }


        public GenericValueModel<ModifierMode> Mode { get; set; }
        public BooleanValueModel Visible { get; set; }

        public EasingModel EasingModel { get; set; }
        public IntegerValueModel CounterModel { get; set; }

        public BooleanValueModel RandomizeLocation { get; set; }
        public Vector3Model Location { get; set; }

        public BooleanValueModel RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }

        public BooleanValueModel RandomizeRotation { get; set; }
        public Vector3Model Rotation { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }
        public TransformModifierNames Name { get; set; }

        public int Count { get; set; }
        public BooleanValueModel Spread { get; internal set; }
        public FloatValueModel Uniform { get; internal set; }
    }
}
