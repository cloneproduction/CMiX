// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Components
{
    public class Project : ObservableObject, IProject
    {
        public Project(CompositionService compositionService)
        {
            Assets = new SortableObservableCollection<IAsset>();
            CompositionService = compositionService;
            CompositionManager = compositionService.GetPrefabManager<Project>();
        }

        public CompositionService CompositionService { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
        public PrefabManagerBase CompositionManager { get; set; }
    }
}
