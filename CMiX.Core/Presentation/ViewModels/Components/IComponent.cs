// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public interface IComponent
    {
        ObservableCollection<Component> Components { get; set; }
        void AddComponent(Component component);
        void RemoveComponent(Component component);
    }
}
