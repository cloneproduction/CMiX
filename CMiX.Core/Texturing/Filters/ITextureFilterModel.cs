// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
        GenericValueModel<float> Control { get; set; }
        BlendModel Blend { get; set; }
        List<ModulatableValueModel<float>> Bindables { get; set; }
        PrefabManagerModel ModulatorManager { get; set; }
    }
}
