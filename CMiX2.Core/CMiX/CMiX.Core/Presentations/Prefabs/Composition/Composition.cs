// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Rendering;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Components
{
    public class Composition : ObservableObject, IPrefab
    {
        public Composition(CompositionModel compositionModel, CompositionService compositionService)
        {
            ID = compositionModel.ID;
            CompositionService = compositionService;
            Name = new StringValue(compositionModel.Name, compositionService);
            IsSelected = new BooleanValue(compositionModel.IsSelected, compositionService);
            IsRenaming = new BooleanValue(compositionModel.IsRenaming, compositionService);
            OutputSettings = new OutputSettings(compositionModel.OutputSettings, compositionService);
            LayerManager = new DraggablePrefabManager<Layer>(compositionModel.LayerManager.ID, compositionService, compositionService.LayerRepository);
            TextureModifierManager = new ModifierManager(compositionModel.TextureModifierManager, new TextureFilterFactory(compositionService), compositionService);
            MasterBeat = new MasterBeat(compositionModel.MasterBeat, compositionService);
        }


        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public CompositionService CompositionService { get; set; }
        public PrefabManager<Layer> LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public MasterBeat MasterBeat { get; set; }
    }
}
