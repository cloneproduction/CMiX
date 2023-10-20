// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class LayerFactory : IPrefabFactory
    {
        public LayerFactory(PrefabRepository layerRepository)
        {
            PrefabRepository = layerRepository;
        }

        PrefabRepository PrefabRepository { get; set; }

        public bool AppliesTo(Type type)
        {
            return (typeof(Layer).Equals(type) || typeof(LayerModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var opacity = new FloatValue(1.0f);
            var backgroundColor = new ColorValue(Color.FromArgb(255, 128, 128, 128));
            var blendMode = new GenericValue<BlendModeEnum>(BlendModeEnum.Normal);
            var ambientOcclusion = new AmbientOcclusion();

            var layerService = new LayerService(opacity, backgroundColor, blendMode, ambientOcclusion);
            var layerMaskService = new LayerMaskService();

            var factories = new List<IPrefabFactory>();
            var entityFactory = new EntityFactory(PrefabRepository);
            var lightFactory = new LightFactory(PrefabRepository);
            var cameraFactory = new CameraFactory();
            var prefabFactory = new PrefabFactory(factories);

            var prefabSlotManager = new PrefabManagerSlot(PrefabRepository, prefabFactory);
            var modifierManager = new ModifierManager(new TextureModifierFactory());

            return new Layer(prefabService, layerService, layerMaskService, prefabSlotManager, modifierManager);
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            return ControlMessenger.Mapper.Map(prefabModel, CreatePrefab(prefabService));
        }
    }
}
