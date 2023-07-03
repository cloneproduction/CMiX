// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Animations;
using CMiX.Core.Mapping;
using CMiX.Core.Prefab;

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
            CompositionRepository = new PrefabRepository(prefabDataBase);
            LayerRepository = new PrefabRepository(prefabDataBase);

            EntityRepository = new PrefabRepository(prefabDataBase);

            MaterialRepository = new PrefabRepository(prefabDataBase);
            MaterialManager = new PrefabManager(ID, this, this.MaterialRepository);

            LightEntityRepository = new PrefabRepository(prefabDataBase);
            CameraRepository = new PrefabRepository(prefabDataBase);
        }

        public Guid ID { get; set; }
        public IMapper Mapper { get; set; }
        public IPrefabDataBase PrefabDataBase { get; set; }
        public PrefabFactory PrefabFactory { get; set; }
        public PrefabRepository LayerRepository { get; set; }
        public PrefabRepository CompositionRepository { get; set; }

        public MasterBeat MasterBeat { get; set; }

        //ENTITY
        public PrefabRepository EntityRepository { get; set; }


        //MATERIAL
        public PrefabRepository MaterialRepository { get; set; }
        public PrefabManager MaterialManager { get; set; }


        //CAMERA
        public PrefabRepository CameraRepository { get; set; }
        public PrefabManager CameraManager { get; set; }


        public PrefabRepository LightEntityRepository { get; set; }


        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }
    }
}
