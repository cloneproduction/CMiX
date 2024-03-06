// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Entities.Lights
{
    public class LightEntityModel : IControlModel, IPrefabModel
    {
        public LightEntityModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;
            ModifierManager = new PrefabManagerModel();
            Settings = new LightSettingsModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public LightSettingsModel Settings { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
    }
}
