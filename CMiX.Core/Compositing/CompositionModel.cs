// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public class CompositionModel : IPrefabModel
    {
        public CompositionModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;
            Name = new GenericValueModel<string>("Composition");
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            MasterBeat = new MasterBeatModel();
            Visibility = new GenericValueModel<bool>();
            OutputSettings = new OutputSettingsModel();

            ModifierManager = new PrefabManagerModel();
            LayerManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
        public MasterBeatModel MasterBeat { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public OutputSettingsModel OutputSettings { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public PrefabManagerModel LayerManager { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
    }
}
