// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Mapping;
using CMiX.Core.Materials;
using CMiX.Core.Networking;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Services
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
            //EntityManager = new PrefabManager<Entity>(ID, this, this.EntityRepository);

            MaterialRepository = new PrefabRepository<Material>(prefabDataBase);
            MaterialManager = new PrefabManager<Material>(ID, this, this.MaterialRepository);

            LightEntityRepository = new PrefabRepository<LightEntity>(prefabDataBase);
            CameraRepository = new PrefabRepository<Camera>(prefabDataBase);
            //ControlMessenger = new ControlMessenger();
        }

        public Guid ID { get; set; }
        public IMapper Mapper { get; set; }
        public IPrefabDataBase PrefabDataBase { get; set; }
        public PrefabFactory PrefabFactory { get; set; }
        //public ControlMessenger ControlMessenger { get; set; }
        public PrefabRepository<Layer> LayerRepository { get; set; }
        public PrefabRepository<Composition> CompositionRepository { get; set; }

        public MasterBeat MasterBeat { get; set; }

        //ENTITY
        public PrefabRepository<Entity> EntityRepository { get; set; }
        //public PrefabManager<Entity> EntityManager { get; set; }


        //MATERIAL
        public PrefabRepository<Material> MaterialRepository { get; set; }
        public PrefabManager<Material> MaterialManager { get; set; }


        //CAMERA
        public PrefabRepository<Camera> CameraRepository { get; set; }
        public PrefabManager<Camera> CameraManager { get; set; }


        public PrefabRepository<LightEntity> LightEntityRepository { get; set; }


        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }
    }
}
