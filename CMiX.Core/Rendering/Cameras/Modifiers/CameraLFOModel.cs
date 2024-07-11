// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraLFOModel : IPrefabModel
    {
        public CameraLFOModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            BeatModifierManager = new PrefabManagerModel();
            Yaw = new GenericValueModel<bool>(false);
            Pitch = new GenericValueModel<bool>(false);
            Zoom = new GenericValueModel<bool>(false);
            PingPong = new GenericValueModel<bool>(false);
            From = new GenericValueModel<float>(0.0f);
            To = new GenericValueModel<float>(1.0f);
            Axis = new GenericValueModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public GenericValueModel<bool> Yaw { get; set; }
        public GenericValueModel<bool> Pitch { get; set; }
        public GenericValueModel<bool> Zoom { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public GenericValueModel<float> From { get; set; }
        public GenericValueModel<float> To { get; set; }
        public GenericValueModel<CameraAxis> Axis { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
    }
}
