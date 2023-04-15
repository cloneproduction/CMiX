// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.BaseControls;
using CMiX.Core.Collections;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Scheduling;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Components
{
    public class Project : ObservableObject, IProject
    {
        public Project(CompositionService compositionService)
        {
            Assets = new SortableObservableCollection<IAsset>();
            CompositionSchedulers = new ObservableCollection<CompositionScheduler>();
            Playlists = new ObservableCollection<Playlist>();

            var CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var MasterBeatManagerID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            CompositionManager = new PrefabManager<Composition>(CompositionManagerID, compositionService, compositionService.CompositionRepository);
        }

        public Guid ID { get; set; }
        public CompositionService CompositionService { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        public SortableObservableCollection<IAsset> Assets { get; set; }
        public PrefabManager<Composition> CompositionManager { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }

        public void Dispose()
        {

        }
    }
}
