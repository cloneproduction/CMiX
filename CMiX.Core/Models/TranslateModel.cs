// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;

namespace CMiX.Core.Models
{
    public class TranslateModel : IModifierModel
    {
        public TranslateModel()
        {
            this.ID = Guid.NewGuid();
            XYZ = new VectorXYZModel();
            Visible = new ToggleButtonModel(true);
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public VectorXYZModel XYZ { get; internal set; }
        public ToggleButtonModel Visible { get; set; }
    }
}
