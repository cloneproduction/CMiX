// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Observer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Transform : ObservableObject, IControl, IObserver
    {
        public Transform(TransformModel transformModel)
        {
            this.ID = transformModel.ID;
            Translate = new Translate(nameof(Translate), transformModel.TranslateModel);
            Scale = new Scale(nameof(Scale), transformModel.ScaleModel);
            Rotation = new Rotation(nameof(Rotation), transformModel.RotationModel);
            Is3D = false;
        }


        public Guid ID { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }

        private bool _is3D;
        public bool Is3D
        {
            get => _is3D;
            set => SetProperty(ref _is3D, value);
        }


        public void Update(int count)
        {
            //this.Count = count;
        }



        public void SetViewModel(IModel model)
        {
            TransformModel transformModel = model as TransformModel;
            this.ID = transformModel.ID;
            this.Translate.SetViewModel(transformModel.TranslateModel);
            this.Scale.SetViewModel(transformModel.ScaleModel);
            this.Rotation.SetViewModel(transformModel.RotationModel);
        }

        public IModel GetModel()
        {
            TransformModel model = new TransformModel();
            model.ID = this.ID;
            model.TranslateModel = (TranslateModel)this.Translate.GetModel();
            model.ScaleModel = (ScaleModel)this.Scale.GetModel();
            model.RotationModel = (RotationModel)this.Rotation.GetModel();
            model.Is3D = this.Is3D;
            return model;
        }
    }
}
