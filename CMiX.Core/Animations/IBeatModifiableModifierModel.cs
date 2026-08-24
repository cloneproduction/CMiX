// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Animations
{
    // Implemented by every beat-modifiable modifier's own Model record (alongside IPrefabModel)
    // so BeatModifiableModifierBase can populate/read the three fields every one of them shares
    // without knowing which concrete Model type it's holding. Standalone rather than extending
    // IPrefabModel/IControlModel, since those only expose ID/PrefabService as get-only.
    public interface IBeatModifiableModifierModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        PrefabManagerModel BeatModifierManager { get; set; }
    }
}
