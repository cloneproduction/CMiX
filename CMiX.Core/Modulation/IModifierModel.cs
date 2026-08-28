// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Standalone rather than extending IPrefabModel/IControlModel, mirroring
    // IBeatModifiableModifierModel: those only expose ID/PrefabService as get-only, and
    // Modifier's PopulateBaseModel/LoadBaseModel need to set them.
    public interface IModifierModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        bool IsExpanded { get; set; }
        List<ChannelModel> Channels { get; set; }
        PrefabManagerModel ModulatorManager { get; set; }
    }
}
