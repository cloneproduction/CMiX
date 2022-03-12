// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Entity : Component, IBeatable, IDisposable
    {
        public Entity(EntityModel entityModel) : base()
        {
            ID = entityModel.ID;
            WeakReferenceMessenger.Default.RegisterAll(this, this.ID);

            Geometry = new Geometry(entityModel.GeometryModel);
            Texture = new Texture(entityModel.TextureModel);
            Material = new Material(entityModel.ColorationModel);

            //Visibility = new Visibility(entityModel.VisibilityModel);
        }


        public Geometry Geometry { get; set; }
        public Texture Texture { get; set; }
        public Material Material { get; set; }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
            Material.SetMasterBeat(masterBeat);
            Geometry.SetMasterBeat(masterBeat);
        }


        public override IComponentModel GetModel()
        {
            EntityModel model = new EntityModel(this.ID);

            model.Name = this.Name;
            model.TextureModel = (TextureModel)this.Texture.GetModel();
            model.GeometryModel = (GeometryModel)this.Geometry.GetModel();
            model.ColorationModel = (MaterialModel)this.Material.GetModel();

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            EntityModel entityModel = model as EntityModel;
            this.ID = entityModel.ID;
            this.Texture.SetViewModel(entityModel.TextureModel);
            this.Geometry.SetViewModel(entityModel.GeometryModel);
            this.Material.SetViewModel(entityModel.ColorationModel);
        }

        public override void Dispose()
        {
            base.Dispose();
            Material.Dispose();
        }
    }
}
