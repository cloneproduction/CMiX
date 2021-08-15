// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Scheduling;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface IProject : IComponent
    {
        SortableObservableCollection<IAsset> Assets { get; set; }
        ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        Composition SelectedComposition { get; set; }
    }
}
