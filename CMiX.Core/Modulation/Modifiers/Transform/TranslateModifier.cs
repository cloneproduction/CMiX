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
    // Discoverable on both Entity and LightEntity, matching RandomPosition's own scope exactly -
    // both stay addable side by side until RandomPosition is confirmed superseded by a live VL
    // check, per this session's migration approach. RandomPosition is not touched by this change.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class TranslateModifier : Modifier, ISpreadableModifier
    {
        public TranslateModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ModifierModeSelector modifierModeSelector,
                                ModulatableFloat channelX,
                                ModulatableFloat channelY,
                                ModulatableFloat channelZ)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            channelX.Label = "X";
            channelY.Label = "Y";
            channelZ.Label = "Z";
            Channels = new List<ModulatableFloat> { channelX, channelY, channelZ };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableVectorXYZ binding -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public ModulatableFloat X => Channels[0];
        public ModulatableFloat Y => Channels[1];
        public ModulatableFloat Z => Channels[2];

        // Non-modulatable, ported as-is from RandomPosition for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }

        // Reaches ModifierModeSelector's own bindable Count for unassign-on-delete/resolve-on-load -
        // see Modifier.AdditionalModulatorBindables.
        protected override IEnumerable<IModulatorBindable> AdditionalModulatorBindables => new IModulatorBindable[] { ModifierModeSelector.Count };

        public override IControlModel ToModel()
        {
            var model = new TranslateModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TranslateModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveModulatorBinding(ModifierModeSelector.Count);
        }
    }
}
