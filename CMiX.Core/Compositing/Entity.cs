// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public partial class Entity : IControl, IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, 
                      Mesh mesh, 
                      Material material,
                      PrefabManager materialManager,
                      PrefabManager modifierManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            Material = material;
            MaterialManager = materialManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public PrefabManager MaterialManager { get; set; }

        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
    }
}
