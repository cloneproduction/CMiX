// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : Component
    {
        public Entity(EntityModel entityModel, MasterBeat masterBeat) : base()
        {
            ID = entityModel.ID;

            MasterBeat = masterBeat;
            BeatModifier = new BeatModifier(entityModel.BeatModifierModel);
            Geometry = new Geometry(entityModel.GeometryModel);
            Texture = new Texture(entityModel.TextureModel);
            Coloration = new Coloration(entityModel.ColorationModel);
            Visibility = new Visibility(entityModel.VisibilityModel);
        }


        public BeatModifier BeatModifier { get; set; }
        public Geometry Geometry { get; set; }
        public Texture Texture { get; set; }
        public Coloration Coloration { get; set; }
        public MasterBeat MasterBeat { get; set; }


        public override IComponentModel GetModel()
        {
            EntityModel model = new EntityModel(this.ID);

            model.Name = this.Name;
            model.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            model.TextureModel = (TextureModel)this.Texture.GetModel();
            model.GeometryModel = (GeometryModel)this.Geometry.GetModel();
            model.ColorationModel = (ColorationModel)this.Coloration.GetModel();

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.BeatModifier.SetViewModel(entityModel.BeatModifierModel);
            this.Texture.SetViewModel(entityModel.TextureModel);
            this.Geometry.SetViewModel(entityModel.GeometryModel);
            this.Coloration.SetViewModel(entityModel.ColorationModel);
        }
    }
}
