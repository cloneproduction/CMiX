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
            ModifierManager = new PrefabManagerModel();
            Settings = new CameraSettingsModel();
        }

        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public CameraSettingsModel Settings { get; set; }

        public PrefabManagerModel ModifierManager { get; set; }
    }
}
