// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Presentation.ViewModels.Services
{
    public class CompositionServiceModel : IModel
    {
        public CompositionServiceModel()
        {
            ID = Guid.NewGuid();

            MaterialManager = new PrefabManagerModel();
            TextureManager = new PrefabManagerModel();
            MeshEntityManager = new PrefabManagerModel();
            LightEntityManager = new PrefabManagerModel();
            CameraManager = new PrefabManagerModel();
            TransformManager = new PrefabManagerModel();
            TextureTransformManager = new PrefabManagerModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public PrefabManagerModel MaterialManager { get; set; }
        public PrefabManagerModel TextureManager { get; internal set; }
        public PrefabManagerModel MeshEntityManager { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
        public PrefabManagerModel CameraManager { get; internal set; }
        public PrefabManagerModel ColorationManager { get; internal set; }
        public PrefabManagerModel EntityManager { get; internal set; }
        public PrefabManagerModel TransformManager { get; set; }
        public PrefabManagerModel TextureTransformManager { get; internal set; }
    }
}
