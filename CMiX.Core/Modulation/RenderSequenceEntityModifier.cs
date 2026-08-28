// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Discoverable on Layer, matching the old RenderSequenceEntity's own scope exactly - both
    // stay addable side by side until RenderSequenceEntity is confirmed superseded by a live VL
    // check, per this session's migration approach. RenderSequenceEntity is not touched by this
    // change. Structurally identical to RenderRandomEntityModifier - "Sequence" here names the
    // selection strategy, not a legacy reroll-on-beat mechanism. EntityType is ported as-is
    // (non-modulatable); Control becomes a modulatable channel.
    [ModifierPanel(typeof(Layer))]
    public partial class RenderSequenceEntityModifier : Modifier
    {
        public RenderSequenceEntityModifier(PrefabService prefabService,
                                            PrefabManager modulatorManager,
                                            GenericValue<EntityType> entityType,
                                            Channel control)
            : base(prefabService, modulatorManager)
        {
            EntityType = entityType;
            control.Label = "Control";
            Channels = new List<Channel> { control };
        }

        // Convenience accessor into Channels, purely for the view's ChannelSlider binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Channel Control => Channels[0];

        // Non-modulatable, ported as-is from RenderSequenceEntity for one-to-one field parity.
        public GenericValue<EntityType> EntityType { get; set; }

        public override IControlModel ToModel()
        {
            var model = new RenderSequenceEntityModifierModel
            {
                EntityType = (GenericValueModel<EntityType>)EntityType.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RenderSequenceEntityModifierModel)model;
            LoadBaseModel(m);
            EntityType.FromModel(m.EntityType);
        }
    }
}
