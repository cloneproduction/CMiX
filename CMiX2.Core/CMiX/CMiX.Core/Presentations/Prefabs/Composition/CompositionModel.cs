// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels.Modifiers;
using CMiX.Core.Beat;

namespace CMiX.Core.Presentations.Components
{
    public class CompositionModel : IComponentModel, IPrefabModel
    {
        public CompositionModel()
        {
            ID = Guid.NewGuid();

            MasterBeat = new MasterBeatModel();
            Visibility = new BooleanValueModel();
            OutputSettings = new OutputSettingsModel();
            TextureModifierManager = new ModifierManagerModel();
            LayerManager = new PrefabManagerModel();
            Name = new StringValueModel("Composition");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
        }


        public BooleanValueModel Visibility { get; set; }
        public MasterBeatModel MasterBeat { get; set; }


        public StringValueModel Name { get; set; }
        public Guid ID { get; set; }


        public OutputSettingsModel OutputSettings { get; set; }
        public ModifierManagerModel TextureModifierManager { get; internal set; }
        public PrefabManagerModel LayerManager { get; set; }
        public BooleanValueModel IsSelected { get; internal set; }
        public BooleanValueModel IsRenaming { get; internal set; }
    }
}
