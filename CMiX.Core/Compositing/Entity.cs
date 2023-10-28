// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Collections;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableRecipient, IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, Mesh mesh, Material material, ICollectionManager modifierManager)
        {
            ID = prefabService.ID;
            Name = prefabService.Name;
            Visibility = prefabService.Visibility;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;

            Mesh = mesh;
            Material = material;

            ModifierManager = modifierManager;
            IsActive = true;
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }
        public ICollectionManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
    }
}
