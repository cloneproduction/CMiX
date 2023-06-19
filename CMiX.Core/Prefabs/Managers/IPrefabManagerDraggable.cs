// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CMiX.Core.Prefab.Managers
{
    public interface IPrefabManagerDraggable : IPrefabManager
    {
        void UpdateComponentOrder(IList<Guid> ids);
        ObservableCollection<Guid> PrefabOrder { get; set; }
    }
}
