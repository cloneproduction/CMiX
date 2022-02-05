// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class ColorationModel : IModel
    {
        public ColorationModel()
        {
            ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();
            ColorSelectorModel = new ColorSelectorModel();
            HueModel = new RangeControlModel();
            SatModel = new RangeControlModel();
            ValModel = new RangeControlModel();
        }

        public ColorationModel(Guid ID) :this()
        {
            this.ID = ID;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public RangeControlModel HueModel { get; set; }
        public RangeControlModel SatModel { get; set; }
        public RangeControlModel ValModel { get; set; }
    }
}
