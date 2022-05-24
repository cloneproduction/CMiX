// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RotationModel : IModifierModel
    {
        public RotationModel()
        {
            this.ID = Guid.NewGuid();
            this.Enabled = true;
            this.XYZ = new VectorXYZModel();
            Visible = new ToggleButtonModel(true);
            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public VectorXYZModel XYZ { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
    }
}
