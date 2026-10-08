// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    public interface IModifierModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        bool IsExpanded { get; set; }
        // A model that keeps its values as named properties has no list. This default stands in for it
        // until every modifier model is moved, then the member goes.
        List<ModulatableValueModel<float>> Bindables { get => new(); set { } }
        PrefabManagerModel ModulatorManager { get; set; }
    }
}
