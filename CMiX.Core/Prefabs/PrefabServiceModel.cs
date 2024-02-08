// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabServiceModel : IControlModel
    {
        public PrefabServiceModel()
        {
            ID = Guid.NewGuid();
            Name = new GenericValueModel<string>();
            IsRenaming = new GenericValueModel<bool>(false);
            IsSelected = new GenericValueModel<bool>(false);
            Visibility = new GenericValueModel<bool>(false);
        }

        public Guid ID { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
    }
}
