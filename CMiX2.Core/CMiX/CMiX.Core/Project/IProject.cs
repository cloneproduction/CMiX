// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Collections;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels.Assets;
using CMiX.Core.ViewModels.Scheduling;

namespace CMiX.Core.Components
{
    public interface IProject : IPrefab
    {
        SortableObservableCollection<IAsset> Assets { get; set; }
        ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        ObservableCollection<Playlist> Playlists { get; set; }
    }
}
