// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapper;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels.Service
{
    public class CompositionService : ObservableRecipient, IService
    {
        public CompositionService(IPrefabDataBase prefabDataBase)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });

            Mapper = config.CreateMapper();

            ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
            
            //Guid LayerManagerID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            //Guid ModelEntityManagerID = Guid.Parse("00000000-0000-0000-0000-000000000003");
            //Guid TextureManagerID = Guid.Parse("00000000-0000-0000-0000-000000000004");
            //Guid ColorationManagerID = Guid.Parse("00000000-0000-0000-0000-000000000005");
            //Guid MaterialManagerID = Guid.Parse("00000000-0000-0000-0000-000000000006");
            //Guid MeshManagerID = Guid.Parse("00000000-0000-0000-0000-000000000007");
            //Guid LightEntityManagerID = Guid.Parse("00000000-0000-0000-0000-000000000008");
            //Guid CameraManagerID = Guid.Parse("00000000-0000-0000-0000-000000000009");
            //Guid TransformManagerID = Guid.Parse("00000000-0000-0000-0000-000000000010");
            //Guid TextureTransformID = Guid.Parse("00000000-0000-0000-0000-000000000011");
            Guid MasterBeatManagerID = Guid.Parse("00000000-0000-0000-0000-000000000012");

            PrefabDataBase = prefabDataBase;
            
            PrefabFactory = new PrefabFactory(this);

            CompositionRepository = new PrefabRepository<Composition>(prefabDataBase);
            LayerRepository = new PrefabRepository<Layer>(prefabDataBase);
            EntityRepository = new PrefabRepository<Entity>(prefabDataBase);
            MaterialRepository = new PrefabRepository<Material>(prefabDataBase);
            MeshRepository = new PrefabRepository<Mesh>(prefabDataBase);
            LightEntityRepository = new PrefabRepository<LightEntity>(prefabDataBase);
            CameraRepository = new PrefabRepository<Camera>(prefabDataBase);
            MasterBeatRepository = new PrefabRepository<MasterBeat>(prefabDataBase);

            MasterBeatManager = new PrefabManager<MasterBeat>(MasterBeatManagerID, this, MasterBeatRepository);

            IsActive = true;
        }

        public IMapper Mapper { get; set; }
        public Guid ID { get; set; }
        public IPrefabDataBase PrefabDataBase { get; set; }
        public PrefabFactory PrefabFactory { get; set; }

 
        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }

        public PrefabManager<MasterBeat> MasterBeatManager { get; set; }


        public PrefabRepository<Layer> LayerRepository { get; set; }
        public PrefabRepository<Composition> CompositionRepository { get; set; }
        public PrefabRepository<MasterBeat> MasterBeatRepository { get; set; }
        public PrefabRepository<Entity> EntityRepository { get; set; }
        public PrefabRepository<Material> MaterialRepository { get; set; }
        public PrefabRepository<Texture> TextureRepository { get; set; }
        public PrefabRepository<LightEntity> LightEntityRepository { get; set; }
        public PrefabRepository<Mesh> MeshRepository { get; set; }
        public PrefabRepository<Camera> CameraRepository { get; set; }
    }
}
