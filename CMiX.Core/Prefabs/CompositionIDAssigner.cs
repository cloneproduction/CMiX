// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Prefabs
{
    public class CompositionIDAssigner
    {
        // Assigns the id to items already in the manager, then keeps assigning it to items
        // added later, from a live edit or from a project load.
        public CompositionIDAssigner(PrefabManager manager, Guid compositionID)
        {
            foreach (var item in manager.Collection.ManagerData.Items)
                if (item is IHasCompositionID owned)
                    owned.CompositionID = compositionID;

            manager.Collection.ManagerData.Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;
                foreach (var item in e.NewItems)
                    if (item is IHasCompositionID owned)
                        owned.CompositionID = compositionID;
            };
        }
    }
}
