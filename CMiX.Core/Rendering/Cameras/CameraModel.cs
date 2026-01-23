// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Rendering.Cameras
{
    public record CameraModel : IPrefabModel
    {
        public CameraModel()
        {
            ID = PrefabService.ID;
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; } = new();
        public CameraSettingsModel Settings { get; set; } = new();
        public PrefabManagerModel ModifierManager { get; set; } = new();
    }
}
