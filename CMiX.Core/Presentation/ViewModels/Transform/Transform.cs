// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Transform : ObservableObject, IPrefab, IControl
    {
        public Transform(TransformModel transformModel, CompositionService compositionService)
        {
            this.IsExpanded = true;
            this.ID = transformModel.ID;

            this.Name = this.GetType().Name;
            this.Enabled = transformModel.Enabled;
            this.Scale = new VectorXYZ(transformModel.Scale);
            this.Rotate = new VectorXYZ(transformModel.Rotate);
            this.Translate = new VectorXYZ(transformModel.Translate);
            this.Uniform = new Slider(nameof(Uniform), transformModel.Uniform);

            TransformModifier = new ModifierManager(transformModel.TransformModifier, new TransformModifierFactory(compositionService));
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public ModifierManager TransformModifier { get; set; }
        public CompositionService CompositionService { get; set; }

        public VectorXYZ Scale { get; set; }
        public VectorXYZ Rotate { get; set; }
        public VectorXYZ Translate { get; set; }
        public Slider Uniform { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void SetViewModel(IModel model)
        {
            TransformModel transformModel = model as TransformModel;
            this.ID = transformModel.ID;
            this.TransformModifier.SetViewModel(transformModel.TransformModifier);
            this.Scale.SetViewModel(transformModel.Scale);
            this.Rotate.SetViewModel(transformModel.Rotate);
            this.Translate.SetViewModel(transformModel.Translate);
            this.Uniform.SetViewModel(transformModel.Uniform);
        }

        public IModel GetModel()
        {
            TransformModel model = new TransformModel();

            model.ID = this.ID;
            model.TransformModifier = (ModifierManagerModel)this.TransformModifier.GetModel();
            model.Scale = (VectorXYZModel)this.Scale.GetModel();
            model.Translate = (VectorXYZModel)this.Translate.GetModel();
            model.Rotate = (VectorXYZModel)this.Rotate.GetModel();
            model.Uniform = (SliderModel)this.Uniform.GetModel();

            return model;
        }

        public void Dispose()
        {

        }
    }
}
