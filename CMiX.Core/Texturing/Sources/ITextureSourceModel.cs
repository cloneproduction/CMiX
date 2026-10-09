// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
