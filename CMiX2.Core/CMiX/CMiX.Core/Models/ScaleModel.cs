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
            Uniform = new FloatValueModel(1.0f);
            XYZ = new Vector3Model(1.0f, 1.0f, 1.0f);
            Visible = new BooleanValueModel(true);
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.ToSpread);
        }

        public Guid ID { get; set; }
        public FloatValueModel Uniform { get; set; }
        public Vector3Model XYZ { get; set; }
        public BooleanValueModel Visible { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; internal set; }
    }
}
