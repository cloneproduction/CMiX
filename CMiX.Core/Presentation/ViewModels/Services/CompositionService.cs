// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Components;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Presentation.ViewModels.Service
{
    public class CompositionService : IService, IControl, IBeatable
    {
        public CompositionService(CompositionServiceModel compositionServiceModel, MasterBeat masterBeat)
        {
            ID = compositionServiceModel.ID;

            TextureManager = new PrefabManager<Texture>(new PrefabManagerModel());
            ColorationManager = new PrefabManager<Coloration>(new PrefabManagerModel());
            MaterialManager = new PrefabManager<Material>(new PrefabManagerModel());
            ModelEntityManager = new PrefabManager<Entity>(new PrefabManagerModel());
            MeshManager = new PrefabManager<Mesh>(new PrefabManagerModel());
            LightEntityManager = new PrefabManager<LightEntity>(new PrefabManagerModel());
            CameraManager = new PrefabManager<Camera>(new PrefabManagerModel());
            TransformManager = new PrefabManager<Transform>(new PrefabManagerModel());
            TextureTransformManager = new PrefabManager<Transform>(new PrefabManagerModel());

            this.SetMasterBeat(masterBeat);
        }


        public Guid ID { get; set; }

        public PrefabManager<Material> MaterialManager { get; set; }
        public PrefabManager<Texture> TextureManager { get; set; }
        public PrefabManager<Mesh> MeshManager { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }
        public PrefabManager<Camera> CameraManager { get; set; }
        public PrefabManager<Coloration> ColorationManager { get; set; }
        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public PrefabManager<Transform> TransformManager { get; set; }
        public PrefabManager<Transform> TextureTransformManager { get; set; }


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
            compositionServiceModel.TransformManager = (PrefabManagerModel)TransformManager.GetModel();
            compositionServiceModel.TextureTransformManager = (PrefabManagerModel)TextureTransformManager.GetModel();

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
            TransformManager.SetMasterBeat(masterBeat);
            TextureTransformManager.SetMasterBeat(masterBeat);
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
            this.TransformManager.SetViewModel(compositionServiceModel.TransformManager);
            this.TextureTransformManager.SetViewModel(compositionServiceModel.TextureTransformManager);
        }
    }
}
