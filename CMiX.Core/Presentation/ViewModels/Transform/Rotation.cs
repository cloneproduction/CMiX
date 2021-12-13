// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Rotation : ObservableObject, IControl
    {
        public Rotation(RotationModel rotationModel)
        {
            this.ID = rotationModel.ID;
            X = new Slider(nameof(X), rotationModel.X);
            Y = new Slider(nameof(Y), rotationModel.Y);
            Z = new Slider(nameof(Z), rotationModel.Z);
        }


        public Guid ID { get; set; }
        public Slider X { get; set; }
        public Slider Y { get; set; }
        public Slider Z { get; set; }


        public IModel GetModel()
        {
            RotationModel model = new RotationModel();
            model.ID = this.ID;
            model.X = (SliderModel)this.X.GetModel();
            model.Y = (SliderModel)this.Y.GetModel();
            model.Z = (SliderModel)this.Z.GetModel();
            return model;
        }

        public void SetViewModel(IModel model)
        {
            RotationModel rotationModel = model as RotationModel;
            this.ID = rotationModel.ID;
            this.X.SetViewModel(rotationModel.X);
            this.Y.SetViewModel(rotationModel.Y);
            this.Z.SetViewModel(rotationModel.Z);
        }
    }
}
