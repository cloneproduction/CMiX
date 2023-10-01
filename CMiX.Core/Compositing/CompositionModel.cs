// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Rendering;

namespace CMiX.Core.Compositing
{
    public class CompositionModel : IPrefabModel
    {
        public CompositionModel()
        {
            MasterBeat = new MasterBeatModel();
            Visibility = new BooleanValueModel();
            OutputSettings = new OutputSettingsModel();
            ModifierManager = new ModifierManagerModel();
            LayerManager = new PrefabManagerModel();
            Name = new StringValueModel("Composition");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValueModel Visibility { get; set; }
        public MasterBeatModel MasterBeat { get; set; }
        public StringValueModel Name { get; set; }
        public OutputSettingsModel OutputSettings { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public PrefabManagerModel LayerManager { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
