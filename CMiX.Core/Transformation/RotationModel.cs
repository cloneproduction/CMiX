// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public record RotationModel : IControlModel, IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3Model XYZ { get; set; } = new(1.0f, 1.0f, 1.0f);
        public GenericValueModel<bool> Visible { get; set; } = new(true);
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
