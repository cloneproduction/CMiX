// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapper;
using CMiX.Core.Presentations.Components;
using CMiX.Core.Presentations.Materials;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Services;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Service
{
    public class CompositionService : IService
    {
        public CompositionService(IPrefabDataBase prefabDataBase)
        {
            var mappingProfile = new MappingProfile();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());

                foreach (var profile in mappingProfile.Profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            Mapper = config.CreateMapper();

            ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
            
            PrefabDataBase = prefabDataBase;
            PrefabFactory = new PrefabFactory(this);

            CompositionRepository = new PrefabRepository<Composition>(prefabDataBase);
            LayerRepository = new PrefabRepository<Layer>(prefabDataBase);
            EntityRepository = new PrefabRepository<Entity>(prefabDataBase);
            MaterialRepository = new PrefabRepository<Material>(prefabDataBase);
            LightEntityRepository = new PrefabRepository<LightEntity>(prefabDataBase);
            CameraRepository = new PrefabRepository<Camera>(prefabDataBase);

            ControlMessenger = new ControlMessenger();
        }

        public IMapper Mapper { get; set; }
        public Guid ID { get; set; }
        public IPrefabDataBase PrefabDataBase { get; set; }
        public PrefabFactory PrefabFactory { get; set; }
        public ControlMessenger ControlMessenger { get; set; }

        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }

        public PrefabRepository<Layer> LayerRepository { get; set; }
        public PrefabRepository<Composition> CompositionRepository { get; set; }
        public PrefabRepository<Entity> EntityRepository { get; set; }
        public PrefabRepository<Material> MaterialRepository { get; set; }
        public PrefabRepository<Texture> TextureRepository { get; set; }
        public PrefabRepository<LightEntity> LightEntityRepository { get; set; }
        public PrefabRepository<Camera> CameraRepository { get; set; }
    }
}
