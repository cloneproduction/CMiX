// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Geometry : ObservableObject, IControl, ITransform
    {
        public Geometry(GeometryModel geometryModel, Guid componentID)
        {
            this.ID = geometryModel.ID;
            Instancer = new Instancer(geometryModel.InstancerModel);
            Transform = new Transform(geometryModel.TransformModel);
            GeometryFX = new GeometryFX(geometryModel.GeometryFXModel);
            GeometrySelector = new GeometrySelector(new AssetGeometry(), geometryModel.AssetPathSelectorModel);
        }


        public Guid ID { get; set; }

        public GeometrySelector GeometrySelector { get; set; }
        public Transform Transform { get; set; }
        public Instancer Instancer { get; set; }
        public GeometryFX GeometryFX { get; set; }


        public IModel GetModel()
        {
            GeometryModel model = new GeometryModel();
            model.ID = this.ID;
            model.TransformModel = (TransformModel)this.Transform.GetModel();
            model.GeometryFXModel = (GeometryFXModel)this.GeometryFX.GetModel();
            model.InstancerModel = (InstancerModel)this.Instancer.GetModel();
            model.AssetPathSelectorModel = (GeometrySelectorModel)this.GeometrySelector.GetModel();
            return model;
        }

        public void SetViewModel(IModel model)
        {
            GeometryModel geometryModel = model as GeometryModel;
            this.ID = geometryModel.ID;
            this.Transform.SetViewModel(geometryModel.TransformModel);
            this.GeometryFX.SetViewModel(geometryModel.GeometryFXModel);
            this.Instancer.SetViewModel(geometryModel.InstancerModel);
            this.GeometrySelector.SetViewModel(geometryModel.AssetPathSelectorModel);
        }
    }
}
