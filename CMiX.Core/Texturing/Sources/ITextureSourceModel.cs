// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Sources
{
    // Implemented by every texture source model, so TextureSourceBase can read and write the shared fields.
    public interface ITextureSourceModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        PrefabManagerModel TextureModifierManager { get; set; }
        GenericValueModel<bool> UseCompositionResolution { get; set; }
    }
}
