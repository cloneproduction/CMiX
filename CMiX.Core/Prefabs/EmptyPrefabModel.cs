// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefabModel : IPrefabModel
    {
        public EmptyPrefabModel()
        {
            ID = Guid.NewGuid();

        }

        public Guid ID { get; set; }
        public BooleanValueModel Visibility { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
        public StringValueModel Name { get; set; }
    }
}
