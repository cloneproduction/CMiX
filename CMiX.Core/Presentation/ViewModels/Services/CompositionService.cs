// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Service
{
    public class CompositionService : ObservableRecipient, IService
    {
        public CompositionService(IPrefabDataBase prefabDataBase)
        {
            ID = Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00");
            Guid CompositionManagerID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid LayerManagerID = Guid.Parse("00000000-0000-0000-0000-000000000002");
            Guid ModelEntityManagerID = Guid.Parse("00000000-0000-0000-0000-000000000003");
            Guid TextureManagerID = Guid.Parse("00000000-0000-0000-0000-000000000004");
            Guid ColorationManagerID = Guid.Parse("00000000-0000-0000-0000-000000000005");
            Guid MaterialManagerID = Guid.Parse("00000000-0000-0000-0000-000000000006");
            Guid MeshManagerID = Guid.Parse("00000000-0000-0000-0000-000000000007");
            Guid LightEntityManagerID = Guid.Parse("00000000-0000-0000-0000-000000000008");
            Guid CameraManagerID = Guid.Parse("00000000-0000-0000-0000-000000000009");
            Guid TransformManagerID = Guid.Parse("00000000-0000-0000-0000-000000000010");
            Guid TextureTransformID = Guid.Parse("00000000-0000-0000-0000-000000000011");
            Guid MasterBeatManagerID = Guid.Parse("00000000-0000-0000-0000-000000000012");

            PrefabDataBase = prefabDataBase;
            
            PrefabFactory = new PrefabFactory(this);

            CompositionRepository = new PrefabRepository<Composition>(prefabDataBase);
            CompositionManager = new PrefabManager<Composition>(CompositionManagerID, PrefabFactory, CompositionRepository);

            LayerRepository = new PrefabRepository<Layer>(prefabDataBase);
            LayerManager = new PrefabManager<Layer>(LayerManagerID, PrefabFactory, LayerRepository);

            EntityRepository = new PrefabRepository<Entity>(prefabDataBase);
            ModelEntityManager = new PrefabManager<Entity>(ModelEntityManagerID, PrefabFactory, EntityRepository);

            ColorationRepository = new PrefabRepository<Coloration>(prefabDataBase);
            ColorationManager = new PrefabManager<Coloration>(ColorationManagerID, PrefabFactory, ColorationRepository);

            MaterialRepository = new PrefabRepository<Material>(prefabDataBase);
            MaterialManager = new PrefabManager<Material>(MaterialManagerID, PrefabFactory, MaterialRepository);

            MeshRepository = new PrefabRepository<Mesh>(prefabDataBase);
            MeshManager = new PrefabManager<Mesh>(MeshManagerID, PrefabFactory, MeshRepository);

            LightEntityRepository = new PrefabRepository<LightEntity>(prefabDataBase);
            LightEntityManager = new PrefabManager<LightEntity>(LightEntityManagerID, PrefabFactory, LightEntityRepository);

            CameraRepository = new PrefabRepository<Camera>(prefabDataBase);
            CameraManager = new PrefabManager<Camera>(CameraManagerID, PrefabFactory, CameraRepository);

            TransformRepository = new PrefabRepository<Transform>(prefabDataBase);
            TransformManager = new PrefabManager<Transform>(TransformManagerID, PrefabFactory, TransformRepository);

            MasterBeatRepository = new PrefabRepository<MasterBeat>(prefabDataBase);
            MasterBeatManager = new PrefabManager<MasterBeat>(MasterBeatManagerID, PrefabFactory, MasterBeatRepository);

            IsActive = true;
        }

        public CompositionService(IPrefabDataBase prefabDataBase, IDialogService dialogService) : this(prefabDataBase)
        {
            DialogService = dialogService;
        }


        public Guid ID { get; set; }
        public IDialogService DialogService { get; set; }
        public IPrefabDataBase PrefabDataBase { get; set; }
        public PrefabFactory PrefabFactory { get; set; }

 
        public IPrefab GetPrefab(Guid id)
        {
            return PrefabDataBase.Prefabs.FirstOrDefault(x => x.ID == id);
        }


        public PrefabRepository<Layer> LayerRepository { get; set; }
        public PrefabManager<Layer> LayerManager { get; set; }

        public PrefabRepository<Composition> CompositionRepository { get; set; }
        public PrefabManager<Composition> CompositionManager { get; set; }

        public PrefabRepository<MasterBeat> MasterBeatRepository { get; set; }
        public PrefabManager<MasterBeat> MasterBeatManager { get; set; }

        public PrefabRepository<Entity> EntityRepository { get; set; }
        public PrefabManager<Entity> ModelEntityManager { get; set; }

        public PrefabRepository<Material> MaterialRepository { get; set; }
        public PrefabManager<Material> MaterialManager { get; set; }

        public PrefabRepository<Texture> TextureRepository { get; set; }
        public PrefabManager<Texture> TextureManager { get; set; }

        public PrefabRepository<LightEntity> LightEntityRepository { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }

        public PrefabRepository<Coloration> ColorationRepository { get; set; }
        public PrefabManager<Coloration> ColorationManager { get; set; }

        public PrefabRepository<Mesh> MeshRepository { get; set; }
        public PrefabManager<Mesh> MeshManager { get; set; }

        public PrefabRepository<Camera> CameraRepository { get; set; }
        public PrefabManager<Camera> CameraManager { get; set; }

        public PrefabRepository<Transform> TransformRepository { get; set; }
        public PrefabManager<Transform> TransformManager { get; set; }
    }
}
