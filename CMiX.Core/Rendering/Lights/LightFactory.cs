// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Entities.Lights;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering.Lights.Modifiers;
using CMiX.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Rendering.Lights
{
    public class LightFactory : IPrefabFactory
    {
        public LightFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Mapper = serviceProvider.GetRequiredService<IMapper>();
            LayerRepository = serviceProvider.GetRequiredService<PrefabRepositories>().LayerRepository;
        }

        IServiceProvider ServiceProvider { get; }
        PrefabRepository LayerRepository { get; set; }
        IMapper Mapper { get; set; }

        public IPrefab GetPrefab(Guid id)
        {
            return LayerRepository.GetPrefab(id);
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(LightEntity).Equals(type) || typeof(LightEntityModel).Equals(type));
        }

        public IPrefab CreatePrefab(PrefabService prefabService)
        {
            //var lightSettings = new LightSettings();
            //var modifierManager = new ModifierManager(new LightModifierFactory(), new ManagerMessenger(Mapper));
            //var lightEntity = new LightEntity(prefabService, lightSettings, modifierManager);
            //LayerRepository.AddPrefab(lightEntity);

            //return lightEntity;

            return null;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            //var lightEntity = Mapper.Map(prefabModel, CreatePrefab(prefabService));
            //LayerRepository.AddPrefab(lightEntity);

            //return lightEntity;

            return null;
        }
    }
}
