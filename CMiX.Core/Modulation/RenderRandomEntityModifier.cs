// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Discoverable on Layer, matching the old RenderRandomEntity's own scope exactly - both stay
    // addable side by side until RenderRandomEntity is confirmed superseded by a live VL check,
    // per this session's migration approach. RenderRandomEntity is not touched by this change.
    // "Random" here names a selection strategy (as opposed to RenderSequenceEntityModifier's
    // sequential strategy), not a legacy reroll-on-beat mechanism, so it is kept in the name
    // rather than dropped like RandomScale/RandomPosition/etc. EntityType is ported as-is
    // (non-modulatable); Control becomes a modulatable channel.
    [ModifierPanel(typeof(Layer))]
    public partial class RenderRandomEntityModifier : Modifier
    {
        public RenderRandomEntityModifier(PrefabService prefabService,
                                          PrefabManager modulatorManager,
                                          GenericValue<EntityType> entityType,
                                          Modulatable control)
            : base(prefabService, modulatorManager)
        {
            EntityType = entityType;
            control.Label = "Control";
            // Matches the old RenderRandomEntity's own default - a freshly added modifier
            // otherwise starts at 0, disabling it.
            control.Value.Value = 1.0f;
            Channels = new List<Modulatable> { control };
        }

        // Convenience accessor into Channels, purely for the view's ModulatableSlider binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable Control => Channels[0];

        // Non-modulatable, ported as-is from RenderRandomEntity for one-to-one field parity.
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
