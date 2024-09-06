// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using Microsoft.Extensions.Options;

namespace CMiX.Core.Prefabs
{
    public class PrefabService : IControl
    {
        public PrefabService(GenericValue<string> name, 
                             GenericValue<bool> isRenaming, 
                             GenericValue<bool> isSelected, 
                             GenericValue<bool> visibility,
                             IOptions<PrefabServiceModel> prefabServiceModel)
        {
            ID = Guid.NewGuid();
            Name = name;
            IsRenaming = isRenaming;
            IsSelected = isSelected;
            Visibility = visibility;
        }

        public Guid ID { get; set; }
        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> Visibility { get; set; }


        public void Set(PrefabServiceModel prefabServiceModel)
        { 
            ID = prefabServiceModel.ID;
            Name.Value = prefabServiceModel.Name.Value;
            IsRenaming.Value = prefabServiceModel.IsRenaming.Value;
            IsSelected.Value = prefabServiceModel.IsSelected.Value;
            Visibility.Value = prefabServiceModel.Visibility.Value;
        }
    }
}
