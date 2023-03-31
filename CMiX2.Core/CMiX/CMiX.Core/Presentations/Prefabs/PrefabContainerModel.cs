// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Presentations.Prefabs
{
    public class PrefabContainerModel : IModel
    {
        public PrefabContainerModel(Guid id)
        {
            ID = id;
            IsSelected = false;
        }

        public PrefabContainerModel()
        {
            ID = Guid.NewGuid();
            IsSelected = false;
        }

        public Guid ID { get; set; }
        public bool IsSelected { get; internal set; }
        public IPrefabModel Prefab { get; set; }
    }
}
