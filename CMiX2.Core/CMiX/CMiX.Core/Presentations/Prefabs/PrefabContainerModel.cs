// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;

namespace CMiX.Core.Presentations.Prefabs
{
    public class PrefabContainerModel : IModel
    {
        public PrefabContainerModel(Guid id)
        {
            ID = id;
            IsSelected = new BooleanValueModel(false);
        }

        public PrefabContainerModel()
        {
            ID = Guid.NewGuid();
            IsSelected = new BooleanValueModel(false);
        }

        public Guid ID { get; set; }
        public BooleanValueModel IsSelected { get; internal set; }
        public IPrefabModel Prefab { get; set; }
    }
}
