// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels.Prefab
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

            if (type == typeof(Transform))
                return new Transform(new TransformModel(), CompositionService);

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

            if (prefabModel is TransformModel transformModel)
                return new Transform(transformModel, CompositionService);

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
