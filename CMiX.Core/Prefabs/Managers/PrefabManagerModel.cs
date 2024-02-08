// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Managers
{
    public class PrefabManagerModel : IControlModel
    {
        public PrefabManagerModel()
        {
            ID = Guid.NewGuid();
            ManagerData = new ManagerDataModel();
        }

        public Guid ID { get; set; }

        public ManagerDataModel ManagerData { get; set; }
    }
}
