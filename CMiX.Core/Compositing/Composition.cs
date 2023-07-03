// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public class Composition : ObservableObject, IPrefab, IModifiable
    {
        public Composition(CompositionModel compositionModel, CompositionService compositionService)
        {
            ID = compositionModel.ID;
            CompositionService = compositionService;
            Name = new StringValue(compositionModel.Name);
            IsSelected = new BooleanValue(compositionModel.IsSelected);
            IsRenaming = new BooleanValue(compositionModel.IsRenaming);
            OutputSettings = new OutputSettings(compositionModel.OutputSettings);
            LayerManager = new DraggablePrefabManager(compositionModel.LayerManager.ID, compositionService, compositionService.LayerRepository);
            ModifierManager = new ModifierManager(compositionModel.ModifierManager, new TextureFilterFactory());
            MasterBeat = new MasterBeat(compositionModel.MasterBeat);

            CompositionService.MasterBeat = MasterBeat;
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public PrefabManager LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public CompositionService CompositionService { get; set; }
    }
}
