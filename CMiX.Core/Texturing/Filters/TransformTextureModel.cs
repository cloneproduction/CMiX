// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing.Filters
{
    public class TransformTextureModel : IPrefabModel
    {
        public TransformTextureModel()
        {
            PrefabService = new PrefabServiceModel();
            SamplerState = new SamplerStateModel();
            Transform2D = new Transform2DModel();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public Transform2DModel Transform2D { get; set; }
    }
}
