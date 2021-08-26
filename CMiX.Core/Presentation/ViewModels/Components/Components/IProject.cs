// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Scheduling;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface IProject : IIDObject, IDisposable
    {
        Visibility Visibility { get; set; }
        ObservableCollection<IComponent> Components { get; set; }
        //bool IsSelected { get; set; }
        //bool IsRenaming { get; set; }

        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);

        void SetViewModel(IComponentModel model);
        IComponentModel GetModel();

        SortableObservableCollection<IAsset> Assets { get; set; }
        ObservableCollection<CompositionScheduler> CompositionSchedulers { get; set; }
        //Composition SelectedComposition { get; set; }
    }
}
