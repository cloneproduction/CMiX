// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components.Factories;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Layer : Component
    {
        public Layer(Composition composition, LayerModel layerModel)
        {
            ID = layerModel.ID;
            Fade = new Slider(nameof(Fade), layerModel.Fade);

            MasterBeat = composition.MasterBeat;
            Visibility = new Visibility(composition.Visibility, layerModel.VisibilityModel);
            PostFX = new PostFX(layerModel.PostFXModel);
            BlendMode = new BlendMode(layerModel.BlendMode);
            Mask = new Mask(layerModel.MaskModel);

            ComponentFactory = new SceneFactory(this);
            Communicator = new Communicator(this);
        }


        public MasterBeat MasterBeat { get; set; }
        public Slider Fade { get; set; }
        public Mask Mask { get; set; }
        public PostFX PostFX { get; set; }
        public BlendMode BlendMode { get; set; }
        public ToggleButton Out { get; set; }


        public override void SetCommunicator(Communicator communicator)
        {
            Communicator.SetCommunicator(communicator);

            Fade.SetCommunicator(Communicator);
            PostFX.SetCommunicator(Communicator);
            BlendMode.SetCommunicator(Communicator);
            Mask.SetCommunicator(Communicator);
        }

        public override void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public override IModel GetModel()
        {
            LayerModel model = new LayerModel(this.ID);

            model.Name = this.Name;
            //model.Out = this.Out;

            model.Fade = (SliderModel)this.Fade.GetModel();
            model.BlendMode = (BlendModeModel)this.BlendMode.GetModel();
            model.PostFXModel = (PostFXModel)this.PostFX.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel() as IComponentModel);

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerModel layerModel = model as LayerModel;

            //this.Out = layerModel.Out;
            this.Fade.SetViewModel(layerModel.Fade);
            this.BlendMode.SetViewModel(layerModel.BlendMode);
            this.PostFX.SetViewModel(layerModel.PostFXModel);

            this.Components.Clear();
            foreach (var componentModel in layerModel.ComponentModels)
            {
                var newComponent = this.ComponentFactory.CreateComponent(componentModel);
                //newComponent.SetReceiver(MessageReceiver);
                //newComponent.SetSender(MessageSender);
                this.AddComponent(newComponent);
            }
        }
    }
}
