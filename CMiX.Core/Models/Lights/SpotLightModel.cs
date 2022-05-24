// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class SpotLightModel : ILightModel
    {
        public SpotLightModel()
        {
            this.ID = Guid.NewGuid();

            Enabled = true;

            PositionX = new SliderModel();
            PositionY = new SliderModel();
            PositionZ = new SliderModel();

            TargetX = new SliderModel();
            TargetY = new SliderModel();
            TargetZ = new SliderModel();

            Intensity = new SliderModel();
            Range = new SliderModel();
            Angle = new SliderModel();

            ColorSelectorModel = new ColorSelectorModel();
        }


        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public SliderModel PositionX { get; set; }
        public SliderModel PositionY { get; set; }
        public SliderModel PositionZ { get; set; }

        public SliderModel TargetX { get; set; }
        public SliderModel TargetY { get; set; }
        public SliderModel TargetZ { get; set; }

        public SliderModel Intensity { get; set; }
        public SliderModel Range { get; set; }
        public SliderModel Angle { get; set; }

        public ColorSelectorModel ColorSelectorModel { get; set; }
    }
}
