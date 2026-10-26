// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Prefabs
{
    public class PrefabService : IControl
    {
        public PrefabService(GenericValue<string> name,
                             GenericValue<bool> visibility,
                             GenericValue<bool> isUpdating)
        {
            ID = Guid.NewGuid();
            Name = name;
            Visibility = visibility;
            IsUpdating = isUpdating;
        }

        public Guid ID { get; set; }
        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> Visibility { get; set; }

        // False stops the engine from updating the object in the main loop.
        public GenericValue<bool> IsUpdating { get; set; }

        public IControlModel ToModel() => new PrefabServiceModel
        {
            ID = ID,
            Name = (GenericValueModel<string>)Name.ToModel(),
            Visibility = (GenericValueModel<bool>)Visibility.ToModel(),
            IsUpdating = (GenericValueModel<bool>)IsUpdating.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (PrefabServiceModel)model;
            ID = m.ID;
            Name.FromModel(m.Name);
            Visibility.FromModel(m.Visibility);
            IsUpdating.FromModel(m.IsUpdating);
        }
    }
}
