// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class LayerFactory : IPrefabFactory
    {
        public LayerFactory(PrefabRepositories prefabRepositories, ControlMessenger controlMessenger)
        {
            PrefabRepositories = prefabRepositories;
            CompositionRepository = prefabRepositories.CompositionRepository;
            ControlMessenger = controlMessenger;
        }

        ControlMessenger ControlMessenger { get; set; }
        PrefabRepository CompositionRepository { get; set; }
        PrefabRepositories PrefabRepositories { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(Layer).Equals(type) || typeof(LayerModel).Equals(type));
        }

        public IPrefab GetPrefab(Guid id)
        {
            return CompositionRepository.GetPrefab(id);
        }

        PrefabManagerSlot CreatePrefabManager()
        {
            var entityFactory = new EntityFactory(PrefabRepositories);
            var lightFactory = new LightFactory(PrefabRepositories);
            var cameraFactory = new CameraFactory(PrefabRepositories);
            var emptyPrefabFactory = new EmptyPrefabFactory();

            var prefabFactory = new PrefabFactory();
            prefabFactory.RegisterFactory(entityFactory);
            prefabFactory.RegisterFactory(lightFactory);
            prefabFactory.RegisterFactory(cameraFactory);
            prefabFactory.RegisterFactory(emptyPrefabFactory);

            var prefabSlotManager = new PrefabManagerSlot(prefabFactory);

            return prefabSlotManager;
        }

        LayerService CreateLayerService()
        {
            var opacity = new FloatValue(1.0f);
            var backgroundColor = new ColorValue(Color.FromArgb(255, 128, 128, 128), ControlMessenger);
            var blendMode = new GenericValue<BlendModeEnum>(BlendModeEnum.Normal);
            var ambientOcclusion = new AmbientOcclusion();

            return new LayerService(opacity, backgroundColor, blendMode, ambientOcclusion);
        }

        LayerMaskService CreateMaskService()
        {
            var isMask = new BooleanValue(false, ControlMessenger);
            var maskChannel = new GenericValue<MaskChannel>();
            var maskMode = new GenericValue<MaskMode>();
            var invert = new BooleanValue(false, ControlMessenger);

            return new LayerMaskService(isMask, maskChannel, maskMode, invert);
        }

        ModifierManager CreateTextureModifierManager()
        {
            var textureModifierFactory = new TextureModifierFactory();
            var modifierManager = new ModifierManager(textureModifierFactory);

            return modifierManager;
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var layerService = CreateLayerService();
            var layerMaskService = CreateMaskService();
            var prefabManager = CreatePrefabManager();
            var textureModifierManager = CreateTextureModifierManager();
            
            var layer = new Layer(prefabService, layerService, layerMaskService, prefabManager, textureModifierManager);

            CompositionRepository.AddPrefab(layer);

            return layer;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var layer = ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
            CompositionRepository.AddPrefab(layer);

            return layer;
        }
    }
}
