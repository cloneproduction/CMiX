// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class ScaleModel : IModifierModel
    {
        public ScaleModel()
        {
            ID = Guid.NewGuid();
            Uniform = new SliderModel(1.0f);
            XYZ = new VectorXYZModel(1.0f, 1.0f, 1.0f);
            Visible = new ToggleButtonModel(true);
            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Uniform { get; set; }
        public VectorXYZModel XYZ { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
    }
}
