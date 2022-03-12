// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Instancer : ObservableObject, IControl, IBeatable
    {
        public Instancer(InstancerModel instancerModel)
        {
            this.ID = instancerModel.ID;

            Transform = new Transform(instancerModel.Transform);
            ModifierManager = new ModifierManager(instancerModel.ModifierManagerModel, new TransformModifierFactory());

            NoAspectRatio = false;
        }


        public Guid ID { get; set; }
        public Transform Transform { get; set; }


        public ModifierManager ModifierManager { get; set; }

        private bool _noAspectRatio;
        public bool NoAspectRatio
        {
            get => _noAspectRatio;
            set => SetProperty(ref _noAspectRatio, value);
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            ModifierManager.SetMasterBeat(masterBeat);
        }

        public void SetViewModel(IModel model)
        {
            InstancerModel instancerModel = model as InstancerModel;
            this.ID = instancerModel.ID;
            this.ModifierManager.SetViewModel(instancerModel.ModifierManagerModel);
            this.Transform.SetViewModel(instancerModel.Transform);
            this.NoAspectRatio = instancerModel.NoAspectRatio;
        }

        public IModel GetModel()
        {
            InstancerModel model = new InstancerModel();
            model.ID = this.ID;
            model.ModifierManagerModel = (ModifierManagerModel)this.ModifierManager.GetModel();
            model.Transform = (TransformModel)this.Transform.GetModel();
            model.NoAspectRatio = this.NoAspectRatio;
            return model;
        }
    }
}
