// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public class RandomUVModel : IPrefabModel
    {
        public RandomUVModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifierManager = new PrefabManagerModel();
            Location = new Vector2Model();
            Scale = new Vector2Model(0.0f, 0.0f);
            Uniform = new GenericValueModel<float>();
            Rotation = new GenericValueModel<float>();
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public Vector2Model Location { get; set; }
        public Vector2Model Scale { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public GenericValueModel<float> Rotation { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
