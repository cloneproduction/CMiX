// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public class ScaleModel : IControlModel, IPrefabModel
    {
        public ScaleModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Uniform = new GenericValueModel<float>(1.0f);
            XYZ = new Vector3Model(1.0f, 1.0f, 1.0f);
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public Vector3Model XYZ { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
