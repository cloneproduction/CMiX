// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.BaseControl
{
    public class VectorXY : ObservableObject, IControl
    {
        public VectorXY(VectorXYModel vectorXYZModel)
        {
            ID = vectorXYZModel.ID;
            Name = vectorXYZModel.Name;

            X = new Slider(nameof(X), vectorXYZModel.X);
            Y = new Slider(nameof(Y), vectorXYZModel.Y);
        }


        public Guid ID { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Slider X { get; set; }
        public Slider Y { get; set; }


        public IModel GetModel()
        {
            VectorXYModel vectorXYZModel = new VectorXYModel(this.Name);
            vectorXYZModel.ID = ID;

            vectorXYZModel.X = (SliderModel)X.GetModel();
            vectorXYZModel.Y = (SliderModel)Y.GetModel();

            return vectorXYZModel;
        }

        public void SetViewModel(IModel model)
        {
            VectorXYModel modelXYZModel = model as VectorXYModel;
            this.ID = modelXYZModel.ID;
            this.Name = modelXYZModel.Name;

            this.X.SetViewModel(modelXYZModel.X);
            this.Y.SetViewModel(modelXYZModel.Y);
        }
    }
}
