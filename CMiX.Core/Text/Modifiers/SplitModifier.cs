// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Text.Modifiers
{
    [ModifierPanel(typeof(TextEntity))]
    public partial class SplitModifier : ObservableObject, IPrefab
    {
        public SplitModifier(PrefabService prefabService,
                     GenericValue<string> separator,
                     GenericValue<SplitType> type)
        {
            PrefabService = prefabService;
            Separator = separator;
            Type = type;
        }

        public PrefabService PrefabService { get; set; }
        public Guid ID { get; set; }

        public GenericValue<string> Separator { get; set; }
        public GenericValue<SplitType> Type { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new SplitModifierModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Separator = (GenericValueModel<string>)Separator.ToModel(),
            Type = (GenericValueModel<SplitType>)Type.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (SplitModifierModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Separator.FromModel(m.Separator);
            Type.FromModel(m.Type);
        }
    }
}
