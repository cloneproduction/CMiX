// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class CameraModel : IModel
    {
        public CameraModel()
        {
            this.ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();

            FOV = new SliderModel();
            FOV.Amount = 0.09f;

            Distance = new SliderModel();
            Distance.Amount = -10f;

            Yaw = new SliderModel();
            Yaw.Amount = 0.15f;

            Pitch = new SliderModel();
            Pitch.Amount = -0.08f;

            TargetX = new SliderModel();
            TargetY = new SliderModel();
            TargetZ = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }

        public SliderModel FOV { get; set; }
        public SliderModel Distance { get; set; }

        public SliderModel Yaw { get; set; }
        public SliderModel Pitch { get; set; }

        public SliderModel TargetX { get; set; }
        public SliderModel TargetY { get; set; }
        public SliderModel TargetZ { get; set; }
    }
}
