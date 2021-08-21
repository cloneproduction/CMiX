// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : Component
    {
        public Entity(EntityModel entityModel) : base()
        {
            ID = entityModel.ID;

            Geometry = new Geometry(entityModel.GeometryModel);
            Texture = new Texture(entityModel.TextureModel);
            Coloration = new Coloration(entityModel.ColorationModel);
            //Visibility = new Visibility(entityModel.VisibilityModel);
        }


        public Geometry Geometry { get; set; }
        public Texture Texture { get; set; }
        public Coloration Coloration { get; set; }


        public override IComponentModel GetModel()
        {
            EntityModel model = new EntityModel(this.ID);

            model.Name = this.Name;
            model.TextureModel = (TextureModel)this.Texture.GetModel();
            model.GeometryModel = (GeometryModel)this.Geometry.GetModel();
            model.ColorationModel = (ColorationModel)this.Coloration.GetModel();

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.Texture.SetViewModel(entityModel.TextureModel);
            this.Geometry.SetViewModel(entityModel.GeometryModel);
            this.Coloration.SetViewModel(entityModel.ColorationModel);
        }
    }
}
