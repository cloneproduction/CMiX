// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    // Implemented by every filter's own Model record (alongside IPrefabModel) so
    // TextureFilterBase can populate/read the fields every filter shares without knowing
    // which concrete Model type it's holding. Standalone rather than extending IPrefabModel/
    // IControlModel, since those only expose ID/PrefabService as get-only.
    public interface ITextureFilterModel
    {
        Guid ID { get; set; }
        PrefabServiceModel PrefabService { get; set; }
        BlendModel Blend { get; set; }
        PrefabManagerModel ModulatorManager { get; set; }
    }
}
