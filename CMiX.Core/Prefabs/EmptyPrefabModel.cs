// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class EmptyPrefabModel : IControlModel, IPrefabModel
    {
        public EmptyPrefabModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Visibility = new GenericValueModel<bool>(false);
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            Name = new GenericValueModel<string>(this.GetType().Name);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<string> Name { get; set; }
    }
}
