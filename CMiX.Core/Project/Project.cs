// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Components
{
    public class Project : ObservableObject, IProject
    {
        public Project(PrefabManagerBase prefabManagerBase)
        {
            Assets = new SortableObservableCollection<IAsset>();
            CompositionManager = prefabManagerBase;
        }

        public SortableObservableCollection<IAsset> Assets { get; set; }
        public PrefabManagerBase CompositionManager { get; set; }
    }
}
