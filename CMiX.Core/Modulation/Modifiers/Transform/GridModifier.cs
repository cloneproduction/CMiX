// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Modulation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class GridModifier : Modifier, ISpreadableModifier3
    {
        public GridModifier(PrefabService prefabService,
                            PrefabManager modulatorManager,
                            ModifierModeSelector3 modifierModeSelector,
                            ModulatableFloat widthX, ModulatableFloat widthY, ModulatableFloat widthZ,
                            ModulatableFloat phaseX, ModulatableFloat phaseY, ModulatableFloat phaseZ)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;

            Width = new ModulatableVector3(widthX, widthY, widthZ);
            Phase = new ModulatableVector3(phaseX, phaseY, phaseZ);
            Bindables = new List<ModulatableFloat> { widthX, widthY, widthZ, phaseX, phaseY, phaseZ };
        }

        public ModulatableVector3 Width { get; }
        public ModulatableVector3 Phase { get; }
        public ModifierModeSelector3 ModifierModeSelector { get; set; }

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
            ResolveNestedBindables();
        }
    }
}
