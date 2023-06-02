// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;

namespace CMiX.Core.Transformation.Modifiers
{
    public class RandomUVModel : IModifierModel
    {
        public RandomUVModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.RandomUV;
            Visible = new BooleanValueModel(true);
            BeatModifier = new BeatModifierModel();
            CounterModel = new IntegerValueModel(1);
            Easing = new EasingModel();
            RandomizeLocation = new BooleanValueModel(true);
            Location = new Vector2Model();
            RandomizeScale = new BooleanValueModel(true);
            Scale = new Vector2Model();
            Uniform = new FloatValueModel();
            RandomizeRotation = new BooleanValueModel(true);
            Rotation = new FloatValueModel();
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visible { get; set; }
        public EasingModel Easing { get; set; }
        public IntegerValueModel CounterModel { get; set; }
        public BooleanValueModel RandomizeLocation { get; set; }
        public Vector2Model Location { get; set; }
        public BooleanValueModel RandomizeScale { get; set; }
        public Vector2Model Scale { get; set; }
        public FloatValueModel Uniform { get; set; }
        public BooleanValueModel RandomizeRotation { get; set; }
        public FloatValueModel Rotation { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public TextureFilterName Name { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
