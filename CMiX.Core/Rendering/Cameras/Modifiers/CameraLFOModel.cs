// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public record CameraLFOModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManagerModel BeatModifierManager { get; set; } = new();
        public GenericValueModel<bool> Yaw { get; set; } = new(false);
        public GenericValueModel<bool> Pitch { get; set; } = new(false);
        public GenericValueModel<bool> Zoom { get; set; } = new(false);
        public GenericValueModel<bool> PingPong { get; set; } = new(false);
        public GenericValueModel<float> From { get; set; } = new(0.0f);
        public GenericValueModel<float> To { get; set; } = new(1.0f);
        public GenericValueModel<CameraAxis> Axis { get; set; } = new(CameraAxis.Zoom);
        public PrefabServiceModel PrefabService { get; set; } = new();
    }
}
