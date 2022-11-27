// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Composition : Component, IPrefab
    {
        public Composition(CompositionModel compositionModel, CompositionService compositionService)
        {
            ID = compositionModel.ID;
            CompositionService = compositionService;

            OutputSettings = new OutputSettings(compositionModel.OutputSettings, compositionService);
            LayerManager = new DraggablePrefabManager<Layer>(compositionModel.LayerManager.ID, compositionService, compositionService.LayerRepository);
            ModifierManager = new ModifierManager(compositionModel.ModifierManager, new TextureFilterFactory(compositionService));

            IsActive = true;
        }


        public CompositionService CompositionService { get; set; }
        public PrefabManager<Layer> LayerManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public ModifierManager ModifierManager { get; set; }


        private MasterBeat _masterBeat;
        public MasterBeat MasterBeat
        {
            get => _masterBeat;
            set
            {
                SetProperty(ref _masterBeat, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageChangePrefab(this.ID, value, nameof(MasterBeat)), MessageType.Out);
            }
        }


        public override IModel GetModel()
        {
            CompositionModel model = new CompositionModel();

            model.Name = this.Name;
            model.ID = this.ID;
            model.OutputSettings = (OutputSettingsModel)this.OutputSettings.GetModel();
            model.ModifierManager = (ModifierManagerModel)this.ModifierManager.GetModel();
            model.LayerManager = (PrefabManagerModel)this.LayerManager.GetModel();
            return model;
        }

        public override void SetViewModel(IModel model)
        {
            CompositionModel compositionModel = model as CompositionModel;
            this.ID = compositionModel.ID;
            this.OutputSettings.SetViewModel(compositionModel.OutputSettings);
            this.ModifierManager.SetViewModel(compositionModel.ModifierManager);
            this.LayerManager.SetViewModel(compositionModel.LayerManager);
        }
    }
}
