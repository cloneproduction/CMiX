// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : Component
    {
        public Entity(Scene scene, EntityModel entityModel) : base()
        {
            ID = entityModel.ID;
            MasterBeat = scene.MasterBeat;

            BeatModifier = new BeatModifier(scene.MasterBeat, entityModel.BeatModifierModel);
            Geometry = new Geometry(scene.MasterBeat, entityModel.GeometryModel);
            Texture = new Texture(scene.MasterBeat, entityModel.TextureModel);
            Coloration = new Coloration(scene.MasterBeat, entityModel.ColorationModel);
            Visibility = new Visibility(scene.Visibility, entityModel.VisibilityModel);
        }


        public BeatModifier BeatModifier { get; set; }
        public Geometry Geometry { get; set; }
        public Texture Texture { get; set; }
        public Coloration Coloration { get; set; }
        public MasterBeat MasterBeat { get; set; }


        public override void SetCommunicator(Communicator communicator)
        {
            Communicator.SetCommunicator(communicator);

            BeatModifier.SetCommunicator(Communicator);
            Geometry.SetCommunicator(Communicator);
            Texture.SetCommunicator(Communicator);
            Coloration.SetCommunicator(Communicator);
        }

        public override void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);

            BeatModifier.UnsetCommunicator(Communicator);
            Geometry.UnsetCommunicator(Communicator);
            Texture.UnsetCommunicator(Communicator);
            Coloration.UnsetCommunicator(Communicator);
        }


        public override IModel GetModel()
        {
            EntityModel model = new EntityModel(this.ID);

            model.Name = this.Name;
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.TextureModel = (TextureModel)this.Texture.GetModel();
            model.GeometryModel = (GeometryModel)this.Geometry.GetModel();
            model.ColorationModel = (ColorationModel)this.Coloration.GetModel();

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.BeatModifier.SetViewModel(entityModel.BeatModifierModel);
            this.Texture.SetViewModel(entityModel.TextureModel);
            this.Geometry.SetViewModel(entityModel.GeometryModel);
            this.Coloration.SetViewModel(entityModel.ColorationModel);
        }
    }
}
