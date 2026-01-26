// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public class TranslateModel : IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public Vector3Model XYZ { get; init; } = new();
        public PrefabServiceModel PrefabService { get; init; } = new();
    }
}
