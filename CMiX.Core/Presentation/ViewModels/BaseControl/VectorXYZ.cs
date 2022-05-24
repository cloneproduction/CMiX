// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.BaseControl
{
    public class VectorXYZ : ObservableObject, IControl
    {
        public VectorXYZ(VectorXYZModel vectorXYZModel)
        {
            ID = vectorXYZModel.ID;
            Name = vectorXYZModel.Name;

            X = new Slider(nameof(X), vectorXYZModel.X);
            Y = new Slider(nameof(Y), vectorXYZModel.Y);
            Z = new Slider(nameof(Z), vectorXYZModel.Z);
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
        public Slider Z { get; set; }


        public IModel GetModel()
        {
            VectorXYZModel vectorXYZModel = new VectorXYZModel(this.Name);
            vectorXYZModel.ID = ID;

            vectorXYZModel.X = (SliderModel)X.GetModel();
            vectorXYZModel.Y = (SliderModel)Y.GetModel();
            vectorXYZModel.Z = (SliderModel)Z.GetModel();

            return vectorXYZModel;
        }

        public void SetViewModel(IModel model)
        {
            VectorXYZModel modelXYZModel = model as VectorXYZModel;
            this.ID = modelXYZModel.ID;
            this.Name = modelXYZModel.Name;

            this.X.SetViewModel(modelXYZModel.X);
            this.Y.SetViewModel(modelXYZModel.Y);
            this.Z.SetViewModel(modelXYZModel.Z);
        }
    }
}
