// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Linq;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Composition : Component, IBeatable
    {
        public Composition(CompositionModel compositionModel)
        {
            ID = compositionModel.ID;
            Transition = new Slider(nameof(Transition), compositionModel.TransitionModel);
            MasterBeat = new MasterBeat(compositionModel.MasterBeatModel);
            
            OutputProperties = new OutputProperties(compositionModel.OutputProperties);

            ModifierManager = new ModifierManager(compositionModel.ModifierManager, new TextureFilterFactory());

            CompositionService = new CompositionService(compositionModel.CompositionService, MasterBeat);

            LayerManager = new LayerManager(this);
        }


        public CompositionService CompositionService { get; set; }

        public Camera Camera { get; set; }
        public Slider Transition { get; set; }
        public LayerManager LayerManager { get; set; }
        public OutputProperties OutputProperties { get; set; }
        public ModifierManager ModifierManager { get; set; }


        private Layer _selectedLayer;
        public Layer SelectedLayer
        {
            get => _selectedLayer;
            set => SetProperty(ref _selectedLayer, value);
        }


        public override void AddComponent(IComponent component)
        {
            base.AddComponent(component);
            if (component is Layer layer)
            {
                SelectedLayer = layer;
                //layer.CameraManager = this.CameraManager;
            }
        }


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
            CompositionService.SetMasterBeat(masterBeat);

            IEnumerable beatables = this.Components.Select(x => x.GetType() == typeof(IBeatable));

            foreach (IBeatable beatable in beatables)
                beatable.SetMasterBeat(this.MasterBeat);
        }

        public override IComponentModel GetModel()
        {
            CompositionModel model = new CompositionModel(this.ID);

            model.Name = this.Name;
            model.ID = this.ID;
            model.MasterBeatModel = (MasterBeatModel)this.MasterBeat.GetModel();
            model.TransitionModel = (SliderModel)this.Transition.GetModel();

            model.OutputProperties = (OutputPropertiesModel)this.OutputProperties.GetModel();
            model.ModifierManager = (ModifierManagerModel)this.ModifierManager.GetModel();
            model.CompositionService = (CompositionServiceModel)this.CompositionService.GetModel();
            //model.MaterialManager = (MaterialManagerModel)this.MaterialManager.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            CompositionModel compositionModel = model as CompositionModel;
            this.ID = compositionModel.ID;
            this.MasterBeat.SetViewModel(compositionModel.MasterBeatModel);
            this.Transition.SetViewModel(compositionModel.TransitionModel);

            this.OutputProperties.SetViewModel(compositionModel.OutputProperties);
            this.ModifierManager.SetViewModel(compositionModel.ModifierManager);
            this.CompositionService.SetViewModel(compositionModel.CompositionService);

            //this.MaterialManager.SetViewModel(compositionModel.MaterialManager);

            this.Components.Clear();

            foreach (var componentModel in compositionModel.ComponentModels)
            {
                //var newComponent = this.ComponentFactory.CreateComponent(compositionModel);
                //this.AddComponent(newComponent);
            }
        }
    }
}
