// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformSRT : ObservableObject, IControl, ITransformModifier
    {
        public TransformSRT(TransformSRTModel transformModel)
        {
            this.IsExpanded = true;
            this.ID = transformModel.ID;
            this.Enabled = transformModel.Enabled;
            this.Visible = new ToggleButton(transformModel.Visible);

            Uniform = new Slider(nameof(Uniform), transformModel.Uniform);
            Translate = new Translate(transformModel.Translate);
            Scale = new Scale(transformModel.Scale);
            Rotation = new Rotation(transformModel.Rotation);
            Mode = new ComboBox<ModifierMode>(transformModel.Mode);
        }


        public Guid ID { get; set; }
        public Slider Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void SetViewModel(IModel model)
        {
            TransformSRTModel transformModel = model as TransformSRTModel;

            this.ID = transformModel.ID;
            this.Uniform.SetViewModel(transformModel.Uniform);
            this.Translate.SetViewModel(transformModel.Translate);
            this.Scale.SetViewModel(transformModel.Scale);
            this.Rotation.SetViewModel(transformModel.Rotation);
            this.Mode.SetViewModel(transformModel.Mode);
        }

        public IModel GetModel()
        {
            TransformSRTModel model = new TransformSRTModel();

            model.ID = this.ID;
            model.Uniform = (SliderModel)this.Uniform.GetModel();
            model.Translate = (TranslateModel)this.Translate.GetModel();
            model.Scale = (ScaleModel)this.Scale.GetModel();
            model.Rotation = (RotationModel)this.Rotation.GetModel();
            model.Mode = (ComboBoxModel<ModifierMode>)this.Mode.GetModel();

            return model;
        }

        public void Dispose()
        {

        }
    }
}
