// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;

namespace CMiX.Core.Models
{
    public class SpotLightModel : ILightModel
    {
        public SpotLightModel()
        {
            this.ID = Guid.NewGuid();
            Enabled = true;

            Position = new Vector3Model();
            Target = new Vector3Model();
            Intensity = new FloatValueModel();
            Range = new FloatValueModel();
            Angle = new FloatValueModel();

            ColorSelectorModel = new ColorSelectorModel();
        }


        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public Vector3Model Position { get; set; }
        public Vector3Model Target { get; set; }

        public FloatValueModel Intensity { get; set; }
        public FloatValueModel Range { get; set; }
        public FloatValueModel Angle { get; set; }

        public ColorSelectorModel ColorSelectorModel { get; set; }
    }
}
