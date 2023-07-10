// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Services;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableRecipient, IPrefab, IModifiable
    {
        public Entity(EntityModel entityModel, CompositionService compositionService)
        {
            ID = entityModel.ID;
            Name = new StringValue(entityModel.Name);
            IsSelected = new BooleanValue(entityModel.IsSelected);
            IsRenaming = new BooleanValue(entityModel.IsRenaming);
            TransformSRT = new TransformSRT(entityModel.TransformSRT);
            Mesh = new Mesh(entityModel.Mesh);
            MaterialSelector = new PrefabSelector<Material>(entityModel.MaterialSelector, compositionService, compositionService.MaterialRepository);
            ModifierManager = new ModifierManager(entityModel.ModifierManager, new ModifierFactory());
            Visibility = new BooleanValue(entityModel.Visibility);
            IsActive = true;

            BaseColor = new ColorSelector(entityModel.BaseColor);
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public Mesh Mesh { get; set; }
        public PrefabSelector<Material> MaterialSelector { get; set; }
        public ColorSelector BaseColor { get; set; }
    }
}
