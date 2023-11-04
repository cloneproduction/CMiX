// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Services;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Texturing
{
    public class TextureFactory : IPrefabFactory
    {
        public TextureFactory(PrefabRepositories prefabRepositories, IMapper mapper) 
        {
            PrefabRepository = prefabRepositories.EntityRepository;
            Mapper = mapper;
        }

        PrefabRepository PrefabRepository { get; }
        IMapper Mapper { get; }
        public IPrefab GetPrefab(Guid id)
        {
            return PrefabRepository.GetPrefab(id);
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(Texture).Equals(type) || typeof(TextureModel).Equals(type));
        }

        Gradient CreateGradient(ControlMessenger controlMessenger)
        {
            var x = new IntegerValue(512, controlMessenger);
            var y = new IntegerValue(512, controlMessenger);
            var resolution = new Integer2(x, y);

            var from = new ColorValue(Color.FromArgb(255, 255, 255, 255), controlMessenger);
            var to = new ColorValue(Color.FromArgb(255, 0, 0, 0), controlMessenger);
            var gamma = new FloatValue(2.2f, controlMessenger);
            var horizontal = new BooleanValue(false, controlMessenger);

            return new Gradient(resolution, from, to, gamma, horizontal);
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            var controlMessenger = new ControlMessenger(Mapper);

            var modifierManager = new ModifierManager(new TextureModifierFactory());

            var textureSourceSelector = new TextureSourceSelector();
            var gradient = CreateGradient(controlMessenger);
            var bubbleNoise = new BubbleNoise();
            var image = new Image();
            var videoIn = new VideoIn();
            var videoPlayer = new VideoPlayer();
            var selectedAssetType = new IntegerValue();
            var typeWriter = new TypeWriter();
            var texture = new Texture(prefabService, modifierManager);

            PrefabRepository.AddPrefab(texture);
            return texture;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var texture = Mapper.Map(prefabModel, CreatePrefab(prefabService));
            PrefabRepository.AddPrefab(texture);
            return texture;
        }
    }
}
