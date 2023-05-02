// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Materials;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;

namespace CMiX.Core.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        private CompositionService CompositionService { get; set; }

        private int nameCount = 0;

        private IPrefab Build(IPrefab prefab)
        {
            prefab.Name.Value = prefab.GetType().Name + "." + nameCount.ToString("000");
            nameCount++;
            prefab.IsSelected.Value = true;
            return prefab;
        }

        public IPrefab CreatePrefab(Type type)
        {
            if (type == typeof(EmptyPrefab))
                return Build(new EmptyPrefab(new EmptyPrefabModel()));

            if (type == typeof(Layer))
                return Build(new Layer(new LayerModel(), CompositionService));

            if (type == typeof(Composition))
                return Build(new Composition(new CompositionModel(), CompositionService));

            if (type == typeof(Entity))
                return Build(new Entity(new EntityModel(), CompositionService));

            if (type == typeof(Camera))
                return Build(new Camera(new CameraModel()));

            if (type == typeof(LightEntity))
                return Build(new LightEntity(new LightEntityModel(), CompositionService));

            if (type == typeof(Texture))
                return Build(new Texture(new TextureModel()));

            if (type == typeof(Material))
                return Build(new Material(new MaterialModel()));

            return null;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            if (prefabModel is EmptyPrefabModel emptyPrefabModel)
                return Build(new EmptyPrefab(emptyPrefabModel));

            if (prefabModel is LayerModel layerModel)
                return Build(new Layer(layerModel, CompositionService));

            if (prefabModel is CompositionModel compositionModel)
                return Build(new Composition(compositionModel, CompositionService));

            if (prefabModel is EntityModel entityModel)
                return Build(new Entity(entityModel, CompositionService));

            if (prefabModel is CameraModel cameraModel)
                return Build(new Camera(cameraModel));

            if (prefabModel is LightEntityModel lightEntityModel)
                return Build(new LightEntity(lightEntityModel, CompositionService));

            if (prefabModel is TextureModel textureModel)
                return Build(new Texture(textureModel));

            if (prefabModel is MaterialModel materialModel)
                return Build(new Material(materialModel));

            return null;
        }
    }
}
