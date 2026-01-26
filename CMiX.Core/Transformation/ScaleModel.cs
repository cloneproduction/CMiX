// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public record ScaleModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> Uniform { get; set; } = new(1.0f);
        public Vector3Model XYZ { get; set; } = new(1.0f, 1.0f, 1.0f);
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
