// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabService
    {
        public PrefabService(StringValue name, BooleanValue isRenaming, BooleanValue isSelected, BooleanValue visibility)
        {
            ID = Guid.NewGuid();
            Name = name;
            IsRenaming = isRenaming;
            IsSelected = isSelected;
            Visibility = visibility;
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }
    }
}
