// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class EntityModel : IControlModel, IPrefabModel
    {
        public EntityModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;

            ModifierManager = new PrefabManagerModel();
            Mesh = new MeshModel();
            Name = new GenericValueModel<string>("Entity " + ID.ToString());
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            Visibility = new GenericValueModel<bool>(false);

            Material = new MaterialModel();
        }

        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public MeshModel Mesh { get; set; }
        public MaterialModel Material { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
    }
}
