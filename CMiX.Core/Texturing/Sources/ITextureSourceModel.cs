// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    // Implemented by every texture source's own Model record (alongside IPrefabModel) so
    // TextureSourceBase can populate/read the three fields every source shares without knowing
    // which concrete Model type it's holding. Standalone rather than extending IPrefabModel/
    // IControlModel, since those only expose ID/PrefabService as get-only.
    public interface ITextureSourceModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        PrefabManagerModel TextureModifierManager { get; set; }
    }
}
