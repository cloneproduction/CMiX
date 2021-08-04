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
            Zoom = new SliderModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public string Rotation { get; set; }
        public string LookAt { get; set; }
        public string View { get; set; }

        public BeatModifierModel BeatModifierModel { get; set; }
        public SliderModel FOV { get; set; }
        public SliderModel Zoom { get; set; }
    }
}
