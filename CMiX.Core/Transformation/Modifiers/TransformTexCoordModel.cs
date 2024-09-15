// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;

namespace CMiX.Core.Transformation.Modifiers
{
    public class TransformTexCoordModel : IPrefabModel
    {
        public TransformTexCoordModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Location = new Vector2Model(0.0f, 0.0f);
            Scale = new Vector2Model(1.0f, 1.0f);
            Uniform = new GenericValueModel<float>(1.0f);
            Rotation = new GenericValueModel<float>(0.0f);
            SamplerState = new SamplerStateModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public Vector2Model Location { get; set; }
        public Vector2Model Scale { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public GenericValueModel<float> Rotation { get; set; }
        public SamplerStateModel SamplerState { get; set; }
    }
}
