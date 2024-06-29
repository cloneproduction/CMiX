// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class RandomUVModel : IPrefabModel
    {
        public RandomUVModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifier = new BeatModifierModel();
            CounterModel = new GenericValueModel<int>(1);
            Easing = new EasingModel();
            RandomizeLocation = new GenericValueModel<bool>(true);
            Location = new Vector2Model();
            RandomizeScale = new GenericValueModel<bool>(true);
            Scale = new Vector2Model();
            Uniform = new GenericValueModel<float>();
            RandomizeRotation = new GenericValueModel<bool>(true);
            Rotation = new GenericValueModel<float>();
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<int> CounterModel { get; set; }
        public GenericValueModel<bool> RandomizeLocation { get; set; }
        public Vector2Model Location { get; set; }
        public GenericValueModel<bool> RandomizeScale { get; set; }
        public Vector2Model Scale { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public GenericValueModel<bool> RandomizeRotation { get; set; }
        public GenericValueModel<float> Rotation { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
