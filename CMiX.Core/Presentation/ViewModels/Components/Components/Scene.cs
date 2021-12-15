// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Scene : Component
    {
        public Scene(SceneModel sceneModel)
        {
            ID = sceneModel.ID;
            // Visibility = new Visibility(layer.Visibility, sceneModel.VisibilityModel);
            BeatModifier = new BeatModifier(sceneModel.BeatModifierModel);
            PostFX = new PostFX(sceneModel.PostFXModel);
            Mask = new Mask(sceneModel.MaskModel);
            Transform = new Transform(sceneModel.TransformModel);

            MasterBeats = WeakReferenceMessenger.Default.Send(new MessageRequestMasterBeats(), MessageType.Internal).Response;
        }


        public Transform Transform { get; set; }
        public Mask Mask { get; set; }
        public PostFX PostFX { get; set; }
        public BeatModifier BeatModifier { get; set; }


        public override IComponentModel GetModel()
        {
            SceneModel model = new SceneModel(this.ID);

            model.Name = this.Name;

            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.PostFXModel = (PostFXModel)this.PostFX.GetModel();
            model.MaskModel = (MaskModel)this.Mask.GetModel();
            model.TransformModel = (TransformModel)this.Transform.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            SceneModel sceneModel = model as SceneModel;
            this.ID = sceneModel.ID;
            this.BeatModifier.SetViewModel(sceneModel.BeatModifierModel);
            this.PostFX.SetViewModel(sceneModel.PostFXModel);
            this.Mask.SetViewModel(sceneModel.MaskModel);
            this.Transform.SetViewModel(sceneModel.TransformModel);

            this.Components.Clear();
            foreach (var componentModel in sceneModel.ComponentModels)
            {
                //var newComponent = this.ComponentFactory.CreateComponent(componentModel);
                //this.AddComponent(newComponent);
            }
        }
    }
}
