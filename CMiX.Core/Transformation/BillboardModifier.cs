// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    [ModifierPanel(typeof(Entity))]
    public partial class BillboardModifier : ObservableObject, IModifier
    {
        public BillboardModifier(PrefabService prefabService)
        {
            PrefabService = prefabService;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }


        [ObservableProperty]
        private bool isExpanded;

        public IControlModel ToModel() => new BillboardModifierModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (BillboardModifierModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
        }
    }
}
