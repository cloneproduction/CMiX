// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;

namespace CMiX.Core.Presentation.Components
{
    public interface IComponent : IIDObject, IDisposable
    {
        ObservableCollection<IComponent> Components { get; set; }
        bool IsSelected { get; set; }
        bool IsRenaming { get; set; }

        string Name { get; set; }
        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);
        void RemoveComponent(Guid componentID);

        IComponent GetComponent(Guid childID);
    }
}
