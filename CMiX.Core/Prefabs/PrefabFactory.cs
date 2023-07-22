// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Materials;
using CMiX.Core.Networking;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;

namespace CMiX.Core.Prefab
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
            if (type == typeof(Composition))
                return Build(new Composition(CompositionService));

            if (type == typeof(EmptyPrefab))
                return Build(new EmptyPrefab());

            if (type == typeof(Layer))
                return Build(new Layer(CompositionService));

            if (type == typeof(Entity))
                return Build(new Entity(CompositionService));

            if (type == typeof(Camera))
                return Build(new Camera());

            if (type == typeof(LightEntity))
                return Build(new LightEntity(CompositionService));

            if (type == typeof(Texture))
                return Build(new Texture());

            return null;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            if (prefabModel is CompositionModel compositionModel)
                return Build(ControlMessenger.Mapper.Map(compositionModel, new Composition(CompositionService)));

            if (prefabModel is EmptyPrefabModel emptyPrefabModel)
                return Build(ControlMessenger.Mapper.Map(emptyPrefabModel, new EmptyPrefab()));

            if (prefabModel is LayerModel layerModel)
                return Build(ControlMessenger.Mapper.Map(layerModel, new Layer(CompositionService)));

            if (prefabModel is EntityModel entityModel)
                return Build(ControlMessenger.Mapper.Map(entityModel, new Entity(CompositionService)));

            if (prefabModel is CameraModel cameraModel)
                return Build(ControlMessenger.Mapper.Map(cameraModel, new Camera()));

            if (prefabModel is LightEntityModel lightEntityModel)
                return Build(ControlMessenger.Mapper.Map(lightEntityModel, new LightEntity(CompositionService)));

            if (prefabModel is TextureModel textureModel)
                return Build(ControlMessenger.Mapper.Map(textureModel, new Texture()));

            return null;
        }
    }
}
