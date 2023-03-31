// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation;
using CMiX.Core.Presentation.Components;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Scheduling;

namespace CMiX.Core.Presentations.Components
{
    public interface IProject : IIDObject, IDisposable, IComponent
    {
        SortableObservableCollection<IAsset> Assets { get; set; }
        ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        ObservableCollection<Playlist> Playlists { get; set; }
    }
}
