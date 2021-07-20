// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components.Factories;
using MediatR;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Scene : Component
    {
        public Scene(Layer layer, SceneModel sceneModel)
        {
            ID = sceneModel.ID;
            MasterBeat = layer.MasterBeat;

            Visibility = new Visibility(layer.Visibility, sceneModel.VisibilityModel);
            BeatModifier = new BeatModifier(layer.MasterBeat, sceneModel.BeatModifierModel);
            PostFX = new PostFX(sceneModel.PostFXModel);
            Mask = new Mask(sceneModel.MaskModel);
            Transform = new Transform(sceneModel.TransformModel);

            ComponentFactory = new EntityFactory(this);
        }


        public Transform Transform { get; set; }
        public Mask Mask { get; set; }
        public PostFX PostFX { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public MasterBeat MasterBeat { get; set; }


        public override void SetCommunicator(Communicator communicator)
        {
            Communicator.SetCommunicator(communicator);

            Transform.SetCommunicator(Communicator);
            Mask.SetCommunicator(Communicator);
            PostFX.SetCommunicator(Communicator);
            BeatModifier.SetCommunicator(Communicator);
            MasterBeat.SetCommunicator(Communicator);
        }

        public override void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public override IModel GetModel()
        {
            SceneModel model = new SceneModel(this.ID);

            //model.Enabled = this.Enabled;
            model.Name = this.Name;

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.PostFXModel = (PostFXModel)this.PostFX.GetModel();
            model.MaskModel = (MaskModel)this.Mask.GetModel();
            model.TransformModel = (TransformModel)this.Transform.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel() as IComponentModel);

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            SceneModel sceneModel = model as SceneModel;

            this.BeatModifier.SetViewModel(sceneModel.BeatModifierModel);
            this.PostFX.SetViewModel(sceneModel.PostFXModel);
            this.Mask.SetViewModel(sceneModel.MaskModel);
            this.Transform.SetViewModel(sceneModel.TransformModel);

            this.Components.Clear();
            foreach (var componentModel in sceneModel.ComponentModels)
            {
                var newComponent = this.ComponentFactory.CreateComponent(componentModel);
                //newComponent.SetReceiver(MessageReceiver);
                //newComponent.SetSender(MessageSender);
                this.AddComponent(newComponent);
            }
        }
    }
}
