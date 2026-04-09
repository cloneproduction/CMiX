// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core
{
    public class AppInitializer
    {
        public AppInitializer(
            ControlActivationService activationService,
            PrefabManager textureManager,
            PrefabManager materialManager,
            PrefabManager entityManager,
            PrefabManager cameraManager,
            PrefabManager lightManager,
            PrefabManager beatManager,
            PrefabManager colorPaletteManager)
        {
            textureManager.Activate();
            materialManager.Activate();
            entityManager.Activate();
            cameraManager.Activate();
            lightManager.Activate();
            beatManager.Activate();
            colorPaletteManager.Activate();
        }
    }
}
