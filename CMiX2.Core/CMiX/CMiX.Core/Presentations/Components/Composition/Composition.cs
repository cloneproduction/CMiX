// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Prefabs.Message;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.ViewModels.Components;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.Components
{
    public class Composition : Component, IPrefab
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

            IsActive = true;
        }

        public CompositionService CompositionService { get; set; }
        public PrefabManager<Layer> LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager TextureModifierManager { get; set; }

        private MasterBeat _masterBeat;
        public MasterBeat MasterBeat
        {
            get => _masterBeat;
            set
            {
                SetProperty(ref _masterBeat, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageChangePrefab(ID, value, nameof(MasterBeat)), MessageType.Out);
            }
        }
    }
}
