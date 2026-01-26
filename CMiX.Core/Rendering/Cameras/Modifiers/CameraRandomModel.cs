// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public record CameraRandomModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public GenericValueModel<bool> PingPong { get; set; } = new(false);
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
        public EasingModel Easing { get; set; } = new();
        public GenericValueModel<float> Width { get; set; } = new(0.0f);
        public GenericValueModel<float> To { get; set; } = new(1.0f);
        public GenericValueModel<CameraAxis> Axis { get; set; } = new(CameraAxis.Zoom);
    }
}
