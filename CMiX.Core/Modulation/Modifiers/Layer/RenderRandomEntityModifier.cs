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
                                          ControlRepository controlRepository,
                                          GenericValue<EntityType> entityType,
                                          ModulatableValue<float> control)
            : base(prefabService, modulatorManager, controlRepository)
        {
            EntityType = entityType;
            Bindables = new List<ModulatableValue<float>> { control };
        }

        public ModulatableValue<float> Control => Bindables[0];

        public GenericValue<EntityType> EntityType { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RenderRandomEntityModifierModel
            {
                EntityType = (GenericValueModel<EntityType>)EntityType.ToModel(),
                Control = (ModulatableValueModel<float>)Control.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RenderRandomEntityModifierModel)model;
            LoadBaseModel(m);
            Control.FromModel(m.Control);
            EntityType.FromModel(m.EntityType);
        }
    }
}
