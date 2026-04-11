// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableRecipient, IControl
    {
        public ManagerData()
        {
            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ObservableCollection<IControl> Items { get; set; } = new();
        public int SelectedIndex { get; set; }

        public IControlModel ToModel() => new ManagerDataModel
        {
            ID = ID,
            SelectedIndex = SelectedIndex,
            Items = new Collection<IControlModel>(
            Items.Select(c => c.ToModel()).ToList())
        };

        public void FromModel(IControlModel model)
        {
            var m = (ManagerDataModel)model;
            ID = m.ID;
            SelectedIndex = m.SelectedIndex;
        }
    }
}
