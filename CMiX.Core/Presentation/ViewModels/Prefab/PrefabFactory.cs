// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabFactory
    {
        public PrefabFactory()
        {

        }

        public IPrefab CreatePrefab(Type type)
        {
            if (type == typeof(Coloration))
                return new Coloration(new ColorationModel());

            if (type == typeof(Transform))
                return new Transform(new TransformModel());

            if (type == typeof(Entity))
                return new Entity(new EntityModel());

            if (type == typeof(Camera))
                return new Camera(new CameraModel());

            if (type == typeof(Mesh))
                return new Mesh(new MeshModel());

            if (type == typeof(LightEntity))
                return new LightEntity(new LightEntityModel());

            if (type == typeof(Texture))
                return new Texture(new TextureModel());

            if (type == typeof(Material))
                return new Material(new MaterialModel());

            return null;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            if (prefabModel is ColorationModel colorationModel)
                return new Coloration(colorationModel);

            if (prefabModel is TransformModel transformModel)
                return new Transform(transformModel);

            if (prefabModel is EntityModel entityModel)
                return new Entity(entityModel);

            if(prefabModel is CameraModel cameraModel)
                return new Camera(cameraModel);

            if(prefabModel is MeshModel meshModel)
                return new Mesh(meshModel);

            if(prefabModel is LightEntityModel lightEntityModel)
                return new LightEntity(lightEntityModel);

            if(prefabModel is TextureModel textureModel)
                return new Texture(textureModel);

            if(prefabModel is MaterialModel materialModel)
                return new Material(materialModel);

            return null;
        }
    }
}
