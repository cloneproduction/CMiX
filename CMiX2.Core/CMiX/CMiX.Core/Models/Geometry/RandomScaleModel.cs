// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RandomScaleModel : IModifierModel
    {
        public RandomScaleModel()
        {
            this.ID = Guid.NewGuid();
            Name = TransformModifierNames.RandomXY;
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);

            Visible = new BooleanValueModel(true);

            BeatModifierModel = new BeatModifierModel();
            CounterModel = new IntegerValueModel(1);
            EasingModel = new EasingModel();

            Scale = new Vector3Model();
            UniformXYZ = new FloatValueModel();

            Spread = new BooleanValueModel();
        }

        public Guid ID { get; set; }


        public GenericValueModel<ModifierMode> Mode { get; set; }
        public BooleanValueModel Visible { get; set; }

        public EasingModel EasingModel { get; set; }
        public IntegerValueModel CounterModel { get; set; }


        public BooleanValueModel RandomizeScale { get; set; }
        public Vector3Model Scale { get; set; }



        public BeatModifierModel BeatModifierModel { get; set; }
        public TransformModifierNames Name { get; set; }

        public BooleanValueModel Spread { get; internal set; }
        public FloatValueModel UniformXYZ { get; internal set; }
    }
}
