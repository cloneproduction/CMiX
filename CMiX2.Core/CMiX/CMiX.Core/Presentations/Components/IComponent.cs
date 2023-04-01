// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Components
{
    public interface IComponent : IIDObject, IDisposable
    {
        ObservableCollection<IComponent> Components { get; set; }

        BooleanValue IsSelected { get; set; }
        BooleanValue IsRenaming { get; set; }
        StringValue Name { get; set; }

        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);
        void RemoveComponent(Guid componentID);

        IComponent GetComponent(Guid childID);
    }
}
