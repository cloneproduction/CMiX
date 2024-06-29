// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public class LFOUVModel : IPrefabModel
    {
        public LFOUVModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifier = new BeatModifierModel();
            PingPong = new GenericValueModel<bool>();
            XAxis = new GenericValueModel<bool>(false);
            YAxis = new GenericValueModel<bool>(true);
            ZAxis = new GenericValueModel<bool>(false);
            TransformType = new GenericValueModel<TransformType>(Transformation.TransformType.Translate);
            Easing = new EasingModel();
            From = new GenericValueModel<float>(-1.0f);
            To = new GenericValueModel<float>(1.0f);
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<TransformType> TransformType { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public GenericValueModel<bool> XAxis { get; set; }
        public GenericValueModel<bool> YAxis { get; set; }
        public GenericValueModel<bool> ZAxis { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<float> From { get; set; }
        public GenericValueModel<float> To { get; set; }
        public SamplerStateModel SamplerState { get; set; }

    }
}
