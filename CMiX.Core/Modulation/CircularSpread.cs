// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;

namespace CMiX.Core.Modulation
{
    // Renamed from CircularSpreadModifier to bare CircularSpread, claiming the name freed up by
    // the old CircularSpread (now CircularSpreadLegacy) so VL's exact-name matching can target
    // this class directly. One-to-one field parity with the old CircularSpread is complete:
    // ModifierModeSelector is ported as-is (non-modulatable); Width (an XY pair), Phase, and
    // Factor become modulatable channels - new capability the old CircularSpread never had.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class CircularSpread : Modifier, IChannelGroupXY
    {
        public CircularSpread(PrefabService prefabService,
                                      PrefabManager modulatorManager,
                                      ModifierModeSelector modifierModeSelector,
                                      Channel widthX,
                                      Channel widthY,
                                      Channel phase,
                                      Channel factor)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            widthX.Label = "X";
            widthY.Label = "Y";
            phase.Label = "Phase";
            factor.Label = "Factor";
            Channels = new List<Channel> { widthX, widthY, phase, factor };
        }

        // Convenience accessors into Channels, purely for the view's bindings - Channels itself
        // stays the source of truth (used by Modifier's own ToModel/FromModel). X/Y (the Width
        // pair) satisfy IChannelGroupXY for ChannelVectorXY; Phase/Factor are each bound directly
        // by their own separate ChannelValue.
        public Channel X => Channels[0];
        public Channel Y => Channels[1];
        public Channel Phase => Channels[2];
        public Channel Factor => Channels[3];

        // Non-modulatable, ported as-is from CircularSpread for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CircularSpreadModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CircularSpreadModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
