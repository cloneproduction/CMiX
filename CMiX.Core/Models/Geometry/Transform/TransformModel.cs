// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class TransformModel : IModifierModel
    {
        public TransformModel()
        {
            this.ID = Guid.NewGuid();

            TranslateModel = new TranslateModel();
            ScaleModel = new ScaleModel();
            RotationModel = new RotationModel();
            Visible = new ToggleButtonModel(true);
            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TranslateModel TranslateModel { get; set; }
        public ScaleModel ScaleModel { get; set; }
        public RotationModel RotationModel { get; set; }



        public bool Is3D { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
    }
}
