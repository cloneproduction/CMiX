// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    // Renamed from GridModifier to bare Grid, claiming the name freed up by the old Grid (now
    // GridLegacy) so VL's exact-name matching can target this class directly. One-to-one field
    // parity with the old Grid is complete: Count and ModifierModeSelector are ported as-is
    // (non-modulatable); Width and Phase become two independent, modulatable XYZ groups sharing
    // this Modifier's one modulator stack - new capability the old Grid never had.
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class GridModifier : Modifier, ISpreadableModifier3
    {
        public GridModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ModifierModeSelector3 modifierModeSelector,
                            ModulatableFloat widthX, ModulatableFloat widthY, ModulatableFloat widthZ,
                            ModulatableFloat phaseX, ModulatableFloat phaseY, ModulatableFloat phaseZ)
            : base(prefabService, modulatorManager)
        {
            ModifierModeSelector = modifierModeSelector;

            Width = new ModulatableVector3(widthX, widthY, widthZ);
            Phase = new ModulatableVector3(phaseX, phaseY, phaseZ);
            Channels = new List<ModulatableFloat> { widthX, widthY, widthZ, phaseX, phaseY, phaseZ };
        }

        // Each group is bound by its own ModulatableVectorXYZ in the view (via DataContext), both
        // sharing this Modifier's single ModulatorManager (set explicitly on each usage, not
        // inherited from DataContext) - see ModulatableVectorXYZ.axaml.cs.
        public ModulatableVector3 Width { get; }
        public ModulatableVector3 Phase { get; }
        public ModifierModeSelector3 ModifierModeSelector { get; set; }

        // Reaches ModifierModeSelector's own bindable CountX/Y/Z for unassign-on-delete/
        // resolve-on-load - see Modifier.AdditionalModulatorBindables.
        protected override IEnumerable<IModulatorBindable> AdditionalModulatorBindables =>
            new IModulatorBindable[] { ModifierModeSelector.CountX, ModifierModeSelector.CountY, ModifierModeSelector.CountZ };

        public override IControlModel ToModel()
        {
            var model = new GridModifierModel
            {
                ModifierModeSelector = (ModifierModeSelector3Model)ModifierModeSelector.ToModel(),
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (GridModifierModel)model;
            LoadBaseModel(m);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
            ResolveModulatorBinding(ModifierModeSelector.CountX);
            ResolveModulatorBinding(ModifierModeSelector.CountY);
            ResolveModulatorBinding(ModifierModeSelector.CountZ);
        }
    }
}
