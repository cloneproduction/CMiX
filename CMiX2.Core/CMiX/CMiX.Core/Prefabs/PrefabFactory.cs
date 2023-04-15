// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Materials;
using CMiX.Core.Texturing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;

namespace CMiX.Core.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        private CompositionService CompositionService { get; set; }

        public IPrefab CreatePrefab(Type type)
        {
            if (type == typeof(Layer))
                return new Layer(new LayerModel(), CompositionService);

            if (type == typeof(Composition))
                return new Composition(new CompositionModel(), CompositionService);

            if (type == typeof(Entity))
                return new Entity(new EntityModel(), CompositionService);

            if (type == typeof(Camera))
                return new Camera(new CameraModel());

            if (type == typeof(LightEntity))
                return new LightEntity(new LightEntityModel(), CompositionService);

            if (type == typeof(Texture))
                return new Texture(new TextureModel());

            if (type == typeof(Material))
                return new Material(new MaterialModel());

            return null;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            if (prefabModel is LayerModel layerModel)
                return new Layer(layerModel, CompositionService);

            if (prefabModel is CompositionModel compositionModel)
                return new Composition(compositionModel, CompositionService);

            if (prefabModel is EntityModel entityModel)
                return new Entity(entityModel, CompositionService);

            if (prefabModel is CameraModel cameraModel)
                return new Camera(cameraModel);

            if (prefabModel is LightEntityModel lightEntityModel)
                return new LightEntity(lightEntityModel, CompositionService);

            if (prefabModel is TextureModel textureModel)
                return new Texture(textureModel);

            if (prefabModel is MaterialModel materialModel)
                return new Material(materialModel);

            return null;
        }
    }
}
