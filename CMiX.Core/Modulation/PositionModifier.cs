// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation
{
    // Discoverable on both Entity and LightEntity, matching RandomPosition's own scope exactly -
    // both stay addable side by side until RandomPosition is confirmed superseded by a live VL
    // check, per this session's migration approach. RandomPosition is not touched by this change.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class PositionModifier : Modifier, ISpreadableModifier
    {
        public PositionModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ModifierModeSelector modifierModeSelector,
                                Modulatable channelX,
                                Modulatable channelY,
                                Modulatable channelZ)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            channelX.Label = "X";
            channelY.Label = "Y";
            channelZ.Label = "Z";
            Channels = new List<Modulatable> { channelX, channelY, channelZ };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableVectorXYZ binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable X => Channels[0];
        public Modulatable Y => Channels[1];
        public Modulatable Z => Channels[2];

        // Non-modulatable, ported as-is from RandomPosition for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new PositionModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (PositionModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
