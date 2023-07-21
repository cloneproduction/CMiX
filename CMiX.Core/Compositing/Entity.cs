// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Services;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableRecipient, IPrefab, IModifiable
    {
        public Entity(CompositionService compositionService)
        {
            Name = new StringValue();
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();
            Mesh = new Mesh();
            MaterialSelector = new PrefabSelector<Material>(compositionService);
            ModifierManager = new ModifierManager(new EntityModifierFactory());
            Visibility = new BooleanValue();
            BaseColor = new ColorSelector(Color.FromArgb(255, 255, 255, 255));

            IsActive = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public PrefabSelector<Material> MaterialSelector { get; set; }
        public ColorSelector BaseColor { get; set; }
    }
}
