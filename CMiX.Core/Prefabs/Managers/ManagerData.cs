// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerData : ObservableObject, IControl
    {
        public ManagerData()
        {

        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ObservableCollection<IControl> Items { get; set; } = new();

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

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
