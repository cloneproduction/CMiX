// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefab : IPrefab
    {
        public EmptyPrefab()
        {
            ID = Guid.NewGuid();
            IsSelected = new BooleanValue(false);
            IsRenaming = new BooleanValue(false);
            Name = new StringValue(String.Empty);
        }
        public EmptyPrefab(EmptyPrefabModel emptyPrefabModel) : this()
        {
            ID = emptyPrefabModel.ID;
        }

        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public Guid ID { get; set; }
    }
}
