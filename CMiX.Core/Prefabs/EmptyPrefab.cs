// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Prefabs
{
    public partial class EmptyPrefab : ObservableObject, IControl, IPrefab
    {
        public EmptyPrefab(PrefabService prefabService)
        {
            PrefabService = prefabService;
            ID = prefabService.ID;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new EmptyPrefabModel
        {
            ID = ID
        };

        public void FromModel(IControlModel model)
        {
            var m = (EmptyPrefabModel)model;
            ID = m.ID;
        }
    }
}
