// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabService : IControl
    {
        public PrefabService(GenericValue<string> name,
                             GenericValue<bool> isSelected,
                             GenericValue<bool> visibility)
        {
            ID = Guid.NewGuid();
            Name = name;
            IsSelected = isSelected;
            Visibility = visibility;
        }

        public Guid ID { get; set; }
        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> Visibility { get; set; }

        public IControlModel ToModel() => new PrefabServiceModel
        {
            ID = ID,
            Name = (GenericValueModel<string>)Name.ToModel(),
            IsSelected = (GenericValueModel<bool>)IsSelected.ToModel(),
            Visibility = (GenericValueModel<bool>)Visibility.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (PrefabServiceModel)model;
            ID = m.ID;
            Name.FromModel(m.Name);
            IsSelected.FromModel(m.IsSelected);
            Visibility.FromModel(m.Visibility);
        }
    }
}
