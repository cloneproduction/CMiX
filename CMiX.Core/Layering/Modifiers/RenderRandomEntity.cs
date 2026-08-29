// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Layering.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.RenderRandomEntityModifier. No longer
    // addable via the picker; kept so already-saved Project data referencing it still loads.
    public partial class RenderRandomEntity : BeatModifiableModifierBase, IModifier
    {
        public RenderRandomEntity(PrefabManager beatModifierManager,
                                  PrefabService prefabService,
                                  GenericValue<EntityType> entityType,
                                  GenericValue<float> control)
            : base(prefabService, beatModifierManager)
        {
            EntityType = entityType;
            Control = control;
        }

        public GenericValue<EntityType> EntityType { get; set; }
        public GenericValue<float> Control { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RenderRandomEntityModel
            {
                EntityType = (GenericValueModel<EntityType>)EntityType.ToModel(),
                Control = (GenericValueModel<float>)Control.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RenderRandomEntityModel)model;
            LoadBaseModel(m);
            EntityType.FromModel(m.EntityType);
            Control.FromModel(m.Control);
        }
    }
}
