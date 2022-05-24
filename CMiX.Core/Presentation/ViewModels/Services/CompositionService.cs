// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Presentation.ViewModels.Service
{
    public class CompositionService : IService, IControl, IBeatable
    {
        public CompositionService(CompositionServiceModel compositionServiceModel, MasterBeat masterBeat)
        {
            ID = compositionServiceModel.ID;

            TextureManager = new TextureManager(new PrefabManagerModel());
            ColorationManager = new ColorationManager(new PrefabManagerModel());
            MaterialManager = new MaterialManager(new PrefabManagerModel());

            ModelEntityManager = new ModelEntityManager(new PrefabManagerModel());
            MeshManager = new MeshManager(new PrefabManagerModel());
            LightEntityManager = new LightEntityManager(new PrefabManagerModel());
            CameraManager = new CameraManager(new PrefabManagerModel());

            this.SetMasterBeat(masterBeat);
        }


        public Guid ID { get; set; }

        public MaterialManager MaterialManager { get; set; }
        public TextureManager TextureManager { get; set; }
        public MeshManager MeshManager { get; set; }
        public LightEntityManager LightEntityManager { get; set; }
        public CameraManager CameraManager { get; set; }
        public ColorationManager ColorationManager { get; set; }
        public ModelEntityManager ModelEntityManager { get; set; }


        public IModel GetModel()
        {
            CompositionServiceModel compositionServiceModel = new CompositionServiceModel();

            compositionServiceModel.ID = ID;

            compositionServiceModel.MaterialManager = (PrefabManagerModel)MaterialManager.GetModel();
            compositionServiceModel.TextureManager = (PrefabManagerModel)TextureManager.GetModel();
            compositionServiceModel.MeshEntityManager = (PrefabManagerModel)MeshManager.GetModel();
            compositionServiceModel.LightEntityManager = (PrefabManagerModel)LightEntityManager.GetModel();
            compositionServiceModel.CameraManager = (PrefabManagerModel)CameraManager.GetModel();
            compositionServiceModel.ColorationManager = (PrefabManagerModel)ColorationManager.GetModel();
            compositionServiceModel.EntityManager = (PrefabManagerModel)ModelEntityManager.GetModel();

            return compositionServiceModel;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            MaterialManager.SetMasterBeat(masterBeat);
            TextureManager.SetMasterBeat(masterBeat);
            MeshManager.SetMasterBeat(masterBeat);
            LightEntityManager.SetMasterBeat(masterBeat);
            ColorationManager.SetMasterBeat(masterBeat);
            CameraManager.SetMasterBeat(masterBeat);
            ModelEntityManager.SetMasterBeat(masterBeat);
        }

        public void SetViewModel(IModel model)
        {
            CompositionServiceModel compositionServiceModel = model as CompositionServiceModel;

            this.ID = compositionServiceModel.ID;

            this.MaterialManager.SetViewModel(compositionServiceModel.MaterialManager);
            this.TextureManager.SetViewModel(compositionServiceModel.TextureManager);
            this.MeshManager.SetViewModel(compositionServiceModel.MeshEntityManager);
            this.LightEntityManager.SetViewModel(compositionServiceModel.LightEntityManager);
            this.CameraManager.SetViewModel(compositionServiceModel.CameraManager);
            this.ColorationManager.SetViewModel(compositionServiceModel.ColorationManager);
            this.ModelEntityManager.SetViewModel(compositionServiceModel.EntityManager);
        }
    }
}
