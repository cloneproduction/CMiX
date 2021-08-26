// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class GeometryFX : ObservableObject, IControl
    {
        public GeometryFX(GeometryFXModel geometryFXModel)
        {
            this.ID = geometryFXModel.ID;
            Explode = new Slider(nameof(Explode), geometryFXModel.Explode);
        }


        public Guid ID { get; set; }
        public Slider Explode { get; set; }



        public void SetViewModel(IModel model)
        {
            GeometryFXModel geometryFXModel = model as GeometryFXModel;
            this.ID = geometryFXModel.ID;
            this.Explode.SetViewModel(geometryFXModel.Explode);
        }

        public IModel GetModel()
        {
            GeometryFXModel geometryFXModel = new GeometryFXModel();
            geometryFXModel.ID = this.ID;
            geometryFXModel.Explode = (SliderModel)this.Explode.GetModel();
            return geometryFXModel;
        }
    }
}
