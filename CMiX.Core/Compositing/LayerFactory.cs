// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Compositing
{
    public class LayerFactory : IPrefabFactory
    {
        public LayerFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            CompositionRepository = serviceProvider.GetRequiredService<PrefabRepositories>().CompositionRepository;
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }

        ControlMessenger ControlMessenger { get; set; }
        PrefabRepository CompositionRepository { get; set; }
        IServiceProvider ServiceProvider { get; set; }
        IMapper Mapper { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(Layer).Equals(type) || typeof(LayerModel).Equals(type));
        }

        public IPrefab GetPrefab(Guid id)
        {
            return CompositionRepository.GetPrefab(id);
        }

        //PrefabManagerSlot CreatePrefabManager()
        //{
        //    var entityFactory = new EntityFactory(ServiceProvider);
        //    var lightFactory = new LightFactory(ServiceProvider);
        //    var cameraFactory = new CameraFactory(ServiceProvider);
        //    var emptyPrefabFactory = new EmptyPrefabFactory();

        //    var prefabFactory = new PrefabFactory();
        //    prefabFactory.RegisterFactory(entityFactory);
        //    prefabFactory.RegisterFactory(lightFactory);
        //    prefabFactory.RegisterFactory(cameraFactory);
        //    prefabFactory.RegisterFactory(emptyPrefabFactory);

        //    var prefabSlotManager = new PrefabManagerSlot(prefabFactory);

        //    return prefabSlotManager;
        //}

        //AmbientOcclusion CreateAmbientOcclusion()
        //{
        //    var isEnabled = new BooleanValue(ControlMessenger);
        //    var samples = new IntegerValue(ControlMessenger);
        //    var projectionScale = new FloatValue(ControlMessenger);
        //    var intensity = new FloatValue(ControlMessenger);
        //    var sampleBias = new FloatValue(ControlMessenger);
        //    var sampleRadius = new FloatValue(ControlMessenger);
        //    var blurCount = new IntegerValue(ControlMessenger);
        //    var blurRadius = new FloatValue(ControlMessenger);
        //    var edgeSharpness = new FloatValue(ControlMessenger);

        //    return new AmbientOcclusion(isEnabled, samples, projectionScale, intensity, sampleBias, sampleRadius, blurCount, blurRadius, edgeSharpness);
        //}

        //LayerService CreateLayerService()
        //{
        //    var opacity = new FloatValue(ControlMessenger);
        //    var backgroundColor = new ColorValue(Color.FromArgb(255, 128, 128, 128), ControlMessenger);
        //    var blendMode = new GenericValue<BlendModeEnum>(ControlMessenger);
        //    var ambientOcclusion = CreateAmbientOcclusion();

        //    return new LayerService(opacity, backgroundColor, blendMode, ambientOcclusion);
        //}

        //LayerMaskService CreateMaskService()
        //{
        //    var isMask = new BooleanValue(ControlMessenger);
        //    var maskChannel = new GenericValue<MaskChannel>(ControlMessenger);
        //    var maskMode = new GenericValue<MaskMode>(ControlMessenger);
        //    var invert = new BooleanValue(ControlMessenger);

        //    return new LayerMaskService(isMask, maskChannel, maskMode, invert);
        //}

        //ModifierManager CreateTextureModifierManager()
        //{
        //    var textureModifierFactory = new TextureModifierFactory();
        //    var managerMessenger = new ManagerMessenger(Mapper);
        //    var modifierManager = new ModifierManager(textureModifierFactory, managerMessenger);

        //    return modifierManager;
        //}

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            //var layerService = CreateLayerService();
            //var layerMaskService = CreateMaskService();
            //var prefabManager = CreatePrefabManager();
            //var textureModifierManager = CreateTextureModifierManager();
            
            //var layer = new Layer(prefabService, layerService, layerMaskService, prefabManager, textureModifierManager);

            //CompositionRepository.AddPrefab(layer);

            //return layer;
            return null;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var layer = ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
            CompositionRepository.AddPrefab(layer);

            return layer;
        }
    }
}
