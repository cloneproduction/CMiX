// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Scale : ObservableObject, ITransformModifier
    {
        public Scale(ScaleModel scaleModel)
        {
            this.ID = scaleModel.ID;
            this.Enabled = scaleModel.Enabled;

            Uniform = new Slider(nameof(Uniform), scaleModel.Uniform);
            XYZ = new VectorXYZ(scaleModel.XYZ);
            Visible = new ToggleButton(scaleModel.Visible);

            Mode = new ComboBox<ModifierMode>(scaleModel.Mode);
            IsExpanded = true;
        }


        public Guid ID { get; set; }

        public Slider Uniform { get; set; }
        public VectorXYZ XYZ { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public ComboBox<ModifierMode> Mode { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            ScaleModel model = new ScaleModel();

            model.ID = this.ID;
            model.Enabled = this.Enabled;
            model.Uniform = (SliderModel)this.Uniform.GetModel();
            model.Visible = (ToggleButtonModel)this.Visible.GetModel();
            model.XYZ = (VectorXYZModel)this.XYZ.GetModel();
            model.Mode = (ComboBoxModel<ModifierMode>)this.Mode.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            ScaleModel scaleModel = model as ScaleModel;

            this.ID = scaleModel.ID;
            this.Enabled = scaleModel.Enabled;
            this.Uniform.SetViewModel(scaleModel.Uniform);
            this.Visible.SetViewModel(scaleModel.Visible);
            this.XYZ.SetViewModel(scaleModel.XYZ);
            this.Mode.SetViewModel(scaleModel.Mode);
        }

        public void Dispose()
        {
            
        }
    }
}
