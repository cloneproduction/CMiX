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
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class CircularSpreadModifier : Modifier, ISpreadableModifier
    {
        public CircularSpreadModifier(PrefabService prefabService,
                                      PrefabManager modulatorManager,
                                      ModifierModeSelector modifierModeSelector,
                                      ModulatableFloat widthX,
                                      ModulatableFloat widthY,
                                      ModulatableFloat phase,
                                      ModulatableFloat factor)
            : base(prefabService, modulatorManager, modifierModeSelector.Bindables)
        {
            ModifierModeSelector = modifierModeSelector;
            widthX.Label = "X";
            widthY.Label = "Y";
            phase.Label = "Phase";
            factor.Label = "Factor";
            widthX.Value.Value = 1.0f;
            widthY.Value.Value = 1.0f;
            factor.Value.Value = 1.0f;
            Bindables = new List<ModulatableFloat> { widthX, widthY, phase, factor };
        }

        public ModulatableFloat X => Bindables[0];
        public ModulatableFloat Y => Bindables[1];
        public ModulatableFloat Phase => Bindables[2];
        public ModulatableFloat Factor => Bindables[3];

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
            ResolveNestedBindables();
        }
    }
}
