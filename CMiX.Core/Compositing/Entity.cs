// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Services;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableRecipient, IPrefab, IModifiable
    {
        public Entity(CompositionService compositionService)
        {
            Name = new StringValue();
            Visibility = new BooleanValue();
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();

            Mesh = new Mesh(compositionService);
            Material = new Material(compositionService);

            ModifierManager = compositionService.GetModifierManager<Entity>();

            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }
    }
}
