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
            this.XYZ = new Vector3Model();
            Visible = new BooleanValueModel(true);
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public Guid ID { get; set; }
        public Vector3Model XYZ { get; set; }
        public BooleanValueModel Visible { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; internal set; }
    }
}
