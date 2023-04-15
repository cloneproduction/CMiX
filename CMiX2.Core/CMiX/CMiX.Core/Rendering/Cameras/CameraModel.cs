// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraModel : IPrefabModel
    {
        public CameraModel()
        {
            ID = Guid.NewGuid();
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
            BeatModifierModel = new BeatModifierModel();
            FOV = new FloatValueModel(0.09f);
            Distance = new FloatValueModel(-10f);
            Yaw = new FloatValueModel(0.0f);
            Pitch = new FloatValueModel(0.0f);
            Target = new Vector3Model();
            FarClip = new FloatValueModel(100f);
            NearClip = new FloatValueModel(0.05f);
            Projection = new BooleanValueModel();
            CameraTransformModifierManager = new ModifierManagerModel();
            Name = new StringValueModel("Camera");
        }

        public Guid ID { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public FloatValueModel FOV { get; set; }
        public FloatValueModel Distance { get; set; }
        public FloatValueModel Yaw { get; set; }
        public FloatValueModel Pitch { get; set; }
        public Vector3Model Target { get; set; }
        public StringValueModel Name { get; set; }
        public FloatValueModel NearClip { get; set; }
        public FloatValueModel FarClip { get; set; }
        public BooleanValueModel Projection { get; set; }
        public ModifierManagerModel CameraTransformModifierManager { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
