// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Models.Component;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public partial class Composition : Component, IPrefab
    {
        public Composition(CompositionModel compositionModel, CompositionService compositionService)
        {
            ID = compositionModel.ID;
            CompositionService = compositionService;

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

        [ObservableProperty]
        private MasterBeat masterBeat;

        partial void OnMasterBeatChanged(MasterBeat value)
        {
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageChangePrefab(this.ID, value, nameof(MasterBeat)), MessageType.Out);
        }
    }
}
