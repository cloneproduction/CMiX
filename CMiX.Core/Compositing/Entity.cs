// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : IControl, IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, 
                      Mesh mesh, 
                      Material material, 
                      ReorderablePrefabManager modifierManager
                     )
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            Material = material;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManagerBase ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }


    }
}
