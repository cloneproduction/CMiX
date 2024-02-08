// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraModel : IPrefabModel
    {
        public CameraModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            BeatModifierModel = new BeatModifierModel();
            FOV = new GenericValueModel<float>(0.09f);
            Distance = new GenericValueModel<float>(-10f);
            Yaw = new GenericValueModel<float>(0.0f);
            Pitch = new GenericValueModel<float>(0.0f);
            Target = new Vector3Model();
            FarClip = new GenericValueModel<float>(100f);
            NearClip = new GenericValueModel<float>(0.05f);
            Projection = new GenericValueModel<bool>();
            ModifierManager = new PrefabManagerModel();
            Name = new GenericValueModel<string>("Camera");
            Visibility = new GenericValueModel<bool>(true);
        }

        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
        public BeatModifierModel BeatModifierModel { get; set; }
        public GenericValueModel<float> FOV { get; set; }
        public GenericValueModel<float> Distance { get; set; }
        public GenericValueModel<float> Yaw { get; set; }
        public GenericValueModel<float> Pitch { get; set; }
        public Vector3Model Target { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public GenericValueModel<float> NearClip { get; set; }
        public GenericValueModel<float> FarClip { get; set; }
        public GenericValueModel<bool> Projection { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
    }
}
