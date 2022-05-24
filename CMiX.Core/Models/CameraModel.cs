// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class CameraModel : IPrefabModel
    {
        public CameraModel()
        {
            this.ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();

            FOV = new SliderModel();
            FOV.Amount = 0.09f;

            Distance = new SliderModel();
            Distance.Amount = -10f;

            Yaw = new SliderModel(0.0f);

            Pitch = new SliderModel(0.0f);

            TargetX = new SliderModel();
            TargetY = new SliderModel();
            TargetZ = new SliderModel();

            FarClip = new SliderModel(100f);
            NearClip = new SliderModel(0.05f);

            Projection = new ToggleButtonModel();

            Name = "Camera";
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
        public string Name { get; internal set; }
        public SliderModel NearClip { get; internal set; }
        public SliderModel FarClip { get; internal set; }
        public ToggleButtonModel Projection { get; internal set; }
    }
}
