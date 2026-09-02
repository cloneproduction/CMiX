// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Discoverable on Entity, matching the old RandomTexCoord's own scope exactly - both stay
    // addable side by side until RandomTexCoord is confirmed superseded by a live VL check, per
    // this session's migration approach. RandomTexCoord is not touched by this change.
    // ModifierModeSelector and SamplerState are ported as-is (non-modulatable); Location, Scale,
    // Rotation, and Uniform all become modulatable channels.
    [ModifierPanel(typeof(Entity))]
    public partial class TexCoordModifier : Modifier, ISpreadableModifier
    {
        public TexCoordModifier(PrefabService prefabService,
                                PrefabManager modulatorManager,
                                ModifierModeSelector modifierModeSelector,
                                SamplerState samplerState,
                                ModulatableFloat locationX, ModulatableFloat locationY,
                                ModulatableFloat scaleX, ModulatableFloat scaleY,
                                ModulatableFloat rotation,
                                ModulatableFloat uniform)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;
            SamplerState = samplerState;
            Location = new ModulatableVector2(locationX, locationY);
            Scale = new ModulatableVector2(scaleX, scaleY);
            rotation.Label = "Rotation";
            uniform.Label = "Uniform";
            Channels = new List<ModulatableFloat> { locationX, locationY, scaleX, scaleY, rotation, uniform };
        }

        // Each group is bound by its own ModulatableVectorXY in the view (via DataContext), all
        // sharing this Modifier's single ModulatorManager (set explicitly on each usage, not
        // inherited from DataContext) - see ModulatableVectorXY.axaml.cs.
        public ModulatableVector2 Location { get; }
        public ModulatableVector2 Scale { get; }

        // Convenience accessors into Channels, purely for the view's ModulatableFloatValue bindings -
        // Channels itself stays the source of truth (used by Modifier's own ToModel/FromModel).
        public ModulatableFloat Rotation => Channels[4];
        public ModulatableFloat Uniform => Channels[5];

        // Non-modulatable, ported as-is from RandomTexCoord for one-to-one field parity.
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public SamplerState SamplerState { get; set; }

        // Reaches ModifierModeSelector's own bindable Count for unassign-on-delete/resolve-on-load -
        // see Modifier.AdditionalModulatorBindables.
        protected override IEnumerable<IModulatorBindable> AdditionalModulatorBindables => new IModulatorBindable[] { ModifierModeSelector.Count };

        public override IControlModel ToModel()
        {
            var model = new TexCoordModifierModel
            {
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
                SamplerState = (SamplerStateModel)SamplerState.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TexCoordModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveModulatorBinding(ModifierModeSelector.Count);
            SamplerState.FromModel(m.SamplerState);
        }
    }
}
