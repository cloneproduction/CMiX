// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Renamed from CircularSpreadModifier to bare CircularSpread, claiming the name freed up by
    // the old CircularSpread (now CircularSpreadLegacy) so VL's exact-name matching can target
    // this class directly. One-to-one field parity with the old CircularSpread is complete:
    // ModifierModeSelector is ported as-is (non-modulatable); Width (an XY pair), Phase, and
    // Factor become modulatable channels - new capability the old CircularSpread never had.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class CircularSpreadModifier : Modifier, ISpreadableModifier
    {
        public CircularSpreadModifier(PrefabService prefabService,
                                      PrefabManager modulatorManager,
                                      ModifierModeSelector modifierModeSelector,
                                      Modulatable widthX,
                                      Modulatable widthY,
                                      Modulatable phase,
                                      Modulatable factor)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            widthX.Label = "X";
            widthY.Label = "Y";
            phase.Label = "Phase";
            factor.Label = "Factor";
            // Matches the old CircularSpread's own defaults (Width (1,1), Factor 1) - a freshly
            // added modifier otherwise starts with zero spread and zero effect.
            widthX.Value.Value = 1.0f;
            widthY.Value.Value = 1.0f;
            factor.Value.Value = 1.0f;
            Channels = new List<Modulatable> { widthX, widthY, phase, factor };
        }

        // Convenience accessors into Channels, purely for the view's bindings - Channels itself
        // stays the source of truth (used by Modifier's own ToModel/FromModel). X/Y (the Width
        // pair) are read directly by ModulatableVectorXY; Phase/Factor are each bound directly
        // by their own separate ModulatableValue.
        public Modulatable X => Channels[0];
        public Modulatable Y => Channels[1];
        public Modulatable Phase => Channels[2];
        public Modulatable Factor => Channels[3];

        // Non-modulatable, ported as-is from CircularSpread for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new CircularSpreadModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (CircularSpreadModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
