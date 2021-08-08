// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Instancer : ObservableObject, IControl
    {
        public Instancer(InstancerModel instancerModel)
        {
            this.ID = instancerModel.ID;

            Transform = new Transform(instancerModel.Transform);
            TransformModifier = new TransformModifier(instancerModel.TransformModifierModel);

            NoAspectRatio = false;
        }


        public Guid ID { get; set; }
        public Transform Transform { get; set; }
        public TransformModifier TransformModifier { get; set; }

        private bool _noAspectRatio;
        public bool NoAspectRatio
        {
            get => _noAspectRatio;
            set => SetProperty(ref _noAspectRatio, value);
        }


        public void SetViewModel(IModel model)
        {
            InstancerModel instancerModel = model as InstancerModel;
            this.ID = instancerModel.ID;
            this.TransformModifier.SetViewModel(instancerModel.TransformModifierModel);
            this.Transform.SetViewModel(instancerModel.Transform);
            this.NoAspectRatio = instancerModel.NoAspectRatio;
        }

        public IModel GetModel()
        {
            InstancerModel model = new InstancerModel();
            model.ID = this.ID;
            model.TransformModifierModel = (TransformModifierModel)this.TransformModifier.GetModel();
            model.Transform = (TransformModel)this.Transform.GetModel();
            model.NoAspectRatio = this.NoAspectRatio;
            return model;
        }
    }
}
