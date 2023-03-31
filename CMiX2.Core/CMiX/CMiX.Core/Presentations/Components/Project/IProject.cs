// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentations.ViewModels.Assets;
using CMiX.Core.Presentations.ViewModels.Scheduling;

namespace CMiX.Core.Presentations.Components
{
    public interface IProject : IIDObject, IDisposable, IComponent
    {
        SortableObservableCollection<IAsset> Assets { get; set; }
        ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        ObservableCollection<Playlist> Playlists { get; set; }
    }
}
