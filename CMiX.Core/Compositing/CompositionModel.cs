// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public class CompositionModel : IControlModel, IPrefabModel
    {
        public CompositionModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;
            MasterBeat = new MasterBeatModel();
            OutputSettings = new OutputSettingsModel();
            TextureModifierManager = new PrefabManagerModel();
            LayerManager = new PrefabManagerModel();
            ModifierManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public MasterBeatModel MasterBeat { get; set; }
        public OutputSettingsModel OutputSettings { get; set; }
        public PrefabManagerModel TextureModifierManager { get; set; }
        public PrefabManagerModel LayerManager { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
    }
}
