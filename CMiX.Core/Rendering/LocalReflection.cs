// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public partial class LocalReflection : ObservableObject, IPrefab
    {
        public LocalReflection(PrefabService prefabService,
                               GenericValue<bool> isEnabled)
        {
            PrefabService = prefabService;
            IsEnabled = isEnabled;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> IsEnabled { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new LocalReflectionModel
        {
            ID = ID,
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LocalReflectionModel)model;
            ID = m.ID;
            IsEnabled.FromModel(m.IsEnabled);
        }
    }
}
