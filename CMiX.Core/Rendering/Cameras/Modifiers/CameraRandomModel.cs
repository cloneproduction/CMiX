// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraRandomModel : IControlModel
    {
        public CameraRandomModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            PingPong = new GenericValueModel<bool>(false);
            BeatModifierManager = new PrefabManagerModel();
            Easing = new EasingModel();
            Width = new GenericValueModel<float>(0.0f);
            To = new GenericValueModel<float>(1.0f);
            Axis = new GenericValueModel<CameraAxis>(CameraAxis.Zoom);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<bool> PingPong { get; set; }
        public PrefabManagerModel BeatModifierManager { get; set; }
        public EasingModel Easing { get; set; }
        public GenericValueModel<float> Width { get; set; }
        public GenericValueModel<float> To { get; set; }
        public GenericValueModel<CameraAxis> Axis { get; set; }
    }
}
