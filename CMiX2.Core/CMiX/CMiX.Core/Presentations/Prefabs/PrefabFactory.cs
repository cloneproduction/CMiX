// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Entities.Lights;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Prefabs
{
    public class PrefabFactory
    {
        public PrefabFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        public CompositionService CompositionService { get; set; }

        public IPrefab CreatePrefab(Type type)
        {
            if (type == typeof(Layer))
                return new Layer(new LayerModel(), CompositionService);

            if (type == typeof(Composition))
                return new Composition(new CompositionModel(), CompositionService);

            if (type == typeof(MasterBeat))
                return new MasterBeat(new MasterBeatModel(), CompositionService);

            if (type == typeof(Entity))
                return new Entity(new EntityModel(), CompositionService);

            if (type == typeof(Camera))
                return new Camera(new CameraModel(), CompositionService);

            if (type == typeof(Mesh))
                return new Mesh(new MeshModel(), CompositionService);

            if (type == typeof(LightEntity))
                return new LightEntity(new LightEntityModel(), CompositionService);

            if (type == typeof(Texture))
                return new Texture(new TextureModel(), CompositionService);

            if (type == typeof(Material))
                return new Material(new MaterialModel(), CompositionService);

            return null;
        }

        public IPrefab CreatePrefab(IPrefabModel prefabModel)
        {
            if (prefabModel is LayerModel layerModel)
                return new Layer(layerModel, CompositionService);

            if (prefabModel is CompositionModel compositionModel)
                return new Composition(compositionModel, CompositionService);

            if (prefabModel is MasterBeatModel masterBeatModel)
                return new MasterBeat(masterBeatModel, CompositionService);

            if (prefabModel is EntityModel entityModel)
                return new Entity(entityModel, CompositionService);

            if(prefabModel is CameraModel cameraModel)
                return new Camera(cameraModel, CompositionService);

            if(prefabModel is MeshModel meshModel)
                return new Mesh(meshModel, CompositionService);

            if(prefabModel is LightEntityModel lightEntityModel)
                return new LightEntity(lightEntityModel, CompositionService);

            if(prefabModel is TextureModel textureModel)
                return new Texture(textureModel, CompositionService);

            if(prefabModel is MaterialModel materialModel)
                return new Material(materialModel, CompositionService);

            return null;
        }
    }
}
