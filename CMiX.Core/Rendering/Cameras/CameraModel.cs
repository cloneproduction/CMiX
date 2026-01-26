// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras
{
    public record CameraModel : IPrefabModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; init; } = new();
        public CameraSettingsModel Settings { get; init; } = new();
        public PrefabManagerModel ModifierManager { get; init; } = new();
    }
}
