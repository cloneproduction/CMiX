// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface IComponent
    {
        ObservableCollection<Component> Components { get; set; }
        void AddComponent(Component component);
        void RemoveComponent(Component component);

        void SetViewModel(IComponentModel model);
        IComponentModel GetModel();
    }
}
