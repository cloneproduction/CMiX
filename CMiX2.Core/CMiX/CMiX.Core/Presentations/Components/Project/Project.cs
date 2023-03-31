// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels.Assets;
using CMiX.Core.Presentations.ViewModels.Components;
using CMiX.Core.Presentations.ViewModels.Scheduling;
using CMiX.Core.Presentations.Service;

namespace CMiX.Core.Presentations.Components
{
    public class Project : Component, IProject
    {
        public Project(CompositionService compositionService)
        {
            CompositionService = compositionService;
            Assets = new SortableObservableCollection<IAsset>();
            CompositionSchedulers = new ObservableCollection<CompositionScheduler>();
            Playlists = new ObservableCollection<Playlist>();

            var CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var MasterBeatManagerID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            CompositionManager = new PrefabManager<Composition>(CompositionManagerID, compositionService, compositionService.CompositionRepository);
        }


        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
        public CompositionService CompositionService { get; set; }
        public PrefabManager<Composition> CompositionManager { get; set; }


        public override void AddComponent(IComponent component)
        {
            Components.Add(component);
        }
    }
}
