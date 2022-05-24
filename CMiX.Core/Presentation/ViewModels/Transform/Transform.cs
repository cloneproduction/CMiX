// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Transform : ObservableObject, IControl, ITransformModifier
    {
        public Transform(TransformModel transformModel)
        {
            this.IsExpanded = true;
            this.ID = transformModel.ID;
            this.Enabled = transformModel.Enabled;
            this.Visible = new ToggleButton(transformModel.Visible);
            Translate = new Translate(transformModel.TranslateModel);
            Scale = new Scale(transformModel.ScaleModel);
            Rotation = new Rotation(transformModel.RotationModel);
            Mode = new ComboBox<ModifierMode>(transformModel.Mode);
        }


        public Guid ID { get; set; }
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
            TransformModel transformModel = model as TransformModel;

            this.ID = transformModel.ID;
            this.Translate.SetViewModel(transformModel.TranslateModel);
            this.Scale.SetViewModel(transformModel.ScaleModel);
            this.Rotation.SetViewModel(transformModel.RotationModel);
            this.Mode.SetViewModel(transformModel.Mode);
        }

        public IModel GetModel()
        {
            TransformModel model = new TransformModel();

            model.ID = this.ID;
            model.TranslateModel = (TranslateModel)this.Translate.GetModel();
            model.ScaleModel = (ScaleModel)this.Scale.GetModel();
            model.RotationModel = (RotationModel)this.Rotation.GetModel();
            model.Mode = (ComboBoxModel<ModifierMode>)this.Mode.GetModel();

            return model;
        }

        public void Dispose()
        {

        }
    }
}
