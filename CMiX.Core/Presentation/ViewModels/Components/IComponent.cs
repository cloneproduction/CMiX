// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Components.Factories;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface IComponent : IIDObject, IDisposable
    {
        Visibility Visibility { get; set; }
        ObservableCollection<IComponent> Components { get; set; }

        bool IsSelected { get; set; }
        bool IsRenaming { get; set; }

        IComponentFactory ComponentFactory { get; set; }
        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);

        void SetViewModel(IComponentModel model);
        IComponentModel GetModel();
    }
}
