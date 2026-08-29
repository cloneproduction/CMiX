// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
