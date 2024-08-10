// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Transformation
{
    public class BillboardModel : IControlModel, IPrefabModel
    {
        public BillboardModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
