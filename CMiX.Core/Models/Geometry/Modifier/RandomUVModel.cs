// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RandomUVModel : ITextureFilterModel
    {
        public RandomUVModel()
        {
            this.ID = Guid.NewGuid();
            Name = TextureFilterName.RandomUV;

            Visible = new BooleanValueModel(true);

            BeatModifierModel = new BeatModifierModel();
            CounterModel = new IntegerValueModel(1);
            EasingModel = new EasingModel();

            RandomizeLocation = new BooleanValueModel(true);
            Location = new Vector2Model();

            RandomizeScale = new BooleanValueModel(true);
            Scale = new Vector2Model();
            Uniform = new FloatValueModel();

            RandomizeRotation = new BooleanValueModel(true);
            Rotation = new FloatValueModel();

            SamplerState = new SamplerStateModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }



        public BooleanValueModel Visible { get; set; }

        public EasingModel EasingModel { get; set; }
        public IntegerValueModel CounterModel { get; set; }

        public BooleanValueModel RandomizeLocation { get; set; }
        public Vector2Model Location { get; set; }

        public BooleanValueModel RandomizeScale { get; set; }
        public Vector2Model Scale { get; set; }
        public FloatValueModel Uniform { get; set; }

        public BooleanValueModel RandomizeRotation { get; set; }
        public FloatValueModel Rotation { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }
        public TextureFilterName Name { get; set; }

        public int Count { get; set; }
        public BooleanValueModel Spread { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
    }
}
