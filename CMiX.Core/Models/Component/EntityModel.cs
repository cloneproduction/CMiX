// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using System;
using System.Collections.ObjectModel;

namespace CMiX.Core.Models
{
    public class EntityModel : IComponentModel, IPrefabModel
    {
        public EntityModel()
        {
            ID = Guid.NewGuid();

            GeometryModel = new GeometryModel();
            TextureModel = new TextureModel();
            ColorationModel = new TextureModel();

            VisibilityModel = new VisibilityModel();
            TransformSRT = new TransformSRTModel();
            TransformModifier = new ModifierManagerModel();
            Mesh = new MeshModel();
            MaterialManager = new PrefabManagerModel();
        }

        public EntityModel(Guid id) : this ()
        {
            ID = id;
        }

        public bool Enabled { get; set; }
        public string Name { get; set; }
        public Guid ID { get; set; }

        public GeometryModel GeometryModel { get; set; }
        public TextureModel TextureModel { get; set; }
        public TextureModel ColorationModel { get; set; }
        public VisibilityModel VisibilityModel { get; set; }

        public string Address { get; set; }
        public bool IsVisible { get; set; }

        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public TransformSRTModel TransformSRT { get; internal set; }
        public MeshModel Mesh { get; internal set; }
        public ModifierManagerModel TransformModifier { get; internal set; }
        public PrefabManagerModel MaterialManager { get; internal set; }
    }
}
