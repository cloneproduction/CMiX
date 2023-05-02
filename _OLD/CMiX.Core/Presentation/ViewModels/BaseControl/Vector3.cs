// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.BaseControl
{
    public class Vector3 : ObservableObject, IControl
    {
        public Vector3(Vector3Model vectorXYZModel)
        {
            ID = vectorXYZModel.ID;
            Name = vectorXYZModel.Name;

            X = new FloatValue(vectorXYZModel.X);
            Y = new FloatValue(vectorXYZModel.Y);
            Z = new FloatValue(vectorXYZModel.Z);
        }


        public Guid ID { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
        public FloatValue Z { get; set; }


        public IModel GetModel()
        {
            Vector3Model vectorXYZModel = new Vector3Model(this.Name);
            vectorXYZModel.ID = ID;

            vectorXYZModel.X = (FloatValueModel)X.GetModel();
            vectorXYZModel.Y = (FloatValueModel)Y.GetModel();
            vectorXYZModel.Z = (FloatValueModel)Z.GetModel();

            return vectorXYZModel;
        }

        public void SetViewModel(IModel model)
        {
            Vector3Model modelXYZModel = model as Vector3Model;
            this.ID = modelXYZModel.ID;
            this.Name = modelXYZModel.Name;

            this.X.SetViewModel(modelXYZModel.X);
            this.Y.SetViewModel(modelXYZModel.Y);
            this.Z.SetViewModel(modelXYZModel.Z);
        }
    }
}
