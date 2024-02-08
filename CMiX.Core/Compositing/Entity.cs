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
    public partial class Entity : IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, Mesh mesh, Material material, ReorderablePrefabManager modifierManager)
        {
            ID = prefabService.ID;

            Name = prefabService.Name;
            Visibility = prefabService.Visibility;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;

            Mesh = mesh;
            Material = material;

            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }
        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> Visibility { get; set; }
        public ReorderablePrefabManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
    }
}
