// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public class TranslateModel : IPrefabModel
    {
        public TranslateModel()
        {
            ID = Guid.NewGuid();
            XYZ = new Vector3Model();
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public Vector3Model XYZ { get; internal set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
