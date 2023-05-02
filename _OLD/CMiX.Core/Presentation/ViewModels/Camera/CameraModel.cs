// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class CameraModel : IPrefabModel
    {
        public CameraModel()
        {
            this.ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();

            FOV = new FloatValueModel();
            FOV.Value = 0.09f;

            Distance = new FloatValueModel();
            Distance.Value = -10f;

            Yaw = new FloatValueModel(0.0f);

            Pitch = new FloatValueModel(0.0f);

            Target = new Vector3Model();

            FarClip = new FloatValueModel(100f);
            NearClip = new FloatValueModel(0.05f);

            Projection = new BooleanValueModel();
            CameraTransformModifierManager = new ModifierManagerModel();
            Name = "Camera";
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }

        public FloatValueModel FOV { get; set; }
        public FloatValueModel Distance { get; set; }

        public FloatValueModel Yaw { get; set; }
        public FloatValueModel Pitch { get; set; }

        public Vector3Model Target { get; set; }
        public string Name { get; internal set; }
        public FloatValueModel NearClip { get; internal set; }
        public FloatValueModel FarClip { get; internal set; }
        public BooleanValueModel Projection { get; internal set; }
        public ModifierManagerModel CameraTransformModifierManager { get; internal set; }
    }
}
