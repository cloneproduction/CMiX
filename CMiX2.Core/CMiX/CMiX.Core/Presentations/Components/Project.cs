// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Scheduling;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Project : Component, IProject
    {
        public Project(CompositionService compositionService)
        {
            CompositionService = compositionService;
            Assets = new SortableObservableCollection<IAsset>();
            CompositionSchedulers = new ObservableCollection<CompositionScheduler>();
            Playlists = new ObservableCollection<Playlist>();

            Guid CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid MasterBeatManagerID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            CompositionManager = new PrefabManager<Composition>(CompositionManagerID, compositionService, compositionService.CompositionRepository);
        }


        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
        public CompositionService CompositionService { get; set; }
        public PrefabManager<Composition> CompositionManager { get; set; }


        public override void AddComponent(IComponent component)
        {
            this.Components.Add(component);
        }
    }
}
