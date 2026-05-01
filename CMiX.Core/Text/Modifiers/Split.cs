// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Text.Modifiers
{
    [ModifierPanel(typeof(TextEntity))]
    public partial class Split : ObservableObject, IPrefab
    {
        public Split(PrefabService prefabService,
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

        public IControlModel ToModel() => new SplitModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Separator = (GenericValueModel<string>)Separator.ToModel(),
            Type = (GenericValueModel<SplitType>)Type.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (SplitModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Separator.FromModel(m.Separator);
            Type.FromModel(m.Type);
        }
    }
}
