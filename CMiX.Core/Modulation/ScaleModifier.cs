// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Modulation
{
    // Discoverable in a real Entity's own "Add Modifier" picker (EntityModifiersSection's
    // AutoSelectionPanel, PanelOwner-based - see Modifier.cs's comment on IModifier for how this
    // differs from VL-side discovery) alongside the old RandomScale, which this is meant to
    // eventually replace. Both stay addable side by side until RandomScale is confirmed
    // superseded by a live VL check, per this session's migration approach - RandomScale is not
    // touched by this change at all.
    [ModifierPanel(typeof(Entity))]
    public partial class ScaleModifier : Modifier, IChannelGroup
    {
        public ScaleModifier(PrefabService prefabService,
                             PrefabManager modulatorManager,
                             ModifierModeSelector modifierModeSelector,
                             Channel channelX,
                             Channel channelY,
                             Channel channelZ,
                             Channel channelUniform)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            channelX.Label = "X";
            channelY.Label = "Y";
            channelZ.Label = "Z";
            channelUniform.Label = "Uniform";
            Channels = new List<Channel> { channelX, channelY, channelZ, channelUniform };
        }

        // Convenience accessors into Channels, purely for the view's ChannelVectorXYZ binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Channel X => Channels[0];
        public Channel Y => Channels[1];
        public Channel Z => Channels[2];

        // Uniform is a 4th, independently modulatable channel - ported from RandomScale's
        // UniformXYZ, which was already the same GenericValue<float> type as X/Y/Z.
        public Channel Uniform => Channels[3];

        // Non-modulatable, ported as-is from RandomScale for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ScaleModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ScaleModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
