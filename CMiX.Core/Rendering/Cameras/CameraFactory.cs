// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Services;

namespace CMiX.Core.Rendering.Cameras
{
    public class CameraFactory : IPrefabFactory
    {
        public CameraFactory(CompositionService compositionService)
        {
            EntityRepository = compositionService.EntityRepository;
        }

        PrefabRepository EntityRepository { get; set; }
        public bool AppliesTo(Type type)
        {
            return (typeof(Camera).Equals(type) || typeof(CameraModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var cameraSettings = new CameraSettings();
            var modifierManager = new ModifierManager(new CameraModifierFactory());
            var camera = new Camera(prefabService, cameraSettings, modifierManager);
            EntityRepository.AddPrefab(camera);

            return camera;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var camera = ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
            EntityRepository.AddPrefab(camera);

            return camera;
        }
    }
}
