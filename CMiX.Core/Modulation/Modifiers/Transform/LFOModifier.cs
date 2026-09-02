// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Discoverable on Entity, matching the old LFO's own scope exactly - both stay addable side
    // by side until LFO is confirmed superseded by a live VL check, per this session's migration
    // approach. LFO is not touched by this change. ModifierModeSelector, PingPong, DirectionXYZ,
    // and TransformType are ported as-is (non-modulatable); RandomizePhase, From, and To all
    // become modulatable channels.
    [ModifierPanel(typeof(Entity))]
    public partial class LFOModifier : Modifier, ISpreadableModifier
    {
        public LFOModifier(PrefabService prefabService,
                           PrefabManager modulatorManager,
                           ModifierModeSelector modifierModeSelector,
                           GenericValue<bool> pingPong,
                           DirectionXYZ directionXYZ,
                           GenericValue<TransformType> transformType,
                           Modulatable from,
                           Modulatable to,
                           Modulatable randomizePhase)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            PingPong = pingPong;
            DirectionXYZ = directionXYZ;
            TransformType = transformType;
            from.Label = "From";
            to.Label = "To";
            randomizePhase.Label = "Randomize Phase";
            // Matches the old LFO's own default (From 0, To 1) - a freshly added LFO otherwise
            // oscillates 0 to 0, a no-op.
            to.Value.Value = 1.0f;
            Channels = new List<Modulatable> { from, to, randomizePhase };
        }

        // Convenience accessors into Channels, purely for the view's ModulatableValue bindings -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public Modulatable From => Channels[0];
        public Modulatable To => Channels[1];
        public Modulatable RandomizePhase => Channels[2];

        // Non-modulatable, ported as-is from LFO for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<bool> PingPong { get; set; }
        public DirectionXYZ DirectionXYZ { get; set; }
        public GenericValue<TransformType> TransformType { get; set; }

        // Reaches ModifierModeSelector's own bindable Count for unassign-on-delete/resolve-on-load -
        // see Modifier.AdditionalModulatorBindables.
        protected override IEnumerable<IModulatorBindable> AdditionalModulatorBindables => new IModulatorBindable[] { ModifierModeSelector.Count };

        public override IControlModel ToModel()
        {
            var model = new LFOModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                PingPong = (GenericValueModel<bool>)PingPong.ToModel(),
                DirectionXYZ = (DirectionXYZModel)DirectionXYZ.ToModel(),
                TransformType = (GenericValueModel<TransformType>)TransformType.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (LFOModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveModulatorBinding(ModifierModeSelector.Count);
            PingPong.FromModel(m.PingPong);
            DirectionXYZ.FromModel(m.DirectionXYZ);
            TransformType.FromModel(m.TransformType);
        }
    }
}
