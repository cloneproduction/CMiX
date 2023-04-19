// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabContainerModel : IPrefabModel
    {
        public PrefabContainerModel(Guid id)
        {
            ID = id;
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
            Name = new StringValueModel();
        }

        public PrefabContainerModel()
        {
            ID = Guid.NewGuid();
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
            Name = new StringValueModel();
        }

        public Guid ID { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
        public StringValueModel Name { get; set; }
        public IPrefabModel Prefab { get; set; }
    }
}
