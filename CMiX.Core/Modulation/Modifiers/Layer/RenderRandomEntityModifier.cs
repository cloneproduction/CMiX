// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(Layer))]
    public partial class RenderRandomEntityModifier : Modifier
    {
        public RenderRandomEntityModifier(PrefabService prefabService,
                                          PrefabManager modulatorManager,
                                          GenericValue<EntityType> entityType,
                                          ModulatableFloat control)
            : base(prefabService, modulatorManager)
        {
            EntityType = entityType;
            control.Label = "Control";
            control.Value.Value = 1.0f;
            Bindables = new List<ModulatableFloat> { control };
        }

        public ModulatableFloat Control => Bindables[0];

        public GenericValue<EntityType> EntityType { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RenderRandomEntityModifierModel
            {
                EntityType = (GenericValueModel<EntityType>)EntityType.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RenderRandomEntityModifierModel)model;
            LoadBaseModel(m);
            EntityType.FromModel(m.EntityType);
        }
    }
}
