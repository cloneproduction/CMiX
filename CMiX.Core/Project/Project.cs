// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public class Project : ObservableObject//, IControl, IPrefab
    {
        public Project(PrefabManager prefabManagerBase)
        {
            prefabManagerBase.ManagerData.ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
            ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
            Assets = new SortableObservableCollection<IAsset>();
            CompositionManager = prefabManagerBase;
        }

        public Guid ID { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
        public PrefabManager CompositionManager { get; set; }
        public PrefabService PrefabService { get; set; }
    }
}
